# 租户平台 — 错误码定义

## 目标

校验并完善租户平台错误码定义，确保符合"后端零文本"国际化规范。

---

## 前置阅读

- `.ai/rules/i18n.md` — 国际化规范
- `.ai/prompts/05-i18n/backend-i18n.md` — 后端国际化提示词

---

## 现有文件

- `Application/Constants/ErrorCodes.cs` — 错误码常量
- `Application/Constants/Messages.cs` — **需要删除**（违反后端零文本原则）

---

## 重构要点

### 1. 删除 Messages.cs

后端零文本原则：后端不存在 Messages 类。所有错误消息通过前端 i18n 翻译。

### 2. ErrorCodes 格式

```csharp
public static class ErrorCodes
{
    // ===== 认证（10xxx）=====
    /// <summary>无效的凭证</summary>
    public const int InvalidCredentials = 10001;
    /// <summary>账户已锁定</summary>
    public const int AccountLocked = 10002;
    /// <summary>Token 已过期</summary>
    public const int TokenExpired = 10003;

    // ===== 权限（11xxx）=====
    /// <summary>权限不足</summary>
    public const int PermissionDenied = 11001;

    // ===== 输入验证（12xxx）=====
    /// <summary>必填字段缺失</summary>
    public const int RequiredFieldMissing = 12001;

    // ===== 唯一性冲突（18xxx）=====
    /// <summary>用户名已存在</summary>
    public const int UsernameExists = 18001;
    /// <summary>角色编码已存在</summary>
    public const int RoleCodeExists = 18002;
    /// <summary>租户编码已存在</summary>
    public const int TenantCodeExists = 18003;

    // ===== 业务规则（19xxx）=====
    /// <summary>用户不存在</summary>
    public const int UserNotExist = 19001;
    /// <summary>租户状态不允许此操作</summary>
    public const int TenantStatusNotAllowed = 19002;

    // ===== 系统错误（50xxx）=====
    /// <summary>内部服务器错误</summary>
    public const int InternalError = 50001;
}
```

### 3. 所有 ApiResult.Fail() 调用

```csharp
// ✅ 正确 — 仅传 ErrorCode
return ApiResult.Fail(ErrorCodes.UserNotExist);

// ❌ 禁止 — 不传 message 参数
return ApiResult.Fail(ErrorCodes.UserNotExist, "some message");
```

---

## 错误码分段分配

| 范围 | 模块 | 说明 |
|------|------|------|
| 10001-10099 | 认证 | 登录、Token |
| 11001-11099 | 权限 | 权限不足 |
| 12001-12099 | 输入验证 | 参数校验 |
| 18001-18099 | 唯一性冲突 | 用户名/角色码/租户码等重复 |
| 19001-19099 | 平台用户 | 用户业务规则 |
| 19101-19199 | 平台角色 | 角色业务规则 |
| 19201-19299 | 权限 | 权限业务规则 |
| 19301-19399 | 租户 | 租户业务规则 |
| 19401-19499 | 套餐 | 套餐业务规则 |
| 19501-19599 | 订阅 | 订阅业务规则 |
| 19601-19699 | 计费 | 计费业务规则 |
| 19701-19799 | API 集成 | API Key 业务规则 |
| 19801-19899 | 通知 | 通知业务规则 |
| 19901-19999 | 文件存储 | 文件业务规则 |
| 50001-50099 | 系统 | 内部错误 |

---

## 验收标准

- [ ] Messages.cs 已删除
- [ ] 所有错误码为 `const int`
- [ ] 每个错误码有 `<summary>` 中文注释
- [ ] 无重复错误码值
- [ ] 所有 `ApiResult.Fail()` 仅传 `ErrorCodes.XXX`（全局搜索验证）
- [ ] 编译通过
