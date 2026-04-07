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

| 前缀 | 适用范围 | 示例 |
|------|---------|------|
| `plt_` | 平台管理表 | `plt_user`, `plt_role`, `plt_permission` |
| `tnt_` | 租户相关表 | `tnt_info`, `tnt_lifecycle_event`, `tnt_resource_quota` |
| `saas_` | SaaS 业务表 | `saas_package`, `saas_subscription`, `saas_billing` |
| `sys_` | 系统配置表 | `sys_config`, `sys_feature_flag`, `sys_dictionary` |
| `log_` | 日志审计表 | `log_audit`, `log_operation`, `log_login` |
| `ntf_` | 通知表 | `ntf_template`, `ntf_record` |
| `api_` | API 集成表 | `api_key`, `api_quota`, `api_webhook` |
| `stg_` | 存储表 | `stg_file`, `stg_policy` |

### 命名规则

1. 前缀 + 业务名称
2. 使用名词或名词短语
3. 单数形式（`plt_user` 不是 `plt_users`）
4. 多个单词用下划线分隔

### 关联表命名

多对多关联表使用 `{前缀}_{表A}_{表B}` 格式：

```
plt_role_permission        # 角色-权限关联
plt_user_role              # 用户-角色关联
tnt_tag_relation           # 租户-标签关联
```

### 特殊表命名

| 表类型 | 命名规则 | 示例 |
|-------|---------|------|
| 历史表 | `{原表名}_history` | `tnt_info_history` |
| 日志表 | `log_{业务}` | `log_audit`, `log_login` |
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
- 示例：`role_id`（引用 `plt_role.id`）、`package_id`（引用 `saas_package.id`）
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
| 普通索引 | `idx_{表名}_{列名}` | `idx_plt_user_username` |
| 唯一索引 | `uq_{表名}_{列名}` | `uq_plt_user_username` |
| 组合索引 | `idx_{表名}_{列1}_{列2}` | `idx_plt_user_role_user_id_role_id` |
| 外键约束 | `fk_{表名}_{引用表名}` | `fk_plt_user_role_role_id` |

---

## 外键规范

- 当前项目以实体引用和应用层验证为主
- 不强制要求数据库级外键约束
- 如果使用外键，命名按上述规范

---

## 表名长度控制

- 目标：表名在 30 字符以内
- 上限：63 字符（PostgreSQL 限制）
- 优先使用缩写前缀（`plt_`、`tnt_`、`saas_`）缩短表名
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
