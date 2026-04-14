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
    /// <summary>平台用户管理端点</summary>
    public static class PlatformUserEndpoints
    {
        /// <summary>注册平台用户管理路由</summary>
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/platform-users")
                .WithTags("平台用户管理");

            group.MapGet("/", async (HttpContext ctx, int? page, int? pageSize, string? keyword, string? status, long? roleId, DateTime? createdAtStart, DateTime? createdAtEnd) =>
            {
                var user = GetCurrentUser(ctx);
                var req = new PagedRequest { Page = page ?? 1, PageSize = pageSize ?? 20, Keyword = keyword, Status = status, RoleId = roleId, CreatedAtStart = createdAtStart, CreatedAtEnd = createdAtEnd };
                var result = await PlatformUserAppService.GetListAsync(0, user.UserId, req);
                await WriteJsonAsync(ctx, ApiResult<PagedResult<PlatformUserRepDTO>>.Ok(result));
            }).WithSummary("获取平台用户分页列表");

            group.MapGet("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformUserAppService.GetByIdAsync(0, user.UserId, id);
                if (result == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.ResourceNotFound), 404); return; }
                await WriteJsonAsync(ctx, ApiResult<PlatformUserRepDTO>.Ok(result));
            }).WithSummary("获取平台用户详情");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<CreatePlatformUserReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformUserAppService.CreateAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建平台用户");

            group.MapPut("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<UpdatePlatformUserReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformUserAppService.UpdateAsync(0, user.UserId, id, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("更新平台用户");

            group.MapPut("/{id:long}/enable", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformUserAppService.SetStatusAsync(0, user.UserId, id, "active");
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("启用平台用户");

            group.MapPut("/{id:long}/disable", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformUserAppService.SetStatusAsync(0, user.UserId, id, "disabled");
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("禁用平台用户");

            group.MapDelete("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformUserAppService.DeleteAsync(0, user.UserId, id);
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("删除平台用户");

            group.MapPut("/{id:long}/reset-password", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<ResetPasswordReqDTO>(ctx.Request, ctx.RequestAborted);
                var result = await PlatformUserAppService.ResetPasswordAsync(0, user.UserId, id, req?.NewPassword);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("重置密码");

            group.MapGet("/check-username-exists", async (HttpContext ctx, string username) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformUserAppService.CheckUsernameExistsAsync(0, user.UserId, username);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("检查用户名是否存在");

            group.MapPut("/batch-enable", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<BatchUserIdsReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null || req.Ids.Length == 0) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformUserAppService.BatchSetStatusAsync(0, user.UserId, req.Ids, Domain.Enums.PlatformUserStatus.Active);
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("批量启用用户");

            group.MapPut("/batch-disable", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<BatchUserIdsReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null || req.Ids.Length == 0) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformUserAppService.BatchSetStatusAsync(0, user.UserId, req.Ids, Domain.Enums.PlatformUserStatus.Disabled);
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("批量禁用用户");
        }

        private static CurrentUser GetCurrentUser(HttpContext ctx) =>
            ctx.Items.TryGetValue(CurrentUser.HttpContextKey, out var u) && u is CurrentUser cu ? cu : CurrentUser.Anonymous;

        private static async Task WriteJsonAsync<T>(HttpContext ctx, T data, int statusCode = 200)
        {
            await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonResponseWriter.WriteAsync(ctx, data, statusCode);
        }
    }
}
