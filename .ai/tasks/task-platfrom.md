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

**状态：⚠️ 大部分通过，有遗留问题**

#### ✅ 已通过

| 检查项 | 状态 | 说明 |
|--------|------|------|
| 表名 `sys_` 前缀 | ✅ 通过 | 全部 54 个实体表已改为 `sys_` 前缀 + 单数形式 |
| 字段 snake_case | ✅ 通过 | 字段名均为 snake_case，由源生成器自动转换 |
| 无裸 `tenant_id` | ✅ 通过 | 统一使用 `tenant_ref_id` |
| 索引命名 `idx_/uq_` | ✅ 通过 | 全部 48 个索引按 `idx_{表名}_{列名}` / `uq_{表名}_{列名}` 命名 |
| 编译通过 | ✅ 通过 | `dotnet build YTStd.slnx` 0 error |

#### ❌ 遗留问题

| 编号 | 问题 | 严重程度 | 说明 |
|------|------|---------|------|
| A1-1 | 缺少 `sys_menu` 实体 | 🔴 阻塞 | `schema.md` 要求菜单表，当前无对应实体 |
| A1-2 | 缺少 `sys_dictionary` 实体 | 🔴 阻塞 | `schema.md` 要求字典表，当前无对应实体 |
| A1-3 | 布尔字段未统一使用 `is_` 前缀 | 🟡 中等 | 如 `MfaEnabled` → 应映射 `is_mfa_enabled`；`Enabled` → `is_enabled`；`AutoRenew` → `is_auto_renew` 等 |
| A1-4 | 状态字段使用 `varchar(32)` 而非 `smallint` | 🟡 中等 | `database.md` 明确要求 `smallint` + 枚举，当前全部用字符串 |
| A1-5 | `schema.md` 中唯一索引前缀写为 `udx_`，与 `database.md` 中 `uq_` 不一致 | 🟢 低 | 以 `database.md` 为准，代码中 `uq_` 正确 |

---

### A2. 初始化数据校验（`database/seed-data.md`）

**状态：⚠️ 部分通过，有阻塞问题**

#### ✅ 已通过

| 检查项 | 状态 | 说明 |
|--------|------|------|
| admin 超级管理员用户 | ✅ 通过 | `DefaultPlatformUsers.cs` 创建 admin |
| super-admin 角色 | ✅ 通过 | `DefaultRoles.cs` 创建 super-admin 角色 |
| admin → super-admin 关联 | ✅ 通过 | `DefaultRoles.cs` 中有 RoleMemberBinding |
| 权限码定义 | ✅ 通过 | `DefaultPermissions.cs` 完整覆盖所有模块 |
| super-admin 全部权限 | ✅ 通过 | `RoleSeedContributor.cs` 绑定全部权限 |
| 默认安全策略 | ✅ 通过 | `DefaultSecurityPolicies.cs` 含密码策略、登录策略 |
| 幂等可重复执行 | ✅ 通过 | Seed 逻辑使用 upsert 模式 |

#### ❌ 遗留问题

| 编号 | 问题 | 严重程度 | 说明 |
|------|------|---------|------|
| A2-1 | 无菜单树种子数据 | 🔴 阻塞 | `sys_menu` 实体不存在，无法种子化 |
| A2-2 | 无字典种子数据 | 🔴 阻塞 | `sys_dictionary` 实体不存在，无法种子化 |

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
| 所有错误码为 `const int` | ✅ 通过 | 112 个常量全部为 `const int` |
| 每个错误码有 `<summary>` 注释 | ✅ 通过 | 全部有中文 XML 注释 |
| 无重复错误码值 | ✅ 通过 | 已验证无重复 |
| 错误码分段正确 | ✅ 通过 | 10xxx/11xxx/12xxx/18xxx/19xxx/50xxx |
| `ApiResult.Fail()` 仅传 code | ✅ 通过 | Fail 方法只有 `Fail(int code)` 签名 |
| 编译通过 | ✅ 通过 | 0 error |

---

## 阶段 A 总结

| 子任务 | 状态 | 备注 |
|--------|------|------|
| A1 数据库表结构 | ⚠️ | 缺 `sys_menu`、`sys_dictionary` 实体；布尔字段 `is_` 前缀待补；状态字段 `smallint` 待改 |
| A2 初始化数据 | ⚠️ | 缺菜单和字典种子数据（依赖 A1 实体创建） |
| A3 后端基础设施 | ✅ | 全部通过 |
| A4 错误码 | ✅ | 全部通过 |

### 阶段 B 前需解决的阻塞项

1. **创建 `sys_menu` 实体和种子数据** — `backend/menu-dictionary-api.md` 模块依赖
2. **创建 `sys_dictionary` 实体和种子数据** — `backend/menu-dictionary-api.md` 模块依赖
3. **布尔字段 `is_` 前缀修正** — 影响数据库实际列名，应尽早处理
4. **状态字段 `smallint` + 枚举** — 影响所有模块的 DTO 和查询，应尽早处理

---

## 阶段 B：核心模块后端重构

- [ ] 5. `backend/auth-api.md` — 认证 API
- [ ] 6. `backend/platform-user-api.md` — 平台用户 API
- [ ] 7. `backend/platform-role-api.md` — 平台角色 API
- [ ] 8. `backend/platform-permission-api.md` — 平台权限 API
- [ ] 9. `backend/tenant-lifecycle-api.md` — 租户生命周期 API
- [ ] 10. `backend/tenant-info-api.md` — 租户信息 API
- [ ] 11. `backend/tenant-resource-api.md` — 租户资源 API
- [ ] 12. `backend/tenant-config-api.md` — 租户配置 API
- [ ] 13. `backend/menu-dictionary-api.md` — 菜单与字典 API

## 阶段 C：扩展模块后端重构

- [ ] 14~21 — 套餐、订阅、计费、API集成、运营、审计、通知、存储

## 阶段 D：后端测试

- [ ] 22. `testing/backend-tests.md` — 补齐测试
- [ ] 23. `testing/postman.md` — 更新 Postman 集合

## 阶段 E：前端重构

- [ ] 24~41 — 脚手架、布局、仪表盘及所有模块页面

## 阶段 F：前端国际化

- [ ] 42. `frontend/i18n.md` — 全量国际化接入