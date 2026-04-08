# 中间件提示词

## 目标

实现后端中间件组件，包含异常处理、日志、权限、限流、审计等。

---

## 适用范围

创建或修改中间件时使用。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/security.md`
- `.ai/rules/global.md`

---

## 输入

- 中间件需求描述
- 已有的中间件管道

---

## 输出

- `Infrastructure/Middleware/{Name}Middleware.cs`
- `Bootstrap/ServiceRegistration.cs` 更新（如需要）

---

## 必须实现的中间件

| 中间件 | 说明 | 优先级 |
|-------|------|-------|
| GlobalExceptionMiddleware | 全局异常捕获，返回统一错误响应 | 最外层 |
| RequestLoggingMiddleware | 请求日志、TraceId 生成 | 第二层 |
| AuthenticationMiddleware | JWT Token 验证 | 第三层 |
| PermissionMiddleware | 权限码检查 | 第四层 |
| RateLimitMiddleware | 限流保护 | 第五层 |
| AuditMiddleware | 审计记录 | 最内层 |

---

## 中间件管道顺序

```
请求入口
  → GlobalExceptionMiddleware（异常捕获）
    → RequestLoggingMiddleware（日志 + TraceId）
      → AuthenticationMiddleware（认证）
        → PermissionMiddleware（授权）
          → RateLimitMiddleware（限流）
            → AuditMiddleware（审计）
              → Endpoint（业务处理）
            ← AuditMiddleware
          ← RateLimitMiddleware
        ← PermissionMiddleware
      ← AuthenticationMiddleware
    ← RequestLoggingMiddleware
  ← GlobalExceptionMiddleware
响应输出
```

---

## 约束

- 中间件错误响应必须使用 PascalCase JSON 属性名（`Code`、`Message`、`Data`）
- 中间件必须使用 `Utf8JsonWriter` 写入 JSON 响应
- 中间件不得吞掉异常
- 权限中间件必须支持超级管理员跳过检查

---

## 禁止事项

- 禁止手动 JSON 字符串拼接
- 禁止使用 camelCase JSON 属性名
- 禁止在中间件中直接操作数据库（缓存除外）

---

## 验收标准

- [ ] 中间件管道顺序正确
- [ ] 异常中间件捕获所有未处理异常
- [ ] 权限中间件正确检查权限码
- [ ] 限流中间件正确限制请求频率
- [ ] 审计中间件记录关键操作
- [ ] 所有错误响应使用 PascalCase
- [ ] 编译通过
- [ ] 中间件行为测试通过
