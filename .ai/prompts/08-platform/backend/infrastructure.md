# 租户平台 — 后端基础设施

## 目标

校验并重构租户平台后端基础设施，确保符合最新规范。

---

## 前置阅读

- `.ai/rules/backend.md` — 后端开发规范
- `.ai/rules/security.md` — 安全规范
- `.ai/prompts/02-backend/middleware.md` — 中间件通用规范
- `.ai/context/existing-modules.md` — 已有模块 API

---

## 现有文件

| 文件 | 说明 |
|------|------|
| `Program.cs` | 主程序入口 |
| `Bootstrap/ServiceRegistration.cs` | 服务注册 |
| `Bootstrap/RouteRegistration.cs` | 路由注册 |
| `Infrastructure/Auth/JwtService.cs` | JWT 认证 |
| `Infrastructure/Auth/PasswordService.cs` | 密码服务 |
| `Infrastructure/Middleware/*.cs` | 中间件 |
| `Infrastructure/Cache/*.cs` | 缓存 |
| `Infrastructure/Initialization/*.cs` | 初始化 |
| `Infrastructure/Serialization/*.cs` | JSON 序列化 |

---

## 重构要点

### 1. 中间件管道顺序

```
请求 → GlobalExceptionMiddleware → TraceIdMiddleware → RequestLogMiddleware
     → JWT 认证 → PermissionMiddleware → RateLimitMiddleware → AuditMiddleware
     → 路由处理 → 响应
```

### 2. JSON 序列化

- 使用 `JsonSerializerContext` 源生成（AOT 友好）
- PascalCase 属性名（`Code`, `Message`, `Data`）
- 所有 DTO 需注册到 JsonSerializerContext

### 3. 认证

- JWT Token 包含：userId, roles, permissions, isSuperAdmin
- 公开端点：`/api/auth/login`、`/api/health`
- 登录失败锁定
- Token 有效期配置

### 4. 中间件错误响应

- 所有中间件错误响应使用 PascalCase JSON
- 格式：`{ "Code": xxx, "Message": "...", "Data": null }`

---

## 约束

- Minimal API，禁止 MVC Controller
- 禁止反射、dynamic、LINQ
- Logger.Debug 使用 `Func<string>` 重载

---

## 验收标准

- [ ] 主程序可启动
- [ ] 中间件管道顺序正确
- [ ] 认证正确
- [ ] 中间件错误响应 PascalCase
- [ ] `dotnet build YTStd.slnx` 通过
- [ ] `dotnet test YTStd.slnx` 通过
