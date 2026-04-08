# 租户平台 — 租户配置 API

## 目标

重构系统配置和功能开关 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `backend/error-codes.md`

---

## API 端点

### 系统配置

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/system-configs` | 配置列表 |
| GET | `/api/system-configs/{key}` | 获取配置 |
| PUT | `/api/system-configs/{key}` | 更新配置 |

### 功能开关

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/feature-flags` | 开关列表 |
| GET | `/api/feature-flags/{key}` | 获取开关 |
| PUT | `/api/feature-flags/{key}` | 更新开关 |
| PUT | `/api/feature-flags/{key}/enable` | 启用 |
| PUT | `/api/feature-flags/{key}/disable` | 禁用 |

---

## 业务规则

1. 配置数据可缓存
2. 配置变更记录审计日志
3. 功能开关支持租户级和全局级

---

## 验收标准

- [ ] 配置 CRUD 正确
- [ ] 功能开关 CRUD 正确
- [ ] 缓存生效
- [ ] 编译通过
