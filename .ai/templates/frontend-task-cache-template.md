# 前端任务快照与缓存模板

在 `.ai/tasks/...` 和 `.ai/workspace/session-summary-*.md` 中推荐使用以下结构。

---

````markdown
## 任务快照

```yaml
task_id: F2-2C
epic: F2-2
title: 平台用户管理 - 表单与校验
status: ready | analyzing | contracting | implementing | building | testing | self_review | documenting | done | blocked
depends_on:
  - F2-2B
updated_at: 2026-04-15T10:30:00+08:00
planned_files:
  - src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue
completed_files:
  - src/WebTenantPlatfrom/src/views/platform-users/PlatformUsersView.vue
pending_items:
  - 编辑场景唯一性校验
blocked_by: []
github_issue: 123
pull_request: 456
```

## 交付缓存

### 本轮读取
- `.ai/prompts/03-frontend/00-governance.md`
- `.ai/prompts/03-frontend/09-production-defaults.md`

### dxdocs 记录
| 组件 | 查询问题 | 采用能力 |
|------|---------|---------|
| DxForm | async validation | async unique |

### 组件复用决策
| 功能点 | 决策 | 说明 |
|--------|------|------|
| 搜索区 | 复用共享样式 | 未新建组件 |

### 验证结果
| 类型 | 命令 | 结果 |
|------|------|------|
| build | `cd src/WebTenantPlatfrom && npm run build` | 通过 |
| e2e | `cd src/WebTenantPlatfrom && npx playwright test ...` | 通过 |

### 下一轮入口
- 从 `{文件路径}` 的 `{功能点}` 继续
- 下一步：{说明}

### 提示词沉淀
| 问题 | 回写文件 | 结果 |
|------|---------|------|
| | | |
````
