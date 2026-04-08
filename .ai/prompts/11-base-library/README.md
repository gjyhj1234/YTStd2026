# 底层框架工程提示词

## 概述

本目录包含底层框架工程（YTStdSqlBuilder、YTStdLogger、YTStdAdo、YTStdEntity、YTStdI18n 及其 Generator）的审查、优化和调整提示词。

---

## 底层工程保护规则

### 默认保护

底层框架工程在完成代码审查与优化后，进入**受保护状态**：

- 禁止任何业务模块开发任务修改底层工程代码
- 禁止在没有独立调整提示词的情况下修改底层工程代码
- 底层工程的调整必须有独立的、明确的提示词任务

### 允许修改的条件

仅在以下条件**全部满足**时才允许修改底层工程：

1. 存在独立的调整提示词（位于本目录 `.ai/prompts/11-base-library/` 下）
2. 提示词明确指定了调整的目标、范围和验收标准
3. 调整不破坏已有的 API 兼容性（除非提示词明确要求 breaking change）

### 调整提示词放置规则

- 所有底层工程的调整提示词**必须**放置在 `.ai/prompts/11-base-library/` 目录下
- 命名格式：`{工程名}-{调整类型}.md`（如 `ytsti18n-generator-frontend-output.md`）
- 每个调整提示词必须包含完整的目标、输入、输出、约束、验收标准

---

## 工程清单

| 工程 | 路径 | 说明 | 状态 |
|------|------|------|------|
| YTStdSqlBuilder | `src/YTStdSqlBuilder/` | SQL 构建器运行时 | 待审查优化 |
| YTStdSqlBuilder.Generator | `src/YTStdSqlBuilder.Generator/` | SQL 构建器源代码生成器 | 待审查优化 |
| YTStdLogger | `src/YTStdLogger/` | 高性能日志系统 | 待审查优化 |
| YTStdAdo | `src/YTStdAdo/` | 数据库访问层 | 待审查优化 |
| YTStdEntity | `src/YTStdEntity/` | 实体框架 | 待审查优化 |
| YTStdEntity.Generator | `src/YTStdEntity.Generator/` | 实体源代码生成器 | 待审查优化 |
| YTStdI18n | `src/YTStdI18n/` | 国际化 | 待审查优化 |
| YTStdI18n.Generator | `src/YTStdI18n.Generator/` | 国际化源代码生成器 | 待审查优化 |

---

## 本目录文件说明

| 文件 | 用途 |
|------|------|
| `README.md` | 本文件 — 底层工程提示词目录说明 |
| `base-library-review.md` | 底层工程统一代码审查提示词 |
| 其他 `{工程名}-*.md` | 特定工程的调整任务提示词（按需创建） |
