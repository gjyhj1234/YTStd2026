# 租户平台 — 前端总览

> 本文件是平台前端模块的总览入口，定义所有模块的执行顺序、依赖关系和当前状态。
> 新 Agent 接手平台前端任务时，必须先阅读本文件。

---

## 一、前置阅读

执行任何平台前端模块任务前，必须先阅读以下文件：

| 序号 | 文件 | 用途 |
|:----:|------|------|
| 1 | `.ai/prompts/03-frontend/00-governance.md` | 前端总治理 |
| 2 | `.ai/prompts/03-frontend/01-task-splitting.md` | 任务拆分规范 |
| 3 | `.ai/prompts/03-frontend/04-devextreme-templates.md` | DevExtreme 规范 |
| 4 | `.ai/prompts/03-frontend/05-axios-standard.md` | axios 规范 |
| 5 | `.ai/prompts/03-frontend/06-i18n-execution.md` | i18n 规范 |
| 6 | `.ai/prompts/03-frontend/03-anti-patterns.md` | 反模式清单 |
| 7 | 本文件 | 模块清单与执行顺序 |

---

## 二、项目信息

| 属性 | 值 |
|------|---|
| 项目路径 | `src/WebTenantPlatfrom` |
| 技术栈 | Vue 3 + TypeScript + Vite + DevExtreme Vue 25.2+ + axios + Pinia + vue-i18n |
| Application Template | DevExtreme Vue Application Template |
| UI Templates | DevExtreme Vue UI Templates |
| 旧项目路径 | `web/tenant-platform-web`（仅参考，不修改） |

---

## 三、模块清单与执行顺序

### 层级 0：基础设施（串行）

| 编号 | 模块 | 提示词 | 状态 |
|:----:|------|--------|:----:|
| F0-1 | 脚手架搭建 | 待创建 | ⬜ |
| F0-2 | axios 封装 | `03-frontend/05-axios-standard.md` | ⬜ |
| F0-3 | i18n 基础设施 | `03-frontend/06-i18n-execution.md` | ⬜ |
| F0-4 | 路由与权限守卫 | 待创建 | ⬜ |
| F0-5 | 状态管理 | 待创建 | ⬜ |
| F0-6 | 通用组件 | 待创建 | ⬜ |

### 层级 1：布局（串行）

| 编号 | 模块 | 提示词 | 状态 |
|:----:|------|--------|:----:|
| F1-1 | 主布局 | `layout.md`（待重写） | ⬜ |
| F1-2 | 登录页 | `login-page.md`（待创建） | ⬜ |

### 层级 2：业务页面（可并行）

| 编号 | 模块 | 提示词 | 状态 | 并行组 |
|:----:|------|--------|:----:|:-----:|
| F2-1 | 仪表盘 | `dashboard-page.md`（待重写） | ⬜ | - |
| F2-2 | 平台用户管理 | `platform-user-page.md`（已重写） | ⬜ | A |
| F2-3 | 平台角色管理 | `platform-role-page.md`（待重写） | ⬜ | A |
| F2-4 | 平台权限管理 | `platform-permission-page.md`（待重写） | ⬜ | A |
| F2-5 | 租户管理 | `tenant-page.md`（待重写） | ⬜ | B |
| F2-6 | 租户信息管理 | `tenant-info-page.md`（待重写） | ⬜ | B |
| F2-7 | 租户资源管理 | `tenant-resource-page.md`（待重写） | ⬜ | B |
| F2-8 | 租户配置管理 | `tenant-config-page.md`（待重写） | ⬜ | B |
| F2-9 | 套餐管理 | `package-page.md`（待重写） | ⬜ | C |
| F2-10 | 订阅管理 | `subscription-page.md`（待重写） | ⬜ | C |
| F2-11 | 账单管理 | `billing-page.md`（待重写） | ⬜ | C |
| F2-12 | API 集成 | `api-integration-page.md`（待重写） | ⬜ | D |
| F2-13 | 审计日志 | `audit-page.md`（待重写） | ⬜ | D |
| F2-14 | 通知管理 | `notification-page.md`（待重写） | ⬜ | D |
| F2-15 | 文件管理 | `storage-page.md`（待重写） | ⬜ | D |

### 层级 3：集成验证（串行）

| 编号 | 模块 | 提示词 | 状态 |
|:----:|------|--------|:----:|
| F3-1 | 全量编译验证 | 无需独立提示词 | ⬜ |
| F3-2 | i18n 完整性验证 | 无需独立提示词 | ⬜ |
| F3-3 | 权限完整性验证 | 无需独立提示词 | ⬜ |

---

## 四、菜单结构（与后端对齐）

```
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
│   └── 账单管理          → F2-11
├── API 集成              → F2-12
└── 系统管理
    ├── 审计日志          → F2-13
    ├── 通知管理          → F2-14
    └── 文件管理          → F2-15
```

---

## 五、后端 API 文档索引

| 模块 | 后端 API 提示词 |
|------|----------------|
| 平台用户 | `.ai/prompts/08-platform/backend/platform-user-api.md` |
| 平台角色 | `.ai/prompts/08-platform/backend/platform-role-api.md` |
| 平台权限 | `.ai/prompts/08-platform/backend/platform-permission-api.md` |
| 租户生命周期 | `.ai/prompts/08-platform/backend/tenant-lifecycle-api.md` |
| 租户信息 | `.ai/prompts/08-platform/backend/tenant-info-api.md` |
| 租户资源 | `.ai/prompts/08-platform/backend/tenant-resource-api.md` |
| 租户配置 | `.ai/prompts/08-platform/backend/tenant-config-api.md` |
| 平台运营 | `.ai/prompts/08-platform/backend/platform-operation-api.md` |

---

## 六、状态标记说明

| 标记 | 含义 |
|:----:|------|
| ⬜ | 未开始 |
| 🔄 | 进行中 |
| ✅ | 已完成且验收通过 |
| ❌ | 已完成但验收未通过 |
