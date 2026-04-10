using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Cache;
using YTStdTenantPlatform.Application.Constants;
using YTStdTenantPlatform.Domain.Enums;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>平台角色应用服务</summary>
    public static class PlatformRoleAppService
    {
        /// <summary>获取角色分页列表</summary>
        public static async ValueTask<PagedResult<PlatformRoleRepDTO>> GetListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (result, data) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<PlatformRoleRepDTO> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<PlatformRole>();
            foreach (var r in data)
            {
                if (!string.IsNullOrEmpty(request.Status) &&
                    (!Enum.TryParse<PlatformRoleStatus>(request.Status, true, out var statusFilter) ||
                     r.Status != (int)statusFilter))
                    continue;
                if (!string.IsNullOrEmpty(request.Keyword) &&
                    r.Name.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    r.Code.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                filtered.Add(r);
            }

            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            var items = new List<PlatformRoleRepDTO>();
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
            {
                var r = filtered[i];
                items.Add(new PlatformRoleRepDTO
                {
                    Id = r.Id, Code = r.Code, Name = r.Name,
                    Description = r.Description, Status = ((PlatformRoleStatus)r.Status).ToString(),
                    CreatedAt = r.CreatedAt
                });
            }

            return new PagedResult<PlatformRoleRepDTO>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>获取角色详情</summary>
        public static async ValueTask<PlatformRoleRepDTO?> GetByIdAsync(int tenantId, long operatorId, long id)
        {
            var (result, data) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return null;
            foreach (var r in data)
            {
                if (r.Id == id)
                    return new PlatformRoleRepDTO
                    {
                        Id = r.Id, Code = r.Code, Name = r.Name,
                        Description = r.Description, Status = ((PlatformRoleStatus)r.Status).ToString(),
                        CreatedAt = r.CreatedAt
                    };
            }
            return null;
        }

        /// <summary>创建角色</summary>
        public static async ValueTask<ApiResult<long>> CreateAsync(
            int tenantId, long operatorId, CreatePlatformRoleReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.Code))
                return ApiResult<long>.Fail(ErrorCodes.RoleCodeRequired);
            if (string.IsNullOrWhiteSpace(req.Name))
                return ApiResult<long>.Fail(ErrorCodes.RoleNameRequired);

            // 唯一性前置校验
            var (chkResult, existing) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
            if (chkResult.Success && existing != null)
            {
                foreach (var item in existing)
                {
                    if (string.Equals(item.Code, req.Code.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.RoleCodeExists);
                }
            }

            var now = DateTime.UtcNow;
            var role = new PlatformRole
            {
                Id = await DB.GetNextLongIdAsync(),
                Code = req.Code.Trim(),
                Name = req.Name.Trim(),
                Description = req.Description,
                Status = (int)PlatformRoleStatus.Active,
                CreatedBy = operatorId,
                UpdatedBy = operatorId,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await PlatformRoleCRUD.InsertAsync(tenantId, operatorId, role);
            if (!insResult.Success)
            {
                // 唯一性后置复核
                var (rechkResult, rechkData) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
                if (rechkResult.Success && rechkData != null)
                {
                    foreach (var item in rechkData)
                    {
                        if (string.Equals(item.Code, role.Code, StringComparison.OrdinalIgnoreCase))
                            return ApiResult<long>.Fail(ErrorCodes.RoleCodeExists);
                    }
                }
                return ApiResult<long>.Fail(ErrorCodes.RoleCreateFailed);
            }

            await PlatformCacheCoordinator.InvalidatePermissionsAsync();
            Logger.Info(tenantId, operatorId, "[PlatformRoleAppService] 创建角色: " + req.Code);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>更新角色</summary>
        public static async ValueTask<ApiResult> UpdateAsync(
            int tenantId, long operatorId, long id, UpdatePlatformRoleReqDTO req)
        {
            var (getResult, roles) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || roles == null) return ApiResult.Fail(ErrorCodes.RoleQueryFailed);

            PlatformRole? target = null;
            foreach (var r in roles) { if (r.Id == id) { target = r; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.RoleNotFound);

            if (req.Name != null) target.Name = req.Name;
            if (req.Description != null) target.Description = req.Description;
            target.UpdatedBy = operatorId;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformRoleCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.RoleUpdateFailed);

            await PlatformCacheCoordinator.InvalidatePermissionsAsync();
            Logger.Info(tenantId, operatorId, "[PlatformRoleAppService] 更新角色: " + target.Code);
            return ApiResult.Ok();
        }

        /// <summary>启用/禁用角色</summary>
        public static async ValueTask<ApiResult> SetStatusAsync(
            int tenantId, long operatorId, long id, string status)
        {
            var (getResult, roles) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || roles == null) return ApiResult.Fail(ErrorCodes.RoleQueryFailed);

            PlatformRole? target = null;
            foreach (var r in roles) { if (r.Id == id) { target = r; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.RoleNotFound);

            if (!Enum.TryParse<PlatformRoleStatus>(status, true, out var parsedStatus))
                return ApiResult.Fail(ErrorCodes.InvalidParameter);
            target.Status = (int)parsedStatus;
            target.UpdatedAt = DateTime.UtcNow;
            var updResult = await PlatformRoleCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.RoleStatusChangeFailed);

            await PlatformCacheCoordinator.InvalidatePermissionsAsync();
            Logger.Info(tenantId, operatorId,
                "[PlatformRoleAppService] 角色状态变更: " + target.Code + " → " + status);
            return ApiResult.Ok();
        }

        /// <summary>角色授权（绑定权限）</summary>
        public static async ValueTask<ApiResult> BindPermissionsAsync(
            int tenantId, long operatorId, long roleId, RolePermissionBindReqDTO req)
        {
            var now = DateTime.UtcNow;
            foreach (var permId in req.PermissionIds)
            {
                var rp = new PlatformRolePermission
                {
                    Id = await DB.GetNextLongIdAsync(),
                    RoleId = roleId,
                    PermissionId = permId,
                    GrantedBy = operatorId,
                    GrantedAt = now
                };
                await PlatformRolePermissionCRUD.InsertAsync(tenantId, operatorId, rp);
            }

            await PlatformCacheCoordinator.InvalidatePermissionsAsync();
            Logger.Info(tenantId, operatorId,
                "[PlatformRoleAppService] 角色授权: roleId=" + roleId + " 权限数=" + req.PermissionIds.Length);
            return ApiResult.Ok();
        }

        /// <summary>角色成员管理（绑定用户）</summary>
        public static async ValueTask<ApiResult> BindMembersAsync(
            int tenantId, long operatorId, long roleId, RoleMemberBindReqDTO req)
        {
            var now = DateTime.UtcNow;
            foreach (var userId in req.UserIds)
            {
                var rm = new PlatformRoleMember
                {
                    Id = await DB.GetNextLongIdAsync(),
                    RoleId = roleId,
                    UserId = userId,
                    AssignedBy = operatorId,
                    AssignedAt = now
                };
                await PlatformRoleMemberCRUD.InsertAsync(tenantId, operatorId, rm);
            }

            await PlatformCacheCoordinator.InvalidateUserRolesAsync();
            Logger.Info(tenantId, operatorId,
                "[PlatformRoleAppService] 角色成员: roleId=" + roleId + " 用户数=" + req.UserIds.Length);
            return ApiResult.Ok();
        }

        /// <summary>删除角色</summary>
        public static async ValueTask<ApiResult> DeleteAsync(int tenantId, long operatorId, long id)
        {
            var (getResult, roles) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || roles == null) return ApiResult.Fail(ErrorCodes.RoleQueryFailed);

            PlatformRole? target = null;
            foreach (var r in roles) { if (r.Id == id) { target = r; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.RoleNotFound);

            if (string.Equals(target.Code, "super-admin", StringComparison.OrdinalIgnoreCase))
                return ApiResult.Fail(ErrorCodes.RoleCannotDeleteSuperAdmin);

            var delResult = await PlatformRoleCRUD.DeleteAsync(tenantId, operatorId, target.Id);
            if (!delResult.Success) return ApiResult.Fail(ErrorCodes.RoleDeleteFailed);

            await PlatformCacheCoordinator.InvalidatePermissionsAsync();
            Logger.Info(tenantId, operatorId, "[PlatformRoleAppService] 删除角色: " + target.Code);
            return ApiResult.Ok();
        }

        /// <summary>获取角色已绑定的权限 ID 列表</summary>
        public static async ValueTask<ApiResult<List<long>>> GetPermissionIdsAsync(int tenantId, long operatorId, long roleId)
        {
            var (result, data) = await PlatformRolePermissionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return ApiResult<List<long>>.Fail(ErrorCodes.RoleQueryFailed);

            var ids = new List<long>();
            foreach (var rp in data)
            {
                if (rp.RoleId == roleId)
                    ids.Add(rp.PermissionId);
            }
            return ApiResult<List<long>>.Ok(ids);
        }

        /// <summary>检查角色编码是否存在</summary>
        public static async ValueTask<ApiResult<bool>> CheckCodeExistsAsync(int tenantId, long operatorId, string code)
        {
            var (result, data) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return ApiResult<bool>.Ok(false);

            foreach (var r in data)
            {
                if (string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase))
                    return ApiResult<bool>.Ok(true);
            }
            return ApiResult<bool>.Ok(false);
        }

        /// <summary>获取全部角色（不分页，用于下拉选择）</summary>
        public static async ValueTask<ApiResult<List<PlatformRoleSimpleRepDTO>>> GetAllAsync(int tenantId, long operatorId)
        {
            var (result, data) = await PlatformRoleCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return ApiResult<List<PlatformRoleSimpleRepDTO>>.Fail(ErrorCodes.RoleQueryFailed);

            var list = new List<PlatformRoleSimpleRepDTO>(data.Count);
            foreach (var r in data)
            {
                list.Add(new PlatformRoleSimpleRepDTO
                {
                    Id = r.Id,
                    Code = r.Code,
                    Name = r.Name,
                    Status = ((PlatformRoleStatus)r.Status).ToString()
                });
            }
            return ApiResult<List<PlatformRoleSimpleRepDTO>>.Ok(list);
        }
    }
}
