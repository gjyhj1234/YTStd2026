# {业务名称} — 后端基础设施

## 目标

搭建 {业务名称} 后端主程序骨架，包含启动引导、中间件管道、认证配置。

---

## 前置阅读

- `.ai/rules/backend.md` — 后端开发规范
- `.ai/rules/security.md` — 安全规范
- `.ai/prompts/02-backend/middleware.md` — 中间件通用规范

---

## 输出

- `src/YTStd{Business}/Program.cs` — 主程序入口
- `src/YTStd{Business}/Bootstrap/ServiceRegistration.cs` — 服务注册
- `src/YTStd{Business}/Bootstrap/RouteRegistration.cs` — 路由注册
- `src/YTStd{Business}/Infrastructure/Middleware/*.cs` — 中间件
- `src/YTStd{Business}/Infrastructure/Auth/JwtService.cs` — JWT 认证
- `src/YTStd{Business}/Infrastructure/Serialization/*JsonSerializerContext.cs` — JSON 序列化上下文

---

## 中间件管道

```
请求 → 全局异常处理 → TraceId → 请求日志 → 认证 → 权限 → 限流 → 审计 → 路由 → 响应
```

---

## 约束

- 使用 Minimal API，禁止 MVC Controller
- JSON 序列化使用 JsonSerializerContext 源生成
- 中间件错误响应使用 PascalCase JSON

---

## 验收标准

- [ ] 主程序可启动
- [ ] 中间件管道正确
- [ ] 认证中间件正确
- [ ] 编译通过
