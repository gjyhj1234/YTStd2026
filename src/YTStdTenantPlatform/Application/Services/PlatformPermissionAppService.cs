using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Cache;
using YTStdTenantPlatform.Application.Constants;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>平台权限应用服务</summary>
    public static class PlatformPermissionAppService
    {
        /// <summary>获取权限树</summary>
        public static async ValueTask<List<PlatformPermissionRepDTO>> GetTreeAsync(int tenantId, long operatorId)
        {
            var (result, data) = await PlatformPermissionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new List<PlatformPermissionRepDTO>();

            return BuildTree(data);
        }

        /// <summary>获取权限平铺列表</summary>
        public static async ValueTask<List<PlatformPermissionRepDTO>> GetFlatListAsync(int tenantId, long operatorId)
        {
            var (result, data) = await PlatformPermissionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new List<PlatformPermissionRepDTO>();

            var list = new List<PlatformPermissionRepDTO>(data.Count);
            foreach (var p in data)
                list.Add(MapToDto(p));
            return list;
        }

        /// <summary>获取权限详情</summary>
        public static async ValueTask<PlatformPermissionRepDTO?> GetByIdAsync(int tenantId, long operatorId, long id)
        {
            var cache = PlatformCacheWarmer.PermissionCache;
            foreach (var kvp in cache)
            {
                if (kvp.Value.Id == id)
                    return MapToDto(kvp.Value);
            }

            var (result, data) = await PlatformPermissionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return null;
            foreach (var p in data)
            {
                if (p.Id == id)
                    return MapToDto(p);
            }
            return null;
        }

        /// <summary>按权限编码查询</summary>
        public static PlatformPermissionRepDTO? GetByCode(string code)
        {
            var cache = PlatformCacheWarmer.PermissionCache;
            if (cache.TryGetValue(code, out var perm))
                return MapToDto(perm);
            return null;
        }

        /// <summary>创建权限</summary>
        public static async ValueTask<ApiResult<long>> CreateAsync(
            int tenantId, long operatorId, CreatePlatformPermissionReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.Code))
                return ApiResult<long>.Fail(ErrorCodes.PermissionCodeRequired);

            // 检查编码唯一性
            var (chkResult, existing) = await PlatformPermissionCRUD.GetListAsync(tenantId, operatorId);
            if (chkResult.Success && existing != null)
            {
                foreach (var p in existing)
                {
                    if (string.Equals(p.Code, req.Code.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.PermissionCodeExists);
                }
            }

            var now = DateTime.UtcNow;
            var perm = new PlatformPermission
            {
                Id = await DB.GetNextLongIdAsync(),
                Code = req.Code.Trim(),
                Name = req.Name?.Trim() ?? "",
                PermissionType = req.PermissionType ?? "",
                ParentId = req.ParentId,
                Resource = req.Resource,
                Action = req.Action,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await PlatformPermissionCRUD.InsertAsync(tenantId, operatorId, perm);
            if (!insResult.Success)
                return ApiResult<long>.Fail(ErrorCodes.PermissionCreateFailed);

            await PlatformCacheCoordinator.InvalidatePermissionsAsync();
            Logger.Info(tenantId, operatorId, "[PlatformPermissionAppService] 创建权限: " + req.Code);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>更新权限</summary>
        public static async ValueTask<ApiResult> UpdateAsync(
            int tenantId, long operatorId, long id, UpdatePlatformPermissionReqDTO req)
        {
            var (getResult, perms) = await PlatformPermissionCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || perms == null) return ApiResult.Fail(ErrorCodes.PermissionQueryFailed);

            PlatformPermission? target = null;
            foreach (var p in perms) { if (p.Id == id) { target = p; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.PermissionNotFound);

            if (req.Name != null) target.Name = req.Name;
            if (req.Resource != null) target.Resource = req.Resource;
            if (req.Action != null) target.Action = req.Action;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformPermissionCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.PermissionUpdateFailed);

            await PlatformCacheCoordinator.InvalidatePermissionsAsync();
            Logger.Info(tenantId, operatorId, "[PlatformPermissionAppService] 更新权限: " + target.Code);
            return ApiResult.Ok();
        }

        /// <summary>删除权限</summary>
        public static async ValueTask<ApiResult> DeleteAsync(int tenantId, long operatorId, long id)
        {
            var (getResult, perms) = await PlatformPermissionCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || perms == null) return ApiResult.Fail(ErrorCodes.PermissionQueryFailed);

            PlatformPermission? target = null;
            foreach (var p in perms) { if (p.Id == id) { target = p; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.PermissionNotFound);

            var delResult = await PlatformPermissionCRUD.DeleteAsync(tenantId, operatorId, target.Id);
            if (!delResult.Success) return ApiResult.Fail(ErrorCodes.PermissionDeleteFailed);

            await PlatformCacheCoordinator.InvalidatePermissionsAsync();
            Logger.Info(tenantId, operatorId, "[PlatformPermissionAppService] 删除权限: " + target.Code);
            return ApiResult.Ok();
        }

        /// <summary>构建权限树</summary>
        private static List<PlatformPermissionRepDTO> BuildTree(IReadOnlyList<PlatformPermission> data)
        {
            var allDtos = new Dictionary<long, PlatformPermissionRepDTO>(data.Count);
            foreach (var p in data)
            {
                var dto = MapToDto(p);
                dto.Children = new List<PlatformPermissionRepDTO>();
                allDtos[p.Id] = dto;
            }

            var roots = new List<PlatformPermissionRepDTO>();
            foreach (var p in data)
            {
                var dto = allDtos[p.Id];
                if (p.ParentId == null || p.ParentId == 0)
                {
                    roots.Add(dto);
                }
                else if (allDtos.TryGetValue(p.ParentId.Value, out var parent))
                {
                    parent.Children!.Add(dto);
                }
            }
            return roots;
        }

        /// <summary>映射实体到 DTO</summary>
        private static PlatformPermissionRepDTO MapToDto(PlatformPermission p)
        {
            return new PlatformPermissionRepDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                PermissionType = p.PermissionType,
                ParentId = p.ParentId,
                Path = p.Path,
                Method = p.Method
            };
        }
    }
}
