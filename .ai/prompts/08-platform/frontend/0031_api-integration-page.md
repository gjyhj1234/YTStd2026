# 租户平台 — API 集成管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块为 API 集成管理，包含 API 密钥管理、API 使用统计、Webhook 管理、Webhook 投递日志四个标签页。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-12 |
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
- `src/YTStdTenantPlatform/Endpoints/ApiIntegrationEndpoints.cs` — 后端 API 端点定义
- `src/YTStdTenantPlatform/Application/Dtos/ApiIntegration/ApiIntegrationRepDTO.cs` — 后端 DTO 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 四个标签页列表远程分页 |
| DxTabPanel | `DxTabPanel items selectedIndex onSelectionChanged` | 四标签页切换（API Keys / 使用统计 / Webhooks / 投递日志） |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 创建 API Key / 创建编辑 Webhook 表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 创建弹窗 / 密钥展示弹窗 / 详情弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 状态筛选、租户筛选 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `ApiIntegrationEndpoints.cs` 中的路由注册。

### API 密钥端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 密钥列表 | GET | `/api/tenant-api-keys?page=1&pageSize=20&keyword=&tenantRefId=` | - | `ApiResult<PagedResult<TenantApiKeyRepDTO>>` |
| 创建密钥 | POST | `/api/tenant-api-keys` | `CreateApiKeyReqDTO` | `ApiResult<long>` |
| 密钥详情 | GET | `/api/tenant-api-keys/{id}` | - | `ApiResult<TenantApiKeyRepDTO>` |
| 删除密钥 | DELETE | `/api/tenant-api-keys/{id}` | - | `ApiResult` |
| 禁用密钥 | PUT | `/api/tenant-api-keys/{id}/disable` | - | `ApiResult` |

### 使用统计端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 使用统计列表 | GET | `/api/tenant-api-usage-stats?page=1&pageSize=20&keyword=&tenantRefId=` | - | `ApiResult<PagedResult<TenantApiUsageStatRepDTO>>` |

### Webhook 端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 事件定义列表 | GET | `/api/webhook-events?page=1&pageSize=20&keyword=` | - | `ApiResult<PagedResult<WebhookEventRepDTO>>` |
| Webhook 列表 | GET | `/api/tenant-webhooks?page=1&pageSize=20&keyword=&tenantRefId=` | - | `ApiResult<PagedResult<TenantWebhookRepDTO>>` |
| 创建 Webhook | POST | `/api/tenant-webhooks` | `CreateWebhookReqDTO` | `ApiResult<long>` |
| 更新 Webhook | PUT | `/api/tenant-webhooks/{id}` | `UpdateWebhookReqDTO` | `ApiResult` |
| 启用 Webhook | PUT | `/api/tenant-webhooks/{id}/enable` | - | `ApiResult` |
| 禁用 Webhook | PUT | `/api/tenant-webhooks/{id}/disable` | - | `ApiResult` |

### 投递日志端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 投递日志列表 | GET | `/api/webhook-delivery-logs?page=1&pageSize=20&keyword=&webhookId=` | - | `ApiResult<PagedResult<WebhookDeliveryLogRepDTO>>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/api-integration/ApiIntegrationView.vue` | 主页面（含 4 个标签页） |
| 2 | `src/WebTenantPlatfrom/src/views/api-integration/ApiIntegrationView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/api-integration/ApiIntegrationView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/api-integration/ApiIntegrationView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/api-integration/ApiIntegrationView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/api-integration/ApiIntegrationView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/api-integration.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/api-integration.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('API 集成管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理 API 密钥、查看使用统计、配置 Webhook 和投递日志')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| 标签页容器 | `DxTabPanel` | 4 个标签页切换 |
| Tab 1: API 密钥 | DxDataGrid + CustomStore | API 密钥列表 + 工具栏 |
| Tab 2: 使用统计 | DxDataGrid + CustomStore | API 使用统计列表（只读） |
| Tab 3: Webhook 管理 | DxDataGrid + CustomStore | Webhook 列表 + 工具栏 |
| Tab 4: Webhook 事件 | DxDataGrid + CustomStore | Webhook 事件定义列表（只读） |
| 新增密钥弹窗 | DxPopup + DxForm | 创建 API 密钥表单 |
| 密钥展示弹窗 | DxPopup（只读） | 创建成功后展示完整密钥（仅此一次） |
| 新增 Webhook 弹窗 | DxPopup + DxForm | 创建 Webhook 表单 |
| 编辑 Webhook 弹窗 | DxPopup + DxForm | 编辑 Webhook 表单 |
| 投递日志弹窗 | DxPopup + DxDataGrid | 查看某 Webhook 的投递日志 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### Tab 1: API 密钥 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入密钥名称')` |
| 2 | TenantRefId | `$t('租户')` | DxSelectBox | `null`（全部） | `$t('请选择租户')` |

### Tab 2: 使用统计 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入 API 路径')` |
| 2 | TenantRefId | `$t('租户')` | DxSelectBox | `null`（全部） | `$t('请选择租户')` |

### Tab 3: Webhook 管理 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入 Webhook 名称')` |
| 2 | TenantRefId | `$t('租户')` | DxSelectBox | `null`（全部） | `$t('请选择租户')` |

### Tab 4: Webhook 事件 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入事件编码或名称')` |

### 查询行为

| 行为 | 要求 |
|------|------|
| 回车搜索 | 在关键词输入框回车触发搜索 |
| 查询按钮 | `$t('查询')`，点击触发搜索 |
| 重置按钮 | `$t('重置')`，清空所有条件并重新加载 |
| 所有文本国际化 | 所有 label、placeholder、按钮文本均使用 `$t()` |

---

## 列表与分页

### Tab 1: API 密钥列表

**表格组件**：DxDataGrid + CustomStore

#### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | 固定宽度 |
| 2 | TenantRefId | `$t('租户ID')` | 100px | 否 | - | |
| 3 | KeyName | `$t('密钥名称')` | auto | 是 | - | |
| 4 | AccessKey | `$t('访问密钥')` | 200px | 否 | `maskedKeyCell` | 仅显示前 8 位 + `****` |
| 5 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 6 | QuotaLimit | `$t('配额上限')` | 100px | 否 | - | |
| 7 | RateLimit | `$t('速率限制')` | 100px | 否 | - | |
| 8 | LastUsedAt | `$t('最后使用时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 9 | ExpiresAt | `$t('过期时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 10 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 11 | - | `$t('操作')` | 200px | 否 | `actionCell` | 操作按钮列 |

### Tab 2: 使用统计列表

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | TenantRefId | `$t('租户ID')` | 100px | 否 | - | |
| 3 | ApiKeyId | `$t('密钥ID')` | 100px | 否 | - | |
| 4 | StatDate | `$t('统计日期')` | 120px | 是 | `dateCell` | `yyyy-MM-dd` |
| 5 | ApiPath | `$t('API 路径')` | auto | 否 | - | |
| 6 | RequestCount | `$t('请求次数')` | 100px | 是 | - | |
| 7 | SuccessCount | `$t('成功次数')` | 100px | 否 | - | |
| 8 | ErrorCount | `$t('错误次数')` | 100px | 否 | - | |
| 9 | AverageLatencyMs | `$t('平均延迟(ms)')` | 120px | 否 | - | |
| 10 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |

### Tab 3: Webhook 管理列表

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | TenantRefId | `$t('租户ID')` | 100px | 否 | - | |
| 3 | WebhookName | `$t('Webhook 名称')` | auto | 是 | - | |
| 4 | TargetUrl | `$t('目标地址')` | auto | 否 | - | |
| 5 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 6 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 7 | - | `$t('操作')` | 380px | 否 | `actionCell` | 操作按钮列 |

### Tab 4: Webhook 事件列表

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | EventCode | `$t('事件编码')` | 150px | 是 | - | |
| 3 | EventName | `$t('事件名称')` | auto | 是 | - | |
| 4 | Description | `$t('描述')` | auto | 否 | - | |
| 5 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |

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

### 状态列颜色（API 密钥 / Webhook 通用）

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| enabled | `$t('已启用')` | 绿色 `#52c41a` | `status-enabled` |
| disabled | `$t('已禁用')` | 红色 `#f5222d` | `status-disabled` |

### 投递状态列颜色

| 状态值 | 显示文本 | 标签颜色 |
|:------:|---------|---------|
| success | `$t('成功')` | 绿色 `#52c41a` |
| failed | `$t('失败')` | 红色 `#f5222d` |
| pending | `$t('待投递')` | 灰色 `#8c8c8c` |

### 空状态与加载

| 状态 | 显示 |
|------|------|
| 空数据 | `:no-data-text="$t('暂无数据')"` |
| 加载中 | `DxLoadPanel`（`visible` 绑定 `pageLoading`） |

---

## 操作按钮

### Tab 1: API 密钥 — 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增 | `$t('新增')` | `add` | `API_KEY_CREATE` | 始终启用 | 打开新增密钥弹窗 | 无 |

### Tab 1: API 密钥 — 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `API_KEY_VIEW` | 始终 | 打开详情弹窗 | 无 |
| 禁用 | `$t('禁用')` | `close` | `API_KEY_DISABLE` | `Status === 'enabled'` | 调用禁用 API | `confirmAction('确认禁用密钥 {name}')` |
| 删除 | `$t('删除')` | `trash` | `API_KEY_DELETE` | 始终 | 调用删除 API | `confirmDelete(row.KeyName)` |

### Tab 3: Webhook 管理 — 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增 | `$t('新增')` | `add` | `WEBHOOK_CREATE` | 始终启用 | 打开新增 Webhook 弹窗 | 无 |

### Tab 3: Webhook 管理 — 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 编辑 | `$t('编辑')` | `edit` | `WEBHOOK_UPDATE` | 始终 | 打开编辑弹窗 | 无 |
| 启用 | `$t('启用')` | `check` | `WEBHOOK_ENABLE` | `Status === 'disabled'` | 调用启用 API | `confirmAction('确认启用 Webhook {name}')` |
| 禁用 | `$t('禁用')` | `close` | `WEBHOOK_DISABLE` | `Status === 'enabled'` | 调用禁用 API | `confirmAction('确认禁用 Webhook {name}')` |
| 投递日志 | `$t('投递日志')` | `description` | `WEBHOOK_VIEW` | 始终 | 打开投递日志弹窗 | 无 |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const API_KEY_VIEW = 'api-key.detail'
export const API_KEY_CREATE = 'api-key.create'
export const API_KEY_DISABLE = 'api-key.disable'
export const API_KEY_DELETE = 'api-key.delete'
export const WEBHOOK_VIEW = 'webhook.detail'
export const WEBHOOK_CREATE = 'webhook.create'
export const WEBHOOK_UPDATE = 'webhook.update'
export const WEBHOOK_ENABLE = 'webhook.enable'
export const WEBHOOK_DISABLE = 'webhook.disable'
```

### 确认框文案（全部国际化）

| 操作 | 文案 key | 说明 |
|------|---------|------|
| 禁用密钥 | `'确认禁用密钥 {name}'` | 禁用后不可调用 API |
| 删除密钥 | `confirmDelete(row.KeyName)` | 使用通用删除确认 |
| 启用 Webhook | `'确认启用 Webhook {name}'` | |
| 禁用 Webhook | `'确认禁用 Webhook {name}'` | |

### 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建密钥 | `notifySuccess('创建成功')` |
| 禁用密钥 | `notifySuccess('禁用成功')` |
| 删除密钥 | `notifySuccess('删除成功')` |
| 创建 Webhook | `notifySuccess('创建成功')` |
| 更新 Webhook | `notifySuccess('更新成功')` |
| 启用 Webhook | `notifySuccess('启用成功')` |
| 禁用 Webhook | `notifySuccess('禁用成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 新增 API 密钥表单

**标题**：`$t('新增 API 密钥')`
**组件**：`DxPopup`（`width: 500`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantRefId | `$t('关联租户')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 2 | KeyName | `$t('密钥名称')` | DxTextBox | 是 | 2-100 | - | - | `''` | - | - |
| 3 | ExpiresAt | `$t('过期时间')` | DxDateBox | 否 | - | 需晚于当前时间 | - | `null` | - | - |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TenantRefId | required | - | `$t('请选择关联租户')` |
| KeyName | required | - | `$t('请输入密钥名称')` |
| KeyName | stringLength | min: 2, max: 100 | `$t('密钥名称长度 2-100 个字符')` |

**提交行为**：

1. 提交前：调用 `formInstance.validate()`，不通过则阻止提交
2. 提交时：`submitting.value = true`，禁用提交按钮并显示 loading
3. 提交成功：关闭新增弹窗 → **打开密钥展示弹窗**（展示完整 AccessKey，提示用户立即保存） → 刷新列表
4. 提交失败：axios 拦截器自动显示错误 → 不关闭弹窗 → `submitting.value = false`

**密钥展示弹窗**：

- 创建成功后显示完整 AccessKey，提示文本：`$t('API 密钥仅在创建时展示一次，请立即保存')`
- 提供复制按钮：`$t('复制密钥')`
- 关闭后不可再次查看完整密钥

### 新增 Webhook 表单

**标题**：`$t('新增 Webhook')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantRefId | `$t('关联租户')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 2 | WebhookName | `$t('Webhook 名称')` | DxTextBox | 是 | 2-100 | - | - | `''` | - | - |
| 3 | TargetUrl | `$t('目标地址')` | DxTextBox | 是 | 1-500 | URL 格式 | - | `''` | - | - |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TenantRefId | required | - | `$t('请选择关联租户')` |
| WebhookName | required | - | `$t('请输入 Webhook 名称')` |
| WebhookName | stringLength | min: 2, max: 100 | `$t('Webhook 名称长度 2-100 个字符')` |
| TargetUrl | required | - | `$t('请输入目标地址')` |
| TargetUrl | pattern | URL 正则 | `$t('请输入有效的 URL 地址')` |

**提交行为**：

1. 提交前：调用 `formInstance.validate()`，不通过则阻止提交
2. 提交时：`submitting.value = true`，禁用提交按钮并显示 loading
3. 提交成功：关闭弹窗 → `notifySuccess('创建成功')` → 刷新列表
4. 提交失败：axios 拦截器自动显示错误 → 不关闭弹窗 → `submitting.value = false`

### 编辑 Webhook 表单

**标题**：`$t('编辑 Webhook')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | WebhookName | `$t('Webhook 名称')` | DxTextBox | 否 | 2-100 | - | - | 从数据加载 | - | - |
| 2 | TargetUrl | `$t('目标地址')` | DxTextBox | 否 | 1-500 | URL 格式 | - | 从数据加载 | - | - |

**注意**：编辑接口 `UpdateWebhookReqDTO` 不含 `TenantRefId` 字段。

**提交行为**：同新增 Webhook 表单，成功提示为 `notifySuccess('更新成功')`。

**弹窗按钮（所有弹窗通用）**：

| 按钮 | 文本 | 位置 | 行为 |
|------|------|------|------|
| 提交 | `$t('确定')` | 弹窗底部右侧 | 提交表单，`submitting` 时 disabled + loading |
| 取消 | `$t('取消')` | 弹窗底部左侧 | 关闭弹窗，重置表单 |

### 类型定义

```typescript
// src/types/api-integration.ts

/** API 密钥响应 */
export interface TenantApiKeyRepDTO {
  Id: number
  TenantRefId: number
  KeyName: string
  AccessKey: string
  Status: string
  QuotaLimit?: number
  RateLimit?: number
  LastUsedAt?: string
  ExpiresAt?: string
  CreatedAt: string
}

/** 创建 API 密钥请求 */
export interface CreateApiKeyReqDTO {
  TenantRefId: number
  KeyName: string
  ExpiresAt?: string
}

/** API 使用统计响应 */
export interface TenantApiUsageStatRepDTO {
  Id: number
  TenantRefId: number
  ApiKeyId?: number
  StatDate: string
  ApiPath: string
  RequestCount: number
  SuccessCount: number
  ErrorCount: number
  AverageLatencyMs: number
  CreatedAt: string
}

/** Webhook 事件响应 */
export interface WebhookEventRepDTO {
  Id: number
  EventCode: string
  EventName: string
  Description?: string
  CreatedAt: string
}

/** Webhook 响应 */
export interface TenantWebhookRepDTO {
  Id: number
  TenantRefId: number
  WebhookName: string
  TargetUrl: string
  Status: string
  CreatedAt: string
}

/** 创建 Webhook 请求 */
export interface CreateWebhookReqDTO {
  TenantRefId: number
  WebhookName: string
  TargetUrl: string
}

/** 更新 Webhook 请求 */
export interface UpdateWebhookReqDTO {
  WebhookName?: string
  TargetUrl?: string
}

/** Webhook 投递日志响应 */
export interface WebhookDeliveryLogRepDTO {
  Id: number
  WebhookId: number
  EventId?: number
  DeliveryStatus: string
  ResponseStatusCode?: number
  RetryCount: number
  DeliveredAt?: string
  CreatedAt: string
}
```

---

## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| 无独立静态配置文件 | 状态字典、列定义均内联在页面组件中 | 页面组件级语言文件 |

> 本模块状态选项简单（仅 enabled / disabled），不需要独立配置文件。

---

## 国际化要求

### 组件级 key（放入 `ApiIntegrationView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| API 集成管理 | API 集成管理 | API Integration | API統合管理 | Pengurusan Integrasi API | API 整合管理 |
| 管理 API 密钥、查看使用统计、配置 Webhook 和投递日志 | (同key) | Manage API keys, view usage statistics, configure webhooks and delivery logs | APIキーの管理、利用統計の表示、Webhookの設定と配信ログ | Urus kunci API, lihat statistik penggunaan, konfigurasi webhook dan log penghantaran | 管理 API 金鑰、查看使用統計、配置 Webhook 和投遞日誌 |
| API 密钥 | API 密钥 | API Keys | APIキー | Kunci API | API 金鑰 |
| 使用统计 | 使用统计 | Usage Statistics | 利用統計 | Statistik Penggunaan | 使用統計 |
| Webhook 管理 | Webhook 管理 | Webhook Management | Webhook管理 | Pengurusan Webhook | Webhook 管理 |
| Webhook 事件 | Webhook 事件 | Webhook Events | Webhookイベント | Acara Webhook | Webhook 事件 |
| 密钥名称 | 密钥名称 | Key Name | キー名 | Nama Kunci | 金鑰名稱 |
| 访问密钥 | 访问密钥 | Access Key | アクセスキー | Kunci Akses | 存取金鑰 |
| 配额上限 | 配额上限 | Quota Limit | クォータ上限 | Had Kuota | 配額上限 |
| 速率限制 | 速率限制 | Rate Limit | レート制限 | Had Kadar | 速率限制 |
| 最后使用时间 | 最后使用时间 | Last Used At | 最終使用日時 | Masa Terakhir Digunakan | 最後使用時間 |
| 过期时间 | 过期时间 | Expires At | 有効期限 | Masa Tamat Tempoh | 過期時間 |
| 请输入密钥名称 | 请输入密钥名称 | Enter key name | キー名を入力 | Masukkan nama kunci | 請輸入金鑰名稱 |
| 密钥名称长度 2-100 个字符 | 密钥名称长度 2-100 个字符 | Key name must be 2-100 characters | キー名は2-100文字 | Nama kunci mestilah 2-100 aksara | 金鑰名稱長度 2-100 個字元 |
| 请选择关联租户 | 请选择关联租户 | Select tenant | テナントを選択 | Pilih penyewa | 請選擇關聯租戶 |
| 关联租户 | 关联租户 | Tenant | テナント | Penyewa | 關聯租戶 |
| 租户ID | 租户ID | Tenant ID | テナントID | ID Penyewa | 租戶ID |
| 新增 API 密钥 | 新增 API 密钥 | Create API Key | APIキー作成 | Cipta Kunci API | 新增 API 金鑰 |
| API 密钥仅在创建时展示一次，请立即保存 | (同key) | API key is shown only once at creation. Please save it immediately | APIキーは作成時に一度だけ表示されます。すぐに保存してください | Kunci API hanya dipaparkan sekali semasa penciptaan. Sila simpan segera | API 金鑰僅在建立時展示一次，請立即保存 |
| 复制密钥 | 复制密钥 | Copy Key | キーをコピー | Salin Kunci | 複製金鑰 |
| 确认禁用密钥 {name} | 确认禁用密钥 {name} | Confirm disable key {name} | キー {name} を無効にしますか | Sahkan lumpuhkan kunci {name} | 確認停用金鑰 {name} |
| Webhook 名称 | Webhook 名称 | Webhook Name | Webhook名 | Nama Webhook | Webhook 名稱 |
| 目标地址 | 目标地址 | Target URL | ターゲットURL | URL Sasaran | 目標地址 |
| 新增 Webhook | 新增 Webhook | Create Webhook | Webhook作成 | Cipta Webhook | 新增 Webhook |
| 编辑 Webhook | 编辑 Webhook | Edit Webhook | Webhook編集 | Edit Webhook | 編輯 Webhook |
| 请输入 Webhook 名称 | 请输入 Webhook 名称 | Enter webhook name | Webhook名を入力 | Masukkan nama webhook | 請輸入 Webhook 名稱 |
| Webhook 名称长度 2-100 个字符 | Webhook 名称长度 2-100 个字符 | Webhook name must be 2-100 characters | Webhook名は2-100文字 | Nama webhook mestilah 2-100 aksara | Webhook 名稱長度 2-100 個字元 |
| 请输入目标地址 | 请输入目标地址 | Enter target URL | ターゲットURLを入力 | Masukkan URL sasaran | 請輸入目標地址 |
| 请输入有效的 URL 地址 | 请输入有效的 URL 地址 | Please enter a valid URL | 有効なURLを入力してください | Sila masukkan URL yang sah | 請輸入有效的 URL 地址 |
| 确认启用 Webhook {name} | 确认启用 Webhook {name} | Confirm enable webhook {name} | Webhook {name} を有効にしますか | Sahkan aktifkan webhook {name} | 確認啟用 Webhook {name} |
| 确认禁用 Webhook {name} | 确认禁用 Webhook {name} | Confirm disable webhook {name} | Webhook {name} を無効にしますか | Sahkan lumpuhkan webhook {name} | 確認停用 Webhook {name} |
| 投递日志 | 投递日志 | Delivery Logs | 配信ログ | Log Penghantaran | 投遞日誌 |
| 投递状态 | 投递状态 | Delivery Status | 配信ステータス | Status Penghantaran | 投遞狀態 |
| 响应状态码 | 响应状态码 | Response Code | レスポンスコード | Kod Respons | 回應狀態碼 |
| 重试次数 | 重试次数 | Retry Count | リトライ回数 | Kiraan Cuba Semula | 重試次數 |
| 投递时间 | 投递时间 | Delivered At | 配信日時 | Masa Penghantaran | 投遞時間 |
| 统计日期 | 统计日期 | Stat Date | 統計日 | Tarikh Statistik | 統計日期 |
| API 路径 | API 路径 | API Path | APIパス | Laluan API | API 路徑 |
| 请输入 API 路径 | 请输入 API 路径 | Enter API path | APIパスを入力 | Masukkan laluan API | 請輸入 API 路徑 |
| 请求次数 | 请求次数 | Request Count | リクエスト数 | Kiraan Permintaan | 請求次數 |
| 成功次数 | 成功次数 | Success Count | 成功数 | Kiraan Berjaya | 成功次數 |
| 错误次数 | 错误次数 | Error Count | エラー数 | Kiraan Ralat | 錯誤次數 |
| 平均延迟(ms) | 平均延迟(ms) | Avg Latency(ms) | 平均レイテンシ(ms) | Purata Latensi(ms) | 平均延遲(ms) |
| 密钥ID | 密钥ID | Key ID | キーID | ID Kunci | 金鑰ID |
| 事件编码 | 事件编码 | Event Code | イベントコード | Kod Acara | 事件編碼 |
| 事件名称 | 事件名称 | Event Name | イベント名 | Nama Acara | 事件名稱 |
| 请输入事件编码或名称 | 请输入事件编码或名称 | Enter event code or name | イベントコードまたは名前を入力 | Masukkan kod acara atau nama | 請輸入事件編碼或名稱 |
| 成功 | 成功 | Success | 成功 | Berjaya | 成功 |
| 失败 | 失败 | Failed | 失敗 | Gagal | 失敗 |
| 待投递 | 待投递 | Pending | 待配信 | Menunggu | 待投遞 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`编辑`、`删除`、`查看`、`确定`、`取消`、`操作`、`ID`、`状态`、`创建时间`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`更新成功`、`删除成功`、`启用成功`、`禁用成功`、`已启用`、`已禁用`、`请选择租户`、`关键词`、`描述`、`启用`、`禁用`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"API 集成管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **DxTabPanel** 包含 4 个标签页：API 密钥、使用统计、Webhook 管理、Webhook 事件
- [ ] **Tab 1 表格列**包含：ID、租户ID、密钥名称、访问密钥（脱敏）、状态、配额上限、速率限制、最后使用时间、过期时间、创建时间、操作
- [ ] **Tab 2 表格列**包含：ID、租户ID、密钥ID、统计日期、API 路径、请求次数、成功次数、错误次数、平均延迟、创建时间
- [ ] **Tab 3 表格列**包含：ID、租户ID、Webhook 名称、目标地址、状态、创建时间、操作
- [ ] **Tab 4 表格列**包含：ID、事件编码、事件名称、描述、创建时间
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 远程分页通过 CustomStore 实现
- [ ] **Tab 1 工具栏**包含：新增按钮
- [ ] **Tab 1 行操作**包含：查看、禁用、删除
- [ ] **Tab 3 工具栏**包含：新增按钮
- [ ] **Tab 3 行操作**包含：编辑、启用、禁用、投递日志
- [ ] **新增密钥弹窗**包含字段：关联租户、密钥名称、过期时间
- [ ] **密钥展示弹窗**创建成功后展示完整密钥 + 复制按钮 + 警告提示
- [ ] **新增 Webhook 弹窗**包含字段：关联租户、Webhook 名称、目标地址
- [ ] **编辑 Webhook 弹窗**包含字段：Webhook 名称、目标地址
- [ ] **投递日志弹窗**展示某 Webhook 的投递日志列表

### P1 — 业务规则完整性

- [ ] 密钥名称 `required` 验证
- [ ] 密钥名称 `stringLength`（2-100）验证
- [ ] 关联租户 `required` 验证
- [ ] API 密钥创建成功后展示完整 AccessKey，仅此一次
- [ ] 列表中 AccessKey 脱敏显示（前 8 位 + `****`）
- [ ] 禁用密钥后该密钥不可调用 API
- [ ] Webhook 名称 `required` 验证
- [ ] Webhook 名称 `stringLength`（2-100）验证
- [ ] 目标地址 `required` 验证
- [ ] 目标地址 URL 格式验证
- [ ] 启用/禁用按钮根据当前状态动态显隐
- [ ] 每个操作按钮有权限码控制
- [ ] 删除有 `confirmDelete` 确认
- [ ] 危险操作有 `confirmAction` 确认
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗
- [ ] Tab 2（使用统计）和 Tab 4（Webhook 事件）为只读列表，无增删改操作

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' ApiIntegrationView.vue | grep -v ':caption'` 结果为 0
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] `grep -rn "notifySuccess(t(" ApiIntegrationView.vue` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有状态显示值已国际化
- [ ] 所有确认框文案已国际化
- [ ] 所有成功提示已国际化
- [ ] 所有标签页标题已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
