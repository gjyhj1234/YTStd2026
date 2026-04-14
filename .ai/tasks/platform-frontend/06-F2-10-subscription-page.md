# 子任务 06 — F2-10 订阅管理页面

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 06-F2-10 |
| 模块名称 | 订阅管理 |
| 并行组 | C |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0029_subscription-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/subscription-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成），F2-9 套餐管理（建议先完成） |

---

## 任务目标

实现订阅管理页面（F2-10），包含订阅列表、创建订阅、续期、升级、取消、到期处理。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0029_subscription-page.md` — 页面功能详细定义
- `.ai/prompts/08-platform/backend/subscription-api.md` — 后端 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue` | 订阅管理页面组件 |
| `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/api/subscriptions.ts` | 订阅 API 封装 |
| `src/WebTenantPlatfrom/src/types/subscriptions.ts` | 订阅类型定义 |
| `src/WebTenantPlatfrom/src/router.ts` | 路由更新 |
| `src/WebTenantPlatfrom/e2e/tests/subscriptions/subscriptions.spec.ts` | E2E 测试 |

---

## 注意事项

- 订阅管理依赖租户和套餐数据，E2E 测试中需要通过 API 创建前置数据
- 参考 `.ai/system/e2e-testing-workflow.md` 中"方式 1：API 直接创建"

---

## 验收标准

- [ ] 页面按 `0029_subscription-page.md` 实现所有功能点
- [ ] 订阅 CRUD 和状态流转功能正常
- [ ] 5 个语言文件 key 完全一致
- [ ] `npm run build` 通过
- [ ] E2E 测试通过
- [ ] self-review F1-F7 全部通过

---

## 续接说明

完成后更新：
1. `.ai/tasks/platform-frontend/README.md` 中 06 行状态 → ✅
2. `.ai/prompts/08-platform/frontend/0000_overview.md` 中 F2-10 状态 → ✅
3. 输出 session-summary
