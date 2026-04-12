# 租户平台 — 订阅管理页面

## 目标

重构订阅管理前端页面。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `backend/subscription-api.md`
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

- 订阅列表
- 续费/升级/取消操作
- 到期预警高亮

---

## 验收标准

- [ ] 订阅列表正确
- [ ] 操作按钮正确
- [ ] `npm run build` 通过
