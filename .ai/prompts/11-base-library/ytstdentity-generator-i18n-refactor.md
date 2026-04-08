# YTStdEntity.Generator i18n 重构

## 目标

对 YTStdEntity.Generator 项目（`src/YTStdEntity.Generator/`）进行 i18n 合规性重构，确保 Generator 生成的代码符合后端零文本原则：生成的代码不包含硬编码的用户可见中文文本，枚举和常量以整形 Code 形式存在，`ColumnAttribute.Title` 仅在编译期使用不进入运行时 API 响应。

---

## 适用范围

- `src/YTStdEntity.Generator/` 目录下的所有源代码文件
- Generator 生成的 `.g.cs` 代码的 i18n 合规性

---

## 前置阅读

- `.ai/rules/i18n.md` — 完整国际化规范（必须通读）
- `.ai/prompts/05-i18n/backend-i18n.md` — 后端国际化提示词
- `.ai/rules/global.md` — 全局开发规范
- `.ai/rules/generator.md` — 源代码生成器规范
- `.ai/context/existing-modules.md` — 模块 API 参考
- `.ai/prompts/11-base-library/ytstdentity-i18n-refactor.md` — YTStdEntity 本体 i18n 重构（前置任务）

---

## 输入

- `src/YTStdEntity.Generator/` 目录下的所有源代码文件：
  - `EntityGenerator.cs` — Generator 入口与解析逻辑
  - `Models/EntityModel.cs` — 实体模型
  - `Models/ColumnModel.cs` — 列模型（含 `Title` 属性）
  - `Models/IndexModel.cs` — 索引模型
  - `Models/DetailRelation.cs` — 主从关系模型
  - `Emitters/DalEmitter.cs` — DAL 代码生成器
  - `Emitters/CrudEmitter.cs` — CRUD 代码生成器
  - `Emitters/DescEmitter.cs` — 描述类代码生成器
  - `Emitters/AuditCrudEmitter.cs` — 审计 CRUD 代码生成器
- YTStdEntity 本体 i18n 重构的评估结论（`ColumnAttribute.Title` 的处理决策）

---

## 输出

- 重构后的 Generator 源代码文件
- 确认生成的 `.g.cs` 代码不包含用户可见的硬编码中文文本
- `dotnet build YTStd.slnx` 通过
- `dotnet test YTStd.slnx` 通过

---

## 执行步骤

### 1. 审查 ColumnModel.Title 在生成代码中的使用

`ColumnModel.Title` 从 `ColumnAttribute.Title` 解析而来。逐一检查以下 Emitter 是否将 `Title` 写入生成代码：

- **DescEmitter.cs**：检查是否在 `{Entity}Desc.g.cs` 中生成了 `Title` 相关常量或元数据
- **CrudEmitter.cs**：检查是否在 `{Entity}CRUD.g.cs` 中使用了 `Title`
- **DalEmitter.cs**：检查是否在 `{Entity}DAL.g.cs` 中使用了 `Title`
- **AuditCrudEmitter.cs**：检查是否在 `{Entity}AuditCRUD.g.cs` 中使用了 `Title`

**处理原则**：
- 如果 `Title` 仅写入元数据描述类（编译期查询用），保留不变
- 如果 `Title` 被写入运行时 API 响应字段，必须移除或改为整形 Code 映射

### 2. 审查 DescEmitter 生成的 YTFieldMeta

检查 `DescEmitter.cs` 生成的 `DictFieldMetas` 字典：

```csharp
// 当前生成的代码：
new YTFieldMeta
{
    Name = "column_name",
    Type = "varchar(50)",
    Length = 50,
    Precision = 2,
    IsNullable = false,
    IsPrimaryKey = false,
    IsTenant = false,
}
```

确认 `YTFieldMeta` 中**没有**包含 `Title` 或其他用户可见文本字段。如果 `YTFieldMeta` 需要支持 `Title`，该 `Title` 必须是编译期元数据，不能包含中文文本（或改为整形 Code）。

### 3. 审查 CrudEmitter 生成的错误处理

检查 `CrudEmitter.cs` 生成的 CRUD 方法中是否有：

- 硬编码的中文错误消息
- 字符串形式的错误提示
- `throw new Exception("中文消息")` 模式

如果存在，必须替换为整形 ErrorCode 或移除中文文本。

### 4. 审查 AuditCrudEmitter 生成的代码

检查 `AuditCrudEmitter.cs` 生成的审计查询代码中是否有用户可见的中文文本。

### 5. 审查 DalEmitter 生成的代码

检查 `DalEmitter.cs` 生成的 DAL 层代码中是否有用户可见的中文文本。

### 6. 审查 EntityGenerator.cs 自身

检查 Generator 入口代码中是否有：

- 诊断消息（`DiagnosticDescriptor`）中的中文文本 — 诊断消息面向开发者，保留中文
- 编译期错误提示 — 面向开发者，保留中文

### 7. 验证 Generator 不使用 System.Linq

Generator 项目目标框架为 `netstandard2.0`。检查 `EntityGenerator.cs` 中是否使用了 `System.Linq`：

- 当前代码中存在 `using System.Linq;` 和 `.FirstOrDefault()`、`.Select()`、`.Where()` 等调用
- 根据全局规范，禁止使用 `System.Linq`
- **注意**：Source Generator 运行在编译器进程中（非 NativeAOT），`System.Linq` 限制的严格程度需要评估
- 如果项目负责人确认 Generator 中允许使用 `System.Linq`（因 Generator 不参与 NativeAOT 发布），则保留不变
- 如果禁止，必须用手动循环替换所有 LINQ 调用

### 8. 执行编译验证

```bash
dotnet build YTStd.slnx
```

验证所有使用了 YTStdEntity.Generator 的项目（如 YTStdTenantPlatform）编译通过，确认生成的代码无变化或变化符合预期。

### 9. 执行测试验证

```bash
dotnet test YTStd.slnx
```

---

## 约束

- 不破坏已有的 public API 签名（Generator 生成的代码签名不变）
- 不删除已有测试
- Generator 诊断消息（Diagnostic）保留中文（面向开发者）
- Generator 内部注释保留中文
- 修改后必须通过 `dotnet build` 和 `dotnet test`
- Generator 生成的代码变化不能导致使用方（如 YTStdTenantPlatform）编译失败

---

## 禁止事项

- 禁止修改 Generator 生成代码的 public API 签名
- 禁止在生成的代码中引入用户可见的硬编码中文文本
- 禁止引入新的 NuGet 依赖
- 禁止修改 Generator 的 TargetFramework（必须保持 `netstandard2.0`）
- 禁止修改不相关的文件
- 禁止删除 `ColumnModel.Title` 属性（即使当前未在生成代码中使用，未来可能需要）

---

## 验收标准

- [ ] Generator 生成的 `.g.cs` 代码不包含硬编码的用户可见中文文本
- [ ] `ColumnModel.Title` 的使用方式已评估并记录
- [ ] `YTFieldMeta` 中没有用户可见的中文文本字段
- [ ] Generator 自身代码中的诊断消息已审查
- [ ] `System.Linq` 的使用已评估并记录决策
- [ ] `dotnet build YTStd.slnx` 通过
- [ ] `dotnet test YTStd.slnx` 通过
- [ ] 无新增编译警告
- [ ] 使用 Generator 的下游项目（YTStdTenantPlatform）编译通过

---

## 续接说明

本任务完成后，记录以下决策供后续任务参考：

1. `ColumnAttribute.Title` / `ColumnModel.Title` 的最终处理决策
2. Generator 中 `System.Linq` 的使用决策
3. 生成代码中是否有任何 i18n 相关变更

---

## 版本

- 版本：1.0
- 创建日期：2026-04-08
