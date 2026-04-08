# {业务名称} — 数据库表结构设计

## 目标

设计 {业务名称} 的数据库表结构，遵循项目数据库规范。

---

## 前置阅读

- `.ai/rules/database.md` — 数据库命名规范
- `.ai/rules/naming.md` — 命名规范总则
- `docs/{Business}/database_dictionary.md` — 数据字典（如已存在）

---

## 表前缀

本业务使用 `{prefix}_` 表前缀。

---

## 表设计

### {prefix}_{table1}（{表1中文名}）

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| id | bigint | PK | 主键 |
| ... | ... | ... | ... |
| created_at | timestamptz | NOT NULL | 创建时间 |
| created_by | bigint | NOT NULL | 创建人 |
| updated_at | timestamptz | | 更新时间 |
| updated_by | bigint | | 更新人 |

索引：
- `idx_{prefix}_{table1}_{field}` UNIQUE

### {prefix}_{table2}（{表2中文名}）

（同上模板）

---

## 枚举值

### {EnumName}

| 值 | 含义 |
|----|------|
| 0 | ... |
| 1 | ... |

---

## 约束

- 所有表使用 `{prefix}_` 前缀
- 所有表有 `id`, `created_at`, `created_by` 审计字段
- 布尔字段使用 `is_` 前缀
- 状态字段使用 `smallint` 类型
- 时间字段使用 `timestamptz`

---

## 验收标准

- [ ] 所有表名有正确前缀
- [ ] 命名符合 `.ai/rules/database.md`
- [ ] 索引命名正确
- [ ] 审计字段完整
- [ ] 数据字典文档已更新
