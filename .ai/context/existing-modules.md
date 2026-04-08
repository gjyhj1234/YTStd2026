# 已有模块参考

## 概述

本文件说明仓库中已完成的基础框架模块及其核心 API，新模块开发时必须引用这些已有能力。

详细 API 签名请查阅 `docs/existing-projects-reference.md`。

---

## 模块状态

| 模块 | 路径 | 状态 | 说明 |
|------|------|------|------|
| YTStdSqlBuilder | `src/YTStdSqlBuilder/` | ✓ 完成 | SQL 构建器运行时 |
| YTStdSqlBuilder.Generator | `src/YTStdSqlBuilder.Generator/` | ✓ 完成 | SQL Source Generator |
| YTStdLogger | `src/YTStdLogger/` | ✓ 完成 | 高性能日志系统 |
| YTStdAdo | `src/YTStdAdo/` | ✓ 完成 | 数据库访问层 |
| YTStdEntity | `src/YTStdEntity/` | ✓ 完成 | 实体框架 |
| YTStdEntity.Generator | `src/YTStdEntity.Generator/` | ✓ 完成 | 实体 Source Generator |
| YTStdI18n | `src/YTStdI18n/` | ✓ 完成 | 国际化 |
| YTStdI18n.Generator | `src/YTStdI18n.Generator/` | ✓ 完成 | 国际化 Source Generator |
| YTStdTenantPlatform | `src/YTStdTenantPlatform/` | ✓ 完成 | 租户平台主程序 |

---

## 核心 API 速查

### YTStdLogger

```csharp
// 初始化
Logger.Init(LogOptions options);

// 日志记录（必须使用 Func<string> 重载用于 Debug）
Logger.Debug(int tenantId, long userId, Func<string> messageFactory);
Logger.Info(int tenantId, long userId, string message);
Logger.Warn(int tenantId, long userId, string message);
Logger.Error(int tenantId, long userId, string message);
Logger.Fatal(int tenantId, long userId, string message);

// 租户级 Debug 开关
Logger.EnableTenantDebug(int tenantId);
Logger.DisableTenantDebug(int tenantId);
```

### YTStdAdo (DB)

```csharp
// ID 生成（必须在 InsertAsync 前调用）
await DB.GetNextLongIdAsync();

// CRUD 操作（由 Source Generator 生成）
await DB.InsertAsync(entity);
await DB.UpdateAsync(entity);
await DB.DeleteAsync<T>(id);
await DB.GetAsync<T>(id);
await DB.GetListAsync<T>(query);
```

### YTStdI18n

```csharp
// 翻译（使用 int 常量索引，零分配）
I18n.T(int tenantId, int key);
```

### YTStdEntity

```csharp
// 实体特性
[Entity(TableName = "sys_user", Description = "平台用户")]
[Column(Name = "username", DbType = "varchar(50)", Description = "用户名")]
[Index(Name = "idx_sys_user_username", Columns = "username", IsUnique = true)]
[DetailOf(typeof(ParentEntity))]  // 主从关系
```

### YTStdSqlBuilder

```csharp
// SQL 构建
var query = PgSql.Select(...).From(...).Where(...).Build();
var insert = PgSql.InsertInto(...).Values(...).Build();
var update = PgSql.Update(...).Set(...).Where(...).Build();
var delete = PgSql.DeleteFrom(...).Where(...).Build();
```

---

## 模块依赖关系

```
YTStdLogger  ←── YTStdSqlBuilder.Generator
    ↑                    ↑
    │                    │
YTStdAdo ────→ YTStdSqlBuilder
    ↑
    │
YTStdEntity ────→ YTStdAdo
    ↑               ↑
    │               │
YTStdI18n ────→ YTStdLogger

YTStdTenantPlatform ──→ 以上全部
```

---

## 已有设计模式

### ApiResult 格式

```csharp
// 统一响应格式
public class ApiResult<T>
{
    public int Code { get; set; }      // 0=成功，非零=ErrorCodes 常量
    public int Message { get; set; }   // Messages.XXX 整形常量（前端根据此 Code 查找翻译）
    public T Data { get; set; }
}
```

### ErrorCodes 体系

- `0`：成功
- `10xxx`：认证相关
- `11xxx`：权限相关
- `12xxx`：输入验证相关
- `18xxx`：唯一性冲突相关
- `19xxx`：业务规则相关
- `50xxx`：系统错误

### JSON 序列化

- 使用 `JsonSerializerContext` 源生成（AOT 友好）
- PascalCase 属性名（`Code`, `Message`, `Data`, `Items`, `Total`）
- 无 `PropertyNamingPolicy` 配置

### CRUD 结果类型

- `DbInsResult`：使用 `.Success` (bool) 属性判断成功
- `DbUdqResult`：使用 `.Success` (bool) 属性判断成功
- 注意：只有 `ApiResult` 有 `.Code` 属性
