# 子任务 F0-4 — 路由与权限守卫

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F0-4 |
| 模块名称 | 路由与权限守卫 |
| 并行组 | — （串行，基础设施） |
| 对应提示词 | 包含在 `.ai/prompts/08-platform/frontend/0010_layout.md` 中 |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/auth-api.md` |
| 依赖任务 | F0-1 脚手架搭建（✅ 已完成）、F0-2 axios 封装（✅ 已完成） |
| 完成会话 | `session-summary-20260413-10` |
| 状态 | ✅ 已完成 |

---

## 任务目标

改造 Application Template 的 router.ts，删除模板 demo 路由，新增 14+ 业务路由（动态导入），配置 `beforeEach` 导航守卫（认证检测），在 http.ts 请求拦截器中启用 Bearer Token 注入。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0010_layout.md` — 主布局提示词（路由部分）
- `.ai/prompts/08-platform/backend/auth-api.md` — 认证 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/router.ts`（修改） | 业务路由 + 导航守卫 |
| `src/WebTenantPlatfrom/src/api/http.ts`（修改） | 启用 Bearer Token 注入 |

---

## 核心功能清单

| 功能 | 说明 |
|------|------|
| 业务路由 | 14+ 个路由定义，均使用 `loadView()` 动态导入 |
| 默认路由 | 从 `/home` 改为 `/dashboard` |
| 导航守卫 | `beforeEach` 检测 `auth.loggedIn()`，未认证跳转登录页 |
| Token 注入 | 请求拦截器从 localStorage 读取 token，注入 `Authorization: Bearer {token}` |
| Hash 模式 | 使用 `createWebHashHistory()` |

---

## 验收标准

- [x] 模板 demo 路由（home/profile/tasks）已删除
- [x] 新增 14+ 业务路由，均使用动态导入
- [x] 默认路由为 `/dashboard`
- [x] `beforeEach` 导航守卫集成 `auth.loggedIn()` 检测
- [x] http.ts 请求拦截器已启用 Bearer Token 注入
- [x] `npm run build` 通过

---

## 已完成说明

本子任务已在 `session-summary-20260413-10` 中完成（与 F1-1 主布局一起实现）。主要产出：

1. **router.ts 全面改造**：
   - 删除模板 demo 路由（home/profile/tasks），新增 14+ 业务路由
   - 所有路由均使用 `loadView()` 动态导入（暂指向 home-page 占位）
   - 默认路由从 `/home` 改为 `/dashboard`
   - `beforeEach` 守卫集成 auth.loggedIn() 检测
2. **http.ts Token 注入**：
   - 请求拦截器中启用 Bearer Token 注入（从 localStorage 读取）
   - 替代原有的占位注释
