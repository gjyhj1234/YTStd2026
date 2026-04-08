# 底层框架工程代码审查与优化提示词

## 目标

对所有底层框架工程进行全面代码审查与优化，确保：

1. 逻辑正确性 — 所有实现逻辑正确无 bug
2. AOT 性能最优 — 基于 .NET 10 NativeAOT 模式，性能优先，低 GC 压力
3. 代码规范 — 完整的中文过程注释，便于阅读代码
4. 国际化支持 — 完整的 i18n 支持（通过 YTStdI18n）
5. 命名规范 — 类名、方法名、变量名符合项目命名规范
6. 注释规范 — 所有 public 成员有完整的 XML 文档注释（中文）

---

## 适用范围

以下底层工程项目（及其对应测试项目）：

- `src/YTStdSqlBuilder/` + `tests/YTStdSqlBuilder.Tests/`
- `src/YTStdSqlBuilder.Generator/` + `tests/YTStdSqlBuilder.Generator.Tests/`
- `src/YTStdLogger/` + `tests/YTStdLogger.Tests/`
- `src/YTStdAdo/` + `tests/YTStdAdo.Tests/`
- `src/YTStdEntity/` + `tests/YTStdEntity.Tests/`
- `src/YTStdEntity.Generator/`
- `src/YTStdI18n/` + `tests/YTStdI18n.Tests/`
- `src/YTStdI18n.Generator/`

本提示词适用于本项目，也可复用于任何使用这些底层工程的其他项目。

---

## 前置阅读

- `.ai/rules/global.md` — 全局开发规范（AOT 友好、性能优先、日志规则等）
- `.ai/rules/naming.md` — 命名规范
- `.ai/context/tech-stack.md` — 技术栈约束
- `.ai/context/existing-modules.md` — 模块 API 参考

---

## 输入

- 底层工程名称（可逐个或批量审查）
- 审查重点（全面审查或聚焦某一方面）

---

## 输出

- 审查报告（问题清单 + 严重性 + 修复建议）
- 修复后的代码（如任务要求执行修复）
- `dotnet build YTStd.slnx` 通过
- `dotnet test YTStd.slnx` 通过

---

## 审查维度

### 1. 正确性审查

- 逻辑是否正确（边界条件、null 处理、异常路径）
- 并发安全性（多线程场景是否有竞态条件）
- 资源释放（IDisposable、连接池、文件句柄）
- 错误处理（异常是否被正确捕获和传播）

### 2. AOT 性能审查

- 禁止反射（`System.Reflection`）
- 禁止 `dynamic`
- 禁止 `System.Linq`
- 禁止 Expression Tree
- 禁止运行时代码生成
- JSON 序列化必须使用 `JsonSerializerContext` 源生成
- 优先使用 `Span<T>` / `ReadOnlySpan<T>` 进行字符串和内存操作
- 优先使用 `ArrayPool<T>` 避免频繁分配
- 优先使用值类型（`struct`）减少 GC 压力
- 避免在热路径上产生堆分配
- 字符串操作优先使用 `StringBuilder` 或 `Span<char>`
- 数组操作优先使用 `Array.Copy` 或 `Span<T>.CopyTo`

### 3. 代码规范审查

- 所有 `public` 类型必须有 `<summary>` XML 文档注释（中文）
- 所有 `public` 方法必须有 `<summary>` 和 `<param>` 注释（中文）
- 所有 `public` 属性必须有 `<summary>` 注释（中文）
- 关键逻辑步骤必须有中文行内注释
- 注释必须描述"为什么"而不仅是"做什么"

### 4. 国际化审查

- 用户可见的错误消息是否使用 i18n key
- 日志消息不需要国际化（确认没有误加 i18n）
- I18n.T() 调用是否正确

### 5. 命名审查

- 类名 PascalCase
- 方法名 PascalCase
- 私有字段 _camelCase
- 局部变量 camelCase
- 常量 PascalCase
- 无拼音命名
- 无含义模糊的缩写

### 6. 测试覆盖审查

- 核心功能是否有测试
- 边界条件是否有测试
- 异常路径是否有测试
- 测试是否使用 xUnit
- 测试是否可重复运行

---

## 执行步骤

### 逐工程审查流程

1. 读取目标工程的所有源代码文件
2. 按上述 6 个维度逐一审查
3. 记录发现的问题（严重性：高/中/低）
4. 如任务要求修复，按问题优先级修复
5. 修复后执行 `dotnet build YTStd.slnx` 验证编译通过
6. 执行 `dotnet test YTStd.slnx` 验证测试通过
7. 输出审查报告

### 推荐审查顺序

按依赖关系从底层到上层：

1. YTStdLogger（无依赖）
2. YTStdSqlBuilder + YTStdSqlBuilder.Generator
3. YTStdAdo（依赖 YTStdSqlBuilder、YTStdLogger）
4. YTStdEntity + YTStdEntity.Generator（依赖 YTStdAdo）
5. YTStdI18n + YTStdI18n.Generator（依赖 YTStdLogger）

---

## 性能优化指导原则

由于是 AI 编码，允许并鼓励增加代码实现复杂度以换取更高性能：

- 手动展开循环（如果能减少分支预测失败）
- 使用 `stackalloc` 代替小数组堆分配
- 使用 `ref struct` 避免装箱
- 使用 `[MethodImpl(MethodImplOptions.AggressiveInlining)]` 标注热路径方法
- 使用对象池复用大对象
- 预分配集合容量
- 使用位运算替代数学运算（如适用）

但是，所有性能优化代码**必须**有完整的中文注释说明优化意图。

---

## 约束

- 审查和修复不得破坏已有的 public API 签名（除非明确要求 breaking change）
- 审查和修复不得删除已有测试
- 修复后必须通过 `dotnet build` 和 `dotnet test`
- 所有修改必须有完整的中文过程注释

---

## 禁止事项

- 禁止引入新的外部依赖
- 禁止修改项目的 TargetFramework
- 禁止使用反射、dynamic、LINQ
- 禁止在修复中引入安全漏洞
- 禁止移除日志调用
- 禁止修改 Logger.Debug 的委托重载约定

---

## 验收标准

- [ ] 所有 public 成员有完整的中文 XML 文档注释
- [ ] 关键逻辑有中文行内注释
- [ ] 无 AOT 不兼容代码（无反射、无 dynamic、无 LINQ）
- [ ] 热路径无不必要的堆分配
- [ ] 所有用户可见错误消息使用 i18n key
- [ ] 命名符合项目命名规范
- [ ] `dotnet build YTStd.slnx` 通过
- [ ] `dotnet test YTStd.slnx` 通过
- [ ] 无新增编译警告

---

## 审查完成后

审查并修复完成的工程进入**受保护状态**（参见本目录 `README.md` 的保护规则）。后续修改必须通过独立的调整提示词进行。
