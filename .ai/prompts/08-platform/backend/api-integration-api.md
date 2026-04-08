# 租户平台 — API 集成管理

## 目标

重构 API 密钥管理 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/security.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/tenant-api-keys` | 密钥列表 |
| GET | `/api/tenant-api-keys/{id}` | 密钥详情 |
| POST | `/api/tenant-api-keys` | 创建密钥 |
| DELETE | `/api/tenant-api-keys/{id}` | 删除密钥 |
| PUT | `/api/tenant-api-keys/{id}/disable` | 禁用密钥 |
| PUT | `/api/tenant-api-keys/{id}/regenerate` | 重新生成密钥 |

---

## 业务规则

1. API Key 创建时生成随机密钥
2. 密钥只在创建时返回完整值，之后只显示前 8 位
3. 密钥存储使用哈希
4. 支持按租户查询

---

## 验收标准

- [ ] CRUD 完整
- [ ] 密钥安全存储
- [ ] 禁用/重新生成正确
- [ ] 编译通过
