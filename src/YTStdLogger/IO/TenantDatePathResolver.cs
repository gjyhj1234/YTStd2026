using System;
using System.IO;
using System.Runtime.CompilerServices;
using YTStdLogger.Logging;

namespace YTStdLogger.IO;

/// <summary>
/// 负责将租户、日期、等级映射到目标目录与文件路径。
/// </summary>
public sealed class TenantDatePathResolver
{
    private readonly string _rootPath;

    /// <summary>
    /// 初始化路径解析器。
    /// </summary>
    public TenantDatePathResolver(string rootPath)
    {
        _rootPath = rootPath;
    }

    /// <summary>
    /// 获取租户日期目录。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetTenantDirectory(DateTime ts, int tenantId)
    {
        // 使用 stackalloc 构建 yyyyMM 和 yyyyMMdd，避免多次 ToString + 字符串拼接
        Span<char> month = stackalloc char[6];
        int mp = 0;
        WriteFixed4(ts.Year, month, ref mp);
        WriteFixed2(ts.Month, month, ref mp);

        Span<char> day = stackalloc char[8];
        month.CopyTo(day);
        int dp = 6;
        WriteFixed2(ts.Day, day, ref dp);

        return Path.Combine(_rootPath, month.ToString(), day.ToString(), tenantId.ToString());
    }

    /// <summary>
    /// 获取日志文件路径。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetLogFilePath(DateTime ts, int tenantId, LogLevel level)
    {
        string dir = GetTenantDirectory(ts, tenantId);
        return Path.Combine(dir, GetFileName(level));
    }

    /// <summary>
    /// 获取等级文件名。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetFileName(LogLevel level)
    {
        return level switch
        {
            LogLevel.Fatal => "fatal.txt",
            LogLevel.Error => "error.txt",
            LogLevel.Warn => "warn.txt",
            LogLevel.Infor => "infor.txt",
            _ => "debug.txt"
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteFixed2(int value, Span<char> buf, ref int pos)
    {
        buf[pos++] = (char)('0' + ((value / 10) % 10));
        buf[pos++] = (char)('0' + (value % 10));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteFixed4(int value, Span<char> buf, ref int pos)
    {
        buf[pos++] = (char)('0' + ((value / 1000) % 10));
        buf[pos++] = (char)('0' + ((value / 100) % 10));
        buf[pos++] = (char)('0' + ((value / 10) % 10));
        buf[pos++] = (char)('0' + (value % 10));
    }
}
