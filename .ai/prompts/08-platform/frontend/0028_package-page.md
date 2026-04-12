# 租户平台 — 套餐管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块为 SaaS 套餐生命周期管理，包含套餐 CRUD、发布/下架、版本管理和版本能力配置。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-9 |
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
- `PackageEndpoints.cs` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 套餐列表远程分页 |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 创建/编辑套餐表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 创建/编辑/详情/版本/能力弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 状态筛选、计费周期选择 |
| DxNumberBox | `DxNumberBox min max step format placeholder` | 资源配额数量、价格输入 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |
| DxTabPanel | `DxTabPanel items selectedIndex` | 版本与能力的 Tab 切换展示 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `PackageEndpoints.cs` 中的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 套餐列表 | GET | `/api/packages?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<SaasPackageRepDTO>>` |
| 套餐详情 | GET | `/api/packages/{id}` | - | `ApiResult<SaasPackageRepDTO>` |
| 创建套餐 | POST | `/api/packages` | `CreateSaasPackageReqDTO` | `ApiResult<long>` |
| 更新套餐 | PUT | `/api/packages/{id}` | `UpdateSaasPackageReqDTO` | `ApiResult` |
| 删除套餐 | DELETE | `/api/packages/{id}` | - | `ApiResult` |
| 发布套餐 | PUT | `/api/packages/{id}/publish` | - | `ApiResult` |
| 下架套餐 | PUT | `/api/packages/{id}/unpublish` | - | `ApiResult` |
| 检查编码唯一 | GET | `/api/packages/check-code-exists?code=xxx&excludeId=` | - | `ApiResult<bool>` |
| 版本列表 | GET | `/api/packages/{packageId}/versions?page=1&pageSize=20&keyword=` | - | `ApiResult<PagedResult<SaasPackageVersionRepDTO>>` |
| 创建版本 | POST | `/api/packages/{packageId}/versions` | `CreateSaasPackageVersionReqDTO` | `ApiResult<long>` |
| 能力列表 | GET | `/api/package-versions/{packageVersionId}/capabilities?page=1&pageSize=20&keyword=` | - | `ApiResult<PagedResult<SaasPackageCapabilityRepDTO>>` |
| 保存能力 | POST | `/api/package-versions/{packageVersionId}/capabilities` | `SaveSaasPackageCapabilityReqDTO` | `ApiResult<long>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue` | 主页面 |
| 2 | `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/packages/PackagesView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/packages.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/packages.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('套餐管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理 SaaS 套餐，包括定价、资源配额模板、版本和能力配置')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| 查询区 | 自定义查询栏 | 关键词 + 状态筛选 |
| 工具栏 | `DxToolbar` | 新增 |
| 表格区 | `DxDataGrid` + `CustomStore` | 套餐列表 |
| 分页 | `DxDataGrid` 内置 `DxPager` + `DxPaging` | 远程分页 |
| 新增弹窗 | `DxPopup` + `DxForm` | 创建套餐表单 |
| 编辑弹窗 | `DxPopup` + `DxForm` | 编辑套餐表单 |
| 详情抽屉 | `DxPopup`（只读展示） | 套餐详情 |
| 版本管理弹窗 | `DxPopup` + `DxDataGrid` | 版本列表 + 新增版本 |
| 能力配置弹窗 | `DxPopup` + `DxDataGrid` | 版本能力列表 + 保存能力 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入套餐编码或名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| draft | `$t('草稿')` |
| published | `$t('已发布')` |
| unpublished | `$t('已下架')` |

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
| 2 | Code | `$t('套餐编码')` | 150px | 是 | - | |
| 3 | Name | `$t('套餐名称')` | auto | 是 | - | |
| 4 | MonthlyPrice | `$t('月价')` | 120px | 否 | `currencyCell` | 货币格式 |
| 5 | YearlyPrice | `$t('年价')` | 120px | 否 | `currencyCell` | 货币格式 |
| 6 | MaxUsers | `$t('最大用户数')` | 120px | 否 | - | 资源配额 |
| 7 | StorageQuotaMB | `$t('存储配额(MB)')` | 130px | 否 | - | 资源配额 |
| 8 | ApiCallsPerMonth | `$t('月 API 调用量')` | 130px | 否 | - | 资源配额 |
| 9 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 10 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 11 | - | `$t('操作')` | 480px | 否 | `actionCell` | 操作按钮列 |

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
| draft | `$t('草稿')` | 灰色 `#8c8c8c` | `status-draft` |
| published | `$t('已发布')` | 绿色 `#52c41a` | `status-published` |
| unpublished | `$t('已下架')` | 红色 `#f5222d` | `status-unpublished` |

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
| 新增 | `$t('新增')` | `add` | `PACKAGE_CREATE` | 始终启用 | 打开新增弹窗 | 无 |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `PACKAGE_VIEW` | 始终 | 打开详情抽屉 | 无 |
| 编辑 | `$t('编辑')` | `edit` | `PACKAGE_UPDATE` | 始终 | 打开编辑弹窗 | 无 |
| 发布 | `$t('发布')` | `check` | `PACKAGE_UPDATE` | `Status !== 'published'` | 调用发布 API | `confirmAction('确认发布套餐 {name}')` |
| 下架 | `$t('下架')` | `close` | `PACKAGE_UPDATE` | `Status === 'published'` | 调用下架 API | `confirmAction('确认下架套餐 {name}')` |
| 版本 | `$t('版本管理')` | `folder` | `PACKAGE_VIEW` | 始终 | 打开版本管理弹窗 | 无 |
| 删除 | `$t('删除')` | `trash` | `PACKAGE_DELETE` | `Status !== 'published'` | 调用删除 API | `confirmDelete(row.Name)` |

### 版本管理弹窗内按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增版本 | `$t('新增版本')` | `add` | `PACKAGE_UPDATE` | 始终 | 打开新增版本表单 | 无 |
| 能力配置 | `$t('能力配置')` | `preferences` | `PACKAGE_VIEW` | 始终 | 打开能力配置弹窗 | 无 |

### 能力配置弹窗内按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 保存能力 | `$t('保存')` | `save` | `PACKAGE_UPDATE` | 始终 | 调用保存能力 API | 无 |

### 状态流转按钮动态显隐规则

> 此规则是本页面的核心业务逻辑。

| 当前状态 | 允许的操作 | 不允许的操作 |
|---------|-----------|------------|
| draft | 发布、编辑、删除 | 下架 |
| published | 下架 | 发布、删除（已发布套餐不可删除） |
| unpublished | 发布、编辑、删除 | 下架 |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const PACKAGE_VIEW = 'package.detail'
export const PACKAGE_CREATE = 'package.create'
export const PACKAGE_UPDATE = 'package.update'
export const PACKAGE_DELETE = 'package.delete'
```

### 确认框文案（全部国际化）

| 操作 | 文案 key | 说明 |
|------|---------|------|
| 发布 | `'确认发布套餐 {name}'` | 发布后租户可选用 |
| 下架 | `'确认下架套餐 {name}'` | 下架后新租户不可选用 |
| 删除 | `confirmDelete(row.Name)` | 使用通用删除确认 |

### 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建 | `notifySuccess('创建成功')` |
| 更新 | `notifySuccess('更新成功')` |
| 发布 | `notifySuccess('发布成功')` |
| 下架 | `notifySuccess('下架成功')` |
| 删除 | `notifySuccess('删除成功')` |
| 创建版本 | `notifySuccess('创建成功')` |
| 保存能力 | `notifySuccess('保存成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 新增表单

**标题**：`$t('新增套餐')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | Code | `$t('套餐编码')` | DxTextBox | 是 | 2-50 | `^[a-zA-Z0-9_-]+$` 字母数字下划线连字符 | 是 async | `''` | - | - |
| 2 | Name | `$t('套餐名称')` | DxTextBox | 是 | 2-100 | - | - | `''` | - | - |
| 3 | Description | `$t('套餐描述')` | DxTextArea | 否 | 0-500 | - | - | `''` | - | - |
| 4 | MonthlyPrice | `$t('月价')` | DxNumberBox | 是 | - | ≥ 0 | - | `0` | - | - |
| 5 | YearlyPrice | `$t('年价')` | DxNumberBox | 是 | - | ≥ 0 | - | `0` | - | - |
| 6 | MaxUsers | `$t('最大用户数')` | DxNumberBox | 是 | - | ≥ 1 整数 | - | `10` | - | - |
| 7 | StorageQuotaMB | `$t('存储配额(MB)')` | DxNumberBox | 是 | - | ≥ 1 整数 | - | `1024` | - | - |
| 8 | ApiCallsPerMonth | `$t('月 API 调用量')` | DxNumberBox | 是 | - | ≥ 0 整数 | - | `10000` | - | - |

**唯一性校验**：

- Code：调用 `GET /api/packages/check-code-exists?code=xxx`
- 编辑时排除当前 Id：`GET /api/packages/check-code-exists?code=xxx&excludeId=123`
- DxForm async validation：

```typescript
{
  type: 'async',
  validationCallback: async (params) => {
    const exists = await checkCodeExists(params.value, editingId.value)
    return !exists
  },
  message: t('套餐编码已存在')
}
```

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| Code | required | - | `$t('请输入套餐编码')` |
| Code | stringLength | min: 2, max: 50 | `$t('套餐编码长度 2-50 个字符')` |
| Code | pattern | `^[a-zA-Z0-9_-]+$` | `$t('套餐编码仅允许字母、数字、下划线和连字符')` |
| Code | async | checkCodeExists | `$t('套餐编码已存在')` |
| Name | required | - | `$t('请输入套餐名称')` |
| Name | stringLength | min: 2, max: 100 | `$t('套餐名称长度 2-100 个字符')` |
| Description | stringLength | max: 500 | `$t('套餐描述长度不超过 500 个字符')` |
| MonthlyPrice | required | - | `$t('请输入月价')` |
| MonthlyPrice | range | min: 0 | `$t('月价不能为负数')` |
| YearlyPrice | required | - | `$t('请输入年价')` |
| YearlyPrice | range | min: 0 | `$t('年价不能为负数')` |
| MaxUsers | required | - | `$t('请输入最大用户数')` |
| MaxUsers | range | min: 1 | `$t('最大用户数至少为 1')` |
| StorageQuotaMB | required | - | `$t('请输入存储配额')` |
| StorageQuotaMB | range | min: 1 | `$t('存储配额至少为 1 MB')` |
| ApiCallsPerMonth | required | - | `$t('请输入月 API 调用量')` |
| ApiCallsPerMonth | range | min: 0 | `$t('月 API 调用量不能为负数')` |

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

**标题**：`$t('编辑套餐')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | Code | `$t('套餐编码')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 2 | Name | `$t('套餐名称')` | DxTextBox | 是 | 2-100 | - | - | 从数据加载 | - | - |
| 3 | Description | `$t('套餐描述')` | DxTextArea | 否 | 0-500 | - | - | 从数据加载 | - | - |
| 4 | MonthlyPrice | `$t('月价')` | DxNumberBox | 是 | - | ≥ 0 | - | 从数据加载 | - | - |
| 5 | YearlyPrice | `$t('年价')` | DxNumberBox | 是 | - | ≥ 0 | - | 从数据加载 | - | - |
| 6 | MaxUsers | `$t('最大用户数')` | DxNumberBox | 是 | - | ≥ 1 整数 | - | 从数据加载 | - | - |
| 7 | StorageQuotaMB | `$t('存储配额(MB)')` | DxNumberBox | 是 | - | ≥ 1 整数 | - | 从数据加载 | - | - |
| 8 | ApiCallsPerMonth | `$t('月 API 调用量')` | DxNumberBox | 是 | - | ≥ 0 整数 | - | 从数据加载 | - | - |

**注意**：编辑时 Code 字段为只读（`disabled: true`），不可修改。

**提交行为**：同新增表单，成功提示为 `notifySuccess('更新成功')`。

### 详情展示

**标题**：`$t('套餐详情')`
**组件**：`DxPopup`（只读展示）

展示字段：

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | Code | `$t('套餐编码')` | - |
| 3 | Name | `$t('套餐名称')` | - |
| 4 | Description | `$t('套餐描述')` | - |
| 5 | MonthlyPrice | `$t('月价')` | 货币格式 |
| 6 | YearlyPrice | `$t('年价')` | 货币格式 |
| 7 | MaxUsers | `$t('最大用户数')` | - |
| 8 | StorageQuotaMB | `$t('存储配额(MB)')` | - |
| 9 | ApiCallsPerMonth | `$t('月 API 调用量')` | - |
| 10 | Status | `$t('状态')` | 状态标签（颜色） |
| 11 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 版本管理弹窗

**标题**：`$t('版本管理')`
**组件**：`DxPopup`（`width: 800`，`height: 500`）+ `DxDataGrid`

- 调用 `GET /api/packages/{packageId}/versions?page=1&pageSize=20&keyword=` 获取版本列表
- 版本列表列定义：

| 序号 | data-field | caption（i18n key） | 宽度 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|--------|------|
| 1 | Id | `$t('ID')` | 80px | - | |
| 2 | VersionCode | `$t('版本号')` | 150px | - | |
| 3 | Description | `$t('版本描述')` | auto | - | |
| 4 | CreatedAt | `$t('创建时间')` | 180px | `dateCell` | |
| 5 | - | `$t('操作')` | 120px | `actionCell` | 能力配置按钮 |

- 新增版本表单字段：

| 序号 | 字段名 | 标签 | 类型 | 必填 | 说明 |
|:----:|--------|------|------|:----:|------|
| 1 | VersionCode | `$t('版本号')` | DxTextBox | 是 | 如 v1.0, v2.0 |
| 2 | Description | `$t('版本描述')` | DxTextArea | 否 | 版本变更说明 |

### 能力配置弹窗

**标题**：`$t('能力配置')`
**组件**：`DxPopup`（`width: 800`，`height: 500`）+ `DxDataGrid`

- 调用 `GET /api/package-versions/{packageVersionId}/capabilities?page=1&pageSize=20&keyword=` 获取能力列表
- 能力列表列定义：

| 序号 | data-field | caption（i18n key） | 宽度 | 格式化 |
|:----:|-----------|---------------------|:----:|--------|
| 1 | Id | `$t('ID')` | 80px | - |
| 2 | CapabilityCode | `$t('能力编码')` | 150px | - |
| 3 | CapabilityName | `$t('能力名称')` | auto | - |
| 4 | Enabled | `$t('是否启用')` | 100px | 布尔值 |

### 类型定义

```typescript
// src/types/packages.ts

/** 创建套餐请求 */
export interface CreateSaasPackageReqDTO {
  Code: string
  Name: string
  Description?: string
  MonthlyPrice: number
  YearlyPrice: number
  MaxUsers: number
  StorageQuotaMB: number
  ApiCallsPerMonth: number
}

/** 更新套餐请求 */
export interface UpdateSaasPackageReqDTO {
  Name: string
  Description?: string
  MonthlyPrice: number
  YearlyPrice: number
  MaxUsers: number
  StorageQuotaMB: number
  ApiCallsPerMonth: number
}

/** 套餐响应 */
export interface SaasPackageRepDTO {
  Id: number
  Code: string
  Name: string
  Description?: string
  MonthlyPrice: number
  YearlyPrice: number
  MaxUsers: number
  StorageQuotaMB: number
  ApiCallsPerMonth: number
  Status: string
  CreatedAt: string
}

/** 创建版本请求 */
export interface CreateSaasPackageVersionReqDTO {
  VersionCode: string
  Description?: string
}

/** 版本响应 */
export interface SaasPackageVersionRepDTO {
  Id: number
  PackageRefId: number
  VersionCode: string
  Description?: string
  CreatedAt: string
}

/** 保存能力请求 */
export interface SaveSaasPackageCapabilityReqDTO {
  CapabilityCode: string
  CapabilityName: string
  Enabled: boolean
}

/** 能力响应 */
export interface SaasPackageCapabilityRepDTO {
  Id: number
  PackageVersionRefId: number
  CapabilityCode: string
  CapabilityName: string
  Enabled: boolean
}
```

---

## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| columns.ts | 套餐表格列定义数组 | 页面组件级语言文件 |
| query-form.ts | 查询表单字段配置 | 页面组件级语言文件 |
| status.ts | 套餐状态字典（draft / published / unpublished） | 页面组件级语言文件 |

---

## 国际化要求

### 组件级 key（放入 `PackagesView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 套餐管理 | 套餐管理 | Package Management | パッケージ管理 | Pengurusan Pakej | 套餐管理 |
| 管理 SaaS 套餐，包括定价、资源配额模板、版本和能力配置 | (同key) | Manage SaaS packages including pricing, resource quota templates, versions and capability configuration | SaaS パッケージ管理：料金設定、リソースクォータテンプレート、バージョンと機能設定 | Urus pakej SaaS termasuk harga, templat kuota sumber, versi dan konfigurasi keupayaan | 管理 SaaS 套餐，包括定價、資源配額範本、版本和能力配置 |
| 请输入套餐编码或名称 | 请输入套餐编码或名称 | Enter package code or name | パッケージコードまたは名前を入力 | Masukkan kod pakej atau nama | 請輸入套餐編碼或名稱 |
| 套餐编码 | 套餐编码 | Package Code | パッケージコード | Kod Pakej | 套餐編碼 |
| 套餐名称 | 套餐名称 | Package Name | パッケージ名 | Nama Pakej | 套餐名稱 |
| 套餐描述 | 套餐描述 | Package Description | パッケージ説明 | Penerangan Pakej | 套餐描述 |
| 月价 | 月价 | Monthly Price | 月額料金 | Harga Bulanan | 月價 |
| 年价 | 年价 | Yearly Price | 年額料金 | Harga Tahunan | 年價 |
| 最大用户数 | 最大用户数 | Max Users | 最大ユーザー数 | Pengguna Maksimum | 最大使用者數 |
| 存储配额(MB) | 存储配额(MB) | Storage Quota (MB) | ストレージクォータ(MB) | Kuota Storan (MB) | 儲存配額(MB) |
| 月 API 调用量 | 月 API 调用量 | Monthly API Calls | 月間API呼び出し数 | Panggilan API Bulanan | 月 API 調用量 |
| 草稿 | 草稿 | Draft | 下書き | Draf | 草稿 |
| 已发布 | 已发布 | Published | 公開中 | Diterbitkan | 已發佈 |
| 已下架 | 已下架 | Unpublished | 非公開 | Dinyahterbit | 已下架 |
| 发布 | 发布 | Publish | 公開 | Terbit | 發佈 |
| 下架 | 下架 | Unpublish | 非公開にする | Nyahterbit | 下架 |
| 新增套餐 | 新增套餐 | Create Package | パッケージ作成 | Cipta Pakej | 新增套餐 |
| 编辑套餐 | 编辑套餐 | Edit Package | パッケージ編集 | Edit Pakej | 編輯套餐 |
| 套餐详情 | 套餐详情 | Package Details | パッケージ詳細 | Butiran Pakej | 套餐詳情 |
| 版本管理 | 版本管理 | Version Management | バージョン管理 | Pengurusan Versi | 版本管理 |
| 版本号 | 版本号 | Version Code | バージョン番号 | Kod Versi | 版本號 |
| 版本描述 | 版本描述 | Version Description | バージョン説明 | Penerangan Versi | 版本描述 |
| 新增版本 | 新增版本 | Create Version | バージョン作成 | Cipta Versi | 新增版本 |
| 能力配置 | 能力配置 | Capability Configuration | 機能設定 | Konfigurasi Keupayaan | 能力配置 |
| 能力编码 | 能力编码 | Capability Code | 機能コード | Kod Keupayaan | 能力編碼 |
| 能力名称 | 能力名称 | Capability Name | 機能名 | Nama Keupayaan | 能力名稱 |
| 是否启用 | 是否启用 | Enabled | 有効 | Diaktifkan | 是否啟用 |
| 请输入套餐编码 | 请输入套餐编码 | Enter package code | パッケージコードを入力 | Masukkan kod pakej | 請輸入套餐編碼 |
| 请输入套餐名称 | 请输入套餐名称 | Enter package name | パッケージ名を入力 | Masukkan nama pakej | 請輸入套餐名稱 |
| 套餐编码长度 2-50 个字符 | 套餐编码长度 2-50 个字符 | Package code must be 2-50 characters | パッケージコードは2-50文字 | Kod pakej mestilah 2-50 aksara | 套餐編碼長度 2-50 個字元 |
| 套餐编码仅允许字母、数字、下划线和连字符 | 套餐编码仅允许字母、数字、下划线和连字符 | Package code can only contain letters, numbers, underscores and hyphens | パッケージコードは英数字、アンダースコア、ハイフンのみ | Kod pakej hanya boleh mengandungi huruf, nombor, garis bawah dan sempang | 套餐編碼僅允許字母、數字、底線和連字符 |
| 套餐编码已存在 | 套餐编码已存在 | Package code already exists | パッケージコードは既に存在します | Kod pakej sudah wujud | 套餐編碼已存在 |
| 套餐名称长度 2-100 个字符 | 套餐名称长度 2-100 个字符 | Package name must be 2-100 characters | パッケージ名は2-100文字 | Nama pakej mestilah 2-100 aksara | 套餐名稱長度 2-100 個字元 |
| 套餐描述长度不超过 500 个字符 | 套餐描述长度不超过 500 个字符 | Package description must not exceed 500 characters | パッケージ説明は500文字以内 | Penerangan pakej mestilah tidak melebihi 500 aksara | 套餐描述長度不超過 500 個字元 |
| 请输入月价 | 请输入月价 | Enter monthly price | 月額料金を入力 | Masukkan harga bulanan | 請輸入月價 |
| 月价不能为负数 | 月价不能为负数 | Monthly price cannot be negative | 月額料金は0以上 | Harga bulanan tidak boleh negatif | 月價不能為負數 |
| 请输入年价 | 请输入年价 | Enter yearly price | 年額料金を入力 | Masukkan harga tahunan | 請輸入年價 |
| 年价不能为负数 | 年价不能为负数 | Yearly price cannot be negative | 年額料金は0以上 | Harga tahunan tidak boleh negatif | 年價不能為負數 |
| 请输入最大用户数 | 请输入最大用户数 | Enter max users | 最大ユーザー数を入力 | Masukkan pengguna maksimum | 請輸入最大使用者數 |
| 最大用户数至少为 1 | 最大用户数至少为 1 | Max users must be at least 1 | 最大ユーザー数は1以上 | Pengguna maksimum mestilah sekurang-kurangnya 1 | 最大使用者數至少為 1 |
| 请输入存储配额 | 请输入存储配额 | Enter storage quota | ストレージクォータを入力 | Masukkan kuota storan | 請輸入儲存配額 |
| 存储配额至少为 1 MB | 存储配额至少为 1 MB | Storage quota must be at least 1 MB | ストレージクォータは1MB以上 | Kuota storan mestilah sekurang-kurangnya 1 MB | 儲存配額至少為 1 MB |
| 请输入月 API 调用量 | 请输入月 API 调用量 | Enter monthly API calls | 月間API呼び出し数を入力 | Masukkan panggilan API bulanan | 請輸入月 API 調用量 |
| 月 API 调用量不能为负数 | 月 API 调用量不能为负数 | Monthly API calls cannot be negative | 月間API呼び出し数は0以上 | Panggilan API bulanan tidak boleh negatif | 月 API 調用量不能為負數 |
| 确认发布套餐 {name} | 确认发布套餐 {name} | Confirm publish package {name} | パッケージ {name} を公開しますか | Sahkan terbit pakej {name} | 確認發佈套餐 {name} |
| 确认下架套餐 {name} | 确认下架套餐 {name} | Confirm unpublish package {name} | パッケージ {name} を非公開にしますか | Sahkan nyahterbit pakej {name} | 確認下架套餐 {name} |
| 发布成功 | 发布成功 | Published successfully | 公開成功 | Penerbitan berjaya | 發佈成功 |
| 下架成功 | 下架成功 | Unpublished successfully | 非公開成功 | Penyahterbit berjaya | 下架成功 |
| 保存成功 | 保存成功 | Saved successfully | 保存成功 | Penyimpanan berjaya | 儲存成功 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`编辑`、`删除`、`查看`、`确定`、`取消`、`操作`、`ID`、`状态`、`创建时间`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`更新成功`、`删除成功`、`请选择状态`、`保存`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"套餐管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **查询区**包含：关键词输入框、状态下拉（3 个状态选项 + 全部）
- [ ] **表格列**包含：ID、套餐编码、套餐名称、月价、年价、最大用户数、存储配额、月 API 调用量、状态、创建时间、操作
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 远程分页通过 CustomStore 实现
- [ ] **工具栏**包含：新增按钮
- [ ] **行操作**包含：查看、编辑、发布、下架、版本管理、删除
- [ ] **新增弹窗**包含字段：套餐编码、套餐名称、套餐描述、月价、年价、最大用户数、存储配额、月 API 调用量
- [ ] **编辑弹窗**包含字段：套餐编码（只读）、套餐名称、套餐描述、月价、年价、最大用户数、存储配额、月 API 调用量
- [ ] **详情抽屉**展示：ID、套餐编码、套餐名称、套餐描述、月价、年价、最大用户数、存储配额、月 API 调用量、状态、创建时间
- [ ] **版本管理弹窗**展示版本列表（ID、版本号、版本描述、创建时间、操作）
- [ ] **版本管理弹窗**包含新增版本按钮和表单
- [ ] **能力配置弹窗**展示能力列表（ID、能力编码、能力名称、是否启用）
- [ ] **能力配置弹窗**包含保存按钮

### P1 — 业务规则完整性

- [ ] 套餐编码 `required` 验证
- [ ] 套餐编码 `stringLength`（2-50）验证
- [ ] 套餐编码 `pattern`（字母数字下划线连字符）验证
- [ ] 套餐编码 `async` 唯一性验证（新增时）
- [ ] 套餐编码 `async` 唯一性验证排除当前 Id（编辑时排除）
- [ ] 编辑时套餐编码字段 disabled（不可修改）
- [ ] 套餐名称 `required` 验证
- [ ] 套餐名称 `stringLength`（2-100）验证
- [ ] 月价 `required` + 不能为负数验证
- [ ] 年价 `required` + 不能为负数验证
- [ ] 最大用户数 `required` + ≥ 1 验证
- [ ] 存储配额 `required` + ≥ 1 验证
- [ ] 月 API 调用量 `required` + ≥ 0 验证
- [ ] 每个操作按钮有权限码控制
- [ ] **已发布套餐不可删除**：删除按钮在 `Status === 'published'` 时隐藏
- [ ] **发布按钮**在 `Status !== 'published'` 时显示
- [ ] **下架按钮**仅在 `Status === 'published'` 时显示
- [ ] 发布有 `confirmAction` 确认
- [ ] 下架有 `confirmAction` 确认
- [ ] 删除有 `confirmDelete` 确认
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' PackagesView.vue | grep -v ':caption'` 结果为 0
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] `grep -rn "notifySuccess(t(" PackagesView.vue` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有状态显示值已国际化（3 种套餐状态）
- [ ] 所有确认框文案已国际化
- [ ] 所有成功提示已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
