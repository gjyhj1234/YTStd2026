# 租户平台 — 租户管理页面

## 目标

重构租户管理前端页面。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/prompts/03-frontend/page-module.md`
- `backend/tenant-lifecycle-api.md`
- `.github/copilot-instructions.md` — 关键编码约束（第 7-12 条为前端约束）

---

## DevExpress 文档查阅（强制前置步骤）

使用 DevExtreme 组件时，必须按照官方 dxdocs 工作流查阅文档（详见 `refactoring-master.md` 第零节）：

1. **调用 `devexpress_docs_search`**（每个问题仅调用一次，使用 `technologies: ["Vue"]`）
2. **调用 `devexpress_docs_get_content`** 获取最相关帮助主题的全文
3. **反思内容**，提取可用的 API、属性、代码示例
4. **基于检索到的信息编码**，引用具体控件和属性名称

---

## 页面功能

- 租户列表（DataGrid）
  - 搜索：租户名、编码、状态
  - 状态标签颜色区分
- 创建租户表单
- 租户详情（含生命周期事件时间线）
- 状态操作按钮（初始化/暂停/恢复/终止/转正）
  - 按当前状态动态显示允许的操作

---

## 状态标签配色

| 状态 | 颜色 | 说明 |
|------|------|------|
| 待初始化 | 灰色 | Pending |
| 活跃 | 绿色 | Active |
| 暂停 | 黄色 | Suspended |
| 试用中 | 蓝色 | Trial |
| 已终止 | 红色 | Terminated |

---

## 验收标准

- [ ] 租户列表正确
- [ ] 状态操作按钮动态显隐
- [ ] 生命周期事件展示
- [ ] `npm run build` 通过
