# 租户平台 — 平台运营 API

## 目标

重构平台运营统计 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/platform-operations/dashboard` | 仪表盘统计 |
| GET | `/api/platform-operations/tenant-statistics` | 租户统计 |
| GET | `/api/platform-operations/user-statistics` | 用户统计 |
| GET | `/api/platform-operations/subscription-statistics` | 订阅统计 |
| GET | `/api/platform-operations/revenue-statistics` | 收入统计 |

---

## 仪表盘数据

- 总租户数 / 活跃租户数
- 总用户数 / 在线用户数
- 今日新增租户
- 本月收入
- 订阅到期预警

---

## 验收标准

- [ ] 各统计接口正确
- [ ] 仪表盘数据完整
- [ ] 编译通过
