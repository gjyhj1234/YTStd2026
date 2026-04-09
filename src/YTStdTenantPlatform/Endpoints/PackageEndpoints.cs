using System;
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
    /// <summary>SaaS 套餐管理端点（套餐、版本、能力）</summary>
    public static class PackageEndpoints
    {
        /// <summary>注册 SaaS 套餐管理路由</summary>
        public static void Map(WebApplication app)
        {
            MapPackageEndpoints(app);
            MapVersionEndpoints(app);
            MapCapabilityEndpoints(app);
        }

        /// <summary>注册套餐路由</summary>
        private static void MapPackageEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/packages")
                .WithTags("SaaS 套餐管理");

            group.MapGet("/", async (HttpContext ctx, int? page, int? pageSize, string? keyword, string? status) =>
            {
                var user = GetCurrentUser(ctx);
                var req = new PagedRequest { Page = page ?? 1, PageSize = pageSize ?? 20, Keyword = keyword, Status = status };
                var result = await PackageAppService.GetPackageListAsync(0, user.UserId, req);
                await WriteJsonAsync(ctx, ApiResult<PagedResult<SaasPackageRepDTO>>.Ok(result));
            }).WithSummary("获取套餐分页列表");

            group.MapGet("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PackageAppService.GetPackageByIdAsync(0, user.UserId, id);
                if (result == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.ResourceNotFound), 404); return; }
                await WriteJsonAsync(ctx, ApiResult<SaasPackageRepDTO>.Ok(result));
            }).WithSummary("获取套餐详情");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<CreateSaasPackageReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PackageAppService.CreatePackageAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建套餐");

            group.MapPut("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<UpdateSaasPackageReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PackageAppService.UpdatePackageAsync(0, user.UserId, id, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("更新套餐");

            group.MapDelete("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PackageAppService.DeletePackageAsync(0, user.UserId, id);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("删除套餐");

            group.MapPut("/{id:long}/publish", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PackageAppService.PublishPackageAsync(0, user.UserId, id);
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("发布套餐");

            group.MapPut("/{id:long}/unpublish", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PackageAppService.UnpublishPackageAsync(0, user.UserId, id);
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("下架套餐");

            group.MapGet("/check-code-exists", async (HttpContext ctx, string code, long? excludeId) =>
            {
                var user = GetCurrentUser(ctx);
                var exists = await PackageAppService.CheckCodeExistsAsync(0, user.UserId, code ?? "", excludeId);
                await WriteJsonAsync(ctx, ApiResult<bool>.Ok(exists));
            }).WithSummary("检查套餐编码是否已存在");
        }

        /// <summary>注册套餐版本路由</summary>
        private static void MapVersionEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/packages/{packageId:long}/versions")
                .WithTags("SaaS 套餐管理");

            group.MapGet("/", async (HttpContext ctx, long packageId, int? page, int? pageSize, string? keyword) =>
            {
                var user = GetCurrentUser(ctx);
                var req = new PagedRequest { Page = page ?? 1, PageSize = pageSize ?? 20, Keyword = keyword };
                var result = await PackageAppService.GetVersionListAsync(0, user.UserId, packageId, req);
                await WriteJsonAsync(ctx, ApiResult<PagedResult<SaasPackageVersionRepDTO>>.Ok(result));
            }).WithSummary("获取套餐版本列表");

            group.MapPost("/", async (HttpContext ctx, long packageId) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<CreateSaasPackageVersionReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                req.PackageId = packageId;
                var result = await PackageAppService.CreateVersionAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建套餐版本");
        }

        /// <summary>注册套餐能力路由</summary>
        private static void MapCapabilityEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/package-versions/{packageVersionId:long}/capabilities")
                .WithTags("SaaS 套餐管理");

            group.MapGet("/", async (HttpContext ctx, long packageVersionId, int? page, int? pageSize, string? keyword) =>
            {
                var user = GetCurrentUser(ctx);
                var req = new PagedRequest { Page = page ?? 1, PageSize = pageSize ?? 20, Keyword = keyword };
                var result = await PackageAppService.GetCapabilityListAsync(0, user.UserId, packageVersionId, req);
                await WriteJsonAsync(ctx, ApiResult<PagedResult<SaasPackageCapabilityRepDTO>>.Ok(result));
            }).WithSummary("获取套餐能力列表");

            group.MapPost("/", async (HttpContext ctx, long packageVersionId) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<SaveSaasPackageCapabilityReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                req.PackageVersionId = packageVersionId;
                var result = await PackageAppService.SaveCapabilityAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建/更新套餐能力");
        }

        private static CurrentUser GetCurrentUser(HttpContext ctx) =>
            ctx.Items.TryGetValue(CurrentUser.HttpContextKey, out var u) && u is CurrentUser cu ? cu : CurrentUser.Anonymous;

        private static async Task WriteJsonAsync<T>(HttpContext ctx, T data, int statusCode = 200)
        {
            await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonResponseWriter.WriteAsync(ctx, data, statusCode);
        }
    }
}
