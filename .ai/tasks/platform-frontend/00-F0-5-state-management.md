# 子任务 F0-5 — 状态管理

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F0-5 |
| 模块名称 | Pinia 状态管理 |
| 并行组 | — （串行，基础设施） |
| 对应提示词 | 包含在 `.ai/prompts/08-platform/frontend/0010_layout.md` 中 |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/auth-api.md` |
| 依赖任务 | F0-1 脚手架搭建（✅ 已完成） |
| 完成会话 | `session-summary-20260413-09`（基础）、`session-summary-20260413-10`（增强） |
| 状态 | ✅ 已完成 |

---

## 任务目标

创建 Pinia 状态管理 store，包括认证状态（auth store）和应用状态（app store）。auth store 负责 Token 持久化、当前用户信息、权限判断方法。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0010_layout.md` — 主布局提示词（状态管理部分）
- `.ai/prompts/08-platform/backend/auth-api.md` — 认证 API（CurrentUser 数据结构）

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/store/auth.ts` | 认证 Pinia store |
| `src/WebTenantPlatfrom/src/store/app.ts` | 应用 Pinia store |

---

## 核心功能清单

### auth store

| 功能 | 说明 |
|------|------|
| token | ref，Bearer Token 值 |
| tenantId | ref，当前租户 ID |
| user | ref，CurrentUser 接口（Id、Username、DisplayName、Email、Permissions） |
| isAuthenticated | computed，基于 token 判断 |
| permissions | computed，从 user.Permissions 获取 |
| hasPermission(code) | 单权限判断方法 |
| hasAnyPermission(...codes) | 任一权限判断方法 |
| setToken(token) | 设置 token 并持久化到 localStorage |
| clearAuth() | 清除 token、user、localStorage |

### app store

| 功能 | 说明 |
|------|------|
| loading | ref，全局 loading 状态 |

---

## 验收标准

- [x] `store/auth.ts` 包含 token、tenantId、user ref
- [x] `store/auth.ts` 包含 isAuthenticated、permissions computed
- [x] `store/auth.ts` 包含 hasPermission、hasAnyPermission 方法
- [x] token 持久化到 localStorage
- [x] `clearAuth()` 同时清除 localStorage
- [x] `store/app.ts` 包含 loading ref
- [x] `npm run build` 通过

---

## 已完成说明

本子任务分两轮完成：

### 第一轮（session-summary-20260413-09）— 基础实现

1. `store/auth.ts` — 认证状态（token、tenantId、clearAuth）
2. `store/app.ts` — 应用状态（loading）

### 第二轮（session-summary-20260413-10）— 增强

1. **store/auth.ts 增强**：
   - 新增 `CurrentUser` 接口（Id、Username、DisplayName、Email、Permissions）
   - 新增 `user` ref 存储当前用户信息
   - 新增 `isAuthenticated` computed（基于 token）
   - 新增 `permissions` computed（从 user.Permissions 获取）
   - 新增 `hasPermission(code)` 和 `hasAnyPermission(...codes)` 方法
   - token 持久化到 localStorage
   - `clearAuth()` 同时清除 localStorage
