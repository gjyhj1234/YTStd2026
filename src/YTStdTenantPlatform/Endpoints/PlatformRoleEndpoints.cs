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
    /// <summary>平台角色管理端点</summary>
    public static class PlatformRoleEndpoints
    {
        /// <summary>注册平台角色管理路由</summary>
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/platform-roles")
                .WithTags("平台角色管理");

            group.MapGet("/", async (HttpContext ctx, int? page, int? pageSize, string? keyword, string? status, string? sortField, string? sortOrder) =>
            {
                var user = GetCurrentUser(ctx);
                var req = new PagedRequest { Page = page ?? 1, PageSize = pageSize ?? 20, Keyword = keyword, Status = status, SortField = sortField, SortOrder = sortOrder };
                var result = await PlatformRoleAppService.GetListAsync(0, user.UserId, req);
                await WriteJsonAsync(ctx, ApiResult<PagedResult<PlatformRoleRepDTO>>.Ok(result));
            }).WithSummary("获取角色分页列表");

            group.MapGet("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformRoleAppService.GetByIdAsync(0, user.UserId, id);
                if (result == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.ResourceNotFound), 404); return; }
                await WriteJsonAsync(ctx, ApiResult<PlatformRoleRepDTO>.Ok(result));
            }).WithSummary("获取角色详情");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<CreatePlatformRoleReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformRoleAppService.CreateAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建角色");

            group.MapPut("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<UpdatePlatformRoleReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformRoleAppService.UpdateAsync(0, user.UserId, id, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, result, 400); return; }
                await WriteJsonAsync(ctx, result);
            }).WithSummary("更新角色");

            group.MapPut("/{id:long}/enable", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformRoleAppService.SetStatusAsync(0, user.UserId, id, "active");
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("启用角色");

            group.MapPut("/{id:long}/disable", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformRoleAppService.SetStatusAsync(0, user.UserId, id, "disabled");
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("禁用角色");

            group.MapPost("/{id:long}/permissions", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<RolePermissionBindReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformRoleAppService.BindPermissionsAsync(0, user.UserId, id, req);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("角色授权（绑定权限）");

            group.MapPost("/{id:long}/members", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<RoleMemberBindReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await PlatformRoleAppService.BindMembersAsync(0, user.UserId, id, req);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("角色成员管理（绑定用户）");

            group.MapDelete("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformRoleAppService.DeleteAsync(0, user.UserId, id);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("删除角色");

            group.MapGet("/{id:long}/permissions", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformRoleAppService.GetPermissionIdsAsync(0, user.UserId, id);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("获取角色已绑定的权限 ID 列表");

            group.MapGet("/{id:long}/members", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformRoleAppService.GetMemberIdsAsync(0, user.UserId, id);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("获取角色已绑定的用户 ID 列表");

            group.MapGet("/check-code-exists", async (HttpContext ctx, string? code) =>
            {
                if (string.IsNullOrWhiteSpace(code)) { await WriteJsonAsync(ctx, ApiResult<bool>.Ok(false)); return; }
                var user = GetCurrentUser(ctx);
                var result = await PlatformRoleAppService.CheckCodeExistsAsync(0, user.UserId, code);
                await WriteJsonAsync(ctx, result);
            }).WithSummary("检查角色编码是否存在");

            group.MapGet("/all", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await PlatformRoleAppService.GetAllAsync(0, user.UserId);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("获取全部角色（不分页）");
        }

        private static CurrentUser GetCurrentUser(HttpContext ctx) =>
            ctx.Items.TryGetValue(CurrentUser.HttpContextKey, out var u) && u is CurrentUser cu ? cu : CurrentUser.Anonymous;

        private static async Task WriteJsonAsync<T>(HttpContext ctx, T data, int statusCode = 200)
        {
            await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonResponseWriter.WriteAsync(ctx, data, statusCode);
        }
    }
}
