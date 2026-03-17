using System;
using System.Threading;
using System.Threading.Tasks;
using YTStdLogger.Core;

namespace YTStdTenantPlatform.Infrastructure.Scheduling
{
    /// <summary>账单生成后台任务，按计费周期自动生成待处理账单</summary>
    public sealed class BillingGenerationTask : IScheduledTask
    {
        /// <summary>任务名称</summary>
        public string Name => "账单生成任务";

        /// <summary>执行间隔（秒），每小时检查一次</summary>
        public int IntervalSeconds => 3600;

        /// <summary>首次延迟（秒）</summary>
        public int InitialDelaySeconds => 120;

        /// <summary>
        /// 执行账单生成。
        /// 检查到期且尚未生成账单的订阅，自动生成 BillingInvoice。
        /// </summary>
        public ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            // 骨架实现：后续阶段将接入实际的账单生成逻辑
            // 1. 查询 TenantSubscription 中已到计费周期的活跃订阅
            // 2. 计算本期费用（基于套餐定价和用量）
            // 3. 生成 BillingInvoice + BillingInvoiceItem
            // 4. 更新订阅的下次计费时间
            // 5. 记录处理日志

            Logger.Debug(0, 0, "[BillingGenerationTask] 检查待生成的账单");
            return ValueTask.CompletedTask;
        }
    }
}
