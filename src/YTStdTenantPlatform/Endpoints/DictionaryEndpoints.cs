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
    /// <summary>数据字典端点</summary>
    public static class DictionaryEndpoints
    {
        /// <summary>注册数据字典路由</summary>
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/dictionaries")
                .WithTags("数据字典");

            group.MapGet("/types", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var list = await DictionaryAppService.GetTypeListAsync(0, user.UserId);
                await WriteJsonAsync(ctx, ApiResult<List<DictionaryTypeRepDTO>>.Ok(list));
            }).WithSummary("获取字典类型列表");

            group.MapGet("/types/{typeCode}", async (HttpContext ctx, string typeCode) =>
            {
                var user = GetCurrentUser(ctx);
                var list = await DictionaryAppService.GetByTypeAsync(0, user.UserId, typeCode);
                await WriteJsonAsync(ctx, ApiResult<List<DictionaryRepDTO>>.Ok(list));
            }).WithSummary("按类型获取字典项");

            group.MapPost("/", async (HttpContext ctx) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<CreateDictionaryReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await DictionaryAppService.CreateAsync(0, user.UserId, req);
                if (result.Code != 0) { await WriteJsonAsync(ctx, ApiResult.Fail(result.Code), 400); return; }
                ctx.Response.StatusCode = 201;
                await WriteJsonAsync(ctx, result);
            }).WithSummary("创建字典项");

            group.MapPut("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var req = await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonRequestReader.ReadAsync<UpdateDictionaryReqDTO>(ctx.Request, ctx.RequestAborted);
                if (req == null) { await WriteJsonAsync(ctx, ApiResult.Fail(ErrorCodes.InvalidRequestBody), 400); return; }
                var result = await DictionaryAppService.UpdateAsync(0, user.UserId, id, req);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("更新字典项");

            group.MapDelete("/{id:long}", async (HttpContext ctx, long id) =>
            {
                var user = GetCurrentUser(ctx);
                var result = await DictionaryAppService.DeleteAsync(0, user.UserId, id);
                await WriteJsonAsync(ctx, result, result.Code == 0 ? 200 : 400);
            }).WithSummary("删除字典项");
        }

        private static CurrentUser GetCurrentUser(HttpContext ctx) =>
            ctx.Items.TryGetValue(CurrentUser.HttpContextKey, out var u) && u is CurrentUser cu ? cu : CurrentUser.Anonymous;

        private static async Task WriteJsonAsync<T>(HttpContext ctx, T data, int statusCode = 200)
        {
            await YTStdTenantPlatform.Infrastructure.Serialization.TenantPlatformJsonResponseWriter.WriteAsync(ctx, data, statusCode);
        }
    }
}
