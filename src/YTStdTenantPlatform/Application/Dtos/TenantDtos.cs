using System;
using System.Collections.Generic;

namespace YTStdTenantPlatform.Application.Dtos
{
    // ──────────────────────────────────────────────────────────
    // 租户生命周期 DTO
    // ──────────────────────────────────────────────────────────

    /// <summary>租户列表项</summary>
    public sealed class TenantDto
    {
        /// <summary>租户 ID</summary>
        public long Id { get; set; }
        /// <summary>租户编码</summary>
        public string TenantCode { get; set; } = "";
        /// <summary>租户名称</summary>
        public string TenantName { get; set; } = "";
        /// <summary>企业名称</summary>
        public string? EnterpriseName { get; set; }
        /// <summary>联系人姓名</summary>
        public string? ContactName { get; set; }
        /// <summary>联系人邮箱</summary>
        public string? ContactEmail { get; set; }
        /// <summary>生命周期状态</summary>
        public string LifecycleStatus { get; set; } = "";
        /// <summary>隔离模式</summary>
        public string IsolationMode { get; set; } = "";
        /// <summary>是否启用</summary>
        public bool Enabled { get; set; }
        /// <summary>开通时间</summary>
        public DateTime? OpenedAt { get; set; }
        /// <summary>到期时间</summary>
        public DateTime? ExpiresAt { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>创建租户请求</summary>
    public sealed class CreateTenantRequest
    {
        /// <summary>租户编码</summary>
        public string TenantCode { get; set; } = "";
        /// <summary>租户名称</summary>
        public string TenantName { get; set; } = "";
        /// <summary>企业名称</summary>
        public string? EnterpriseName { get; set; }
        /// <summary>联系人姓名</summary>
        public string? ContactName { get; set; }
        /// <summary>联系人手机</summary>
        public string? ContactPhone { get; set; }
        /// <summary>联系人邮箱</summary>
        public string? ContactEmail { get; set; }
        /// <summary>来源类型</summary>
        public string SourceType { get; set; } = "manual";
        /// <summary>隔离模式（shared/schema/database）</summary>
        public string IsolationMode { get; set; } = "shared";
        /// <summary>默认语言</summary>
        public string DefaultLanguage { get; set; } = "zh-CN";
        /// <summary>默认时区</summary>
        public string DefaultTimezone { get; set; } = "Asia/Shanghai";
    }

    /// <summary>更新租户请求</summary>
    public sealed class UpdateTenantRequest
    {
        /// <summary>租户名称</summary>
        public string? TenantName { get; set; }
        /// <summary>企业名称</summary>
        public string? EnterpriseName { get; set; }
        /// <summary>联系人姓名</summary>
        public string? ContactName { get; set; }
        /// <summary>联系人手机</summary>
        public string? ContactPhone { get; set; }
        /// <summary>联系人邮箱</summary>
        public string? ContactEmail { get; set; }
    }

    /// <summary>租户状态变更请求</summary>
    public sealed class TenantStatusChangeRequest
    {
        /// <summary>目标状态（active/suspended/closed）</summary>
        public string TargetStatus { get; set; } = "";
        /// <summary>变更原因</summary>
        public string? Reason { get; set; }
    }

    /// <summary>租户生命周期事件列表项</summary>
    public sealed class TenantLifecycleEventDto
    {
        /// <summary>事件 ID</summary>
        public long Id { get; set; }
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>事件类型</summary>
        public string EventType { get; set; } = "";
        /// <summary>原状态</summary>
        public string? FromStatus { get; set; }
        /// <summary>目标状态</summary>
        public string? ToStatus { get; set; }
        /// <summary>原因</summary>
        public string? Reason { get; set; }
        /// <summary>发生时间</summary>
        public DateTime OccurredAt { get; set; }
    }

    // ──────────────────────────────────────────────────────────
    // 租户信息 DTO
    // ──────────────────────────────────────────────────────────

    /// <summary>租户分组列表项</summary>
    public sealed class TenantGroupDto
    {
        /// <summary>分组 ID</summary>
        public long Id { get; set; }
        /// <summary>分组编码</summary>
        public string GroupCode { get; set; } = "";
        /// <summary>分组名称</summary>
        public string GroupName { get; set; } = "";
        /// <summary>描述</summary>
        public string? Description { get; set; }
        /// <summary>父级分组 ID</summary>
        public long? ParentId { get; set; }
        /// <summary>子节点列表</summary>
        public List<TenantGroupDto>? Children { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>创建租户分组请求</summary>
    public sealed class CreateTenantGroupRequest
    {
        /// <summary>分组编码</summary>
        public string GroupCode { get; set; } = "";
        /// <summary>分组名称</summary>
        public string GroupName { get; set; } = "";
        /// <summary>描述</summary>
        public string? Description { get; set; }
        /// <summary>父级分组 ID</summary>
        public long? ParentId { get; set; }
    }

    /// <summary>租户域名列表项</summary>
    public sealed class TenantDomainDto
    {
        /// <summary>域名 ID</summary>
        public long Id { get; set; }
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>域名</summary>
        public string Domain { get; set; } = "";
        /// <summary>域名类型</summary>
        public string DomainType { get; set; } = "";
        /// <summary>是否为主域名</summary>
        public bool IsPrimary { get; set; }
        /// <summary>验证状态</summary>
        public string VerificationStatus { get; set; } = "";
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>创建租户域名请求</summary>
    public sealed class CreateTenantDomainRequest
    {
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>域名</summary>
        public string Domain { get; set; } = "";
        /// <summary>域名类型（primary/alias/custom）</summary>
        public string DomainType { get; set; } = "custom";
    }

    /// <summary>租户标签列表项</summary>
    public sealed class TenantTagDto
    {
        /// <summary>标签 ID</summary>
        public long Id { get; set; }
        /// <summary>标签键</summary>
        public string TagKey { get; set; } = "";
        /// <summary>标签值</summary>
        public string TagValue { get; set; } = "";
        /// <summary>标签类型</summary>
        public string TagType { get; set; } = "";
        /// <summary>描述</summary>
        public string? Description { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>创建租户标签请求</summary>
    public sealed class CreateTenantTagRequest
    {
        /// <summary>标签键</summary>
        public string TagKey { get; set; } = "";
        /// <summary>标签值</summary>
        public string TagValue { get; set; } = "";
        /// <summary>标签类型</summary>
        public string TagType { get; set; } = "custom";
        /// <summary>描述</summary>
        public string? Description { get; set; }
    }

    /// <summary>标签绑定请求</summary>
    public sealed class TagBindRequest
    {
        /// <summary>租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>标签 ID 列表</summary>
        public long[] TagIds { get; set; } = Array.Empty<long>();
    }

    // ──────────────────────────────────────────────────────────
    // 租户资源管理 DTO
    // ──────────────────────────────────────────────────────────

    /// <summary>租户资源配额列表项</summary>
    public sealed class TenantResourceQuotaDto
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

    /// <summary>创建/更新租户资源配额请求</summary>
    public sealed class SaveTenantResourceQuotaRequest
    {
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>配额类型</summary>
        public string QuotaType { get; set; } = "";
        /// <summary>配额上限</summary>
        public long QuotaLimit { get; set; }
        /// <summary>告警阈值</summary>
        public long? WarningThreshold { get; set; }
        /// <summary>重置周期（daily/monthly/yearly/none）</summary>
        public string? ResetCycle { get; set; }
    }

    // ──────────────────────────────────────────────────────────
    // 租户配置中心 DTO
    // ──────────────────────────────────────────────────────────

    /// <summary>租户系统配置</summary>
    public sealed class TenantSystemConfigDto
    {
        /// <summary>配置 ID</summary>
        public long Id { get; set; }
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>系统名称</summary>
        public string? SystemName { get; set; }
        /// <summary>Logo 地址</summary>
        public string? LogoUrl { get; set; }
        /// <summary>系统主题</summary>
        public string? SystemTheme { get; set; }
        /// <summary>默认语言</summary>
        public string? DefaultLanguage { get; set; }
        /// <summary>默认时区</summary>
        public string? DefaultTimezone { get; set; }
        /// <summary>更新时间</summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>更新租户系统配置请求</summary>
    public sealed class UpdateTenantSystemConfigRequest
    {
        /// <summary>系统名称</summary>
        public string? SystemName { get; set; }
        /// <summary>Logo 地址</summary>
        public string? LogoUrl { get; set; }
        /// <summary>系统主题</summary>
        public string? SystemTheme { get; set; }
        /// <summary>默认语言</summary>
        public string? DefaultLanguage { get; set; }
        /// <summary>默认时区</summary>
        public string? DefaultTimezone { get; set; }
    }

    /// <summary>租户功能开关列表项</summary>
    public sealed class TenantFeatureFlagDto
    {
        /// <summary>功能开关 ID</summary>
        public long Id { get; set; }
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>功能键</summary>
        public string FeatureKey { get; set; } = "";
        /// <summary>功能名称</summary>
        public string FeatureName { get; set; } = "";
        /// <summary>是否启用</summary>
        public bool Enabled { get; set; }
        /// <summary>灰度类型</summary>
        public string RolloutType { get; set; } = "";
        /// <summary>更新时间</summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>创建/更新租户功能开关请求</summary>
    public sealed class SaveTenantFeatureFlagRequest
    {
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>功能键</summary>
        public string FeatureKey { get; set; } = "";
        /// <summary>功能名称</summary>
        public string FeatureName { get; set; } = "";
        /// <summary>是否启用</summary>
        public bool Enabled { get; set; }
        /// <summary>灰度类型（all/percentage/whitelist）</summary>
        public string RolloutType { get; set; } = "all";
    }

    /// <summary>租户参数列表项</summary>
    public sealed class TenantParameterDto
    {
        /// <summary>参数 ID</summary>
        public long Id { get; set; }
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>参数键</summary>
        public string ParamKey { get; set; } = "";
        /// <summary>参数名称</summary>
        public string ParamName { get; set; } = "";
        /// <summary>参数类型</summary>
        public string ParamType { get; set; } = "";
        /// <summary>参数值</summary>
        public string ParamValue { get; set; } = "";
        /// <summary>更新时间</summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>创建/更新租户参数请求</summary>
    public sealed class SaveTenantParameterRequest
    {
        /// <summary>关联租户 ID</summary>
        public long TenantRefId { get; set; }
        /// <summary>参数键</summary>
        public string ParamKey { get; set; } = "";
        /// <summary>参数名称</summary>
        public string ParamName { get; set; } = "";
        /// <summary>参数类型（string/number/boolean/json）</summary>
        public string ParamType { get; set; } = "string";
        /// <summary>参数值</summary>
        public string ParamValue { get; set; } = "";
    }
}
