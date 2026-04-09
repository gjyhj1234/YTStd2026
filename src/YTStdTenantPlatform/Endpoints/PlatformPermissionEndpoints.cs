using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using YTStdTenantPlatform.Application.Constants;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Application.Services;
using YTStdTenantPlatform.Infrastructure.Auth;

namespace YTStdTenantPlatform.Endpoints
{
    /// <summary>平台权限端点</summary>
    public static class PlatformPermissionEndpoints
    {
        /// <summary>注册平台权限路由</summary>
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/platform-permissions")
                .WithTags("平台权限");

            group.MapGet("/tree", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var tree = await PlatformPermissionAppService.GetTreeAsync(0, user.UserId);
                await WriteJsonAsync(ctx, ApiResult<List<PlatformPermissionRepDTO>>.Ok(tree));
            }).WithSummary("获取权限树");

            group.MapGet("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var list = await PlatformPermissionAppService.GetFlatListAsync(0, user.UserId);
                await WriteJsonAsync(ctx, ApiResult<List<PlatformPermissionRepDTO>>.Ok(list));
            }).WithSummary("获取权限平铺列表");

            group.MapGet("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformPermissionAppService.GetByIdAsync(0, user.UserId, id);
                if (result == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.ResourceNotFound), 404); return; }
                await WriteJsonAsync(ctx, ApiResult<PlatformPermissionRepDTO>.Ok(result));
            }).WithSummary("获取权限详情");

            group.MapGet("/code/{code}", (HttpContext ctx, string code) =>
            {
                var result = PlatformPermissionAppService.GetByCode(code);
                if (result == null) { return WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.ResourceNotFound), 404); }
                return WriteJsonAsync(ctx, ApiResult<PlatformPermissionRepDTO>.Ok(result));
            }).WithSummary("按编码查询权限");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<CreatePlatformPermissionReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformPermissionAppService.CreateAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建权限");

            group.MapPut("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<UpdatePlatformPermissionReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformPermissionAppService.UpdateAsync(0, user.UserId, id, req);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("更新权限");

            group.MapDelete("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformPermissionAppService.DeleteAsync(0, user.UserId, id);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("删除权限");
        }

        private static CurrentUser GetCurrentUser(HttpContext ctx) =>
            ctx.Items.TryGetValue(CurrentUser.HttpContextKey, out var u) && u is CurrentUser cu ? cu : CurrentUser.Anonymous;

        private static async Task WriteJsonAsync<T>(HttpContext ctx, T data, int statusCode = 200)
        {
            await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonResponseWriter.WriteAsync(ctx, data, statusCode);
        }
    }
}
