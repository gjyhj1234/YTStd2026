# 提示词体系审计报告

## 审计日期

2026-04-07

---

## 1. 现有提示词清单

### `.github/prompts/` 下的文件

| 序号 | 文件 | 用途 |
|-----|------|------|
| 1 | `README.md` | 提示词目录说明、全局硬约束 |
| 2 | `entity-prompt.md` | 实体框架使用提示词 |
| 3 | `sql-builder-prompt.md` | SQL 构建器使用提示词 |
| 4 | `ado-prompt.md` | 数据库访问层使用提示词 |
| 5 | `i18n-prompt.md` | 国际化使用提示词 |
| 6 | `logger-prompt.md` | 日志系统使用提示词 |
| 7 | `tenant-platform-backend-prompt.md` | 租户平台后端总规范 |
| 8 | `tenant-platform-frontend-prompt.md` | 租户平台前端总规范 |
| 9 | `tenant-platform-initdata-prompt.md` | 初始化数据规范 |
| 10 | `tenant-platform-stage-01-entity-prompt.md` | 阶段01：实体建模 |
| 11 | `tenant-platform-stage-02-generator-prompt.md` | 阶段02：生成器协同 |
| 12 | `tenant-platform-stage-03-initdata-bootstrap-prompt.md` | 阶段03：数据库引导 |
| 13 | `tenant-platform-stage-04-backend-infrastructure-prompt.md` | 阶段04：后端基础设施 |
| 14 | `tenant-platform-stage-05-backend-api-core-prompt.md` | 阶段05：核心API |
| 15 | `tenant-platform-stage-06-backend-api-extended-prompt.md` | 阶段06：扩展API |
| 16 | `tenant-platform-stage-07-frontend-foundation-prompt.md` | 阶段07：前端骨架 |
| 17 | `tenant-platform-stage-08-frontend-modules-prompt.md` | 阶段08：前端模块 |
| 18 | `tenant-platform-stage-09-final-validation-prompt.md` | 阶段09：最终校验 |
| 19 | `tenant-platform-stage-10-frontend-i18n-prompt.md` | 阶段10：前端i18n |

### 其他位置

| 序号 | 文件 | 用途 |
|-----|------|------|
| 20 | `.github/copilot-instructions.md` | Copilot 全局指令 |
| 21 | `docs/prompt-authoring-guide.md` | 提示词编写指南 |
| 22 | `docs/existing-projects-reference.md` | 已有项目API参考 |

---

## 2. 问题总览

### 2.1 结构问题

| 问题 | 严重性 | 说明 |
|------|:------:|------|
| 目录扁平无分层 | 高 | 所有文件放在同一目录，无分类 |
| 职责边界不清 | 高 | 总规范和阶段提示词重复内容多 |
| 缺少系统级协议 | 高 | 无 Agent 协作契约、无执行策略 |
| 缺少任务拆分规范 | 高 | 无标准化任务拆分和续接机制 |
| 缺少通用模板 | 中 | 无任务模板、子任务模板、会话总结模板 |

### 2.2 内容问题

| 问题 | 严重性 | 说明 |
|------|:------:|------|
| 国际化规范不统一 | 高 | 前后端 i18n 职责分工不明确 |
| 数据库命名不完整 | 高 | 前缀体系、索引命名、外键命名缺失 |
| 生成器覆盖不全面 | 高 | 只关注实体，不考虑完整产出链 |
| Postman 测试缺失 | 高 | 无 Postman 测试生成规范 |
| 前端任务粒度过大 | 高 | 单个阶段包含所有模块，导致执行不完整 |
| 后端任务粒度过大 | 高 | 核心 API 和扩展 API 各放一个阶段 |
| 安全规范分散 | 中 | 安全要求散落在多个文件中 |
| 测试规范不完整 | 中 | 测试覆盖要求不明确 |

### 2.3 文档质量问题

| 问题 | 严重性 | 说明 |
|------|:------:|------|
| 术语不统一 | 中 | "应该"和"必须"混用 |
| 缺少验收标准 | 高 | 多数阶段提示词没有明确的完成标准 |
| 缺少输入输出定义 | 高 | 多数提示词没有明确的输入输出 |
| 缺少续接规则 | 高 | 无标准化的多轮续接机制 |
| 缺少禁止事项清单 | 中 | 禁止项散落在不同位置 |

---

## 3. 高风险问题

1. **无任务拆分机制**：大任务在 Agent 超时后无法续接，导致工作丢失
2. **前端阶段过大**：Stage 08 要求一次完成所有业务模块页面，Agent 经常完成不了
3. **国际化混乱**：前后端 i18n 职责不清，导致部分文本硬编码、部分使用 Messages
4. **缺少 Postman**：后端 API 完成后没有配套的 Postman 测试，测试覆盖不足
5. **生成器产出不完整**：只关注实体本身，忽略 DTO、服务、端点、测试等关联产出

---

## 4. 已清理项

| 已删除文件 | 原因 |
|-----------|------|
| `.github/prompts/` 下全部 21 个文件 | 已迁移到 `.ai/` 体系，避免干扰 |
| `docs/prompt-authoring-guide.md` | 功能已被 `.ai/system/prompt-writing-guide.md` 替代 |

---

## 5. 应合并项

| 旧文件 | 合并到 |
|-------|-------|
| `entity-prompt.md` + `stage-01` + `stage-02` | `.ai/rules/generator.md` + `.ai/prompts/02-backend/entity-modeling.md` |
| `sql-builder-prompt.md` + `ado-prompt.md` | `.ai/context/existing-modules.md` |
| `logger-prompt.md` | `.ai/rules/global.md`（日志规则部分） |
| `i18n-prompt.md` + `stage-10` | `.ai/rules/i18n.md` |
| `backend-prompt.md` + `stage-04/05/06` | `.ai/rules/backend.md` + `.ai/prompts/02-backend/` |
| `frontend-prompt.md` + `stage-07/08` | `.ai/rules/frontend.md` + `.ai/prompts/03-frontend/` |
| `initdata-prompt.md` + `stage-03` | `.ai/prompts/04-database/seed-data.md` |

---

## 6. 应拆分项

| 旧文件 | 拆分为 |
|-------|-------|
| `backend-prompt.md`（800+ 行） | `rules/backend.md` + `rules/api-design.md` + `rules/naming.md` + 多个提示词文件 |
| `frontend-prompt.md`（500+ 行） | `rules/frontend.md` + `rules/i18n.md` + 多个提示词文件 |
| `stage-05`（核心 API） | 每个模块独立子任务 |
| `stage-06`（扩展 API） | 每个模块独立子任务 |
| `stage-08`（前端模块） | 每个模块独立子任务 |

---

## 7. 缺失项

| 缺失内容 | 新文件位置 |
|---------|-----------|
| Agent 协作契约 | `.ai/system/agent-contract.md` |
| 执行策略 | `.ai/system/execution-policy.md` |
| 任务拆分规范 | `.ai/system/task-splitting.md` |
| 多轮续接协议 | `.ai/system/session-handoff.md` |
| 输出格式规范 | `.ai/system/output-format.md` |
| 完成标准定义 | `.ai/system/done-criteria.md` |
| 审查策略 | `.ai/system/review-policy.md` |
| 风险控制 | `.ai/system/risk-control.md` |
| 任务模板 | `.ai/templates/task-template.md` |
| 子任务模板 | `.ai/templates/subtask-template.md` |
| 会话总结模板 | `.ai/templates/session-summary-template.md` |
| 数据库完整命名规范 | `.ai/rules/database.md` |
| Postman 测试规范 | `.ai/rules/postman.md` |
| 安全规范 | `.ai/rules/security.md` |
| 文档规范 | `.ai/rules/documentation.md` |
| 阶段推进计划 | `.ai/prompts/00-governance/phase-plan.md` |
| 重建指南 | `.ai/prompts/00-governance/reconstruction-guide.md` |
| 平台模块提示词 | `.ai/prompts/08-platform/` |
| 审查提示词 | `.ai/prompts/10-review/` |

---

## 8. 新体系设计原则

1. **结构化优先**：标准化结构（目标/输入/输出/约束/验收）
2. **可执行优先**：具体步骤，非原则声明
3. **小步迭代优先**：大任务拆分为可独立执行的子任务
4. **可续跑优先**：标准化续接协议和会话总结
5. **一致性优先**：命名、格式、规则跨文件统一
6. **平台优先于业务**：先底座后业务
7. **测试不后置**：每个模块同步测试和 Postman
8. **最小风险优先**：可验证、可回滚的推进方式
