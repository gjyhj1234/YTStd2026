# 后端开发规范

## 目标

定义后端 C# / .NET 代码的开发规范，确保所有后端代码风格统一、性能优良、AOT 友好。

---

## 适用范围

`src/` 和 `tests/` 下所有 C# 项目。

---

## 架构规范

### 单体部署

- 必须保持单体主程序架构
- 所有 WebAPI、权限判断、后台任务、缓存在同一进程中运行
- 禁止拆分为微服务
- 禁止引入服务发现、API 网关、消息队列
- 横向扩展只在入口层增加负载均衡

### 分层结构

```
src/{Project}/
├── Program.cs                 # 入口
├── Bootstrap/                 # 启动引导
│   ├── ServiceRegistration.cs # 服务注册
│   ├── RouteRegistration.cs   # 路由注册
│   └── StartupInitialization.cs # 启动初始化
├── Application/               # 应用层
│   ├── Constants/             # 常量
│   ├── Dtos/                  # 数据传输对象
│   └── Services/              # 应用服务
├── Domain/                    # 领域层
│   ├── Enums/                 # 枚举
│   └── ValueObjects/          # 值对象
├── Endpoints/                 # API 端点
├── Infrastructure/            # 基础设施
│   ├── Auth/                  # 认证
│   ├── Cache/                 # 缓存
│   ├── Initialization/        # 初始化
│   ├── Middleware/             # 中间件
│   ├── Scheduling/            # 后台任务
│   └── Serialization/         # JSON 序列化
└── entity/                    # 实体
    └── {Module}/              # 模块实体
```

---

## 实体规范

### 实体定义位置

所有实体必须放在 `src/{Project}/entity/{Module}/*.cs`

### 实体特性

```csharp
[Entity(TableName = "sys_user", Description = "平台用户")]
public class PlatformUser
{
    [Column(Name = "id", DbType = "bigint", IsPrimaryKey = true, Description = "主键 ID")]
    public long Id { get; set; }

    [Column(Name = "username", DbType = "varchar(50)", Description = "用户名")]
    public string Username { get; set; }
}
```

### 租户字段命名

- 禁止使用裸 `TenantId` / `tenant_id`
- 必须使用业务语义化命名：`TenantRefId`、`OwnerTenantId`、`SourceTenantId`
- 原因：框架会把裸 `TenantId` 识别为租户业务分区字段

---

## 应用服务规范

### 服务命名

- 命名格式：`{Module}AppService`
- 位置：`Application/Services/{Module}AppService.cs`

### 服务实现规则

1. 所有 `InsertAsync` 前必须获取 ID：`entity.Id = await DB.GetNextLongIdAsync();`
2. 所有 `ApiResult.Fail()` 仅传 `ErrorCodes.XXX`（不传 message 参数）
3. 所有 `Logger.Debug` 使用 `Func<string>` 延迟求值
4. 唯一性验证使用 `GetListAsync` + 内存 foreach 循环
5. 错误码使用 `ErrorCodes.XXX` 常量
6. 返回值使用 `ApiResult<T>` 包装

### 唯一性验证模式

所有包含唯一索引字段的实体，Create/Save 方法必须遵循**唯一性双重校验模式**：

**第一重：前置校验** — 在 InsertAsync 前检查唯一字段是否已存在

```csharp
var (chkResult, existing) = await XxxCRUD.GetListAsync(tenantId, operatorId);
if (chkResult.Success && existing != null)
{
    foreach (var item in existing)
    {
        if (string.Equals(item.Code, req.Code.Trim(), StringComparison.OrdinalIgnoreCase))
            return ApiResult<long>.Fail(ErrorCodes.XxxCodeExists);
    }
}
```

**第二重：后置复核** — InsertAsync 失败时重新检查唯一性，返回精确错误码

```csharp
var insResult = await XxxCRUD.InsertAsync(tenantId, operatorId, entity);
if (!insResult.Success)
{
    // 重新检查唯一性，判断是否因唯一约束冲突导致插入失败
    var (rechkResult, rechkData) = await XxxCRUD.GetListAsync(tenantId, operatorId);
    if (rechkResult.Success && rechkData != null)
    {
        foreach (var item in rechkData)
        {
            if (string.Equals(item.Code, entity.Code, StringComparison.OrdinalIgnoreCase))
                return ApiResult<long>.Fail(ErrorCodes.XxxCodeExists);
        }
    }
    return ApiResult<long>.Fail(ErrorCodes.XxxCreateFailed);
}
```

**规则：**
- 每个唯一字段必须有对应的 `ErrorCodes.XxxExists` 错误码（位于 18xxx 段）
- 不允许唯一字段冲突时仅返回笼统的 `XxxCreateFailed` 错误码
- 并发竞争场景下（前置校验通过但 InsertAsync 仍因唯一索引失败），必须通过后置复核返回精确的 `XxxExists` 错误码
- Update 方法如果修改了唯一字段，也需要排他性唯一校验（排除自身 Id）

---

## API 端点规范

### 注册方式

使用 Minimal API / RouteGroup 方式，禁止 MVC Controller：

```csharp
public static class PlatformUserEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", ListAsync);
        group.MapGet("/{id:long}", GetAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:long}", UpdateAsync);
        group.MapPut("/{id:long}/enable", EnableAsync);
        group.MapPut("/{id:long}/disable", DisableAsync);
        group.MapDelete("/{id:long}", DeleteAsync);
    }
}
```

### 响应格式

所有端点返回 `ApiResult<T>`：

- 成功：`code=0`
- 业务失败：`code=ErrorCodes.XXX`
- 系统错误：由全局异常中间件捕获

---

## 缓存规范

### 缓存策略

- 使用 Local Cache（进程内缓存）
- 禁止 Redis / 分布式缓存
- 缓存刷新策略：启动预热 + 显式失效 + 定时轻量刷新

### 缓存内容

至少缓存：
- 用户-角色关系
- 角色-权限关系
- 权限码与菜单树
- 功能开关 / 基础配置

---

## 中间件规范

必须实现的中间件：
1. 全局异常处理中间件
2. 请求日志 / TraceId 中间件
3. 权限中间件
4. 限流中间件
5. 审计记录中间件

中间件错误响应必须使用 PascalCase JSON 属性名（`Code`、`Message`、`Data`）。

---

## 测试规范

### 测试类型

- 实体编译与生成器协同测试
- 初始化数据幂等性测试
- 中间件行为测试
- 权限与限流测试
- 核心 API 测试

### 测试位置

`tests/{Project}.Tests/*.cs`

### CRUD 结果判断

```csharp
// 正确 — CRUD 结果使用 .Success
var result = await DB.InsertAsync(entity);
Assert.True(result.Success);

// 错误 — CRUD 结果没有 .Code 属性（只有 ApiResult 有）
```
