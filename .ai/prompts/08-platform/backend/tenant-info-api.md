# 租户平台 — 租户信息 API

## 目标

重构租户信息管理（域名绑定、分组、标签等）。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/api-design.md`
- `backend/error-codes.md`

---

## API 端点

### 租户分组

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/tenant-groups` | 分组列表 |
| POST | `/api/tenant-groups` | 创建分组 |
| PUT | `/api/tenant-groups/{id}` | 更新分组 |
| DELETE | `/api/tenant-groups/{id}` | 删除分组 |

### 租户域名

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/tenants/{id}/domains` | 域名列表 |
| POST | `/api/tenants/{id}/domains` | 绑定域名 |
| DELETE | `/api/tenants/{id}/domains/{domainId}` | 解绑域名 |

### 租户标签

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/tenant-tags` | 标签列表 |
| POST | `/api/tenant-tags` | 创建标签 |
| DELETE | `/api/tenant-tags/{id}` | 删除标签 |
| PUT | `/api/tenants/{id}/tags` | 设置租户标签 |

---

## 验收标准

- [ ] 分组、域名、标签 CRUD 完整
- [ ] 域名唯一性检查
- [ ] 编译通过
