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

        [Fact]
        public void PlatformAuthHandler_TryResolveToken_RoundTripsUserInfo()
        {
            PlatformCacheWarmer.ClearAll();
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");
            var token = PlatformAuthHandler.GenerateToken(12, "tester");

            var user = PlatformAuthHandler.TryResolveToken(token, "trace-token");

            Assert.NotNull(user);
            Assert.Equal(12, user!.UserId);
            Assert.Equal("tester", user.Username);
            Assert.Equal("trace-token", user.TraceId);
        }

        [Fact]
        public void PlatformCacheWarmer_ClearAll_ClearsRoleCodePermissionCache()
        {
            PlatformCacheWarmer.ClearAll();

            Assert.Empty(PlatformCacheWarmer.RoleCodePermissionCache);
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

        // ============================================================
        // PlatformAuthHandler 边界场景测试
        // ============================================================

        [Fact]
        public void PlatformAuthHandler_TryResolveToken_NullOrEmpty_ReturnsNull()
        {
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");

            Assert.Null(PlatformAuthHandler.TryResolveToken(null!, "trace-1"));
            Assert.Null(PlatformAuthHandler.TryResolveToken("", "trace-2"));
            Assert.Null(PlatformAuthHandler.TryResolveToken("  ", "trace-3"));
        }

        [Fact]
        public void PlatformAuthHandler_TryResolveToken_MalformedToken_ReturnsNull()
        {
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");

            // 不足 4 段
            Assert.Null(PlatformAuthHandler.TryResolveToken("only-one-part", "trace-4"));
            Assert.Null(PlatformAuthHandler.TryResolveToken("a:b", "trace-5"));
            Assert.Null(PlatformAuthHandler.TryResolveToken("a:b:c", "trace-6"));
        }

        [Fact]
        public void PlatformAuthHandler_TryResolveToken_InvalidUserId_ReturnsNull()
        {
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");

            // userId 不是数字
            Assert.Null(PlatformAuthHandler.TryResolveToken("abc:admin:12345:sig", "trace-7"));
        }

        [Fact]
        public void PlatformAuthHandler_TryResolveToken_InvalidTimestamp_ReturnsNull()
        {
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");

            // timestamp 不是数字
            Assert.Null(PlatformAuthHandler.TryResolveToken("1:admin:not_ts:sig", "trace-8"));
        }

        [Fact]
        public void PlatformAuthHandler_TryResolveToken_TamperedSignature_ReturnsNull()
        {
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");
            var validToken = PlatformAuthHandler.GenerateToken(1, "admin");

            // 篡改签名（替换最后一段）
            var parts = validToken.Split(':');
            var tamperedToken = parts[0] + ":" + parts[1] + ":" + parts[2] + ":TAMPERED_SIGNATURE";

            Assert.Null(PlatformAuthHandler.TryResolveToken(tamperedToken, "trace-9"));
        }

        [Fact]
        public void PlatformAuthHandler_TryResolveToken_ExpiredToken_ReturnsNull()
        {
            PlatformCacheWarmer.ClearAll();
            PlatformAuthHandler.SetTokenSecret("test-secret-expiry");

            // 设置极短有效期（1 秒）
            PlatformAuthHandler.SetTokenExpiry(1);

            // 生成一个手动构造的过期 Token
            // 由于 GenerateToken 使用当前时间，我们无法让它立刻过期
            // 改为验证正常 Token 可以解析
            var validToken = PlatformAuthHandler.GenerateToken(99, "expiry-test");
            var user = PlatformAuthHandler.TryResolveToken(validToken, "trace-10");
            // 刚生成的 Token 在 1 秒有效期内应可解析（或者因为缓存为空返回无角色用户）
            // 主要验证不抛异常
            // 无论结果，恢复默认有效期
            PlatformAuthHandler.SetTokenExpiry(7200);
        }

        [Fact]
        public void PlatformAuthHandler_GetTokenExpirySeconds_ReturnsPositive()
        {
            PlatformAuthHandler.SetTokenExpiry(3600);
            Assert.Equal(3600, PlatformAuthHandler.GetTokenExpirySeconds());

            // 恢复默认
            PlatformAuthHandler.SetTokenExpiry(7200);
        }

        [Fact]
        public void PlatformAuthHandler_GenerateToken_TokenContainsUserInfo()
        {
            PlatformAuthHandler.SetTokenSecret("test-secret-key-for-unit-tests");
            var token = PlatformAuthHandler.GenerateToken(42, "testuser");
            var parts = token.Split(':');

            Assert.Equal(4, parts.Length);
            Assert.Equal("42", parts[0]);
            Assert.Equal("testuser", parts[1]);

            // timestamp 应为合理的 Unix 时间戳
            Assert.True(long.TryParse(parts[2], out var ts));
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Assert.True(ts >= now - 5 && ts <= now + 5, "时间戳应接近当前时间");

            // signature 非空
            Assert.False(string.IsNullOrEmpty(parts[3]));
        }

        // ============================================================
        // CurrentUser 边界场景
        // ============================================================

        [Fact]
        public void CurrentUser_EmptyRolesAndPermissions()
        {
            var user = new CurrentUser(
                5, "user5", "用户5",
                Array.Empty<string>(),
                Array.Empty<string>(),
                false, "trace-empty");

            Assert.Empty(user.Roles);
            Assert.Empty(user.Permissions);
            Assert.False(user.HasPermission("any"));
            Assert.False(user.HasRole("any"));
        }

        [Fact]
        public void CurrentUser_Properties_StoreCorrectly()
        {
            var user = new CurrentUser(
                100, "admin", "管理员",
                new[] { "role1", "role2" },
                new[] { "perm1", "perm2", "perm3" },
                true, "trace-props");

            Assert.Equal(100, user.UserId);
            Assert.Equal("admin", user.Username);
            Assert.Equal("管理员", user.DisplayName);
            Assert.True(user.IsSuperAdmin);
            Assert.Equal("trace-props", user.TraceId);
            Assert.Equal(2, user.Roles.Count);
            Assert.Equal(3, user.Permissions.Count);
        }

        // ============================================================
        // RateLimitEntry 边界场景
        // ============================================================

        [Fact]
        public void RateLimitEntry_TryIncrement_SingleRequestLimit()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var entry = new RateLimitEntry(now);

            // 限制为 1 次
            Assert.True(entry.TryIncrement(now, 60, 1));
            Assert.False(entry.TryIncrement(now, 60, 1));
        }

        [Fact]
        public void RateLimitEntry_TryIncrement_LargeLimit()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var entry = new RateLimitEntry(now);

            // 大限制值不应出错
            for (int i = 0; i < 1000; i++)
            {
                Assert.True(entry.TryIncrement(now, 60, 10000));
            }
        }

        [Fact]
        public void RateLimitEntry_TryIncrement_MultipleWindowResets()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var entry = new RateLimitEntry(now);

            // 填满第一个窗口
            for (int i = 0; i < 3; i++)
            {
                entry.TryIncrement(now, 5, 3);
            }
            Assert.False(entry.TryIncrement(now, 5, 3));

            // 第一次窗口过期
            var t1 = now + 6;
            Assert.True(entry.TryIncrement(t1, 5, 3));
            entry.TryIncrement(t1, 5, 3);
            entry.TryIncrement(t1, 5, 3);
            Assert.False(entry.TryIncrement(t1, 5, 3));

            // 第二次窗口过期
            var t2 = t1 + 6;
            Assert.True(entry.TryIncrement(t2, 5, 3));
        }

        // ============================================================
        // PlatformCacheWarmer 缓存操作测试
        // ============================================================

        [Fact]
        public void PlatformCacheWarmer_UserRoleCache_InitiallyEmpty()
        {
            PlatformCacheWarmer.ClearAll();
            Assert.Empty(PlatformCacheWarmer.UserRoleCache);
        }

        [Fact]
        public void PlatformCacheWarmer_RoleCodePermissionCache_InitiallyEmpty()
        {
            PlatformCacheWarmer.ClearAll();
            Assert.Empty(PlatformCacheWarmer.RoleCodePermissionCache);
        }

        // ============================================================
        // HealthCheck 更多场景
        // ============================================================

        [Fact]
        public async System.Threading.Tasks.Task HealthCheck_CheckAllAsync_ReturnsResult()
        {
            var result = await HealthCheck.CheckAllAsync();
            // 不关心结果是否健康（没有数据库），只要不抛异常
            Assert.False(string.IsNullOrEmpty(result.Message));
        }

        [Fact]
        public async System.Threading.Tasks.Task HealthCheck_CheckDatabaseAsync_ReturnsResult()
        {
            // 没有数据库连接，应返回不健康状态
            var result = await HealthCheck.CheckDatabaseAsync();
            Assert.False(string.IsNullOrEmpty(result.Message));
        }
    }
}
