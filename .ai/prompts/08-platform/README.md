# 租户平台（Tenant Platform）— 业务总览

## 目标

实现 SaaS 多租户管控平台，提供租户管理、用户角色权限、套餐订阅计费、API 集成、审计运营等完整功能。

---

## 技术栈

- **后端项目**：`src/YTStdTenantPlatform/`（已完成，383 个测试全部通过）
- **新前端项目**：`src/WebTenantPlatfrom`（基于 DevExtreme Vue Application Template 重建，S5 F0 层 + F1-1 布局层 + F1-2 登录页已完成，`npm run build` 通过）
- **旧前端项目**：`web/tenant-platform-web/`（仅作参考，不得删除）
- **底层框架**：YTStdSqlBuilder、YTStdLogger、YTStdAdo、YTStdEntity、YTStdI18n（及其 Generator）
- **数据库**：PostgreSQL，表前缀 `sys_`
- **文档**：`docs/TenantPlatform/`

---

## 模块总览

### 核心模块（P5）

| 模块 | 后端提示词 | 前端提示词 | 后端状态 | 前端提示词状态 |
|------|-----------|-----------|---------|--------------|
| 认证 | `backend/auth-api.md` | —（登录页内嵌） | ✅ 已完成 | ✅ 已重写 |
| 平台用户 | `backend/platform-user-api.md` | `frontend/0021_platform-user-page.md` | ✅ 已完成 | ✅ 已重写 |
| 平台角色 | `backend/platform-role-api.md` | `frontend/0022_platform-role-page.md` | ✅ 已完成 | ✅ 已重写 |
| 平台权限 | `backend/platform-permission-api.md` | `frontend/0023_platform-permission-page.md` | ✅ 已完成 | ✅ 已重写 |
| 租户生命周期 | `backend/tenant-lifecycle-api.md` | `frontend/0024_tenant-page.md` | ✅ 已完成 | ✅ 已重写 |
| 租户信息 | `backend/tenant-info-api.md` | `frontend/0025_tenant-info-page.md` | ✅ 已完成 | ✅ 已重写 |
| 租户资源 | `backend/tenant-resource-api.md` | `frontend/0026_tenant-resource-page.md` | ✅ 已完成 | ✅ 已重写 |
| 租户配置 | `backend/tenant-config-api.md` | `frontend/0027_tenant-config-page.md` | ✅ 已完成 | ✅ 已重写 |

### 扩展模块（P6）

| 模块 | 后端提示词 | 前端提示词 | 后端状态 | 前端提示词状态 |
|------|-----------|-----------|---------|--------------|
| 套餐管理 | `backend/package-api.md` | `frontend/0028_package-page.md` | ✅ 已完成 | ✅ 已重写 |
| 订阅管理 | `backend/subscription-api.md` | `frontend/0029_subscription-page.md` | ✅ 已完成 | ✅ 已重写 |
| 计费账单 | `backend/billing-api.md` | `frontend/0030_billing-page.md` | ✅ 已完成 | ✅ 已重写 |
| API 集成 | `backend/api-integration-api.md` | `frontend/0031_api-integration-page.md` | ✅ 已完成 | ✅ 已重写 |
| 平台运营 | `backend/platform-operation-api.md` | `frontend/0035_platform-operation-page.md` | ✅ 已完成 | ✅ 已重写 |
| 审计日志 | `backend/audit-api.md` | `frontend/0032_audit-page.md` | ✅ 已完成 | ✅ 已重写 |
| 通知系统 | `backend/notification-api.md` | `frontend/0033_notification-page.md` | ✅ 已完成 | ✅ 已重写 |
| 文件存储 | `backend/storage-api.md` | `frontend/0034_storage-page.md` | ✅ 已完成 | ✅ 已重写 |

### 基础设施

| 内容 | 提示词 | 状态 |
|------|--------|------|
| 数据库设计 | `database/schema.md` | ✅ 已完成 |
| 初始化数据 | `database/seed-data.md` | ✅ 已完成 |
| 后端基础设施 | `backend/infrastructure.md` | ✅ 已完成 |
| 错误码 | `backend/error-codes.md` | ✅ 已完成 |
| 菜单与字典 | `backend/menu-dictionary-api.md` | ✅ 已完成 |
| 前端脚手架 | `frontend/0001_scaffold.md` | ✅ 已实现（S5 F0-1） |
| 前端布局 | `frontend/0010_layout.md` | ✅ 已实现（S5 F1-1） |
| 前端登录页 | `frontend/0011_login-page.md` | ✅ 已实现（S5 F1-2） |
| 前端首页 | `frontend/0020_dashboard-page.md` | ✅ 提示词已重写 |
| 前端国际化 | `frontend/0002_i18n.md` | ✅ 已实现（S5 F0-3） |
| 后端测试 | `testing/backend-tests.md` | ✅ 已完成（383 个测试通过） |
| Postman 集合 | `testing/postman.md` | ✅ 已完成 |

### 顶层模块概要文件

本目录根下的 `.md` 文件是各模块的功能概要与设计说明（非子目录内的 API/前端/数据库提示词）：

| 文件 | 说明 |
|------|------|
| `auth.md` | 认证模块功能概要 |
| `permission.md` | 权限模块功能概要 |
| `tenant.md` | 租户模块功能概要 |
| `config.md` | 配置模块功能概要 |
| `menu.md` | 菜单模块功能概要 |
| `dictionary.md` | 字典模块功能概要 |
| `audit.md` | 审计模块功能概要 |

---

## 执行顺序

### 阶段 A：数据库与基础设施校验

1. `database/schema.md` — 校验现有表结构是否符合新规范
2. `database/seed-data.md` — 校验初始化数据
3. `backend/infrastructure.md` — 校验后端基础设施
4. `backend/error-codes.md` — 校验错误码（const int、无 Messages 类）

### 阶段 B：核心模块后端重构

5. `backend/auth-api.md` — 认证 API
6. `backend/platform-user-api.md` — 平台用户 API
7. `backend/platform-role-api.md` — 平台角色 API
8. `backend/platform-permission-api.md` — 平台权限 API
9. `backend/tenant-lifecycle-api.md` — 租户生命周期 API
10. `backend/tenant-info-api.md` — 租户信息 API
11. `backend/tenant-resource-api.md` — 租户资源 API
12. `backend/tenant-config-api.md` — 租户配置 API
13. `backend/menu-dictionary-api.md` — 菜单与字典 API

### 阶段 C：扩展模块后端重构

14. `backend/package-api.md` — 套餐 API
15. `backend/subscription-api.md` — 订阅 API
16. `backend/billing-api.md` — 计费 API
17. `backend/api-integration-api.md` — API 集成
18. `backend/platform-operation-api.md` — 平台运营
19. `backend/audit-api.md` — 审计日志
20. `backend/notification-api.md` — 通知系统
21. `backend/storage-api.md` — 文件存储

### 阶段 D：后端测试

22. `testing/backend-tests.md` — 补齐测试
23. `testing/postman.md` — 更新 Postman 集合

### 阶段 E：前端重构

24. `frontend/0001_scaffold.md` — 校验前端工程
25. `frontend/0010_layout.md` — 校验布局
26. `frontend/0020_dashboard-page.md` — 首页仪表盘
27. `frontend/0021_platform-user-page.md` — 平台用户
28. `frontend/0022_platform-role-page.md` — 平台角色
29. `frontend/0023_platform-permission-page.md` — 平台权限
30. `frontend/0024_tenant-page.md` — 租户管理
31. `frontend/0025_tenant-info-page.md` — 租户信息
32. `frontend/0026_tenant-resource-page.md` — 租户资源
33. `frontend/0027_tenant-config-page.md` — 租户配置
34. `frontend/0028_package-page.md` — 套餐
35. `frontend/0029_subscription-page.md` — 订阅
36. `frontend/0030_billing-page.md` — 计费
37. `frontend/0031_api-integration-page.md` — API 集成
38. `frontend/0035_platform-operation-page.md` — 平台运营
39. `frontend/0032_audit-page.md` — 审计日志
40. `frontend/0033_notification-page.md` — 通知
41. `frontend/0034_storage-page.md` — 文件存储

### 阶段 F：前端国际化

42. `frontend/0002_i18n.md` — 全量国际化接入

---

## 前置阅读（全局）

所有任务开始前，Agent 必须阅读：

- `.ai/system/agent-contract.md`
- `.ai/rules/global.md`
- `.ai/context/tech-stack.md`
- `.ai/context/project-structure.md`
- `.ai/context/existing-modules.md`

---

## 文档

- 数据字典：`docs/TenantPlatform/database_dictionary.md`
- API 文档：`docs/TenantPlatform/API.md`
- 架构文档：`docs/TenantPlatform/architecture.md`

---

## 与通用提示词的关系

本目录下的提示词只包含**租户平台特有的业务细节**。通用规范（实体建模方式、服务层结构、端点写法、DTO 规范等）在 `.ai/prompts/02-backend/`、`.ai/prompts/03-frontend/` 等通用目录下定义，本目录提示词通过"前置阅读"引用。

如需开发其他业务系统（如 SaaS 租户侧），参照 `.ai/prompts/12-business-template/` 创建新的业务目录。
