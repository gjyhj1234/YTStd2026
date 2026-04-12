# 租户平台 — 平台角色管理页面

## 目标

重构平台角色管理前端页面，实现完整 CRUD、权限绑定、成员绑定功能。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/rules/i18n.md`
- `.ai/prompts/03-frontend/page-module.md`
- `.ai/prompts/08-platform/frontend/refactoring-master.md`（第八节质量检查 + 第九节代码模板）
- `.ai/prompts/08-platform/backend/platform-role-api.md`
- `.github/copilot-instructions.md`

---

## DevExpress 文档查阅（强制前置步骤）

使用 dxdocs MCP 工具查阅：

```
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid CustomStore remote paging load function skip take")
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeList selection mode multiple recursive selectedRowKeys")
devexpress_docs_search(technologies: ["Vue"], question: "DxForm async validation validationCallback")
devexpress_docs_search(technologies: ["Vue"], question: "DxPopup content template slot visible hiding event")
```

---

## API 端点（精确匹配）

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 角色列表 | GET | /api/platform-roles?Page=1&PageSize=20&Keyword=&Status= | - | ApiResult<PagedResult<PlatformRoleRepDTO>> |
| 创建角色 | POST | /api/platform-roles | { Code, Name, Description } | ApiResult<long> |
| 编辑角色 | PUT | /api/platform-roles/{id} | { Name, Description } | ApiResult |
| 删除角色 | DELETE | /api/platform-roles/{id} | - | ApiResult |
| 启用 | PUT | /api/platform-roles/{id}/enable | - | ApiResult |
| 禁用 | PUT | /api/platform-roles/{id}/disable | - | ApiResult |
| 检查编码 | GET | /api/platform-roles/check-code-exists?code=xxx | - | ApiResult<bool> |
| 获取角色权限 | GET | /api/platform-roles/{id}/permissions | - | ApiResult<number[]> |
| 绑定权限 | POST | /api/platform-roles/{id}/permissions | { PermissionIds: number[] } | ApiResult |
| 绑定成员 | POST | /api/platform-roles/{id}/members | { UserIds: number[] } | ApiResult |

---

## 必须产出的文件

1. `src/views/platform-roles/PlatformRolesView.vue`
2. `src/views/platform-roles/PlatformRolesView.vue.zh-CN.json`
3. `src/views/platform-roles/PlatformRolesView.vue.en-US.json`
4. `src/views/platform-roles/PlatformRolesView.vue.ja-JP.json`
5. `src/views/platform-roles/PlatformRolesView.vue.ms-MY.json`
6. `src/views/platform-roles/PlatformRolesView.vue.zh-TW.json`

---

## 页面功能

### 1. 角色列表（DxDataGrid）

- 使用 CustomStore 实现远程分页
- **所有 DxColumn 的 caption 必须使用 `:caption="$t('...')"` 绑定**

```vue
<!-- ✅ 正确 -->
<DxColumn data-field="Id" :caption="$t('ID')" :width="80" :allow-sorting="false" />
<DxColumn data-field="Code" :caption="$t('角色编码')" />
<DxColumn data-field="Name" :caption="$t('角色名称')" />
<DxColumn data-field="Description" :caption="$t('描述')" />
<DxColumn data-field="Status" :caption="$t('状态')" cell-template="statusCell" :width="100" />
<DxColumn data-field="CreatedAt" :caption="$t('创建时间')" cell-template="dateCell" />
<DxColumn :caption="$t('操作')" cell-template="actionCell" :width="380" :allow-sorting="false" />

<!-- ❌ 错误 — 禁止硬编码 caption -->
<DxColumn data-field="Id" caption="ID" />
```

### 2. 创建角色弹窗（DxPopup + DxForm）

- 表单字段：Code（required + stringLength + async 唯一性检查）、Name（required + stringLength）、Description（optional）
- DxPopup 使用 `<template #content>` 插槽

### 3. 编辑角色弹窗（DxPopup + DxForm）

- 表单字段：Name（required + stringLength）、Description（optional）
- Code 不可编辑（创建后不可修改）

### 4. 权限绑定弹窗（DxPopup + DxTreeList）

- 使用 DxTreeList 展示权限树
- DxSelection mode="multiple" recursive=true
- 预加载当前已绑定权限（selectedRowKeys）
- 保存时调用 bindRolePermissions API

### 5. 成员绑定弹窗（DxPopup + DxDataGrid）

- 展示所有用户列表
- 多选绑定

### 6. 操作按钮（按权限显隐）

```typescript
// 每个操作按钮必须有权限控制
v-if="perm.has(PLATFORM_ROLE_CREATE)"
v-if="perm.has(PLATFORM_ROLE_UPDATE)"
v-if="perm.has(PLATFORM_ROLE_DELETE)"
v-if="perm.has(PLATFORM_ROLE_ENABLE)"
v-if="perm.has(PLATFORM_ROLE_DISABLE)"
v-if="perm.has(PLATFORM_ROLE_ASSIGN_PERMISSION)"
v-if="perm.has(PLATFORM_ROLE_ASSIGN_MEMBER)"
```

---

## 操作反馈规范

```typescript
// ✅ 正确 — 仅传 i18n key
notifySuccess('创建成功')
notifySuccess('更新成功')
notifySuccess('删除成功')
notifySuccess('启用成功')
notifySuccess('禁用成功')
notifySuccess('保存成功')
const confirmed = await confirmDelete(role.Name)
const confirmed = await confirmAction('确认启用此角色')
const confirmed = await confirmAction('确认禁用此角色')

// ❌ 错误 — 禁止双重 t()
notifySuccess(t('创建成功'))
```

---

## 验收标准（可执行检查）

- [ ] 角色列表使用 CustomStore 远程分页
- [ ] `grep -rn 'caption="' PlatformRolesView.vue | grep -v ':caption'` 结果为 0
- [ ] `grep -rn "notifySuccess(t(" PlatformRolesView.vue` 结果为 0
- [ ] 创建弹窗有表单验证（Code: required + stringLength + async 唯一性）
- [ ] 编辑弹窗有表单验证（Name: required）
- [ ] 权限绑定弹窗预加载当前已绑定权限
- [ ] 成员绑定弹窗正确展示用户列表
- [ ] 所有操作有成功 Toast（notifySuccess）
- [ ] 危险操作有确认弹窗（confirmDelete / confirmAction）
- [ ] 权限控制到按钮级（v-if="perm.has()"）
- [ ] 5 个语言文件已创建且 key 一致
- [ ] 功能说明卡片和操作指引完整
- [ ] `npm run build` 通过
