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

**状态：🔄 进行中**

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

**状态：⏳ 待执行**

### D4. Postman 集合更新

**状态：⏳ 待执行**

- [ ] 22. `testing/backend-tests.md` — 补齐测试（D1 ✅ | D2 ⏳ | D3 ⏳）
- [ ] 23. `testing/postman.md` — 更新 Postman 集合（D4 ⏳）

## 阶段 E：前端重构

- [ ] 24~41 — 脚手架、布局、仪表盘及所有模块页面

## 阶段 F：前端国际化

- [ ] 42. `frontend/i18n.md` — 全量国际化接入