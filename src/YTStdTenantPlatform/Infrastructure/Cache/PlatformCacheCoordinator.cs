using System;
using System.Threading;
using System.Threading.Tasks;
using YTStdLogger.Core;

namespace YTStdTenantPlatform.Infrastructure.Cache
{
    /// <summary>平台缓存协调组件，负责缓存的显式失效、局部刷新与定时轻量刷新</summary>
    public static class PlatformCacheCoordinator
    {
        /// <summary>定时刷新间隔（秒），默认 300 秒（5 分钟）</summary>
        private static volatile int _refreshIntervalSeconds = 300;

        /// <summary>定时刷新是否启用</summary>
        private static volatile bool _periodicRefreshEnabled;

        /// <summary>定时刷新 CancellationTokenSource</summary>
        private static CancellationTokenSource? _refreshCts;

        /// <summary>定时刷新后台任务引用</summary>
        private static Task? _refreshTask;

        /// <summary>设置定时刷新间隔（秒）</summary>
        public static void SetRefreshInterval(int seconds)
        {
            if (seconds < 10) seconds = 10;
            _refreshIntervalSeconds = seconds;
        }

        /// <summary>启动定时缓存轻量刷新</summary>
        public static void StartPeriodicRefresh()
        {
            if (_periodicRefreshEnabled) return;

            _periodicRefreshEnabled = true;
            _refreshCts = new CancellationTokenSource();
            var token = _refreshCts.Token;

            _refreshTask = Task.Run(async () =>
            {
                Logger.Info(0, 0, "[PlatformCacheCoordinator] 定时刷新已启动, 间隔=" +
                    _refreshIntervalSeconds + "秒");

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(_refreshIntervalSeconds * 1000, token);
                        if (token.IsCancellationRequested) break;

                        await RefreshAllAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(0, 0, "[PlatformCacheCoordinator] 定时刷新异常: " + ex.Message);
                    }
                }

                Logger.Info(0, 0, "[PlatformCacheCoordinator] 定时刷新已停止");
            }, token);
        }

        /// <summary>停止定时缓存刷新</summary>
        public static void StopPeriodicRefresh()
        {
            _periodicRefreshEnabled = false;
            _refreshCts?.Cancel();

            // 等待刷新任务完成（非阻塞，设超时保护）
            var task = _refreshTask;
            if (task != null && !task.IsCompleted)
            {
                task.Wait(TimeSpan.FromSeconds(5));
            }

            _refreshCts?.Dispose();
            _refreshCts = null;
            _refreshTask = null;
        }

        /// <summary>刷新全部缓存</summary>
        public static async ValueTask RefreshAllAsync()
        {
            Logger.Debug(0, 0, "[PlatformCacheCoordinator] 执行全量缓存刷新");
            await PlatformCacheWarmer.WarmUpAsync(0, 0);
        }

        /// <summary>刷新权限相关缓存（权限快照 + 角色权限映射）</summary>
        public static async ValueTask InvalidatePermissionsAsync()
        {
            Logger.Debug(0, 0, "[PlatformCacheCoordinator] 刷新权限缓存");
            await PlatformCacheWarmer.WarmUpPermissionsAsync(0, 0);
            await PlatformCacheWarmer.WarmUpRolePermissionsAsync(0, 0);
        }

        /// <summary>刷新用户角色缓存</summary>
        public static async ValueTask InvalidateUserRolesAsync()
        {
            Logger.Debug(0, 0, "[PlatformCacheCoordinator] 刷新用户角色缓存");
            await PlatformCacheWarmer.WarmUpUserRolesAsync(0, 0);
        }

        /// <summary>刷新功能开关缓存</summary>
        public static async ValueTask InvalidateFeatureFlagsAsync()
        {
            Logger.Debug(0, 0, "[PlatformCacheCoordinator] 刷新功能开关缓存");
            await PlatformCacheWarmer.WarmUpFeatureFlagsAsync(0, 0);
        }

        /// <summary>刷新平台配置缓存</summary>
        public static async ValueTask InvalidateConfigAsync()
        {
            Logger.Debug(0, 0, "[PlatformCacheCoordinator] 刷新平台配置缓存");
            await PlatformCacheWarmer.WarmUpConfigAsync(0, 0);
        }

        /// <summary>清理限流中间件中的过期计数器</summary>
        public static void CleanupRateLimitCounters()
        {
            Middleware.RateLimitMiddleware.CleanupExpired();
        }
    }
}
