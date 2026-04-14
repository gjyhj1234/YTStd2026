# 子任务 07 — F2-11 账单管理页面

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 07-F2-11 |
| 模块名称 | 账单管理 |
| 并行组 | C |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0030_billing-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/billing-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成），F2-10 订阅管理（建议先完成） |

---

## 任务目标

实现账单管理页面（F2-11），包含账单列表、账单详情、支付状态管理。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0030_billing-page.md` — 页面功能详细定义
- `.ai/prompts/08-platform/backend/billing-api.md` — 后端 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/billing/BillingView.vue` | 账单管理页面组件 |
| `src/WebTenantPlatfrom/src/views/billing/BillingView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/billing/BillingView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/billing/BillingView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/billing/BillingView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/billing/BillingView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/api/billing.ts` | 账单 API 封装 |
| `src/WebTenantPlatfrom/src/types/billing.ts` | 账单类型定义 |
| `src/WebTenantPlatfrom/src/router.ts` | 路由更新 |
| `src/WebTenantPlatfrom/e2e/tests/billing/billing.spec.ts` | E2E 测试 |

---

## 注意事项

- 账单管理依赖订阅数据，E2E 测试中需要通过 API 创建前置数据
- 参考 `.ai/system/e2e-testing-workflow.md` 中"方式 1：API 直接创建"

---

## 验收标准

- [ ] 页面按 `0030_billing-page.md` 实现所有功能点
- [ ] 账单列表和详情展示正常
- [ ] 5 个语言文件 key 完全一致
- [ ] `npm run build` 通过
- [ ] E2E 测试通过
- [ ] self-review F1-F7 全部通过

---

## 续接说明

完成后更新：
1. `.ai/tasks/platform-frontend/README.md` 中 07 行状态 → ✅
2. `.ai/prompts/08-platform/frontend/0000_overview.md` 中 F2-11 状态 → ✅
3. 输出 session-summary
