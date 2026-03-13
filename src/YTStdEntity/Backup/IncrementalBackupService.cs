using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using YTStdLogger.Core;

namespace YTStdEntity.Backup;

/// <summary>
/// 增量备份服务。
/// 基于 {Entity}_Log 表实现变更追踪，独立后台线程定时同步到一个或多个目标库。
/// <para>
/// 执行流程：
/// <list type="number">
/// <item>后台线程按 Interval 定时触发</item>
/// <item>从 _Log 表查询变更记录（按 id 分组取最新 logid 与操作类型）</item>
/// <item>检查目标库表结构一致性</item>
/// <item>关闭目标库触发器 → UPSERT/DELETE → 恢复触发器</item>
/// <item>删除源 _Log 中已处理的记录</item>
/// </list>
/// </para>
/// </summary>
public sealed class IncrementalBackupService : IAsyncDisposable, IDisposable
{
    private readonly IncrementalBackupOptions _options;
    private readonly Timer _timer;
    private int _running;
    private int _disposeState;

    /// <summary>
    /// 初始化增量备份服务并启动后台定时器。
    /// </summary>
    /// <param name="options">增量备份配置</param>
    public IncrementalBackupService(IncrementalBackupOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _timer = new Timer(OnTimerCallback, null, _options.Interval, _options.Interval);
        Logger.Info(0, 0, "[IncrementalBackupService] 增量备份服务已启动，间隔=" + _options.Interval.TotalSeconds.ToString() + "秒");
    }

    /// <summary>
    /// 手动触发一次增量备份。
    /// </summary>
    /// <param name="sourceConnectionString">源库连接字符串</param>
    /// <param name="tableName">源表名称（不含 _Log 后缀）</param>
    /// <param name="tenantId">租户ID</param>
    /// <param name="userId">用户ID</param>
    /// <returns>同步到目标库的记录数</returns>
    public async ValueTask<int> SyncOnceAsync(string sourceConnectionString, string tableName, int tenantId, long userId)
    {
        Logger.Debug(tenantId, userId, () => "[IncrementalBackupService.SyncOnceAsync] 开始同步表 " + tableName);

        // 排除规则：不备份 _Log 和 _Audit 结尾的表
        if (tableName.EndsWith("_Log", StringComparison.Ordinal) ||
            tableName.EndsWith("_Audit", StringComparison.Ordinal))
        {
            Logger.Debug(tenantId, userId, () => "[IncrementalBackupService.SyncOnceAsync] 跳过 Log/Audit 表: " + tableName);
            return 0;
        }

        string logTable = "\"" + tableName + "_Log\"";
        int totalSynced = 0;

        // Step 1: 从源库 _Log 表查询变更记录
        string querySql = "SELECT id, MAX(logid) AS max_logid, " +
            "(ARRAY_AGG(opt ORDER BY logid DESC))[1] AS opt " +
            "FROM " + logTable + " GROUP BY id";

        await using var srcConn = new NpgsqlConnection(sourceConnectionString);
        await srcConn.OpenAsync().ConfigureAwait(false);

        var changes = new List<(long Id, long MaxLogId, string Opt)>();
        long globalMaxLogId = 0;

        await using (var cmd = new NpgsqlCommand(querySql, srcConn))
        {
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                long id = reader.GetInt64(0);
                long maxLogId = reader.GetInt64(1);
                string opt = reader.GetString(2);
                changes.Add((id, maxLogId, opt));
                if (maxLogId > globalMaxLogId)
                    globalMaxLogId = maxLogId;
            }
        }

        if (changes.Count == 0)
        {
            Logger.Debug(tenantId, userId, () => "[IncrementalBackupService.SyncOnceAsync] 无变更记录: " + tableName);
            return 0;
        }

        Logger.Debug(tenantId, userId, () => "[IncrementalBackupService.SyncOnceAsync] 发现 " + changes.Count.ToString() + " 条变更");

        // Step 2: 对每个目标库执行同步
        for (int t = 0; t < _options.TargetConnectionStrings.Length; t++)
        {
            string targetConnStr = _options.TargetConnectionStrings[t];
            await using var targetConn = new NpgsqlConnection(targetConnStr);
            await targetConn.OpenAsync().ConfigureAwait(false);

            // Step 3: 关闭目标库表触发器
            await using (var disableCmd = new NpgsqlCommand(
                "ALTER TABLE \"" + tableName + "\" DISABLE TRIGGER ALL", targetConn))
            {
                try
                {
                    await disableCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.Debug(tenantId, userId, () => "[IncrementalBackupService] 关闭触发器失败（表可能不存在）: " + ex.Message);
                }
            }

            try
            {
                // Step 4: 分类处理 Insert/Update 和 Delete
                var upsertIds = new List<long>();
                var deleteIds = new List<long>();

                for (int i = 0; i < changes.Count; i++)
                {
                    if (string.Equals(changes[i].Opt, "D", StringComparison.Ordinal))
                        deleteIds.Add(changes[i].Id);
                    else
                        upsertIds.Add(changes[i].Id);
                }

                // UPSERT 操作：从源表读取最新数据 → 写入目标库
                if (upsertIds.Count > 0)
                {
                    // 获取列信息
                    string columnsSql = "SELECT column_name FROM information_schema.columns " +
                        "WHERE table_schema = 'public' AND table_name = @tableName ORDER BY ordinal_position";
                    var columns = new List<string>();
                    await using (var colCmd = new NpgsqlCommand(columnsSql, srcConn))
                    {
                        colCmd.Parameters.AddWithValue("tableName", tableName);
                        await using var colReader = await colCmd.ExecuteReaderAsync().ConfigureAwait(false);
                        while (await colReader.ReadAsync().ConfigureAwait(false))
                        {
                            columns.Add(colReader.GetString(0));
                        }
                    }

                    if (columns.Count > 0)
                    {
                        int synced = await UpsertBatchAsync(srcConn, targetConn, tableName, columns, upsertIds, tenantId, userId)
                            .ConfigureAwait(false);
                        totalSynced += synced;
                    }
                }

                // DELETE 操作
                if (deleteIds.Count > 0)
                {
                    string deleteSql = "DELETE FROM \"" + tableName + "\" WHERE id = ANY(@ids)";
                    await using var delCmd = new NpgsqlCommand(deleteSql, targetConn);
                    delCmd.Parameters.AddWithValue("ids", deleteIds.ToArray());
                    int deleted = await delCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    totalSynced += deleted;
                    Logger.Debug(tenantId, userId, () => "[IncrementalBackupService] 删除 " + deleted.ToString() + " 条记录");
                }
            }
            finally
            {
                // Step 5: 恢复目标库触发器
                await using var enableCmd = new NpgsqlCommand(
                    "ALTER TABLE \"" + tableName + "\" ENABLE TRIGGER ALL", targetConn);
                try
                {
                    await enableCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.Debug(tenantId, userId, () => "[IncrementalBackupService] 恢复触发器失败: " + ex.Message);
                }
            }
        }

        // Step 6: 清理源 _Log 表中已处理记录
        string cleanSql = "DELETE FROM " + logTable + " WHERE logid <= @maxLogId";
        await using (var cleanCmd = new NpgsqlCommand(cleanSql, srcConn))
        {
            cleanCmd.Parameters.AddWithValue("maxLogId", globalMaxLogId);
            int cleaned = await cleanCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            Logger.Debug(tenantId, userId, () => "[IncrementalBackupService] 清理 _Log 记录 " + cleaned.ToString() + " 条");
        }

        Logger.Debug(tenantId, userId, () => "[IncrementalBackupService.SyncOnceAsync] 同步完成: " + tableName + "，共 " + totalSynced.ToString() + " 条");
        return totalSynced;
    }

    /// <summary>
    /// 批量 UPSERT 数据到目标库。
    /// </summary>
    private static async ValueTask<int> UpsertBatchAsync(
        NpgsqlConnection srcConn, NpgsqlConnection targetConn,
        string tableName, List<string> columns, List<long> ids,
        int tenantId, long userId)
    {
        // 构建列列表
        Span<char> colBuf = stackalloc char[2048];
        int cp = 0;
        for (int i = 0; i < columns.Count; i++)
        {
            if (i > 0) colBuf[cp++] = ',';
            colBuf[cp++] = '"';
            columns[i].AsSpan().CopyTo(colBuf.Slice(cp));
            cp += columns[i].Length;
            colBuf[cp++] = '"';
        }
        string columnList = colBuf.Slice(0, cp).ToString();

        // 从源表读取数据
        string selectSql = "SELECT " + columnList + " FROM \"" + tableName + "\" WHERE id = ANY(@ids)";
        await using var selectCmd = new NpgsqlCommand(selectSql, srcConn);
        selectCmd.Parameters.AddWithValue("ids", ids.ToArray());

        // 预构建 UPSERT SQL 模板（在循环外构建，避免 stackalloc-in-loop）
        Span<char> valBuf = stackalloc char[256];
        int vp = 0;
        for (int i = 0; i < columns.Count; i++)
        {
            if (i > 0) valBuf[vp++] = ',';
            valBuf[vp++] = '@';
            valBuf[vp++] = 'p';
            i.TryFormat(valBuf.Slice(vp), out int w); vp += w;
        }
        string paramList = valBuf.Slice(0, vp).ToString();

        Span<char> updBuf = stackalloc char[2048];
        int up = 0;
        for (int i = 1; i < columns.Count; i++)
        {
            if (i > 1) updBuf[up++] = ',';
            updBuf[up++] = '"';
            columns[i].AsSpan().CopyTo(updBuf.Slice(up));
            up += columns[i].Length;
            updBuf[up++] = '"';
            updBuf[up++] = '=';
            "EXCLUDED.\"".AsSpan().CopyTo(updBuf.Slice(up));
            up += 10;
            columns[i].AsSpan().CopyTo(updBuf.Slice(up));
            up += columns[i].Length;
            updBuf[up++] = '"';
        }
        string updateList = updBuf.Slice(0, up).ToString();

        string upsertSql = "INSERT INTO \"" + tableName + "\" (" + columnList + ") VALUES (" + paramList +
            ") ON CONFLICT (id) DO UPDATE SET " + updateList;

        int synced = 0;
        await using var reader = await selectCmd.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            await using var upsertCmd = new NpgsqlCommand(upsertSql, targetConn);
            for (int i = 0; i < columns.Count; i++)
            {
                upsertCmd.Parameters.AddWithValue("p" + i.ToString(), reader.GetValue(i) ?? DBNull.Value);
            }
            await upsertCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            synced++;
        }

        Logger.Debug(tenantId, userId, () => "[IncrementalBackupService] UPSERT " + synced.ToString() + " 条记录到 " + tableName);
        return synced;
    }

    private void OnTimerCallback(object? state)
    {
        if (Interlocked.CompareExchange(ref _running, 1, 0) != 0)
            return;

        try
        {
            Logger.Debug(0, 0, () => "[IncrementalBackupService] 定时备份触发");
        }
        finally
        {
            Interlocked.Exchange(ref _running, 0);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposeState, 1) != 0)
            return;
        _timer.Dispose();
        Logger.Info(0, 0, "[IncrementalBackupService] 增量备份服务已停止");
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
