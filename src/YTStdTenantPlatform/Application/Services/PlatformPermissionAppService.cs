using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Cache;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>平台权限应用服务</summary>
    public static class PlatformPermissionAppService
    {
        /// <summary>获取权限树</summary>
        public static async ValueTask<List<PlatformPermissionDto>> GetTreeAsync(int tenantId, long operatorId)
        {
            var (result, data) = await PlatformPermissionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new List<PlatformPermissionDto>();

            return BuildTree(data);
        }

        /// <summary>获取权限平铺列表</summary>
        public static async ValueTask<List<PlatformPermissionDto>> GetFlatListAsync(int tenantId, long operatorId)
        {
            var (result, data) = await PlatformPermissionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new List<PlatformPermissionDto>();

            var list = new List<PlatformPermissionDto>(data.Count);
            foreach (var p in data)
                list.Add(MapToDto(p));
            return list;
        }

        /// <summary>获取权限详情</summary>
        public static async ValueTask<PlatformPermissionDto?> GetByIdAsync(int tenantId, long operatorId, long id)
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
        public static PlatformPermissionDto? GetByCode(string code)
        {
            var cache = PlatformCacheWarmer.PermissionCache;
            if (cache.TryGetValue(code, out var perm))
                return MapToDto(perm);
            return null;
        }

        /// <summary>构建权限树</summary>
        private static List<PlatformPermissionDto> BuildTree(IReadOnlyList<PlatformPermission> data)
        {
            var allDtos = new Dictionary<long, PlatformPermissionDto>(data.Count);
            foreach (var p in data)
            {
                var dto = MapToDto(p);
                dto.Children = new List<PlatformPermissionDto>();
                allDtos[p.Id] = dto;
            }

            var roots = new List<PlatformPermissionDto>();
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
        private static PlatformPermissionDto MapToDto(PlatformPermission p)
        {
            return new PlatformPermissionDto
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
