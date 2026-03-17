using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using YTStdTenantPlatform.Infrastructure.Auth;
using YTStdTenantPlatform.Infrastructure.Cache;
using YTStdTenantPlatform.Infrastructure.Middleware;
using YTStdTenantPlatform.Infrastructure.Persistence;
using YTStdTenantPlatform.Infrastructure.Scheduling;

namespace YTStdTenantPlatform.Tests
{
    /// <summary>后端基础设施组件测试</summary>
    public class InfrastructureTests
    {
        // ============================================================
        // CurrentUser 测试
        // ============================================================

        [Fact]
        public void CurrentUser_Anonymous_HasZeroUserId()
        {
            var anon = CurrentUser.Anonymous;
            Assert.Equal(0, anon.UserId);
            Assert.Equal("anonymous", anon.Username);
            Assert.False(anon.IsSuperAdmin);
            Assert.Empty(anon.Roles);
            Assert.Empty(anon.Permissions);
        }

        [Fact]
        public void CurrentUser_HasPermission_ReturnsTrueForExisting()
        {
            var user = new CurrentUser(
                1, "admin", "管理员",
                new[] { "super_admin" },
                new[] { "platform:user:list", "platform:role:list" },
                false, "trace-123");

            Assert.True(user.HasPermission("platform:user:list"));
            Assert.True(user.HasPermission("PLATFORM:USER:LIST")); // 不区分大小写
            Assert.False(user.HasPermission("platform:unknown"));
        }

        [Fact]
        public void CurrentUser_SuperAdmin_HasAllPermissions()
        {
            var user = new CurrentUser(
                1, "admin", "管理员",
                new[] { "super_admin" },
                Array.Empty<string>(),
                true, "trace-456");

            Assert.True(user.HasPermission("any:permission:code"));
            Assert.True(user.HasRole("any_role"));
        }

        [Fact]
        public void CurrentUser_HasRole_ReturnsTrueForExisting()
        {
            var user = new CurrentUser(
                2, "operator", "操作员",
                new[] { "admin", "operator" },
                Array.Empty<string>(),
                false, "trace-789");

            Assert.True(user.HasRole("admin"));
            Assert.True(user.HasRole("OPERATOR")); // 不区分大小写
            Assert.False(user.HasRole("unknown_role"));
        }

        [Fact]
        public void CurrentUser_HttpContextKey_IsNotEmpty()
        {
            Assert.False(string.IsNullOrEmpty(CurrentUser.HttpContextKey));
        }

        // ============================================================
        // PlatformAuthHandler 测试
        // ============================================================

        [Fact]
        public void PlatformAuthHandler_GenerateToken_ReturnsNonEmpty()
        {
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");
            var token = PlatformAuthHandler.GenerateToken(1, "admin");
            Assert.False(string.IsNullOrEmpty(token));

            // Token 应包含 4 个部分（userId:username:timestamp:signature）
            var parts = token.Split(':');
            Assert.Equal(4, parts.Length);
            Assert.Equal("1", parts[0]);
            Assert.Equal("admin", parts[1]);
        }

        [Fact]
        public void PlatformAuthHandler_GenerateToken_DifferentUsersProduceDifferentTokens()
        {
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");
            var token1 = PlatformAuthHandler.GenerateToken(1, "admin");
            var token2 = PlatformAuthHandler.GenerateToken(2, "operator");
            Assert.NotEqual(token1, token2);
        }

        // ============================================================
        // HealthCheck 测试
        // ============================================================

        [Fact]
        public void HealthCheck_CheckCache_ReturnsResult()
        {
            // 清空缓存后检查
            PlatformCacheWarmer.ClearAll();
            var result = HealthCheck.CheckCache();
            // 缓存为空时应该不健康
            Assert.False(result.IsHealthy);
            Assert.False(string.IsNullOrEmpty(result.Message));
        }

        [Fact]
        public void HealthCheckResult_StoresValues()
        {
            var result = new HealthCheckResult(true, "测试通过");
            Assert.True(result.IsHealthy);
            Assert.Equal("测试通过", result.Message);

            var failed = new HealthCheckResult(false, "检查失败");
            Assert.False(failed.IsHealthy);
            Assert.Equal("检查失败", failed.Message);
        }

        // ============================================================
        // PlatformCacheCoordinator 测试
        // ============================================================

        [Fact]
        public void PlatformCacheCoordinator_SetRefreshInterval_AcceptsValue()
        {
            // 不抛异常即通过
            PlatformCacheCoordinator.SetRefreshInterval(60);
            PlatformCacheCoordinator.SetRefreshInterval(5); // 低于最小值 10，应被修正
        }

        [Fact]
        public void PlatformCacheCoordinator_CleanupRateLimitCounters_DoesNotThrow()
        {
            // 不抛异常即通过
            PlatformCacheCoordinator.CleanupRateLimitCounters();
        }

        // ============================================================
        // RateLimitEntry 测试
        // ============================================================

        [Fact]
        public void RateLimitEntry_TryIncrement_AllowsWithinLimit()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var entry = new RateLimitEntry(now);

            // 在限制内应该允许
            for (int i = 0; i < 10; i++)
            {
                Assert.True(entry.TryIncrement(now, 60, 10));
            }
        }

        [Fact]
        public void RateLimitEntry_TryIncrement_BlocksOverLimit()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var entry = new RateLimitEntry(now);

            // 填满限制
            for (int i = 0; i < 5; i++)
            {
                Assert.True(entry.TryIncrement(now, 60, 5));
            }

            // 超限应该阻止
            Assert.False(entry.TryIncrement(now, 60, 5));
        }

        [Fact]
        public void RateLimitEntry_TryIncrement_ResetsAfterWindow()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var entry = new RateLimitEntry(now);

            // 填满限制
            for (int i = 0; i < 5; i++)
            {
                entry.TryIncrement(now, 10, 5);
            }
            Assert.False(entry.TryIncrement(now, 10, 5));

            // 窗口过期后应该重置
            var future = now + 11;
            Assert.True(entry.TryIncrement(future, 10, 5));
        }

        // ============================================================
        // IScheduledTask 实现测试
        // ============================================================

        [Fact]
        public void TenantInitializationTask_HasCorrectProperties()
        {
            var task = new TenantInitializationTask();
            Assert.Equal("租户初始化任务", task.Name);
            Assert.True(task.IntervalSeconds > 0);
            Assert.True(task.InitialDelaySeconds >= 0);
        }

        [Fact]
        public void ExpiryReminderTask_HasCorrectProperties()
        {
            var task = new ExpiryReminderTask();
            Assert.Equal("订阅到期提醒任务", task.Name);
            Assert.True(task.IntervalSeconds > 0);
            Assert.True(task.InitialDelaySeconds >= 0);
        }

        [Fact]
        public void BillingGenerationTask_HasCorrectProperties()
        {
            var task = new BillingGenerationTask();
            Assert.Equal("账单生成任务", task.Name);
            Assert.True(task.IntervalSeconds > 0);
            Assert.True(task.InitialDelaySeconds >= 0);
        }

        [Fact]
        public void WebhookRetryTask_HasCorrectProperties()
        {
            var task = new WebhookRetryTask();
            Assert.Equal("Webhook重试任务", task.Name);
            Assert.True(task.IntervalSeconds > 0);
            Assert.True(task.InitialDelaySeconds >= 0);
            Assert.True(WebhookRetryTask.MaxRetryCount > 0);
        }

        [Fact]
        public async System.Threading.Tasks.Task AllScheduledTasks_ExecuteWithoutException()
        {
            var tasks = new IScheduledTask[]
            {
                new TenantInitializationTask(),
                new ExpiryReminderTask(),
                new BillingGenerationTask(),
                new WebhookRetryTask()
            };

            foreach (var task in tasks)
            {
                // 骨架任务执行不应抛异常
                await task.ExecuteAsync(CancellationToken.None);
            }
        }

        [Fact]
        public void AllScheduledTasks_HaveUniqueNames()
        {
            var tasks = new IScheduledTask[]
            {
                new TenantInitializationTask(),
                new ExpiryReminderTask(),
                new BillingGenerationTask(),
                new WebhookRetryTask()
            };

            var names = new HashSet<string>();
            foreach (var task in tasks)
            {
                Assert.True(names.Add(task.Name),
                    "任务名称重复: " + task.Name);
            }
        }

        // ============================================================
        // GlobalExceptionMiddleware 构造测试
        // ============================================================

        [Fact]
        public void GlobalExceptionMiddleware_CanBeConstructed()
        {
            // 使用空委托验证构造不抛异常
            var middleware = new GlobalExceptionMiddleware((_) => System.Threading.Tasks.Task.CompletedTask);
            Assert.NotNull(middleware);
        }

        [Fact]
        public void RequestLoggingMiddleware_CanBeConstructed()
        {
            var middleware = new RequestLoggingMiddleware((_) => System.Threading.Tasks.Task.CompletedTask);
            Assert.NotNull(middleware);
        }

        [Fact]
        public void PermissionMiddleware_CanBeConstructed()
        {
            var middleware = new PermissionMiddleware((_) => System.Threading.Tasks.Task.CompletedTask);
            Assert.NotNull(middleware);
        }

        [Fact]
        public void RateLimitMiddleware_CanBeConstructed()
        {
            var middleware = new RateLimitMiddleware((_) => System.Threading.Tasks.Task.CompletedTask);
            Assert.NotNull(middleware);
        }

        [Fact]
        public void AuditMiddleware_CanBeConstructed()
        {
            var middleware = new AuditMiddleware((_) => System.Threading.Tasks.Task.CompletedTask);
            Assert.NotNull(middleware);
        }

        [Fact]
        public void RateLimitMiddleware_Configure_DoesNotThrow()
        {
            // 配置限流参数不应抛异常
            RateLimitMiddleware.Configure(200, 120);
        }
    }
}
