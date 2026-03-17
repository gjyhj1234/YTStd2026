using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>租户信息应用服务（分组、域名、标签）</summary>
    public static class TenantInfoAppService
    {
        // ──────────────────────────────────────────────────────
        // 租户分组
        // ──────────────────────────────────────────────────────

        /// <summary>获取租户分组树</summary>
        public static async ValueTask<List<TenantGroupDto>> GetGroupTreeAsync(int tenantId, long operatorId)
        {
            var (result, data) = await TenantGroupCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<TenantGroupDto>();
            return BuildGroupTree(data);
        }

        /// <summary>获取分组平铺列表</summary>
        public static async ValueTask<List<TenantGroupDto>> GetGroupListAsync(int tenantId, long operatorId)
        {
            var (result, data) = await TenantGroupCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<TenantGroupDto>();

            var list = new List<TenantGroupDto>(data.Count);
            foreach (var g in data)
                list.Add(MapGroupToDto(g));
            return list;
        }

        /// <summary>创建租户分组</summary>
        public static async ValueTask<ApiResult<long>> CreateGroupAsync(
            int tenantId, long operatorId, CreateTenantGroupRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.GroupCode))
                return ApiResult<long>.Fail("分组编码不能为空");

            var group = new TenantGroup
            {
                GroupCode = req.GroupCode.Trim(),
                GroupName = req.GroupName.Trim(),
                Description = req.Description,
                ParentId = req.ParentId,
                CreatedBy = operatorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insResult = await TenantGroupCRUD.InsertAsync(tenantId, operatorId, group);
            if (!insResult.Success) return ApiResult<long>.Fail("创建分组失败: " + insResult.ErrorMessage);

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 创建分组: " + req.GroupCode);
            return ApiResult<long>.Ok(insResult.Id);
        }

        // ──────────────────────────────────────────────────────
        // 租户域名
        // ──────────────────────────────────────────────────────

        /// <summary>获取租户域名列表</summary>
        public static async ValueTask<List<TenantDomainDto>> GetDomainsAsync(
            int tenantId, long operatorId, long tenantRefId)
        {
            var (result, data) = await TenantDomainCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<TenantDomainDto>();

            var list = new List<TenantDomainDto>();
            foreach (var d in data)
            {
                if (d.TenantRefId == tenantRefId)
                    list.Add(MapDomainToDto(d));
            }
            return list;
        }

        /// <summary>创建租户域名</summary>
        public static async ValueTask<ApiResult<long>> CreateDomainAsync(
            int tenantId, long operatorId, CreateTenantDomainRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Domain))
                return ApiResult<long>.Fail("域名不能为空");

            var domain = new TenantDomain
            {
                TenantRefId = req.TenantRefId,
                Domain = req.Domain.Trim(),
                DomainType = req.DomainType,
                IsPrimary = false,
                VerificationStatus = "pending",
                CreatedAt = DateTime.UtcNow
            };

            var insResult = await TenantDomainCRUD.InsertAsync(tenantId, operatorId, domain);
            if (!insResult.Success) return ApiResult<long>.Fail("创建域名失败: " + insResult.ErrorMessage);

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 创建域名: " + req.Domain);
            return ApiResult<long>.Ok(insResult.Id);
        }

        // ──────────────────────────────────────────────────────
        // 租户标签
        // ──────────────────────────────────────────────────────

        /// <summary>获取标签列表</summary>
        public static async ValueTask<PagedResult<TenantTagDto>> GetTagListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (result, data) = await TenantTagCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<TenantTagDto> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<TenantTag>();
            foreach (var tag in data)
            {
                if (!string.IsNullOrEmpty(request.Keyword) &&
                    tag.TagKey.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    tag.TagValue.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                filtered.Add(tag);
            }

            var items = new List<TenantTagDto>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
                items.Add(MapTagToDto(filtered[i]));

            return new PagedResult<TenantTagDto>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>创建标签</summary>
        public static async ValueTask<ApiResult<long>> CreateTagAsync(
            int tenantId, long operatorId, CreateTenantTagRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.TagKey))
                return ApiResult<long>.Fail("标签键不能为空");

            var tag = new TenantTag
            {
                TagKey = req.TagKey.Trim(),
                TagValue = req.TagValue.Trim(),
                TagType = req.TagType,
                Description = req.Description,
                CreatedAt = DateTime.UtcNow
            };

            var insResult = await TenantTagCRUD.InsertAsync(tenantId, operatorId, tag);
            if (!insResult.Success) return ApiResult<long>.Fail("创建标签失败: " + insResult.ErrorMessage);

            Logger.Info(tenantId, operatorId, "[TenantInfoAppService] 创建标签: " + req.TagKey);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>标签绑定</summary>
        public static async ValueTask<ApiResult> BindTagsAsync(
            int tenantId, long operatorId, TagBindRequest req)
        {
            foreach (var tagId in req.TagIds)
            {
                var binding = new TenantTagBinding
                {
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

        // ──────────────────────────────────────────────────────
        // Mapping helpers
        // ──────────────────────────────────────────────────────

        private static TenantGroupDto MapGroupToDto(TenantGroup g)
        {
            return new TenantGroupDto
            {
                Id = g.Id, GroupCode = g.GroupCode, GroupName = g.GroupName,
                Description = g.Description, ParentId = g.ParentId,
                CreatedAt = g.CreatedAt
            };
        }

        private static List<TenantGroupDto> BuildGroupTree(IReadOnlyList<TenantGroup> data)
        {
            var allDtos = new Dictionary<long, TenantGroupDto>(data.Count);
            foreach (var g in data)
            {
                var dto = MapGroupToDto(g);
                dto.Children = new List<TenantGroupDto>();
                allDtos[g.Id] = dto;
            }

            var roots = new List<TenantGroupDto>();
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

        private static TenantDomainDto MapDomainToDto(TenantDomain d)
        {
            return new TenantDomainDto
            {
                Id = d.Id, TenantRefId = d.TenantRefId, Domain = d.Domain,
                DomainType = d.DomainType, IsPrimary = d.IsPrimary,
                VerificationStatus = d.VerificationStatus, CreatedAt = d.CreatedAt
            };
        }

        private static TenantTagDto MapTagToDto(TenantTag t)
        {
            return new TenantTagDto
            {
                Id = t.Id, TagKey = t.TagKey, TagValue = t.TagValue,
                TagType = t.TagType, Description = t.Description,
                CreatedAt = t.CreatedAt
            };
        }
    }
}
