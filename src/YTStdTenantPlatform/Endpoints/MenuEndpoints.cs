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
    /// <summary>菜单管理端点</summary>
    public static class MenuEndpoints
    {
        /// <summary>注册菜单路由</summary>
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/menus")
                .WithTags("菜单管理");

            group.MapGet("/tree", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var tree = await MenuAppService.GetTreeAsync(0, user.UserId);
                await WriteJsonAsync(ctx, ApiResult<List<MenuRepDTO>>.Ok(tree));
            }).WithSummary("获取菜单树");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<CreateMenuReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await MenuAppService.CreateAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建菜单");

            group.MapPut("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<UpdateMenuReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await MenuAppService.UpdateAsync(0, user.UserId, id, req);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("更新菜单");

            group.MapDelete("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await MenuAppService.DeleteAsync(0, user.UserId, id);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("删除菜单");

            group.MapPut("/{id:long}/sort", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<MenuSortReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await MenuAppService.SetSortOrderAsync(0, user.UserId, id, req.SortOrder);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("调整菜单排序");

            group.MapPut("/{id:long}/enable", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await MenuAppService.SetEnabledAsync(0, user.UserId, id, true);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("启用菜单");

            group.MapPut("/{id:long}/disable", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await MenuAppService.SetEnabledAsync(0, user.UserId, id, false);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("禁用菜单");
        }

        private static CurrentUser GetCurrentUser(HttpContext ctx) =>
            ctx.Items.TryGetValue(CurrentUser.HttpContextKey, out var u) && u is CurrentUser cu ? cu : CurrentUser.Anonymous;

        private static async Task WriteJsonAsync<T>(HttpContext ctx, T data, int statusCode = 200)
        {
            await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonResponseWriter.WriteAsync(ctx, data, statusCode);
        }
    }
}
