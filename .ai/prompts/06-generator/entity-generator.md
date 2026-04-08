# 实体生成器提示词

## 目标

使用 YTStdEntity.Generator 生成实体的 DAL/CRUD 代码，并验证生成结果。

---

## 适用范围

实体创建后触发 Source Generator 时使用。

---

## 前置阅读

- `.ai/rules/generator.md`
- `.ai/rules/backend.md`
- `docs/existing-projects-reference.md`

---

## 输入

- 已定义的实体类（带 `[Entity]`、`[Column]`、`[Index]` 特性）

---

## 输出

- 编译成功后由 Source Generator 自动生成的 DAL/CRUD 代码

---

## 执行步骤

1. 确认实体文件位于 `entity/{Module}/` 目录
2. 确认所有特性标注正确
3. 执行 `dotnet build YTStd.slnx`
4. 检查编译输出，确认 Source Generator 成功运行
5. 如有编译错误，分析错误信息并修正实体定义
6. 重复步骤 3-5 直到编译通过
7. 验证生成的 CRUD 方法可用

---

## 常见编译错误及解决方案

| 错误 | 原因 | 解决方案 |
|------|------|---------|
| 实体类未被识别 | 缺少 `[Entity]` 特性 | 添加 `[Entity]` 特性 |
| 列类型不匹配 | `DbType` 与 C# 类型不对应 | 修正类型映射 |
| 索引列不存在 | `[Index]` 引用的列名错误 | 修正列名 |
| 命名空间冲突 | 与生成代码命名空间冲突 | 调整命名空间 |

---

## 约束

- 不手写 `.g.cs` 文件
- 编译成功是必要条件
- 生成的代码在 obj/ 目录中，不提交到 Git

---

## 验收标准

- [ ] `dotnet build YTStd.slnx` 通过
- [ ] Source Generator 无警告
- [ ] 生成的 CRUD 方法在代码中可引用
