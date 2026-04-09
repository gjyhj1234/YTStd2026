using System;

namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>创建订阅请求</summary>
    public sealed class CreateSubscriptionReqDTO
    {
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>套餐版本 ID</summary>
        public long PackageVersionId { get; set; }
        /// <summary>订阅类型（standard/enterprise/custom）</summary>
        public string SubscriptionType { get; set; } = "standard";
        /// <summary>是否自动续费</summary>
        public bool AutoRenew { get; set; } = false;
    }

    /// <summary>续费订阅请求</summary>
    public sealed class RenewSubscriptionReqDTO
    {
        /// <summary>续费时长（月）</summary>
        public int Months { get; set; } = 12;
    }

    /// <summary>升级订阅请求</summary>
    public sealed class UpgradeSubscriptionReqDTO
    {
        /// <summary>目标套餐版本 ID</summary>
        public long TargetPackageVersionId { get; set; }
    }

    /// <summary>创建试用请求</summary>
    public sealed class CreateTrialReqDTO
    {
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>套餐版本 ID</summary>
        public long PackageVersionId { get; set; }
    }
}
