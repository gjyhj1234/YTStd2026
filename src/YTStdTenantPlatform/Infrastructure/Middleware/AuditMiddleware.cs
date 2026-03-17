using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using YTStdLogger.Core;
using YTStdTenantPlatform.Infrastructure.Auth;

namespace YTStdTenantPlatform.Infrastructure.Middleware
{
    /// <summary>审计记录中间件，记录写操作（POST/PUT/PATCH/DELETE）的审计信息</summary>
    public sealed class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>需要审计的 HTTP 方法</summary>
        private static readonly string[] AuditableMethods = new[] { "POST", "PUT", "PATCH", "DELETE" };

        /// <summary>构造审计记录中间件</summary>
        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>处理请求，对写操作记录审计日志</summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;

            if (!IsAuditableMethod(method))
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value ?? "/";
            var traceId = context.TraceIdentifier;
            var currentUser = context.Items.TryGetValue(CurrentUser.HttpContextKey, out var userObj) && userObj is CurrentUser cu
                ? cu
                : CurrentUser.Anonymous;

            Logger.Debug(0, currentUser.UserId,
                "[AuditMiddleware] 开始审计: " + method + " " + path +
                " User=" + currentUser.Username + " TraceId=" + traceId);

            await _next(context);

            var statusCode = context.Response.StatusCode;
            var success = statusCode >= 200 && statusCode < 300;

            Logger.Info(0, currentUser.UserId,
                "[AuditMiddleware] 审计记录: " + method + " " + path +
                " Status=" + statusCode +
                " Success=" + success +
                " User=" + currentUser.Username +
                " TraceId=" + traceId);

            // 异步写入审计日志表（后续阶段实现实际持久化）
            // 此处仅记录到日志系统，不阻塞请求
        }

        /// <summary>判断是否为需要审计的 HTTP 方法</summary>
        private static bool IsAuditableMethod(string method)
        {
            for (int i = 0; i < AuditableMethods.Length; i++)
            {
                if (string.Equals(AuditableMethods[i], method, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
