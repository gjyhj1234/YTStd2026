# 前端交付 workflow（任务快照、缓存、收尾）

> 本文件定义每个前端 slice 从开工到收尾的状态机，以及必须更新哪些文件。它解决的问题不是“怎么写代码”，而是“怎么证明做完了，并让下一轮能接上”。

---

## 一、状态机

每个前端 slice 必须按以下状态推进：

```text
ready
  -> analyzing
  -> contracting
  -> design_review
  -> implementing
  -> building
  -> testing
  -> self_review
  -> documenting
  -> done
```

如果遇到阻塞，则进入：

```text
blocked
```

---

## 二、每个状态必须产出的内容

| 状态 | 必须产物 |
| ------ | --------- |
| analyzing | 读取文件清单、风险点 |
| contracting | 需求补全矩阵、组件复用决策、dxdocs 查询清单 |
| design_review | 页面结构蓝图、关键交互流、字段覆盖矩阵、操作/API 对照矩阵、状态反馈矩阵 |
| implementing | 已改文件清单、未改共享文件清单 |
| building | `npm run build` 结果 |
| testing | Playwright 命令、结果、覆盖矩阵 |
| self_review | F1-F7 检查结果 |
| documenting | `.ai/tasks/...` 快照、`.ai/workspace/...` 总结、README 同步 |

---

## 三、必须更新的文件

| 场景 | 文件 |
| ------ | ------ |
| 每轮都必须更新 | 当前 `.ai/tasks/platform-frontend/*.md` 任务文件 |
| 每轮都必须新增/更新 | `.ai/workspace/session-summary-*.md` |
| 有目录或状态变化时 | `.ai/README.md`、`.ai/prompts/08-platform/README.md`、相关索引 README |
| 发现新通用缺陷时 | `03-frontend/03/09/10/08/12/13` 中对应文件 |

---

## 四、任务快照最小字段

推荐使用 `.ai/templates/frontend-task-cache-template.md`，最少也要包含：

1. `task_id`
2. `epic`
3. `status`
4. `depends_on`
5. `planned_files`
6. `completed_files`
7. `pending_items`
8. `blocked_by`
9. `updated_at`

---

## 五、session summary 最小字段

1. 本轮目标。
2. 本轮已完成。
3. 实际修改文件。
4. 构建结果。
5. 测试结果。
6. 自审结果。
7. 下一轮继续点。
8. 是否更新了提示词沉淀。

---

## 六、收尾清单

每轮结束前必须逐项确认：

- [ ] 当前 slice 状态已更新到任务文件
- [ ] 可视化设计审查包已完成并与当前实现一致
- [ ] session summary 已写入 `.ai/workspace/`
- [ ] 构建结果已记录
- [ ] E2E 结果已记录
- [ ] 自审结果已记录
- [ ] README 已同步（如有必要）
- [ ] 若发现新通用问题，已更新相应提示词文件

---

## 七、阻塞处理

若进入 `blocked`，必须写清：

1. 阻塞原因。
2. 当前已完成到哪一步。
3. 哪些文件已经安全落地。
4. 下一轮或下一位 Agent 解除阻塞后从哪里继续。

---

## 八、版本

- 版本：1.1
- 创建日期：2026-04-15
- 创建原因：解决“任务进度与任务缓存经常不落文件、续接成本高”的问题，并强制在实现前产出可视化设计审查包
