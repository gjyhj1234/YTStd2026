namespace YTStdTenantPlatform.Application.Constants;

/// <summary>API 错误码常量定义，用于前端国际化处理</summary>
public static class ErrorCodes
{
    // ── 成功 (0) ──

    /// <summary>操作成功</summary>
    public const int Success = 0;

    // ── 认证 (10001-10099) ──

    /// <summary>用户名或密码不能为空</summary>
    public const int AuthCredentialsRequired = 10001;
    /// <summary>用户名或密码错误</summary>
    public const int AuthInvalidCredentials = 10002;
    /// <summary>账户已禁用</summary>
    public const int AuthAccountDisabled = 10003;
    /// <summary>账户已锁定</summary>
    public const int AuthAccountLocked = 10004;
    /// <summary>登录状态更新失败</summary>
    public const int AuthLoginUpdateFailed = 10005;
    /// <summary>令牌无效或已过期</summary>
    public const int AuthTokenInvalid = 10006;
    /// <summary>旧密码不正确</summary>
    public const int AuthOldPasswordInvalid = 10007;
    /// <summary>修改密码失败</summary>
    public const int AuthChangePasswordFailed = 10008;
    /// <summary>新密码不能为空</summary>
    public const int AuthNewPasswordRequired = 10009;

    // ── 权限 (11001-11099) ──

    /// <summary>权限不足</summary>
    public const int Forbidden = 11001;

    // ── 输入验证 (12001-12099) ──

    /// <summary>请求体无效</summary>
    public const int InvalidRequestBody = 12001;
    /// <summary>资源不存在</summary>
    public const int ResourceNotFound = 12002;
    /// <summary>参数错误</summary>
    public const int InvalidParameter = 12003;
    /// <summary>操作无效</summary>
    public const int InvalidOperation = 12004;

    // ── 唯一性冲突 (18001-18099) ──

    /// <summary>用户名已存在</summary>
    public const int UsernameExists = 18001;
    /// <summary>角色编码已存在</summary>
    public const int RoleCodeExists = 18002;
    /// <summary>租户编码已存在</summary>
    public const int TenantCodeExists = 18003;

    // ── 平台用户 (19001-19099) ──

    /// <summary>用户名不能为空</summary>
    public const int UserUsernameRequired = 19001;
    /// <summary>邮箱不能为空</summary>
    public const int UserEmailRequired = 19002;
    /// <summary>密码不能为空</summary>
    public const int UserPasswordRequired = 19003;
    /// <summary>创建用户失败</summary>
    public const int UserCreateFailed = 19004;
    /// <summary>查询用户失败</summary>
    public const int UserQueryFailed = 19005;
    /// <summary>用户不存在</summary>
    public const int UserNotFound = 19006;
    /// <summary>更新用户失败</summary>
    public const int UserUpdateFailed = 19007;
    /// <summary>用户状态变更失败</summary>
    public const int UserStatusChangeFailed = 19008;

    // ── 平台角色 (19101-19199) ──

    /// <summary>角色编码不能为空</summary>
    public const int RoleCodeRequired = 19101;
    /// <summary>角色名称不能为空</summary>
    public const int RoleNameRequired = 19102;
    /// <summary>创建角色失败</summary>
    public const int RoleCreateFailed = 19103;
    /// <summary>查询角色失败</summary>
    public const int RoleQueryFailed = 19104;
    /// <summary>角色不存在</summary>
    public const int RoleNotFound = 19105;
    /// <summary>更新角色失败</summary>
    public const int RoleUpdateFailed = 19106;
    /// <summary>角色状态变更失败</summary>
    public const int RoleStatusChangeFailed = 19107;
    /// <summary>角色权限绑定失败</summary>
    public const int RolePermissionBindFailed = 19108;
    /// <summary>角色成员绑定失败</summary>
    public const int RoleMemberBindFailed = 19109;

    // ── 租户 (19301-19399) ──

    /// <summary>租户编码不能为空</summary>
    public const int TenantCodeRequired = 19301;
    /// <summary>租户名称不能为空</summary>
    public const int TenantNameRequired = 19302;
    /// <summary>创建租户失败</summary>
    public const int TenantCreateFailed = 19303;
    /// <summary>查询租户失败</summary>
    public const int TenantQueryFailed = 19304;
    /// <summary>租户不存在</summary>
    public const int TenantNotFound = 19305;
    /// <summary>更新租户失败</summary>
    public const int TenantUpdateFailed = 19306;
    /// <summary>租户状态变更失败</summary>
    public const int TenantStatusChangeFailed = 19307;
    /// <summary>租户状态流转不允许</summary>
    public const int TenantStatusTransitionDenied = 19308;
    /// <summary>分组编码不能为空</summary>
    public const int GroupCodeRequired = 19309;
    /// <summary>创建分组失败</summary>
    public const int GroupCreateFailed = 19310;
    /// <summary>域名不能为空</summary>
    public const int DomainRequired = 19311;
    /// <summary>创建域名失败</summary>
    public const int DomainCreateFailed = 19312;
    /// <summary>标签键不能为空</summary>
    public const int TagKeyRequired = 19313;
    /// <summary>创建标签失败</summary>
    public const int TagCreateFailed = 19314;
    /// <summary>标签绑定失败</summary>
    public const int TagBindFailed = 19315;
    /// <summary>配额类型不能为空</summary>
    public const int QuotaTypeRequired = 19316;
    /// <summary>配额上限必须大于0</summary>
    public const int QuotaLimitInvalid = 19317;
    /// <summary>保存配额失败</summary>
    public const int QuotaSaveFailed = 19318;
    /// <summary>查询配置失败</summary>
    public const int ConfigQueryFailed = 19319;
    /// <summary>更新配置失败</summary>
    public const int ConfigUpdateFailed = 19320;
    /// <summary>功能键不能为空</summary>
    public const int FeatureKeyRequired = 19321;
    /// <summary>保存功能开关失败</summary>
    public const int FeatureFlagSaveFailed = 19322;
    /// <summary>功能开关不存在</summary>
    public const int FeatureFlagNotFound = 19323;
    /// <summary>功能开关状态变更失败</summary>
    public const int FeatureFlagToggleFailed = 19324;
    /// <summary>参数键不能为空</summary>
    public const int ParamKeyRequired = 19325;
    /// <summary>保存参数失败</summary>
    public const int ParamSaveFailed = 19326;
    /// <summary>删除参数失败</summary>
    public const int ParamDeleteFailed = 19327;

    // ── 套餐 (19401-19499) ──

    /// <summary>套餐编码不能为空</summary>
    public const int PackageCodeRequired = 19401;
    /// <summary>套餐名称不能为空</summary>
    public const int PackageNameRequired = 19402;
    /// <summary>创建套餐失败</summary>
    public const int PackageCreateFailed = 19403;
    /// <summary>查询套餐失败</summary>
    public const int PackageQueryFailed = 19404;
    /// <summary>套餐不存在</summary>
    public const int PackageNotFound = 19405;
    /// <summary>更新套餐失败</summary>
    public const int PackageUpdateFailed = 19406;
    /// <summary>套餐状态变更失败</summary>
    public const int PackageStatusChangeFailed = 19407;
    /// <summary>版本编码不能为空</summary>
    public const int PackageVersionCodeRequired = 19408;
    /// <summary>版本名称不能为空</summary>
    public const int PackageVersionNameRequired = 19409;
    /// <summary>创建版本失败</summary>
    public const int PackageVersionCreateFailed = 19410;
    /// <summary>能力键不能为空</summary>
    public const int PackageCapabilityKeyRequired = 19411;
    /// <summary>保存能力失败</summary>
    public const int PackageCapabilitySaveFailed = 19412;

    // ── 订阅 (19501-19599) ──

    /// <summary>创建订阅失败</summary>
    public const int SubscriptionCreateFailed = 19501;
    /// <summary>查询订阅失败</summary>
    public const int SubscriptionQueryFailed = 19502;
    /// <summary>订阅不存在</summary>
    public const int SubscriptionNotFound = 19503;
    /// <summary>取消订阅失败</summary>
    public const int SubscriptionCancelFailed = 19504;
    /// <summary>创建试用失败</summary>
    public const int TrialCreateFailed = 19505;

    // ── 计费 (19601-19699) ──

    /// <summary>创建发票失败</summary>
    public const int InvoiceCreateFailed = 19601;
    /// <summary>查询发票失败</summary>
    public const int InvoiceQueryFailed = 19602;
    /// <summary>发票不存在</summary>
    public const int InvoiceNotFound = 19603;
    /// <summary>作废发票失败</summary>
    public const int InvoiceVoidFailed = 19604;
    /// <summary>创建支付单失败</summary>
    public const int PaymentOrderCreateFailed = 19605;
    /// <summary>创建退款失败</summary>
    public const int RefundCreateFailed = 19606;

    // ── API集成 (19701-19799) ──

    /// <summary>创建API密钥失败</summary>
    public const int ApiKeyCreateFailed = 19701;
    /// <summary>禁用API密钥失败</summary>
    public const int ApiKeyDisableFailed = 19702;
    /// <summary>查询API密钥失败</summary>
    public const int ApiKeyQueryFailed = 19703;
    /// <summary>API密钥不存在</summary>
    public const int ApiKeyNotFound = 19704;
    /// <summary>创建Webhook失败</summary>
    public const int WebhookCreateFailed = 19705;
    /// <summary>查询Webhook失败</summary>
    public const int WebhookQueryFailed = 19706;
    /// <summary>Webhook不存在</summary>
    public const int WebhookNotFound = 19707;
    /// <summary>更新Webhook失败</summary>
    public const int WebhookUpdateFailed = 19708;
    /// <summary>Webhook状态变更失败</summary>
    public const int WebhookStatusChangeFailed = 19709;

    // ── 通知 (19801-19899) ──

    /// <summary>模板名称不能为空</summary>
    public const int NotificationTemplateNameRequired = 19801;
    /// <summary>创建通知模板失败</summary>
    public const int NotificationTemplateCreateFailed = 19802;
    /// <summary>查询通知模板失败</summary>
    public const int NotificationTemplateQueryFailed = 19803;
    /// <summary>通知模板不存在</summary>
    public const int NotificationTemplateNotFound = 19804;
    /// <summary>更新通知模板失败</summary>
    public const int NotificationTemplateUpdateFailed = 19805;
    /// <summary>通知模板状态变更失败</summary>
    public const int NotificationTemplateStatusChangeFailed = 19806;
    /// <summary>创建通知失败</summary>
    public const int NotificationCreateFailed = 19807;
    /// <summary>查询通知失败</summary>
    public const int NotificationQueryFailed = 19808;
    /// <summary>通知不存在</summary>
    public const int NotificationNotFound = 19809;
    /// <summary>标记通知已读失败</summary>
    public const int NotificationMarkReadFailed = 19810;

    // ── 文件存储 (19901-19999) ──

    /// <summary>策略名称不能为空</summary>
    public const int StorageStrategyNameRequired = 19901;
    /// <summary>创建存储策略失败</summary>
    public const int StorageStrategyCreateFailed = 19902;
    /// <summary>查询存储策略失败</summary>
    public const int StorageStrategyQueryFailed = 19903;
    /// <summary>存储策略不存在</summary>
    public const int StorageStrategyNotFound = 19904;
    /// <summary>更新存储策略失败</summary>
    public const int StorageStrategyUpdateFailed = 19905;
    /// <summary>存储策略状态变更失败</summary>
    public const int StorageStrategyStatusChangeFailed = 19906;
    /// <summary>删除文件失败</summary>
    public const int FileDeleteFailed = 19907;
    /// <summary>保存文件访问策略失败</summary>
    public const int FileAccessPolicySaveFailed = 19908;

    // ── 系统 (50001-50099) ──

    /// <summary>服务器内部错误</summary>
    public const int InternalServerError = 50001;
    /// <summary>操作失败</summary>
    public const int OperationFailed = 50002;
    /// <summary>系统繁忙</summary>
    public const int SystemBusy = 50003;
}
