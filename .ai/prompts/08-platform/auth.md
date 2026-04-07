# 认证模块提示词

## 目标

实现平台认证能力，包含登录、Token 管理、密码管理。

---

## 适用范围

实现或修改认证相关功能时使用。

---

## 前置阅读

- `.ai/rules/security.md`
- `.ai/rules/backend.md`
- `.ai/rules/api-design.md`

---

## 输入

- 认证需求（JWT、密码策略）
- 已完成的平台用户实体

---

## 输出

- `Infrastructure/Auth/JwtService.cs` — JWT 生成与验证
- `Infrastructure/Auth/PasswordService.cs` — 密码哈希与验证
- `Bootstrap/RouteRegistration.cs` — 登录路由（公开端点）
- 认证相关测试

---

## 执行步骤

1. 实现 JWT Token 生成（包含 userId、roles、permissions、isSuperAdmin）
2. 实现 JWT Token 验证
3. 实现密码安全哈希
4. 实现登录端点（POST /api/auth/login）
5. 实现 Token 刷新端点（如需要）
6. 实现登录失败锁定逻辑
7. 实现登录日志记录
8. 编写认证测试
9. 验证编译和测试

---

## 登录响应格式

```json
{
  "Code": 0,
  "Message": "success",
  "Data": {
    "Token": "eyJhbG...",
    "ExpiresIn": 3600,
    "UserId": 1,
    "Username": "admin",
    "DisplayName": "管理员",
    "Roles": ["super-admin"],
    "Permissions": ["platform.user.list", "..."],
    "IsSuperAdmin": true
  }
}
```

---

## 约束

- 密码必须使用安全哈希存储
- Token 必须设置有效期
- 登录失败必须限制尝试次数
- 登录日志必须记录 IP 和时间
- 公开端点：`/api/auth/login`、`/api/health`

---

## 禁止事项

- 禁止明文存储密码
- 禁止在 Token 中存储密码
- 禁止在日志中输出密码和 Token

---

## 验收标准

- [ ] 登录成功返回有效 Token
- [ ] 登录失败返回正确错误码
- [ ] 密码验证安全
- [ ] 登录日志完整
- [ ] 编译和测试通过
