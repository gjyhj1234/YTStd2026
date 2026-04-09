## 会话总结

### 会话信息
- **日期**：2026-04-09
- **轮次**：第 7 轮（提示词优化专项 — 非编码任务）
- **任务编号**：TASK-PLATFORM-001
- **任务标题**：租户平台检查与重构 — 提示词优化

### 当前所处阶段
- 阶段 D 后端测试已完成
- 本轮为**提示词优化专项**，不涉及编码修改
- 阶段 E 前端重构尚未开始

### 本轮目标
- 诊断 B 阶段 InsertAsync 缺少 GetNextLongIdAsync 的根因
- 诊断 Postman 集合与实际 webapi 路由不一致的根因
- 优化提示词体系，防止此类问题再次发生
- 记录发现的编码问题，供下轮编码任务修复

### 本轮已完成

| 序号 | 操作 | 文件路径 | 说明 |
|-----|------|---------|------|
| 1 | 修改 | `.github/copilot-instructions.md` | 内联关键编码约束（InsertAsync、ApiResult.Fail、Logger.Debug、Postman一致性），添加强制代码审查说明 |
| 2 | 新建 | `.ai/system/self-review-protocol.md` | 定义 7 项强制审查项，包含 grep 搜索命令、验证规则、执行时机、输出格式 |
| 3 | 修改 | `.ai/system/execution-policy.md` | 添加步骤 4.5（代码搜索审查），明确验收闭环 |
| 4 | 修改 | `.ai/system/done-criteria.md` | 通用标准+应用服务标准+Postman标准增加代码搜索验证 |
| 5 | 修改 | `.ai/system/review-policy.md` | Agent 自检项改为 grep 搜索驱动 |
| 6 | 修改 | `.ai/system/agent-contract.md` | 执行后输出增加"代码搜索审查结果" |
| 7 | 修改 | `.ai/system/prompt-writing-guide.md` | 新增"关键约束内联化"和"验收标准可执行化"原则 |
| 8 | 修改 | `.ai/prompts/10-review/code-review.md` | 全面改为 grep 命令驱动的审查流程 |
| 9 | 修改 | `.ai/prompts/02-backend/app-service.md` | InsertAsync 零容忍规则（含关联表），验收增加 grep 验证 |
| 10 | 修改 | `.ai/prompts/07-testing/postman-collection.md` | 增加路由一致性验证步骤和约束 |
| 11 | 修改 | `.ai/README.md` | 目录结构增加 self-review-protocol.md |
| 12 | 修改 | `.ai/tasks/task-platfrom.md` | 记录发现的 3 个问题及详细分析 |
| 13 | 新建 | `.ai/workspace/session-summary-20260409-06.md` | 本轮会话总结 |

### 根因分析

#### InsertAsync 违规根因

1. **提示词引用深度问题**：`copilot-instructions.md` 仅指向 `.ai/rules/global.md`，Agent 可能未完整阅读或在长任务执行中遗忘
2. **验收标准过于简单**：仅以 `dotnet build` 编译通过作为验收，而 InsertAsync 缺少 GetNextLongIdAsync 不会导致编译错误
3. **缺乏自动化审查**：没有要求 Agent 使用 `grep` 搜索验证关键约束的遵守情况
4. **B 阶段 vs C 阶段差异**：C 阶段的提示词中内联了更强的约束提示，因此 C 阶段未复现此问题

#### Postman 路由不一致根因

1. **Postman 生成未从代码提取路由**：Agent 可能从规格文档或记忆生成 Postman 请求，未从实际 Endpoints 代码提取
2. **缺乏路由一致性验证步骤**：原有的 Postman 验收标准不包含与代码的路由比对

### 提示词优化方案总结

| 优化维度 | 具体措施 | 预期效果 |
|---------|---------|---------|
| **关键约束内联** | 在 `copilot-instructions.md` 中直接展示关键规则的正确/错误代码示例 | Agent 在任务开始时即看到关键约束，降低遗忘概率 |
| **自动化代码审查** | 新建 `self-review-protocol.md`，定义 7 项 grep 搜索审查 | Agent 在编码完成后执行搜索验证，不依赖人工审查 |
| **验收标准升级** | 从"编译通过"升级为"编译 + 搜索审查 + 审查结果记录" | 编译不再是唯一验收标准 |
| **执行闭环强化** | 在 `execution-policy.md` 中插入步骤 4.5（代码搜索审查） | 审查成为执行流程的强制步骤，不可跳过 |
| **Postman 路由验证** | 在 Postman 提示词中增加路由一致性验证步骤 | Postman 集合必须从代码提取路由，并逐一比对 |
| **约束内联化原则** | 在 `prompt-writing-guide.md` 中新增编写原则 | 未来新提示词也会遵循内联关键约束的模式 |

### 发现的待修复编码问题（需下轮编码任务处理）

1. **B 阶段 InsertAsync 违规**：7 个 AppService 文件中共 15 处 InsertAsync 缺少 GetNextLongIdAsync
   - PlatformUserAppService.cs (1处)
   - PlatformRoleAppService.cs (3处)
   - PlatformPermissionAppService.cs (1处)
   - TenantLifecycleAppService.cs (2处)
   - TenantInfoAppService.cs (4处，仅第268行正确)
   - TenantConfigAppService.cs (3处)
   - TenantResourceAppService.cs (1处)

2. **Postman 路由不匹配**：
   - `POST /api/auth/refresh` 应为 `POST /api/auth/refresh-token`
   - 缺少 `POST /api/auth/change-password`

### 未完成内容（下阶段）

#### 优先：修复发现的编码问题
- 修复 B 阶段 15 处 InsertAsync 违规
- 修复 Postman 路由不一致
- 修复后执行 `.ai/system/self-review-protocol.md` 全部审查项验证

#### 后续：阶段 E 前端重构
- E24：脚手架与项目结构
- E25~E41：各模块页面

#### 后续：阶段 F 前端国际化

### 风险与待确认
1. **提示词修改权限**：`agent-contract.md` 中定义 `.ai/system/` 目录为 Agent 只读，但本轮任务由人类明确授权修改提示词，属于特殊情况
2. **Token 消耗增加**：新增的代码搜索审查步骤会增加每次编码任务的 Token 消耗，但保证了代码质量
3. **B 阶段修复范围**：15 处 InsertAsync 修复需要触及 7 个核心服务文件，修复后需重新编译和测试

### 下一轮应继续
1. **优先修复编码问题**：按本轮记录的违规清单，逐一修复 B 阶段 InsertAsync 和 Postman 路由
2. 修复完成后执行 `self-review-protocol.md` 全部审查项
3. 然后进入阶段 E 前端重构

### 下一轮必须保持一致的规则
- 错误码体系：10xxx/11xxx/12xxx/18xxx/19xxx/50xxx
- 表名前缀：`sys_` + 单数
- API 路由：Phase C 已统一的新路由
- ApiResult.Fail() 仅传 code（int 类型）
- **DB.GetNextLongIdAsync() 必须在每次 InsertAsync 之前调用（零容忍）**
- Logger.Debug 使用 Func<string> 委托
- 测试框架：xUnit，CRUD 结果用 .Success 断言
- **编码任务完成后必须执行 `.ai/system/self-review-protocol.md` 全部审查项**

### 下一轮建议阅读的文件
- `.ai/workspace/session-summary-20260409-06.md`（本轮总结）
- `.ai/system/self-review-protocol.md`（新建的自动化审查协议）
- `.ai/tasks/task-platfrom.md`（更新后的任务文件，含待修复问题清单）
- `.ai/rules/global.md`（全局规范，InsertAsync 规则）
- `.ai/rules/backend.md`（后端规范）
