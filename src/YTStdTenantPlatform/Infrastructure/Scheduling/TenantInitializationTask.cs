using System;
using System.Threading;
using System.Threading.Tasks;
using YTStdLogger.Core;

namespace YTStdTenantPlatform.Infrastructure.Scheduling
{
    /// <summary>租户初始化后台任务，轮询并执行待处理的租户初始化任务</summary>
    public sealed class TenantInitializationTask : IScheduledTask
    {
        /// <summary>任务名称</summary>
        public string Name => "租户初始化任务";

        /// <summary>执行间隔（秒），每 30 秒检查一次</summary>
        public int IntervalSeconds => 30;

        /// <summary>首次延迟（秒）</summary>
        public int InitialDelaySeconds => 10;

        /// <summary>
        /// 执行租户初始化任务。
        /// 查找 status=pending 的 TenantInitializationTask 记录并依次执行。
        /// </summary>
        public ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            // 骨架实现：后续阶段将接入实际的初始化任务执行逻辑
            // 1. 查询 TenantInitializationTask 表中 status=pending 的记录
            // 2. 按 created_at 排序逐条执行
            // 3. 更新任务状态为 running → completed / failed
            // 4. 记录执行日志

            Logger.Debug(0, 0, "[TenantInitializationTask] 检查待处理的租户初始化任务");
            return ValueTask.CompletedTask;
        }
    }
}
