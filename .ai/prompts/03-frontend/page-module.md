# 前端页面模块提示词

## 目标

为指定业务模块实现完整的前端页面，包含列表、表单、详情、权限控制和辅助内容。

---

## 适用范围

实现单个业务模块的前端页面时使用。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/rules/naming.md`
- `.ai/rules/i18n.md`
- `.github/copilot-instructions.md` — 关键编码约束（第 7-12 条为前端约束）

---

## DevExpress 文档查阅（强制前置步骤）

在开始编码前，必须通过 `dxdocs` MCP 工具查阅以下组件的官方文档：

```
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid CustomStore remote operations load function")
devexpress_docs_search(technologies: ["Vue"], question: "DxForm validation rules required stringLength async")
devexpress_docs_search(technologies: ["Vue"], question: "DxPopup content template slot")
```

---

## 输入

- 模块名称
- 后端 API 文档
- 后端 DTO 字段说明
- 权限码列表

---

## 输出

每个模块产出以下文件：

1. `src/api/{module}.ts` — API 封装
2. `src/types/{module}.ts` — 类型定义
3. `src/views/{module}/ListView.vue` — 列表页
4. `src/views/{module}/CreateView.vue` — 创建页
5. `src/views/{module}/EditView.vue` — 编辑页（可与创建页合并）
6. `src/views/{module}/DetailView.vue` — 详情页/抽屉
7. `src/router/index.ts` 更新 — 路由注册
8. `src/constants/permissions.ts` 更新 — 权限码
9. 组件级语言文件 — 5 个语言文件（zh-CN、en-US、ja-JP、ms-MY、zh-TW）

---

## 执行步骤

1. **查阅 DevExtreme 文档**（使用 dxdocs MCP 工具查阅将要使用的组件）
2. 创建类型定义（匹配后端 DTO）
3. 创建 API 封装（匹配后端端点）
4. 实现列表页（DxDataGrid + CustomStore 远程分页 + 搜索 + 操作按钮）
5. 实现创建/编辑表单（DxForm + 验证规则，含 required、stringLength、async 唯一性检查）
6. 实现详情页/抽屉（DxPopup，使用 `<template #content>` 插槽）
7. 添加功能说明卡片内容
8. 添加操作指引内容
9. 注册路由并配置权限守卫
10. 更新权限码常量
11. 创建 5 个组件级语言文件（zh-CN、en-US、ja-JP、ms-MY、zh-TW）
12. 执行 `npm run build` 验证
13. **执行前端自检协议**（self-review-protocol.md 中的 F1-F5 审查项）

---

## 列表页要求

- 使用 DxDataGrid
- 使用 CustomStore 实现远程分页
- 所有 DxColumn 的 caption 必须使用 `:caption="$t('...')"` 绑定（禁止硬编码 `caption="xxx"`）
- 支持关键词搜索
- 支持状态筛选
- 操作列包含：查看、编辑、启用/禁用、删除（按权限显隐）
- 包含 FunctionDescriptionCard
- 包含 OperationGuideDrawer 入口

## 表单要求

- 使用 DxForm
- 必填字段有验证（type: 'required'）
- 长度限制验证（type: 'stringLength'）
- 唯一字段有异步检查（type: 'async', validationCallback 调用 check-exists API）
- 提交前调用 `formInstance.validate()`，验证不通过禁止提交
- 提交后使用 `notifySuccess('操作成功')` 反馈（仅传 i18n key，不包裹 t()）

## 详情页要求

- 展示所有关键字段
- 展示状态信息
- 展示审计信息（创建时间、更新时间）
- 支持从列表页跳转

## 操作反馈要求

```typescript
// ✅ 正确调用方式 — 仅传 i18n key
notifySuccess('创建成功')
notifySuccess('更新成功')
notifySuccess('删除成功')
const confirmed = await confirmDelete(item.Name)
const confirmed = await confirmAction('确认禁用此项')

// ❌ 错误 — 禁止双重 t()
notifySuccess(t('创建成功'))
confirmAction(t('确认禁用'))
```

---

## 约束

- 类型使用 PascalCase
- 权限控制到按钮级
- 所有用户可见文本使用 `t()`/`$t()`，key 为中文
- DxColumn caption 必须使用绑定形式 `:caption="$t()"`
- notifySuccess/confirmAction 仅传 i18n key，不双重 t()

---

## 禁止事项

- 禁止在组件中硬编码中文文本（必须使用 `t()`）
- 禁止省略权限检查
- 禁止省略功能说明和操作指引
- 禁止硬编码 DxColumn caption（必须 `:caption="$t()"` 绑定）
- 禁止在 notifySuccess/confirmAction 参数中使用 t() 包裹
- 禁止不查阅 dxdocs 文档就猜测 DevExtreme API

---

## 验收标准

- [ ] 列表页正常渲染，使用 CustomStore 远程分页
- [ ] 创建/编辑表单有完整验证（required + stringLength + async 唯一性）
- [ ] 详情页展示完整信息
- [ ] 权限控制正确（v-if="perm.has()"）
- [ ] 功能说明和操作指引完整
- [ ] 5 个语言文件已创建且 key 一致
- [ ] `grep -rn 'caption="' {Component}.vue | grep -v ':caption'` 结果为 0
- [ ] `grep -rn "notifySuccess(t(" {Component}.vue` 结果为 0
- [ ] `npm run build` 通过
