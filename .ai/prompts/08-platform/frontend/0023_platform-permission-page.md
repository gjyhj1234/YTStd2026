# 租户平台 — 平台权限管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 权限管理为**只读展示**页面（权限由种子数据维护），无 CRUD 操作。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-4 |
| 所属阶段 | 层级 2：业务页面层 |
| 依赖任务 | F1-1 主布局 |
| 预计文件数 | 8 个（含语言文件） |
| 新前端项目路径 | `src/WebTenantPlatfrom` |

---

## 前置阅读

- `.ai/prompts/03-frontend/00-governance.md` — 前端总治理
- `.ai/prompts/03-frontend/04-devextreme-templates.md` — DevExtreme 规范
- `.ai/prompts/03-frontend/05-axios-standard.md` — axios 规范
- `.ai/prompts/03-frontend/06-i18n-execution.md` — i18n 规范
- `.ai/prompts/03-frontend/03-anti-patterns.md` — 反模式清单
- `.ai/rules/frontend.md` — 前端开发规范
- `.ai/rules/i18n.md` — 国际化规范
- `.github/copilot-instructions.md` — 关键编码约束（第 7-13 条为前端约束）
- `.ai/prompts/08-platform/backend/platform-permission-api.md` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxTreeList | `DxTreeList key-expr parent-id-expr auto-expand-all column-auto-width filter-mode` | 权限树展示 |
| DxTreeList | `DxTreeList searchPanel visible search-mode search-expr` | 权限树搜索 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 关键词搜索输入 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 加载状态 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `PlatformPermissionEndpoints.cs` 中的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 权限树 | GET | `/api/platform-permissions/tree` | - | `ApiResult<PlatformPermissionRepDTO[]>` |
| 权限列表（平铺） | GET | `/api/platform-permissions` | - | `ApiResult<PlatformPermissionRepDTO[]>` |
| 权限详情 | GET | `/api/platform-permissions/{id}` | - | `ApiResult<PlatformPermissionRepDTO>` |
| 按编码查询 | GET | `/api/platform-permissions/code/{code}` | - | `ApiResult<PlatformPermissionRepDTO>` |

> 本页面主要使用 `/tree` 端点获取完整权限树。权限数据由种子数据维护，前端只读展示。

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue` | 主页面 |
| 2 | `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/platform-permissions/PlatformPermissionsView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/platform-permissions.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/platform-permissions.ts` | 类型定义 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('平台权限管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('查看平台权限树层级结构，权限数据由系统维护')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| 搜索区 | DxTextBox | 关键词搜索（按编码或名称过滤） |
| 权限树区 | `DxTreeList` | 权限树层级展示 |
| 加载状态 | `DxLoadPanel` | 数据加载中 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### 搜索条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入权限编码或名称')` |

### 搜索行为

| 行为 | 要求 |
|------|------|
| 实时搜索 | 输入关键词时实时过滤权限树 |
| 子节点匹配展示父链 | 搜索匹配子节点时自动展示其所有父节点 |
| 空关键词 | 恢复完整权限树展示 |
| 大小写不敏感 | 搜索不区分大小写 |

> **推荐方案**：优先使用 DxTreeList 内置的 `searchPanel` 功能，或使用 `filterMode` + 自定义过滤。通过 dxdocs 查阅 DxTreeList 的 `searchPanel` 属性确认最佳实现方式。

---

## 列表与分页

### 树形组件

使用 `DxTreeList`（非 DxDataGrid），数据来源 `GET /api/platform-permissions/tree`。

后端返回嵌套树形数据（`Children` 字段），需递归扁平化后传入 DxTreeList：

```typescript
function flattenTree(nodes: PlatformPermissionRepDTO[], result: FlatPermission[] = []): FlatPermission[] {
  for (const node of nodes) {
    result.push({ Id: node.Id, Code: node.Code, Name: node.Name, PermissionType: node.PermissionType, ParentId: node.ParentId, Path: node.Path, Method: node.Method })
    if (node.Children && node.Children.length > 0) {
      flattenTree(node.Children, result)
    }
  }
  return result
}
```

### DxTreeList 关键配置

```
key-expr="Id"
parent-id-expr="ParentId"
:auto-expand-all="true"
:show-borders="true"
:column-auto-width="true"
:hover-state-enabled="true"
```

### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|--------|------|
| 1 | Id | `$t('ID')` | 80px | - | 固定宽度 |
| 2 | Code | `$t('权限编码')` | auto | - | |
| 3 | Name | `$t('权限名称')` | auto | - | |
| 4 | PermissionType | `$t('权限类型')` | 120px | `typeCell` | 彩色标签 |
| 5 | Path | `$t('路径')` | auto | - | API 权限的请求路径 |
| 6 | Method | `$t('HTTP方法')` | 100px | `methodCell` | HTTP 方法标签 |

**所有 caption 必须使用 `:caption="$t('...')"` 绑定，禁止硬编码。**

### 权限类型标签颜色

| 类型值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| menu | `$t('菜单权限')` | 蓝色 `#1890ff` | `type-menu` |
| api | `$t('API权限')` | 绿色 `#52c41a` | `type-api` |
| action | `$t('操作权限')` | 橙色 `#fa8c16` | `type-action` |
| data | `$t('数据权限')` | 紫色 `#722ed1` | `type-data` |

### HTTP 方法标签颜色

| 方法 | 标签颜色 |
|------|---------|
| GET | 绿色 `#52c41a` |
| POST | 蓝色 `#1890ff` |
| PUT | 橙色 `#fa8c16` |
| DELETE | 红色 `#f5222d` |

### 空状态与加载

| 状态 | 显示 |
|------|------|
| 空数据 | `$t('暂无权限数据')` |
| 加载中 | `DxLoadPanel`（`visible` 绑定 `loading`） |

---

## 操作按钮

本页面为**只读展示**，无工具栏按钮、无行操作按钮、无新增/编辑/删除弹窗。

---

## 表单功能

本页面无表单功能。

---

## 类型定义

```typescript
// src/types/platform-permissions.ts

/** 权限树节点（后端返回嵌套结构） */
export interface PlatformPermissionRepDTO {
  Id: number
  Code: string
  Name: string
  PermissionType: string
  ParentId: number | null
  Path?: string
  Method?: string
  Children?: PlatformPermissionRepDTO[]
}

/** 扁平化权限节点（用于 DxTreeList） */
export interface FlatPermission {
  Id: number
  Code: string
  Name: string
  PermissionType: string
  ParentId: number | null
  Path?: string
  Method?: string
}
```

---

## 国际化要求

### 组件级 key（放入 `PlatformPermissionsView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 平台权限管理 | 平台权限管理 | Platform Permission Management | プラットフォーム権限管理 | Pengurusan Kebenaran Platform | 平台權限管理 |
| 查看平台权限树层级结构，权限数据由系统维护 | (同key) | View platform permission tree structure, permission data is maintained by the system | プラットフォーム権限ツリーの階層構造を表示します。権限データはシステムで管理されます | Lihat struktur hierarki kebenaran platform, data kebenaran diselenggara oleh sistem | 查看平台權限樹層級結構，權限資料由系統維護 |
| 请输入权限编码或名称 | 请输入权限编码或名称 | Enter permission code or name | 権限コードまたは名前を入力 | Masukkan kod kebenaran atau nama | 請輸入權限編碼或名稱 |
| 权限编码 | 权限编码 | Permission Code | 権限コード | Kod Kebenaran | 權限編碼 |
| 权限名称 | 权限名称 | Permission Name | 権限名 | Nama Kebenaran | 權限名稱 |
| 权限类型 | 权限类型 | Permission Type | 権限タイプ | Jenis Kebenaran | 權限類型 |
| 路径 | 路径 | Path | パス | Laluan | 路徑 |
| HTTP方法 | HTTP方法 | HTTP Method | HTTPメソッド | Kaedah HTTP | HTTP方法 |
| 菜单权限 | 菜单权限 | Menu Permission | メニュー権限 | Kebenaran Menu | 選單權限 |
| API权限 | API权限 | API Permission | API権限 | Kebenaran API | API權限 |
| 操作权限 | 操作权限 | Action Permission | 操作権限 | Kebenaran Operasi | 操作權限 |
| 数据权限 | 数据权限 | Data Permission | データ権限 | Kebenaran Data | 資料權限 |
| 暂无权限数据 | 暂无权限数据 | No permission data | 権限データなし | Tiada data kebenaran | 暫無權限資料 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`关键词`、`ID`、`暂无数据`、`功能说明`、`操作指南`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"平台权限管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **搜索区**包含：关键词输入框
- [ ] 权限树正确展示层级结构（DxTreeList）
- [ ] 权限树自动展开所有节点
- [ ] **列**包含：ID、权限编码、权限名称、权限类型、路径、HTTP方法
- [ ] 搜索过滤正确（子节点匹配时展示父节点链）
- [ ] 权限类型标签颜色区分正确（菜单=蓝、API=绿、操作=橙、数据=紫）
- [ ] HTTP 方法标签颜色区分正确（GET=绿、POST=蓝、PUT=橙、DELETE=红）
- [ ] 加载中显示 DxLoadPanel
- [ ] 空数据显示"暂无权限数据"

### P1 — 业务规则完整性

- [ ] 页面为只读展示，无新增/编辑/删除操作
- [ ] 后端树形数据正确扁平化
- [ ] DxTreeList key-expr 和 parent-id-expr 设置正确
- [ ] 搜索不区分大小写
- [ ] 搜索结果保持树形层级关系

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' PlatformPermissionsView.vue | grep -v ':caption'` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有权限类型显示值已国际化
- [ ] 所有搜索字段 placeholder 已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet`
