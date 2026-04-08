# 全局开发规范

## 目标

定义适用于所有代码（前端和后端）的全局开发规范。

---

## 适用范围

仓库中的所有源代码、测试代码和文档。

---

## 语言与注释

### 注释语言

- 所有代码注释使用中文
- 所有 XML 文档注释（`/// <summary>`）使用中文
- 所有 README 和文档使用中文
- 变量名、方法名、类名使用英文

### XML 注释要求

- 所有 `public` 类型必须有 `<summary>` 注释
- 所有 `public` 方法必须有 `<summary>` 和 `<param>` 注释
- 所有 `public` 属性必须有 `<summary>` 注释
- DTO 的每个属性必须有注释说明用途和约束

---

## 编码通用规则

### AOT 友好

所有后端代码必须兼容 .NET NativeAOT：

- 禁止反射（`System.Reflection`）
- 禁止 `dynamic`
- 禁止 `System.Linq`
- 禁止 Expression Tree
- 禁止运行时代码生成
- JSON 序列化必须使用 `JsonSerializerContext` 源生成

### 性能优先

- 优先使用 `Span<T>` / `ReadOnlySpan<T>` 进行字符串和内存操作
- 优先使用 `ArrayPool<T>` 避免频繁分配
- 优先使用 `Func<string>` 延迟求值避免不必要的字符串构造
- 优先使用值类型（`struct`）减少 GC 压力
- 避免在热路径上产生堆分配

### 错误处理

- 禁止吞掉异常（`catch {}` 空块）
- 异常捕获必须记录日志
- 后端错误响应统一使用 `ApiResult<T>` 格式
- 错误码统一使用 `ErrorCodes` 常量
- 错误消息统一使用 `Messages` 整形常量（`const int`，前端根据 Code 翻译）

---

## ID 生成规则

所有实体的主键 ID 必须在插入前显式获取和设置：

```csharp
// 正确
entity.Id = await DB.GetNextLongIdAsync();
await DB.InsertAsync(entity);

// 错误 — 禁止依赖数据库自增或框架自动分配
await DB.InsertAsync(entity); // 此时 entity.Id 为 0
```

原因：事务插入常依赖主表 ID，业务代码必须显式控制 ID 生成。

---

## 日志规则

### Logger.Debug 必须使用委托重载

```csharp
// 正确
Logger.Debug(tenantId, userId, () => $"[方法名] 参数: {param}");

// 错误 — 禁止直接传字符串
Logger.Debug(tenantId, userId, $"[方法名] 参数: {param}");

// 错误 — 禁止字符串连接
Logger.Debug(tenantId, userId, "[方法名] 参数: " + param);
```

原因：当 Debug 等级未启用时，`Func<string>` 不会被调用，避免不必要的字符串分配。

### 其他等级日志

`Logger.Error`、`Logger.Fatal` 可以直接传字符串（这些等级始终启用）：

```csharp
Logger.Error(tenantId, userId, $"[方法名] 异常: {ex}");
```

---

## JSON 规则

### 禁止手动 JSON 字符串拼接

```csharp
// 错误
var json = "{\"code\":" + code + ",\"message\":\"" + message + "\"}";

// 正确 — 使用 Utf8JsonWriter
using var writer = new Utf8JsonWriter(stream);
writer.WriteStartObject();
writer.WriteNumber("Code", code);
writer.WriteString("Message", message);
writer.WriteEndObject();

// 正确 — 使用 JsonSerializerContext 源生成
JsonSerializer.Serialize(result, TenantPlatformJsonSerializerContext.Default.ApiResult);
```

### JSON 属性名

- 后端统一使用 PascalCase（`Code`、`Message`、`Data`、`Items`、`Total`）
- 不配置 `PropertyNamingPolicy`
- 前端类型定义必须使用 PascalCase 匹配

---

## 数据库规则

- 仅支持 PostgreSQL + Npgsql
- 所有 SQL 使用参数化查询（通过 YTStdSqlBuilder）
- 禁止手写 SQL 字符串拼接
- 表结构由实体驱动生成，不手写 DDL

---

## 版本控制规则

- 每次修改后必须确保 `dotnet build YTStd.slnx` 通过
- 每次修改后必须确保 `dotnet test YTStd.slnx` 通过
- 不提交编译不通过的代码
- 不提交测试失败的代码
- 提交信息使用 Conventional Commits 格式

---

## 安全规则

- 禁止硬编码密码、密钥、Token
- 禁止在日志中输出敏感信息
- 禁止信任客户端输入（必须服务端验证）
- 禁止 SQL 拼接
- 认证 Token 必须验证签名和有效期
- 权限检查不可绕过
