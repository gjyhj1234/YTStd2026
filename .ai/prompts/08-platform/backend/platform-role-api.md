# 租户平台 — 平台角色 API

## 目标

重构平台角色管理 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/api-design.md`
- `.ai/prompts/02-backend/app-service.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | `/api/platform-roles` | platform.role.list | 分页列表 |
| GET | `/api/platform-roles/{id}` | platform.role.detail | 获取详情 |
| POST | `/api/platform-roles` | platform.role.create | 创建角色 |
| PUT | `/api/platform-roles/{id}` | platform.role.update | 更新角色 |
| DELETE | `/api/platform-roles/{id}` | platform.role.delete | 删除角色 |
| PUT | `/api/platform-roles/{id}/permissions` | platform.role.update | 分配权限 |
| GET | `/api/platform-roles/{id}/permissions` | platform.role.detail | 获取角色权限 |
| GET | `/api/platform-roles/check-code-exists` | platform.role.create | 检查角色编码 |
| GET | `/api/platform-roles/all` | platform.role.list | 获取全部角色（不分页，给下拉框用） |

---

## 业务规则

1. 角色编码唯一
2. 禁止删除 super-admin 角色
3. 权限分配时刷新 PermissionSnapshotCache

---

## 验收标准

- [ ] API 端点完整
- [ ] 角色编码唯一性检查
- [ ] 权限缓存刷新
- [ ] 编译通过
