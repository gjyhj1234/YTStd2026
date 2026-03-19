using System;
using System.Collections.Generic;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Serialization;

namespace YTStdTenantPlatform.Infrastructure.Initialization.SeedData
{
    /// <summary>默认基础设施种子数据</summary>
    public static class DefaultInfrastructure
    {
        /// <summary>获取默认限流策略列表</summary>
        public static IReadOnlyList<RateLimitPolicy> GetDefaultRateLimitPolicies()
        {
            var now = DateTime.UtcNow;
            return new[]
            {
                new RateLimitPolicy
                {
                    SubjectType = "api",
                    SubjectKey = "global",
                    WindowSeconds = 60,
                    LimitCount = 1000,
                    Status = "active",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new RateLimitPolicy
                {
                    SubjectType = "tenant",
                    SubjectKey = "default",
                    WindowSeconds = 60,
                    LimitCount = 100,
                    Status = "active",
                    CreatedAt = now,
                    UpdatedAt = now
                }
            };
        }

        /// <summary>获取默认数据隔离策略列表</summary>
        public static IReadOnlyList<DataIsolationPolicy> GetDefaultDataIsolationPolicies()
        {
            var now = DateTime.UtcNow;
            return new[]
            {
                new DataIsolationPolicy
                {
                    IsolationType = "tenant_isolation",
                    PolicyName = "默认租户隔离策略",
                    PolicyConfig = BuildIsolationPolicyConfig(),
                    Status = "active",
                    CreatedAt = now,
                    UpdatedAt = now
                }
            };
        }

        /// <summary>获取默认基础设施组件列表</summary>
        public static IReadOnlyList<InfrastructureComponent> GetDefaultComponents()
        {
            var now = DateTime.UtcNow;
            return new[]
            {
                new InfrastructureComponent
                {
                    ComponentType = "cache",
                    ComponentName = "本地缓存",
                    Status = "active",
                    ComponentConfig = BuildCacheComponentConfig(),
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new InfrastructureComponent
                {
                    ComponentType = "scheduler",
                    ComponentName = "任务调度器",
                    Status = "active",
                    ComponentConfig = BuildSchedulerComponentConfig(),
                    CreatedAt = now,
                    UpdatedAt = now
                }
            };
        }

        private static string BuildIsolationPolicyConfig()
        {
            return Utf8JsonWriterHelper.BuildString(
                false,
                static (writer, _) =>
                {
                    writer.WriteStartObject();
                    writer.WriteString("mode", "shared_database");
                    writer.WriteBoolean("row_level_security", true);
                    writer.WriteEndObject();
                });
        }

        private static string BuildCacheComponentConfig()
        {
            return Utf8JsonWriterHelper.BuildString(
                false,
                static (writer, _) =>
                {
                    writer.WriteStartObject();
                    writer.WriteString("provider", "memory");
                    writer.WriteNumber("default_ttl_seconds", 300);
                    writer.WriteEndObject();
                });
        }

        private static string BuildSchedulerComponentConfig()
        {
            return Utf8JsonWriterHelper.BuildString(
                false,
                static (writer, _) =>
                {
                    writer.WriteStartObject();
                    writer.WriteString("provider", "built_in");
                    writer.WriteNumber("max_concurrent_jobs", 10);
                    writer.WriteEndObject();
                });
        }
    }
}
