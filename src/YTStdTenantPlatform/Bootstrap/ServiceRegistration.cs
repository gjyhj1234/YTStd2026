using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YTStdTenantPlatform.Infrastructure.Middleware;
using YTStdTenantPlatform.Infrastructure.Scheduling;

namespace YTStdTenantPlatform.Bootstrap
{
    /// <summary>服务注册入口，集中管理所有服务和中间件的注册</summary>
    public static class ServiceRegistration
    {
        /// <summary>注册平台所需的所有服务</summary>
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            // 后台任务调度器
            builder.Services.AddHostedService<BackgroundTaskScheduler>();
        }

        /// <summary>配置中间件管道</summary>
        public static void ConfigureMiddleware(WebApplication app)
        {
            // 全局异常处理（最外层）
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // 请求日志 / TraceId
            app.UseMiddleware<RequestLoggingMiddleware>();

            // 限流
            app.UseMiddleware<RateLimitMiddleware>();

            // 权限认证
            app.UseMiddleware<PermissionMiddleware>();

            // 审计记录
            app.UseMiddleware<AuditMiddleware>();
        }
    }
}
