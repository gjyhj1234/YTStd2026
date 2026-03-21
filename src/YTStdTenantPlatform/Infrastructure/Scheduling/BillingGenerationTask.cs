using System;
using System.Threading;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Serialization;

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
        public async ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.Debug(0, 0, () => "[BillingGenerationTask] 检查待生成的账单");

            var (subscriptionResult, subscriptions) = await TenantSubscriptionCRUD.GetListAsync(0, 0);
            var (versionResult, versions) = await SaasPackageVersionCRUD.GetListAsync(0, 0);
            var (invoiceResult, invoices) = await BillingInvoiceCRUD.GetListAsync(0, 0);
            if (!subscriptionResult.Success || subscriptions == null ||
                !versionResult.Success || versions == null ||
                !invoiceResult.Success || invoices == null)
            {
                Logger.Error(0, 0, "[BillingGenerationTask] 账单生成依赖数据加载失败");
                return;
            }

            var now = DateTime.UtcNow;
            for (int i = 0; i < subscriptions.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var subscription = subscriptions[i];
                if (string.Equals(subscription.SubscriptionStatus, "cancelled", StringComparison.OrdinalIgnoreCase) || subscription.ExpiresAt > now)
                {
                    continue;
                }

                var version = FindVersion(versions, subscription.PackageVersionId);
                if (version == null || !version.Enabled)
                {
                    continue;
                }

                var periodEnd = subscription.ExpiresAt;
                if (HasInvoice(invoices, subscription.Id, periodEnd))
                {
                    continue;
                }

                var periodStart = GetPeriodStart(subscription, version);
                var invoiceId = await DB.GetNextLongIdAsync();
                var invoice = new BillingInvoice
                {
                    Id = invoiceId,
                    InvoiceNo = BuildInvoiceNo(invoiceId, now),
                    TenantRefId = subscription.TenantRefId,
                    SubscriptionId = subscription.Id,
                    InvoiceStatus = "pending",
                    BillingPeriodStart = periodStart,
                    BillingPeriodEnd = periodEnd,
                    SubtotalAmount = version.Price,
                    ExtraAmount = 0m,
                    DiscountAmount = 0m,
                    TotalAmount = version.Price,
                    CurrencyCode = version.CurrencyCode,
                    IssuedAt = now,
                    DueAt = now.AddDays(7),
                    CreatedAt = now,
                    UpdatedAt = now
                };

                var invoiceInsertResult = await BillingInvoiceCRUD.InsertAsync(0, 0, invoice);
                if (!invoiceInsertResult.Success)
                {
                    Logger.Error(0, 0, "[BillingGenerationTask] 创建账单失败: " + invoiceInsertResult.ErrorMessage);
                    continue;
                }

                var item = new BillingInvoiceItem
                {
                    Id = await DB.GetNextLongIdAsync(),
                    InvoiceId = invoiceId,
                    ItemType = "subscription",
                    ItemName = version.VersionName,
                    Quantity = 1m,
                    UnitPrice = version.Price,
                    Amount = version.Price,
                    ItemMetadata = BuildItemMetadata(version.Id, version.BillingCycle),
                    CreatedAt = now
                };

                var itemInsertResult = await BillingInvoiceItemCRUD.InsertAsync(0, 0, item);
                if (!itemInsertResult.Success)
                {
                    Logger.Error(0, 0, "[BillingGenerationTask] 创建账单明细失败: " + itemInsertResult.ErrorMessage);
                }
            }
        }

        private static SaasPackageVersion? FindVersion(System.Collections.Generic.IReadOnlyList<SaasPackageVersion> versions, long packageVersionId)
        {
            for (int i = 0; i < versions.Count; i++)
            {
                if (versions[i].Id == packageVersionId)
                {
                    return versions[i];
                }
            }

            return null;
        }

        private static bool HasInvoice(System.Collections.Generic.IReadOnlyList<BillingInvoice> invoices, long subscriptionId, DateTime periodEnd)
        {
            for (int i = 0; i < invoices.Count; i++)
            {
                var invoice = invoices[i];
                if (invoice.SubscriptionId == subscriptionId && invoice.BillingPeriodEnd == periodEnd)
                {
                    return true;
                }
            }

            return false;
        }

        private static DateTime GetPeriodStart(TenantSubscription subscription, SaasPackageVersion version)
        {
            if (string.Equals(version.BillingCycle, "yearly", StringComparison.OrdinalIgnoreCase))
            {
                return subscription.ExpiresAt.AddYears(-1);
            }

            if (string.Equals(version.BillingCycle, "quarterly", StringComparison.OrdinalIgnoreCase))
            {
                return subscription.ExpiresAt.AddMonths(-3);
            }

            if (string.Equals(version.BillingCycle, "one_time", StringComparison.OrdinalIgnoreCase))
            {
                return subscription.StartedAt;
            }

            return subscription.ExpiresAt.AddMonths(-1);
        }

        private static string BuildInvoiceNo(long invoiceId, DateTime now)
        {
            return "INV-" + now.ToString("yyyyMMddHHmmss") + "-" + invoiceId;
        }

        private static string BuildItemMetadata(long packageVersionId, string billingCycle)
        {
            return Utf8JsonWriterHelper.BuildString(
                (packageVersionId, billingCycle),
                static (writer, state) =>
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("packageVersionId", state.packageVersionId);
                    writer.WriteString("billingCycle", state.billingCycle);
                    writer.WriteEndObject();
                });
        }
    }
}
