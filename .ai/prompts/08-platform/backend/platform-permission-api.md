# 租户平台 — 平台权限 API

## 目标

重构平台权限管理 API 和权限中间件。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/security.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | `/api/platform-permissions` | platform.permission.list | 权限列表 |
| GET | `/api/platform-permissions/tree` | platform.permission.list | 权限树 |
| POST | `/api/platform-permissions` | platform.permission.create | 创建权限 |
| PUT | `/api/platform-permissions/{id}` | platform.permission.update | 更新权限 |
| DELETE | `/api/platform-permissions/{id}` | platform.permission.delete | 删除权限 |

---

## 权限类型

| 类型 | 格式 | 示例 |
|------|------|------|
| 菜单权限 | `{域}.{资源}.menu` | `platform.user.menu` |
| API 权限 | `{域}.{资源}.{动作}` | `platform.user.list` |
| 操作权限 | `{域}.{资源}.{操作}` | `platform.user.create` |

---

## 缓存与中间件

1. 启动时预热 PermissionSnapshotCache（角色ID → 权限码集合）
2. 权限变更时刷新缓存
3. PermissionMiddleware：
   - 获取当前用户 roles
   - 从缓存获取权限集合
   - 匹配端点权限码
   - 超级管理员跳过检查
   - 权限不足返回 403 + `ErrorCodes.PermissionDenied`

---

## 验收标准

- [ ] 权限 CRUD 完整
- [ ] 权限缓存预热正确
- [ ] 权限变更刷新缓存
- [ ] 中间件正确拦截
- [ ] 超级管理员跳过检查
- [ ] 编译通过
