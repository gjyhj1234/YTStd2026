# 租户平台 — 租户信息管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块包含三个子模块：租户分组管理（树形）、租户域名管理、租户标签管理。建议使用 Tab 页切换。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-6 |
| 所属阶段 | 层级 2：业务页面层 |
| 依赖任务 | F1-1 主布局、F2-5 租户管理 |
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
- `.ai/prompts/08-platform/backend/tenant-info-api.md` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxTabPanel | `DxTabPanel items selected-index title template slot` | 三子模块 Tab 切换 |
| DxTreeList | `DxTreeList key-expr parent-id-expr data-source columns editing` | 分组树管理 |
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 标签列表远程分页 |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 新增/编辑表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 新增/编辑弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 父级分组选择、域名类型选择 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |
| DxTagBox | `DxTagBox data-source value display-expr value-expr` | 租户标签绑定多选 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `TenantInfoEndpoints.cs` 中的路由注册。

### 租户分组

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 分组树 | GET | `/api/tenant-groups/tree` | - | `ApiResult<List<TenantGroupRepDTO>>` |
| 分组列表（平铺） | GET | `/api/tenant-groups` | - | `ApiResult<List<TenantGroupRepDTO>>` |
| 创建分组 | POST | `/api/tenant-groups` | `CreateTenantGroupReqDTO` | `ApiResult` |
| 更新分组 | PUT | `/api/tenant-groups/{id}` | `UpdateTenantGroupReqDTO` | `ApiResult` |
| 删除分组 | DELETE | `/api/tenant-groups/{id}` | - | `ApiResult` |

### 租户域名

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 域名列表 | GET | `/api/tenants/{tenantRefId}/domains` | - | `ApiResult<List<TenantDomainRepDTO>>` |
| 绑定域名 | POST | `/api/tenants/{tenantRefId}/domains` | `CreateTenantDomainReqDTO` | `ApiResult` |
| 解绑域名 | DELETE | `/api/tenants/{tenantRefId}/domains/{domainId}` | - | `ApiResult` |

### 租户标签

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 标签列表 | GET | `/api/tenant-tags?page=1&pageSize=20&keyword=` | - | `ApiResult<PagedResult<TenantTagRepDTO>>` |
| 创建标签 | POST | `/api/tenant-tags` | `CreateTenantTagReqDTO` | `ApiResult` |
| 删除标签 | DELETE | `/api/tenant-tags/{id}` | - | `ApiResult` |
| 批量绑定标签 | POST | `/api/tenant-tags/bind` | `{ TenantRefId, TagIds[] }` | `ApiResult` |
| 设置租户标签 | PUT | `/api/tenants/{id}/tags` | `{ TenantRefId, TagIds[] }` | `ApiResult` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/tenant-info/TenantInfoView.vue` | 主页面（Tab 容器） |
| 2 | `src/WebTenantPlatfrom/src/views/tenant-info/TenantInfoView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/tenant-info/TenantInfoView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/tenant-info/TenantInfoView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/tenant-info/TenantInfoView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/tenant-info/TenantInfoView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/tenant-info.ts` | API 封装（分组 + 域名 + 标签） |
| 8 | `src/WebTenantPlatfrom/src/types/tenant-info.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('租户信息管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理租户分组、域名绑定和标签')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| Tab 容器 | `DxTabPanel` | 三个 Tab：分组管理、域名管理、标签管理 |
| Tab 1 — 分组管理 | `DxTreeList` + `DxToolbar` | 树形分组列表 + 新增/编辑/删除 |
| Tab 2 — 域名管理 | 租户选择器 + `DxDataGrid` | 选择租户后展示域名列表 |
| Tab 3 — 标签管理 | `DxDataGrid` + `DxToolbar` | 标签列表 + 新增/删除 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## Tab 1：分组管理

### 树形表格

**组件**：`DxTreeList`
- `key-expr="Id"`，`parent-id-expr="ParentId"`
- 数据来源：`GET /api/tenant-groups/tree`
- 展示为树形结构，根节点 `ParentId = null`

### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 说明 |
|:----:|-----------|---------------------|:----:|------|
| 1 | Id | `$t('ID')` | 80px | |
| 2 | GroupCode | `$t('分组编码')` | 150px | |
| 3 | GroupName | `$t('分组名称')` | auto | |
| 4 | Description | `$t('描述')` | auto | |
| 5 | CreatedAt | `$t('创建时间')` | 180px | `yyyy-MM-dd HH:mm` |
| 6 | - | `$t('操作')` | 280px | 编辑、删除 |

### 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 |
|------|------|------|--------|---------|
| 新增分组 | `$t('新增分组')` | `add` | `TENANT_GROUP_CREATE` | 打开新增分组弹窗 |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 | 确认框 |
|------|------|------|--------|---------|--------|
| 编辑 | `$t('编辑')` | `edit` | `TENANT_GROUP_UPDATE` | 打开编辑弹窗 | 无 |
| 删除 | `$t('删除')` | `trash` | `TENANT_GROUP_DELETE` | 调用删除 API | `confirmDelete(row.GroupName)` |

### 新增分组表单

**标题**：`$t('新增分组')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 唯一性 | 默认值 |
|:----:|--------|------|------|:----:|---------|:------:|--------|
| 1 | GroupCode | `$t('分组编码')` | DxTextBox | 是 | 2-50 | - | `''` |
| 2 | GroupName | `$t('分组名称')` | DxTextBox | 是 | 2-100 | - | `''` |
| 3 | Description | `$t('描述')` | DxTextArea | 否 | 0-500 | - | `''` |
| 4 | ParentId | `$t('父级分组')` | DxSelectBox | 否 | - | - | `null` |

**父级分组选择**：从 `GET /api/tenant-groups`（平铺列表）获取，`display-expr="GroupName"`，`value-expr="Id"`。

**验证规则**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| GroupCode | required | - | `$t('请输入分组编码')` |
| GroupCode | stringLength | min: 2, max: 50 | `$t('分组编码长度 2-50 个字符')` |
| GroupName | required | - | `$t('请输入分组名称')` |
| GroupName | stringLength | min: 2, max: 100 | `$t('分组名称长度 2-100 个字符')` |
| Description | stringLength | max: 500 | `$t('描述长度不超过 500 个字符')` |

### 编辑分组表单

**标题**：`$t('编辑分组')`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 禁用条件 |
|:----:|--------|------|------|:----:|---------|
| 1 | GroupCode | `$t('分组编码')` | DxTextBox | - | **始终禁用**（只读） |
| 2 | GroupName | `$t('分组名称')` | DxTextBox | 否 | - |
| 3 | Description | `$t('描述')` | DxTextArea | 否 | - |
| 4 | ParentId | `$t('父级分组')` | DxSelectBox | 否 | - |

**注意**：编辑时 GroupCode 为只读。`UpdateTenantGroupReqDTO` 不含 GroupCode。

---

## Tab 2：域名管理

### 租户选择器

在域名 Tab 顶部提供一个 `DxSelectBox` 用于选择目标租户：
- 数据来源：使用租户列表 API 或缓存的租户列表
- `display-expr`：`TenantName`（或 `TenantCode + ' - ' + TenantName`）
- `value-expr`：`Id`
- placeholder：`$t('请选择租户')`
- 选择后自动加载该租户的域名列表

### 域名列表

**组件**：`DxDataGrid`（无需分页，域名数通常较少）

| 序号 | data-field | caption（i18n key） | 宽度 | 说明 |
|:----:|-----------|---------------------|:----:|------|
| 1 | Id | `$t('ID')` | 80px | |
| 2 | Domain | `$t('域名')` | auto | |
| 3 | DomainType | `$t('域名类型')` | 120px | |
| 4 | IsPrimary | `$t('是否主域名')` | 100px | 布尔值 |
| 5 | VerificationStatus | `$t('验证状态')` | 120px | 状态标签 |
| 6 | CreatedAt | `$t('创建时间')` | 180px | `yyyy-MM-dd HH:mm` |
| 7 | - | `$t('操作')` | 120px | 解绑 |

### 域名类型标签

| 类型值 | 显示文本 | 颜色 |
|:------:|---------|------|
| custom | `$t('自定义')` | 蓝色 `#1890ff` |
| system | `$t('系统')` | 灰色 `#8c8c8c` |

### 验证状态标签

| 状态值 | 显示文本 | 颜色 |
|:------:|---------|------|
| verified | `$t('已验证')` | 绿色 `#52c41a` |
| pending | `$t('待验证')` | 黄色 `#faad14` |
| failed | `$t('验证失败')` | 红色 `#f5222d` |

### 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 |
|------|------|------|--------|---------|---------|
| 绑定域名 | `$t('绑定域名')` | `add` | `TENANT_DOMAIN_CREATE` | 已选择租户 | 打开绑定域名弹窗 |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 | 确认框 |
|------|------|------|--------|---------|--------|
| 解绑 | `$t('解绑')` | `remove` | `TENANT_DOMAIN_DELETE` | 调用解绑 API | `confirmAction('确认解绑域名 {domain}')` |

### 绑定域名表单

**标题**：`$t('绑定域名')`
**组件**：`DxPopup`（`width: 500`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 格式校验 | 默认值 |
|:----:|--------|------|------|:----:|---------|--------|
| 1 | Domain | `$t('域名')` | DxTextBox | 是 | 域名格式 | `''` |
| 2 | DomainType | `$t('域名类型')` | DxSelectBox | 是 | - | `'custom'` |

**验证规则**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| Domain | required | - | `$t('请输入域名')` |
| Domain | pattern | 域名格式正则 | `$t('请输入有效的域名格式')` |

---

## Tab 3：标签管理

### 标签列表

**组件**：`DxDataGrid` + `CustomStore` 远程分页

### 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入标签关键词')` |

### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 说明 |
|:----:|-----------|---------------------|:----:|------|
| 1 | Id | `$t('ID')` | 80px | |
| 2 | TagKey | `$t('标签键')` | 150px | |
| 3 | TagValue | `$t('标签值')` | auto | |
| 4 | TagType | `$t('标签类型')` | 120px | |
| 5 | Description | `$t('描述')` | auto | |
| 6 | CreatedAt | `$t('创建时间')` | 180px | `yyyy-MM-dd HH:mm` |
| 7 | - | `$t('操作')` | 120px | 删除 |

### 标签类型标签

| 类型值 | 显示文本 | 颜色 |
|:------:|---------|------|
| custom | `$t('自定义')` | 蓝色 `#1890ff` |
| system | `$t('系统')` | 灰色 `#8c8c8c` |

### 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 |
|------|------|------|--------|---------|
| 新增标签 | `$t('新增标签')` | `add` | `TENANT_TAG_CREATE` | 打开新增标签弹窗 |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 | 确认框 |
|------|------|------|--------|---------|--------|
| 删除 | `$t('删除')` | `trash` | `TENANT_TAG_DELETE` | 调用删除 API | `confirmDelete(row.TagKey)` |

### 新增标签表单

**标题**：`$t('新增标签')`
**组件**：`DxPopup`（`width: 500`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 默认值 |
|:----:|--------|------|------|:----:|---------|--------|
| 1 | TagKey | `$t('标签键')` | DxTextBox | 是 | 2-50 | `''` |
| 2 | TagValue | `$t('标签值')` | DxTextBox | 是 | 1-200 | `''` |
| 3 | TagType | `$t('标签类型')` | DxSelectBox | 是 | - | `'custom'` |
| 4 | Description | `$t('描述')` | DxTextArea | 否 | 0-500 | `''` |

**验证规则**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TagKey | required | - | `$t('请输入标签键')` |
| TagKey | stringLength | min: 2, max: 50 | `$t('标签键长度 2-50 个字符')` |
| TagValue | required | - | `$t('请输入标签值')` |
| TagValue | stringLength | min: 1, max: 200 | `$t('标签值长度 1-200 个字符')` |
| Description | stringLength | max: 500 | `$t('描述长度不超过 500 个字符')` |

---

## 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const TENANT_GROUP_CREATE = 'tenant.create'
export const TENANT_GROUP_UPDATE = 'tenant.update'
export const TENANT_GROUP_DELETE = 'tenant.delete'
export const TENANT_DOMAIN_CREATE = 'tenant.create'
export const TENANT_DOMAIN_DELETE = 'tenant.delete'
export const TENANT_TAG_CREATE = 'tenant.create'
export const TENANT_TAG_DELETE = 'tenant.delete'
```

---

## 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建分组 | `notifySuccess('创建成功')` |
| 更新分组 | `notifySuccess('更新成功')` |
| 删除分组 | `notifySuccess('删除成功')` |
| 绑定域名 | `notifySuccess('绑定成功')` |
| 解绑域名 | `notifySuccess('解绑成功')` |
| 创建标签 | `notifySuccess('创建成功')` |
| 删除标签 | `notifySuccess('删除成功')` |
| 绑定标签 | `notifySuccess('绑定成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 类型定义

```typescript
// src/types/tenant-info.ts

/** 租户分组响应 */
export interface TenantGroupRepDTO {
  Id: number
  GroupCode: string
  GroupName: string
  Description?: string
  ParentId?: number
  Children?: TenantGroupRepDTO[]
  CreatedAt: string
}

/** 创建分组请求 */
export interface CreateTenantGroupReqDTO {
  GroupCode: string
  GroupName: string
  Description?: string
  ParentId?: number
}

/** 更新分组请求 */
export interface UpdateTenantGroupReqDTO {
  GroupName?: string
  Description?: string
  ParentId?: number
}

/** 租户域名响应 */
export interface TenantDomainRepDTO {
  Id: number
  TenantRefId: number
  Domain: string
  DomainType: string
  IsPrimary: boolean
  VerificationStatus: string
  CreatedAt: string
}

/** 创建域名请求 */
export interface CreateTenantDomainReqDTO {
  TenantRefId: number
  Domain: string
  DomainType: string
}

/** 租户标签响应 */
export interface TenantTagRepDTO {
  Id: number
  TagKey: string
  TagValue: string
  TagType: string
  Description?: string
  CreatedAt: string
}

/** 创建标签请求 */
export interface CreateTenantTagReqDTO {
  TagKey: string
  TagValue: string
  TagType: string
  Description?: string
}

/** 标签绑定请求 */
export interface TagBindReqDTO {
  TenantRefId: number
  TagIds: number[]
}
```

---

## 国际化要求

### 组件级 key（放入 `TenantInfoView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 租户信息管理 | 租户信息管理 | Tenant Information Management | テナント情報管理 | Pengurusan Maklumat Penyewa | 租戶資訊管理 |
| 管理租户分组、域名绑定和标签 | 管理租户分组、域名绑定和标签 | Manage tenant groups, domain bindings, and tags | テナントグループ、ドメインバインド、タグを管理 | Urus kumpulan penyewa, pengikatan domain dan tag | 管理租戶分組、域名綁定和標籤 |
| 分组管理 | 分组管理 | Group Management | グループ管理 | Pengurusan Kumpulan | 分組管理 |
| 域名管理 | 域名管理 | Domain Management | ドメイン管理 | Pengurusan Domain | 域名管理 |
| 标签管理 | 标签管理 | Tag Management | タグ管理 | Pengurusan Tag | 標籤管理 |
| 分组编码 | 分组编码 | Group Code | グループコード | Kod Kumpulan | 分組編碼 |
| 分组名称 | 分组名称 | Group Name | グループ名 | Nama Kumpulan | 分組名稱 |
| 父级分组 | 父级分组 | Parent Group | 親グループ | Kumpulan Induk | 父級分組 |
| 新增分组 | 新增分组 | Create Group | グループ作成 | Cipta Kumpulan | 新增分組 |
| 编辑分组 | 编辑分组 | Edit Group | グループ編集 | Edit Kumpulan | 編輯分組 |
| 域名 | 域名 | Domain | ドメイン | Domain | 域名 |
| 域名类型 | 域名类型 | Domain Type | ドメインタイプ | Jenis Domain | 域名類型 |
| 是否主域名 | 是否主域名 | Is Primary | プライマリ | Domain Utama | 是否主域名 |
| 验证状态 | 验证状态 | Verification Status | 検証ステータス | Status Pengesahan | 驗證狀態 |
| 已验证 | 已验证 | Verified | 検証済み | Disahkan | 已驗證 |
| 待验证 | 待验证 | Pending Verification | 検証待ち | Menunggu Pengesahan | 待驗證 |
| 验证失败 | 验证失败 | Verification Failed | 検証失敗 | Pengesahan Gagal | 驗證失敗 |
| 绑定域名 | 绑定域名 | Bind Domain | ドメインバインド | Ikat Domain | 綁定域名 |
| 解绑 | 解绑 | Unbind | 解除 | Nyahikat | 解綁 |
| 请选择租户 | 请选择租户 | Select tenant | テナントを選択 | Pilih penyewa | 請選擇租戶 |
| 请输入域名 | 请输入域名 | Enter domain | ドメインを入力 | Masukkan domain | 請輸入域名 |
| 请输入有效的域名格式 | 请输入有效的域名格式 | Please enter a valid domain format | 有効なドメイン形式を入力してください | Sila masukkan format domain yang sah | 請輸入有效的域名格式 |
| 确认解绑域名 {domain} | 确认解绑域名 {domain} | Confirm unbind domain {domain} | ドメイン {domain} を解除しますか | Sahkan nyahikat domain {domain} | 確認解綁域名 {domain} |
| 标签键 | 标签键 | Tag Key | タグキー | Kunci Tag | 標籤鍵 |
| 标签值 | 标签值 | Tag Value | タグ値 | Nilai Tag | 標籤值 |
| 标签类型 | 标签类型 | Tag Type | タグタイプ | Jenis Tag | 標籤類型 |
| 新增标签 | 新增标签 | Create Tag | タグ作成 | Cipta Tag | 新增標籤 |
| 请输入标签关键词 | 请输入标签关键词 | Enter tag keyword | タグキーワードを入力 | Masukkan kata kunci tag | 請輸入標籤關鍵詞 |
| 请输入分组编码 | 请输入分组编码 | Enter group code | グループコードを入力 | Masukkan kod kumpulan | 請輸入分組編碼 |
| 请输入分组名称 | 请输入分组名称 | Enter group name | グループ名を入力 | Masukkan nama kumpulan | 請輸入分組名稱 |
| 分组编码长度 2-50 个字符 | 分组编码长度 2-50 个字符 | Group code must be 2-50 characters | グループコードは2-50文字 | Kod kumpulan mestilah 2-50 aksara | 分組編碼長度 2-50 個字元 |
| 分组名称长度 2-100 个字符 | 分组名称长度 2-100 个字符 | Group name must be 2-100 characters | グループ名は2-100文字 | Nama kumpulan mestilah 2-100 aksara | 分組名稱長度 2-100 個字元 |
| 请输入标签键 | 请输入标签键 | Enter tag key | タグキーを入力 | Masukkan kunci tag | 請輸入標籤鍵 |
| 标签键长度 2-50 个字符 | 标签键长度 2-50 个字符 | Tag key must be 2-50 characters | タグキーは2-50文字 | Kunci tag mestilah 2-50 aksara | 標籤鍵長度 2-50 個字元 |
| 请输入标签值 | 请输入标签值 | Enter tag value | タグ値を入力 | Masukkan nilai tag | 請輸入標籤值 |
| 标签值长度 1-200 个字符 | 标签值长度 1-200 个字符 | Tag value must be 1-200 characters | タグ値は1-200文字 | Nilai tag mestilah 1-200 aksara | 標籤值長度 1-200 個字元 |
| 自定义 | 自定义 | Custom | カスタム | Tersuai | 自訂 |
| 系统 | 系统 | System | システム | Sistem | 系統 |
| 绑定成功 | 绑定成功 | Binding successful | バインド成功 | Pengikatan berjaya | 綁定成功 |
| 解绑成功 | 解绑成功 | Unbinding successful | 解除成功 | Nyahikatan berjaya | 解綁成功 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`编辑`、`删除`、`确定`、`取消`、`操作`、`ID`、`描述`、`创建时间`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`更新成功`、`删除成功`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"租户信息管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **Tab 面板**包含三个 Tab：分组管理、域名管理、标签管理
- [ ] **分组管理**：DxTreeList 展示分组树（Id、分组编码、分组名称、描述、创建时间、操作）
- [ ] **分组管理**：新增/编辑/删除分组功能完整
- [ ] **域名管理**：租户选择器存在
- [ ] **域名管理**：选择租户后展示域名列表（Id、域名、域名类型、是否主域名、验证状态、创建时间、操作）
- [ ] **域名管理**：绑定域名/解绑域名功能完整
- [ ] **标签管理**：标签列表展示（Id、标签键、标签值、标签类型、描述、创建时间、操作）
- [ ] **标签管理**：新增/删除标签功能完整

### P1 — 业务规则完整性

- [ ] 分组编码 `required` + `stringLength`（2-50）验证
- [ ] 分组名称 `required` + `stringLength`（2-100）验证
- [ ] 编辑分组时 GroupCode 字段 disabled
- [ ] 域名 `required` + 格式验证
- [ ] 绑定域名前必须已选择租户
- [ ] 解绑域名有 `confirmAction` 确认
- [ ] 标签键 `required` + `stringLength`（2-50）验证
- [ ] 标签值 `required` + `stringLength`（1-200）验证
- [ ] 删除分组/标签有 `confirmDelete` 确认
- [ ] 每个操作按钮有权限码控制
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn / DxTreeList column caption 全部使用 `:caption="$t()"`
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有域名类型/验证状态/标签类型显示值已国际化
- [ ] Tab 标题已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
