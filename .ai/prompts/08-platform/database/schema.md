# 租户平台 — 数据库表结构

## 目标

校验并完善租户平台的数据库表结构，确保与 `.ai/rules/database.md` 完全一致。

---

## 前置阅读

- `.ai/rules/database.md` — 数据库命名规范
- `.ai/rules/naming.md` — 命名规范总则
- `docs/TenantPlatform/database_dictionary.md` — 已有数据字典

---

## 表前缀

本业务统一使用 `sys_` 表前缀。

---

## 表清单

### 平台管理

| 表名 | 说明 |
|------|------|
| `sys_user` | 平台用户 |
| `sys_role` | 平台角色 |
| `sys_permission` | 权限定义 |
| `sys_user_role` | 用户-角色关联 |
| `sys_role_permission` | 角色-权限关联 |
| `sys_menu` | 菜单树 |
| `sys_dictionary` | 数据字典 |

### 租户管理

| 表名 | 说明 |
|------|------|
| `sys_tenant` | 租户 |
| `sys_tenant_lifecycle_event` | 租户生命周期事件 |
| `sys_tenant_group` | 租户分组 |
| `sys_tenant_domain` | 租户域名绑定 |
| `sys_tenant_tag` | 租户标签 |

### 资源与配置

| 表名 | 说明 |
|------|------|
| `sys_resource_quota` | 资源配额 |
| `sys_config` | 系统配置 |
| `sys_feature_flag` | 功能开关 |

### 业务支撑

| 表名 | 说明 |
|------|------|
| `sys_package` | 套餐 |
| `sys_subscription` | 订阅 |
| `sys_billing` | 账单 |

### 运营支撑

| 表名 | 说明 |
|------|------|
| `sys_audit_log` | 审计日志 |
| `sys_operation_log` | 操作日志 |
| `sys_login_log` | 登录日志 |
| `sys_notification_template` | 通知模板 |
| `sys_api_key` | API 密钥 |
| `sys_file` | 文件存储 |

---

## 校验要点

1. 所有表名使用 `sys_` 前缀
2. 字段命名使用 `snake_case`
3. 布尔字段使用 `is_` 前缀（如 `is_enabled`）
4. 状态字段使用 `smallint`
5. 时间字段使用 `timestamptz`
6. 审计字段：`created_at`, `created_by`, `updated_at`, `updated_by`
7. 禁止裸 `tenant_id`，使用 `tenant_ref_id` 引用租户
8. 索引命名：`idx_{表名}_{字段名}`
9. 唯一索引命名：`udx_{表名}_{字段名}`

---

## 验收标准

- [ ] 所有表名符合 `sys_` 前缀规范
- [ ] 所有字段命名符合 snake_case
- [ ] 无裸 `tenant_id` 字段
- [ ] 索引命名正确
- [ ] `docs/TenantPlatform/database_dictionary.md` 已更新
