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
9. `src/locales/zh-CN.json` 更新 — 中文资源
10. `src/locales/en-US.json` 更新 — 英文资源

---

## 执行步骤

1. 创建类型定义（匹配后端 DTO）
2. 创建 API 封装（匹配后端端点）
3. 实现列表页（DxDataGrid + 搜索 + 操作按钮）
4. 实现创建/编辑表单（DxForm + 验证）
5. 实现详情页/抽屉（DxPopup 或独立页）
6. 添加功能说明卡片内容
7. 添加操作指引内容
8. 注册路由并配置权限守卫
9. 更新权限码常量
10. 更新国际化资源（zh-CN、en-US）
11. 执行 `npm run build` 验证

---

## 列表页要求

- 使用 DxDataGrid
- 支持分页（远程分页）
- 支持关键词搜索
- 支持状态筛选
- 操作列包含：查看、编辑、启用/禁用、删除（按权限显隐）
- 包含 FunctionDescriptionCard
- 包含 OperationGuideDrawer 入口

## 表单要求

- 使用 DxForm
- 必填字段有验证
- 唯一字段有实时检查（调用 check-exists API）
- 提交后显示结果反馈

## 详情页要求

- 展示所有关键字段
- 展示状态信息
- 展示审计信息（创建时间、更新时间）
- 支持从列表页跳转

---

## 约束

- 类型使用 PascalCase
- 权限控制到按钮级
- 所有用户可见文本使用 `$t()`

---

## 禁止事项

- 禁止在组件中硬编码中文文本（必须使用 i18n）
- 禁止省略权限检查
- 禁止省略功能说明和操作指引

---

## 验收标准

- [ ] 列表页正常渲染
- [ ] 创建/编辑表单有验证
- [ ] 详情页展示完整信息
- [ ] 权限控制正确
- [ ] 功能说明和操作指引完整
- [ ] `npm run build` 通过
