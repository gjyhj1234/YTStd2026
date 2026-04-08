# 租户平台 — 订阅管理 API

## 目标

重构订阅管理 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/subscriptions` | 订阅列表 |
| GET | `/api/subscriptions/{id}` | 订阅详情 |
| POST | `/api/subscriptions` | 创建订阅 |
| PUT | `/api/subscriptions/{id}/renew` | 续费 |
| PUT | `/api/subscriptions/{id}/upgrade` | 升级套餐 |
| PUT | `/api/subscriptions/{id}/cancel` | 取消订阅 |
| GET | `/api/tenants/{id}/subscription` | 获取租户当前订阅 |

---

## 业务规则

1. 一个租户同一时间只有一个有效订阅
2. 订阅到期自动暂停租户（由后台任务处理）
3. 升级即时生效，原订阅到期后切换
4. 取消订阅不退款，到期后失效

---

## 验收标准

- [ ] CRUD 完整
- [ ] 续费/升级/取消逻辑正确
- [ ] 编译通过
