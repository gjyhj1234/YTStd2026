using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Cache;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>租户配置中心应用服务（系统配置、功能开关、参数）</summary>
    public static class TenantConfigAppService
    {
        // ──────────────────────────────────────────────────────
        // 系统配置
        // ──────────────────────────────────────────────────────

        /// <summary>获取租户系统配置</summary>
        public static async ValueTask<TenantSystemConfigDto?> GetSystemConfigAsync(
            int tenantId, long operatorId, long tenantRefId)
        {
            var (result, data) = await TenantSystemConfigCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return null;
            foreach (var c in data)
            {
                if (c.TenantRefId == tenantRefId)
                    return MapConfigToDto(c);
            }
            return null;
        }

        /// <summary>更新租户系统配置</summary>
        public static async ValueTask<ApiResult> UpdateSystemConfigAsync(
            int tenantId, long operatorId, long tenantRefId, UpdateTenantSystemConfigRequest req)
        {
            var (getResult, configs) = await TenantSystemConfigCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || configs == null) return ApiResult.Fail("查询配置失败");

            TenantSystemConfig? target = null;
            foreach (var c in configs) { if (c.TenantRefId == tenantRefId) { target = c; break; } }

            var now = DateTime.UtcNow;
            if (target == null)
            {
                // 创建新配置
                target = new TenantSystemConfig
                {
                    TenantRefId = tenantRefId,
                    SystemName = req.SystemName,
                    LogoUrl = req.LogoUrl,
                    SystemTheme = req.SystemTheme,
                    DefaultLanguage = req.DefaultLanguage,
                    DefaultTimezone = req.DefaultTimezone,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await TenantSystemConfigCRUD.InsertAsync(tenantId, operatorId, target);
            }
            else
            {
                if (req.SystemName != null) target.SystemName = req.SystemName;
                if (req.LogoUrl != null) target.LogoUrl = req.LogoUrl;
                if (req.SystemTheme != null) target.SystemTheme = req.SystemTheme;
                if (req.DefaultLanguage != null) target.DefaultLanguage = req.DefaultLanguage;
                if (req.DefaultTimezone != null) target.DefaultTimezone = req.DefaultTimezone;
                target.UpdatedAt = now;
                await TenantSystemConfigCRUD.UpdateAsync(tenantId, operatorId, target);
            }

            Logger.Info(tenantId, operatorId,
                "[TenantConfigAppService] 更新系统配置: tenant=" + tenantRefId);
            return ApiResult.Ok();
        }

        // ──────────────────────────────────────────────────────
        // 功能开关
        // ──────────────────────────────────────────────────────

        /// <summary>获取功能开关列表</summary>
        public static async ValueTask<PagedResult<TenantFeatureFlagDto>> GetFeatureFlagListAsync(
            int tenantId, long operatorId, PagedRequest request, long? tenantRefId = null)
        {
            var (result, data) = await TenantFeatureFlagCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<TenantFeatureFlagDto> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<TenantFeatureFlag>();
            foreach (var f in data)
            {
                if (tenantRefId.HasValue && f.TenantRefId != tenantRefId.Value) continue;
                if (!string.IsNullOrEmpty(request.Keyword) &&
                    f.FeatureKey.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    f.FeatureName.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                filtered.Add(f);
            }

            var items = new List<TenantFeatureFlagDto>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
                items.Add(MapFlagToDto(filtered[i]));

            return new PagedResult<TenantFeatureFlagDto>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>创建/更新功能开关</summary>
        public static async ValueTask<ApiResult<long>> SaveFeatureFlagAsync(
            int tenantId, long operatorId, SaveTenantFeatureFlagRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.FeatureKey))
                return ApiResult<long>.Fail("功能键不能为空");

            var now = DateTime.UtcNow;
            var flag = new TenantFeatureFlag
            {
                TenantRefId = req.TenantRefId,
                FeatureKey = req.FeatureKey.Trim(),
                FeatureName = req.FeatureName.Trim(),
                Enabled = req.Enabled,
                RolloutType = req.RolloutType,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await TenantFeatureFlagCRUD.InsertAsync(tenantId, operatorId, flag);
            if (!insResult.Success)
                return ApiResult<long>.Fail("保存功能开关失败: " + insResult.ErrorMessage);

            await PlatformCacheCoordinator.InvalidateFeatureFlagsAsync();
            Logger.Info(tenantId, operatorId,
                "[TenantConfigAppService] 保存功能开关: " + req.FeatureKey + " enabled=" + req.Enabled);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>切换功能开关启停</summary>
        public static async ValueTask<ApiResult> ToggleFeatureFlagAsync(
            int tenantId, long operatorId, long id, bool enabled)
        {
            var (getResult, flags) = await TenantFeatureFlagCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || flags == null) return ApiResult.Fail("查询功能开关失败");

            TenantFeatureFlag? target = null;
            foreach (var f in flags) { if (f.Id == id) { target = f; break; } }
            if (target == null) return ApiResult.Fail("功能开关不存在");

            target.Enabled = enabled;
            target.UpdatedAt = DateTime.UtcNow;
            var updResult = await TenantFeatureFlagCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail("状态变更失败");

            await PlatformCacheCoordinator.InvalidateFeatureFlagsAsync();
            Logger.Info(tenantId, operatorId,
                "[TenantConfigAppService] 切换功能开关: " + target.FeatureKey + " → " + enabled);
            return ApiResult.Ok();
        }

        // ──────────────────────────────────────────────────────
        // 租户参数
        // ──────────────────────────────────────────────────────

        /// <summary>获取租户参数列表</summary>
        public static async ValueTask<PagedResult<TenantParameterDto>> GetParameterListAsync(
            int tenantId, long operatorId, PagedRequest request, long? tenantRefId = null)
        {
            var (result, data) = await TenantParameterCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<TenantParameterDto> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<TenantParameter>();
            foreach (var p in data)
            {
                if (tenantRefId.HasValue && p.TenantRefId != tenantRefId.Value) continue;
                if (!string.IsNullOrEmpty(request.Keyword) &&
                    p.ParamKey.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    p.ParamName.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                filtered.Add(p);
            }

            var items = new List<TenantParameterDto>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
            {
                var p = filtered[i];
                items.Add(new TenantParameterDto
                {
                    Id = p.Id, TenantRefId = p.TenantRefId,
                    ParamKey = p.ParamKey, ParamName = p.ParamName,
                    ParamType = p.ParamType, ParamValue = p.ParamValue,
                    UpdatedAt = p.UpdatedAt
                });
            }

            return new PagedResult<TenantParameterDto>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>创建/更新租户参数</summary>
        public static async ValueTask<ApiResult<long>> SaveParameterAsync(
            int tenantId, long operatorId, SaveTenantParameterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.ParamKey))
                return ApiResult<long>.Fail("参数键不能为空");

            var now = DateTime.UtcNow;
            var param = new TenantParameter
            {
                TenantRefId = req.TenantRefId,
                ParamKey = req.ParamKey.Trim(),
                ParamName = req.ParamName.Trim(),
                ParamType = req.ParamType,
                ParamValue = req.ParamValue,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await TenantParameterCRUD.InsertAsync(tenantId, operatorId, param);
            if (!insResult.Success)
                return ApiResult<long>.Fail("保存参数失败: " + insResult.ErrorMessage);

            Logger.Info(tenantId, operatorId,
                "[TenantConfigAppService] 保存参数: " + req.ParamKey);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>删除租户参数</summary>
        public static async ValueTask<ApiResult> DeleteParameterAsync(
            int tenantId, long operatorId, long id)
        {
            var delResult = await TenantParameterCRUD.DeleteAsync(tenantId, operatorId, id);
            if (!delResult.Success) return ApiResult.Fail("删除参数失败");

            Logger.Info(tenantId, operatorId, "[TenantConfigAppService] 删除参数: id=" + id);
            return ApiResult.Ok();
        }

        // ──────────────────────────────────────────────────────
        // Mapping helpers
        // ──────────────────────────────────────────────────────

        private static TenantSystemConfigDto MapConfigToDto(TenantSystemConfig c)
        {
            return new TenantSystemConfigDto
            {
                Id = c.Id, TenantRefId = c.TenantRefId,
                SystemName = c.SystemName, LogoUrl = c.LogoUrl,
                SystemTheme = c.SystemTheme, DefaultLanguage = c.DefaultLanguage,
                DefaultTimezone = c.DefaultTimezone, UpdatedAt = c.UpdatedAt
            };
        }

        private static TenantFeatureFlagDto MapFlagToDto(TenantFeatureFlag f)
        {
            return new TenantFeatureFlagDto
            {
                Id = f.Id, TenantRefId = f.TenantRefId,
                FeatureKey = f.FeatureKey, FeatureName = f.FeatureName,
                Enabled = f.Enabled, RolloutType = f.RolloutType,
                UpdatedAt = f.UpdatedAt
            };
        }
    }
}
