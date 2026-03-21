using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Auth;
using YTStdTenantPlatform.Infrastructure.Serialization;

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
                () => BuildAuditStartDebugMessage(method, path, currentUser.Username, traceId));

            await _next(context);

            var statusCode = context.Response.StatusCode;
            var success = statusCode >= 200 && statusCode < 300;

            Logger.Info(0, currentUser.UserId,
                BuildAuditRecordInfoMessage(method, path, statusCode, success, currentUser.Username, traceId));

            try
            {
                var auditLog = new AuditLog
                {
                    Id = await DB.GetNextLongIdAsync(),
                    AuditType = "http_request",
                    Severity = success ? "low" : "medium",
                    SubjectType = currentUser.UserId > 0 ? "platform_user" : "anonymous",
                    SubjectId = currentUser.UserId > 0 ? currentUser.UserId.ToString() : null,
                    ChangeSummary = BuildChangeSummary(method, path, statusCode, success, traceId, currentUser.Username, context.Connection.RemoteIpAddress?.ToString()),
                    ComplianceTag = "api_audit",
                    CreatedAt = DateTime.UtcNow
                };

                var insertResult = await AuditLogCRUD.InsertAsync(0, currentUser.UserId, auditLog);
                if (!insertResult.Success)
                {
                    Logger.Error(0, currentUser.UserId,
                        "[AuditMiddleware] 审计日志持久化失败: " + insertResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(0, currentUser.UserId,
                    "[AuditMiddleware] 审计日志持久化异常: " + ex.Message);
            }
        }

        private static string BuildChangeSummary(
            string method,
            string path,
            int statusCode,
            bool success,
            string traceId,
            string username,
            string? ipAddress)
        {
            return Utf8JsonWriterHelper.BuildString(
                (method, path, statusCode, success, traceId, username, ipAddress),
                static (writer, state) =>
                {
                    writer.WriteStartObject();
                    writer.WriteString("method", state.method);
                    writer.WriteString("path", state.path);
                    writer.WriteNumber("statusCode", state.statusCode);
                    writer.WriteBoolean("success", state.success);
                    writer.WriteString("traceId", state.traceId);
                    writer.WriteString("username", state.username);
                    writer.WriteString("ipAddress", state.ipAddress ?? string.Empty);
                    writer.WriteEndObject();
                });
        }

        private static string BuildAuditStartDebugMessage(string method, string path, string username, string traceId)
        {
            const string prefix = "[AuditMiddleware] 开始审计: ";
            const string userLabel = " User=";
            const string traceLabel = " TraceId=";
            return string.Create(
                prefix.Length + method.Length + 1 + path.Length + userLabel.Length + username.Length + traceLabel.Length + traceId.Length,
                (method, path, username, traceId),
                static (span, state) =>
                {
                    var offset = 0;
                    offset = CopyTo(span, offset, prefix);
                    offset = CopyTo(span, offset, state.method);
                    offset = CopyTo(span, offset, " ");
                    offset = CopyTo(span, offset, state.path);
                    offset = CopyTo(span, offset, userLabel);
                    offset = CopyTo(span, offset, state.username);
                    offset = CopyTo(span, offset, traceLabel);
                    CopyTo(span, offset, state.traceId);
                });
        }

        private static string BuildAuditRecordInfoMessage(
            string method,
            string path,
            int statusCode,
            bool success,
            string username,
            string traceId)
        {
            const string prefix = "[AuditMiddleware] 审计记录: ";
            const string statusLabel = " Status=";
            const string successLabel = " Success=";
            const string userLabel = " User=";
            const string traceLabel = " TraceId=";
            var statusText = statusCode.ToString();
            var successText = success ? "True" : "False";
            return string.Create(
                prefix.Length + method.Length + 1 + path.Length + statusLabel.Length + statusText.Length + successLabel.Length + successText.Length + userLabel.Length + username.Length + traceLabel.Length + traceId.Length,
                (method, path, statusText, successText, username, traceId),
                static (span, state) =>
                {
                    var offset = 0;
                    offset = CopyTo(span, offset, prefix);
                    offset = CopyTo(span, offset, state.method);
                    offset = CopyTo(span, offset, " ");
                    offset = CopyTo(span, offset, state.path);
                    offset = CopyTo(span, offset, statusLabel);
                    offset = CopyTo(span, offset, state.statusText);
                    offset = CopyTo(span, offset, successLabel);
                    offset = CopyTo(span, offset, state.successText);
                    offset = CopyTo(span, offset, userLabel);
                    offset = CopyTo(span, offset, state.username);
                    offset = CopyTo(span, offset, traceLabel);
                    CopyTo(span, offset, state.traceId);
                });
        }

        private static int CopyTo(Span<char> destination, int offset, string value)
        {
            value.AsSpan().CopyTo(destination[offset..]);
            return offset + value.Length;
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
