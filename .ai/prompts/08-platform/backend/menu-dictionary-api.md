# 租户平台 — 菜单与字典 API

## 目标

重构菜单管理和数据字典 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `backend/error-codes.md`

---

## API 端点

### 菜单管理

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/menus` | 菜单列表（树形） |
| GET | `/api/menus/user-menus` | 当前用户菜单（按权限过滤） |
| POST | `/api/menus` | 创建菜单 |
| PUT | `/api/menus/{id}` | 更新菜单 |
| DELETE | `/api/menus/{id}` | 删除菜单 |
| PUT | `/api/menus/{id}/sort` | 调整排序 |
| PUT | `/api/menus/{id}/enable` | 启用 |
| PUT | `/api/menus/{id}/disable` | 禁用 |

### 数据字典

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/dictionaries` | 字典类型列表 |
| GET | `/api/dictionaries/{typeCode}` | 按类型获取字典项 |
| POST | `/api/dictionaries` | 创建字典 |
| PUT | `/api/dictionaries/{id}` | 更新字典 |
| DELETE | `/api/dictionaries/{id}` | 删除字典 |

---

## 业务规则

### 菜单
1. 菜单为树形结构，支持多级
2. 菜单与权限码关联
3. 前端根据用户权限过滤显示
4. 初始菜单树与 `docs/TenantPlatform/architecture.md` 一致

### 字典
1. 字典数据可缓存
2. 支持按类型编码查询
3. 缓存刷新策略与其他缓存一致

---

## 验收标准

- [ ] 菜单 CRUD + 树形查询正确
- [ ] 字典 CRUD + 按类型查询正确
- [ ] 菜单权限过滤正确
- [ ] 缓存生效
- [ ] 编译通过
