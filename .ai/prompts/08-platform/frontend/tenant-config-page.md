# 租户平台 — 租户配置页面

## 目标

重构系统配置和功能开关前端页面。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `backend/tenant-config-api.md`
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

- 系统配置编辑（键值对表格）
- 功能开关列表（开关切换）

---

## 验收标准

- [ ] 配置编辑正确
- [ ] 开关切换正确
- [ ] `npm run build` 通过
