# API 端点提示词

## 目标

为指定模块创建 Minimal API 端点，包含路由注册、权限标注和响应处理。

---

## 适用范围

创建或修改 API 端点时使用。

---

## 前置阅读

- `.ai/rules/api-design.md`
- `.ai/rules/backend.md`
- `.ai/rules/naming.md`
- `.ai/rules/security.md`

---

## 输入

- 已完成的应用服务
- API 路由设计

---

## 输出

- `Endpoints/{Module}Endpoints.cs`
- `Bootstrap/RouteRegistration.cs` 更新

---

## 执行步骤

1. 创建端点类（静态方法）
2. 定义 Map 方法注册路由
3. 每个端点方法实现请求解析和响应输出
4. 更新 RouteRegistration 注册该端点组
5. 执行 `dotnet build` 验证

---

## 端点实现模式

```csharp
/// <summary>
/// 平台用户管理 API 端点
/// </summary>
public static class PlatformUserEndpoints
{
    /// <summary>
    /// 注册平台用户相关路由
    /// </summary>
    public static void Map(RouteGroupBuilder group)
    {
        var users = group.MapGroup("/platform-users");

        users.MapGet("/", ListAsync);
        users.MapGet("/{id:long}", GetAsync);
        users.MapPost("/", CreateAsync);
        users.MapPut("/{id:long}", UpdateAsync);
        users.MapPut("/{id:long}/enable", EnableAsync);
        users.MapPut("/{id:long}/disable", DisableAsync);
        users.MapDelete("/{id:long}", DeleteAsync);
        users.MapGet("/check-username-exists", CheckUsernameExistsAsync);
    }

    /// <summary>
    /// 获取平台用户列表
    /// </summary>
    private static async Task<IResult> ListAsync(HttpContext context)
    {
        // 从 context 获取认证信息
        // 调用应用服务
        // 返回 ApiResult
    }
}
```

---

## 约束

- 使用 Minimal API / RouteGroup 方式
- 所有端点返回 `ApiResult<T>`
- 所有非公开端点有权限码
- JSON 响应使用 PascalCase

---

## 禁止事项

- 禁止使用 MVC Controller
- 禁止在端点中直接操作数据库
- 禁止跳过权限检查

---

## 验收标准

- [ ] 所有端点已注册到 RouteRegistration
- [ ] HTTP 方法和路径符合 RESTful 规范
- [ ] 有唯一索引的字段有 check-exists 端点
- [ ] 编译通过
