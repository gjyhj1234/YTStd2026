# 租户平台 — 租户生命周期 API

## 目标

重构租户生命周期管理 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/api-design.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | `/api/tenants` | tenant.list | 分页列表 |
| GET | `/api/tenants/{id}` | tenant.detail | 获取详情 |
| POST | `/api/tenants` | tenant.create | 创建租户 |
| PUT | `/api/tenants/{id}` | tenant.update | 更新租户 |
| DELETE | `/api/tenants/{id}` | tenant.delete | 删除租户 |
| PUT | `/api/tenants/{id}/initialize` | tenant.update | 初始化 |
| PUT | `/api/tenants/{id}/suspend` | tenant.update | 暂停 |
| PUT | `/api/tenants/{id}/resume` | tenant.update | 恢复 |
| PUT | `/api/tenants/{id}/terminate` | tenant.update | 终止 |
| PUT | `/api/tenants/{id}/convert-trial` | tenant.update | 试用转正 |
| GET | `/api/tenants/check-code-exists` | tenant.create | 检查编码唯一 |
| GET | `/api/tenants/{id}/lifecycle-events` | tenant.detail | 生命周期事件 |

---

## 生命周期状态机

```
注册 → 待初始化 → 活跃 → 暂停 → 恢复/终止
                 ↓
              试用中 → 转正/暂停/终止
```

| 当前状态 | 允许操作 | 目标状态 |
|---------|---------|---------|
| 待初始化 | 初始化 | 活跃/试用中 |
| 活跃 | 暂停 | 暂停 |
| 活跃 | 终止 | 已终止 |
| 暂停 | 恢复 | 活跃 |
| 暂停 | 终止 | 已终止 |
| 试用中 | 转正 | 活跃 |
| 试用中 | 暂停 | 暂停 |
| 试用中 | 终止 | 已终止 |
| 已终止 | — | 不允许任何操作 |

---

## 业务规则

1. 每次状态变更写入 `sys_tenant_lifecycle_event`
2. 租户编码唯一
3. 禁止裸 `TenantId` 字段，使用 `tenant_ref_id`
4. 终止后不可恢复

---

## 验收标准

- [ ] CRUD 完整
- [ ] 状态流转正确
- [ ] 非法状态流转返回 `ErrorCodes.TenantStatusNotAllowed`
- [ ] 生命周期事件记录
- [ ] 编码唯一性检查
- [ ] 编译通过
