# 租户平台 — 平台运营页面

## 目标

重构平台运营统计前端页面。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `backend/platform-operation-api.md`
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

- 与首页仪表盘关联
- 租户统计详细页
- 用户统计详细页
- 收入报表

---

## 验收标准

- [ ] 统计数据展示正确
- [ ] 图表渲染正确
- [ ] `npm run build` 通过
