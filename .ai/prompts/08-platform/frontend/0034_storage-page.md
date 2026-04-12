# 租户平台 — 文件存储管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块为文件存储管理，包含存储策略管理、租户文件管理、文件访问策略三个标签页。文件本地存储，文件路径不暴露给前端。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-15 |
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
- `src/YTStdTenantPlatform/Endpoints/StorageEndpoints.cs` — 后端 API 端点定义
- `src/YTStdTenantPlatform/Application/Dtos/Storage/StorageRepDTO.cs` — 后端响应 DTO 定义
- `src/YTStdTenantPlatform/Application/Dtos/Storage/StorageReqDTO.cs` — 后端请求 DTO 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 三个标签页列表远程分页 |
| DxTabPanel | `DxTabPanel items selectedIndex onSelectionChanged` | 三标签页切换（存储策略 / 租户文件 / 访问策略） |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 创建/编辑存储策略、创建访问策略表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 创建/编辑/详情弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 提供商类型选择、状态筛选 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `StorageEndpoints.cs` 中的路由注册。

### 存储策略端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 策略列表 | GET | `/api/storage-strategies?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<StorageStrategyRepDTO>>` |
| 策略详情 | GET | `/api/storage-strategies/{id}` | - | `ApiResult<StorageStrategyRepDTO>` |
| 创建策略 | POST | `/api/storage-strategies` | `CreateStorageStrategyReqDTO` | `ApiResult<long>` |
| 更新策略 | PUT | `/api/storage-strategies/{id}` | `UpdateStorageStrategyReqDTO` | `ApiResult` |
| 启用策略 | PUT | `/api/storage-strategies/{id}/enable` | - | `ApiResult` |
| 禁用策略 | PUT | `/api/storage-strategies/{id}/disable` | - | `ApiResult` |

### 文件端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 文件列表 | GET | `/api/files?page=1&pageSize=20&keyword=&tenantRefId=` | - | `ApiResult<PagedResult<TenantFileRepDTO>>` |
| 文件详情 | GET | `/api/files/{id}` | - | `ApiResult<TenantFileRepDTO>` |
| 删除文件 | DELETE | `/api/files/{id}` | - | `ApiResult` |

### 文件访问策略端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 访问策略列表 | GET | `/api/file-access-policies?page=1&pageSize=20&keyword=&fileId=` | - | `ApiResult<PagedResult<FileAccessPolicyRepDTO>>` |
| 创建访问策略 | POST | `/api/file-access-policies` | `SaveFileAccessPolicyReqDTO` | `ApiResult<long>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/storage/StorageView.vue` | 主页面（含 3 个标签页） |
| 2 | `src/WebTenantPlatfrom/src/views/storage/StorageView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/storage/StorageView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/storage/StorageView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/storage/StorageView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/storage/StorageView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/storage.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/storage.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('文件存储管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理存储策略、租户文件和文件访问策略')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| 标签页容器 | `DxTabPanel` | 3 个标签页切换 |
| Tab 1: 存储策略 | DxDataGrid + CustomStore | 存储策略列表 + 工具栏 |
| Tab 2: 租户文件 | DxDataGrid + CustomStore | 文件列表 + 工具栏（仅查看/删除） |
| Tab 3: 访问策略 | DxDataGrid + CustomStore | 访问策略列表 + 工具栏 |
| 新增策略弹窗 | DxPopup + DxForm | 创建存储策略表单 |
| 编辑策略弹窗 | DxPopup + DxForm | 编辑存储策略表单 |
| 策略详情弹窗 | DxPopup（只读展示） | 存储策略详情 |
| 文件详情弹窗 | DxPopup（只读展示） | 文件详情 |
| 新增访问策略弹窗 | DxPopup + DxForm | 创建文件访问策略表单 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### Tab 1: 存储策略 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入策略编码或名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| enabled | `$t('已启用')` |
| disabled | `$t('已禁用')` |

### Tab 2: 租户文件 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入文件名')` |
| 2 | TenantRefId | `$t('租户')` | DxSelectBox | `null`（全部） | `$t('请选择租户')` |

### Tab 3: 访问策略 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入主体类型或权限编码')` |
| 2 | FileId | `$t('文件ID')` | DxTextBox | `''` | `$t('请输入文件ID')` |

### 查询行为

| 行为 | 要求 |
|------|------|
| 回车搜索 | 在关键词输入框回车触发搜索 |
| 查询按钮 | `$t('查询')`，点击触发搜索 |
| 重置按钮 | `$t('重置')`，清空所有条件并重新加载 |
| 所有文本国际化 | 所有 label、placeholder、按钮文本均使用 `$t()` |

---

## 列表与分页

### Tab 1: 存储策略列表

**表格组件**：DxDataGrid + CustomStore

#### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | 固定宽度 |
| 2 | StrategyCode | `$t('策略编码')` | 150px | 是 | - | |
| 3 | StrategyName | `$t('策略名称')` | auto | 是 | - | |
| 4 | ProviderType | `$t('提供商类型')` | 120px | 否 | `providerCell` | 标签显示 |
| 5 | BucketName | `$t('存储桶名称')` | 150px | 否 | - | |
| 6 | BasePath | `$t('基础路径')` | auto | 否 | - | |
| 7 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 8 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 9 | - | `$t('操作')` | 380px | 否 | `actionCell` | 操作按钮列 |

### Tab 2: 租户文件列表

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | TenantRefId | `$t('租户ID')` | 100px | 否 | - | |
| 3 | FileName | `$t('文件名')` | auto | 是 | - | |
| 4 | FileExt | `$t('扩展名')` | 80px | 否 | - | |
| 5 | MimeType | `$t('MIME 类型')` | 150px | 否 | - | |
| 6 | FileSize | `$t('文件大小')` | 120px | 是 | `fileSizeCell` | 格式化为 KB/MB/GB |
| 7 | UploaderType | `$t('上传者类型')` | 100px | 否 | - | |
| 8 | Visibility | `$t('可见性')` | 100px | 否 | `visibilityCell` | 标签显示 |
| 9 | DownloadCount | `$t('下载次数')` | 100px | 否 | - | |
| 10 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 11 | - | `$t('操作')` | 200px | 否 | `actionCell` | 操作按钮列 |

### Tab 3: 文件访问策略列表

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | FileId | `$t('文件ID')` | 100px | 否 | - | |
| 3 | SubjectType | `$t('主体类型')` | 120px | 否 | - | |
| 4 | SubjectId | `$t('主体ID')` | 120px | 否 | - | |
| 5 | PermissionCode | `$t('权限编码')` | 120px | 否 | `permissionCell` | 标签显示 |
| 6 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |

### 分页配置（所有标签页通用）

| 配置 | 值 |
|------|---|
| 支持分页 | 是 |
| 默认页大小 | 20 |
| 可选页大小 | `[10, 20, 50, 100]` |
| 显示总数 | 是（`showInfo: true`） |
| 显示页大小选择 | 是（`showPageSizeSelector: true`） |
| 显示导航按钮 | 是（`showNavigationButtons: true`） |
| 远程分页 | 是（CustomStore） |

### 策略状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| enabled | `$t('已启用')` | 绿色 `#52c41a` | `status-enabled` |
| disabled | `$t('已禁用')` | 红色 `#f5222d` | `status-disabled` |

### 提供商类型标签

| 类型值 | 显示文本 |
|:------:|---------|
| local | `$t('本地存储')` |
| s3 | `$t('Amazon S3')` |
| oss | `$t('阿里云 OSS')` |
| azure | `$t('Azure Blob')` |

### 权限编码标签

| 编码值 | 显示文本 |
|:------:|---------|
| read | `$t('读取')` |
| write | `$t('写入')` |
| delete | `$t('删除权限')` |
| admin | `$t('管理员')` |

### 文件可见性标签

| 可见性值 | 显示文本 |
|:--------:|---------|
| public | `$t('公开')` |
| private | `$t('私有')` |

### 空状态与加载

| 状态 | 显示 |
|------|------|
| 空数据 | `:no-data-text="$t('暂无数据')"` |
| 加载中 | `DxLoadPanel`（`visible` 绑定 `pageLoading`） |

---

## 操作按钮

### Tab 1: 存储策略 — 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增 | `$t('新增')` | `add` | `STORAGE_STRATEGY_CREATE` | 始终启用 | 打开新增策略弹窗 | 无 |

### Tab 1: 存储策略 — 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `STORAGE_STRATEGY_VIEW` | 始终 | 打开详情弹窗 | 无 |
| 编辑 | `$t('编辑')` | `edit` | `STORAGE_STRATEGY_UPDATE` | 始终 | 打开编辑弹窗 | 无 |
| 启用 | `$t('启用')` | `check` | `STORAGE_STRATEGY_ENABLE` | `Status === 'disabled'` | 调用启用 API | `confirmAction('确认启用策略 {name}')` |
| 禁用 | `$t('禁用')` | `close` | `STORAGE_STRATEGY_DISABLE` | `Status === 'enabled'` | 调用禁用 API | `confirmAction('确认禁用策略 {name}')` |

### Tab 2: 租户文件 — 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `FILE_VIEW` | 始终 | 打开文件详情弹窗 | 无 |
| 删除 | `$t('删除')` | `trash` | `FILE_DELETE` | 始终 | 调用删除 API | `confirmDelete(row.FileName)` |

### Tab 3: 访问策略 — 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增 | `$t('新增')` | `add` | `FILE_ACCESS_POLICY_CREATE` | 始终启用 | 打开新增访问策略弹窗 | 无 |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const STORAGE_STRATEGY_VIEW = 'storage-strategy.detail'
export const STORAGE_STRATEGY_CREATE = 'storage-strategy.create'
export const STORAGE_STRATEGY_UPDATE = 'storage-strategy.update'
export const STORAGE_STRATEGY_ENABLE = 'storage-strategy.enable'
export const STORAGE_STRATEGY_DISABLE = 'storage-strategy.disable'
export const FILE_VIEW = 'file.detail'
export const FILE_DELETE = 'file.delete'
export const FILE_ACCESS_POLICY_CREATE = 'file-access-policy.create'
```

### 确认框文案（全部国际化）

| 操作 | 文案 key | 说明 |
|------|---------|------|
| 启用策略 | `'确认启用策略 {name}'` | |
| 禁用策略 | `'确认禁用策略 {name}'` | |
| 删除文件 | `confirmDelete(row.FileName)` | 使用通用删除确认 |

### 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建策略 | `notifySuccess('创建成功')` |
| 更新策略 | `notifySuccess('更新成功')` |
| 启用策略 | `notifySuccess('启用成功')` |
| 禁用策略 | `notifySuccess('禁用成功')` |
| 删除文件 | `notifySuccess('删除成功')` |
| 创建访问策略 | `notifySuccess('创建成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 新增存储策略表单

**标题**：`$t('新增存储策略')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | StrategyCode | `$t('策略编码')` | DxTextBox | 是 | 2-50 | `^[a-zA-Z0-9_-]+$` | - | `''` | - | - |
| 2 | StrategyName | `$t('策略名称')` | DxTextBox | 是 | 2-100 | - | - | `''` | - | - |
| 3 | ProviderType | `$t('提供商类型')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 4 | BucketName | `$t('存储桶名称')` | DxTextBox | 否 | 0-200 | - | - | `''` | - | - |
| 5 | BasePath | `$t('基础路径')` | DxTextBox | 否 | 0-500 | - | - | `''` | - | - |

**提供商类型选项**：

| 值 | 显示文本 |
|:--:|---------|
| local | `$t('本地存储')` |
| s3 | `$t('Amazon S3')` |
| oss | `$t('阿里云 OSS')` |
| azure | `$t('Azure Blob')` |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| StrategyCode | required | - | `$t('请输入策略编码')` |
| StrategyCode | stringLength | min: 2, max: 50 | `$t('策略编码长度 2-50 个字符')` |
| StrategyCode | pattern | `^[a-zA-Z0-9_-]+$` | `$t('策略编码仅允许字母、数字、下划线和连字符')` |
| StrategyName | required | - | `$t('请输入策略名称')` |
| StrategyName | stringLength | min: 2, max: 100 | `$t('策略名称长度 2-100 个字符')` |
| ProviderType | required | - | `$t('请选择提供商类型')` |
| BucketName | stringLength | max: 200 | `$t('存储桶名称长度不超过 200 个字符')` |
| BasePath | stringLength | max: 500 | `$t('基础路径长度不超过 500 个字符')` |

**提交行为**：

1. 提交前：调用 `formInstance.validate()`，不通过则阻止提交
2. 提交时：`submitting.value = true`，禁用提交按钮并显示 loading
3. 提交成功：关闭弹窗 → `notifySuccess('创建成功')` → 刷新列表
4. 提交失败：axios 拦截器自动显示错误 → 不关闭弹窗 → `submitting.value = false`

### 编辑存储策略表单

**标题**：`$t('编辑存储策略')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | StrategyCode | `$t('策略编码')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 2 | StrategyName | `$t('策略名称')` | DxTextBox | 否 | 2-100 | - | - | 从数据加载 | - | - |
| 3 | ProviderType | `$t('提供商类型')` | DxSelectBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 4 | BucketName | `$t('存储桶名称')` | DxTextBox | 否 | 0-200 | - | - | 从数据加载 | - | - |
| 5 | BasePath | `$t('基础路径')` | DxTextBox | 否 | 0-500 | - | - | 从数据加载 | - | - |

**注意**：编辑接口 `UpdateStorageStrategyReqDTO` 仅含 `StrategyName`、`BucketName`、`BasePath` 字段。`StrategyCode` 和 `ProviderType` 编辑时为只读。

**提交行为**：同新增策略表单，成功提示为 `notifySuccess('更新成功')`。

### 新增文件访问策略表单

**标题**：`$t('新增访问策略')`
**组件**：`DxPopup`（`width: 500`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | FileId | `$t('文件ID')` | DxTextBox | 是 | - | 数字 | - | `''` | - | - |
| 2 | SubjectType | `$t('主体类型')` | DxTextBox | 是 | 1-50 | - | - | `''` | - | - |
| 3 | SubjectId | `$t('主体ID')` | DxTextBox | 否 | 0-50 | - | - | `''` | - | - |
| 4 | PermissionCode | `$t('权限编码')` | DxSelectBox | 是 | - | - | - | `null` | - | - |

**权限编码选项**：

| 值 | 显示文本 |
|:--:|---------|
| read | `$t('读取')` |
| write | `$t('写入')` |
| delete | `$t('删除权限')` |
| admin | `$t('管理员')` |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| FileId | required | - | `$t('请输入文件ID')` |
| SubjectType | required | - | `$t('请输入主体类型')` |
| SubjectType | stringLength | max: 50 | `$t('主体类型长度不超过 50 个字符')` |
| PermissionCode | required | - | `$t('请选择权限编码')` |

**提交行为**：同新增策略表单，成功提示为 `notifySuccess('创建成功')`。

### 策略详情展示

**标题**：`$t('策略详情')`
**组件**：`DxPopup`（只读展示，`width: 600`，`height: auto`）

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | StrategyCode | `$t('策略编码')` | - |
| 3 | StrategyName | `$t('策略名称')` | - |
| 4 | ProviderType | `$t('提供商类型')` | 提供商标签 |
| 5 | BucketName | `$t('存储桶名称')` | - |
| 6 | BasePath | `$t('基础路径')` | - |
| 7 | Status | `$t('状态')` | 状态标签（颜色） |
| 8 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 文件详情展示

**标题**：`$t('文件详情')`
**组件**：`DxPopup`（只读展示，`width: 600`，`height: auto`）

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | TenantRefId | `$t('租户ID')` | - |
| 3 | StorageStrategyId | `$t('存储策略ID')` | - |
| 4 | FileName | `$t('文件名')` | - |
| 5 | FileExt | `$t('扩展名')` | - |
| 6 | MimeType | `$t('MIME 类型')` | - |
| 7 | FileSize | `$t('文件大小')` | 格式化为 KB/MB/GB |
| 8 | UploaderType | `$t('上传者类型')` | - |
| 9 | UploaderId | `$t('上传者ID')` | - |
| 10 | Visibility | `$t('可见性')` | 可见性标签 |
| 11 | DownloadCount | `$t('下载次数')` | - |
| 12 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

**注意**：`FilePath` 不展示在前端详情中（安全要求）。

**弹窗按钮（所有弹窗通用）**：

| 按钮 | 文本 | 位置 | 行为 |
|------|------|------|------|
| 提交 | `$t('确定')` | 弹窗底部右侧 | 提交表单，`submitting` 时 disabled + loading |
| 取消 | `$t('取消')` | 弹窗底部左侧 | 关闭弹窗，重置表单 |

### 类型定义

```typescript
// src/types/storage.ts

/** 存储策略响应 */
export interface StorageStrategyRepDTO {
  Id: number
  StrategyCode: string
  StrategyName: string
  ProviderType: string
  BucketName?: string
  BasePath?: string
  Status: string
  CreatedAt: string
}

/** 创建存储策略请求 */
export interface CreateStorageStrategyReqDTO {
  StrategyCode: string
  StrategyName: string
  ProviderType: string
  BucketName?: string
  BasePath?: string
}

/** 更新存储策略请求 */
export interface UpdateStorageStrategyReqDTO {
  StrategyName?: string
  BucketName?: string
  BasePath?: string
}

/** 租户文件响应 */
export interface TenantFileRepDTO {
  Id: number
  TenantRefId: number
  StorageStrategyId?: number
  FileName: string
  FilePath: string
  FileExt?: string
  MimeType?: string
  FileSize: number
  UploaderType: string
  UploaderId?: number
  Visibility: string
  DownloadCount: number
  CreatedAt: string
}

/** 文件访问策略响应 */
export interface FileAccessPolicyRepDTO {
  Id: number
  FileId: number
  SubjectType: string
  SubjectId?: string
  PermissionCode: string
  CreatedAt: string
}

/** 创建文件访问策略请求 */
export interface SaveFileAccessPolicyReqDTO {
  FileId: number
  SubjectType: string
  SubjectId?: string
  PermissionCode: string
}
```

---

## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| 无独立静态配置文件 | 提供商类型、权限编码、可见性等字典均内联在页面组件中 | 页面组件级语言文件 |

> 本模块选项数量有限，不需要独立配置文件。

---

## 国际化要求

### 组件级 key（放入 `StorageView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 文件存储管理 | 文件存储管理 | File Storage Management | ファイルストレージ管理 | Pengurusan Storan Fail | 檔案儲存管理 |
| 管理存储策略、租户文件和文件访问策略 | (同key) | Manage storage strategies, tenant files, and file access policies | ストレージ戦略、テナントファイル、アクセスポリシーの管理 | Urus strategi storan, fail penyewa dan dasar akses fail | 管理儲存策略、租戶檔案和檔案存取策略 |
| 存储策略 | 存储策略 | Storage Strategies | ストレージ戦略 | Strategi Storan | 儲存策略 |
| 租户文件 | 租户文件 | Tenant Files | テナントファイル | Fail Penyewa | 租戶檔案 |
| 访问策略 | 访问策略 | Access Policies | アクセスポリシー | Dasar Akses | 存取策略 |
| 策略编码 | 策略编码 | Strategy Code | 戦略コード | Kod Strategi | 策略編碼 |
| 策略名称 | 策略名称 | Strategy Name | 戦略名 | Nama Strategi | 策略名稱 |
| 提供商类型 | 提供商类型 | Provider Type | プロバイダータイプ | Jenis Pembekal | 提供商類型 |
| 存储桶名称 | 存储桶名称 | Bucket Name | バケット名 | Nama Baldi | 儲存桶名稱 |
| 基础路径 | 基础路径 | Base Path | ベースパス | Laluan Asas | 基礎路徑 |
| 本地存储 | 本地存储 | Local Storage | ローカルストレージ | Storan Tempatan | 本地儲存 |
| Amazon S3 | Amazon S3 | Amazon S3 | Amazon S3 | Amazon S3 | Amazon S3 |
| 阿里云 OSS | 阿里云 OSS | Alibaba Cloud OSS | Alibaba Cloud OSS | Alibaba Cloud OSS | 阿里雲 OSS |
| Azure Blob | Azure Blob | Azure Blob | Azure Blob | Azure Blob | Azure Blob |
| 新增存储策略 | 新增存储策略 | Create Storage Strategy | ストレージ戦略作成 | Cipta Strategi Storan | 新增儲存策略 |
| 编辑存储策略 | 编辑存储策略 | Edit Storage Strategy | ストレージ戦略編集 | Edit Strategi Storan | 編輯儲存策略 |
| 策略详情 | 策略详情 | Strategy Details | 戦略詳細 | Butiran Strategi | 策略詳情 |
| 请输入策略编码或名称 | 请输入策略编码或名称 | Enter strategy code or name | 戦略コードまたは名前を入力 | Masukkan kod strategi atau nama | 請輸入策略編碼或名稱 |
| 请输入策略编码 | 请输入策略编码 | Enter strategy code | 戦略コードを入力 | Masukkan kod strategi | 請輸入策略編碼 |
| 请输入策略名称 | 请输入策略名称 | Enter strategy name | 戦略名を入力 | Masukkan nama strategi | 請輸入策略名稱 |
| 策略编码长度 2-50 个字符 | 策略编码长度 2-50 个字符 | Strategy code must be 2-50 characters | 戦略コードは2-50文字 | Kod strategi mestilah 2-50 aksara | 策略編碼長度 2-50 個字元 |
| 策略编码仅允许字母、数字、下划线和连字符 | 策略编码仅允许字母、数字、下划线和连字符 | Strategy code can only contain letters, numbers, underscores and hyphens | 戦略コードは英数字、アンダースコア、ハイフンのみ | Kod strategi hanya boleh mengandungi huruf, nombor, garis bawah dan sempang | 策略編碼僅允許字母、數字、底線和連字符 |
| 策略名称长度 2-100 个字符 | 策略名称长度 2-100 个字符 | Strategy name must be 2-100 characters | 戦略名は2-100文字 | Nama strategi mestilah 2-100 aksara | 策略名稱長度 2-100 個字元 |
| 请选择提供商类型 | 请选择提供商类型 | Select provider type | プロバイダータイプを選択 | Pilih jenis pembekal | 請選擇提供商類型 |
| 存储桶名称长度不超过 200 个字符 | 存储桶名称长度不超过 200 个字符 | Bucket name must not exceed 200 characters | バケット名は200文字以内 | Nama baldi mestilah tidak melebihi 200 aksara | 儲存桶名稱長度不超過 200 個字元 |
| 基础路径长度不超过 500 个字符 | 基础路径长度不超过 500 个字符 | Base path must not exceed 500 characters | ベースパスは500文字以内 | Laluan asas mestilah tidak melebihi 500 aksara | 基礎路徑長度不超過 500 個字元 |
| 确认启用策略 {name} | 确认启用策略 {name} | Confirm enable strategy {name} | 戦略 {name} を有効にしますか | Sahkan aktifkan strategi {name} | 確認啟用策略 {name} |
| 确认禁用策略 {name} | 确认禁用策略 {name} | Confirm disable strategy {name} | 戦略 {name} を無効にしますか | Sahkan lumpuhkan strategi {name} | 確認停用策略 {name} |
| 文件名 | 文件名 | File Name | ファイル名 | Nama Fail | 檔案名 |
| 扩展名 | 扩展名 | Extension | 拡張子 | Sambungan | 副檔名 |
| MIME 类型 | MIME 类型 | MIME Type | MIMEタイプ | Jenis MIME | MIME 類型 |
| 文件大小 | 文件大小 | File Size | ファイルサイズ | Saiz Fail | 檔案大小 |
| 上传者类型 | 上传者类型 | Uploader Type | アップロード者タイプ | Jenis Pemuat Naik | 上傳者類型 |
| 上传者ID | 上传者ID | Uploader ID | アップロード者ID | ID Pemuat Naik | 上傳者ID |
| 可见性 | 可见性 | Visibility | 可視性 | Keterlihatan | 可見性 |
| 公开 | 公开 | Public | 公開 | Awam | 公開 |
| 私有 | 私有 | Private | プライベート | Persendirian | 私有 |
| 下载次数 | 下载次数 | Download Count | ダウンロード回数 | Kiraan Muat Turun | 下載次數 |
| 请输入文件名 | 请输入文件名 | Enter file name | ファイル名を入力 | Masukkan nama fail | 請輸入檔案名 |
| 文件详情 | 文件详情 | File Details | ファイル詳細 | Butiran Fail | 檔案詳情 |
| 存储策略ID | 存储策略ID | Storage Strategy ID | ストレージ戦略ID | ID Strategi Storan | 儲存策略ID |
| 新增访问策略 | 新增访问策略 | Create Access Policy | アクセスポリシー作成 | Cipta Dasar Akses | 新增存取策略 |
| 文件ID | 文件ID | File ID | ファイルID | ID Fail | 檔案ID |
| 主体类型 | 主体类型 | Subject Type | 主体タイプ | Jenis Subjek | 主體類型 |
| 主体ID | 主体ID | Subject ID | 主体ID | ID Subjek | 主體ID |
| 权限编码 | 权限编码 | Permission Code | 権限コード | Kod Kebenaran | 權限編碼 |
| 读取 | 读取 | Read | 読み取り | Baca | 讀取 |
| 写入 | 写入 | Write | 書き込み | Tulis | 寫入 |
| 删除权限 | 删除权限 | Delete | 削除 | Padam | 刪除權限 |
| 管理员 | 管理员 | Admin | 管理者 | Pentadbir | 管理員 |
| 请输入文件ID | 请输入文件ID | Enter file ID | ファイルIDを入力 | Masukkan ID fail | 請輸入檔案ID |
| 请输入主体类型 | 请输入主体类型 | Enter subject type | 主体タイプを入力 | Masukkan jenis subjek | 請輸入主體類型 |
| 主体类型长度不超过 50 个字符 | 主体类型长度不超过 50 个字符 | Subject type must not exceed 50 characters | 主体タイプは50文字以内 | Jenis subjek mestilah tidak melebihi 50 aksara | 主體類型長度不超過 50 個字元 |
| 请选择权限编码 | 请选择权限编码 | Select permission code | 権限コードを選択 | Pilih kod kebenaran | 請選擇權限編碼 |
| 请输入主体类型或权限编码 | 请输入主体类型或权限编码 | Enter subject type or permission code | 主体タイプまたは権限コードを入力 | Masukkan jenis subjek atau kod kebenaran | 請輸入主體類型或權限編碼 |
| 租户ID | 租户ID | Tenant ID | テナントID | ID Penyewa | 租戶ID |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`编辑`、`删除`、`查看`、`确定`、`取消`、`操作`、`ID`、`状态`、`创建时间`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`更新成功`、`删除成功`、`启用成功`、`禁用成功`、`已启用`、`已禁用`、`请选择状态`、`请选择租户`、`关键词`、`启用`、`禁用`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"文件存储管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **DxTabPanel** 包含 3 个标签页：存储策略、租户文件、访问策略
- [ ] **Tab 1 表格列**包含：ID、策略编码、策略名称、提供商类型、存储桶名称、基础路径、状态、创建时间、操作
- [ ] **Tab 2 表格列**包含：ID、租户ID、文件名、扩展名、MIME 类型、文件大小、上传者类型、可见性、下载次数、创建时间、操作
- [ ] **Tab 3 表格列**包含：ID、文件ID、主体类型、主体ID、权限编码、创建时间
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 远程分页通过 CustomStore 实现
- [ ] **Tab 1 工具栏**包含：新增按钮
- [ ] **Tab 1 行操作**包含：查看、编辑、启用、禁用
- [ ] **Tab 2 行操作**包含：查看、删除
- [ ] **Tab 3 工具栏**包含：新增按钮
- [ ] **新增策略弹窗**包含字段：策略编码、策略名称、提供商类型、存储桶名称、基础路径
- [ ] **编辑策略弹窗**包含字段：策略编码（只读）、策略名称、提供商类型（只读）、存储桶名称、基础路径
- [ ] **策略详情弹窗**展示所有策略字段
- [ ] **文件详情弹窗**展示所有文件字段（不含 FilePath）
- [ ] **新增访问策略弹窗**包含字段：文件ID、主体类型、主体ID、权限编码

### P1 — 业务规则完整性

- [ ] 策略编码 `required` 验证
- [ ] 策略编码 `stringLength`（2-50）验证
- [ ] 策略编码 `pattern`（字母数字下划线连字符）验证
- [ ] 编辑时策略编码和提供商类型字段 disabled（不可修改）
- [ ] 策略名称 `required` 验证
- [ ] 策略名称 `stringLength`（2-100）验证
- [ ] 提供商类型 `required` 验证
- [ ] 文件ID `required` 验证
- [ ] 主体类型 `required` 验证
- [ ] 权限编码 `required` 验证
- [ ] 启用/禁用按钮根据当前状态动态显隐
- [ ] 每个操作按钮有权限码控制
- [ ] 删除文件有 `confirmDelete` 确认
- [ ] 危险操作有 `confirmAction` 确认
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗
- [ ] 文件路径（FilePath）不展示在前端
- [ ] 文件大小格式化为可读的 KB/MB/GB

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' StorageView.vue | grep -v ':caption'` 结果为 0
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] `grep -rn "notifySuccess(t(" StorageView.vue` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有状态显示值已国际化
- [ ] 所有提供商类型显示值已国际化（4 种类型）
- [ ] 所有权限编码显示值已国际化（4 种权限）
- [ ] 所有可见性显示值已国际化（2 种可见性）
- [ ] 所有确认框文案已国际化
- [ ] 所有成功提示已国际化
- [ ] 所有标签页标题已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
