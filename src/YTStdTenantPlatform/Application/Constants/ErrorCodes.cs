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
    /// <summary>权限编码已存在</summary>
    public const int PermissionCodeExists = 18004;
    /// <summary>邮箱已存在</summary>
    public const int EmailExists = 18007;
    /// <summary>分组编码已存在</summary>
    public const int GroupCodeExists = 18008;
    /// <summary>域名已存在</summary>
    public const int DomainExists = 18009;
    /// <summary>标签键已存在</summary>
    public const int TagKeyExists = 18010;
    /// <summary>功能开关键已存在</summary>
    public const int FeatureKeyExists = 18011;
    /// <summary>参数键已存在</summary>
    public const int ParamKeyExists = 18012;
    /// <summary>字典项编码已存在</summary>
    public const int DictItemCodeExists = 18013;
    /// <summary>套餐版本编码已存在</summary>
    public const int PackageVersionCodeExists = 18014;
    /// <summary>通知模板编码已存在</summary>
    public const int NotificationTemplateCodeExists = 18015;

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
    /// <summary>删除用户失败</summary>
    public const int UserDeleteFailed = 19009;
    /// <summary>重置密码失败</summary>
    public const int UserResetPasswordFailed = 19010;
    /// <summary>禁止删除超级管理员</summary>
    public const int UserCannotDeleteAdmin = 19011;
    /// <summary>禁止禁用自己</summary>
    public const int UserCannotDisableSelf = 19012;

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
    /// <summary>删除角色失败</summary>
    public const int RoleDeleteFailed = 19110;
    /// <summary>禁止删除超级管理员角色</summary>
    public const int RoleCannotDeleteSuperAdmin = 19111;
    /// <summary>角色下存在关联用户，禁止删除</summary>
    public const int RoleHasAssociatedUsers = 19112;

    // ── 平台权限 (19201-19299) ──

    /// <summary>权限编码不能为空</summary>
    public const int PermissionCodeRequired = 19201;
    /// <summary>创建权限失败</summary>
    public const int PermissionCreateFailed = 19202;
    /// <summary>查询权限失败</summary>
    public const int PermissionQueryFailed = 19203;
    /// <summary>权限不存在</summary>
    public const int PermissionNotFound = 19204;
    /// <summary>更新权限失败</summary>
    public const int PermissionUpdateFailed = 19205;
    /// <summary>删除权限失败</summary>
    public const int PermissionDeleteFailed = 19206;

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
    /// <summary>删除租户失败</summary>
    public const int TenantDeleteFailed = 19328;
    /// <summary>查询分组失败</summary>
    public const int GroupQueryFailed = 19329;
    /// <summary>分组不存在</summary>
    public const int GroupNotFound = 19330;
    /// <summary>更新分组失败</summary>
    public const int GroupUpdateFailed = 19331;
    /// <summary>删除分组失败</summary>
    public const int GroupDeleteFailed = 19332;
    /// <summary>查询域名失败</summary>
    public const int DomainQueryFailed = 19333;
    /// <summary>域名不存在</summary>
    public const int DomainNotFound = 19334;
    /// <summary>删除域名失败</summary>
    public const int DomainDeleteFailed = 19335;
    /// <summary>查询标签失败</summary>
    public const int TagQueryFailed = 19336;
    /// <summary>标签不存在</summary>
    public const int TagNotFound = 19337;
    /// <summary>删除标签失败</summary>
    public const int TagDeleteFailed = 19338;
    /// <summary>设置租户标签失败</summary>
    public const int TenantTagSetFailed = 19339;
    /// <summary>查询资源使用情况失败</summary>
    public const int ResourceUsageQueryFailed = 19340;
    /// <summary>更新配额失败</summary>
    public const int QuotaUpdateFailed = 19341;
    /// <summary>配置不存在</summary>
    public const int ConfigNotFound = 19342;

    // ── 菜单 (19350-19369) ──

    /// <summary>菜单编码不能为空</summary>
    public const int MenuCodeRequired = 19350;
    /// <summary>菜单名称不能为空</summary>
    public const int MenuNameRequired = 19351;
    /// <summary>创建菜单失败</summary>
    public const int MenuCreateFailed = 19352;
    /// <summary>查询菜单失败</summary>
    public const int MenuQueryFailed = 19353;
    /// <summary>菜单不存在</summary>
    public const int MenuNotFound = 19354;
    /// <summary>更新菜单失败</summary>
    public const int MenuUpdateFailed = 19355;
    /// <summary>删除菜单失败</summary>
    public const int MenuDeleteFailed = 19356;
    /// <summary>菜单编码已存在</summary>
    public const int MenuCodeExists = 18005;

    // ── 字典 (19370-19389) ──

    /// <summary>字典类型编码不能为空</summary>
    public const int DictTypeCodeRequired = 19370;
    /// <summary>字典项编码不能为空</summary>
    public const int DictItemCodeRequired = 19371;
    /// <summary>创建字典失败</summary>
    public const int DictCreateFailed = 19372;
    /// <summary>查询字典失败</summary>
    public const int DictQueryFailed = 19373;
    /// <summary>字典不存在</summary>
    public const int DictNotFound = 19374;
    /// <summary>更新字典失败</summary>
    public const int DictUpdateFailed = 19375;
    /// <summary>删除字典失败</summary>
    public const int DictDeleteFailed = 19376;

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
    /// <summary>套餐编码已存在</summary>
    public const int PackageCodeExists = 18006;
    /// <summary>已发布的套餐不可直接删除，需先下架</summary>
    public const int PackagePublishedCannotDelete = 19413;
    /// <summary>删除套餐失败</summary>
    public const int PackageDeleteFailed = 19414;
    /// <summary>套餐状态流转不允许</summary>
    public const int PackageStatusTransitionDenied = 19415;

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
    /// <summary>续费订阅失败</summary>
    public const int SubscriptionRenewFailed = 19506;
    /// <summary>升级订阅失败</summary>
    public const int SubscriptionUpgradeFailed = 19507;
    /// <summary>订阅状态不允许此操作</summary>
    public const int SubscriptionStatusDenied = 19508;

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
    /// <summary>标记发票支付失败</summary>
    public const int InvoicePayFailed = 19607;

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
    /// <summary>删除API密钥失败</summary>
    public const int ApiKeyDeleteFailed = 19710;

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
    /// <summary>删除通知模板失败</summary>
    public const int NotificationTemplateDeleteFailed = 19811;

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
