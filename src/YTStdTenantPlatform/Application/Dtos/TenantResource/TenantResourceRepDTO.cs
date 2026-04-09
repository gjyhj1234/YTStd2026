using System;

namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>租户资源配额列表项</summary>
    public sealed class TenantResourceQuotaRepDTO
    {
        /// <summary>配额 ID</summary>
        public long Id { get; set; }
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>配额类型</summary>
        public string QuotaType { get; set; } = "";
        /// <summary>配额上限</summary>
        public long QuotaLimit { get; set; }
        /// <summary>告警阈值</summary>
        public long? WarningThreshold { get; set; }
        /// <summary>重置周期</summary>
        public string? ResetCycle { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>租户资源使用情况</summary>
    public sealed class TenantResourceUsageRepDTO
    {
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>配额类型</summary>
        public string QuotaType { get; set; } = "";
        /// <summary>配额上限</summary>
        public long QuotaLimit { get; set; }
        /// <summary>当前用量</summary>
        public long CurrentUsage { get; set; }
        /// <summary>使用百分比</summary>
        public decimal UsagePercent { get; set; }
    }
}
