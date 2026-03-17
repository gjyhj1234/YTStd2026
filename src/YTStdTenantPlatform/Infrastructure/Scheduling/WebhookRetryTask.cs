using System;
using System.Threading;
using System.Threading.Tasks;
using YTStdLogger.Core;

namespace YTStdTenantPlatform.Infrastructure.Scheduling
{
    /// <summary>Webhook 投递重试后台任务，重试失败的 Webhook 投递</summary>
    public sealed class WebhookRetryTask : IScheduledTask
    {
        /// <summary>任务名称</summary>
        public string Name => "Webhook重试任务";

        /// <summary>执行间隔（秒），每 2 分钟检查一次</summary>
        public int IntervalSeconds => 120;

        /// <summary>首次延迟（秒）</summary>
        public int InitialDelaySeconds => 30;

        /// <summary>最大重试次数</summary>
        public const int MaxRetryCount = 5;

        /// <summary>
        /// 执行 Webhook 重试。
        /// 查找投递失败且重试次数未超限的记录进行重试。
        /// </summary>
        public ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            // 骨架实现：后续阶段将接入实际的 Webhook 重试逻辑
            // 1. 查询 WebhookDeliveryLog 中 status=failed 且 retry_count < MaxRetryCount 的记录
            // 2. 按优先级和创建时间排序
            // 3. 逐条重试投递（HTTP POST 到目标 URL）
            // 4. 更新投递状态和重试计数
            // 5. 记录重试日志

            Logger.Debug(0, 0, "[WebhookRetryTask] 检查待重试的 Webhook 投递");
            return ValueTask.CompletedTask;
        }
    }
}
