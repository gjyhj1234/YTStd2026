# 前端并行执行与续接规范（含任务快照）

> 本文件定义哪些前端任务可以并行、哪些必须串行，以及每轮如何把状态写回 `.ai/tasks/` 与 `.ai/workspace/`，避免接力时丢上下文。

---

## 一、总原则

1. **模块内串行，模块间择机并行。**
2. **共享资产未冻结前，不做大规模并行。**
3. **每轮必须写回任务快照，不能只在聊天里说明。**
4. **并行的前提不是“看起来能并行”，而是“共享文件冲突可控”。**

---

## 二、可以并行的场景

### 2.1 模块间并行

在以下条件全部满足时，不同模块的 slice 可以并行：

1. 主布局和公共基础设施稳定。
2. 共享组件标准已冻结。
3. 不会同时修改同一个共享文件。
4. 每个 slice 都有独立的任务快照文件和 session summary。

### 2.2 可安全并行的典型组合

| 组合 | 前提 |
| ------ | ------ |
| 平台用户 vs 租户管理 | 路由和权限常量分段追加，不共改同一个表单资产 |
| 审计日志 vs 通知管理 | 共享组件不变，仅各自模块目录改动 |
| 套餐管理 vs API 集成 | 不共改同一共享枚举/状态资产 |

---

## 三、必须串行的场景

以下情况必须串行：

1. 同一模块的多个 slice。
2. 任一 slice 需要修改共享组件或共享 hooks。
3. 任一 slice 需要改 `router`、`permissions`、全局布局、语言加载核心文件。
4. 任一 slice 需要落新的组件资产标准。
5. 任一 slice 需要更新 `0040_e2e-testing-protocol.md` 或共享测试 helpers。

---

## 四、共享文件锁定清单

以下文件默认视为“共享文件”，同一时刻只允许一个 slice 修改：

| 文件/目录 | 锁定原因 |
| ----------- | --------- |
| `src/WebTenantPlatfrom/src/router*` | 路由注册冲突高 |
| `src/WebTenantPlatfrom/src/constants/permissions.ts` | 权限常量冲突高 |
| `src/WebTenantPlatfrom/src/components/` 共享组件 | 多模块共用 |
| `src/WebTenantPlatfrom/src/composables/` | 行为复用核心 |
| `src/WebTenantPlatfrom/src/locales/index.ts` | i18n 核心 |
| `src/WebTenantPlatfrom/e2e/helpers/` | 所有 E2E 依赖 |
| `.ai/prompts/03-frontend/*` | 治理规则本身 |
| `.ai/prompts/08-platform/frontend/0050_common-components-standard.md` | 平台前端共享标准 |

---

## 五、任务快照（强制）

每轮结束时，必须在当前 `.ai/tasks/...` 任务文件中维护一个可机读的快照区块。推荐格式：

````markdown
## 任务快照

```yaml
task_id: F2-2C
epic: F2-2
status: in_progress
depends_on:
  - F2-2B
updated_at: 2026-04-15T10:30:00+08:00
planned_files:
  - src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue
  - src/WebTenantPlatfrom/src/api/platform-users.ts
completed_files:
  - src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue
pending_items:
  - 编辑表单唯一性校验
  - E2E 用例补齐
blocked_by: []
```
````

> 没有任务快照，就视为本轮不可续接。

---

## 六、任务缓存（强制）

除了任务快照，还必须在 `.ai/workspace/session-summary-*.md` 中记录任务缓存，至少包含：

1. 本轮读取的规则文件。
2. dxdocs 查询问题与采用的属性。
3. 组件复用决策。
4. 构建命令与结果。
5. E2E 命令与结果。
6. 剩余风险和下一轮入口。

推荐直接使用 `.ai/templates/frontend-task-cache-template.md`。

---

## 七、续接规则

新一轮接手前，Agent 必须按顺序做以下事情：

1. 阅读最新的 `.ai/workspace/session-summary-*.md`。
2. 阅读当前任务文件中的“任务快照”。
3. 对照 Git 当前状态确认已修改文件是否与快照一致。
4. 只从快照中的 `pending_items` 继续，不得擅自重开已完成切片。
5. 如果发现快照与代码不一致，必须先在 session summary 中记录差异再继续。

---

## 八、中断处理

### 8.1 时间将尽时

如果预计本轮无法收尾，必须优先做以下事情：

1. 完成当前最小逻辑单元。
2. 更新任务快照。
3. 写 session summary。
4. 明确指出下一轮应从哪个文件、哪个函数、哪个测试继续。

### 8.2 并行冲突时

1. 不自行 merge 冲突。
2. 在任务快照中把状态改为 `blocked`。
3. 明确写出冲突文件和冲突原因。
4. 等待共享文件持有者完成后再继续。

---

## 九、与 GitHub Issue 的关系

| 实体 | 作用 |
| ------ | ------ |
| GitHub Issue | 描述 slice 的计划与依赖 |
| `.ai/tasks/...` | 记录 slice 的运行中状态 |
| `.ai/workspace/session-summary-*.md` | 记录 slice 的实际执行证据 |

三者缺一不可。只有 Issue 没有任务快照，会导致中断后无法无损续接。

---

## 十、版本

- 版本：2.0
- 更新日期：2026-04-15
- 更新重点：新增任务快照与任务缓存要求，明确前端并行的文件锁和续接机制
