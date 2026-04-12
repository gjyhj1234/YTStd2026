# 租户平台 — 通知管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块为通知管理，包含通知模板管理和通知记录两个标签页。模板使用整数 Code（零文本设计），通知内容由前端根据 Code 翻译。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-14 |
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
- `src/YTStdTenantPlatform/Endpoints/NotificationEndpoints.cs` — 后端 API 端点定义
- `src/YTStdTenantPlatform/Application/Dtos/Notification/NotificationRepDTO.cs` — 后端响应 DTO 定义
- `src/YTStdTenantPlatform/Application/Dtos/Notification/NotificationReqDTO.cs` — 后端请求 DTO 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 两个标签页列表远程分页 |
| DxTabPanel | `DxTabPanel items selectedIndex onSelectionChanged` | 两标签页切换（模板 / 通知记录） |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 创建/编辑模板、发送通知表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 创建/编辑/详情弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxTextArea | `DxTextArea height min-height auto-resize-enabled` | 正文模板输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 渠道选择、状态筛选 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `NotificationEndpoints.cs` 中的路由注册。

### 通知模板端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 模板列表 | GET | `/api/notification-templates?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<NotificationTemplateRepDTO>>` |
| 模板详情 | GET | `/api/notification-templates/{id}` | - | `ApiResult<NotificationTemplateRepDTO>` |
| 创建模板 | POST | `/api/notification-templates` | `CreateNotificationTemplateReqDTO` | `ApiResult<long>` |
| 更新模板 | PUT | `/api/notification-templates/{id}` | `UpdateNotificationTemplateReqDTO` | `ApiResult` |
| 启用模板 | PUT | `/api/notification-templates/{id}/enable` | - | `ApiResult` |
| 禁用模板 | PUT | `/api/notification-templates/{id}/disable` | - | `ApiResult` |
| 删除模板 | DELETE | `/api/notification-templates/{id}` | - | `ApiResult` |

### 通知记录端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 通知列表 | GET | `/api/notifications?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<NotificationRepDTO>>` |
| 通知详情 | GET | `/api/notifications/{id}` | - | `ApiResult<NotificationRepDTO>` |
| 发送通知 | POST | `/api/notifications` | `CreateNotificationReqDTO` | `ApiResult<long>` |
| 标记已读 | PUT | `/api/notifications/{id}/read` | - | `ApiResult` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/notification/NotificationView.vue` | 主页面（含 2 个标签页） |
| 2 | `src/WebTenantPlatfrom/src/views/notification/NotificationView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/notification/NotificationView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/notification/NotificationView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/notification/NotificationView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/notification/NotificationView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/notification.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/notification.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('通知管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理通知模板和通知记录，支持站内通知')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| 标签页容器 | `DxTabPanel` | 2 个标签页切换 |
| Tab 1: 通知模板 | DxDataGrid + CustomStore | 模板列表 + 工具栏 |
| Tab 2: 通知记录 | DxDataGrid + CustomStore | 通知列表 + 工具栏 |
| 新增模板弹窗 | DxPopup + DxForm | 创建通知模板表单 |
| 编辑模板弹窗 | DxPopup + DxForm | 编辑通知模板表单 |
| 模板详情弹窗 | DxPopup（只读展示） | 模板详情 |
| 发送通知弹窗 | DxPopup + DxForm | 发送通知表单 |
| 通知详情弹窗 | DxPopup（只读展示） | 通知详情 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### Tab 1: 通知模板 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入模板编码或名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| enabled | `$t('已启用')` |
| disabled | `$t('已禁用')` |

### Tab 2: 通知记录 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入接收人或主题')` |
| 2 | Status | `$t('发送状态')` | DxSelectBox | `null`（全部） | `$t('请选择发送状态')` |

**发送状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| pending | `$t('待发送')` |
| sent | `$t('已发送')` |
| read | `$t('已读')` |
| failed | `$t('发送失败')` |

### 查询行为

| 行为 | 要求 |
|------|------|
| 回车搜索 | 在关键词输入框回车触发搜索 |
| 查询按钮 | `$t('查询')`，点击触发搜索 |
| 重置按钮 | `$t('重置')`，清空所有条件并重新加载 |
| 所有文本国际化 | 所有 label、placeholder、按钮文本均使用 `$t()` |

---

## 列表与分页

### Tab 1: 通知模板列表

**表格组件**：DxDataGrid + CustomStore

#### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | 固定宽度 |
| 2 | TemplateCode | `$t('模板编码')` | 150px | 是 | - | |
| 3 | TemplateName | `$t('模板名称')` | auto | 是 | - | |
| 4 | Channel | `$t('通知渠道')` | 120px | 否 | `channelCell` | 标签显示 |
| 5 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 6 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 7 | - | `$t('操作')` | 380px | 否 | `actionCell` | 操作按钮列 |

### Tab 2: 通知记录列表

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | TenantRefId | `$t('租户ID')` | 100px | 否 | - | |
| 3 | Channel | `$t('通知渠道')` | 100px | 否 | `channelCell` | 标签显示 |
| 4 | Recipient | `$t('接收人')` | 150px | 否 | - | |
| 5 | Subject | `$t('主题')` | auto | 是 | - | |
| 6 | SendStatus | `$t('发送状态')` | 100px | 否 | `sendStatusCell` | 颜色标签 |
| 7 | SentAt | `$t('发送时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 8 | ReadAt | `$t('阅读时间')` | 180px | 否 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 9 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 10 | - | `$t('操作')` | 200px | 否 | `actionCell` | 操作按钮列 |

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

### 模板状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| enabled | `$t('已启用')` | 绿色 `#52c41a` | `status-enabled` |
| disabled | `$t('已禁用')` | 红色 `#f5222d` | `status-disabled` |

### 发送状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| pending | `$t('待发送')` | 灰色 `#8c8c8c` | `send-pending` |
| sent | `$t('已发送')` | 蓝色 `#1890ff` | `send-sent` |
| read | `$t('已读')` | 绿色 `#52c41a` | `send-read` |
| failed | `$t('发送失败')` | 红色 `#f5222d` | `send-failed` |

### 通知渠道标签

| 渠道值 | 显示文本 |
|:------:|---------|
| email | `$t('邮件')` |
| sms | `$t('短信')` |
| webhook | `$t('Webhook')` |
| in_app | `$t('站内通知')` |

### 空状态与加载

| 状态 | 显示 |
|------|------|
| 空数据 | `:no-data-text="$t('暂无数据')"` |
| 加载中 | `DxLoadPanel`（`visible` 绑定 `pageLoading`） |

---

## 操作按钮

### Tab 1: 通知模板 — 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增 | `$t('新增')` | `add` | `NOTIFICATION_TEMPLATE_CREATE` | 始终启用 | 打开新增模板弹窗 | 无 |

### Tab 1: 通知模板 — 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `NOTIFICATION_TEMPLATE_VIEW` | 始终 | 打开详情弹窗 | 无 |
| 编辑 | `$t('编辑')` | `edit` | `NOTIFICATION_TEMPLATE_UPDATE` | 始终 | 打开编辑弹窗 | 无 |
| 启用 | `$t('启用')` | `check` | `NOTIFICATION_TEMPLATE_ENABLE` | `Status === 'disabled'` | 调用启用 API | `confirmAction('确认启用模板 {name}')` |
| 禁用 | `$t('禁用')` | `close` | `NOTIFICATION_TEMPLATE_DISABLE` | `Status === 'enabled'` | 调用禁用 API | `confirmAction('确认禁用模板 {name}')` |
| 删除 | `$t('删除')` | `trash` | `NOTIFICATION_TEMPLATE_DELETE` | 始终 | 调用删除 API | `confirmDelete(row.TemplateName)` |

### Tab 2: 通知记录 — 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 发送通知 | `$t('发送通知')` | `message` | `NOTIFICATION_CREATE` | 始终启用 | 打开发送通知弹窗 | 无 |

### Tab 2: 通知记录 — 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `NOTIFICATION_VIEW` | 始终 | 打开通知详情弹窗 | 无 |
| 标记已读 | `$t('标记已读')` | `check` | `NOTIFICATION_UPDATE` | `SendStatus !== 'read'` | 调用标记已读 API | `confirmAction('确认标记该通知为已读')` |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const NOTIFICATION_TEMPLATE_VIEW = 'notification-template.detail'
export const NOTIFICATION_TEMPLATE_CREATE = 'notification-template.create'
export const NOTIFICATION_TEMPLATE_UPDATE = 'notification-template.update'
export const NOTIFICATION_TEMPLATE_ENABLE = 'notification-template.enable'
export const NOTIFICATION_TEMPLATE_DISABLE = 'notification-template.disable'
export const NOTIFICATION_TEMPLATE_DELETE = 'notification-template.delete'
export const NOTIFICATION_VIEW = 'notification.detail'
export const NOTIFICATION_CREATE = 'notification.create'
export const NOTIFICATION_UPDATE = 'notification.update'
```

### 确认框文案（全部国际化）

| 操作 | 文案 key | 说明 |
|------|---------|------|
| 启用模板 | `'确认启用模板 {name}'` | |
| 禁用模板 | `'确认禁用模板 {name}'` | |
| 删除模板 | `confirmDelete(row.TemplateName)` | 使用通用删除确认 |
| 标记已读 | `'确认标记该通知为已读'` | |

### 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建模板 | `notifySuccess('创建成功')` |
| 更新模板 | `notifySuccess('更新成功')` |
| 启用模板 | `notifySuccess('启用成功')` |
| 禁用模板 | `notifySuccess('禁用成功')` |
| 删除模板 | `notifySuccess('删除成功')` |
| 发送通知 | `notifySuccess('发送成功')` |
| 标记已读 | `notifySuccess('标记已读成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 新增模板表单

**标题**：`$t('新增通知模板')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TemplateCode | `$t('模板编码')` | DxTextBox | 是 | 2-50 | `^[a-zA-Z0-9_-]+$` | - | `''` | - | - |
| 2 | TemplateName | `$t('模板名称')` | DxTextBox | 是 | 2-100 | - | - | `''` | - | - |
| 3 | Channel | `$t('通知渠道')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 4 | SubjectTemplate | `$t('主题模板')` | DxTextBox | 否 | 0-200 | - | - | `''` | - | - |
| 5 | BodyTemplate | `$t('正文模板')` | DxTextArea | 是 | 1-5000 | - | - | `''` | - | - |

**通知渠道选项**：

| 值 | 显示文本 |
|:--:|---------|
| email | `$t('邮件')` |
| sms | `$t('短信')` |
| webhook | `$t('Webhook')` |
| in_app | `$t('站内通知')` |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TemplateCode | required | - | `$t('请输入模板编码')` |
| TemplateCode | stringLength | min: 2, max: 50 | `$t('模板编码长度 2-50 个字符')` |
| TemplateCode | pattern | `^[a-zA-Z0-9_-]+$` | `$t('模板编码仅允许字母、数字、下划线和连字符')` |
| TemplateName | required | - | `$t('请输入模板名称')` |
| TemplateName | stringLength | min: 2, max: 100 | `$t('模板名称长度 2-100 个字符')` |
| Channel | required | - | `$t('请选择通知渠道')` |
| BodyTemplate | required | - | `$t('请输入正文模板')` |
| BodyTemplate | stringLength | max: 5000 | `$t('正文模板长度不超过 5000 个字符')` |
| SubjectTemplate | stringLength | max: 200 | `$t('主题模板长度不超过 200 个字符')` |

**提交行为**：

1. 提交前：调用 `formInstance.validate()`，不通过则阻止提交
2. 提交时：`submitting.value = true`，禁用提交按钮并显示 loading
3. 提交成功：关闭弹窗 → `notifySuccess('创建成功')` → 刷新列表
4. 提交失败：axios 拦截器自动显示错误 → 不关闭弹窗 → `submitting.value = false`

### 编辑模板表单

**标题**：`$t('编辑通知模板')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TemplateCode | `$t('模板编码')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 2 | TemplateName | `$t('模板名称')` | DxTextBox | 否 | 2-100 | - | - | 从数据加载 | - | - |
| 3 | Channel | `$t('通知渠道')` | DxSelectBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 4 | SubjectTemplate | `$t('主题模板')` | DxTextBox | 否 | 0-200 | - | - | 从数据加载 | - | - |
| 5 | BodyTemplate | `$t('正文模板')` | DxTextArea | 否 | 1-5000 | - | - | 从数据加载 | - | - |

**注意**：编辑接口 `UpdateNotificationTemplateReqDTO` 仅含 `TemplateName`、`SubjectTemplate`、`BodyTemplate` 字段。`TemplateCode` 和 `Channel` 编辑时为只读。

**提交行为**：同新增模板表单，成功提示为 `notifySuccess('更新成功')`。

### 发送通知表单

**标题**：`$t('发送通知')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantRefId | `$t('关联租户')` | DxSelectBox | 否 | - | - | - | `null` | - | - |
| 2 | TemplateId | `$t('通知模板')` | DxSelectBox | 否 | - | - | - | `null` | - | - |
| 3 | Channel | `$t('通知渠道')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 4 | Recipient | `$t('接收人')` | DxTextBox | 是 | 1-200 | - | - | `''` | - | - |
| 5 | Subject | `$t('主题')` | DxTextBox | 否 | 0-200 | - | - | `''` | - | - |
| 6 | Body | `$t('正文')` | DxTextArea | 是 | 1-5000 | - | - | `''` | - | - |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| Channel | required | - | `$t('请选择通知渠道')` |
| Recipient | required | - | `$t('请输入接收人')` |
| Recipient | stringLength | max: 200 | `$t('接收人长度不超过 200 个字符')` |
| Body | required | - | `$t('请输入正文')` |
| Body | stringLength | max: 5000 | `$t('正文长度不超过 5000 个字符')` |
| Subject | stringLength | max: 200 | `$t('主题长度不超过 200 个字符')` |

**提交行为**：同新增模板表单，成功提示为 `notifySuccess('发送成功')`。

### 模板详情展示

**标题**：`$t('模板详情')`
**组件**：`DxPopup`（只读展示，`width: 700`，`height: auto`）

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | TemplateCode | `$t('模板编码')` | - |
| 3 | TemplateName | `$t('模板名称')` | - |
| 4 | Channel | `$t('通知渠道')` | 渠道标签 |
| 5 | SubjectTemplate | `$t('主题模板')` | - |
| 6 | BodyTemplate | `$t('正文模板')` | 长文本展示（pre-wrap） |
| 7 | Status | `$t('状态')` | 状态标签（颜色） |
| 8 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 通知详情展示

**标题**：`$t('通知详情')`
**组件**：`DxPopup`（只读展示，`width: 700`，`height: auto`）

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | TenantRefId | `$t('租户ID')` | - |
| 3 | TemplateId | `$t('模板ID')` | - |
| 4 | Channel | `$t('通知渠道')` | 渠道标签 |
| 5 | Recipient | `$t('接收人')` | - |
| 6 | Subject | `$t('主题')` | - |
| 7 | Body | `$t('正文')` | 长文本展示（pre-wrap） |
| 8 | SendStatus | `$t('发送状态')` | 状态标签（颜色） |
| 9 | SentAt | `$t('发送时间')` | `yyyy-MM-dd HH:mm:ss` |
| 10 | ReadAt | `$t('阅读时间')` | `yyyy-MM-dd HH:mm:ss` |
| 11 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

**弹窗按钮（所有弹窗通用）**：

| 按钮 | 文本 | 位置 | 行为 |
|------|------|------|------|
| 提交 | `$t('确定')` | 弹窗底部右侧 | 提交表单，`submitting` 时 disabled + loading |
| 取消 | `$t('取消')` | 弹窗底部左侧 | 关闭弹窗，重置表单 |

### 类型定义

```typescript
// src/types/notification.ts

/** 通知模板响应 */
export interface NotificationTemplateRepDTO {
  Id: number
  TemplateCode: string
  TemplateName: string
  Channel: string
  SubjectTemplate?: string
  BodyTemplate: string
  Status: string
  CreatedAt: string
}

/** 创建通知模板请求 */
export interface CreateNotificationTemplateReqDTO {
  TemplateCode: string
  TemplateName: string
  Channel: string
  SubjectTemplate?: string
  BodyTemplate: string
}

/** 更新通知模板请求 */
export interface UpdateNotificationTemplateReqDTO {
  TemplateName?: string
  SubjectTemplate?: string
  BodyTemplate?: string
}

/** 通知响应 */
export interface NotificationRepDTO {
  Id: number
  TenantRefId?: number
  TemplateId?: number
  Channel: string
  Recipient: string
  Subject?: string
  Body: string
  SendStatus: string
  SentAt?: string
  ReadAt?: string
  CreatedAt: string
}

/** 发送通知请求 */
export interface CreateNotificationReqDTO {
  TenantRefId?: number
  TemplateId?: number
  Channel: string
  Recipient: string
  Subject?: string
  Body: string
}
```

---

## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| 无独立静态配置文件 | 渠道选项、状态字典均内联在页面组件中 | 页面组件级语言文件 |

> 本模块渠道和状态选项数量有限，不需要独立配置文件。

---

## 国际化要求

### 组件级 key（放入 `NotificationView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 通知管理 | 通知管理 | Notification Management | 通知管理 | Pengurusan Pemberitahuan | 通知管理 |
| 管理通知模板和通知记录，支持站内通知 | (同key) | Manage notification templates and records, supports in-app notifications | 通知テンプレートと通知記録を管理、アプリ内通知に対応 | Urus templat pemberitahuan dan rekod, menyokong pemberitahuan dalam aplikasi | 管理通知範本和通知記錄，支援站內通知 |
| 通知模板 | 通知模板 | Notification Templates | 通知テンプレート | Templat Pemberitahuan | 通知範本 |
| 通知记录 | 通知记录 | Notifications | 通知記録 | Rekod Pemberitahuan | 通知記錄 |
| 模板编码 | 模板编码 | Template Code | テンプレートコード | Kod Templat | 範本編碼 |
| 模板名称 | 模板名称 | Template Name | テンプレート名 | Nama Templat | 範本名稱 |
| 通知渠道 | 通知渠道 | Channel | 通知チャネル | Saluran | 通知渠道 |
| 主题模板 | 主题模板 | Subject Template | 件名テンプレート | Templat Subjek | 主題範本 |
| 正文模板 | 正文模板 | Body Template | 本文テンプレート | Templat Isi | 正文範本 |
| 邮件 | 邮件 | Email | メール | E-mel | 郵件 |
| 短信 | 短信 | SMS | SMS | SMS | 簡訊 |
| 站内通知 | 站内通知 | In-App | アプリ内通知 | Dalam Aplikasi | 站內通知 |
| 新增通知模板 | 新增通知模板 | Create Notification Template | 通知テンプレート作成 | Cipta Templat Pemberitahuan | 新增通知範本 |
| 编辑通知模板 | 编辑通知模板 | Edit Notification Template | 通知テンプレート編集 | Edit Templat Pemberitahuan | 編輯通知範本 |
| 模板详情 | 模板详情 | Template Details | テンプレート詳細 | Butiran Templat | 範本詳情 |
| 请输入模板编码或名称 | 请输入模板编码或名称 | Enter template code or name | テンプレートコードまたは名前を入力 | Masukkan kod templat atau nama | 請輸入範本編碼或名稱 |
| 请输入模板编码 | 请输入模板编码 | Enter template code | テンプレートコードを入力 | Masukkan kod templat | 請輸入範本編碼 |
| 请输入模板名称 | 请输入模板名称 | Enter template name | テンプレート名を入力 | Masukkan nama templat | 請輸入範本名稱 |
| 模板编码长度 2-50 个字符 | 模板编码长度 2-50 个字符 | Template code must be 2-50 characters | テンプレートコードは2-50文字 | Kod templat mestilah 2-50 aksara | 範本編碼長度 2-50 個字元 |
| 模板编码仅允许字母、数字、下划线和连字符 | 模板编码仅允许字母、数字、下划线和连字符 | Template code can only contain letters, numbers, underscores and hyphens | テンプレートコードは英数字、アンダースコア、ハイフンのみ | Kod templat hanya boleh mengandungi huruf, nombor, garis bawah dan sempang | 範本編碼僅允許字母、數字、底線和連字符 |
| 模板名称长度 2-100 个字符 | 模板名称长度 2-100 个字符 | Template name must be 2-100 characters | テンプレート名は2-100文字 | Nama templat mestilah 2-100 aksara | 範本名稱長度 2-100 個字元 |
| 请选择通知渠道 | 请选择通知渠道 | Select channel | チャネルを選択 | Pilih saluran | 請選擇通知渠道 |
| 请输入正文模板 | 请输入正文模板 | Enter body template | 本文テンプレートを入力 | Masukkan templat isi | 請輸入正文範本 |
| 正文模板长度不超过 5000 个字符 | 正文模板长度不超过 5000 个字符 | Body template must not exceed 5000 characters | 本文テンプレートは5000文字以内 | Templat isi mestilah tidak melebihi 5000 aksara | 正文範本長度不超過 5000 個字元 |
| 主题模板长度不超过 200 个字符 | 主题模板长度不超过 200 个字符 | Subject template must not exceed 200 characters | 件名テンプレートは200文字以内 | Templat subjek mestilah tidak melebihi 200 aksara | 主題範本長度不超過 200 個字元 |
| 确认启用模板 {name} | 确认启用模板 {name} | Confirm enable template {name} | テンプレート {name} を有効にしますか | Sahkan aktifkan templat {name} | 確認啟用範本 {name} |
| 确认禁用模板 {name} | 确认禁用模板 {name} | Confirm disable template {name} | テンプレート {name} を無効にしますか | Sahkan lumpuhkan templat {name} | 確認停用範本 {name} |
| 发送通知 | 发送通知 | Send Notification | 通知送信 | Hantar Pemberitahuan | 發送通知 |
| 通知详情 | 通知详情 | Notification Details | 通知詳細 | Butiran Pemberitahuan | 通知詳情 |
| 接收人 | 接收人 | Recipient | 受信者 | Penerima | 接收人 |
| 主题 | 主题 | Subject | 件名 | Subjek | 主題 |
| 正文 | 正文 | Body | 本文 | Isi | 正文 |
| 发送状态 | 发送状态 | Send Status | 送信ステータス | Status Penghantaran | 發送狀態 |
| 请选择发送状态 | 请选择发送状态 | Select send status | 送信ステータスを選択 | Pilih status penghantaran | 請選擇發送狀態 |
| 发送时间 | 发送时间 | Sent At | 送信日時 | Masa Dihantar | 發送時間 |
| 阅读时间 | 阅读时间 | Read At | 既読日時 | Masa Dibaca | 閱讀時間 |
| 请输入接收人或主题 | 请输入接收人或主题 | Enter recipient or subject | 受信者または件名を入力 | Masukkan penerima atau subjek | 請輸入接收人或主題 |
| 请输入接收人 | 请输入接收人 | Enter recipient | 受信者を入力 | Masukkan penerima | 請輸入接收人 |
| 接收人长度不超过 200 个字符 | 接收人长度不超过 200 个字符 | Recipient must not exceed 200 characters | 受信者は200文字以内 | Penerima mestilah tidak melebihi 200 aksara | 接收人長度不超過 200 個字元 |
| 请输入正文 | 请输入正文 | Enter body | 本文を入力 | Masukkan isi | 請輸入正文 |
| 正文长度不超过 5000 个字符 | 正文长度不超过 5000 个字符 | Body must not exceed 5000 characters | 本文は5000文字以内 | Isi mestilah tidak melebihi 5000 aksara | 正文長度不超過 5000 個字元 |
| 主题长度不超过 200 个字符 | 主题长度不超过 200 个字符 | Subject must not exceed 200 characters | 件名は200文字以内 | Subjek mestilah tidak melebihi 200 aksara | 主題長度不超過 200 個字元 |
| 模板ID | 模板ID | Template ID | テンプレートID | ID Templat | 範本ID |
| 通知模板 | 通知模板 | Notification Template | 通知テンプレート | Templat Pemberitahuan | 通知範本 |
| 待发送 | 待发送 | Pending | 送信待ち | Menunggu | 待發送 |
| 已发送 | 已发送 | Sent | 送信済み | Dihantar | 已發送 |
| 已读 | 已读 | Read | 既読 | Dibaca | 已讀 |
| 发送失败 | 发送失败 | Failed | 送信失敗 | Gagal | 發送失敗 |
| 标记已读 | 标记已读 | Mark as Read | 既読にする | Tanda Dibaca | 標記已讀 |
| 确认标记该通知为已读 | 确认标记该通知为已读 | Confirm mark this notification as read | この通知を既読にしますか | Sahkan tanda pemberitahuan ini dibaca | 確認標記該通知為已讀 |
| 发送成功 | 发送成功 | Sent successfully | 送信成功 | Berjaya dihantar | 發送成功 |
| 标记已读成功 | 标记已读成功 | Marked as read successfully | 既読に設定しました | Berjaya ditanda dibaca | 標記已讀成功 |
| 关联租户 | 关联租户 | Tenant | テナント | Penyewa | 關聯租戶 |
| 租户ID | 租户ID | Tenant ID | テナントID | ID Penyewa | 租戶ID |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`编辑`、`删除`、`查看`、`确定`、`取消`、`操作`、`ID`、`状态`、`创建时间`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`更新成功`、`删除成功`、`启用成功`、`禁用成功`、`已启用`、`已禁用`、`请选择状态`、`关键词`、`启用`、`禁用`、`Webhook`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"通知管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **DxTabPanel** 包含 2 个标签页：通知模板、通知记录
- [ ] **Tab 1 表格列**包含：ID、模板编码、模板名称、通知渠道、状态、创建时间、操作
- [ ] **Tab 2 表格列**包含：ID、租户ID、通知渠道、接收人、主题、发送状态、发送时间、阅读时间、创建时间、操作
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 远程分页通过 CustomStore 实现
- [ ] **Tab 1 工具栏**包含：新增按钮
- [ ] **Tab 1 行操作**包含：查看、编辑、启用、禁用、删除
- [ ] **Tab 2 工具栏**包含：发送通知按钮
- [ ] **Tab 2 行操作**包含：查看、标记已读
- [ ] **新增模板弹窗**包含字段：模板编码、模板名称、通知渠道、主题模板、正文模板
- [ ] **编辑模板弹窗**包含字段：模板编码（只读）、模板名称、通知渠道（只读）、主题模板、正文模板
- [ ] **模板详情弹窗**展示所有模板字段
- [ ] **发送通知弹窗**包含字段：关联租户、通知模板、通知渠道、接收人、主题、正文
- [ ] **通知详情弹窗**展示所有通知字段

### P1 — 业务规则完整性

- [ ] 模板编码 `required` 验证
- [ ] 模板编码 `stringLength`（2-50）验证
- [ ] 模板编码 `pattern`（字母数字下划线连字符）验证
- [ ] 编辑时模板编码和通知渠道字段 disabled（不可修改）
- [ ] 模板名称 `required` 验证
- [ ] 模板名称 `stringLength`（2-100）验证
- [ ] 通知渠道 `required` 验证
- [ ] 正文模板 `required` 验证
- [ ] 接收人 `required` 验证
- [ ] 正文 `required` 验证
- [ ] 启用/禁用按钮根据当前状态动态显隐
- [ ] 标记已读按钮仅在 SendStatus !== 'read' 时显示
- [ ] 每个操作按钮有权限码控制
- [ ] 删除有 `confirmDelete` 确认
- [ ] 危险操作有 `confirmAction` 确认
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' NotificationView.vue | grep -v ':caption'` 结果为 0
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] `grep -rn "notifySuccess(t(" NotificationView.vue` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有状态显示值已国际化（模板状态 + 发送状态）
- [ ] 所有渠道显示值已国际化（4 种渠道）
- [ ] 所有确认框文案已国际化
- [ ] 所有成功提示已国际化
- [ ] 所有标签页标题已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
