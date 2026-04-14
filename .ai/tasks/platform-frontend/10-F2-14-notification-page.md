# 子任务 10 — F2-14 通知管理页面

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 10-F2-14 |
| 模块名称 | 通知管理 |
| 并行组 | D |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0033_notification-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/notification-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成） |

---

## 任务目标

实现通知管理页面（F2-14），包含通知模板管理、发送记录、通知渠道配置。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0033_notification-page.md` — 页面功能详细定义
- `.ai/prompts/08-platform/backend/notification-api.md` — 后端 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/notifications/NotificationsView.vue` | 通知管理页面组件 |
| `src/WebTenantPlatfrom/src/views/notifications/NotificationsView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/notifications/NotificationsView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/notifications/NotificationsView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/notifications/NotificationsView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/notifications/NotificationsView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/api/notifications.ts` | 通知 API 封装 |
| `src/WebTenantPlatfrom/src/types/notifications.ts` | 通知类型定义 |
| `src/WebTenantPlatfrom/src/router.ts` | 路由更新 |
| `src/WebTenantPlatfrom/e2e/tests/notifications/notifications.spec.ts` | E2E 测试 |

---

## 验收标准

- [ ] 页面按 `0033_notification-page.md` 实现所有功能点
- [ ] 通知模板和渠道管理功能正常
- [ ] 5 个语言文件 key 完全一致
- [ ] `npm run build` 通过
- [ ] E2E 测试通过
- [ ] self-review F1-F7 全部通过

---

## 续接说明

完成后更新：
1. `.ai/tasks/platform-frontend/README.md` 中 10 行状态 → ✅
2. `.ai/prompts/08-platform/frontend/0000_overview.md` 中 F2-14 状态 → ✅
3. 输出 session-summary
