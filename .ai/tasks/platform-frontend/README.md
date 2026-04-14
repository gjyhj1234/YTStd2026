# 租户平台前端 — 子任务索引

> 本目录将"前端实现"拆分为独立的、可单次完成的子任务。
> 每个子任务聚焦**一个功能模块**，确保 Agent 在单次会话内能够完整交付（编码 → 构建 → E2E 测试 → 自审）。

---

## 目录结构

```
platform-frontend/
├── README.md                          # 本文件 — 子任务索引
├── 00-common-prereqs.md               # 通用前置阅读与环境约束（每个子任务前必读）
├── 00-F0-1-scaffold.md                # F0-1 脚手架搭建 ✅
├── 00-F0-2-axios.md                   # F0-2 axios 封装 ✅
├── 00-F0-3-i18n.md                    # F0-3 i18n 基础设施 ✅
├── 00-F0-4-router-guards.md           # F0-4 路由与权限守卫 ✅
├── 00-F0-5-state-management.md        # F0-5 状态管理 ✅
├── 00-F0-6-common-components.md       # F0-6 通用组件 ✅
├── 01-F2-5-tenant-page.md             # F2-5 租户管理
├── 02-F2-6-tenant-info-page.md        # F2-6 租户信息管理
├── 03-F2-7-tenant-resource-page.md    # F2-7 租户资源管理
├── 04-F2-8-tenant-config-page.md      # F2-8 租户配置管理
├── 05-F2-9-package-page.md            # F2-9 套餐管理
├── 06-F2-10-subscription-page.md      # F2-10 订阅管理
├── 07-F2-11-billing-page.md           # F2-11 账单管理
├── 08-F2-12-api-integration-page.md   # F2-12 API 集成
├── 09-F2-13-audit-page.md             # F2-13 审计日志
├── 10-F2-14-notification-page.md      # F2-14 通知管理
├── 11-F2-15-storage-page.md           # F2-15 文件管理
├── 12-F2-16-platform-operation-page.md # F2-16 平台运营
├── 13-e2e-catch-up.md                 # E2E 测试补齐（F2-2/F2-3/F2-4）
└── 14-F3-integration-verify.md        # F3 集成验证（全量编译+i18n+权限）
```

---

## 执行原则

1. **一个子任务 = 一个功能模块 = 一次 Agent 会话**
2. 每个子任务文件的第一行指向 `00-common-prereqs.md`，不重复罗列通用依赖
3. 每个子任务对应 `.ai/prompts/08-platform/frontend/` 下的一个业务提示词文件（如 `0024_tenant-page.md`）
4. 子任务编号前缀按执行建议顺序排列，但同一并行组内可任意顺序
5. Agent 完成子任务后必须输出 session-summary 至 `.ai/workspace/`

---

## 执行状态

| 序号 | 子任务 | 模块 | 并行组 | 状态 |
|:----:|--------|------|:------:|:----:|
| 00 | 通用前置 | — | — | ✅ 已编写 |
| F0-1 | 脚手架搭建 | 基础设施 | — | ✅ |
| F0-2 | axios 封装 | 基础设施 | — | ✅ |
| F0-3 | i18n 基础设施 | 基础设施 | — | ✅ |
| F0-4 | 路由与权限守卫 | 基础设施 | — | ✅ |
| F0-5 | 状态管理 | 基础设施 | — | ✅ |
| F0-6 | 通用组件 | 基础设施 | — | ✅ |
| 01 | F2-5 租户管理 | 租户列表 | B | ⬜ |
| 02 | F2-6 租户信息 | 租户信息 | B | ⬜ |
| 03 | F2-7 租户资源 | 资源配额 | B | ⬜ |
| 04 | F2-8 租户配置 | 配置开关 | B | ⬜ |
| 05 | F2-9 套餐管理 | 套餐管理 | C | ⬜ |
| 06 | F2-10 订阅管理 | 订阅管理 | C | ⬜ |
| 07 | F2-11 账单管理 | 账单管理 | C | ⬜ |
| 08 | F2-12 API 集成 | API 集成 | D | ⬜ |
| 09 | F2-13 审计日志 | 审计日志 | D | ⬜ |
| 10 | F2-14 通知管理 | 通知管理 | D | ⬜ |
| 11 | F2-15 文件管理 | 文件管理 | D | ⬜ |
| 12 | F2-16 平台运营 | 平台运营 | D | ⬜ |
| 13 | E2E 补齐 | F2-2/3/4 | — | ⬜ |
| 14 | 集成验证 | 全量 | — | ⬜ |

---

## 并行组说明

| 组 | 包含模块 | 依赖 | 说明 |
|:--:|---------|------|------|
| B | F2-5 ~ F2-8 | F1-1 布局层 | 租户管理相关，组内无依赖 |
| C | F2-9 ~ F2-11 | F1-1 布局层 | SaaS 运营相关，F2-10 依赖 F2-9，F2-11 依赖 F2-10 |
| D | F2-12 ~ F2-16 | F1-1 布局层 | 系统管理相关，组内无依赖 |

---

## 已完成的子任务（历史）

以下模块已在之前的会话中完成，无需重复执行：

| 编号 | 模块 | 完成会话 | 子任务文件 |
|:----:|------|---------|-----------|
| F0-1 | 脚手架搭建 | session-summary-20260413-09 | `00-F0-1-scaffold.md` |
| F0-2 | axios 封装 | session-summary-20260413-09 | `00-F0-2-axios.md` |
| F0-3 | i18n 基础设施 | session-summary-20260413-09 | `00-F0-3-i18n.md` |
| F0-4 | 路由与权限守卫 | session-summary-20260413-10 | `00-F0-4-router-guards.md` |
| F0-5 | 状态管理 | session-summary-20260413-09/10 | `00-F0-5-state-management.md` |
| F0-6 | 通用组件 | session-summary-20260413-10 | `00-F0-6-common-components.md` |
| F1-1 | 主布局 | session-summary-20260413-10 | — |
| F1-2 | 登录页 | session-summary-20260413-11 | — |
| F2-1 | 仪表盘 | session-summary-20260413-12 | — |
| F2-2 | 平台用户管理 | session-summary-20260413-13 | — |
| F2-3 | 平台角色管理 | session-summary-20260413-13 | — |
| F2-4 | 平台权限管理 | session-summary-20260413-13 | — |

---

## 启动指引

执行某个子任务时，使用以下提示词模板：

```markdown
请继续前端开发。

## 上下文恢复
1. 阅读 `.ai/tasks/platform-frontend/00-common-prereqs.md`
2. 阅读 `.ai/tasks/platform-frontend/{nn}-{子任务文件名}.md`
3. 阅读 `.ai/workspace/session-summary-{最新日期}.md`

## 本轮目标
完成子任务 {编号} — {模块名}
```
