# Agent 协作契约

## 目标

定义 GitHub Agents 与人类开发者之间的协作规则，确保 AI 行为可控、可预测、可审计。

---

## 适用范围

- 所有通过 GitHub Agents 执行的任务
- 所有通过 GitHub Copilot Chat 执行的交互式开发
- Visual Studio 2026 中的 Copilot 辅助编码

---

## Agent 行为准则

### 1. 执行前必须确认

Agent 在开始任何编码任务之前，必须：

1. 阅读 `.ai/system/agent-contract.md`（本文件）
2. 阅读 `.ai/rules/global.md`（全局开发规范）
3. 阅读任务指定的规则文件
4. 阅读任务指定的上下文文件
5. 确认理解任务目标与边界

### 2. 执行中必须遵守

- **只做任务要求的事情**，不自行扩展范围
- **不修改不相关的文件**，除非任务明确要求
- **不引入未要求的依赖**
- **不自行发明与现有框架冲突的实现模式**
- **不删除或修改已有的测试用例**（除非任务明确要求）
- **遇到不确定的设计决策时，必须停下来说明情况并等待人类确认**

### 3. 执行后必须输出

每个任务完成后，Agent 必须输出：

1. 已完成的文件清单
2. 已修改的文件清单
3. 构建验证结果（`dotnet build` / `npm run build`）
4. 测试验证结果（`dotnet test` / `npm run test`，如适用）
5. **代码搜索审查结果**（按 `.ai/system/self-review-protocol.md` 执行，包含每项审查的搜索命令输出和合规数据）
6. 风险点与待确认事项
7. 会话总结（按 `.ai/templates/session-summary-template.md` 格式）

> **重要**：编译和测试通过不等于任务完成。Agent 必须执行代码搜索审查，确认所有编码约束（如 InsertAsync 前有 GetNextLongIdAsync、Logger.Debug 使用 Func<string>、ApiResult.Fail 仅传 ErrorCodes 等）被严格遵守。

---

## 人类职责

人类开发者负责：

1. 制定任务描述和优先级
2. 审查 Agent 输出的代码
3. 在 Visual Studio 2026 中进行本地调试与验证
4. 确认设计决策
5. 合并代码到主分支
6. 管理数据库迁移与生产部署

---

## 文件权限

| 目录/文件 | Agent 权限 | 说明 |
|-----------|-----------|------|
| `.ai/system/` | 只读 | 系统级规则，Agent 不得修改 |
| `.ai/context/` | 只读 | 项目上下文，Agent 不得修改 |
| `.ai/rules/` | 只读 | 开发规则，Agent 不得修改 |
| `.ai/templates/` | 只读 | 模板文件，Agent 不得修改 |
| `.ai/prompts/` | 只读 | 提示词文件，Agent 不得修改 |
| `.ai/tasks/` | 读写 | 任务追踪文件，Agent 可创建和更新 |
| `.ai/workspace/` | 读写 | 临时工作区，Agent 可自由使用 |
| `src/` | 读写 | 源代码，Agent 按任务要求修改 |
| `tests/` | 读写 | 测试代码，Agent 按任务要求修改 |
| `web/` | 读写 | 前端代码，Agent 按任务要求修改 |
| `docs/` | 读写 | 文档，Agent 按任务要求修改 |

---

## 冲突解决

当规则文件之间存在冲突时，优先级顺序为：

1. `.ai/system/agent-contract.md`（最高优先级）
2. `.ai/rules/global.md`
3. 具体领域规则（如 `backend.md`、`frontend.md`）
4. 具体提示词文件
5. Agent 自身判断（最低优先级）

---

## 紧急停止条件

Agent 必须立即停止执行并报告问题，当遇到以下情况：

1. 发现任务要求与全局规则冲突
2. 需要修改的文件超出任务范围
3. 发现潜在的安全漏洞
4. 构建失败且无法在合理范围内修复
5. 需要数据库结构变更但未在任务中明确授权
6. 任务执行时间预计超过单次 Agent 限制

---

## 版本

- 版本：1.0
- 最后更新：2026-04-07
- 维护者：项目负责人
