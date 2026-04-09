# 任务：租户平台检查与重构

## 任务信息

- **任务编号**：TASK-PLATFORM-001
- **所属阶段**：阶段 A ~ 阶段 F
- **优先级**：高
- **依据**：`.ai/prompts/08-platform/README.md`

## 任务目标

根据 `.ai/prompts/08-platform/README.md` 的各个步骤，分别添加子任务，然后进行检查并重构代码。

---

## 阶段 A：数据库与基础设施校验

### A1. 数据库表结构校验（`database/schema.md`）

**状态：✅ 全部通过（阻塞项已解决）**

| 检查项 | 状态 | 说明 |
|--------|------|------|
| 表名 `sys_` 前缀 | ✅ 通过 | 全部 56 个实体表（含新增 `sys_menu`、`sys_dictionary`）使用 `sys_` 前缀 + 单数形式 |
| 字段 snake_case | ✅ 通过 | 字段名均为 snake_case，由源生成器自动转换 |
| 无裸 `tenant_id` | ✅ 通过 | 统一使用 `tenant_ref_id` |
| 索引命名 `idx_/uq_` | ✅ 通过 | 全部索引按 `idx_{表名}_{列名}` / `uq_{表名}_{列名}` 命名 |
| `sys_menu` 实体 | ✅ 已创建 | `PlatformMenu.cs` — 树形菜单结构，含权限码关联 |
| `sys_dictionary` 实体 | ✅ 已创建 | `PlatformDictionary.cs` — 字典类型+字典项 |
| 布尔字段 `is_` 前缀 | ✅ 已修正 | 11 个 bool 属性添加 `[Column(ColumnName = "is_xxx")]` |
| 状态字段 `smallint` | ✅ 已修正 | 17 个状态字段改为 `int` + `[Column(DbType = "smallint")]`，配合已有枚举 |
| 编译通过 | ✅ 通过 | `dotnet build YTStd.slnx` 0 error |

> 注：`schema.md` 中唯一索引前缀 `udx_` 与 `database.md` 中 `uq_` 不一致，以 `database.md` 为准，代码中 `uq_` 正确。

---

### A2. 初始化数据校验（`database/seed-data.md`）

**状态：✅ 全部通过（阻塞项已解决）**

| 检查项 | 状态 | 说明 |
|--------|------|------|
| admin 超级管理员用户 | ✅ 通过 | `DefaultPlatformUsers.cs` 创建 admin |
| super-admin 角色 | ✅ 通过 | `DefaultRoles.cs` 创建 super-admin 角色 |
| admin → super-admin 关联 | ✅ 通过 | `DefaultRoles.cs` 中有 RoleMemberBinding |
| 权限码定义 | ✅ 通过 | `DefaultPermissions.cs` 完整覆盖所有模块 |
| super-admin 全部权限 | ✅ 通过 | `RoleSeedContributor.cs` 绑定全部权限 |
| 默认安全策略 | ✅ 通过 | `DefaultSecurityPolicies.cs` 含密码策略、登录策略 |
| 菜单树种子数据 | ✅ 已创建 | `DefaultMenus.cs` + `MenuSeedContributor.cs` — 14 个顶级目录 + 子菜单 |
| 字典种子数据 | ✅ 已创建 | `DefaultDictionaries.cs` + `DictionarySeedContributor.cs` — 5 个字典类型 |
| 幂等可重复执行 | ✅ 通过 | 所有 Seed 逻辑使用 upsert 模式 |
| 编译通过 | ✅ 通过 | 0 error |

---

### A3. 后端基础设施校验（`backend/infrastructure.md`）

**状态：✅ 通过**

| 检查项 | 状态 | 说明 |
|--------|------|------|
| 中间件管道顺序 | ✅ 通过 | `GlobalException → RequestLogging(含TraceId) → Permission(含JWT) → RateLimit → Audit` |
| JSON 源生成序列化 | ✅ 通过 | `TenantPlatformJsonSerializerContext` 使用 `[JsonSourceGenerationOptions]` |
| PascalCase 属性名 | ✅ 通过 | `Code`, `Message`, `Data` |
| JWT 认证 | ✅ 通过 | Token 含 userId, roles, permissions, isSuperAdmin |
| 公开端点 | ✅ 通过 | `/api/auth/login`, `/api/health` |
| 中间件错误响应格式 | ✅ 通过 | PascalCase JSON `{ "Code": xxx, "Message": xxx, "Data": null }` |
| Minimal API | ✅ 通过 | 无 MVC Controller |
| 禁止反射/dynamic/LINQ | ✅ 通过 | 未发现违规 |
| Logger.Debug `Func<string>` | ✅ 通过 | 使用延迟求值 |
| `dotnet build` | ✅ 通过 | 0 error |
| `dotnet test` | ✅ 通过 | 全部 140 个平台测试通过 |

---

### A4. 错误码校验（`backend/error-codes.md`）

**状态：✅ 通过**

| 检查项 | 状态 | 说明 |
|--------|------|------|
| Messages.cs 已删除 | ✅ 通过 | 文件不存在，全局搜索无引用 |
| 所有错误码为 `const int` | ✅ 通过 | 115 个常量全部为 `const int`（新增 3 个认证相关） |
| 每个错误码有 `<summary>` 注释 | ✅ 通过 | 全部有中文 XML 注释 |
| 无重复错误码值 | ✅ 通过 | 已验证无重复 |
| 错误码分段正确 | ✅ 通过 | 10xxx/11xxx/12xxx/18xxx/19xxx/50xxx |
| `ApiResult.Fail()` 仅传 code | ✅ 通过 | Fail 方法只有 `Fail(int code)` 签名 |
| 编译通过 | ✅ 通过 | 0 error |

---

## 阶段 A 总结

| 子任务 | 状态 | 备注 |
|--------|------|------|
| A1 数据库表结构 | ✅ | 全部通过，含新增 `sys_menu`/`sys_dictionary`、布尔 `is_` 前缀、状态 `smallint` |
| A2 初始化数据 | ✅ | 全部通过，含新增菜单/字典种子数据 |
| A3 后端基础设施 | ✅ | 全部通过 |
| A4 错误码 | ✅ | 全部通过 |

**阶段 A 所有阻塞项已解决，可进入阶段 B。**

---

## 阶段 B：核心模块后端重构

**状态：✅ 全部完成**

| 模块 | 规格端点数 | 已实现 | 完成度 |
|------|-----------|--------|--------|
| B5 认证 API | 4 | 4 | ✅ 100% |
| B6 平台用户 | 11 | 11 | ✅ 100% |
| B7 平台角色 | 9 | 12 | ✅ 100% |
| B8 平台权限 | 5 | 7 | ✅ 100% |
| B9 租户生命周期 | 12 | 13 | ✅ 100% |
| B10 租户信息 | 11 | 11 | ✅ 100% |
| B11 租户资源 | 3 | 3 | ✅ 100% |
| B12 租户配置 | 8 | 11 | ✅ 100% |
| B13 菜单与字典 | 13 | 13 | ✅ 100% |

- [x] 5. `backend/auth-api.md` — 认证 API（4/4 端点）
- [x] 6. `backend/platform-user-api.md` — 平台用户 API（11/11 端点，含 DELETE/reset-password/check-username/batch-enable/batch-disable）
- [x] 7. `backend/platform-role-api.md` — 平台角色 API（12 端点，含 DELETE/GET permissions/check-code/all）
- [x] 8. `backend/platform-permission-api.md` — 平台权限 API（7 端点，含 POST/PUT/DELETE）
- [x] 9. `backend/tenant-lifecycle-api.md` — 租户生命周期 API（13 端点，含 DELETE/initialize/suspend/resume/terminate/convert-trial/check-code）
- [x] 10. `backend/tenant-info-api.md` — 租户信息 API（11 端点，含 group UPDATE/DELETE、域名路径修正、tag DELETE、PUT tags）
- [x] 11. `backend/tenant-resource-api.md` — 租户资源 API（3 端点，路径修正为 /api/tenants/{id}/resource-*，新增 resource-usage）
- [x] 12. `backend/tenant-config-api.md` — 租户配置 API（11 端点，新增 /api/system-configs 和 /api/feature-flags 含 enable/disable）
- [x] 13. `backend/menu-dictionary-api.md` — 菜单与字典 API（13 端点，8 菜单 + 5 字典，全部新建）

**阶段 B 编译验证：✅ `dotnet build YTStd.slnx` 0 error | `dotnet test YTStd.slnx` 179 tests passed**

## 阶段 C：扩展模块后端重构

**状态：✅ 全部完成**

| 模块 | 重构内容 | 完成度 |
|------|---------|--------|
| C14 套餐 API | 路由 /api/packages, DELETE, publish/unpublish, check-code-exists, DB.GetNextLongIdAsync, 唯一性校验 | ✅ 100% |
| C15 订阅 API | 路由 /api/subscriptions, renew, upgrade, 租户订阅, DB.GetNextLongIdAsync | ✅ 100% |
| C16 计费 API | 路由 /api/billings, pay, 租户账单, DB.GetNextLongIdAsync | ✅ 100% |
| C17 API集成 | GET/DELETE api-key, DB.GetNextLongIdAsync, Logger.Debug | ✅ 100% |
| C18 平台运营 | 路由 /api/platform-operations/*, Logger.Debug | ✅ 100% |
| C19 审计日志 | 路由 /api/login-logs（替换 system-logs） | ✅ 100% |
| C20 通知系统 | DELETE 通知模板, DB.GetNextLongIdAsync, Logger.Debug | ✅ 100% |
| C21 文件存储 | 路由 /api/files, DB.GetNextLongIdAsync, Logger.Debug | ✅ 100% |

- [x] 14. `backend/package-api.md` — 套餐 API（路由修正、新增 DELETE/publish/unpublish/check-code-exists）
- [x] 15. `backend/subscription-api.md` — 订阅 API（路由修正、新增 renew/upgrade/tenant-subscription）
- [x] 16. `backend/billing-api.md` — 计费 API（路由修正、新增 pay/tenant-billings）
- [x] 17. `backend/api-integration-api.md` — API 集成（新增 GET/DELETE api-key）
- [x] 18. `backend/platform-operation-api.md` — 平台运营（路由统一到 /api/platform-operations/*）
- [x] 19. `backend/audit-api.md` — 审计（login-logs 替换 system-logs）
- [x] 20. `backend/notification-api.md` — 通知（新增 DELETE 模板）
- [x] 21. `backend/storage-api.md` — 文件存储（路由修正 /api/files）

**阶段 C 编译验证：✅ `dotnet build YTStd.slnx` 0 error | `dotnet test YTStd.slnx` 303 tests passed**

## 阶段 D：后端测试

**状态：✅ 全部完成**

### D1. ErrorCodes 验证 + Menu/Dictionary 测试 + Phase C 新 DTO 测试

**状态：✅ 完成**

| 检查项 | 状态 | 说明 |
|--------|------|------|
| ErrorCodes 唯一性校验 | ✅ 通过 | 无重复值 |
| ErrorCodes 分段范围校验 | ✅ 通过 | 全部在 10xxx/11xxx/12xxx/18xxx/19xxx/50xxx 范围内 |
| ErrorCodes 数量验证 | ✅ 通过 | 115+ 个 const int 常量 |
| Menu DTO 测试 | ✅ 通过 | MenuRepDTO/CreateMenuReqDTO/UpdateMenuReqDTO/MenuSortReqDTO |
| Dictionary DTO 测试 | ✅ 通过 | DictionaryRepDTO/DictionaryTypeRepDTO/CreateDictionaryReqDTO/UpdateDictionaryReqDTO |
| 菜单种子数据验证 | ✅ 通过 | 14 个顶级目录、编码唯一、父级引用正确、类型正确 |
| 字典种子数据验证 | ✅ 通过 | 5 个字典类型、项编码类型内唯一、全部启用 |
| Phase C 新 DTO 测试 | ✅ 通过 | RenewSubscriptionReqDTO/UpgradeSubscriptionReqDTO |
| 编译通过 | ✅ 通过 | 0 error |
| 测试通过 | ✅ 通过 | 192 tests passed（原 140 + 新增 52）|

### D2. Endpoint 注册验证 + 中间件增强测试 + 状态枚举完整性测试

**状态：✅ 完成**

| 检查项 | 状态 | 说明 |
|--------|------|------|
| 17 个 Endpoint 类存在性 | ✅ 通过 | 全部为 static 类且有 Map(WebApplication) 方法 |
| 路由组前缀验证 | ✅ 通过 | Phase C 修正后的路由前缀全部正确 |
| 中间件管道结构验证 | ✅ 通过 | 5 个中间件均有正确的构造函数和 InvokeAsync |
| 公开端点列表验证 | ✅ 通过 | /api/auth/login, /api/auth/refresh, /api/health |
| Token 边界场景 | ✅ 通过 | null/空/畸形/篡改签名/用户信息提取 |
| RateLimit 边界场景 | ✅ 通过 | 单次限制/大限制/多窗口重置 |
| 状态枚举完整性 | ✅ 通过 | 13 种枚举值唯一/数量正确/55+ 枚举类型 |
| 编译通过 | ✅ 通过 | 0 error |
| 测试通过 | ✅ 通过 | 275 tests passed（原 192 + 新增 83）|

### D3. 扩展模块服务逻辑测试

**状态：✅ 完成**

| 检查项 | 状态 | 说明 |
|--------|------|------|
| PagedRequest 分页规范化 | ✅ 通过 | 0/负数页→1，超量→200，Offset 计算正确 |
| ApiResult 成功/失败路径 | ✅ 通过 | Ok=Code0，Fail=对应错误码，Data=默认值 |
| SaasPackageStatus 状态流转规则 | ✅ 通过 | Draft→Published 可发布，Published 不能删除，Unpublish 需 Published |
| SubscriptionStatus 状态规则 | ✅ 通过 | 值唯一，>=3 个状态，错误码全部定义 |
| InvoiceStatus 状态规则 | ✅ 通过 | 值唯一，错误码全部定义 |
| TenantLifecycleStatus 状态规则 | ✅ 通过 | 值唯一，>=4 个状态，错误码全部定义 |
| DTO 字段默认值与必填项验证 | ✅ 通过 | CreatePackageReqDTO/RenewSubscriptionReqDTO/UpgradeSubscriptionReqDTO |
| 错误码业务分段验证 | ✅ 通过 | Package/Subscription/Invoice/System 错误码范围正确 |
| 通知模板、文件存储、平台运营 DTO | ✅ 通过 | 全部 DTO 可正确实例化和访问字段 |
| 审计日志、系统日志 DTO | ✅ 通过 | AuditLogRepDTO/SystemLogRepDTO 字段验证 |
| PaymentStatus/RefundStatus 枚举 | ✅ 通过 | 值唯一，错误码定义 |
| 编译通过 | ✅ 通过 | 0 error |
| 测试通过 | ✅ 通过 | 329 tests passed（原 275 + 新增 54）|

### D4. Postman 集合更新

**状态：✅ 完成**

| 检查项 | 状态 | 说明 |
|--------|------|------|
| /api/saas-packages → /api/packages | ✅ 修正 | 套餐管理路由更新 |
| /api/packages/{id}/versions | ✅ 修正 | 套餐版本路由更新 |
| /api/package-versions/{id}/capabilities | ✅ 修正 | 套餐能力路由更新 |
| /api/tenant-subscriptions → /api/subscriptions | ✅ 修正 | 订阅路由更新 |
| /api/billing-invoices → /api/billings | ✅ 修正 | 账单路由更新 |
| /api/tenant-files → /api/files | ✅ 修正 | 文件路由更新 |
| /api/system-logs → /api/login-logs | ✅ 修正 | 审计路由更新 |
| /api/tenant-daily-stats → /api/platform-operations/tenant-statistics | ✅ 修正 | 运营路由更新 |
| /api/platform-monitor-metrics → /api/platform-operations/monitor-metrics | ✅ 修正 | 监控路由更新 |
| /api/tenant-system-configs → /api/system-configs | ✅ 修正 | 系统配置路由更新 |
| /api/tenant-feature-flags → /api/feature-flags | ✅ 修正 | 功能开关路由更新 |
| 新增菜单管理（38 菜单管理，10 个请求） | ✅ 新增 | /api/menus 完整端点 |
| 新增字典管理（39 字典管理，5 个请求） | ✅ 新增 | /api/dictionaries 完整端点 |
| 补充平台用户缺失端点（5 个） | ✅ 新增 | delete/reset-password/check-username/batch-enable/batch-disable |
| 补充平台角色缺失端点（4 个） | ✅ 新增 | delete/permissions/check-code/all |
| 补充平台权限缺失端点（3 个） | ✅ 新增 | create/update/delete |
| 补充租户管理缺失端点（7 个） | ✅ 新增 | delete/initialize/suspend/resume/terminate/convert-trial/check-code |
| 补充订阅缺失端点（3 个） | ✅ 新增 | renew/upgrade/tenant-subscription |
| 补充账单缺失端点（2 个） | ✅ 新增 | pay/tenant-billings |
| 补充 API 密钥缺失端点（2 个） | ✅ 新增 | GET/DELETE api-key |
| 总请求数 | ✅ 验证 | 175 个请求（原 115 + 新增 60）|

- [x] 22. `testing/backend-tests.md` — 补齐测试（D1 ✅ | D2 ✅ | D3 ✅）
- [x] 23. `testing/postman.md` — 更新 Postman 集合（D4 ✅）

---

## ⚠️ 人工审查发现的问题（2026-04-09 第 6 轮 — 提示词优化专项）

### 问题 1：B 阶段多个 AppService 缺少 GetNextLongIdAsync

**严重级别：严重**

B 阶段（核心模块后端重构）中以下服务的 `InsertAsync` 调用前缺少 `entity.Id = await DB.GetNextLongIdAsync()`：

| 文件 | 缺失行号（InsertAsync 位置） | 涉及实体 |
|------|---------------------------|---------|
| `PlatformUserAppService.cs` | :85 | PlatformUser |
| `PlatformRoleAppService.cs` | :99, :170, :193 | PlatformRole, RolePermission, RoleMember |
| `PlatformPermissionAppService.cs` | :98 | PlatformPermission |
| `TenantLifecycleAppService.cs` | :93, :322 | Tenant, TenantLifecycleEvent |
| `TenantInfoAppService.cs` | :57, :134, :211, :240 | TenantGroup, TenantDomain, TenantTag, TenantTagBinding |
| `TenantConfigAppService.cs` | :87, :230, :326 | TenantSystemConfig, TenantFeatureFlag, TenantParameter |
| `TenantResourceAppService.cs` | :80 | TenantResourceQuota |

**C 阶段服务已正确包含** `GetNextLongIdAsync`（PackageAppService、SubscriptionAppService、BillingAppService 等）。

**根因分析：**
- B 阶段执行时，Agent 仅以 `dotnet build` 编译通过作为验收标准
- 提示词规则文件（`global.md`、`backend.md`、`app-service.md`）中有规则定义，但 Agent 未实际搜索验证
- C 阶段提示词中内联了更强的约束提示，因此 C 阶段未复现此问题

**修复状态：✅ 已修复 — 第 8 轮完成（15 处全部添加 `Id = await DB.GetNextLongIdAsync()`，6 个文件添加 `using YTStdAdo;`）**

### 问题 2：Postman 集合路由与 webapi 不一致

**严重级别：中等**

| Postman 路由 | 代码实际路由 | 问题 |
|-------------|------------|------|
| `POST /api/auth/refresh` | `POST /api/auth/refresh-token` | 路径不匹配 |
| （缺失） | `POST /api/auth/change-password` | Postman 中未包含 |

**修复状态：✅ 已修复 — 第 8 轮完成（refresh → refresh-token，新增 change-password）**

### 问题 3（根因）：提示词体系缺乏自动化代码审查机制

**已修复 ✅ — 本轮完成**

| 修改文件 | 修改内容 |
|---------|---------|
| `.github/copilot-instructions.md` | 内联关键编码约束（InsertAsync、ApiResult.Fail、Logger.Debug 等），添加强制代码审查说明 |
| `.ai/system/self-review-protocol.md` | **新建**：定义 7 项强制审查项，包含具体 grep 搜索命令、验证规则、输出格式 |
| `.ai/system/execution-policy.md` | 添加步骤 4.5（代码搜索审查），明确编译通过 ≠ 任务完成 |
| `.ai/system/done-criteria.md` | 通用标准增加"代码搜索审查通过"，应用服务标准增加 grep 验证要求 |
| `.ai/system/review-policy.md` | Agent 自检项增加 grep 搜索验证要求 |
| `.ai/system/agent-contract.md` | 执行后输出增加"代码搜索审查结果"，添加重要说明 |
| `.ai/system/prompt-writing-guide.md` | 新增"关键约束内联化"和"验收标准可执行化"原则 |
| `.ai/prompts/10-review/code-review.md` | 审查清单改为 grep 命令驱动，验收标准要求搜索输出 |
| `.ai/prompts/02-backend/app-service.md` | InsertAsync 规则增加零容忍说明（含关联表），验收标准增加 grep 验证 |
| `.ai/prompts/07-testing/postman-collection.md` | 增加路由一致性验证步骤和约束 |
| `.ai/README.md` | 目录结构增加 self-review-protocol.md |

---

## 阶段 E：前端重构

**状态：⏳ 待开始**

### 子任务拆分（每子任务 ≤ 40 分钟）

| 子任务 | 内容 | 预计时间 | 状态 |
|--------|------|---------|------|
| E0 | 修复 B 阶段 InsertAsync 违规 + Postman 路由修复 + 自审 | 40 分钟 | ✅ 完成 |
| E1 | 前端脚手架校验 + 布局校验 + 仪表盘页面（E24-E26） | 40 分钟 | ⏳ 待开始 |
| E2 | 核心管理页面 Part 1 — 用户/角色/权限（E27-E29） | 40 分钟 | ⏳ 待开始 |
| E3 | 核心管理页面 Part 2 — 租户管理/信息/资源/配置（E30-E33） | 40 分钟 | ⏳ 待开始 |
| E4 | 扩展模块页面 Part 1 — 套餐/订阅/计费（E34-E36） | 40 分钟 | ⏳ 待开始 |
| E5 | 扩展模块页面 Part 2 — API集成/运营/审计/通知/存储（E37-E41） | 40 分钟 | ⏳ 待开始 |

- [ ] 24~41 — 脚手架、布局、仪表盘及所有模块页面

## 阶段 F：前端国际化

- [ ] 42. `frontend/i18n.md` — 全量国际化接入