# 租户平台 — 首页仪表盘

## 目标

实现或完善首页仪表盘页面。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/prompts/03-frontend/page-module.md`
- `backend/platform-operation-api.md`

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
