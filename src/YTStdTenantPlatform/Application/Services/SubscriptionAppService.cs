using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Application.Constants;
using YTStdTenantPlatform.Domain.Enums;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>订阅管理应用服务</summary>
    public static class SubscriptionAppService
    {
        // ──────────────────────────────────────────────────────
        // 租户订阅
        // ──────────────────────────────────────────────────────

        /// <summary>获取订阅分页列表</summary>
        public static async ValueTask<PagedResult<TenantSubscriptionRepDTO>> GetSubscriptionListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (result, data) = await TenantSubscriptionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<TenantSubscriptionRepDTO> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<TenantSubscription>();
            foreach (var s in data)
            {
                if (!string.IsNullOrEmpty(request.Status) &&
                    !string.Equals(s.SubscriptionStatus, request.Status, StringComparison.OrdinalIgnoreCase))
                    continue;
                filtered.Add(s);
            }

            var items = new List<TenantSubscriptionRepDTO>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
                items.Add(MapSubscriptionToDto(filtered[i]));

            return new PagedResult<TenantSubscriptionRepDTO>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>获取订阅详情</summary>
        public static async ValueTask<TenantSubscriptionRepDTO?> GetSubscriptionByIdAsync(
            int tenantId, long operatorId, long id)
        {
            var (result, data) = await TenantSubscriptionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return null;
            foreach (var s in data)
            {
                if (s.Id == id)
                    return MapSubscriptionToDto(s);
            }
            return null;
        }

        /// <summary>获取租户当前有效订阅</summary>
        public static async ValueTask<TenantSubscriptionRepDTO?> GetTenantSubscriptionAsync(
            int tenantId, long operatorId, long tenantRefId)
        {
            var (result, data) = await TenantSubscriptionCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return null;
            foreach (var s in data)
            {
                if (s.TenantRefId == tenantRefId &&
                    string.Equals(s.SubscriptionStatus, nameof(SubscriptionStatus.Active), StringComparison.OrdinalIgnoreCase))
                    return MapSubscriptionToDto(s);
            }
            return null;
        }

        /// <summary>创建订阅</summary>
        public static async ValueTask<ApiResult<long>> CreateSubscriptionAsync(
            int tenantId, long operatorId, CreateSubscriptionReqDTO req)
        {
            if (req.TenantRefId <= 0)
                return ApiResult<long>.Fail(ErrorCodes.InvalidParameter);
            if (req.PackageVersionId <= 0)
                return ApiResult<long>.Fail(ErrorCodes.InvalidParameter);

            var now = DateTime.UtcNow;
            var entity = new TenantSubscription
            {
                Id = await DB.GetNextLongIdAsync(),
                TenantRefId = req.TenantRefId,
                PackageVersionId = req.PackageVersionId,
                SubscriptionStatus = nameof(SubscriptionStatus.Active),
                SubscriptionType = req.SubscriptionType,
                StartedAt = now,
                ExpiresAt = now.AddYears(1),
                AutoRenew = req.AutoRenew,
                CreatedBy = operatorId,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await TenantSubscriptionCRUD.InsertAsync(tenantId, operatorId, entity);
            if (!insResult.Success)
                return ApiResult<long>.Fail(ErrorCodes.SubscriptionCreateFailed);

            Logger.Debug(tenantId, operatorId, () => "[SubscriptionAppService] 创建订阅: tenant=" + req.TenantRefId);
            return ApiResult<long>.Ok(entity.Id);
        }

        /// <summary>续费订阅</summary>
        public static async ValueTask<ApiResult> RenewSubscriptionAsync(
            int tenantId, long operatorId, long id, RenewSubscriptionReqDTO req)
        {
            var (getResult, subscriptions) = await TenantSubscriptionCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || subscriptions == null) return ApiResult.Fail(ErrorCodes.SubscriptionQueryFailed);

            TenantSubscription? target = null;
            foreach (var s in subscriptions) { if (s.Id == id) { target = s; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.SubscriptionNotFound);

            if (!string.Equals(target.SubscriptionStatus, nameof(SubscriptionStatus.Active), StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(target.SubscriptionStatus, nameof(SubscriptionStatus.Expiring), StringComparison.OrdinalIgnoreCase))
                return ApiResult.Fail(ErrorCodes.SubscriptionStatusDenied);

            var months = req.Months > 0 ? req.Months : 12;
            target.ExpiresAt = target.ExpiresAt.AddMonths(months);
            target.SubscriptionStatus = nameof(SubscriptionStatus.Active);
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await TenantSubscriptionCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.SubscriptionRenewFailed);

            // 记录变更
            await RecordChangeAsync(tenantId, operatorId, target.TenantRefId, target.Id,
                nameof(SubscriptionChangeType.Renew), target.PackageVersionId, target.PackageVersionId, "续费 " + months + " 个月");

            Logger.Debug(tenantId, operatorId, () => "[SubscriptionAppService] 续费订阅: id=" + id);
            return ApiResult.Ok();
        }

        /// <summary>升级订阅套餐</summary>
        public static async ValueTask<ApiResult> UpgradeSubscriptionAsync(
            int tenantId, long operatorId, long id, UpgradeSubscriptionReqDTO req)
        {
            var (getResult, subscriptions) = await TenantSubscriptionCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || subscriptions == null) return ApiResult.Fail(ErrorCodes.SubscriptionQueryFailed);

            TenantSubscription? target = null;
            foreach (var s in subscriptions) { if (s.Id == id) { target = s; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.SubscriptionNotFound);

            if (!string.Equals(target.SubscriptionStatus, nameof(SubscriptionStatus.Active), StringComparison.OrdinalIgnoreCase))
                return ApiResult.Fail(ErrorCodes.SubscriptionStatusDenied);

            var fromVersionId = target.PackageVersionId;
            target.PackageVersionId = req.TargetPackageVersionId;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await TenantSubscriptionCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.SubscriptionUpgradeFailed);

            await RecordChangeAsync(tenantId, operatorId, target.TenantRefId, target.Id,
                nameof(SubscriptionChangeType.Upgrade), fromVersionId, req.TargetPackageVersionId, "升级套餐");

            Logger.Debug(tenantId, operatorId, () => "[SubscriptionAppService] 升级订阅: id=" + id);
            return ApiResult.Ok();
        }

        /// <summary>取消订阅</summary>
        public static async ValueTask<ApiResult> CancelSubscriptionAsync(
            int tenantId, long operatorId, long id)
        {
            var (getResult, subscriptions) = await TenantSubscriptionCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || subscriptions == null) return ApiResult.Fail(ErrorCodes.SubscriptionQueryFailed);

            TenantSubscription? target = null;
            foreach (var s in subscriptions) { if (s.Id == id) { target = s; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.SubscriptionNotFound);

            target.SubscriptionStatus = nameof(SubscriptionStatus.Cancelled);
            target.CancelledAt = DateTime.UtcNow;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await TenantSubscriptionCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.SubscriptionCancelFailed);

            await RecordChangeAsync(tenantId, operatorId, target.TenantRefId, target.Id,
                nameof(SubscriptionChangeType.Cancel), target.PackageVersionId, null, "取消订阅");

            Logger.Debug(tenantId, operatorId, () => "[SubscriptionAppService] 取消订阅: id=" + id);
            return ApiResult.Ok();
        }

        /// <summary>记录订阅变更</summary>
        private static async ValueTask RecordChangeAsync(
            int tenantId, long operatorId, long tenantRefId, long subscriptionId,
            string changeType, long? fromVersionId, long? toVersionId, string? remark)
        {
            var change = new TenantSubscriptionChange
            {
                Id = await DB.GetNextLongIdAsync(),
                TenantRefId = tenantRefId,
                SubscriptionId = subscriptionId,
                ChangeType = changeType,
                FromPackageVersionId = fromVersionId,
                ToPackageVersionId = toVersionId,
                EffectiveAt = DateTime.UtcNow,
                Remark = remark,
                CreatedAt = DateTime.UtcNow
            };
            await TenantSubscriptionChangeCRUD.InsertAsync(tenantId, operatorId, change);
        }

        // ──────────────────────────────────────────────────────
        // 租户试用
        // ──────────────────────────────────────────────────────

        /// <summary>获取试用分页列表</summary>
        public static async ValueTask<PagedResult<TenantTrialRepDTO>> GetTrialListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (result, data) = await TenantTrialCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<TenantTrialRepDTO> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var items = new List<TenantTrialRepDTO>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < data.Count && i < offset + size; i++)
                items.Add(MapTrialToDto(data[i]));

            return new PagedResult<TenantTrialRepDTO>
            {
                Items = items, Total = data.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>创建试用</summary>
        public static async ValueTask<ApiResult<long>> CreateTrialAsync(
            int tenantId, long operatorId, CreateTrialReqDTO req)
        {
            if (req.TenantRefId <= 0)
                return ApiResult<long>.Fail(ErrorCodes.InvalidParameter);

            var now = DateTime.UtcNow;
            var entity = new TenantTrial
            {
                Id = await DB.GetNextLongIdAsync(),
                TenantRefId = req.TenantRefId,
                PackageVersionId = req.PackageVersionId,
                Status = (int)TrialStatus.Active,
                StartedAt = now,
                ExpiresAt = now.AddDays(30),
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await TenantTrialCRUD.InsertAsync(tenantId, operatorId, entity);
            if (!insResult.Success)
                return ApiResult<long>.Fail(ErrorCodes.TrialCreateFailed);

            Logger.Debug(tenantId, operatorId, () => "[SubscriptionAppService] 创建试用: tenant=" + req.TenantRefId);
            return ApiResult<long>.Ok(entity.Id);
        }

        // ──────────────────────────────────────────────────────
        // 订阅变更
        // ──────────────────────────────────────────────────────

        /// <summary>获取订阅变更记录列表</summary>
        public static async ValueTask<PagedResult<TenantSubscriptionChangeRepDTO>> GetSubscriptionChangesAsync(
            int tenantId, long operatorId, long tenantRefId, PagedRequest request)
        {
            var (result, data) = await TenantSubscriptionChangeCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<TenantSubscriptionChangeRepDTO> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<TenantSubscriptionChange>();
            foreach (var c in data)
            {
                if (c.TenantRefId == tenantRefId)
                    filtered.Add(c);
            }

            var items = new List<TenantSubscriptionChangeRepDTO>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
            {
                var c = filtered[i];
                items.Add(new TenantSubscriptionChangeRepDTO
                {
                    Id = c.Id, TenantRefId = c.TenantRefId,
                    SubscriptionId = c.SubscriptionId, ChangeType = c.ChangeType,
                    FromPackageVersionId = c.FromPackageVersionId,
                    ToPackageVersionId = c.ToPackageVersionId,
                    EffectiveAt = c.EffectiveAt, Remark = c.Remark,
                    CreatedAt = c.CreatedAt
                });
            }

            return new PagedResult<TenantSubscriptionChangeRepDTO>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        // ──────────────────────────────────────────────────────
        // Mapping helpers
        // ──────────────────────────────────────────────────────

        private static TenantSubscriptionRepDTO MapSubscriptionToDto(TenantSubscription s) => new TenantSubscriptionRepDTO
        {
            Id = s.Id, TenantRefId = s.TenantRefId, PackageVersionId = s.PackageVersionId,
            SubscriptionStatus = s.SubscriptionStatus, SubscriptionType = s.SubscriptionType,
            StartedAt = s.StartedAt, ExpiresAt = s.ExpiresAt,
            AutoRenew = s.AutoRenew, CancelledAt = s.CancelledAt,
            CreatedAt = s.CreatedAt
        };

        private static TenantTrialRepDTO MapTrialToDto(TenantTrial t) => new TenantTrialRepDTO
        {
            Id = t.Id, TenantRefId = t.TenantRefId, PackageVersionId = t.PackageVersionId,
            Status = ((TrialStatus)t.Status).ToString(), StartedAt = t.StartedAt, ExpiresAt = t.ExpiresAt,
            ConvertedSubscriptionId = t.ConvertedSubscriptionId, CreatedAt = t.CreatedAt
        };
    }
}
