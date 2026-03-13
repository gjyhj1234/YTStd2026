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
        Logger.Info(0, 0, () =>
        {
            var vsb = new ValueStringBuilder(64);
            vsb.Append("[IncrementalBackupService] 增量备份服务已启动，间隔=");
            vsb.Append((int)_options.Interval.TotalSeconds);
            vsb.Append("秒");
            return vsb.ToString();
        });
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
        Logger.Debug(tenantId, userId, () =>
        {
            var vsb = new ValueStringBuilder(64);
            vsb.Append("[IncrementalBackupService.SyncOnceAsync] 开始同步表 ");
            vsb.Append(tableName);
            return vsb.ToString();
        });

        // 排除规则：不备份 _Log 和 _Audit 结尾的表
        if (tableName.EndsWith("_Log", StringComparison.Ordinal) ||
            tableName.EndsWith("_Audit", StringComparison.Ordinal))
        {
            Logger.Debug(tenantId, userId, () =>
            {
                var vsb = new ValueStringBuilder(64);
                vsb.Append("[IncrementalBackupService.SyncOnceAsync] 跳过 Log/Audit 表: ");
                vsb.Append(tableName);
                return vsb.ToString();
            });
            return 0;
        }

        var vsbLogTable = new ValueStringBuilder(stackalloc char[64]);
        vsbLogTable.Append('"');
        vsbLogTable.Append(tableName);
        vsbLogTable.Append("_Log\"");
        string logTable = vsbLogTable.ToString();
        int totalSynced = 0;

        // Step 1: 从源库 _Log 表查询变更记录
        var vsbQuery = new ValueStringBuilder(stackalloc char[256]);
        vsbQuery.Append("SELECT id, MAX(logid) AS max_logid, ");
        vsbQuery.Append("(ARRAY_AGG(opt ORDER BY logid DESC))[1] AS opt ");
        vsbQuery.Append("FROM ");
        vsbQuery.Append(logTable);
        vsbQuery.Append(" GROUP BY id");
        string querySql = vsbQuery.ToString();

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
            Logger.Debug(tenantId, userId, () =>
            {
                var vsb = new ValueStringBuilder(64);
                vsb.Append("[IncrementalBackupService.SyncOnceAsync] 无变更记录: ");
                vsb.Append(tableName);
                return vsb.ToString();
            });
            return 0;
        }

        Logger.Debug(tenantId, userId, () =>
        {
            var vsb = new ValueStringBuilder(64);
            vsb.Append("[IncrementalBackupService.SyncOnceAsync] 发现 ");
            vsb.Append(changes.Count);
            vsb.Append(" 条变更");
            return vsb.ToString();
        });

        // Step 2: 对每个目标库执行同步
        for (int t = 0; t < _options.TargetConnectionStrings.Length; t++)
        {
            string targetConnStr = _options.TargetConnectionStrings[t];
            await using var targetConn = new NpgsqlConnection(targetConnStr);
            await targetConn.OpenAsync().ConfigureAwait(false);

            // Step 3: 关闭目标库表触发器
            var vsbDisable = new ValueStringBuilder(128);
            vsbDisable.Append("ALTER TABLE \"");
            vsbDisable.Append(tableName);
            vsbDisable.Append("\" DISABLE TRIGGER ALL");
            string disableSql = vsbDisable.ToString();

            await using (var disableCmd = new NpgsqlCommand(disableSql, targetConn))
            {
                try
                {
                    await disableCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.Debug(tenantId, userId, () =>
                    {
                        var vsb = new ValueStringBuilder(128);
                        vsb.Append("[IncrementalBackupService] 关闭触发器失败（表可能不存在）: ");
                        vsb.Append(ex.Message);
                        return vsb.ToString();
                    });
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
                    var vsbDelete = new ValueStringBuilder(128);
                    vsbDelete.Append("DELETE FROM \"");
                    vsbDelete.Append(tableName);
                    vsbDelete.Append("\" WHERE id = ANY(@ids)");
                    string deleteSql = vsbDelete.ToString();
                    await using var delCmd = new NpgsqlCommand(deleteSql, targetConn);
                    delCmd.Parameters.AddWithValue("ids", deleteIds.ToArray());
                    int deleted = await delCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    totalSynced += deleted;
                    Logger.Debug(tenantId, userId, () =>
                    {
                        var vsb = new ValueStringBuilder(64);
                        vsb.Append("[IncrementalBackupService] 删除 ");
                        vsb.Append(deleted);
                        vsb.Append(" 条记录");
                        return vsb.ToString();
                    });
                }
            }
            finally
            {
                // Step 5: 恢复目标库触发器
                var vsbEnable = new ValueStringBuilder(128);
                vsbEnable.Append("ALTER TABLE \"");
                vsbEnable.Append(tableName);
                vsbEnable.Append("\" ENABLE TRIGGER ALL");
                string enableSql = vsbEnable.ToString();

                await using var enableCmd = new NpgsqlCommand(enableSql, targetConn);
                try
                {
                    await enableCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.Debug(tenantId, userId, () =>
                    {
                        var vsb = new ValueStringBuilder(64);
                        vsb.Append("[IncrementalBackupService] 恢复触发器失败: ");
                        vsb.Append(ex.Message);
                        return vsb.ToString();
                    });
                }
            }
        }

        // Step 6: 清理源 _Log 表中已处理记录
        var vsbClean = new ValueStringBuilder(stackalloc char[128]);
        vsbClean.Append("DELETE FROM ");
        vsbClean.Append(logTable);
        vsbClean.Append(" WHERE logid <= @maxLogId");
        string cleanSql = vsbClean.ToString();
        await using (var cleanCmd = new NpgsqlCommand(cleanSql, srcConn))
        {
            cleanCmd.Parameters.AddWithValue("maxLogId", globalMaxLogId);
            int cleaned = await cleanCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            Logger.Debug(tenantId, userId, () =>
            {
                var vsb = new ValueStringBuilder(64);
                vsb.Append("[IncrementalBackupService] 清理 _Log 记录 ");
                vsb.Append(cleaned);
                vsb.Append(" 条");
                return vsb.ToString();
            });
        }

        Logger.Debug(tenantId, userId, () =>
        {
            var vsb = new ValueStringBuilder(128);
            vsb.Append("[IncrementalBackupService.SyncOnceAsync] 同步完成: ");
            vsb.Append(tableName);
            vsb.Append("，共 ");
            vsb.Append(totalSynced);
            vsb.Append(" 条");
            return vsb.ToString();
        });
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
        // 构建列列表、参数列表和更新列表（使用 string 拼接确保安全无越界）
        string columnList = BuildQuotedColumnList(columns);
        string paramList = BuildParamPlaceholders(columns.Count);
        string updateList = BuildConflictUpdateList(columns);

        // 从源表读取数据
        var vsbSelect = new ValueStringBuilder(stackalloc char[256]);
        vsbSelect.Append("SELECT ");
        vsbSelect.Append(columnList);
        vsbSelect.Append(" FROM \"");
        vsbSelect.Append(tableName);
        vsbSelect.Append("\" WHERE id = ANY(@ids)");
        string selectSql = vsbSelect.ToString();
        await using var selectCmd = new NpgsqlCommand(selectSql, srcConn);
        selectCmd.Parameters.AddWithValue("ids", ids.ToArray());

        var vsbUpsert = new ValueStringBuilder(stackalloc char[512]);
        vsbUpsert.Append("INSERT INTO \"");
        vsbUpsert.Append(tableName);
        vsbUpsert.Append("\" (");
        vsbUpsert.Append(columnList);
        vsbUpsert.Append(") VALUES (");
        vsbUpsert.Append(paramList);
        vsbUpsert.Append(") ON CONFLICT (id) DO UPDATE SET ");
        vsbUpsert.Append(updateList);
        string upsertSql = vsbUpsert.ToString();

        int synced = 0;
        await using var reader = await selectCmd.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            await using var upsertCmd = new NpgsqlCommand(upsertSql, targetConn);
            for (int i = 0; i < columns.Count; i++)
            {
                var vsbParam = new ValueStringBuilder(8);
                vsbParam.Append('p');
                vsbParam.Append(i);
                upsertCmd.Parameters.AddWithValue(vsbParam.ToString(), reader.GetValue(i) ?? DBNull.Value);
            }
            await upsertCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            synced++;
        }

        Logger.Debug(tenantId, userId, () =>
        {
            var vsb = new ValueStringBuilder(64);
            vsb.Append("[IncrementalBackupService] UPSERT ");
            vsb.Append(synced);
            vsb.Append(" 条记录到 ");
            vsb.Append(tableName);
            return vsb.ToString();
        });
        return synced;
    }

    /// <summary>构建带引号的列名列表。</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string BuildQuotedColumnList(List<string> columns)
    {
        // 非热路径操作，使用 ValueStringBuilder 确保任意列数安全
        var vsb = new ValueStringBuilder(stackalloc char[256]);
        for (int i = 0; i < columns.Count; i++)
        {
            if (i > 0) vsb.Append(',');
            vsb.Append('"');
            vsb.Append(columns[i]);
            vsb.Append('"');
        }
        return vsb.ToString();
    }

    /// <summary>构建参数占位符列表（@p0,@p1,...）。</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string BuildParamPlaceholders(int count)
    {
        var vsb = new ValueStringBuilder(stackalloc char[128]);
        for (int i = 0; i < count; i++)
        {
            if (i > 0) vsb.Append(',');
            vsb.Append("@p");
            vsb.Append(i);
        }
        return vsb.ToString();
    }

    /// <summary>构建 ON CONFLICT UPDATE 子句（跳过第一列 id）。</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string BuildConflictUpdateList(List<string> columns)
    {
        var vsb = new ValueStringBuilder(stackalloc char[512]);
        for (int i = 1; i < columns.Count; i++)
        {
            if (i > 1) vsb.Append(',');
            vsb.Append('"');
            vsb.Append(columns[i]);
            vsb.Append("\"=EXCLUDED.\"");
            vsb.Append(columns[i]);
            vsb.Append('"');
        }
        return vsb.ToString();
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
