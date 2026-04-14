# 子任务 F1-1 — 主布局

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F1-1 |
| 模块名称 | 主布局（基于 DevExtreme Vue Application Template） |
| 并行组 | — （串行，布局层） |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0010_layout.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/auth-api.md` |
| 依赖任务 | F0-1 ~ F0-3（✅ 已完成） |
| 完成会话 | `session-summary-20260413-10` |
| 状态 | ✅ 已完成 |

---

## 任务目标

基于 DevExtreme Vue Application Template 的 `side-nav-outer-toolbar` 布局实现完整的主布局层，包括：导航菜单（国际化 + 权限过滤）、顶栏（语言切换 + 用户下拉）、侧边栏（DxDrawer + DxTreeView 反偏移 CSS）、认证模块对接后端 API。同时完成 F0-4（路由守卫）、F0-5（状态管理增强）、F0-6（通用组件）。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0010_layout.md` — 主布局提示词（含 F0-4/F0-5/F0-6 部分）
- `.ai/prompts/03-frontend/04-devextreme-templates.md` — DevExtreme 模板规范
- `.ai/prompts/08-platform/backend/auth-api.md` — 认证 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/app-navigation.ts`（修改） | 业务菜单结构 + i18n + 权限过滤 |
| `src/WebTenantPlatfrom/src/auth.ts`（修改） | axios 对接后端 /auth/login 和 /auth/me |
| `src/WebTenantPlatfrom/src/router.ts`（修改） | 14+ 业务路由 + beforeEach 权限守卫 |
| `src/WebTenantPlatfrom/src/app-info.ts`（修改） | title 改为"租户管理平台" |
| `src/WebTenantPlatfrom/src/store/auth.ts`（修改） | CurrentUser、权限方法、localStorage 持久化 |
| `src/WebTenantPlatfrom/src/api/http.ts`（修改） | 启用 Bearer Token 注入 |
| `src/WebTenantPlatfrom/src/locales/index.ts`（修改） | 支持从 localStorage 恢复语言偏好 |
| `src/WebTenantPlatfrom/src/components/header-toolbar.vue`（修改） | 语言切换 + 用户下拉 |
| `src/WebTenantPlatfrom/src/components/side-nav-menu.vue`（修改） | i18n + 权限过滤 + 反偏移 CSS |
| `src/WebTenantPlatfrom/src/layouts/side-nav-outer-toolbar.vue`（修改） | Composition API + i18n 标题 |
| `src/WebTenantPlatfrom/src/components/header-toolbar.vue.{5 locales}.json` | 顶栏语言文件 × 5 |
| `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue` + 5 语言文件 | 功能说明卡片组件 |
| `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue` + 5 语言文件 | 操作指南抽屉组件 |
| `src/WebTenantPlatfrom/src/utils/notify.ts` | 通知工具函数 |
| `src/WebTenantPlatfrom/src/styles/sidebar.css` | 侧边栏反偏移 CSS |
| `src/WebTenantPlatfrom/src/locales/{5 locales}.json`（修改） | 新增布局 key |

---

## 核心功能清单

| 功能 | 说明 |
|------|------|
| app-navigation.ts | `getNavigationItems(t)` 接收 i18n t 函数，`filterNavigationByPermission()` 按权限码过滤，16 个叶子菜单项 |
| auth.ts | `logIn()` 使用 httpPost `/auth/login`，`getUser()` 使用 httpGet `/auth/me`，`loggedIn()` 基于 localStorage token |
| header-toolbar.vue | script setup、标题 `$t('租户管理平台')`、DxSelectBox 语言切换（5 语言）、DxDropDownButton 用户下拉（修改密码 + 退出登录） |
| side-nav-menu.vue | script setup、computed 响应式菜单、DxTreeView `selectByClick=true` + `focusStateEnabled=false`、反偏移 CSS |
| router.ts | 14+ 业务路由（动态导入）、默认路由 `/dashboard`、`beforeEach` 守卫 |
| store/auth.ts | CurrentUser 接口、hasPermission(code)、hasAnyPermission(...codes)、token 持久化 |

---

## 验收标准

- [x] side-nav-outer-toolbar.vue 作为默认布局
- [x] 顶栏包含：菜单按钮、Logo/标题、语言切换、用户下拉
- [x] 侧边栏使用 DxDrawer + DxTreeView，菜单与后端模块一致（16 叶子）
- [x] 菜单按权限过滤（filterNavigationByPermission）
- [x] 语言切换后菜单文本实时更新
- [x] 退出登录清除 token 并跳转登录页
- [x] DxTreeView focusStateEnabled=false + 反偏移 CSS
- [x] FunctionDescriptionCard + OperationGuideDrawer 通用组件存在
- [x] notify.ts 仅接收 i18n key
- [x] 所有组件语言文件 5 个一组，key 完全一致
- [x] `npm run build` 通过

---

## 已完成说明

本子任务已在 `session-summary-20260413-10` 中完成。同时包含 F0-4（路由守卫）、F0-5（状态管理增强）、F0-6（通用组件）的实现。主要产出：

1. **app-navigation.ts** — 完整业务菜单结构 + i18n + 权限过滤
2. **auth.ts** — axios 对接后端 /auth/login 和 /auth/me
3. **header-toolbar.vue** — script setup + 语言切换 DxSelectBox + 用户下拉 DxDropDownButton
4. **side-nav-menu.vue** — script setup + computed 响应式 + DxTreeView 反偏移
5. **side-nav-outer-toolbar.vue** — Composition API + i18n 标题
6. **router.ts** — 14+ 业务路由 + beforeEach 权限守卫
7. **store/auth.ts** — CurrentUser、hasPermission、hasAnyPermission、localStorage 持久化
8. **FunctionDescriptionCard.vue** + **OperationGuideDrawer.vue** — 通用组件 + 各 5 语言文件
9. **utils/notify.ts** — notifySuccess、notifyError、confirmAction、confirmDelete
10. **styles/sidebar.css** — 侧边栏反偏移 CSS
