# 实体建模提示词

## 目标

基于数据字典创建实体类、枚举和值对象，触发 Source Generator 生成 DAL/CRUD 代码。

---

## 适用范围

新建或修改实体时使用。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/database.md`
- `.ai/rules/naming.md`
- `.ai/rules/generator.md`
- `docs/TenantPlatform/database_dictionary.md`
- `docs/existing-projects-reference.md`

---

## 输入

- 数据字典中的表定义
- 表之间的关系说明

---

## 输出

- `entity/{Module}/{Entity}.cs` — 实体类
- `Domain/Enums/{Enum}.cs` — 枚举类
- `Domain/ValueObjects/{VO}.cs` — 值对象（如需要）

---

## 执行步骤

1. 从数据字典提取表定义
2. 确定实体名（PascalCase，对应表名）
3. 确定每个列的 C# 属性名和类型
4. 标注 `[Entity]`、`[Column]`、`[Index]` 特性
5. 创建状态和类型的枚举定义
6. 处理主从关系（`[DetailOf]`）
7. 添加审计字段（如需要）
8. 编写完整 XML 注释
9. 执行 `dotnet build` 触发 Source Generator
10. 修正编译错误

---

## 实体定义示例

```csharp
using YTStdEntity;

namespace YTStdTenantPlatform.Entity.TenantPlatform;

/// <summary>
/// 平台用户实体
/// </summary>
[Entity(TableName = "sys_user", Description = "平台管理员用户表")]
[Index(Name = "uq_sys_user_username", Columns = "username", IsUnique = true)]
[Index(Name = "uq_sys_user_email", Columns = "email", IsUnique = true)]
public class PlatformUser
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [Column(Name = "id", DbType = "bigint", IsPrimaryKey = true, Description = "主键")]
    public long Id { get; set; }

    /// <summary>
    /// 用户名（登录名）
    /// </summary>
    [Column(Name = "username", DbType = "varchar(50)", Description = "用户名")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    [Column(Name = "display_name", DbType = "varchar(100)", Description = "显示名称")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    [Column(Name = "is_enabled", DbType = "boolean", Description = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Name = "created_at", DbType = "timestamp with time zone", Description = "创建时间")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column(Name = "updated_at", DbType = "timestamp with time zone", Description = "更新时间")]
    public DateTimeOffset UpdatedAt { get; set; }
}
```

---

## 约束

- 实体命名空间：`{Project}.Entity.{Module}`
- 实体文件位置：`entity/{Module}/*.cs`
- 主键统一为 `long Id`
- 所有属性有 `[Column]` 特性
- 所有唯一约束有 `[Index(IsUnique = true)]`

---

## 禁止事项

- 禁止使用裸 `TenantId` / `tenant_id`
- 禁止省略 XML 注释
- 禁止手写 `.g.cs` 文件
- 禁止跳过编译验证步骤

---

## 验收标准

- [ ] 所有实体文件位于正确目录
- [ ] 所有特性标注完整
- [ ] 枚举值与数据字典一致
- [ ] XML 注释完整
- [ ] `dotnet build` 通过且 Source Generator 成功
- [ ] 无裸 `TenantId` 字段
