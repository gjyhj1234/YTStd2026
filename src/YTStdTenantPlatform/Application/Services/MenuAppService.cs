using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Constants;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>菜单管理应用服务</summary>
    public static class MenuAppService
    {
        /// <summary>获取菜单树</summary>
        public static async ValueTask<List<MenuRepDTO>> GetTreeAsync(int tenantId, long operatorId)
        {
            var (result, data) = await PlatformMenuCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<MenuRepDTO>();
            return BuildTree(data);
        }

        /// <summary>创建菜单</summary>
        public static async ValueTask<ApiResult<long>> CreateAsync(int tenantId, long operatorId, CreateMenuReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.Code))
                return ApiResult<long>.Fail(ErrorCodes.MenuCodeRequired);
            if (string.IsNullOrWhiteSpace(req.Name))
                return ApiResult<long>.Fail(ErrorCodes.MenuNameRequired);

            var (listResult, existing) = await PlatformMenuCRUD.GetListAsync(tenantId, operatorId);
            if (listResult.Success && existing != null)
            {
                foreach (var m in existing)
                {
                    if (string.Equals(m.Code, req.Code.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.MenuCodeExists);
                }
            }

            var now = DateTime.UtcNow;
            var menu = new PlatformMenu
            {
                Id = await DB.GetNextLongIdAsync(),
                ParentId = req.ParentId,
                Code = req.Code.Trim(),
                Name = req.Name.Trim(),
                Icon = req.Icon,
                RoutePath = req.RoutePath,
                ComponentPath = req.ComponentPath,
                PermissionCode = req.PermissionCode,
                MenuType = req.MenuType,
                IsEnabled = true,
                IsExternal = req.IsExternal,
                IsVisible = req.IsVisible,
                SortOrder = req.SortOrder,
                Remark = req.Remark,
                CreatedBy = operatorId,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await PlatformMenuCRUD.InsertAsync(tenantId, operatorId, menu);
            if (!insResult.Success)
            {
                // 唯一性后置复核
                var (rechkResult, rechkData) = await PlatformMenuCRUD.GetListAsync(tenantId, operatorId);
                if (rechkResult.Success && rechkData != null)
                {
                    foreach (var item in rechkData)
                    {
                        if (string.Equals(item.Code, menu.Code, StringComparison.OrdinalIgnoreCase))
                            return ApiResult<long>.Fail(ErrorCodes.MenuCodeExists);
                    }
                }
                return ApiResult<long>.Fail(ErrorCodes.MenuCreateFailed);
            }

            Logger.Info(tenantId, operatorId, "[MenuAppService] 创建菜单: " + req.Code);
            return ApiResult<long>.Ok(menu.Id);
        }

        /// <summary>更新菜单</summary>
        public static async ValueTask<ApiResult> UpdateAsync(int tenantId, long operatorId, long id, UpdateMenuReqDTO req)
        {
            var (getResult, menus) = await PlatformMenuCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || menus == null) return ApiResult.Fail(ErrorCodes.MenuQueryFailed);

            PlatformMenu? target = null;
            foreach (var m in menus) { if (m.Id == id) { target = m; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.MenuNotFound);

            if (req.Name != null) target.Name = req.Name;
            if (req.Icon != null) target.Icon = req.Icon;
            if (req.RoutePath != null) target.RoutePath = req.RoutePath;
            if (req.ComponentPath != null) target.ComponentPath = req.ComponentPath;
            if (req.PermissionCode != null) target.PermissionCode = req.PermissionCode;
            if (req.MenuType.HasValue) target.MenuType = req.MenuType.Value;
            if (req.IsExternal.HasValue) target.IsExternal = req.IsExternal.Value;
            if (req.IsVisible.HasValue) target.IsVisible = req.IsVisible.Value;
            if (req.SortOrder.HasValue) target.SortOrder = req.SortOrder.Value;
            if (req.Remark != null) target.Remark = req.Remark;
            target.UpdatedBy = operatorId;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformMenuCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.MenuUpdateFailed);

            Logger.Info(tenantId, operatorId, "[MenuAppService] 更新菜单: " + target.Code);
            return ApiResult.Ok();
        }

        /// <summary>删除菜单</summary>
        public static async ValueTask<ApiResult> DeleteAsync(int tenantId, long operatorId, long id)
        {
            var delResult = await PlatformMenuCRUD.DeleteAsync(tenantId, operatorId, id);
            if (!delResult.Success) return ApiResult.Fail(ErrorCodes.MenuDeleteFailed);

            Logger.Info(tenantId, operatorId, "[MenuAppService] 删除菜单: id=" + id);
            return ApiResult.Ok();
        }

        /// <summary>调整排序</summary>
        public static async ValueTask<ApiResult> SetSortOrderAsync(int tenantId, long operatorId, long id, int sortOrder)
        {
            var (getResult, menus) = await PlatformMenuCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || menus == null) return ApiResult.Fail(ErrorCodes.MenuQueryFailed);

            PlatformMenu? target = null;
            foreach (var m in menus) { if (m.Id == id) { target = m; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.MenuNotFound);

            target.SortOrder = sortOrder;
            target.UpdatedBy = operatorId;
            target.UpdatedAt = DateTime.UtcNow;
            var updResult = await PlatformMenuCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.MenuUpdateFailed);

            Logger.Info(tenantId, operatorId, "[MenuAppService] 排序菜单: " + target.Code + " → " + sortOrder);
            return ApiResult.Ok();
        }

        /// <summary>设置菜单启用状态</summary>
        public static async ValueTask<ApiResult> SetEnabledAsync(int tenantId, long operatorId, long id, bool enabled)
        {
            var (getResult, menus) = await PlatformMenuCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || menus == null) return ApiResult.Fail(ErrorCodes.MenuQueryFailed);

            PlatformMenu? target = null;
            foreach (var m in menus) { if (m.Id == id) { target = m; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.MenuNotFound);

            target.IsEnabled = enabled;
            target.UpdatedBy = operatorId;
            target.UpdatedAt = DateTime.UtcNow;
            var updResult = await PlatformMenuCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.MenuUpdateFailed);

            Logger.Info(tenantId, operatorId, "[MenuAppService] " + (enabled ? "启用" : "禁用") + "菜单: " + target.Code);
            return ApiResult.Ok();
        }

        private static MenuRepDTO MapToDto(PlatformMenu m)
        {
            return new MenuRepDTO
            {
                Id = m.Id, ParentId = m.ParentId, Code = m.Code, Name = m.Name,
                Icon = m.Icon, RoutePath = m.RoutePath, ComponentPath = m.ComponentPath,
                PermissionCode = m.PermissionCode, MenuType = m.MenuType,
                IsEnabled = m.IsEnabled, IsExternal = m.IsExternal, IsVisible = m.IsVisible,
                SortOrder = m.SortOrder, Remark = m.Remark, CreatedAt = m.CreatedAt
            };
        }

        private static List<MenuRepDTO> BuildTree(IReadOnlyList<PlatformMenu> data)
        {
            var allDtos = new Dictionary<long, MenuRepDTO>(data.Count);
            foreach (var m in data)
            {
                var dto = MapToDto(m);
                dto.Children = new List<MenuRepDTO>();
                allDtos[m.Id] = dto;
            }
            var roots = new List<MenuRepDTO>();
            foreach (var m in data)
            {
                var dto = allDtos[m.Id];
                if (m.ParentId == 0)
                    roots.Add(dto);
                else if (allDtos.TryGetValue(m.ParentId, out var parent))
                    parent.Children!.Add(dto);
            }
            return roots;
        }
    }
}
