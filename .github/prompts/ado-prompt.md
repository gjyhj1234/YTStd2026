# 面向 Claude Opus 4.6 的 ADO 数据访问层生产级实现提示词
## PostgreSQL / Npgsql 专用，高性能、低 GC、AOT 友好的 ADO 数据访问层

> **状态**：模板 / 待完善。此文件展示了新提示词如何引用已有项目。

你是一名**顶级 .NET 基础架构工程师 / PostgreSQL & Npgsql 专家 / 高性能框架作者**。
请实现一个**仅适配 PostgreSQL / Npgsql** 的、**AOT 友好**、**高性能**、**低 GC**、**生产级可维护** 的 ADO 数据访问层。

这不是 demo，不是玩具组件，而是**框架基座级实现**。
请严格按"**生产级顶级标准**"完成完整代码、测试与设计说明。

---

## 依赖项目参考

> **重要**：本模块依赖以下已有项目。完整的类型定义、API 签名和使用模式，
> 请查阅 [已有项目参考文档](../../docs/existing-projects-reference.md)。
>
> AI 在实现本模块时，必须遵循参考文档中定义的类型签名，不得自行定义重复类型。

### 本模块使用的已有类型

| 类型 | 来源项目 | 用途 |
|------|---------|------|
| `PgSqlRenderResult` | YTStdSqlBuilder | SQL 构建结果，包含 `.Sql` 和 `.Params` |
| `PgSqlParam` | YTStdSqlBuilder | SQL 参数，包含 `.Name`、`.Value`、`.DbType` |
| `PgSqlQueryBuilder` | YTStdSqlBuilder | 查询构建器（用于子查询等场景） |
| `DBNullable<T>` | YTStd.Common | 数据库可空值包装（待添加后使用） |

### 关键交互模式

ADO 模块是 SQL Builder 的**执行层**。它接收 `PgSqlRenderResult` 并执行：

```csharp
// 1. SQL Builder 构建查询
var user = Table.Def("users").As("u");
var query = PgSql
    .Select(user["id"].As<int>("id"), user["name"])
    .From(user)
    .Where(user["age"], Op.Gte, Param.Value(18))
    .Build();

// 2. ADO 模块执行查询
await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
var users = await PgAdo.QueryAsync(conn, query, reader => new {
    Id = reader.GetInt32(0),
    Name = reader.GetString(1)
});

// 3. Source Generator 生成的查询可直接使用
var typedQuery = UserQueries.GetUserById(42);
var user = await PgAdo.QuerySingleAsync(conn, typedQuery, UserQueries.ReadGetByIdRow);
```

---

## 1. 最终目标

实现一个轻量级的 ADO 数据访问层，提供以下核心能力：

### 1.1 查询执行

- `QueryAsync<T>` — 执行查询并映射结果集
- `QuerySingleAsync<T>` — 执行查询并映射单条结果
- `QuerySingleOrDefaultAsync<T>` — 执行查询，无结果返回默认值
- `ExecuteAsync` — 执行非查询（INSERT / UPDATE / DELETE），返回受影响行数
- `ExecuteScalarAsync<T>` — 执行标量查询

### 1.2 参数绑定

- 从 `PgSqlRenderResult.Params` 自动绑定参数到 `NpgsqlCommand`
- 支持 `NpgsqlDbType` 类型提示
- 支持 `DBNull.Value` 处理

### 1.3 事务支持

- `BeginTransactionAsync` — 开始事务
- `TransactionScope` — 事务范围（嵌套支持）

---

## 2. 技术约束

- 目标框架：net10.0
- 数据库：PostgreSQL（Npgsql）
- AOT 兼容：是
- 不使用反射（AOT 友好）
- 不使用 `dynamic`
- 依赖项目：
  - `YTStd.Common` — 通用工具
  - `YTStdSqlBuilder` — SQL 构建
  - `Npgsql` — PostgreSQL 驱动

---

## 3. 项目结构

生成的代码应放置在以下位置：

| 项目 | 路径 | 说明 |
|------|------|------|
| 主项目 | `src/YTStd.Ado/` | ADO 数据访问层 |
| 测试 | `tests/YTStd.Ado.Tests/` | 单元测试 |

---

## 4. 核心设计

### 4.1 核心类型

```csharp
// 主入口
public static class PgAdo
{
    // 查询
    public static ValueTask<List<T>> QueryAsync<T>(
        NpgsqlConnection conn,
        PgSqlRenderResult query,
        Func<NpgsqlDataReader, T> mapper,
        CancellationToken ct = default);

    public static ValueTask<T> QuerySingleAsync<T>(
        NpgsqlConnection conn,
        PgSqlRenderResult query,
        Func<NpgsqlDataReader, T> mapper,
        CancellationToken ct = default);

    public static ValueTask<T?> QuerySingleOrDefaultAsync<T>(
        NpgsqlConnection conn,
        PgSqlRenderResult query,
        Func<NpgsqlDataReader, T> mapper,
        CancellationToken ct = default);

    // 非查询
    public static ValueTask<int> ExecuteAsync(
        NpgsqlConnection conn,
        PgSqlRenderResult command,
        CancellationToken ct = default);

    // 标量
    public static ValueTask<T> ExecuteScalarAsync<T>(
        NpgsqlConnection conn,
        PgSqlRenderResult query,
        CancellationToken ct = default);
}
```

### 4.2 使用示例

```csharp
// 基本查询
var query = PgSql
    .Select(user["id"].As<int>("id"), user["name"])
    .From(user)
    .Where(user["active"], Op.Eq, Param.Value(true))
    .Build();

var users = await PgAdo.QueryAsync(conn, query, r => new UserDto {
    Id = r.GetInt32(0),
    Name = r.GetString(1)
});

// 插入并返回 ID
var insertCmd = PgSql.InsertInto(user)
    .Set(user["name"], Param.Value("Alice"))
    .Set(user["age"], Param.Value(25))
    .Returning(user["id"])
    .Build();

var newId = await PgAdo.ExecuteScalarAsync<int>(conn, insertCmd);

// 配合 Source Generator 使用
var typedQuery = UserQueries.GetUserById(42);
var row = await PgAdo.QuerySingleAsync(conn, typedQuery, UserQueries.ReadGetByIdRow);
// row.Id, row.Name 等属性直接可用
```

---

## N. 最终质量标准

若设计上有取舍，请遵循以下优先级：

**正确性 > 语义完整性 > 安全性 > 可维护性 > 可读性 > 复杂度可控 > 性能 > 易用性 > 技巧炫技**

---

## N+1. 最终指令

请现在开始实现，直接交付：

- 完整源码（`src/YTStd.Ado/`）
- 完整测试（`tests/YTStd.Ado.Tests/`）
- 完整设计说明

不要只给思路。
不要偷懒省略。
不要输出 demo 级方案。
必须按**生产级顶级标准**完成。
