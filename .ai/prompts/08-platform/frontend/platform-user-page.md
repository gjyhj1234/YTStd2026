# 租户平台 — 平台用户管理页面

## 目标

重构平台用户管理前端页面。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/rules/i18n.md`
- `.ai/prompts/03-frontend/page-module.md`
- `.ai/prompts/08-platform/frontend/refactoring-master.md`
- `backend/platform-user-api.md` — 后端 API
- `.github/copilot-instructions.md`

---

## DevExpress 文档查阅（强制前置步骤）

使用 dxdocs MCP 工具查阅：

```
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid CustomStore remote paging load function")
devexpress_docs_search(technologies: ["Vue"], question: "DxForm async validation validationCallback")
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid selection mode multiple showCheckBoxesMode batch operations")
```

---

## 页面列表

| 页面 | 路径 | 说明 |
|------|------|------|
| 用户列表 | `views/PlatformUser/ListView.vue` | 用户列表（DataGrid） |
| 用户创建 | 弹窗/抽屉 | 创建用户表单 |
| 用户编辑 | 弹窗/抽屉 | 编辑用户表单 |
| 用户详情 | 弹窗/抽屉 | 用户详情展示 |

---

## 列表功能

- 搜索：用户名、显示名、状态
- 排序：创建时间
- 分页
- 操作：编辑、启用/禁用、重置密码、删除
- 批量操作：批量启用、批量禁用
- 权限控制按钮显隐

---

## 表单字段

### 创建

| 字段 | 类型 | 必填 | 说明 |
|------|------|:----:|------|
| 用户名 | 文本 | ✅ | 唯一，实时检查 |
| 密码 | 密码 | ✅ | |
| 显示名 | 文本 | ✅ | |
| 邮箱 | 文本 | | |
| 手机 | 文本 | | |
| 角色 | 多选 | | 角色下拉框 |

### 编辑

除用户名和密码外同上。

---

## API 对应

| 操作 | API |
|------|-----|
| 列表 | GET `/api/platform-users` |
| 详情 | GET `/api/platform-users/{id}` |
| 创建 | POST `/api/platform-users` |
| 更新 | PUT `/api/platform-users/{id}` |
| 删除 | DELETE `/api/platform-users/{id}` |
| 启用 | PUT `/api/platform-users/{id}/enable` |
| 禁用 | PUT `/api/platform-users/{id}/disable` |
| 重置密码 | PUT `/api/platform-users/{id}/reset-password` |
| 检查用户名 | GET `/api/platform-users/check-username-exists` |

---

## 验收标准（可执行检查）

- [ ] 列表页使用 CustomStore 远程分页
- [ ] `grep -rn 'caption="' PlatformUsersView.vue | grep -v ':caption'` 结果为 0
- [ ] `grep -rn "notifySuccess(t(" PlatformUsersView.vue` 结果为 0
- [ ] CRUD 操作正确（创建/编辑/删除/启用/禁用/重置密码）
- [ ] 权限按钮显隐正确（v-if="perm.has()"）
- [ ] 用户名唯一性异步验证
- [ ] 所有操作有反馈（notifySuccess + confirmDelete/confirmAction）
- [ ] 5 个语言文件已创建且 key 一致
- [ ] 功能说明卡片和操作指引完整
- [ ] `npm run build` 通过
