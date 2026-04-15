# 租户平台 — 前端总览

> 本文件是平台前端模块的总览入口，定义所有模块的执行顺序、依赖关系和当前状态。
> 新 Agent 接手平台前端任务时，必须先阅读本文件。

---

## 一、前置阅读

执行任何平台前端模块任务前，必须先阅读以下文件：

| 序号 | 文件 | 用途 |
| ------ | ------ | ------ |
| 1 | `.ai/prompts/03-frontend/00-governance.md` | 前端总治理 |
| 2 | `.ai/prompts/03-frontend/01-task-splitting.md` | 任务拆分规范 |
| 3 | `.ai/prompts/03-frontend/04-devextreme-templates.md` | DevExtreme 规范 |
| 4 | `.ai/prompts/03-frontend/05-axios-standard.md` | axios 规范 |
| 5 | `.ai/prompts/03-frontend/06-i18n-execution.md` | i18n 规范 |
| 6 | `.ai/prompts/03-frontend/03-anti-patterns.md` | 反模式清单 |
| 7 | `.ai/prompts/03-frontend/08-playwright-e2e.md` | Playwright E2E 测试规范 |
| 8 | `.ai/prompts/03-frontend/09-production-defaults.md` | 生产级默认能力补全 |
| 9 | `.ai/prompts/03-frontend/10-component-asset-catalog.md` | 组件资产目录与复用决策 |
| 10 | `.ai/prompts/03-frontend/11-delivery-workflow.md` | 任务快照与交付流程 |
| 11 | `.ai/prompts/03-frontend/12-github-automation-workflow.md` | GitHub 顺序执行 workflow |
| 12 | `.ai/prompts/03-frontend/13-prompt-evolution-workflow.md` | 缺陷沉淀与提示词迭代 |
| 13 | 本文件 | 模块清单与执行顺序 |

---

## 二、项目信息

| 属性 | 值 |
| ------ | --- |
| 项目路径 | `src/WebTenantPlatfrom` |
| 技术栈 | Vue 3 + TypeScript + Vite + DevExtreme Vue 25.2+ + axios + Pinia + vue-i18n |
| Application Template | DevExtreme Vue Application Template |
| UI Templates | DevExtreme Vue UI Templates |
| 旧项目路径 | `web/tenant-platform-web`（仅参考，不修改） |

---

## 二点五、执行方式升级

1. 本目录下的 `00xx_*.md` 文件是 **模块 Epic 合同**，不是一次性执行单元。
2. 实际执行时，必须先按 `.ai/prompts/03-frontend/01-task-splitting.md` 把模块拆成 slice。
3. GitHub Copilot coding agent 只应被指派到单个 slice issue，而不是整页模块。
4. 每轮必须按 `.ai/prompts/03-frontend/11-delivery-workflow.md` 更新任务快照和 session summary。

---

## 三、模块清单与执行顺序

### 层级 0：基础设施（串行）

| 编号 | 模块 | 提示词 | 状态 |
| ------ | ------ | -------- | ------ |
| F0-1 | 脚手架搭建 | `0001_scaffold.md`（✅ 已重写） | ✅ |
| F0-2 | axios 封装 | `03-frontend/05-axios-standard.md` | ✅ |
| F0-3 | i18n 基础设施 | `0002_i18n.md`（✅ 已重写） / `03-frontend/06-i18n-execution.md` | ✅ |
| F0-4 | 路由与权限守卫 | 包含在 `0010_layout.md` 中 | ✅ |
| F0-5 | 状态管理 | 包含在 `0010_layout.md` 中 | ✅ |
| F0-6 | 通用组件 | 包含在 `0010_layout.md` 中 | ✅ |

### 层级 1：布局（串行）

| 编号 | 模块 | 提示词 | 状态 |
| ------ | ------ | -------- | ------ |
| F1-1 | 主布局 | `0010_layout.md`（✅ 已重写） | ✅ |
| F1-2 | 登录页 | `0011_login-page.md`（✅ 已创建） | ✅ |

### 层级 2：业务页面（可并行）

| 编号 | 模块 | 提示词 | 状态 | 并行组 |
| ------ | ------ | -------- | ------ | ----- |
| F2-1 | 仪表盘 | `0020_dashboard-page.md`（✅ 已重写） | ✅ | - |
| F2-2 | 平台用户管理 | `0021_platform-user-page.md`（✅ 已重写） | ✅ | A |
| F2-3 | 平台角色管理 | `0022_platform-role-page.md`（✅ 已重写） | ✅ | A |
| F2-4 | 平台权限管理 | `0023_platform-permission-page.md`（✅ 已重写） | ✅ | A |
| F2-5 | 租户管理 | `0024_tenant-page.md`（✅ 已重写） | ⬜ | B |
| F2-6 | 租户信息管理 | `0025_tenant-info-page.md`（✅ 已重写） | ⬜ | B |
| F2-7 | 租户资源管理 | `0026_tenant-resource-page.md`（✅ 已重写） | ⬜ | B |
| F2-8 | 租户配置管理 | `0027_tenant-config-page.md`（✅ 已重写） | ⬜ | B |
| F2-9 | 套餐管理 | `0028_package-page.md`（✅ 已重写） | ⬜ | C |
| F2-10 | 订阅管理 | `0029_subscription-page.md`（✅ 已重写） | ⬜ | C |
| F2-11 | 账单管理 | `0030_billing-page.md`（✅ 已重写） | ⬜ | C |
| F2-12 | API 集成 | `0031_api-integration-page.md`（✅ 已重写） | ⬜ | D |
| F2-13 | 审计日志 | `0032_audit-page.md`（✅ 已重写） | ⬜ | D |
| F2-14 | 通知管理 | `0033_notification-page.md`（✅ 已重写） | ⬜ | D |
| F2-15 | 文件管理 | `0034_storage-page.md`（✅ 已重写） | ⬜ | D |
| F2-16 | 平台运营 | `0035_platform-operation-page.md`（✅ 已重写） | ⬜ | D |

### 层级 3：集成验证（串行）

| 编号 | 模块 | 提示词 | 状态 |
| ------ | ------ | -------- | ------ |
| F3-1 | 全量编译验证 | 无需独立提示词 | ⬜ |
| F3-2 | i18n 完整性验证 | 无需独立提示词 | ⬜ |
| F3-3 | 权限完整性验证 | 无需独立提示词 | ⬜ |

### E2E 测试（每个模块完成后立即执行）

| 编号 | 模块 | 提示词 | 状态 |
| ------ | ------ | -------- | ------ |
| E2E | Playwright E2E 测试协议 | `0040_e2e-testing-protocol.md` | ✅ |

> **每个业务模块（F2-1 ~ F2-16）编码完成后，必须按 `0040_e2e-testing-protocol.md` 编写并执行 E2E 测试。**
> 环境预检与迭代流程见 `.ai/system/e2e-testing-workflow.md`。
> Playwright 测试规范见 `.ai/prompts/03-frontend/08-playwright-e2e.md`。

### 子任务执行体系

> 从 S14 起，F2-5 ~ F2-16 的开发任务已拆分为独立的子任务文件。
> 详见 `.ai/tasks/platform-frontend/README.md`。
> 每个子任务文件现在视为 **模块 Epic**；单次会话只执行其中一个 slice，确保 Agent 在单次会话内完整交付。

---

## 四、菜单结构（与后端对齐）

```text
├── 首页/仪表盘          → F2-1
├── 平台管理
│   ├── 用户管理          → F2-2
│   ├── 角色管理          → F2-3
│   └── 权限管理          → F2-4
├── 租户管理
│   ├── 租户列表          → F2-5
│   ├── 租户信息          → F2-6
│   ├── 资源配额          → F2-7
│   └── 配置与开关        → F2-8
├── SaaS 运营
│   ├── 套餐管理          → F2-9
│   ├── 订阅管理          → F2-10
│   ├── 账单管理          → F2-11
│   └── 平台运营          → F2-16
├── API 集成              → F2-12
└── 系统管理
    ├── 审计日志          → F2-13
    ├── 通知管理          → F2-14
    └── 文件管理          → F2-15
```

---

## 五、后端 API 文档索引

| 模块 | 后端 API 提示词 |
| ------ | ---------------- |
| 认证 | `.ai/prompts/08-platform/backend/auth-api.md` |
| 平台用户 | `.ai/prompts/08-platform/backend/platform-user-api.md` |
| 平台角色 | `.ai/prompts/08-platform/backend/platform-role-api.md` |
| 平台权限 | `.ai/prompts/08-platform/backend/platform-permission-api.md` |
| 租户生命周期 | `.ai/prompts/08-platform/backend/tenant-lifecycle-api.md` |
| 租户信息 | `.ai/prompts/08-platform/backend/tenant-info-api.md` |
| 租户资源 | `.ai/prompts/08-platform/backend/tenant-resource-api.md` |
| 租户配置 | `.ai/prompts/08-platform/backend/tenant-config-api.md` |
| 套餐管理 | `.ai/prompts/08-platform/backend/package-api.md` |
| 订阅管理 | `.ai/prompts/08-platform/backend/subscription-api.md` |
| 账单管理 | `.ai/prompts/08-platform/backend/billing-api.md` |
| API 集成 | `.ai/prompts/08-platform/backend/api-integration-api.md` |
| 审计日志 | `.ai/prompts/08-platform/backend/audit-api.md` |
| 通知管理 | `.ai/prompts/08-platform/backend/notification-api.md` |
| 文件管理 | `.ai/prompts/08-platform/backend/storage-api.md` |
| 平台运营 | `.ai/prompts/08-platform/backend/platform-operation-api.md` |
| 菜单与字典 | `.ai/prompts/08-platform/backend/menu-dictionary-api.md` |
| 错误码 | `.ai/prompts/08-platform/backend/error-codes.md` |

---

## 六、文件命名规范

本目录下所有提示词文件必须遵循 `00xx_xxx.md` 或 `00xx_xxx_async.md` 命名规则：

| 编号范围 | 层级 | 说明 |
| -------- | ------ | ------ |
| 0000 | 总览 | 本文件（`0000_overview.md`） |
| 0001-0009 | 层级 0 | 基础设施（脚手架、i18n 等） |
| 0010-0019 | 层级 1 | 布局层（主布局、登录页） |
| 0020-0039 | 层级 2 | 业务页面 |
| 9900+ | 归档 | 已归档的历史文件 |

**新增文件时**：在对应层级编号范围内顺序递增。`_async` 后缀表示该任务可并行执行。

---

## 七、状态标记说明

| 标记 | 含义 |
| ------ | ------ |
| ⬜ | 未开始 |
| 🔄 | 进行中 |
| ✅ | 已完成且验收通过 |
| ❌ | 已完成但验收未通过 |
