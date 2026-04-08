# 租户平台 — 初始化数据

## 目标

校验并完善租户平台的初始化数据，确保幂等可重复执行。

---

## 前置阅读

- `.ai/prompts/04-database/seed-data.md` — 初始化数据通用规范
- `database/schema.md` — 本业务表结构

---

## 必须初始化的数据

### 1. 超级管理员

- `sys_user`：admin 用户
- `sys_role`：super-admin 角色
- `sys_user_role`：admin → super-admin 关联

### 2. 权限

- `sys_permission`：所有权限码定义（格式 `{域}.{资源}.{操作}`）
- `sys_role_permission`：super-admin 角色的全部权限

### 3. 菜单

- `sys_menu`：完整菜单树（与 `docs/TenantPlatform/architecture.md` 一致）

### 4. 字典

- `sys_dictionary`：基础字典数据

### 5. 配置

- `sys_config`：默认系统配置（密码策略、登录策略等）
- `sys_feature_flag`：默认功能开关

---

## 验收标准

- [ ] 初始化数据可重复执行无脏数据
- [ ] 超级管理员可登录
- [ ] 权限完整
- [ ] 菜单树正确
- [ ] 编译通过
