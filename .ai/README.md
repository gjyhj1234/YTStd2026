# AI 提示词体系

## 概述

本目录包含驱动 GitHub Agents 与 Visual Studio 2026 协作构建整个解决方案的完整提示词体系。

所有提示词面向 **GitHub Copilot / GitHub Agents**（兼容 Claude、GPT 等大语言模型），以结构化方式组织，支持：

- 分阶段执行
- 多轮会话续接
- 前后端分块交付
- 平台优先于业务的构建顺序

---

## 目录结构

```
.ai/
├── README.md                      # 本文件 — 提示词体系总览
├── system/                        # 系统级协议与策略
│   ├── agent-contract.md          # Agent 协作契约
│   ├── execution-policy.md        # 执行策略与行为边界
│   ├── task-splitting.md          # 任务拆分规范
│   ├── session-handoff.md         # 多轮会话续接协议
│   ├── output-format.md           # 输出格式规范
│   ├── done-criteria.md           # 完成标准定义
│   ├── review-policy.md           # 审查策略
│   ├── risk-control.md            # 风险控制规范
│   └── prompt-writing-guide.md    # 提示词编写指南
│
├── context/                       # 项目上下文（不可由 Agent 修改）
│   ├── tech-stack.md              # 技术栈与框架约束
│   ├── project-structure.md       # 项目结构说明
│   └── existing-modules.md        # 已有模块参考
│
├── rules/                         # 开发规则（全局生效）
│   ├── global.md                  # 全局开发规范
│   ├── backend.md                 # 后端开发规范
│   ├── frontend.md                # 前端开发规范
│   ├── database.md                # 数据库与命名规范
│   ├── api-design.md              # API 设计规范
│   ├── naming.md                  # 命名规范总则
│   ├── i18n.md                    # 国际化规范
│   ├── generator.md               # 源代码生成器规范
│   ├── testing.md                 # 测试规范
│   ├── postman.md                 # Postman 测试规范
│   ├── security.md                # 安全规范
│   └── documentation.md           # 文档规范
│
├── templates/                     # 任务与会话模板
│   ├── task-template.md           # 任务模板
│   ├── subtask-template.md        # 子任务模板
│   └── session-summary-template.md # 会话总结模板
│
├── prompts/                       # 分类提示词
│   ├── 00-governance/             # 治理与流程
│   │   ├── phase-plan.md          # 阶段推进计划
│   │   └── reconstruction-guide.md # 重建指南
│   ├── 01-project/                # 项目级提示词
│   │   ├── solution-init.md       # 解决方案初始化
│   │   └── module-scaffold.md     # 模块脚手架
│   ├── 02-backend/                # 后端提示词
│   │   ├── entity-modeling.md     # 实体建模
│   │   ├── app-service.md         # 应用服务
│   │   ├── endpoint.md            # API 端点
│   │   └── middleware.md          # 中间件
│   ├── 03-frontend/               # 前端提示词
│   │   ├── scaffold.md            # 前端脚手架
│   │   ├── page-module.md         # 页面模块
│   │   └── component.md           # 组件
│   ├── 04-database/               # 数据库提示词
│   │   ├── schema-design.md       # 表结构设计
│   │   └── seed-data.md           # 初始化数据
│   ├── 05-i18n/                   # 国际化提示词
│   │   ├── backend-i18n.md        # 后端国际化
│   │   └── frontend-i18n.md       # 前端国际化
│   ├── 06-generator/              # 生成器提示词
│   │   ├── entity-generator.md    # 实体生成器
│   │   └── crud-generator.md      # CRUD 生成器
│   ├── 07-testing/                # 测试提示词
│   │   ├── unit-test.md           # 单元测试
│   │   ├── integration-test.md    # 集成测试
│   │   └── postman-collection.md  # Postman 集合
│   ├── 08-platform/               # 平台模块提示词
│   │   ├── auth.md                # 认证
│   │   ├── permission.md          # 权限
│   │   ├── tenant.md              # 租户
│   │   ├── menu.md                # 菜单
│   │   ├── dictionary.md          # 字典
│   │   ├── config.md              # 配置
│   │   └── audit.md               # 审计
│   ├── 09-docs/                   # 文档提示词
│   │   ├── api-doc.md             # API 文档
│   │   └── architecture-doc.md    # 架构文档
│   ├── 10-review/                 # 审查提示词
│   │   ├── code-review.md         # 代码审查
│   │   └── architecture-review.md # 架构审查
│   └── 11-base-library/           # 底层框架工程提示词
│       ├── README.md              # 底层工程保护规则与目录说明
│       └── base-library-review.md # 底层工程统一代码审查
│
├── tasks/                         # 当前任务追踪（工作区）
│   └── .gitkeep
│
└── workspace/                     # Agent 临时工作区
    └── .gitkeep
```

---

## 核心设计原则

| 原则 | 说明 |
|------|------|
| 结构化优先 | 所有提示词采用标准化结构，包含目标、输入、输出、约束、验收标准 |
| 可执行优先 | 提示词必须可直接指导 AI 执行，不仅是原则声明 |
| 小步迭代优先 | 大任务必须拆分为可独立执行的子任务 |
| 可续跑优先 | 每个长任务支持多轮会话续接 |
| 一致性优先 | 所有规则、命名、格式在提示词间保持统一 |
| 平台优先于业务 | 先构建平台底座，再构建业务模块 |
| 测试不后置 | 测试与 Postman 纳入每个阶段 |
| 最小风险优先 | 以最小范围、可验证、可回滚的方式推进 |

---

## 使用方式

### 首次初始化项目

1. Agent 首先阅读 `.ai/system/agent-contract.md` 了解协作规则
2. 阅读 `.ai/context/tech-stack.md` 了解技术约束
3. 阅读 `.ai/rules/global.md` 了解全局规范
4. 按 `.ai/prompts/00-governance/phase-plan.md` 制定执行计划

### 每轮任务执行

1. 使用 `.ai/templates/task-template.md` 创建任务描述
2. Agent 阅读任务关联的规则文件和上下文文件
3. 执行任务，输出结果
4. 使用 `.ai/templates/session-summary-template.md` 输出会话总结

### 中断后续接

1. 阅读上一轮的会话总结
2. 参照 `.ai/system/session-handoff.md` 恢复上下文
3. 从上次中断处继续执行

---

## 文件编号约定

`prompts/` 下的子目录使用两位数字前缀表示优先级和依赖顺序：

| 编号 | 类别 | 说明 |
|------|------|------|
| 00 | 治理 | 流程与阶段控制 |
| 01 | 项目 | 解决方案与模块初始化 |
| 02 | 后端 | 后端实现提示词 |
| 03 | 前端 | 前端实现提示词 |
| 04 | 数据库 | 数据库设计与数据 |
| 05 | 国际化 | i18n 相关（YTStdI18n.Generator 驱动） |
| 06 | 生成器 | Source Generator 相关 |
| 07 | 测试 | 测试与 Postman |
| 08 | 平台 | 平台能力模块 |
| 09 | 文档 | 文档生成 |
| 10 | 审查 | 代码与架构审查 |
| 11 | 底层框架 | 底层工程审查、优化与调整（受保护） |

---

## 底层框架工程保护

底层框架工程（YTStdSqlBuilder、YTStdLogger、YTStdAdo、YTStdEntity、YTStdI18n 及其 Generator）在完成代码审查与优化后进入**受保护状态**：

- 禁止任何业务模块开发任务修改底层工程代码
- 修改底层工程必须有独立的调整提示词（位于 `.ai/prompts/11-base-library/`）
- 调整提示词必须明确指定目标、范围和验收标准

这些工程的提示词设计为可跨项目复用——任何使用这些底层库的项目都可以引用这些提示词。

---

## 旧提示词已清理

旧提示词（原 `.github/prompts/` 目录下的 21 个文件）已全部删除，不再保留历史参考。所有规范和经验已迁移到 `.ai/` 体系中。新任务统一使用 `.ai/` 下的提示词。
