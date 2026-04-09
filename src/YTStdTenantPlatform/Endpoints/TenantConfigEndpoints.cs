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
    /// <summary>租户配置中心端点（系统配置、功能开关、参数）</summary>
    public static class TenantConfigEndpoints
    {
        /// <summary>注册租户配置路由</summary>
        public static void Map(WebApplication app)
        {
            MapSystemConfigEndpoints(app);
            MapFeatureFlagEndpoints(app);
            MapParameterEndpoints(app);
        }

        /// <summary>注册系统配置路由</summary>
        private static void MapSystemConfigEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/system-configs")
                .WithTags("系统配置");

            group.MapGet("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var list = await TenantConfigAppService.GetAllSystemConfigsAsync(0, user.UserId);
                await WriteJsonAsync(ctx, ApiResult<List<TenantSystemConfigRepDTO>>.Ok(list));
            }).WithSummary("获取所有系统配置");

            group.MapGet("/{key}", async (HttpContext ctx, string key) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await TenantConfigAppService.GetSystemConfigByKeyAsync(0, user.UserId, key);
                if (result == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.ConfigNotFound), 404); return; }
                await WriteJsonAsync(ctx, ApiResult<TenantSystemConfigRepDTO>.Ok(result));
            }).WithSummary("按键获取系统配置");

            group.MapPut("/{key}", async (HttpContext ctx, string key) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<UpdateTenantSystemConfigReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await TenantConfigAppService.UpdateSystemConfigByKeyAsync(0, user.UserId, key, req);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("按键更新系统配置");
        }

        /// <summary>注册功能开关路由</summary>
        private static void MapFeatureFlagEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/feature-flags")
                .WithTags("功能开关");

            group.MapGet("/", async (HttpContext ctx, int? page, int? pageSize, string? keyword, long? tenantRefId) =>
            {
                var user = GetCurrentUser(ctx);
                var list = await TenantConfigAppService.GetAllFeatureFlagsAsync(0, user.UserId);
                await WriteJsonAsync(ctx, ApiResult<List<TenantFeatureFlagRepDTO>>.Ok(list));
            }).WithSummary("获取功能开关列表");

            group.MapGet("/{key}", async (HttpContext ctx, string key) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await TenantConfigAppService.GetFeatureFlagByKeyAsync(0, user.UserId, key);
                if (result == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.FeatureFlagNotFound), 404); return; }
                await WriteJsonAsync(ctx, ApiResult<TenantFeatureFlagRepDTO>.Ok(result));
            }).WithSummary("按键获取功能开关");

            group.MapPut("/{key}", async (HttpContext ctx, string key) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<SaveTenantFeatureFlagReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await TenantConfigAppService.UpdateFeatureFlagByKeyAsync(0, user.UserId, key, req);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("按键更新功能开关");

            group.MapPut("/{key}/enable", async (HttpContext ctx, string key) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await TenantConfigAppService.EnableFeatureFlagByKeyAsync(0, user.UserId, key);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("启用功能开关");

            group.MapPut("/{key}/disable", async (HttpContext ctx, string key) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await TenantConfigAppService.DisableFeatureFlagByKeyAsync(0, user.UserId, key);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("禁用功能开关");
        }

        /// <summary>注册租户参数路由</summary>
        private static void MapParameterEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/tenant-parameters")
                .WithTags("租户参数");

            group.MapGet("/", async (HttpContext ctx, int? page, int? pageSize, string? keyword, long? tenantRefId) =>
            {
                var user = GetCurrentUser(ctx);
                var req = new PagedRequest { Page = page ?? 1, PageSize = pageSize ?? 20, Keyword = keyword };
                var result = await TenantConfigAppService.GetParameterListAsync(0, user.UserId, req, tenantRefId);
                await WriteJsonAsync(ctx, ApiResult<PagedResult<TenantParameterRepDTO>>.Ok(result));
            }).WithSummary("获取参数列表");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<SaveTenantParameterReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await TenantConfigAppService.SaveParameterAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建/更新参数");

            group.MapDelete("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await TenantConfigAppService.DeleteParameterAsync(0, user.UserId, id);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("删除参数");
        }

        private static CurrentUser GetCurrentUser(HttpContext ctx) =>
            ctx.Items.TryGetValue(CurrentUser.HttpContextKey, out var u) && u is CurrentUser cu ? cu : CurrentUser.Anonymous;

        private static async Task WriteJsonAsync<T>(HttpContext ctx, T data, int statusCode = 200)
        {
            await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonResponseWriter.WriteAsync(ctx, data, statusCode);
        }
    }
}
