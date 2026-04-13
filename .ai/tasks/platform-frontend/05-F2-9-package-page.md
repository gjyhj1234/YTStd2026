# 子任务 05 — F2-9 套餐管理页面

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 05-F2-9 |
| 模块名称 | 套餐管理 |
| 并行组 | C |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0028_package-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/package-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成） |

---

## 任务目标

实现套餐管理页面（F2-9），包含套餐列表、CRUD、套餐定价、功能模块配置。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0028_package-page.md` — 页面功能详细定义
- `.ai/prompts/08-platform/backend/package-api.md` — 后端 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue` | 套餐管理页面组件 |
| `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/api/packages.ts` | 套餐 API 封装 |
| `src/WebTenantPlatfrom/src/types/packages.ts` | 套餐类型定义 |
| `src/WebTenantPlatfrom/src/router.ts` | 路由更新 |
| `src/WebTenantPlatfrom/e2e/tests/packages/packages.spec.ts` | E2E 测试 |

---

## 验收标准

- [ ] 页面按 `0028_package-page.md` 实现所有功能点
- [ ] 套餐 CRUD 功能正常
- [ ] 5 个语言文件 key 完全一致
- [ ] `npm run build` 通过
- [ ] E2E 测试通过
- [ ] self-review F1-F7 全部通过

---

## 续接说明

完成后更新：
1. `.ai/tasks/platform-frontend/README.md` 中 05 行状态 → ✅
2. `.ai/prompts/08-platform/frontend/0000_overview.md` 中 F2-9 状态 → ✅
3. 输出 session-summary
