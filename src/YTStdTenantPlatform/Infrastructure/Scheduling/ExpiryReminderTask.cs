using System;
using System.Threading;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Entity.TenantPlatform;

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
        public async ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.Debug(0, 0, () => "[ExpiryReminderTask] 检查即将到期的订阅");

            var (subscriptionResult, subscriptions) = await TenantSubscriptionCRUD.GetListAsync(0, 0);
            if (!subscriptionResult.Success || subscriptions == null || subscriptions.Count == 0)
            {
                return;
            }

            var (tenantResult, tenants) = await TenantCRUD.GetListAsync(0, 0);
            if (!tenantResult.Success || tenants == null)
            {
                Logger.Error(0, 0, "[ExpiryReminderTask] 查询租户失败: " + tenantResult.ErrorMessage);
                return;
            }

            var (notificationResult, notifications) = await NotificationCRUD.GetListAsync(0, 0);
            System.Collections.Generic.IReadOnlyList<Notification> existingNotifications = notificationResult.Success && notifications != null
                ? notifications
                : Array.Empty<Notification>();
            var now = DateTime.UtcNow;

            for (int i = 0; i < subscriptions.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var subscription = subscriptions[i];
                if (string.Equals(subscription.SubscriptionStatus, "cancelled", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var tenant = FindTenant(tenants, subscription.TenantRefId);
                if (tenant == null)
                {
                    continue;
                }

                var targetStatus = GetTargetStatus(subscription.ExpiresAt, now);
                if (!string.Equals(subscription.SubscriptionStatus, targetStatus, StringComparison.OrdinalIgnoreCase))
                {
                    subscription.SubscriptionStatus = targetStatus;
                    subscription.UpdatedAt = now;
                    await TenantSubscriptionCRUD.UpdateAsync(0, 0, subscription);
                }

                var reminderDays = GetReminderDays(subscription.ExpiresAt, now);
                if (reminderDays == int.MinValue)
                {
                    continue;
                }

                var marker = BuildReminderMarker(subscription.Id, reminderDays);
                if (HasReminder(existingNotifications, subscription.TenantRefId, marker))
                {
                    continue;
                }

                var notification = new Notification
                {
                    Id = await DB.GetNextLongIdAsync(),
                    TenantRefId = subscription.TenantRefId,
                    Channel = string.IsNullOrWhiteSpace(tenant.ContactEmail) ? "in_app" : "email",
                    Recipient = string.IsNullOrWhiteSpace(tenant.ContactEmail) ? tenant.TenantCode : tenant.ContactEmail,
                    Subject = reminderDays < 0 ? "订阅已到期提醒" : "订阅到期提醒",
                    Body = marker + " 您的订阅#" + subscription.Id + (reminderDays < 0 ? " 已到期，请及时处理。" : " 将在 " + reminderDays + " 天后到期，请及时续费。"),
                    SendStatus = "pending",
                    CreatedAt = now
                };

                var insertResult = await NotificationCRUD.InsertAsync(0, 0, notification);
                if (!insertResult.Success)
                {
                    Logger.Error(0, 0, "[ExpiryReminderTask] 写入提醒通知失败: " + insertResult.ErrorMessage);
                }
            }
        }

        private static Tenant? FindTenant(System.Collections.Generic.IReadOnlyList<Tenant> tenants, long tenantId)
        {
            for (int i = 0; i < tenants.Count; i++)
            {
                if (tenants[i].Id == tenantId && tenants[i].DeletedAt == null)
                {
                    return tenants[i];
                }
            }

            return null;
        }

        private static string GetTargetStatus(DateTime expiresAt, DateTime now)
        {
            if (expiresAt <= now)
            {
                return "expired";
            }

            if ((expiresAt - now).TotalDays <= 7d)
            {
                return "expiring";
            }

            return "active";
        }

        private static int GetReminderDays(DateTime expiresAt, DateTime now)
        {
            var days = (int)Math.Ceiling((expiresAt.Date - now.Date).TotalDays);
            if (days <= 0)
            {
                return -1;
            }

            if (days == 1 || days == 3 || days == 7)
            {
                return days;
            }

            return int.MinValue;
        }

        private static string BuildReminderMarker(long subscriptionId, int days)
        {
            return "[SUBSCRIPTION:" + subscriptionId + "][DAYS:" + days + "]";
        }

        private static bool HasReminder(System.Collections.Generic.IReadOnlyList<Notification> notifications, long tenantId, string marker)
        {
            for (int i = 0; i < notifications.Count; i++)
            {
                var notification = notifications[i];
                if (notification.TenantRefId != tenantId)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(notification.Body) && notification.Body.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
