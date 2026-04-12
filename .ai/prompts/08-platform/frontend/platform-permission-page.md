# 租户平台 — 平台权限管理页面

## 目标

重构平台权限管理前端页面，展示权限树层级结构（只读），支持搜索过滤。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/rules/i18n.md`
- `.ai/prompts/03-frontend/page-module.md`
- `.ai/prompts/08-platform/frontend/refactoring-master.md`
- `.ai/prompts/08-platform/backend/platform-permission-api.md`
- `.github/copilot-instructions.md`

---

## DevExpress 文档查阅（强制前置步骤）

```
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeList parent-id-expr key-expr auto-expand-all filter-mode")
devexpress_docs_search(technologies: ["Vue"], question: "DxLoadPanel visible position usage in Vue")
```

---

## API 端点（精确匹配）

| 操作 | HTTP 方法 | URL | 响应体 |
|------|----------|-----|--------|
| 权限树 | GET | /api/platform-permissions/tree | ApiResult<PlatformPermissionRepDTO[]> |

（权限数据由种子数据维护，前端只读展示）

---

## 必须产出的文件

1. `src/views/platform-permissions/PlatformPermissionsView.vue`
2. `src/views/platform-permissions/PlatformPermissionsView.vue.zh-CN.json`
3. `src/views/platform-permissions/PlatformPermissionsView.vue.en-US.json`
4. `src/views/platform-permissions/PlatformPermissionsView.vue.ja-JP.json`
5. `src/views/platform-permissions/PlatformPermissionsView.vue.ms-MY.json`
6. `src/views/platform-permissions/PlatformPermissionsView.vue.zh-TW.json`

---

## 页面功能

### 1. 权限树展示（DxTreeList）

- 使用 DxTreeList（非 DxDataGrid），支持层级展开
- 扁平化后端返回的树形数据（递归 flattenTree）
- **所有 DxColumn 的 caption 必须使用 `:caption="$t('...')"` 绑定**

```vue
<DxTreeList
  :data-source="treeData"
  :show-borders="true"
  :column-auto-width="true"
  :hover-state-enabled="true"
  key-expr="Id"
  parent-id-expr="ParentId"
  :auto-expand-all="true"
>
  <DxColumn data-field="Id" :caption="$t('ID')" :width="80" />
  <DxColumn data-field="Code" :caption="$t('权限编码')" />
  <DxColumn data-field="Name" :caption="$t('权限名称')" />
  <DxColumn data-field="PermissionType" :caption="$t('权限类型')" cell-template="typeCell" :width="120" />
  <DxColumn data-field="Path" :caption="$t('路径')" />
  <DxColumn data-field="Method" :caption="$t('HTTP方法')" :width="100" />
</DxTreeList>
```

### 2. 搜索过滤

- 关键词搜索按权限编码或名称过滤
- 匹配子节点时自动展示其父节点链

### 3. 权限类型显示

- Menu → 菜单权限（蓝色标签）
- Api → API权限（绿色标签）
- Operation → 操作权限（橙色标签）
- Data → 数据权限（紫色标签）

---

## Mock 数据要求

权限树 Mock 必须包含**至少 3 级层级**，每级至少 2 个节点。示例结构：

```
├── 平台管理 (Menu)
│   ├── 用户管理 (Menu)
│   │   ├── platform.user.list (Api, GET /api/platform-users)
│   │   ├── platform.user.create (Api, POST /api/platform-users)
│   ├── 角色管理 (Menu)
│   │   ├── platform.role.list (Api, GET /api/platform-roles)
│   │   ├── platform.role.create (Api, POST /api/platform-roles)
```

---

## 验收标准（可执行检查）

- [ ] 权限树正确展示层级结构
- [ ] `grep -rn 'caption="' PlatformPermissionsView.vue | grep -v ':caption'` 结果为 0
- [ ] 搜索过滤正确（子节点匹配时展示父节点链）
- [ ] 权限类型标签颜色区分正确
- [ ] 加载中显示 DxLoadPanel
- [ ] 空数据显示 no-data-text
- [ ] 5 个语言文件已创建且 key 一致
- [ ] 功能说明卡片和操作指引完整
- [ ] `npm run build` 通过
