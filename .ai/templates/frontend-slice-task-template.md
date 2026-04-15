# 前端 Slice 任务模板

使用本模板创建 GitHub issue、`.ai/tasks/` 子任务文件或本地执行任务说明。

---

```markdown
# 任务：{模块名称} - {Slice 名称}

## 任务信息

| 属性 | 值 |
|------|---|
| Epic | {如 F2-2} |
| Slice | {如 F2-2C} |
| 标题 | {如 平台用户管理 - 表单与校验} |
| 依赖 | {前置 slice} |
| 预计耗时 | 20-35 分钟 |
| 目标文件数 | 2-6 |

## 前置阅读

- `.ai/prompts/03-frontend/00-governance.md`
- `.ai/prompts/03-frontend/01-task-splitting.md`
- `.ai/prompts/03-frontend/04-devextreme-templates.md`
- `.ai/prompts/03-frontend/08-playwright-e2e.md`
- `.ai/prompts/03-frontend/09-production-defaults.md`
- `.ai/prompts/03-frontend/10-component-asset-catalog.md`
- `.ai/prompts/03-frontend/11-delivery-workflow.md`
- `.ai/prompts/03-frontend/12-github-automation-workflow.md`
- {对应业务提示词}

## 需求补全矩阵

| 类别 | 显式需求 | 生产默认补全 | 待确认项 | 明确不做项 |
|------|---------|-------------|---------|-----------|
| 页面结构 | | | | |
| 组件能力 | | | | |
| 权限 | | | | |
| 测试 | | | | |

## 组件复用决策表

| 功能点 | 复用资产 | 当前仓库状态 | 决策 | 理由 |
|--------|---------|-------------|------|------|
| | | | | |

## 可视化设计审查包

> 如果当前 slice 只覆盖模块的一部分，只保留本 slice 相关的页面区块、字段、操作和状态，禁止复制整页无关内容。

### 页面结构蓝图

~~~mermaid
flowchart TB
  Page["页面 / 子流程"]
  Page --> SectionA["区块 A"]
  Page --> SectionB["区块 B"]
  Page --> Popup["弹窗 / 抽屉"]
~~~

### 关键交互流

~~~mermaid
flowchart LR
  Enter[进入当前 slice] --> Action[执行关键操作]
  Action --> Validate{"校验通过?"}
  Validate -- 否 --> Stay[停留并反馈]
  Validate -- 是 --> Submit[调用 API]
  Submit --> Done[刷新 / 定位 / 关闭]
~~~

### 字段覆盖矩阵

| 字段 | 查询 | 列表 | 详情 | 新增 | 编辑 | 必填 | 只读 | 校验/说明 |
|------|------|------|------|------|------|------|------|-----------|
| | | | | | | | | |

### 操作/API 对照矩阵

| 操作 | 入口 | 权限 | API | 确认 | 成功反馈 | E2E |
|------|------|------|-----|------|----------|-----|
| | | | | | | |

### 状态反馈矩阵

| 场景 | 触发条件 | UI 承载 | 用户可见反馈 | 恢复方式 |
|------|---------|---------|-------------|---------|
| | | | | |

## dxdocs 查询清单

| 组件 | 查询问题 | 预期采用能力 |
|------|---------|-------------|
| | | |

## DevExtreme 能力矩阵

| 组件 | 能力 | 状态 | 配置 |
|------|------|------|------|
| DxDataGrid | 默认排序 | | |
| DxDataGrid | 列隐藏 | | |
| DxForm | 异步唯一性校验 | | |

## 必须产出的文件

- {文件路径} — {作用}

## 测试证据矩阵

| 场景 | 测试文件 | 核心断言 | 前置数据 |
|------|---------|---------|---------|
| | | | |

## 退出条件

- [ ] 当前 slice 功能点完成
- [ ] 可视化设计审查包已补齐并与当前 slice 一致
- [ ] 当前 slice 对应文件已更新
- [ ] `npm run build` 结果已记录
- [ ] Playwright 结果已记录（如适用）
- [ ] `.ai/tasks/...` 任务快照已更新
- [ ] `.ai/workspace/session-summary-*.md` 已输出
```
