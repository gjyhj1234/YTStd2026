# 子任务 F2-3 — 平台角色管理

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F2-3 |
| 模块名称 | 平台角色管理 |
| 并行组 | A |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0022_platform-role-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/platform-role-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成） |
| 完成会话 | `session-summary-20260413-13`（初始完成）, `session-summary-20260414-02`（E2E 修复） |
| 状态 | ✅ 已完成（含 E2E 修复） |

---

## 任务目标

实现平台角色管理页面（F2-3），包含 DxDataGrid + CustomStore 远程分页、搜索筛选、角色 CRUD、权限绑定弹窗（DxTreeList 多选）、成员绑定弹窗（DxDataGrid 多选）。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0022_platform-role-page.md` — 页面功能详细定义
- `.ai/prompts/08-platform/backend/platform-role-api.md` — 后端 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue` | 平台角色管理页面组件 |
| `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/api/platform-roles.ts` | 平台角色 API 封装（12 个函数） |
| `src/WebTenantPlatfrom/src/types/platform-roles.ts` | 平台角色类型定义（5 个类型） |
| `src/WebTenantPlatfrom/src/constants/permissions.ts`（修改） | 平台角色权限码常量 |
| `src/WebTenantPlatfrom/src/router.ts`（修改） | 路由指向实际组件 |

---

## 核心功能清单

| 功能 | 说明 |
|------|------|
| DxDataGrid + CustomStore | 远程分页 |
| 搜索区 | 关键词 DxTextBox + 状态 DxSelectBox |
| 行操作 | 查看、编辑、启用、禁用、分配权限、分配成员、删除（7 个） |
| 新增/编辑弹窗 | 角色编码 async 唯一性校验 |
| 权限绑定弹窗 | DxTreeList + recursive 多选，预加载已绑定权限 |
| 成员绑定弹窗 | DxDataGrid 多选 |
| 详情弹窗 | 6 个展示字段 |

---

## 验收标准

- [x] 页面按 `0022_platform-role-page.md` 实现所有功能点
- [x] DxDataGrid + CustomStore 远程分页正常
- [x] 搜索和筛选功能正常
- [x] 角色 CRUD 功能正常
- [x] 角色编码 async 唯一性校验
- [x] 权限绑定弹窗（DxTreeList recursive 多选）正常
- [x] 成员绑定弹窗（DxDataGrid 多选）正常
- [x] DxColumn caption 使用 `:caption="$t()"`
- [x] 5 个语言文件 key 完全一致（35+ key）
- [x] `npm run build` 通过
- [x] self-review F1-F7 全部通过

---

## 已完成说明

本子任务已在 `session-summary-20260413-13` 中完成（与 F2-2、F2-4 一起实现）。主要产出：

1. **PlatformRolesView.vue** — 完整平台角色管理页面
   - DxDataGrid + CustomStore 远程分页
   - 搜索区（关键词 + 状态）
   - 行操作 7 个
   - 新增/编辑弹窗（角色编码唯一性校验）
   - 权限绑定弹窗（DxTreeList recursive 多选，预加载已绑定权限）
   - 成员绑定弹窗（DxDataGrid 多选）
   - 详情弹窗
2. **api/platform-roles.ts** — 12 个 API 函数
3. **types/platform-roles.ts** — 5 个类型定义
4. **5 个语言文件** — 35+ key
5. **constants/permissions.ts** — 平台角色权限码
