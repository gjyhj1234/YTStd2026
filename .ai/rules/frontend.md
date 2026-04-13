# 前端开发规范

## 目标

定义前端 Vue 3 + TypeScript + DevExtreme Vue 的开发规范。

---

## 适用范围

本规范适用于以下前端项目：

| 项目 | 路径 | 状态 | HTTP 方案 | 国际化 |
|------|------|------|----------|--------|
| 新前端（活跃开发） | `src/WebTenantPlatfrom` | 活跃 | **axios** | vue-i18n（import.meta.glob） |
| 旧前端（已冻结） | `web/tenant-platform-web` | 冻结 | fetch | 自定义 t()/gt() |

> **注意**：新前端项目路径 `WebTenantPlatfrom` 为项目约定拼写，不得修正。

---

## 技术约束

### 新前端项目（`src/WebTenantPlatfrom`）

- 框架：Vue 3 + Composition API
- 语言：TypeScript 严格模式
- 构建：Vite
- UI：DevExtreme Vue（`devextreme-vue` 25.2+）
- 路由：vue-router@4
- 状态管理：Pinia
- HTTP：**axios**（禁止使用原生 fetch）
- 国际化：vue-i18n + import.meta.glob 自动加载
- Application Template：DevExtreme Vue Application Template
- UI Templates：DevExtreme Vue UI Templates

### 旧前端项目（`web/tenant-platform-web`，已冻结）

- 框架：Vue 3 + Composition API
- 语言：TypeScript 严格模式
- 构建：Vite
- UI：DevExtreme Vue（`devextreme-vue`）
- 路由：vue-router
- 状态管理：Pinia
- HTTP：原生 fetch 封装
- 国际化：自定义 t()/gt() 实现（`locales/runtime/t.ts`）
- Mock：MSW 2.x + vite-plugin-mock-dev-server

---

## 工程结构

```
web/{project}/src/
├── main.ts              # 入口
├── App.vue              # 根组件
├── api/                 # API 封装（按模块拆分）
├── assets/              # 静态资源
├── components/          # 通用组件
├── composables/         # 组合式函数
├── constants/           # 常量（权限码、错误码）
├── layouts/             # 布局组件
├── locales/             # 国际化资源
├── mocks/               # Mock 数据
│   ├── browser.ts       # MSW 浏览器配置
│   ├── handlers/        # Mock 处理器
│   └── data/            # Mock 数据
├── router/              # 路由配置
├── store/               # Pinia 状态管理
├── types/               # TypeScript 类型定义
├── utils/               # 工具函数
└── views/               # 页面视图（按模块分目录）
```

---

## 类型定义规范

### API 响应类型

必须使用 PascalCase 匹配后端：

```typescript
// src/types/base.ts
export interface ApiResult<T = any> {
  Code: number;
  Message: string;
  Data?: T;
}

export interface PagedResult<T> {
  Items: T[];
  Total: number;
  Page: number;
  PageSize: number;
}
```

### 业务类型

每个模块的类型定义位于 `src/types/{module}.ts`：

```typescript
// src/types/platformUser.ts
export interface PlatformUser {
  Id: number;
  Username: string;
  DisplayName: string;
  Status: number;
  CreatedAt: string;
}
```

---

## API 封装规范

### API 模块

每个模块的 API 封装位于 `src/api/{module}.ts`：

```typescript
// src/api/platformUser.ts
import { http } from '@/utils/http';
import type { ApiResult, PagedResult } from '@/types/base';
import type { PlatformUser } from '@/types/platformUser';

export function getPlatformUsers(params: object): Promise<ApiResult<PagedResult<PlatformUser>>> {
  return http.get('/api/platform-users', params);
}
```

### HTTP 封装

- 统一错误处理
- 统一 Token 注入
- 统一响应拦截
- `code=0` 为成功，非零自动提示错误

---

## 组件规范

### 页面组件

每个模块至少包含：

1. **ListView.vue** — 列表页（DxDataGrid + 搜索 + 操作）
2. **CreateView.vue** / **EditView.vue** — 表单页（DxForm + 验证）
3. **DetailView.vue** — 详情页/抽屉（DxPopup 或独立页面）

### 必须包含的辅助内容

每个模块必须包含：

1. **FunctionDescriptionCard** — 功能说明卡片（模块用途、关键字段、权限要求、风险提示）
2. **OperationGuideDrawer** — 操作指引抽屉（操作步骤、字段说明、常见错误处理）

### DevExtreme 组件使用

- 表格使用 `DxDataGrid` / `DxTreeList`
- 表单使用 `DxForm`
- 弹窗使用 `DxPopup`
- 按钮使用 `DxButton`
- 按照 DevExtreme 文档正确配置列、编辑、分页

### DevExtreme 组件 — 强制使用 dxdocs MCP 工具

本仓库已配置 DevExpress MCP Server（工具名：`dxdocs`），提供 `devexpress_docs_search` 和 `devexpress_docs_get_content` 两个工具。

**官方 dxdocs 工作流（必须严格遵循）：**

1. **调用 `devexpress_docs_search`** — 获取相关帮助主题列表（每个问题仅调用一次）
2. **调用 `devexpress_docs_get_content`** — 获取最相关帮助主题的全文内容
3. **反思获取到的内容** — 分析文档中的 API、属性、代码示例
4. **基于检索到的信息编码** — 使用文档中的具体控件和属性名称

**使用约束：**

- 每个问题仅调用一次 `devexpress_docs_search`，避免冗余查询
- **必须基于从 dxdocs 获取的信息编码**，禁止凭记忆或猜测
- 如果文档中有相关代码示例，**必须参考这些代码示例**
- 必须引用文档中提到的**具体 DevExtreme 控件和属性名称**
- 调用时使用 `technologies: ["Vue"]` 限定 Vue 相关文档
- 调用示例：`devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid remote paging CustomStore")`

### DevExtreme 组件使用约束

1. **DxForm label-mode**：登录页必须使用 `label-mode="static"`（避免浏览器自动填充与 floating label 重叠）
2. **DxColumn caption**：所有 DxColumn 的 caption 必须使用 `:caption="$t('...')"` 绑定形式，禁止硬编码 `caption="xxx"`
3. **DxTreeView 侧边栏**：必须通过 dxdocs 查阅选中态 CSS 定制方案，确保点击子菜单后不出现靠左对齐偏移
4. **DxPopup 内容**：使用 `<template #content>` 插槽放置内容，不使用 default slot

---

## 权限控制规范

### 权限常量

权限码集中定义在 `src/constants/permissions.ts`：

```typescript
export const Permissions = {
  PLATFORM_USER_LIST: 'platform.user.list',
  PLATFORM_USER_CREATE: 'platform.user.create',
  PLATFORM_USER_UPDATE: 'platform.user.update',
  PLATFORM_USER_DELETE: 'platform.user.delete',
  // ...
} as const;
```

### 权限控制方式

1. 路由级：路由守卫中检查菜单权限
2. 按钮级：通过 `v-if` 或自定义指令控制按钮显隐
3. 页面级：无权限时显示无权限提示页

---

## 路由规范

### 路由配置

```typescript
{
  path: '/platform-users',
  component: () => import('@/views/platform-users/ListView.vue'),
  meta: {
    title: '用户管理',
    permission: 'platform.user.list',
    requiresAuth: true,
  },
}
```

### 路由守卫

- 未登录跳转登录页
- 无权限显示 403 页面
- Token 过期自动跳转登录页

---

## 状态管理规范

- 全局状态使用 Pinia
- 认证状态：`store/auth.ts`（Token、用户信息、权限列表）
- 应用状态：`store/app.ts`（菜单折叠、语言、主题）
- 业务状态：按需创建

---

## Mock 规范

- 使用 MSW 2.x 提供开发环境 Mock
- Mock 通过 `VITE_ENABLE_MOCK=true` 环境变量控制
- Mock 处理器位于 `src/mocks/handlers/`
- Mock 数据位于 `src/mocks/data/`
- Mock 返回格式必须与后端 `ApiResult<T>` 一致

---

## 国际化规范

详见 `.ai/rules/i18n.md` 中的完整规范。

核心要求：
- 所有用户可见文本使用 `t()`，key 为中文
- 组件语言文件紧贴 .vue 文件：`{Component}.vue.zh-CN.json`
- 资源文件位于 `src/locales/`
  - `generated/` — 由 YTStdI18n.Generator 自动生成（允许手动编辑翻译，Generator 不覆盖已有 key）
  - `common/` — 全局复用资源
  - `runtime/` — t()/gt()/loader 实现
- 组件 JSON 中复用文本使用 `null`（由 t() 自动从 common 获取）
- 支持 zh-CN（基准）、en-US、ms-MY、zh-TW、ja-JP
- DevExtreme 组件本地化需单独配置
- 新增组件时必须同步创建对应的语言文件
