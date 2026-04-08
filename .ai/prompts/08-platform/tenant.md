# 租户模块提示词

## 目标

实现租户生命周期管理、租户信息管理和租户资源管理。

---

## 适用范围

实现或修改租户相关功能时使用。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/database.md`
- `.ai/rules/api-design.md`

---

## 输入

- 租户相关实体
- 租户业务需求

---

## 输出

### 生命周期

- `Application/Services/TenantLifecycleAppService.cs`
- `Endpoints/TenantEndpoints.cs`

### 信息管理

- `Application/Services/TenantInfoAppService.cs`
- `Endpoints/TenantInfoEndpoints.cs`

### 资源管理

- `Application/Services/TenantResourceAppService.cs`
- `Endpoints/TenantResourceEndpoints.cs`

---

## 租户生命周期

```
注册 → 初始化 → 活跃 → 暂停 → 恢复/终止
                    ↓
                  试用到期
```

### 状态流转规则

| 当前状态 | 允许的操作 |
|---------|-----------|
| 待初始化 | 初始化 |
| 活跃 | 暂停、终止 |
| 暂停 | 恢复、终止 |
| 试用中 | 转正、暂停、终止 |
| 已终止 | 不允许任何操作 |

---

## 约束

- 禁止使用裸 `TenantId` / `tenant_id`
- 使用 `tenant_ref_id` 引用租户
- 状态流转必须验证当前状态
- 关键操作必须记录生命周期事件

---

## 验收标准

- [ ] 租户 CRUD 完整
- [ ] 状态流转逻辑正确
- [ ] 生命周期事件记录
- [ ] 无裸 TenantId 字段
- [ ] 编译和测试通过
