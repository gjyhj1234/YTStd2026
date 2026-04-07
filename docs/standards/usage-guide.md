# 提示词体系使用说明

## 概述

本文档说明如何使用 `.ai/` 下的提示词体系配合 GitHub Agents 和 Visual Studio 2026 进行开发。

---

## 第一次初始化项目

1. 确认 `.ai/` 目录下所有文件就绪
2. 向 Agent 发送初始化指令：

```markdown
请阅读以下文件，然后执行阶段 P0 和 P1：
- .ai/system/agent-contract.md
- .ai/rules/global.md
- .ai/context/tech-stack.md
- .ai/prompts/00-governance/phase-plan.md
```

3. Agent 确认规则后，人类审查确认
4. 开始按 `phase-plan.md` 的顺序推进

---

## 每一轮任务执行

### 向 Agent 发送任务

```markdown
任务：{任务描述}

请先阅读：
- .ai/system/agent-contract.md
- .ai/rules/global.md
- {相关规则文件}
- {相关提示词文件}

然后执行：
- {具体要求}

完成后请按 .ai/templates/session-summary-template.md 输出会话总结。
```

### Agent 执行流程

1. 阅读指定文件
2. 分析任务
3. 如任务过大，按 `.ai/system/task-splitting.md` 拆分
4. 逐步实现
5. 编译验证
6. 输出会话总结

---

## 中断后续接

### 人类操作

1. 查看最新的 `.ai/workspace/session-summary-*.md`
2. 向 Agent 发送续接指令：

```markdown
请继续上一轮未完成的任务。

请先阅读：
- .ai/workspace/session-summary-{最新日期}.md
- .ai/system/agent-contract.md
- .ai/rules/global.md

然后从上次中断处继续。
```

### Agent 恢复

1. 阅读会话总结
2. 检查代码库状态
3. 从"下一轮应继续"处接续

---

## 大任务分块控制

### 后端模块

每个业务模块拆分为独立子任务：
- 子任务A：实体与枚举
- 子任务B：DTO
- 子任务C：应用服务
- 子任务D：API 端点
- 子任务E：测试
- 子任务F：Postman

### 前端模块

每个业务模块拆分为独立子任务：
- 子任务A：类型与 API 封装
- 子任务B：列表页
- 子任务C：表单页
- 子任务D：详情页
- 子任务E：权限与路由
- 子任务F：国际化

---

## 逐步重建系统

按 `docs/reconstruction/roadmap.md` 中的阶段顺序执行：

1. 基础建立（数据库 + 实体）
2. 后端底座（中间件 + 初始化）
3. 核心后端 API
4. 前端构建
5. 完善（i18n + 测试 + 文档）
6. 审查收尾

---

## 人类介入点

### 必须停下确认的操作

- 数据字典变更
- 新增外部依赖
- 中间件管道顺序
- 权限模型设计
- 数据库结构变更
- 删除文件

### 可以让 Agent 连续执行的操作

- 创建 DTO
- 创建应用服务
- 创建 API 端点
- 创建前端页面
- 创建测试
- 添加注释
- 更新文档

---

## Visual Studio 2026 本地协作

### 人类在 VS 中负责

- 调试后端应用
- 调试前端应用
- 审查 Agent 提交的代码
- 执行数据库迁移
- 测试集成环境
- 确认 UI 效果

### Agent 负责

- 编写代码
- 生成测试
- 生成文档
- 生成 Postman 集合
- 代码审查
