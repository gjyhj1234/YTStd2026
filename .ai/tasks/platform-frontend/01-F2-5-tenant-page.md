# 子任务 01 — F2-5 租户管理页面

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 01-F2-5 |
| 模块名称 | 租户管理（租户列表） |
| 并行组 | B |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0024_tenant-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/tenant-lifecycle-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成） |

---

## 任务目标

实现租户管理页面（F2-5），包含租户列表、CRUD、状态流转（初始化/暂停/恢复/终止/转正）、生命周期事件查看。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0024_tenant-page.md` — 页面功能详细定义
- `.ai/prompts/08-platform/backend/tenant-lifecycle-api.md` — 后端 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue` | 租户管理页面组件 |
| `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/api/tenants.ts` | 租户 API 封装 |
| `src/WebTenantPlatfrom/src/types/tenants.ts` | 租户类型定义 |
| `src/WebTenantPlatfrom/src/router.ts` | 路由更新（指向实际组件） |
| `src/WebTenantPlatfrom/src/constants/permissions.ts` | 权限码更新 |
| `src/WebTenantPlatfrom/e2e/tests/tenants/tenants.spec.ts` | E2E 测试 |

---

## 验收标准

- [ ] 页面按 `0024_tenant-page.md` 实现所有功能点
- [ ] DxDataGrid + CustomStore 远程分页正常
- [ ] 租户 CRUD（创建/编辑/删除）功能正常
- [ ] 状态流转操作（暂停/恢复/终止/转正）有确认弹窗
- [ ] 生命周期事件查看弹窗正常
- [ ] 搜索和筛选功能正常
- [ ] 5 个语言文件 key 完全一致
- [ ] `npm run build` 通过
- [ ] E2E 测试通过
- [ ] self-review F1-F7 全部通过

---

## 续接说明

完成后更新：
1. `.ai/tasks/platform-frontend/README.md` 中 01 行状态 → ✅
2. `.ai/prompts/08-platform/frontend/0000_overview.md` 中 F2-5 状态 → ✅
3. 输出 session-summary
