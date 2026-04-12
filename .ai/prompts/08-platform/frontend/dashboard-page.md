# 租户平台 — 首页仪表盘

## 目标

实现或完善首页仪表盘页面。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/prompts/03-frontend/page-module.md`
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

## 页面内容

### 统计卡片

| 指标 | 数据源 |
|------|--------|
| 租户总数 | GET `/api/platform-operations/dashboard` |
| 活跃租户 | 同上 |
| 用户总数 | 同上 |
| 本月收入 | 同上 |

### 图表

1. 租户增长趋势（最近 30 天）
2. 订阅分布（按套餐）
3. 收入趋势（最近 12 月）

### 快捷操作

- 创建租户
- 创建用户
- 查看审计日志

---

## 验收标准

- [ ] 统计数据正确展示
- [ ] 图表渲染正常
- [ ] 快捷操作可用
- [ ] `npm run build` 通过
