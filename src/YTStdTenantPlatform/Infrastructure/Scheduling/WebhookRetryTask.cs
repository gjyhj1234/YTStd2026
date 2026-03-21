using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YTStdLogger.Core;
using YTStdTenantPlatform.Entity.TenantPlatform;

namespace YTStdTenantPlatform.Infrastructure.Scheduling
{
    /// <summary>Webhook 投递重试后台任务，重试失败的 Webhook 投递</summary>
    public sealed class WebhookRetryTask : IScheduledTask
    {
        private static readonly HttpClient HttpClient = new HttpClient();

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
        public async ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.Debug(0, 0, () => "[WebhookRetryTask] 检查待重试的 Webhook 投递");

            var (deliveryResult, deliveryLogs) = await WebhookDeliveryLogCRUD.GetListAsync(0, 0);
            var (webhookResult, webhooks) = await TenantWebhookCRUD.GetListAsync(0, 0);
            if (!deliveryResult.Success || deliveryLogs == null || !webhookResult.Success || webhooks == null)
            {
                Logger.Error(0, 0, "[WebhookRetryTask] 加载重试数据失败");
                return;
            }

            for (int i = 0; i < deliveryLogs.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var deliveryLog = deliveryLogs[i];
                if (!string.Equals(deliveryLog.DeliveryStatus, "failed", StringComparison.OrdinalIgnoreCase) || deliveryLog.RetryCount >= MaxRetryCount)
                {
                    continue;
                }

                var webhook = FindWebhook(webhooks, deliveryLog.WebhookId);
                if (webhook == null || !string.Equals(webhook.Status, "active", StringComparison.OrdinalIgnoreCase))
                {
                    deliveryLog.RetryCount = deliveryLog.RetryCount + 1;
                    deliveryLog.ResponseBody = "Webhook 不存在或未启用";
                    await WebhookDeliveryLogCRUD.UpdateAsync(0, 0, deliveryLog);
                    continue;
                }

                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, webhook.TargetUrl);
                    request.Content = new StringContent(deliveryLog.RequestBody ?? "{}", Encoding.UTF8);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    ApplyHeaders(request, deliveryLog.RequestHeaders);

                    using var response = await HttpClient.SendAsync(request, cancellationToken);
                    var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    deliveryLog.ResponseStatusCode = (int)response.StatusCode;
                    deliveryLog.ResponseBody = responseBody;
                    deliveryLog.DeliveredAt = DateTime.UtcNow;
                    if (response.IsSuccessStatusCode)
                    {
                        deliveryLog.DeliveryStatus = "success";
                    }
                    else
                    {
                        deliveryLog.DeliveryStatus = "failed";
                        deliveryLog.RetryCount = deliveryLog.RetryCount + 1;
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    deliveryLog.DeliveryStatus = "failed";
                    deliveryLog.RetryCount = deliveryLog.RetryCount + 1;
                    deliveryLog.ResponseBody = ex.Message;
                    deliveryLog.DeliveredAt = DateTime.UtcNow;
                }

                await WebhookDeliveryLogCRUD.UpdateAsync(0, 0, deliveryLog);
            }
        }

        private static TenantWebhook? FindWebhook(System.Collections.Generic.IReadOnlyList<TenantWebhook> webhooks, long webhookId)
        {
            for (int i = 0; i < webhooks.Count; i++)
            {
                if (webhooks[i].Id == webhookId)
                {
                    return webhooks[i];
                }
            }

            return null;
        }

        private static void ApplyHeaders(HttpRequestMessage request, string? headersJson)
        {
            if (string.IsNullOrWhiteSpace(headersJson))
            {
                return;
            }

            try
            {
                using var document = JsonDocument.Parse(headersJson);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return;
                }

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    var value = property.Value.ValueKind == JsonValueKind.String
                        ? property.Value.GetString()
                        : property.Value.ToString();
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }

                    if (!request.Headers.TryAddWithoutValidation(property.Name, value) && request.Content != null)
                    {
                        request.Content.Headers.TryAddWithoutValidation(property.Name, value);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
