# 租户平台 — 审计日志 API

## 目标

重构审计日志查询 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/security.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/audit-logs` | 审计日志列表（分页） |
| GET | `/api/audit-logs/{id}` | 审计日志详情 |
| GET | `/api/operation-logs` | 操作日志列表 |
| GET | `/api/login-logs` | 登录日志列表 |

---

## 业务规则

1. 审计日志只读，不允许修改和删除
2. 支持按操作人、操作类型、时间范围筛选
3. 变更内容使用 JSON 存储（变更前/后数据）

---

## 日志类型

| 类型 | 表 | 记录内容 |
|------|---|---------|
| 审计日志 | `sys_audit_log` | 数据变更 |
| 操作日志 | `sys_operation_log` | 关键业务操作 |
| 登录日志 | `sys_login_log` | 登录/登出 |

---

## 验收标准

- [ ] 日志查询支持分页和筛选
- [ ] 日志数据只读
- [ ] 编译通过
