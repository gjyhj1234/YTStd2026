# 租户平台 — 平台角色管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 其他业务模块提示词必须达到同样的详细程度。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-3 |
| 所属阶段 | 层级 2：业务页面层 |
| 依赖任务 | F1-1 主布局 |
| 预计文件数 | 10+ 个（含语言文件） |
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
- `.ai/prompts/08-platform/backend/platform-role-api.md` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount sort` | 角色列表远程分页与排序 |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 新增/编辑表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 新增/编辑/权限绑定/成员绑定弹窗 |
| DxTreeView | `DxTreeView showCheckBoxesMode selectNodesRecursive searchEnabled selectionMode multiple` | 权限绑定树（带级联勾选、搜索、计数） |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |
| DxDropDownButton | `DxDropDownButton items display-expr key-expr item-click drop-down-options` | 操作列「更多」菜单 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `PlatformRoleEndpoints.cs` 中的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 角色列表 | GET | `/api/platform-roles?page=1&pageSize=20&keyword=&status=&sortField=&sortOrder=` | - | `ApiResult<PagedResult<PlatformRoleRepDTO>>` |
| 角色详情 | GET | `/api/platform-roles/{id}` | - | `ApiResult<PlatformRoleRepDTO>` |
| 创建角色 | POST | `/api/platform-roles` | `{ Code, Name, Description }` | `ApiResult<long>` |
| 更新角色 | PUT | `/api/platform-roles/{id}` | `{ Name, Description }` | `ApiResult` |
| 删除角色 | DELETE | `/api/platform-roles/{id}` | - | `ApiResult` |
| 启用角色 | PUT | `/api/platform-roles/{id}/enable` | - | `ApiResult` |
| 禁用角色 | PUT | `/api/platform-roles/{id}/disable` | - | `ApiResult` |
| 检查编码 | GET | `/api/platform-roles/check-code-exists?code=xxx` | - | `ApiResult<bool>` |
| 获取角色权限 | GET | `/api/platform-roles/{id}/permissions` | - | `ApiResult<long[]>` |
| 绑定权限 | POST | `/api/platform-roles/{id}/permissions` | `{ PermissionIds: long[] }` | `ApiResult` |
| 绑定成员 | POST | `/api/platform-roles/{id}/members` | `{ UserIds: long[] }` | `ApiResult` |
| 全部角色 | GET | `/api/platform-roles/all` | - | `ApiResult<PlatformRoleRepDTO[]>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue` | 主页面 |
| 2 | `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/platform-roles/PlatformRolesView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/platform-roles.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/platform-roles.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题区 | `.page-header` > `.page-header-text` | `h2` 标题 + `p` 副标题 |
| 页面操作区 | `.page-header` > `.page-header-actions` | `FunctionDescriptionCard` + `OperationGuideDrawer` |
| 查询区 | `.search-area` > `.search-row` | 关键词（带标签） + 状态筛选（带标签） + 查询/重置按钮 |
| 工具栏 | `DxToolbar` | 新增 |
| 数据列表 | `DxDataGrid` + `CustomStore` | 远程分页+排序，`focused-row-enabled`，操作列使用 `DxDropDownButton`「更多」 |
| 新增/编辑弹窗 | `DxPopup` + `DxForm` | 角色编码、名称、描述 |
| 详情弹窗 | `DxPopup` | 只读字段展示 |
| 权限绑定弹窗 | `DxPopup` + `DxTreeView` | 快捷模板 + 搜索框 + 计数 + 级联勾选树 + 取消/保存 |
| 成员绑定弹窗 | `DxPopup` + `DxDataGrid` | 多选用户列表 + 取消/保存 |
| 表格区 | `DxDataGrid` + `CustomStore` | 角色列表 |
| 分页 | `DxDataGrid` 内置 `DxPager` + `DxPaging` | 远程分页 |
| 新增弹窗 | `DxPopup` + `DxForm` | 创建角色表单 |
| 编辑弹窗 | `DxPopup` + `DxForm` | 编辑角色表单 |
| 详情抽屉 | `DxPopup`（只读展示） | 角色详情 |
| 权限绑定弹窗 | `DxPopup` + `DxTreeList` | 权限树多选 |
| 成员绑定弹窗 | `DxPopup` + `DxDataGrid` | 用户多选 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入角色编码或名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| active | `$t('已启用')` |
| disabled | `$t('已禁用')` |

### 查询行为

| 行为 | 要求 |
|------|------|
| 回车搜索 | 在关键词输入框回车触发搜索 |
| 查询按钮 | `$t('查询')`，点击触发搜索 |
| 重置按钮 | `$t('重置')`，清空所有条件并重新加载 |
| 所有文本国际化 | 所有 label、placeholder、按钮文本均使用 `$t()` |

---

## 列表与分页

### 表格组件

使用 `DxDataGrid` + `CustomStore` 实现远程分页。

### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | 固定宽度 |
| 2 | Code | `$t('角色编码')` | auto | 是 | - | |
| 3 | Name | `$t('角色名称')` | auto | 是 | - | |
| 4 | Description | `$t('描述')` | auto | 否 | - | |
| 5 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 6 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 7 | - | `$t('操作')` | 480px | 否 | `actionCell` | 操作按钮列（7 个按钮） |

**所有 caption 必须使用 `:caption="$t('...')"` 绑定，禁止硬编码。**

### 分页配置

| 配置 | 值 |
|------|---|
| 支持分页 | 是 |
| 默认页大小 | 20 |
| 可选页大小 | `[10, 20, 50, 100]` |
| 显示总数 | 是（`showInfo: true`） |
| 显示页大小选择 | 是（`showPageSizeSelector: true`） |
| 显示导航按钮 | 是（`showNavigationButtons: true`） |
| 远程分页 | 是（CustomStore） |

### 状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| active | `$t('已启用')` | 绿色 `#52c41a` | `status-enabled` |
| disabled | `$t('已禁用')` | 红色 `#f5222d` | `status-disabled` |

### 空状态与加载

| 状态 | 显示 |
|------|------|
| 空数据 | `:no-data-text="$t('暂无数据')"` |
| 加载中 | `DxLoadPanel`（`visible` 绑定 `pageLoading`） |

---

## 操作按钮

### 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增 | `$t('新增')` | `add` | `PLATFORM_ROLE_CREATE` | 始终启用 | 打开新增弹窗 | 无 |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `PLATFORM_ROLE_VIEW` | 始终 | 打开详情抽屉 | 无 |
| 编辑 | `$t('编辑')` | `edit` | `PLATFORM_ROLE_UPDATE` | 始终 | 打开编辑弹窗 | 无 |
| 启用 | `$t('启用')` | `check` | `PLATFORM_ROLE_ENABLE` | `Status === 'disabled'` | 调用启用 API | `confirmAction('确认启用角色 {Name}')` |
| 禁用 | `$t('禁用')` | `close` | `PLATFORM_ROLE_DISABLE` | `Status === 'active'` | 调用禁用 API | `confirmAction('确认禁用角色 {Name}')` |
| 分配权限 | `$t('分配权限')` | `hierarchy` | `PLATFORM_ROLE_ASSIGN_PERMISSION` | 始终 | 打开权限绑定弹窗 | 无 |
| 分配成员 | `$t('分配成员')` | `group` | `PLATFORM_ROLE_ASSIGN_MEMBER` | 始终 | 打开成员绑定弹窗 | 无 |
| 删除 | `$t('删除')` | `trash` | `PLATFORM_ROLE_DELETE` | 始终 | 调用删除 API | `confirmDelete(row.Name)` |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const PLATFORM_ROLE_VIEW = 'platform.role.detail'
export const PLATFORM_ROLE_CREATE = 'platform.role.create'
export const PLATFORM_ROLE_UPDATE = 'platform.role.update'
export const PLATFORM_ROLE_DELETE = 'platform.role.delete'
export const PLATFORM_ROLE_ENABLE = 'platform.role.update'
export const PLATFORM_ROLE_DISABLE = 'platform.role.update'
export const PLATFORM_ROLE_ASSIGN_PERMISSION = 'platform.role.update'
export const PLATFORM_ROLE_ASSIGN_MEMBER = 'platform.role.update'
```

### 确认框文案（全部国际化）

| 操作 | 文案 key | 说明 |
|------|---------|------|
| 启用 | `'确认启用角色 {name}'` | 需在 common 语言文件中有对应翻译 |
| 禁用 | `'确认禁用角色 {name}'` | 同上 |
| 删除 | `confirmDelete(row.Name)` | 使用通用删除确认 |

### 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建 | `notifySuccess('创建成功')` |
| 更新 | `notifySuccess('更新成功')` |
| 启用 | `notifySuccess('启用成功')` |
| 禁用 | `notifySuccess('禁用成功')` |
| 删除 | `notifySuccess('删除成功')` |
| 权限保存 | `notifySuccess('保存成功')` |
| 成员保存 | `notifySuccess('保存成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 新增表单

**标题**：`$t('新增角色')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | Code | `$t('角色编码')` | DxTextBox | 是 | 2-50 | `^[a-zA-Z0-9_-]+$` 字母数字下划线连字符 | 是 async | `''` | - | - |
| 2 | Name | `$t('角色名称')` | DxTextBox | 是 | 2-50 | - | - | `''` | - | - |
| 3 | Description | `$t('描述')` | DxTextArea | 否 | 0-500 | - | - | `''` | - | - |

**唯一性校验**：

- Code：调用 `GET /api/platform-roles/check-code-exists?code=xxx`
- 新增时不传 `excludeId`
- DxForm async validation：

```typescript
{
  type: 'async',
  validationCallback: async (params) => {
    const exists = await checkCodeExists(params.value)
    return !exists
  },
  message: t('角色编码已存在')
}
```

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| Code | required | - | `$t('请输入角色编码')` |
| Code | stringLength | min: 2, max: 50 | `$t('角色编码长度 2-50 个字符')` |
| Code | pattern | `^[a-zA-Z0-9_-]+$` | `$t('角色编码仅允许字母、数字、下划线和连字符')` |
| Code | async | checkCodeExists | `$t('角色编码已存在')` |
| Name | required | - | `$t('请输入角色名称')` |
| Name | stringLength | min: 2, max: 50 | `$t('角色名称长度 2-50 个字符')` |
| Description | stringLength | max: 500 | `$t('描述长度不超过 500 个字符')` |

**提交行为**：

1. 提交前：调用 `formInstance.validate()`，不通过则阻止提交
2. 提交时：`submitting.value = true`，禁用提交按钮并显示 loading
3. 提交成功：关闭弹窗 → `notifySuccess('创建成功')` → 刷新列表
4. 提交失败：axios 拦截器自动显示错误 → 不关闭弹窗 → `submitting.value = false`

**弹窗按钮**：

| 按钮 | 文本 | 位置 | 行为 |
|------|------|------|------|
| 提交 | `$t('确定')` | 弹窗底部右侧 | 提交表单，`submitting` 时 disabled + loading |
| 取消 | `$t('取消')` | 弹窗底部左侧 | 关闭弹窗，重置表单 |

### 编辑表单

**标题**：`$t('编辑角色')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | Code | `$t('角色编码')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 2 | Name | `$t('角色名称')` | DxTextBox | 是 | 2-50 | - | - | 从数据加载 | - | - |
| 3 | Description | `$t('描述')` | DxTextArea | 否 | 0-500 | - | - | 从数据加载 | - | - |

**注意**：编辑时 Code 字段为只读（`disabled: true`），不可修改。

**提交行为**：同新增表单，成功提示为 `notifySuccess('更新成功')`。

### 详情展示

**标题**：`$t('角色详情')`
**组件**：`DxPopup`（只读展示）

展示字段：

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | Code | `$t('角色编码')` | - |
| 3 | Name | `$t('角色名称')` | - |
| 4 | Description | `$t('描述')` | - |
| 5 | Status | `$t('状态')` | 状态标签 |
| 6 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 权限绑定弹窗

**标题**：`$t('分配权限')`
**组件**：`DxPopup`（`width: 800`，`height: 600`）+ `DxTreeList`

- 使用 `DxTreeList` 展示权限树（从 `GET /api/platform-permissions/tree` 获取）
- 扁平化后端返回的树形数据（递归 flattenTree）
- `key-expr="Id"`，`parent-id-expr="ParentId"`
- `DxSelection` mode="multiple" recursive=true
- 预加载当前角色已绑定权限（`GET /api/platform-roles/{id}/permissions` → `selectedRowKeys`）
- 保存时调用 `POST /api/platform-roles/{id}/permissions`，请求体 `{ PermissionIds: selectedRowKeys }`
- 保存成功：`notifySuccess('保存成功')` → 关闭弹窗

**DxTreeList 列**：

| data-field | caption | 宽度 |
|-----------|---------|:----:|
| Name | `$t('权限名称')` | auto |
| Code | `$t('权限编码')` | auto |
| PermissionType | `$t('权限类型')` | 120px |

### 成员绑定弹窗

**标题**：`$t('分配成员')`
**组件**：`DxPopup`（`width: 800`，`height: 600`）+ `DxDataGrid`

- 使用 `DxDataGrid` 展示全部平台用户
- `DxSelection` mode="multiple"，showCheckBoxesMode="always"
- 预加载当前角色已绑定成员（如有对应 API）
- 保存时调用 `POST /api/platform-roles/{id}/members`，请求体 `{ UserIds: selectedRowKeys }`
- 保存成功：`notifySuccess('保存成功')` → 关闭弹窗

---

## 类型定义

```typescript
// src/types/platform-roles.ts

/** 角色创建请求 */
export interface CreatePlatformRoleReqDTO {
  Code: string
  Name: string
  Description?: string
}

/** 角色更新请求 */
export interface UpdatePlatformRoleReqDTO {
  Name?: string
  Description?: string
}

/** 角色响应 */
export interface PlatformRoleRepDTO {
  Id: number
  Code: string
  Name: string
  Description?: string
  Status: string
  CreatedAt: string
}

/** 角色权限绑定请求 */
export interface RolePermissionBindReqDTO {
  PermissionIds: number[]
}

/** 角色成员绑定请求 */
export interface RoleMemberBindReqDTO {
  UserIds: number[]
}
```

---

## 国际化要求

### 组件级 key（放入 `PlatformRolesView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 平台角色管理 | 平台角色管理 | Platform Role Management | プラットフォームロール管理 | Pengurusan Peranan Platform | 平台角色管理 |
| 管理平台级角色，包括创建、编辑、启用/禁用、权限分配和成员管理 | (同key) | Manage platform roles including create, edit, enable/disable, permission assignment, and member management | プラットフォームロールの作成、編集、有効/無効、権限割り当て、メンバー管理を行います | Urus peranan platform termasuk cipta, edit, aktif/nyahaktif, tugasan kebenaran dan pengurusan ahli | 管理平台級角色，包括建立、編輯、啟用/停用、權限分配和成員管理 |
| 请输入角色编码或名称 | 请输入角色编码或名称 | Enter role code or name | ロールコードまたは名前を入力 | Masukkan kod peranan atau nama | 請輸入角色編碼或名稱 |
| 请选择状态 | 请选择状态 | Select status | ステータスを選択 | Pilih status | 請選擇狀態 |
| 角色编码 | 角色编码 | Role Code | ロールコード | Kod Peranan | 角色編碼 |
| 角色名称 | 角色名称 | Role Name | ロール名 | Nama Peranan | 角色名稱 |
| 描述 | 描述 | Description | 説明 | Penerangan | 描述 |
| 新增角色 | 新增角色 | Create Role | ロール作成 | Cipta Peranan | 新增角色 |
| 编辑角色 | 编辑角色 | Edit Role | ロール編集 | Edit Peranan | 編輯角色 |
| 角色详情 | 角色详情 | Role Details | ロール詳細 | Butiran Peranan | 角色詳情 |
| 分配权限 | 分配权限 | Assign Permissions | 権限割り当て | Tugasan Kebenaran | 分配權限 |
| 分配成员 | 分配成员 | Assign Members | メンバー割り当て | Tugasan Ahli | 分配成員 |
| 请输入角色编码 | 请输入角色编码 | Enter role code | ロールコードを入力 | Masukkan kod peranan | 請輸入角色編碼 |
| 请输入角色名称 | 请输入角色名称 | Enter role name | ロール名を入力 | Masukkan nama peranan | 請輸入角色名稱 |
| 角色编码长度 2-50 个字符 | 角色编码长度 2-50 个字符 | Role code must be 2-50 characters | ロールコードは2-50文字 | Kod peranan mestilah 2-50 aksara | 角色編碼長度 2-50 個字元 |
| 角色编码仅允许字母、数字、下划线和连字符 | 角色编码仅允许字母、数字、下划线和连字符 | Role code can only contain letters, numbers, underscores and hyphens | ロールコードは英数字、アンダースコア、ハイフンのみ | Kod peranan hanya boleh mengandungi huruf, nombor, garis bawah dan sempang | 角色編碼僅允許字母、數字、底線和連字符 |
| 角色编码已存在 | 角色编码已存在 | Role code already exists | ロールコードは既に存在します | Kod peranan sudah wujud | 角色編碼已存在 |
| 角色名称长度 2-50 个字符 | 角色名称长度 2-50 个字符 | Role name must be 2-50 characters | ロール名は2-50文字 | Nama peranan mestilah 2-50 aksara | 角色名稱長度 2-50 個字元 |
| 描述长度不超过 500 个字符 | 描述长度不超过 500 个字符 | Description must not exceed 500 characters | 説明は500文字以内 | Penerangan mestilah tidak melebihi 500 aksara | 描述長度不超過 500 個字元 |
| 权限名称 | 权限名称 | Permission Name | 権限名 | Nama Kebenaran | 權限名稱 |
| 权限编码 | 权限编码 | Permission Code | 権限コード | Kod Kebenaran | 權限編碼 |
| 权限类型 | 权限类型 | Permission Type | 権限タイプ | Jenis Kebenaran | 權限類型 |
| 确认启用角色 {name} | 确认启用角色 {name} | Confirm enable role {name} | ロール {name} を有効にしますか | Sahkan aktifkan peranan {name} | 確認啟用角色 {name} |
| 确认禁用角色 {name} | 确认禁用角色 {name} | Confirm disable role {name} | ロール {name} を無効にしますか | Sahkan nyahaktifkan peranan {name} | 確認停用角色 {name} |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`编辑`、`删除`、`查看`、`启用`、`禁用`、`确定`、`取消`、`操作`、`ID`、`状态`、`创建时间`、`已启用`、`已禁用`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`更新成功`、`删除成功`、`启用成功`、`禁用成功`、`保存成功`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"平台角色管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **查询区**包含：关键词输入框、状态下拉
- [ ] **表格列**包含：ID、角色编码、角色名称、描述、状态、创建时间、操作
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 远程分页通过 CustomStore 实现
- [ ] **工具栏**包含：新增按钮
- [ ] **行操作**包含：查看、编辑、启用、禁用、分配权限、分配成员、删除（共 7 个）
- [ ] **新增弹窗**包含字段：角色编码、角色名称、描述
- [ ] **编辑弹窗**包含字段：角色编码（只读）、角色名称、描述
- [ ] **详情抽屉**展示：ID、角色编码、角色名称、描述、状态、创建时间
- [ ] **权限绑定弹窗**使用 DxTreeList 展示权限树，预加载已绑定权限
- [ ] **成员绑定弹窗**使用 DxDataGrid 展示用户列表，支持多选

### P1 — 业务规则完整性

- [ ] 角色编码 `required` 验证
- [ ] 角色编码 `stringLength`（2-50）验证
- [ ] 角色编码 `pattern`（字母数字下划线连字符）验证
- [ ] 角色编码 `async` 唯一性验证（新增时）
- [ ] 编辑时角色编码字段 disabled（不可修改）
- [ ] 角色名称 `required` 验证
- [ ] 角色名称 `stringLength`（2-50）验证
- [ ] 描述 `stringLength`（max: 500）验证
- [ ] 每个操作按钮有权限码控制（`v-if="perm.has(...)"`)
- [ ] 启用按钮仅在 `Status === 'disabled'` 时显示
- [ ] 禁用按钮仅在 `Status === 'active'` 时显示
- [ ] 删除有 `confirmDelete` 确认
- [ ] 启用/禁用有 `confirmAction` 确认
- [ ] 权限绑定弹窗预加载当前已绑定权限 ID
- [ ] 权限绑定保存调用正确 API
- [ ] 成员绑定弹窗展示用户列表
- [ ] 成员绑定保存调用正确 API
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' PlatformRolesView.vue | grep -v ':caption'` 结果为 0
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] `grep -rn "notifySuccess(t(" PlatformRolesView.vue` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有状态显示值已国际化
- [ ] 所有确认框文案已国际化
- [ ] 所有成功提示已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
