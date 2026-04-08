# 租户平台 — 租户资源 API

## 目标

重构租户资源配额管理 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/tenants/{id}/resource-quotas` | 获取租户资源配额 |
| PUT | `/api/tenants/{id}/resource-quotas` | 更新资源配额 |
| GET | `/api/tenants/{id}/resource-usage` | 获取资源使用情况 |

---

## 资源类型

| 资源 | 说明 |
|------|------|
| 用户数上限 | 租户可创建的最大用户数 |
| 存储空间 | 文件存储上限（MB） |
| API 调用数 | 每日/每月 API 调用上限 |

---

## 验收标准

- [ ] 配额查询和更新正确
- [ ] 使用量统计正确
- [ ] 编译通过
