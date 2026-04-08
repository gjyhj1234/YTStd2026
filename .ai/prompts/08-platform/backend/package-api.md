# 租户平台 — 套餐管理 API

## 目标

重构套餐管理 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/api-design.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/packages` | 套餐列表 |
| GET | `/api/packages/{id}` | 套餐详情 |
| POST | `/api/packages` | 创建套餐 |
| PUT | `/api/packages/{id}` | 更新套餐 |
| DELETE | `/api/packages/{id}` | 删除套餐 |
| PUT | `/api/packages/{id}/publish` | 发布套餐 |
| PUT | `/api/packages/{id}/unpublish` | 下架套餐 |
| GET | `/api/packages/check-code-exists` | 检查编码唯一 |

---

## 业务规则

1. 套餐编码唯一
2. 已发布的套餐不可删除（需先下架）
3. 套餐包含资源配额模板（用户数、存储空间、API 调用数等）
4. 套餐支持定价（月付/年付）

---

## 验收标准

- [ ] CRUD 完整
- [ ] 发布/下架状态流转正确
- [ ] 编码唯一性检查
- [ ] 编译通过
