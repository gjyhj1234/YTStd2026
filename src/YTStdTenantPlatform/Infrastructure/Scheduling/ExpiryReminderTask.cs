using System;
using System.Threading;
using System.Threading.Tasks;
using YTStdLogger.Core;

namespace YTStdTenantPlatform.Infrastructure.Scheduling
{
    /// <summary>订阅到期提醒后台任务，检查即将到期的订阅并发送提醒通知</summary>
    public sealed class ExpiryReminderTask : IScheduledTask
    {
        /// <summary>任务名称</summary>
        public string Name => "订阅到期提醒任务";

        /// <summary>执行间隔（秒），每小时检查一次</summary>
        public int IntervalSeconds => 3600;

        /// <summary>首次延迟（秒）</summary>
        public int InitialDelaySeconds => 60;

        /// <summary>
        /// 执行到期提醒检查。
        /// 查找即将到期（如 7 天内）的订阅并触发通知。
        /// </summary>
        public ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            // 骨架实现：后续阶段将接入实际的到期检查逻辑
            // 1. 查询 TenantSubscription 表中即将到期的记录
            // 2. 按到期时间分级（7 天、3 天、1 天）
            // 3. 生成提醒通知到 Notification 表
            // 4. 记录处理日志

            Logger.Debug(0, 0, "[ExpiryReminderTask] 检查即将到期的订阅");
            return ValueTask.CompletedTask;
        }
    }
}
