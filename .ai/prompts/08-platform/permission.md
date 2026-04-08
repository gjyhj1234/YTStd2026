# 权限模块提示词

## 目标

实现平台权限管理能力，包含权限定义、角色-权限绑定、权限检查。

---

## 适用范围

实现或修改权限相关功能时使用。

---

## 前置阅读

- `.ai/rules/security.md`
- `.ai/rules/backend.md`

---

## 输入

- 权限模型设计
- 已完成的权限实体

---

## 输出

- `Application/Services/PlatformPermissionAppService.cs`
- `Endpoints/PlatformPermissionEndpoints.cs`
- `Infrastructure/Cache/PermissionSnapshotCache.cs`
- `Infrastructure/Middleware/PermissionMiddleware.cs`
- 权限初始化数据
- 权限测试

---

## 权限类型

| 类型 | 说明 | 示例 |
|------|------|------|
| 菜单权限 | 控制菜单和页面可见性 | `platform.user.menu` |
| API 权限 | 控制 API 端点访问 | `platform.user.list` |
| 操作权限 | 控制按钮和操作可用性 | `platform.user.create` |
| 数据权限 | 控制数据行级可见性 | `tenant.data.own` |

---

## 权限检查流程

```
请求进入
→ PermissionMiddleware
  → 获取当前用户 roles
  → 从 PermissionSnapshotCache 获取角色权限
  → 匹配当前端点需要的权限码
  → 超级管理员跳过检查
  → 权限通过则继续，否则返回 403
```

---

## 缓存策略

- 启动时预热权限缓存
- 权限变更时刷新缓存
- 缓存结构：角色ID → 权限码集合

---

## 约束

- 权限码格式：`{域}.{资源}.{操作}`
- 超级管理员跳过权限检查
- 权限中间件错误响应使用 PascalCase JSON

---

## 验收标准

- [ ] 权限 CRUD 完整
- [ ] 权限缓存正确
- [ ] 权限中间件正确拦截
- [ ] 超级管理员跳过检查
- [ ] 编译和测试通过
