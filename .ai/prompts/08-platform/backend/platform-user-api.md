# 租户平台 — 平台用户 API

## 目标

重构平台用户管理 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/api-design.md`
- `.ai/prompts/02-backend/app-service.md`
- `.ai/prompts/02-backend/endpoint.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | `/api/platform-users` | platform.user.list | 分页列表 |
| GET | `/api/platform-users/{id}` | platform.user.detail | 获取详情 |
| POST | `/api/platform-users` | platform.user.create | 创建用户 |
| PUT | `/api/platform-users/{id}` | platform.user.update | 更新用户 |
| DELETE | `/api/platform-users/{id}` | platform.user.delete | 删除用户 |
| PUT | `/api/platform-users/{id}/enable` | platform.user.update | 启用 |
| PUT | `/api/platform-users/{id}/disable` | platform.user.update | 禁用 |
| PUT | `/api/platform-users/{id}/reset-password` | platform.user.update | 重置密码 |
| GET | `/api/platform-users/check-username-exists` | platform.user.create | 检查用户名 |
| PUT | `/api/platform-users/batch-enable` | platform.user.update | 批量启用 |
| PUT | `/api/platform-users/batch-disable` | platform.user.update | 批量禁用 |

---

## 业务规则

1. 创建用户前检查用户名唯一性
2. 创建用户时 `entity.Id = await DB.GetNextLongIdAsync()`
3. 密码使用 `PasswordService` 哈希存储
4. 禁止删除超级管理员
5. 禁止禁用自己
6. 批量操作使用 foreach + 单条更新（不使用批量 SQL）

---

## DTO

- `CreatePlatformUserReqDTO`：username, password, displayName, email, phone, roleIds
- `UpdatePlatformUserReqDTO`：displayName, email, phone, roleIds
- `PlatformUserRepDTO`：id, username, displayName, email, phone, status, roles, createdAt
- `PlatformUserListRepDTO`：分页包装

---

## 验收标准

- [ ] API 端点完整（11 个）
- [ ] 唯一性检查正确
- [ ] ID 生成正确
- [ ] 密码哈希正确
- [ ] 超级管理员保护
- [ ] 编译通过
