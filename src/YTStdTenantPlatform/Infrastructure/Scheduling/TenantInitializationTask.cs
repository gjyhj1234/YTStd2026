using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YTStdLogger.Core;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Serialization;

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
        public async ValueTask ExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.Debug(0, 0, () => "[TenantInitializationTask] 检查待处理的租户初始化任务");

            var (taskResult, tasks) = await YTStdTenantPlatform.Entity.TenantPlatform.TenantInitializationTaskCRUD.GetListAsync(0, 0);
            if (!taskResult.Success || tasks == null || tasks.Count == 0)
            {
                return;
            }

            var (tenantResult, tenants) = await TenantCRUD.GetListAsync(0, 0);
            if (!tenantResult.Success || tenants == null)
            {
                Logger.Error(0, 0, "[TenantInitializationTask] 查询租户失败: " + tenantResult.ErrorMessage);
                return;
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var task = tasks[i];
                if (!string.Equals(task.TaskStatus, "pending", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                await ProcessTaskAsync(task, tenants);
            }
        }

        private static async ValueTask ProcessTaskAsync(
            YTStdTenantPlatform.Entity.TenantPlatform.TenantInitializationTask task,
            IReadOnlyList<Tenant> tenants)
        {
            var now = DateTime.UtcNow;
            task.TaskStatus = "running";
            task.StartedAt = now;
            task.Details = BuildTaskDetails(task.TaskType, "running", "初始化任务开始执行");
            await YTStdTenantPlatform.Entity.TenantPlatform.TenantInitializationTaskCRUD.UpdateAsync(0, 0, task);

            Tenant? tenant = null;
            for (int i = 0; i < tenants.Count; i++)
            {
                if (tenants[i].Id == task.TenantRefId && tenants[i].DeletedAt == null)
                {
                    tenant = tenants[i];
                    break;
                }
            }

            if (tenant == null)
            {
                task.TaskStatus = "failed";
                task.FinishedAt = DateTime.UtcNow;
                task.Details = BuildTaskDetails(task.TaskType, "failed", "关联租户不存在");
                await YTStdTenantPlatform.Entity.TenantPlatform.TenantInitializationTaskCRUD.UpdateAsync(0, 0, task);
                return;
            }

            var tenantUpdated = false;
            if (string.IsNullOrEmpty(tenant.SchemaName))
            {
                tenant.SchemaName = "tenant_" + tenant.Id;
                tenantUpdated = true;
            }

            if (string.IsNullOrEmpty(tenant.DatabaseName) && !string.Equals(tenant.IsolationMode, "shared_database", StringComparison.OrdinalIgnoreCase))
            {
                tenant.DatabaseName = "tenant_" + tenant.Id;
                tenantUpdated = true;
            }

            if (string.IsNullOrEmpty(tenant.DefaultDomain))
            {
                tenant.DefaultDomain = tenant.TenantCode.ToLowerInvariant() + ".platform.local";
                tenantUpdated = true;
            }

            if (tenantUpdated)
            {
                tenant.UpdatedAt = DateTime.UtcNow;
                await TenantCRUD.UpdateAsync(0, 0, tenant);
            }

            task.TaskStatus = "success";
            task.FinishedAt = DateTime.UtcNow;
            task.Details = BuildTaskDetails(task.TaskType, "success", "初始化任务执行完成");
            await YTStdTenantPlatform.Entity.TenantPlatform.TenantInitializationTaskCRUD.UpdateAsync(0, 0, task);
        }

        private static string BuildTaskDetails(string taskType, string status, string message)
        {
            return Utf8JsonWriterHelper.BuildString(
                (taskType, status, message, handledAt: DateTime.UtcNow),
                static (writer, state) =>
                {
                    writer.WriteStartObject();
                    writer.WriteString("taskType", state.taskType);
                    writer.WriteString("status", state.status);
                    writer.WriteString("message", state.message);
                    writer.WriteString("handledAt", state.handledAt);
                    writer.WriteEndObject();
                });
        }
    }
}
