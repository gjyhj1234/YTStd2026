# 租户平台 — 认证 API

## 目标

重构认证 API，确保符合最新规范（后端零文本、ApiResult 格式）。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/security.md`
- `.ai/rules/api-design.md`
- `.ai/prompts/02-backend/endpoint.md`
- `backend/error-codes.md` — 错误码定义

---

## API 端点

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| POST | `/api/auth/login` | 公开 | 登录 |
| POST | `/api/auth/refresh-token` | 认证 | 刷新 Token（如需要） |
| POST | `/api/auth/change-password` | 认证 | 修改密码 |
| GET | `/api/health` | 公开 | 健康检查 |

---

## 登录响应

```json
{
  "Code": 0,
  "Message": 0,
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

## 业务规则

1. 密码使用安全哈希存储
2. 登录失败限制尝试次数（默认 5 次，锁定 15 分钟）
3. 登录事件写入 `sys_login_log`
4. JWT Token 包含：userId, roles, permissions, isSuperAdmin
5. Token 有效期可配置（默认 1 小时）

---

## 验收标准

- [ ] 登录成功返回有效 Token
- [ ] 登录失败返回 `ErrorCodes.InvalidCredentials`
- [ ] 账户锁定返回 `ErrorCodes.AccountLocked`
- [ ] 登录日志记录完整
- [ ] 编译和测试通过
