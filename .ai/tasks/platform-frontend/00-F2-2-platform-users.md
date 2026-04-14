# 子任务 F2-2 — 平台用户管理

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F2-2 |
| 模块名称 | 平台用户管理 |
| 并行组 | A |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0021_platform-user-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/platform-user-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成） |
| 完成会话 | `session-summary-20260413-13` |
| 状态 | ✅ 已完成 |

---

## 任务目标

实现平台用户管理页面（F2-2），包含 DxDataGrid + CustomStore 远程分页、搜索/高级查询、用户 CRUD（新增/编辑/详情/删除）、状态操作（启用/禁用/重置密码）、批量操作、权限码控制。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0021_platform-user-page.md` — 页面功能详细定义
- `.ai/prompts/08-platform/backend/platform-user-api.md` — 后端 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue` | 平台用户管理页面组件 |
| `src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/api/platform-users.ts` | 平台用户 API 封装（11 个函数） |
| `src/WebTenantPlatfrom/src/types/platform-users.ts` | 平台用户类型定义（4 个类型） |
| `src/WebTenantPlatfrom/src/constants/permissions.ts`（修改） | 平台用户权限码常量 |
| `src/WebTenantPlatfrom/src/router.ts`（修改） | 路由指向实际组件 |
| `src/WebTenantPlatfrom/src/locales/common/{5 locales}.json`（修改） | 新增 18 个公共 key |

---

## 核心功能清单

| 功能 | 说明 |
|------|------|
| DxDataGrid + CustomStore | 远程分页，避免前端一次加载全部数据 |
| 搜索区 | 关键词 DxTextBox + 状态 DxSelectBox |
| 高级查询 | 角色 DxSelectBox + 创建时间 DxDateRangeBox（可展开/收起） |
| 工具栏 | 新增、批量启用、批量禁用 |
| 行操作 | 查看、编辑、启用、禁用、重置密码、删除（6 个，权限码控制） |
| 新增弹窗 | DxPopup + DxForm，用户名 async 唯一性校验，密码、显示名、邮箱、手机、角色 DxTagBox |
| 编辑弹窗 | 用户名只读，无密码字段 |
| 详情弹窗 | 9 个展示字段 |
| 状态标签 | 1=已启用绿色，2=已禁用红色 |

---

## 验收标准

- [x] 页面按 `0021_platform-user-page.md` 实现所有功能点
- [x] DxDataGrid + CustomStore 远程分页正常
- [x] 搜索和高级查询功能正常
- [x] 用户 CRUD 功能正常
- [x] 状态操作（启用/禁用/重置密码）有确认弹窗
- [x] 批量操作功能正常
- [x] 行操作按权限码控制
- [x] 新增弹窗用户名 async 唯一性校验
- [x] DxColumn caption 使用 `:caption="$t()"`
- [x] 5 个语言文件 key 完全一致（40+ key）
- [x] `npm run build` 通过
- [x] self-review F1-F7 全部通过

---

## 已完成说明

本子任务已在 `session-summary-20260413-13` 中完成（与 F2-3、F2-4 一起实现）。主要产出：

1. **PlatformUsersView.vue** — 完整平台用户管理页面
   - DxDataGrid + CustomStore 远程分页
   - 搜索区（关键词 + 状态） + 高级查询（角色 + 日期范围）
   - 工具栏（新增 + 批量启用/禁用）
   - 行操作 6 个（权限码控制）
   - 新增/编辑/详情弹窗
2. **api/platform-users.ts** — 11 个 API 函数
3. **types/platform-users.ts** — 4 个类型定义
4. **5 个语言文件** — 40+ key
5. **constants/permissions.ts** — 平台用户权限码
6. **utils/notify.ts** — notifySuccess、notifyError、confirmAction、confirmDelete
7. **common 语言文件更新** — 新增 18 个公共 key
