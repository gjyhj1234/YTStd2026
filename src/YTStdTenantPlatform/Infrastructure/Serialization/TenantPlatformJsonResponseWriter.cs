using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace YTStdTenantPlatform.Infrastructure.Serialization;

internal static class TenantPlatformJsonResponseWriter
{
    public static Task WriteAsync<T>(HttpContext context, T data, int statusCode = 200)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var typeInfo = TenantPlatformJsonSerializerContext.Default.GetTypeInfo(typeof(T));
        if (typeInfo == null)
        {
            throw new NotSupportedException("缺少 JSON 源生成元数据: " + typeof(T).FullName);
        }

        return JsonSerializer.SerializeAsync(context.Response.Body, (object?)data, typeInfo, context.RequestAborted);
    }
}
