# {业务名称} — {模块名} 前端页面

## 目标

实现 {模块名} 的前端管理页面，包含列表、创建、编辑、详情功能。

---

## 前置阅读

- `.ai/rules/frontend.md` — 前端开发规范
- `.ai/prompts/03-frontend/page-module.md` — 页面模块通用规范
- `backend/{module}-api.md` — 对应的后端 API 提示词
- `docs/{Business}/API.md` — API 文档

---

## 输入

- 后端 API 端点清单
- 后端 DTO 定义

---

## 输出

- `src/api/{module}.ts` — API 封装
- `src/types/{module}.ts` — TypeScript 类型
- `src/views/{Module}/ListView.vue` — 列表页
- `src/views/{Module}/CreateView.vue` — 创建页（如适用）
- `src/views/{Module}/EditView.vue` — 编辑页（如适用）
- `src/views/{Module}/DetailView.vue` — 详情页（如适用）
- `src/router/index.ts` — 更新路由

---

## 列表页功能

- 分页查询
- 搜索与筛选
- 排序
- 批量操作
- 权限控制按钮显隐
- 功能说明卡片（FunctionDescriptionCard）
- 操作指引抽屉（OperationGuideDrawer）

---

## 约束

- 使用 DevExtreme 组件
- 使用 `t()` 国际化
- API 使用 HTTP 封装
- 类型使用 PascalCase（匹配后端）

---

## 验收标准

- [ ] 列表页正常渲染
- [ ] CRUD 操作正确
- [ ] 权限控制正确
- [ ] `npm run build` 通过
