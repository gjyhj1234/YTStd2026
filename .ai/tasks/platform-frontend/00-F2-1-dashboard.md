# 子任务 F2-1 — 仪表盘

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F2-1 |
| 模块名称 | 仪表盘（Dashboard） |
| 并行组 | — （首个业务页面，串行） |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0020_dashboard-page.md` |
| 后端 API 提示词 | 无（仪表盘使用静态/mock 数据或汇总 API） |
| 依赖任务 | F1-1 主布局（✅ 已完成）、F1-2 登录页（✅ 已完成） |
| 完成会话 | `session-summary-20260413-12` |
| 状态 | ✅ 已完成 |

---

## 任务目标

实现仪表盘页面（F2-1），作为登录后的默认首页，包含统计卡片、图表区域、快捷操作入口。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0020_dashboard-page.md` — 仪表盘页面功能定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue` | 仪表盘页面组件 |
| `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/router.ts`（修改） | dashboard 路由指向实际组件 |
| `src/WebTenantPlatfrom/e2e/tests/dashboard/dashboard.spec.ts` | 仪表盘 E2E 测试（20+ 用例） |

---

## 核心功能清单

| 功能 | 说明 |
|------|------|
| 统计卡片 | 4 个统计卡片（租户总数、活跃租户、本月订阅、本月收入），含图标和数值 |
| 图表区域 | 折线图（趋势）、柱状图（对比）、饼图（分布） |
| 快捷操作 | 常用管理入口快捷链接 |
| 路由 | `/dashboard` 为默认首页 |

---

## 验收标准

- [x] 页面按 `0020_dashboard-page.md` 实现所有功能点
- [x] 统计卡片渲染正确（4 个，含图标和数值）
- [x] 图表区域存在（折线图、柱状图、饼图容器）
- [x] 快捷操作区域可见
- [x] 5 个语言文件 key 完全一致
- [x] `npm run build` 通过
- [x] E2E 测试编写完成（20+ 用例）

---

## 已完成说明

本子任务已在 `session-summary-20260413-12` 中完成（与 F1-2 增强一起实现）。主要产出：

1. **DashboardView.vue** — 统计卡片（4 个）、图表区域（折线/柱状/饼图）、快捷操作入口
2. **5 个语言文件** — 仪表盘相关 i18n key
3. **E2E 测试** — dashboard.spec.ts 20+ 个用例（渲染 + 卡片 + 图表 + 快捷操作 + 侧边栏 + 多语言）
