using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using YTStdTenantPlatform.Infrastructure.Auth;
using YTStdTenantPlatform.Infrastructure.Persistence;

namespace YTStdTenantPlatform.Bootstrap
{
    /// <summary>路由注册入口，集中管理所有 API 路由组的注册</summary>
    public static class RouteRegistration
    {
        /// <summary>注册所有路由</summary>
        public static void MapRoutes(WebApplication app)
        {
            // 健康检查端点
            MapHealthEndpoints(app);

            // 认证端点
            MapAuthEndpoints(app);

            // ──────────────────────────────────────────────
            // 以下路由组为后续阶段实现的挂载点
            // 每个模块的具体 Endpoint 将在阶段 05/06 实现
            // ──────────────────────────────────────────────

            // 模块 1：平台管理体系
            // MapPlatformUserEndpoints(app);
            // MapPlatformRoleEndpoints(app);
            // MapPlatformPermissionEndpoints(app);

            // 模块 2：租户生命周期体系
            // MapTenantEndpoints(app);

            // 模块 6：SaaS 套餐系统
            // MapPackageEndpoints(app);

            // 模块 7：订阅系统
            // MapSubscriptionEndpoints(app);

            // 模块 8：计费与账单系统
            // MapBillingEndpoints(app);
        }

        /// <summary>注册健康检查端点</summary>
        private static void MapHealthEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/health")
                .WithTags("健康检查");

            group.MapGet("/", async (HttpContext ctx) =>
            {
                var result = await HealthCheck.CheckAllAsync();
                ctx.Response.ContentType = "application/json; charset=utf-8";
                if (result.IsHealthy)
                {
                    ctx.Response.StatusCode = 200;
                }
                else
                {
                    ctx.Response.StatusCode = 503;
                }
                await ctx.Response.WriteAsync(
                    "{\"status\":\"" + (result.IsHealthy ? "healthy" : "unhealthy") +
                    "\",\"message\":\"" + result.Message + "\"}");
            }).WithSummary("综合健康检查");

            group.MapGet("/db", async (HttpContext ctx) =>
            {
                var result = await HealthCheck.CheckDatabaseAsync();
                ctx.Response.ContentType = "application/json; charset=utf-8";
                if (result.IsHealthy)
                {
                    ctx.Response.StatusCode = 200;
                }
                else
                {
                    ctx.Response.StatusCode = 503;
                }
                await ctx.Response.WriteAsync(
                    "{\"status\":\"" + (result.IsHealthy ? "healthy" : "unhealthy") +
                    "\",\"message\":\"" + result.Message + "\"}");
            }).WithSummary("数据库健康检查");

            group.MapGet("/cache", async (HttpContext ctx) =>
            {
                var result = HealthCheck.CheckCache();
                ctx.Response.ContentType = "application/json; charset=utf-8";
                if (result.IsHealthy)
                {
                    ctx.Response.StatusCode = 200;
                }
                else
                {
                    ctx.Response.StatusCode = 503;
                }
                await ctx.Response.WriteAsync(
                    "{\"status\":\"" + (result.IsHealthy ? "healthy" : "unhealthy") +
                    "\",\"message\":\"" + result.Message + "\"}");
            }).WithSummary("缓存健康检查");
        }

        /// <summary>注册认证端点（登录/刷新 Token）</summary>
        private static void MapAuthEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/auth")
                .WithTags("认证");

            group.MapPost("/login", async (HttpContext context) =>
            {
                // 骨架实现：后续阶段接入实际的用户名密码验证
                // 1. 从请求体解析 username/password
                // 2. 查询 PlatformUser 验证密码哈希
                // 3. 检查用户状态（active/locked）
                // 4. 生成 Token 返回
                // 5. 记录登录日志

                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(
                    "{\"success\":true,\"message\":\"登录接口骨架，后续阶段实现\",\"token\":\"\"}");
            }).WithSummary("平台用户登录");

            group.MapPost("/refresh", async (HttpContext context) =>
            {
                // 骨架实现：后续阶段接入 Token 刷新逻辑
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(
                    "{\"success\":true,\"message\":\"Token刷新接口骨架，后续阶段实现\",\"token\":\"\"}");
            }).WithSummary("刷新访问令牌");

            group.MapGet("/me", async (HttpContext context) =>
            {
                var currentUser = context.Items.TryGetValue(CurrentUser.HttpContextKey, out var userObj) && userObj is CurrentUser cu
                    ? cu
                    : CurrentUser.Anonymous;

                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(
                    "{\"success\":true,\"data\":{" +
                    "\"userId\":" + currentUser.UserId +
                    ",\"username\":\"" + currentUser.Username +
                    "\",\"displayName\":\"" + currentUser.DisplayName +
                    "\",\"isSuperAdmin\":" + (currentUser.IsSuperAdmin ? "true" : "false") +
                    "}}");
            }).WithSummary("获取当前登录用户信息");
        }
    }
}
