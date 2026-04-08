# Copilot Instructions

## 提示词体系入口

本仓库使用 `.ai/` 目录下的结构化提示词体系指导 AI 开发。

### 首次执行任务前必须阅读

1. `.ai/README.md` — 提示词体系总览与项目整体结构说明
2. `.ai/system/agent-contract.md` — Agent 协作契约
3. `.ai/rules/global.md` — 全局开发规范
4. `.ai/context/tech-stack.md` — 技术栈约束

### 按需阅读

- `.ai/rules/backend.md` — 后端开发规范
- `.ai/rules/frontend.md` — 前端开发规范
- `.ai/rules/database.md` — 数据库命名规范
- `.ai/rules/api-design.md` — API 设计规范
- `.ai/rules/naming.md` — 命名规范总则
- `.ai/rules/i18n.md` — 国际化规范
- `.ai/rules/generator.md` — 源代码生成器规范
- `.ai/rules/testing.md` — 测试规范
- `.ai/rules/postman.md` — Postman 测试规范
- `.ai/rules/security.md` — 安全规范

### 任务执行

- 使用 `.ai/templates/task-template.md` 定义任务
- 按 `.ai/system/execution-policy.md` 执行
- 按 `.ai/system/session-handoff.md` 续接

### 底层框架维护

- 底层工程（YTStdSqlBuilder、YTStdLogger、YTStdAdo、YTStdEntity、YTStdI18n）的代码审查与优化提示词位于 `.ai/prompts/11-base-library/`
- 这些底层工程除非有独立的明确调整任务提示词，否则禁止修改其代码
- 调整底层工程的提示词同样必须放置在 `.ai/prompts/11-base-library/` 目录下