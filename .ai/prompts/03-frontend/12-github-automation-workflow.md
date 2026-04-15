# GitHub Copilot 前端顺序执行 workflow

> 本文件定义如何利用 GitHub Issues、标签、Copilot coding agent、PR 和合并动作，把前端任务变成“少量人工触发、按顺序推进”的流水线。

---

## 一、目标

解决以下问题：

1. 单个任务超过 GitHub Agent 1 小时限制。
2. 大模块一次性开工，最后留下一堆未收尾内容。
3. 每次都需要人工重新描述上下文。
4. 任务之间没有顺序约束，导致前后依赖错乱。

---

## 二、推荐使用的 GitHub 能力

| 能力 | 用途 |
| ------ | ------ |
| Issue | 承载 Epic 和 slice |
| Labels | 表示状态、依赖、模块归属 |
| Projects | 观察切片队列 |
| Copilot coding agent | 执行单个 slice |
| Pull Request | 审查与合并 |
| Auto-merge / Merge Queue | 减少人工重复点击 |

---

## 三、Issue 分层

### 3.1 Epic Issue

一个完整模块一个 Epic，例如：

```text
[FE][F2-2] 平台用户管理
```

Epic 负责：

1. 汇总完整模块需求。
2. 链接业务提示词。
3. 列出所有 slice issue。

### 3.2 Slice Issue

每个 slice 一个 issue，例如：

```text
[FE][F2-2A] 平台用户管理 - 合同与骨架
[FE][F2-2B] 平台用户管理 - 搜索与列表
[FE][F2-2C] 平台用户管理 - 表单与校验
[FE][F2-2D] 平台用户管理 - 状态动作与权限
[FE][F2-2E] 平台用户管理 - E2E 与自审
```

---

## 四、标签体系

| 标签 | 含义 |
| ------ | ------ |
| `frontend` | 前端任务 |
| `copilot` | 由 Copilot coding agent 执行 |
| `slice` | 可在单轮内完成的执行单元 |
| `ready` | 依赖已满足，可启动 |
| `blocked` | 被依赖或外部条件阻塞 |
| `agent-running` | 当前正在由 Agent 执行 |
| `await-review` | PR 已创建，等待审查 |
| `done` | 合并完成 |

---

## 五、顺序执行规则

1. 同一 Epic 同一时刻只允许一个 `ready` slice。
2. 上一个 slice 合并前，下一个 slice 不得变为 `ready`。
3. 遇到共享文件改动或共享资产升级时，后续 slice 一律暂停。
4. 如果某个 slice 预计超过 35 分钟编码或超过 2 个共享文件改动，必须再拆子 slice。

---

## 六、推荐流程

### 6.1 一次性准备

人类或 Agent 先做一次准备：

1. 创建 Epic issue。
2. 按 `01-task-splitting.md` 创建全部 slice issue。
3. 仅给第一个 slice 打 `ready` 标签。

### 6.2 顺序执行

每轮只做以下少量动作：

1. 给 `ready` slice 指派 Copilot。
2. Copilot 完成后创建 PR。
3. 审查并合并 PR。
4. 把当前 slice 标记为 `done`。
5. 把下一个 slice 标记为 `ready` 并指派 Copilot。

---

## 七、最小人工输入策略

为了尽量减少人工输入，建议：

1. Epic issue 中预先写好全部 slice 标题和依赖顺序。
2. slice issue 正文直接使用 `.ai/templates/frontend-slice-task-template.md`。
3. 每个 slice 正文都链接相同的前置阅读文件，避免每轮重写上下文。
4. PR 描述中固定带上“下一 issue 编号”。

---

## 八、示例：F2-2 的顺序队列

```text
Epic: [FE][F2-2] 平台用户管理
  -> Slice: [FE][F2-2A] 合同与骨架          (ready)
  -> Slice: [FE][F2-2B] 搜索与列表          (blocked by F2-2A)
  -> Slice: [FE][F2-2C] 表单与校验          (blocked by F2-2B)
  -> Slice: [FE][F2-2D] 状态动作与权限      (blocked by F2-2C)
  -> Slice: [FE][F2-2E] E2E 与自审          (blocked by F2-2D)
```

---

## 九、与仓库内文件的关系

| GitHub 实体 | 仓库内对应 |
| ------------ | ----------- |
| Epic / Slice issue | `.ai/tasks/platform-frontend/*.md` 当前状态 |
| PR | `.ai/workspace/session-summary-*.md` 记录的实际结果 |
| 审查意见 | 必要时回写到 `03-frontend/03` 或 `13` |

---

## 十、版本

- 版本：1.0
- 创建日期：2026-04-15
- 创建原因：适配 GitHub Copilot coding agent 的 1 小时限制，降低人工重复输入成本
