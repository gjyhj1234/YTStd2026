# 数据库与命名规范

## 目标

统一数据库表命名、字段命名、索引命名等规则，确保所有数据库对象名称一致、可读、无歧义。

---

## 适用范围

所有 PostgreSQL 数据库表和相关对象。

---

## 通用命名规则

- 所有数据库对象名使用 **snake_case**（小写 + 下划线）
- 单词间使用单下划线分隔
- 禁止使用 PostgreSQL 保留字作为表名或列名
- 禁止使用缩写（除公认缩写如 `id`、`url`、`ip`、`mfa`）
- 表名长度控制在 63 字符以内（PostgreSQL 限制）
- 优先保证可读性，其次控制长度

---

## 表名规范

### 前缀体系

#### 租户平台表（系统管理）

| 前缀 | 适用范围 | 示例 | 说明 |
|------|---------|------|------|
| `sys_` | 租户平台所有表 | `sys_user`, `sys_role`, `sys_permission`, `sys_tenant`, `sys_config`, `sys_dictionary`, `sys_audit_log`, `sys_notification`, `sys_api_key`, `sys_file` | 租户后台统一前缀，涵盖平台管理、租户管理、系统配置、日志审计、通知、API 集成、存储等所有系统管理相关表。使用 `sys_` 前缀表示这些表不参与业务分表 |

#### 业务模块表（后续 SaaS 业务扩展时使用）

| 前缀 | 适用范围 | 示例 | 说明 |
|------|---------|------|------|
| `pat_` | 患者相关业务表 | `pat_patient`, `pat_medical_record` | 患者业务模块，会进行分表处理 |
| `fee_` | 费用相关业务表 | `fee_charge`, `fee_invoice` | 费用业务模块，会进行分表处理 |
| 其他 | 其他业务域 | 按业务域缩写定义 | 每个业务域使用独立的 2-4 字母前缀 |

#### 前缀设计原则

1. `sys_` 前缀表示**不需要分表**的系统管理表，包括但不限于：平台用户、角色、权限、菜单、租户、租户信息、资源配额、套餐、订阅、计费、配置、功能开关、字典、审计日志、操作日志、登录日志、通知模板、通知记录、API 密钥、API 配额、Webhook、文件存储
2. 业务域前缀表示**可能需要分表**的业务数据表
3. 新增业务模块时，由项目负责人确定业务前缀并记录到本文件

### 命名规则

1. 前缀 + 业务名称
2. 使用名词或名词短语
3. 单数形式（`sys_user` 不是 `sys_users`）
4. 多个单词用下划线分隔

### 关联表命名

多对多关联表使用 `{前缀}_{表A}_{表B}` 格式：

```
sys_role_permission        # 角色-权限关联
sys_user_role              # 用户-角色关联
sys_tenant_tag             # 租户-标签关联
```

### 特殊表命名

| 表类型 | 命名规则 | 示例 |
|-------|---------|------|
| 历史表 | `{原表名}_history` | `sys_tenant_history` |
| 日志表 | `sys_{业务}_log` | `sys_audit_log`, `sys_login_log` |
| 配置表 | `sys_{配置项}` | `sys_config`, `sys_feature_flag` |
| 字典表 | `sys_dictionary` | 统一一张字典表 |
| 审计表 | 实体框架自动生成 | 由 `[Entity]` 审计特性控制 |

---

## 字段命名规范

### 主键

- 所有表的主键统一命名为 `id`
- 类型为 `bigint`
- 由应用层通过 `DB.GetNextLongIdAsync()` 生成

### 外键引用字段

- 命名格式：`{被引用表简称}_id`
- 示例：`role_id`（引用 `sys_role.id`）、`package_id`（引用 `sys_package.id`）
- 特殊情况：`tenant_ref_id`（引用租户主表，禁止使用裸 `tenant_id`）

### 租户引用字段

- 禁止使用 `tenant_id`
- 必须使用 `tenant_ref_id`、`owner_tenant_id`、`source_tenant_id` 等语义化名称
- 原因：框架会把 `tenant_id` 识别为租户业务分区字段

### 审计字段

| 字段名 | 类型 | 说明 |
|-------|------|------|
| `created_at` | `timestamp with time zone` | 创建时间 |
| `updated_at` | `timestamp with time zone` | 更新时间 |
| `created_by` | `bigint` | 创建人 ID |
| `updated_by` | `bigint` | 更新人 ID |

### 布尔字段

- 命名格式：`is_{形容词}` 或 `has_{名词}`
- 示例：`is_enabled`、`is_deleted`、`is_default`、`has_mfa`
- 类型：`boolean`
- 禁止使用 `int` 表示布尔值

### 状态字段

- 命名：`status`
- 类型：`smallint`
- 必须有对应的枚举定义
- 禁止使用字符串表示状态

### 时间字段

- 命名格式：`{动作}_at`
- 类型：`timestamp with time zone`
- 示例：`expired_at`、`activated_at`、`suspended_at`、`deleted_at`
- 禁止使用 `_time` 或 `_date` 后缀

### 排序字段

- 命名：`sort_order`
- 类型：`int`
- 默认值：`0`

### 描述/备注字段

- 短文本描述：`description`，类型 `varchar(500)`
- 长文本备注：`remark`，类型 `text`

---

## 索引命名规范

| 索引类型 | 命名格式 | 示例 |
|---------|---------|------|
| 普通索引 | `idx_{表名}_{列名}` | `idx_sys_user_username` |
| 唯一索引 | `uq_{表名}_{列名}` | `uq_sys_user_username` |
| 组合索引 | `idx_{表名}_{列1}_{列2}` | `idx_sys_user_role_user_id_role_id` |
| 外键约束 | `fk_{表名}_{引用表名}` | `fk_sys_user_role_role_id` |

---

## 外键规范

- 当前项目以实体引用和应用层验证为主
- 不强制要求数据库级外键约束
- 如果使用外键，命名按上述规范

---

## 关系建模规范

### 一对多关系（1:N）

在子表中添加逻辑外键字段指向父表，**必须**为逻辑外键添加排序索引。通过查询子表获取关联数据。

| 组件 | 示例 |
|------|------|
| 子表逻辑外键 | `Tenant.GroupId`（指向 `sys_tenant_group.id`） |
| 排序索引 | `[Index("idx_sys_tenant_group_id", "group_id")]` |
| 查询方式 | `SELECT * FROM sys_tenant WHERE group_id = @groupId` |

**禁止**在父表使用 `bigint[]` 数组字段存储子表 ID 集合。原因：
- 数组随子表记录增加而膨胀，更新开销大
- 每次子表变更都需要更新父表数组，写放大严重
- 无法利用索引进行高效的反向查询

### 多对多关系（N:N）

**标准方式**：使用中间关联表（如 `sys_role_permission`、`sys_tenant_tag_binding`）。

**例外**：当关联集合**同时满足**以下全部条件时，可考虑使用 PostgreSQL `bigint[]` 数组字段替代中间表：

1. 集合元素数量极少（通常 < 20）
2. 变更频率极低（配置级别，非业务操作级别）
3. 不需要在数组字段上进行反向查询（即不需要"根据子 ID 查父"）

若上述**任一条件**不满足，**必须**使用中间关联表。

### 历史教训

`TenantGroup.TenantRefIds`（`bigint[]`）曾被用于存储分组下的租户 ID 列表。由于租户数量不可控且分组成员变更频繁，导致数组膨胀和频繁的父表行更新。正确做法是在 `Tenant` 表上使用 `GroupId` 逻辑外键 + `idx_sys_tenant_group_id` 排序索引。

---

## 表名长度控制

- 目标：表名在 30 字符以内
- 上限：63 字符（PostgreSQL 限制）
- 优先使用缩写前缀（`sys_`、以及业务域前缀如 `pat_`、`fee_`）缩短表名
- 如仍然过长，可缩写中间部分但必须保持可读性

---

## 数据类型约定

| 用途 | 推荐类型 | 说明 |
|------|---------|------|
| 主键 | `bigint` | 应用层生成 |
| 短字符串 | `varchar(N)` | 指定最大长度 |
| 长文本 | `text` | 不限长度 |
| 状态/枚举 | `smallint` | 节省空间 |
| 布尔 | `boolean` | 不用 int |
| 时间 | `timestamp with time zone` | 带时区 |
| 金额 | `numeric(18,2)` | 精确计算 |
| JSON | `jsonb` | PostgreSQL 原生 |
| UUID | `uuid` | 特殊场景使用 |
