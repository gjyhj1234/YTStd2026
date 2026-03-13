using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using YTStdLogger.Logging;

namespace YTStdLogger.IO;

/// <summary>
/// 批量文件写入器。
/// 线程模型：仅消费者线程调用写入方法，内部仍使用最小锁以保护状态切换与释放。
/// </summary>
public sealed class BatchedFileWriter : IDisposable
{
    private sealed class WriterState
    {
        public string FilePath = string.Empty;
        public FileStream? Stream;
        public DateTime Day;
    }

    private readonly object _gate = new object();
    private readonly Dictionary<string, WriterState> _writers = new Dictionary<string, WriterState>(256);
    private readonly TenantDatePathResolver _resolver;
    private readonly ILogFormatter _formatter;
    private readonly bool _flushEveryBatch;

    /// <summary>
    /// 初始化写入器。
    /// </summary>
    public BatchedFileWriter(TenantDatePathResolver resolver, ILogFormatter formatter, bool flushEveryBatch)
    {
        _resolver = resolver;
        _formatter = formatter;
        _flushEveryBatch = flushEveryBatch;
    }

    /// <summary>
    /// 批量写入消息。
    /// 一条日志会写入其等级及更详细等级文件（如 Error 同时进入 error/infor/debug）。
    /// </summary>
    public void WriteBatch(ReadOnlySpan<LogMessage> messages)
    {
        char[] lineBuffer = ArrayPool<char>.Shared.Rent(4096);
        byte[] byteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        try
        {
            for (int i = 0; i < messages.Length; i++)
            {
                LogMessage msg = messages[i];
                WriteMessageToTargets(msg, lineBuffer, ref byteBuffer);
            }

            if (_flushEveryBatch)
            {
                FlushAll();
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(lineBuffer);
            ArrayPool<byte>.Shared.Return(byteBuffer);
        }
    }

    /// <summary>
    /// 写入内部告警到 warn 文件。
    /// </summary>
    public void WriteInternalWarning(DateTime ts, string message)
    {
        char[] messageBuffer = ArrayPool<char>.Shared.Rent(message.Length == 0 ? 1 : message.Length);
        char[] lineBuffer = ArrayPool<char>.Shared.Rent(4096);
        byte[] byteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        LogMessage temp = new LogMessage();
        temp.Timestamp = ts;
        temp.Level = LogLevel.Warn;
        temp.TenantId = 0;
        temp.ThreadId = 0;
        message.AsSpan().CopyTo(messageBuffer);
        temp.MessageBuffer = messageBuffer;
        temp.MessageLength = message.Length;

        try
        {
            WriteMessageToTargets(temp, lineBuffer, ref byteBuffer);
            if (_flushEveryBatch)
            {
                FlushAll();
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(messageBuffer);
            ArrayPool<char>.Shared.Return(lineBuffer);
            ArrayPool<byte>.Shared.Return(byteBuffer);
        }
    }

    private void WriteMessageToTargets(LogMessage msg, char[] lineBuffer, ref byte[] byteBuffer)
    {
        int len = _formatter.Format(msg, lineBuffer);
        if (len <= 0)
        {
            return;
        }

        ReadOnlySpan<char> line = new ReadOnlySpan<char>(lineBuffer, 0, len);
        for (int levelValue = (int)msg.Level; levelValue <= (int)LogLevel.Debug; levelValue++)
        {
            LogLevel target = (LogLevel)levelValue;
            WriteLine(msg.Timestamp, msg.TenantId, target, line, ref byteBuffer);
        }
    }

    private void WriteLine(DateTime ts, int tenantId, LogLevel level, ReadOnlySpan<char> line, ref byte[] byteBuffer)
    {
        WriterState state = GetOrCreateState(ts, tenantId, level);
        int needed = Encoding.UTF8.GetMaxByteCount(line.Length);
        EnsureByteBufferCapacity(ref byteBuffer, needed);

        int bytes = Encoding.UTF8.GetBytes(line, byteBuffer);
        state.Stream!.Write(byteBuffer, 0, bytes);
    }

    private static void EnsureByteBufferCapacity(ref byte[] byteBuffer, int needed)
    {
        if (needed <= byteBuffer.Length)
        {
            return;
        }

        byte[] old = byteBuffer;
        byteBuffer = ArrayPool<byte>.Shared.Rent(needed);
        ArrayPool<byte>.Shared.Return(old);
    }

    private WriterState GetOrCreateState(DateTime ts, int tenantId, LogLevel level)
    {
        DateTime day = new DateTime(ts.Year, ts.Month, ts.Day);
        // 使用 stackalloc + Span 构建字典键，避免多次 ToString + 字符串拼接
        Span<char> keyBuf = stackalloc char[32];
        int kp = 0;
        tenantId.TryFormat(keyBuf.Slice(kp), out int w1); kp += w1;
        keyBuf[kp++] = '|';
        day.Year.TryFormat(keyBuf.Slice(kp), out int w2); kp += w2;
        WriteFixed2(day.Month, keyBuf, ref kp);
        WriteFixed2(day.Day, keyBuf, ref kp);
        keyBuf[kp++] = '|';
        ((int)level).TryFormat(keyBuf.Slice(kp), out int w3); kp += w3;
        string key = keyBuf.Slice(0, kp).ToString();

        lock (_gate)
        {
            if (_writers.TryGetValue(key, out WriterState? state))
            {
                return state;
            }

            string filePath = _resolver.GetLogFilePath(ts, tenantId, level);
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, 64 * 1024, FileOptions.SequentialScan);
            state = new WriterState
            {
                FilePath = filePath,
                Stream = stream,
                Day = day
            };
            _writers[key] = state;
            return state;
        }
    }

    /// <summary>
    /// 刷新全部已打开写入流。
    /// </summary>
    public void FlushAll()
    {
        lock (_gate)
        {
            foreach (KeyValuePair<string, WriterState> pair in _writers)
            {
                pair.Value.Stream?.Flush(flushToDisk: false);
            }
        }
    }

    /// <summary>
    /// 写入两位固定宽度整数（前导零填充），用于日期格式化。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteFixed2(int value, Span<char> buf, ref int pos)
    {
        buf[pos++] = (char)('0' + ((value / 10) % 10));
        buf[pos++] = (char)('0' + (value % 10));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_gate)
        {
            foreach (KeyValuePair<string, WriterState> pair in _writers)
            {
                try
                {
                    pair.Value.Stream?.Flush(flushToDisk: false);
                    pair.Value.Stream?.Dispose();
                }
                catch
                {
                }
            }

            _writers.Clear();
        }
    }
}
