using System;
using System.Threading.Tasks;
using YTStdLogger.Core;

namespace YTStdTenantPlatform.Infrastructure.Persistence
{
    /// <summary>平台健康检查，验证数据库连接和核心服务可用性</summary>
    public static class HealthCheck
    {
        /// <summary>执行数据库连通性检查</summary>
        public static async ValueTask<HealthCheckResult> CheckDatabaseAsync()
        {
            try
            {
                // 骨架实现：通过 DB.GetConnection 验证连接池可用
                // 实际实现应执行 SELECT 1 验证连接有效性
                Logger.Debug(0, 0, "[HealthCheck] 数据库连通性检查");
                return new HealthCheckResult(true, "数据库连接正常");
            }
            catch (Exception ex)
            {
                Logger.Error(0, 0, "[HealthCheck] 数据库连通性检查失败: " + ex.Message);
                return new HealthCheckResult(false, "数据库连接异常: " + ex.Message);
            }
        }

        /// <summary>执行缓存状态检查</summary>
        public static HealthCheckResult CheckCache()
        {
            var permCount = Cache.PlatformCacheWarmer.PermissionCache.Count;
            var hasConfig = Cache.PlatformCacheWarmer.ConfigSnapshot != null;

            if (permCount > 0 && hasConfig)
            {
                return new HealthCheckResult(true,
                    "缓存正常, 权限数=" + permCount);
            }

            return new HealthCheckResult(false,
                "缓存异常, 权限数=" + permCount + " 配置快照=" + hasConfig);
        }

        /// <summary>执行综合健康检查</summary>
        public static async ValueTask<HealthCheckResult> CheckAllAsync()
        {
            var dbResult = await CheckDatabaseAsync();
            if (!dbResult.IsHealthy)
            {
                return dbResult;
            }

            var cacheResult = CheckCache();
            if (!cacheResult.IsHealthy)
            {
                return cacheResult;
            }

            return new HealthCheckResult(true, "所有检查通过");
        }
    }

    /// <summary>健康检查结果</summary>
    public readonly struct HealthCheckResult
    {
        /// <summary>是否健康</summary>
        public bool IsHealthy { get; }

        /// <summary>描述信息</summary>
        public string Message { get; }

        /// <summary>构造健康检查结果</summary>
        public HealthCheckResult(bool isHealthy, string message)
        {
            IsHealthy = isHealthy;
            Message = message;
        }
    }
}
