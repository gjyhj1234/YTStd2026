using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Application.Constants;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>租户信息应用服务（分组、域名、标签）</summary>
    public static class TenantInfoAppService
    {
        // ──────────────────────────────────────────────────────
        // 租户分组
        // ──────────────────────────────────────────────────────

        /// <summary>获取租户分组树</summary>
        public static async ValueTask<List<TenantGroupRepDTO>> GetGroupTreeAsync(int tenantId, long operatorId)
        {
            var (result, data) = await TenantGroupCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<TenantGroupRepDTO>();
            return BuildGroupTree(data);
        }

        /// <summary>获取分组平铺列表</summary>
        public static async ValueTask<List<TenantGroupRepDTO>> GetGroupListAsync(int tenantId, long operatorId)
        {
            var (result, data) = await TenantGroupCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<TenantGroupRepDTO>();

            var list = new List<TenantGroupRepDTO>(data.Count);
            foreach (var g in data)
                list.Add(MapGroupToDto(g));
            return list;
        }

        /// <summary>创建租户分组</summary>
        public static async ValueTask<ApiResult<long>> CreateGroupAsync(
            int tenantId, long operatorId, CreateTenantGroupReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.GroupCode))
                return ApiResult<long>.Fail(ErrorCodes.GroupCodeRequired);

            // 唯一性前置校验
            var (chkResult, existing) = await TenantGroupCRUD.GetListAsync(tenantId, operatorId);
            if (chkResult.Success && existing != null)
            {
                foreach (var item in existing)
                {
                    if (string.Equals(item.GroupCode, req.GroupCode.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.GroupCodeExists);
                }
            }

            var group = new TenantGroup
            {
                Id = await DB.GetNextLongIdAsync(),
                GroupCode = req.GroupCode.Trim(),
                GroupName = req.GroupName.Trim(),
                Description = req.Description,
                ParentId = req.ParentId,
                CreatedBy = operatorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insResult = await TenantGroupCRUD.InsertAsync(tenantId, operatorId, group);
            if (!insResult.Success)
            {
                // 唯一性后置复核
                var (rechkResult, rechkData) = await TenantGroupCRUD.GetListAsync(tenantId, operatorId);
                if (rechkResult.Success && rechkData != null)
                {
                    foreach (var item in rechkData)
                    {
                        if (string.Equals(item.GroupCode, group.GroupCode, StringComparison.OrdinalIgnoreCase))
                            return ApiResult<long>.Fail(ErrorCodes.GroupCodeExists);
                    }
                }
                return ApiResult<long>.Fail(ErrorCodes.GroupCreateFailed);
            }

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 创建分组: " + req.GroupCode);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>更新租户分组</summary>
        public static async ValueTask<ApiResult> UpdateGroupAsync(
            int tenantId, long operatorId, long id, UpdateTenantGroupReqDTO req)
        {
            var (getResult, groups) = await TenantGroupCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || groups == null) return ApiResult.Fail(ErrorCodes.GroupQueryFailed);

            TenantGroup? target = null;
            foreach (var g in groups) { if (g.Id == id) { target = g; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.GroupNotFound);

            if (req.GroupName != null) target.GroupName = req.GroupName;
            if (req.Description != null) target.Description = req.Description;
            if (req.ParentId.HasValue) target.ParentId = req.ParentId;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await TenantGroupCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.GroupUpdateFailed);

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 更新分组: id=" + id);
            return ApiResult.Ok();
        }

        /// <summary>删除租户分组</summary>
        public static async ValueTask<ApiResult> DeleteGroupAsync(int tenantId, long operatorId, long id)
        {
            var delResult = await TenantGroupCRUD.DeleteAsync(tenantId, operatorId, id);
            if (!delResult.Success) return ApiResult.Fail(ErrorCodes.GroupDeleteFailed);

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 删除分组: id=" + id);
            return ApiResult.Ok();
        }

        // ──────────────────────────────────────────────────────
        // 租户域名
        // ──────────────────────────────────────────────────────

        /// <summary>获取租户域名列表</summary>
        public static async ValueTask<List<TenantDomainRepDTO>> GetDomainsAsync(
            int tenantId, long operatorId, long tenantRefId)
        {
            var (result, data) = await TenantDomainCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<TenantDomainRepDTO>();

            var list = new List<TenantDomainRepDTO>();
            foreach (var d in data)
            {
                if (d.TenantRefId == tenantRefId)
                    list.Add(MapDomainToDto(d));
            }
            return list;
        }

        /// <summary>创建租户域名</summary>
        public static async ValueTask<ApiResult<long>> CreateDomainAsync(
            int tenantId, long operatorId, CreateTenantDomainReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.Domain))
                return ApiResult<long>.Fail(ErrorCodes.DomainRequired);

            // 唯一性前置校验（域名全局唯一）
            var (chkResult, existingDomains) = await TenantDomainCRUD.GetListAsync(tenantId, operatorId);
            if (chkResult.Success && existingDomains != null)
            {
                foreach (var item in existingDomains)
                {
                    if (string.Equals(item.Domain, req.Domain.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.DomainExists);
                }
            }

            var domain = new TenantDomain
            {
                Id = await DB.GetNextLongIdAsync(),
                TenantRefId = req.TenantRefId,
                Domain = req.Domain.Trim(),
                DomainType = req.DomainType,
                IsPrimary = false,
                VerificationStatus = "pending",
                CreatedAt = DateTime.UtcNow
            };

            var insResult = await TenantDomainCRUD.InsertAsync(tenantId, operatorId, domain);
            if (!insResult.Success)
            {
                // 唯一性后置复核
                var (rechkResult, rechkData) = await TenantDomainCRUD.GetListAsync(tenantId, operatorId);
                if (rechkResult.Success && rechkData != null)
                {
                    foreach (var item in rechkData)
                    {
                        if (string.Equals(item.Domain, domain.Domain, StringComparison.OrdinalIgnoreCase))
                            return ApiResult<long>.Fail(ErrorCodes.DomainExists);
                    }
                }
                return ApiResult<long>.Fail(ErrorCodes.DomainCreateFailed);
            }

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 创建域名: " + req.Domain);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>删除租户域名</summary>
        public static async ValueTask<ApiResult> DeleteDomainAsync(
            int tenantId, long operatorId, long tenantRefId, long domainId)
        {
            var (getResult, domains) = await TenantDomainCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || domains == null) return ApiResult.Fail(ErrorCodes.DomainQueryFailed);

            TenantDomain? target = null;
            foreach (var d in domains) { if (d.Id == domainId) { target = d; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.DomainNotFound);
            if (target.TenantRefId != tenantRefId) return ApiResult.Fail(ErrorCodes.DomainNotFound);

            var delResult = await TenantDomainCRUD.DeleteAsync(tenantId, operatorId, domainId);
            if (!delResult.Success) return ApiResult.Fail(ErrorCodes.DomainDeleteFailed);

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 删除域名: id=" + domainId);
            return ApiResult.Ok();
        }

        // ──────────────────────────────────────────────────────
        // 租户标签
        // ──────────────────────────────────────────────────────

        /// <summary>获取标签列表</summary>
        public static async ValueTask<PagedResult<TenantTagRepDTO>> GetTagListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (result, data) = await TenantTagCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<TenantTagRepDTO> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<TenantTag>();
            foreach (var tag in data)
            {
                if (!string.IsNullOrEmpty(request.Keyword) &&
                    tag.TagKey.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    tag.TagValue.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                filtered.Add(tag);
            }

            var items = new List<TenantTagRepDTO>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
                items.Add(MapTagToDto(filtered[i]));

            return new PagedResult<TenantTagRepDTO>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>创建标签</summary>
        public static async ValueTask<ApiResult<long>> CreateTagAsync(
            int tenantId, long operatorId, CreateTenantTagReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.TagKey))
                return ApiResult<long>.Fail(ErrorCodes.TagKeyRequired);

            // 唯一性前置校验
            var (chkResult, existingTags) = await TenantTagCRUD.GetListAsync(tenantId, operatorId);
            if (chkResult.Success && existingTags != null)
            {
                foreach (var item in existingTags)
                {
                    if (string.Equals(item.TagKey, req.TagKey.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.TagKeyExists);
                }
            }

            var tag = new TenantTag
            {
                Id = await DB.GetNextLongIdAsync(),
                TagKey = req.TagKey.Trim(),
                TagValue = req.TagValue.Trim(),
                TagType = req.TagType,
                Description = req.Description,
                CreatedAt = DateTime.UtcNow
            };

            var insResult = await TenantTagCRUD.InsertAsync(tenantId, operatorId, tag);
            if (!insResult.Success)
            {
                // 唯一性后置复核
                var (rechkResult, rechkData) = await TenantTagCRUD.GetListAsync(tenantId, operatorId);
                if (rechkResult.Success && rechkData != null)
                {
                    foreach (var item in rechkData)
                    {
                        if (string.Equals(item.TagKey, tag.TagKey, StringComparison.OrdinalIgnoreCase))
                            return ApiResult<long>.Fail(ErrorCodes.TagKeyExists);
                    }
                }
                return ApiResult<long>.Fail(ErrorCodes.TagCreateFailed);
            }

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 创建标签: " + req.TagKey);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>删除标签</summary>
        public static async ValueTask<ApiResult> DeleteTagAsync(int tenantId, long operatorId, long id)
        {
            var delResult = await TenantTagCRUD.DeleteAsync(tenantId, operatorId, id);
            if (!delResult.Success) return ApiResult.Fail(ErrorCodes.TagDeleteFailed);

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 删除标签: id=" + id);
            return ApiResult.Ok();
        }

        /// <summary>标签绑定</summary>
        public static async ValueTask<ApiResult> BindTagsAsync(
            int tenantId, long operatorId, TagBindReqDTO req)
        {
            foreach (var tagId in req.TagIds)
            {
                var binding = new TenantTagBinding
                {
                    Id = await DB.GetNextLongIdAsync(),
                    TenantRefId = req.TenantRefId,
                    TagId = tagId,
                    CreatedAt = DateTime.UtcNow
                };
                await TenantTagBindingCRUD.InsertAsync(tenantId, operatorId, binding);
            }

            Logger.Info(tenantId, operatorId,
                "[TenantInfoAppService] 标签绑定: tenant=" + req.TenantRefId + " 标签数=" + req.TagIds.Length);
            return ApiResult.Ok();
        }

        /// <summary>设置租户标签（先删后插）</summary>
        public static async ValueTask<ApiResult> SetTenantTagsAsync(
            int tenantId, long operatorId, long tenantRefId, TagBindReqDTO req)
        {
            // 获取现有绑定并删除
            var (getResult, bindings) = await TenantTagBindingCRUD.GetListAsync(tenantId, operatorId);
            if (getResult.Success && bindings != null)
            {
                foreach (var b in bindings)
                {
                    if (b.TenantRefId == tenantRefId)
                        await TenantTagBindingCRUD.DeleteAsync(tenantId, operatorId, b.Id);
                }
            }

            // 插入新绑定
            foreach (var tagId in req.TagIds)
            {
                var binding = new TenantTagBinding
                {
                    Id = await DB.GetNextLongIdAsync(),
                    TenantRefId = tenantRefId,
                    TagId = tagId,
                    CreatedAt = DateTime.UtcNow
                };
                var insResult = await TenantTagBindingCRUD.InsertAsync(tenantId, operatorId, binding);
                if (!insResult.Success) return ApiResult.Fail(ErrorCodes.TenantTagSetFailed);
            }

            Logger.Info(tenantId, operatorId,
                "[TenantInfoAppService] 设置租户标签: tenant=" + tenantRefId + " 标签数=" + req.TagIds.Length);
            return ApiResult.Ok();
        }

        // ──────────────────────────────────────────────────────
        // Mapping helpers
        // ──────────────────────────────────────────────────────

        private static TenantGroupRepDTO MapGroupToDto(TenantGroup g)
        {
            return new TenantGroupRepDTO
            {
                Id = g.Id, GroupCode = g.GroupCode, GroupName = g.GroupName,
                Description = g.Description, ParentId = g.ParentId,
                CreatedAt = g.CreatedAt
            };
        }

        private static List<TenantGroupRepDTO> BuildGroupTree(IReadOnlyList<TenantGroup> data)
        {
            var allDtos = new Dictionary<long, TenantGroupRepDTO>(data.Count);
            foreach (var g in data)
            {
                var dto = MapGroupToDto(g);
                dto.Children = new List<TenantGroupRepDTO>();
                allDtos[g.Id] = dto;
            }

            var roots = new List<TenantGroupRepDTO>();
            foreach (var g in data)
            {
                var dto = allDtos[g.Id];
                if (g.ParentId == null || g.ParentId == 0)
                    roots.Add(dto);
                else if (allDtos.TryGetValue(g.ParentId.Value, out var parent))
                    parent.Children!.Add(dto);
            }
            return roots;
        }

        private static TenantDomainRepDTO MapDomainToDto(TenantDomain d)
        {
            return new TenantDomainRepDTO
            {
                Id = d.Id, TenantRefId = d.TenantRefId, Domain = d.Domain,
                DomainType = d.DomainType, IsPrimary = d.IsPrimary,
                VerificationStatus = d.VerificationStatus, CreatedAt = d.CreatedAt
            };
        }

        private static TenantTagRepDTO MapTagToDto(TenantTag t)
        {
            return new TenantTagRepDTO
            {
                Id = t.Id, TagKey = t.TagKey, TagValue = t.TagValue,
                TagType = t.TagType, Description = t.Description,
                CreatedAt = t.CreatedAt
            };
        }
    }
}
