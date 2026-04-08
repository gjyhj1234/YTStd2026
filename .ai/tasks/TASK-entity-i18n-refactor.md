# 任务：YTStdEntity 与 YTStdEntity.Generator i18n 重构

## 任务信息

- **任务编号**：TASK-ENTITY-I18N-001
- **所属阶段**：底层框架 i18n 合规性重构
- **优先级**：高
- **预估时间**：40 分钟（子任务 A + B + C）
- **前置任务**：无

## 任务目标

对 YTStdEntity 和 YTStdEntity.Generator 两个底层框架工程进行 i18n 合规性审查与重构，确保代码符合 `.ai/rules/i18n.md` 定义的后端零文本原则。本任务是底层框架 i18n 合规的必要前置步骤，完成后两个工程将进入受保护状态。

## 前置阅读

执行本任务前，Agent 必须阅读以下文件：

- `.ai/system/agent-contract.md` — Agent 协作契约
- `.ai/rules/global.md` — 全局开发规范
- `.ai/rules/i18n.md` — 完整国际化规范
- `.ai/prompts/05-i18n/backend-i18n.md` — 后端国际化提示词
- `.ai/rules/generator.md` — 源代码生成器规范
- `.ai/context/existing-modules.md` — 模块 API 参考
- `.ai/prompts/11-base-library/ytstdentity-i18n-refactor.md` — YTStdEntity 调整提示词
- `.ai/prompts/11-base-library/ytstdentity-generator-i18n-refactor.md` — YTStdEntity.Generator 调整提示词

## 输入

- `src/YTStdEntity/` — YTStdEntity 完整源代码
- `src/YTStdEntity.Generator/` — YTStdEntity.Generator 完整源代码
- `tests/YTStdEntity.Tests/` — 已有测试

## 预期输出

- 重构后的 `src/YTStdEntity/` 源代码（如需修改）
- 重构后的 `src/YTStdEntity.Generator/` 源代码（如需修改）
- i18n 合规性评估报告（包含所有评估决策）
- `dotnet build YTStd.slnx` 通过
- `dotnet test YTStd.slnx` 通过

## 执行步骤

本任务按三个子任务顺序执行：

### 子任务 A：YTStdEntity 本体 i18n 审查与重构（15 分钟）

按 `.ai/prompts/11-base-library/ytstdentity-i18n-refactor.md` 执行：

1. 扫描 `Audit/AuditOpt.cs` 和 `Attributes/IndexAttribute.cs` 中的枚举定义，确认无 `[Description]` 文本属性
2. 逐文件扫描所有字符串字面量，区分日志消息（保留）、注释（保留）、异常消息（保留）、SQL（保留）、API 错误消息（必须为整形 Code）
3. 确认 `Backup/IncrementalBackupService.cs` 中的 Logger 中文文本保留（日志不国际化）
4. 确认 `Tenant/TenantSeparationService.cs` 中的 Logger 中文文本保留（日志不国际化）
5. 评估 `ColumnAttribute.Title` 属性的使用方式，记录评估结论
6. 确认 `DbNullable.ToString()` 中的调试文本保留
7. 执行 `dotnet build YTStd.slnx` 验证编译通过

### 子任务 B：YTStdEntity.Generator i18n 审查与重构（15 分钟）

按 `.ai/prompts/11-base-library/ytstdentity-generator-i18n-refactor.md` 执行：

1. 审查 `DescEmitter.cs`，确认生成的 `YTFieldMeta` 不包含用户可见中文文本
2. 审查 `CrudEmitter.cs`，确认生成的 CRUD 方法不包含硬编码中文错误消息
3. 审查 `AuditCrudEmitter.cs`，确认生成的审计代码不包含中文文本
4. 审查 `DalEmitter.cs`，确认生成的 DAL 代码不包含中文文本
5. 审查 `ColumnModel.Title` 在所有 Emitter 中的使用方式，记录评估结论
6. 审查 `EntityGenerator.cs` 中的 `System.Linq` 使用情况，评估是否需要移除
7. 执行 `dotnet build YTStd.slnx` 验证编译通过

### 子任务 C：综合验证（10 分钟）

1. 执行 `dotnet build YTStd.slnx` 确认全量编译通过
2. 执行 `dotnet test YTStd.slnx` 确认所有测试通过
3. 确认使用 Generator 的下游项目（YTStdTenantPlatform）编译通过
4. 汇总所有评估决策和修改记录
5. 输出会话总结

## 约束

- 不破坏已有的 public API 签名
- 不删除已有测试
- 不修改不相关的文件
- 日志消息保留中文（日志不需要国际化）
- XML 注释保留中文（注释不需要国际化）
- 调试用 `ToString()` 文本保留
- 异常消息保留（面向开发者）
- Generator 诊断消息保留中文（面向开发者）
- 修改后必须通过 `dotnet build` 和 `dotnet test`

## 禁止事项

- 禁止修改 Logger 调用中的中文日志文本
- 禁止移除 XML 文档注释中的中文
- 禁止引入新的外部依赖
- 禁止修改项目的 TargetFramework
- 禁止使用反射、dynamic
- 禁止修改 `ValueStringBuilder` 的内部实现
- 禁止修改 Generator 生成代码的 public API 签名
- 禁止删除 `ColumnModel.Title` 属性

## 验收标准

- [ ] 所有枚举值无 `[Description]` 或其他文本属性
- [ ] 没有用户可见的硬编码中文文本通过 API 返回（日志和注释除外）
- [ ] `ColumnAttribute.Title` / `ColumnModel.Title` 的使用方式已评估并记录
- [ ] Generator 生成的 `.g.cs` 代码不包含用户可见的硬编码中文文本
- [ ] `System.Linq` 的使用已评估并记录决策
- [ ] `dotnet build YTStd.slnx` 通过
- [ ] `dotnet test YTStd.slnx` 通过
- [ ] 无新增编译警告
- [ ] public API 签名未变
- [ ] 使用 Generator 的下游项目编译通过
- [ ] 公开类型和方法有中文 XML 注释

## 会话结束时

完成本任务后，请按 `.ai/templates/session-summary-template.md` 输出会话总结。

总结中必须包含以下决策记录：

1. `ColumnAttribute.Title` / `ColumnModel.Title` — 评估结论与处理决策
2. Generator 中 `System.Linq` — 是否允许在 Generator（netstandard2.0）中使用的决策
3. 实际修改的文件清单（如有修改）
4. 发现但未修改的问题清单（如有）
