# YTStdEntity i18n 重构

## 目标

对 YTStdEntity 项目（`src/YTStdEntity/`）进行 i18n 合规性重构，确保项目内所有代码符合后端零文本原则，枚举无 `[Description]` 文本属性，日志中文文本保留（日志不需要国际化），XML 注释中文保留（注释不需要国际化）。

---

## 适用范围

- `src/YTStdEntity/` 目录下的所有源代码文件
- 不包含 `src/YTStdEntity.Generator/`（Generator 有独立的调整提示词）

---

## 前置阅读

- `.ai/rules/i18n.md` — 完整国际化规范（必须通读）
- `.ai/prompts/05-i18n/backend-i18n.md` — 后端国际化提示词
- `.ai/rules/global.md` — 全局开发规范
- `.ai/rules/generator.md` — 源代码生成器规范
- `.ai/context/existing-modules.md` — 模块 API 参考

---

## 输入

- `src/YTStdEntity/` 目录下的所有源代码文件：
  - `Attributes/EntityAttribute.cs`
  - `Attributes/ColumnAttribute.cs`
  - `Attributes/IndexAttribute.cs`
  - `Attributes/DetailOfAttribute.cs`
  - `Audit/AuditOpt.cs`
  - `Audit/AuditRecord.cs`
  - `Audit/AuditDiffField.cs`
  - `Audit/AuditQueryFilter.cs`
  - `Audit/MasterDetailAuditResult.cs`
  - `Backup/IncrementalBackupOptions.cs`
  - `Backup/IncrementalBackupService.cs`
  - `Tenant/TenantSeparationOptions.cs`
  - `Tenant/TenantSeparationService.cs`
  - `DBNULL.cs`
  - `DbNullable.cs`
  - `YTFieldMeta.cs`
  - `ValueStringBuilder.cs`

---

## 输出

- 重构后的源代码文件（仅修改不合规的部分）
- `dotnet build YTStd.slnx` 通过
- `dotnet test YTStd.slnx` 通过

---

## 执行步骤

### 1. 扫描枚举定义

检查以下枚举，确保符合 i18n 规范：

- `Audit/AuditOpt.cs` — `AuditOpt` 枚举
- `Attributes/IndexAttribute.cs` — `IndexKind` 枚举

**检查项**：
- 枚举值必须为整形，不附带 `[Description]` 或其他文本属性
- 枚举的 XML 注释中文保留（注释不需要国际化）

### 2. 扫描用户可见的文本字符串

逐文件检查所有字符串字面量，区分以下类别：

| 类别 | 处理方式 |
|------|---------|
| Logger 日志消息（`Logger.Debug`、`Logger.Info` 等） | 保留中文，日志不需要国际化 |
| XML 注释 `<summary>`、`<param>` | 保留中文，注释不需要国际化 |
| 异常消息（`throw new ArgumentNullException` 等） | 保留，异常消息面向开发者 |
| SQL 字符串 | 保留，技术标识 |
| API 错误消息 | 必须使用 `ErrorCodes.XXX` 整形常量 |

### 3. 检查 Backup/IncrementalBackupService.cs

该文件包含大量 Logger 调用中的中文文本，例如：
- `"[IncrementalBackupService] 增量备份服务已启动，间隔="`
- `"[IncrementalBackupService.SyncOnceAsync] 开始同步表 "`

**处理方式**：保留不变。日志消息不需要国际化。

### 4. 检查 Tenant/TenantSeparationService.cs

该文件包含大量 Logger 调用中的中文文本，例如：
- `"[TenantSeparationService] 开始租户分离, 租户="`
- `"[TenantSeparationService] 迁移完成，共 "`

**处理方式**：保留不变。日志消息不需要国际化。

### 5. 验证 ColumnAttribute.Title 属性

`ColumnAttribute.cs` 中的 `Title` 属性用于显示标题。检查该属性在 Generator 中的使用方式：

- 如果 `Title` 仅在编译期由 Generator 使用（生成描述类或前端代码），保留不变
- 如果 `Title` 在运行时作为用户可见文本返回给前端，必须评估是否需要移除或改为整形 Code

### 6. 验证 DbNullable.ToString() 中的文本

`DbNullable.cs` 的 `ToString()` 方法包含：
- `"[Unset]"` — 调试用文本
- `"[Set: null]"` — 调试用文本
- `"[Set: "` — 调试用文本

**处理方式**：保留不变。`ToString()` 仅用于调试，不是用户可见消息。

### 7. 执行编译验证

```bash
dotnet build YTStd.slnx
```

### 8. 执行测试验证

```bash
dotnet test YTStd.slnx
```

---

## 约束

- 不破坏已有的 public API 签名
- 不删除已有测试
- 日志消息保留中文（日志不需要国际化）
- XML 注释保留中文（注释不需要国际化）
- 调试用 `ToString()` 文本保留
- 异常消息保留（面向开发者）
- 修改后必须通过 `dotnet build` 和 `dotnet test`

---

## 禁止事项

- 禁止修改 Logger 调用中的中文日志文本（日志不国际化）
- 禁止移除 XML 文档注释中的中文
- 禁止引入新的外部依赖
- 禁止修改项目的 TargetFramework
- 禁止使用反射、dynamic、LINQ
- 禁止修改 `ValueStringBuilder` 的内部实现
- 禁止修改不相关的文件

---

## 验收标准

- [ ] 所有枚举值无 `[Description]` 或其他文本属性
- [ ] 没有用户可见的硬编码中文文本通过 API 返回（日志和注释除外）
- [ ] `ColumnAttribute.Title` 的使用方式已评估并记录
- [ ] `dotnet build YTStd.slnx` 通过
- [ ] `dotnet test YTStd.slnx` 通过
- [ ] 无新增编译警告
- [ ] public API 签名未变

---

## 续接说明

本任务完成后，继续执行 YTStdEntity.Generator 的 i18n 重构（参见 `ytstdentity-generator-i18n-refactor.md`）。需要传递以下信息：

- `ColumnAttribute.Title` 的评估结论
- YTStdEntity 中是否有任何变更影响 Generator 的输入模型

---

## 版本

- 版本：1.0
- 创建日期：2026-04-08
