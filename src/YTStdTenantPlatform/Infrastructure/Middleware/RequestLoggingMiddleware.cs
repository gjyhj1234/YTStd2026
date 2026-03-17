using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using YTStdLogger.Core;
using YTStdTenantPlatform.Infrastructure.Auth;

namespace YTStdTenantPlatform.Infrastructure.Middleware
{
    /// <summary>请求日志与 TraceId 中间件，为每个请求生成 TraceId 并记录请求耗时</summary>
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>构造请求日志中间件</summary>
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>处理请求，注入 TraceId 并记录请求日志</summary>
        public async Task InvokeAsync(HttpContext context)
        {
            // 生成或复用 TraceId
            var traceId = context.TraceIdentifier;
            context.Response.Headers["X-Trace-Id"] = traceId;

            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? "/";
            var startTicks = Stopwatch.GetTimestamp();

            Logger.Debug(0, 0, "[Request] " + method + " " + path + " TraceId=" + traceId);

            await _next(context);

            var elapsedMs = GetElapsedMs(startTicks);
            var statusCode = context.Response.StatusCode;

            Logger.Info(0, 0, "[Request] " + method + " " + path +
                " Status=" + statusCode +
                " Elapsed=" + elapsedMs + "ms" +
                " TraceId=" + traceId);
        }

        /// <summary>计算耗时（毫秒）</summary>
        private static long GetElapsedMs(long startTicks)
        {
            var elapsed = Stopwatch.GetTimestamp() - startTicks;
            return elapsed * 1000 / Stopwatch.Frequency;
        }
    }
}
