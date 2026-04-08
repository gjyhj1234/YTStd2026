# 租户平台 — 通知系统 API

## 目标

重构通知系统 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/notification-templates` | 通知模板列表 |
| GET | `/api/notification-templates/{id}` | 模板详情 |
| POST | `/api/notification-templates` | 创建模板 |
| PUT | `/api/notification-templates/{id}` | 更新模板 |
| DELETE | `/api/notification-templates/{id}` | 删除模板 |
| POST | `/api/notifications/send` | 发送通知 |
| GET | `/api/notifications` | 通知列表 |

---

## 业务规则

1. 通知模板使用整形 Code 标识（不含文本）
2. 通知内容由前端根据 Code 翻译
3. 支持站内通知

---

## 验收标准

- [ ] 模板 CRUD 完整
- [ ] 通知发送正确
- [ ] 使用整形 Code（后端零文本）
- [ ] 编译通过
