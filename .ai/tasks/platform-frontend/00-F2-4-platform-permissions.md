# 子任务 F2-4 — 平台权限管理

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F2-4 |
| 模块名称 | 平台权限管理 |
| 并行组 | A |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0023_platform-permission-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/platform-permission-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成） |
| 完成会话 | `session-summary-20260413-13` |
| 状态 | ✅ 已完成 |

---

## 任务目标

实现平台权限管理页面（F2-4），使用 DxTreeList 树形展示权限列表，包含关键词搜索（含祖先节点保留）、权限类型颜色标签、HTTP 方法颜色标签。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0023_platform-permission-page.md` — 页面功能详细定义
- `.ai/prompts/08-platform/backend/platform-permission-api.md` — 后端 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue` | 平台权限管理页面组件 |
| `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/api/platform-permissions.ts` | 平台权限 API 封装（3 个函数） |
| `src/WebTenantPlatfrom/src/types/platform-permissions.ts` | 平台权限类型定义（2 个类型） |
| `src/WebTenantPlatfrom/src/router.ts`（修改） | 路由指向实际组件 |

---

## 核心功能清单

| 功能 | 说明 |
|------|------|
| DxTreeList 树形展示 | key-expr="Id"、parent-id-expr="ParentId"、auto-expand-all |
| 关键词搜索 | 含祖先节点保留（匹配子节点时保留其所有祖先） |
| 权限类型标签 | 菜单/API/操作/数据，4 种颜色区分 |
| HTTP 方法标签 | GET（绿）/POST（蓝）/PUT（橙）/DELETE（红） |
| flattenTree 工具 | 递归扁平化后端嵌套数据供 DxTreeList 使用 |

---

## 验收标准

- [x] 页面按 `0023_platform-permission-page.md` 实现所有功能点
- [x] DxTreeList 树形展示正常（auto-expand-all）
- [x] 关键词搜索含祖先节点保留
- [x] 权限类型颜色标签正确
- [x] HTTP 方法颜色标签正确
- [x] DxColumn caption 使用 `:caption="$t()"`
- [x] 5 个语言文件 key 完全一致（15+ key）
- [x] `npm run build` 通过
- [x] self-review F1-F7 全部通过

---

## 已完成说明

本子任务已在 `session-summary-20260413-13` 中完成（与 F2-2、F2-3 一起实现）。主要产出：

1. **PlatformPermissionsView.vue** — 完整平台权限管理页面
   - DxTreeList 树形展示（key-expr="Id"、parent-id-expr="ParentId"、auto-expand-all）
   - 关键词搜索（含祖先节点保留）
   - 权限类型颜色标签（菜单/API/操作/数据）
   - HTTP 方法颜色标签（GET/POST/PUT/DELETE）
2. **api/platform-permissions.ts** — 3 个 API 函数
3. **types/platform-permissions.ts** — 2 个类型定义
4. **5 个语言文件** — 15+ key

### 技术决策

| 决策 | 理由 |
|------|------|
| flattenTree 递归工具 | DxTreeList 需要扁平数据（key-expr + parent-id-expr），后端返回嵌套数据 |
| 祖先节点保留搜索 | 搜索匹配子节点时，保留其所有祖先节点以维持树形结构完整性 |
