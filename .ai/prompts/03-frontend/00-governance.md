# 前端总治理提示词（生产级执行版）

> 本文件是前端提示词体系的唯一入口。目标不是“把页面写出来”，而是让 Agent 在需求不完整、时间受限、多人接力、GitHub Copilot 自动编码受 1 小时限制的场景下，仍能稳定交付生产级前端。

---

## 一、必读文件

Agent 执行任何前端任务前，必须按顺序阅读以下文件：

| 序号 | 文件 | 作用 |
| :----: | ------ | ------ |
| 1 | `.ai/system/agent-contract.md` | 协作边界 |
| 2 | `.ai/system/execution-policy.md` | 通用执行闭环 |
| 3 | `.ai/rules/global.md` | 全局规则 |
| 4 | `.ai/rules/frontend.md` | 前端规则 |
| 5 | `.ai/rules/i18n.md` | 国际化规则 |
| 6 | `.ai/context/tech-stack.md` | 技术栈与项目路径 |
| 7 | `.github/copilot-instructions.md` | 内联高频约束 |
| 8 | 本文件 | 前端总治理 |
| 9 | `03-frontend/01-task-splitting.md` | 任务切片规则 |
| 10 | `03-frontend/02-parallel-execution.md` | 并行与续接规则 |
| 11 | `03-frontend/03-anti-patterns.md` | 历史反模式 |
| 12 | `03-frontend/04-devextreme-templates.md` | DevExtreme 与 dxdocs 能力矩阵 |
| 13 | `03-frontend/05-axios-standard.md` | axios 实现规范 |
| 14 | `03-frontend/06-i18n-execution.md` | i18n 执行规范 |
| 15 | `03-frontend/07-business-prompt-template.md` | 业务提示词模板 |
| 16 | `03-frontend/08-playwright-e2e.md` | E2E 证据规范 |
| 17 | `03-frontend/09-production-defaults.md` | 生产级默认能力补全 |
| 18 | `03-frontend/10-component-asset-catalog.md` | 组件资产目录与复用决策 |
| 19 | `03-frontend/11-delivery-workflow.md` | 任务快照、缓存与交付流程 |
| 20 | `03-frontend/12-github-automation-workflow.md` | GitHub Issue 切片与顺序执行 |
| 21 | `03-frontend/13-prompt-evolution-workflow.md` | 缺陷沉淀与提示词迭代 |

---

## 二、体系目标

| 目标 | 说明 |
| ------ | ------ |
| 功能完整优先 | 编译通过不是首要目标，先保证功能点不缺项 |
| 生产默认优先 | 需求没写全时，按生产级默认能力补齐，而不是直接省略 |
| 资产复用优先 | 同类页面、查询区、操作列、表单壳不得反复复制重写 |
| 证据优先 | “已测试”“已验证”必须有命令、结果、覆盖矩阵和快照文件支撑 |
| 续接优先 | 每轮结束必须更新 `.ai/tasks/` 与 `.ai/workspace/`，让下一轮无损接手 |
| GitHub 编排优先 | 任务必须能拆成适配 Copilot Agent 1 小时限制的 issue 切片 |
| 沉淀优先 | 修复一个缺陷，必须反推更新至少一处提示词或 workflow |

---

## 三、前端执行系统（五层）

| 层 | 文件 | 责任 |
| ---- | ------ | ------ |
| 治理层 | `00-governance.md` | 定义总目标、总边界、总验收顺序 |
| 默认能力层 | `09-production-defaults.md` | 在需求缺失时补齐生产级默认行为 |
| 资产复用层 | `10-component-asset-catalog.md` | 定义复用优先、共享资产升级规则 |
| 交付工作流层 | `01/02/08/11` | 定义切片、续接、E2E、任务快照与交付证据 |
| GitHub 编排与演进层 | `12/13` | 定义 issue 队列、顺序执行、缺陷反哺机制 |

---

## 四、编码前必须完成的六个工位

**以下六项未完成前，禁止进入编码。**

| 工位 | 必须产物 | 来源文件 |
| ------ | --------- | --------- |
| 1. 需求补全矩阵 | 明确“显式需求 / 生产默认补全 / 待确认项 / 明确不做项” | `09-production-defaults.md` |
| 2. 组件复用决策表 | 每个关键功能点明确“复用哪个共享资产 / 是否新建资产 / 理由” | `10-component-asset-catalog.md` |
| 3. dxdocs 检索记录 | 每个首次使用或复杂使用的 DevExtreme 组件都要有查询记录 | `04-devextreme-templates.md` |
| 4. DevExtreme 能力矩阵 | DataGrid / Form / Popup / TreeView 等必须逐项声明“启用 / 禁用 / 不适用” | `04-devextreme-templates.md` |
| 5. 权限入口可见性矩阵 | 菜单入口、路由访问、查询权限、按钮权限的关系必须写清 | `07-business-prompt-template.md` |
| 6. E2E 测试证据矩阵 | 先定义准备测什么、如何证明，而不是事后口头说“测过了” | `08-playwright-e2e.md` |

---

## 五、前端任务的强制输出物

每个前端任务必须同时更新以下两类文件：

| 文件 | 作用 | 是否强制 |
| ------ | ------ | :--------: |
| `.ai/tasks/...` 当前任务文件 | 保存任务状态、切片状态、待办、阻塞项 | ✅ |
| `.ai/workspace/session-summary-*.md` | 保存本轮实际执行结果、验证结果、下一轮续接点 | ✅ |

每个前端任务还必须保留以下信息：

1. 实际读取了哪些规则文件。
2. dxdocs 查询了哪些问题，采用了哪些属性和组件。
3. 复用了哪些共享资产，哪些决定提升为共享资产。
4. 实际修改了哪些文件。
5. 实际运行了哪些命令。
6. E2E 测试覆盖了哪些场景，哪些未覆盖，为什么未覆盖。
7. 是否发现了新的通用缺陷模式，若有，更新到哪份提示词。

---

## 六、验收优先级

前端任务必须按以下顺序验收，不能颠倒：

| 优先级 | 维度 | 说明 |
| :------: | ------ | ------ |
| P0 | 功能点完整性 | 页面结构、字段、按钮、状态流转、入口与路径是否完整 |
| P1 | 业务规则完整性 | 权限、校验、默认值、显示条件、危险操作确认是否完整 |
| P2 | 生产质量完整性 | 组件复用、响应式、DataGrid 能力、异常态、加载态、空态是否完整 |
| P3 | i18n 与文本完整性 | 5 语言文件、key 归属、静态配置翻译、提示文本是否完整 |
| P4 | 自动化验证 | `npm run build`、E2E、self-review 是否真实执行并留痕 |

> “编译通过”只能排在 P4，不能代替前面的验收。

---

## 七、生产级默认要求

如果业务提示词未明确说明，Agent 必须按 `09-production-defaults.md` 自动补齐默认能力。典型包括：

1. 管理页默认需要：标题、功能说明、操作指引、搜索、列表、分页、加载态、空态、错误态。
2. CRUD 表单默认需要：字段顺序、必填校验、长度校验、格式校验、异步唯一性校验、提交 loading、成功刷新。
3. 登录/认证页默认需要：连续失败触发验证码、回车提交、密码可见性决策、密码重置路径决策。
4. 权限页默认需要：入口可见性、路由守卫、按钮显隐、只读与不可达的区别。
5. DataGrid 默认必须声明：排序、分页、页大小、固定列、列宽、移动端列隐藏、焦点行、列能力是否启用。

如果默认能力与后端能力冲突，必须停下说明冲突，不能直接删掉该能力。

---

## 八、GitHub Copilot Agent 执行模式

1. **模块提示词是 Epic，不是一次性执行单元。**
2. 实际执行单元必须是 `01-task-splitting.md` 定义的 slice。
3. 每个 slice 的目标时长应控制在 **20-35 分钟编码 + 10-20 分钟验证**。
4. 同一模块同一时刻只允许一个活跃 slice。
5. 多模块并行仅在共享资产和共享文件冻结后才允许。
6. GitHub 自动执行流程、issue 标题、标签和推进规则见 `12-github-automation-workflow.md`。

---

## 九、缺陷沉淀要求

任何前端缺陷如果满足以下任一条件，必须触发提示词更新，而不是只修代码：

1. 同类问题已出现第 2 次。
2. 问题根因是提示词缺失、模板过粗、任务切片过大、验证不足。
3. 问题本可通过默认能力、共享资产或测试协议预防。

更新方向：

| 问题类型 | 必须更新的文件 |
| --------- | --------------- |
| 需求漏项 | `09-production-defaults.md` 或业务提示词 |
| 重复造轮子 | `10-component-asset-catalog.md` 或 `0050_common-components-standard.md` |
| 测试伪通过 | `08-playwright-e2e.md` 或 `0040_e2e-testing-protocol.md` |
| 切片过大/续接丢失 | `01-task-splitting.md`、`02-parallel-execution.md`、`11-delivery-workflow.md` |
| GitHub 队列不顺畅 | `12-github-automation-workflow.md` |
| 通用反模式 | `03-anti-patterns.md` |

---

## 十、跨文件索引

| 需求 | 参考文件 |
| ------ | --------- |
| 任务切成多小才合适 | `03-frontend/01-task-splitting.md` |
| 如何并行与续接 | `03-frontend/02-parallel-execution.md` |
| 历史坑有哪些 | `03-frontend/03-anti-patterns.md` |
| DevExtreme 能力怎么写细 | `03-frontend/04-devextreme-templates.md` |
| 生产默认能力有哪些 | `03-frontend/09-production-defaults.md` |
| 哪些能力必须组件化复用 | `03-frontend/10-component-asset-catalog.md` |
| 每轮如何输出任务快照和缓存 | `03-frontend/11-delivery-workflow.md` |
| GitHub Issue 如何顺序执行 | `03-frontend/12-github-automation-workflow.md` |
| 缺陷如何反哺提示词 | `03-frontend/13-prompt-evolution-workflow.md` |

---

## 版本

- 版本：2.0
- 更新日期：2026-04-15
- 更新重点：将前端提示词体系从“规则集合”升级为“生产级执行系统”，新增默认能力补全、组件资产目录、交付快照、GitHub 顺序执行和缺陷沉淀闭环
