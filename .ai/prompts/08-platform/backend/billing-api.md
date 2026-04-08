# 租户平台 — 计费账单 API

## 目标

重构计费与账单管理 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/billings` | 账单列表 |
| GET | `/api/billings/{id}` | 账单详情 |
| POST | `/api/billings` | 创建账单 |
| PUT | `/api/billings/{id}/pay` | 标记已支付 |
| PUT | `/api/billings/{id}/void` | 作废账单 |
| GET | `/api/tenants/{id}/billings` | 获取租户账单 |
| GET | `/api/billings/statistics` | 账单统计 |

---

## 验收标准

- [ ] CRUD 完整
- [ ] 支付/作废状态流转正确
- [ ] 统计查询正确
- [ ] 编译通过
