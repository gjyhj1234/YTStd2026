using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Application.Services;
using YTStdTenantPlatform.Infrastructure.Auth;

namespace YTStdTenantPlatform.Endpoints
{
    /// <summary>租户信息端点（分组、域名、标签）</summary>
    public static class TenantInfoEndpoints
    {
        /// <summary>注册租户信息路由</summary>
        public static void Map(WebApplication app)
        {
            MapGroupEndpoints(app);
            MapDomainEndpoints(app);
            MapTagEndpoints(app);
        }

        /// <summary>注册租户分组路由</summary>
        private static void MapGroupEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/tenant-groups")
                .WithTags("租户分组");

            group.MapGet("/tree", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var tree = await TenantInfoAppService.GetGroupTreeAsync(0, user.UserId);
                await WriteJsonAsync(ctx, ApiResult<List<TenantGroupDto>>.Ok(tree));
            }).WithSummary("获取分组树");

            group.MapGet("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var list = await TenantInfoAppService.GetGroupListAsync(0, user.UserId);
                await WriteJsonAsync(ctx, ApiResult<List<TenantGroupDto>>.Ok(list));
            }).WithSummary("获取分组平铺列表");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await ctx.Request.ReadFromJsonAsync<CreateTenantGroupRequest>();
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail("请求体无效"), 400); return; }
                var result = await TenantInfoAppService.CreateGroupAsync(0, user.UserId, req);
                if (!result.Success) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Message), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建租户分组");
        }

        /// <summary>注册租户域名路由</summary>
        private static void MapDomainEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/tenant-domains")
                .WithTags("租户域名");

            group.MapGet("/", async (HttpContext ctx, long tenantRefId) =>
            {
                var user = GetCurrentUser(ctx);
                var list = await TenantInfoAppService.GetDomainsAsync(0, user.UserId, tenantRefId);
                await WriteJsonAsync(ctx, ApiResult<List<TenantDomainDto>>.Ok(list));
            }).WithSummary("获取租户域名列表");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await ctx.Request.ReadFromJsonAsync<CreateTenantDomainRequest>();
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail("请求体无效"), 400); return; }
                var result = await TenantInfoAppService.CreateDomainAsync(0, user.UserId, req);
                if (!result.Success) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Message), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建租户域名");
        }

        /// <summary>注册租户标签路由</summary>
        private static void MapTagEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/tenant-tags")
                .WithTags("租户标签");

            group.MapGet("/", async (HttpContext ctx, int? page, int? pageSize, string? keyword) =>
            {
                var user = GetCurrentUser(ctx);
                var req = new PagedRequest { Page = page ?? 1, PageSize = pageSize ?? 20, Keyword = keyword };
                var result = await TenantInfoAppService.GetTagListAsync(0, user.UserId, req);
                await WriteJsonAsync(ctx, ApiResult<PagedResult<TenantTagDto>>.Ok(result));
            }).WithSummary("获取标签列表");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await ctx.Request.ReadFromJsonAsync<CreateTenantTagRequest>();
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail("请求体无效"), 400); return; }
                var result = await TenantInfoAppService.CreateTagAsync(0, user.UserId, req);
                if (!result.Success) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Message), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建标签");

            group.MapPost("/bind", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await ctx.Request.ReadFromJsonAsync<TagBindRequest>();
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail("请求体无效"), 400); return; }
                var result = await TenantInfoAppService.BindTagsAsync(0, user.UserId, req);
                await WriteJsonAsync(ctx, result, result.Success ? 200 : 400);
            }).WithSummary("批量绑定标签");
        }

        private static CurrentUser GetCurrentUser(HttpContext ctx) =>
            ctx.Items.TryGetValue(CurrentUser.HttpContextKey, out var u) && u is CurrentUser cu ? cu : CurrentUser.Anonymous;

        private static async Task WriteJsonAsync<T>(HttpContext ctx, T data, int statusCode = 200)
        {
            ctx.Response.StatusCode = statusCode;
            ctx.Response.ContentType = "application/json; charset=utf-8";
            await System.Text.Json.JsonSerializer.SerializeAsync(ctx.Response.Body, data);
        }
    }
}
