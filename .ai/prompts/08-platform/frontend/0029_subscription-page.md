# 租户平台 — 订阅管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块为租户订阅生命周期管理，包含订阅列表、续费、升级、取消，以及试用管理和订阅变更记录查看。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-10 |
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
- `SubscriptionEndpoints.cs` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 订阅列表 / 试用列表 / 变更记录列表远程分页 |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 创建订阅 / 续费 / 升级 / 创建试用表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 创建 / 续费 / 升级 / 详情 / 变更记录弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 状态筛选、套餐选择 |
| DxNumberBox | `DxNumberBox min max step format placeholder` | 续费时长输入 |
| DxDateBox | `DxDateBox type displayFormat min max` | 到期日期展示 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |
| DxTabPanel | `DxTabPanel items selectedIndex` | 订阅 / 试用 / 变更记录 Tab 切换 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `SubscriptionEndpoints.cs` 中的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 订阅列表 | GET | `/api/subscriptions?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<TenantSubscriptionRepDTO>>` |
| 订阅详情 | GET | `/api/subscriptions/{id}` | - | `ApiResult<TenantSubscriptionRepDTO>` |
| 创建订阅 | POST | `/api/subscriptions` | `CreateSubscriptionReqDTO` | `ApiResult<long>` |
| 续费订阅 | PUT | `/api/subscriptions/{id}/renew` | `RenewSubscriptionReqDTO` | `ApiResult` |
| 升级订阅 | PUT | `/api/subscriptions/{id}/upgrade` | `UpgradeSubscriptionReqDTO` | `ApiResult` |
| 取消订阅 | PUT | `/api/subscriptions/{id}/cancel` | - | `ApiResult` |
| 租户当前订阅 | GET | `/api/tenants/{id}/subscription` | - | `ApiResult<TenantSubscriptionRepDTO>` |
| 试用列表 | GET | `/api/tenant-trials?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<TenantTrialRepDTO>>` |
| 创建试用 | POST | `/api/tenant-trials` | `CreateTrialReqDTO` | `ApiResult<long>` |
| 订阅变更记录 | GET | `/api/subscription-changes?page=1&pageSize=20&keyword=&tenantRefId=` | - | `ApiResult<PagedResult<TenantSubscriptionChangeRepDTO>>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue` | 主页面 |
| 2 | `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/subscriptions/SubscriptionsView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/subscriptions.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/subscriptions.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('订阅管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理租户订阅生命周期，包括创建、续费、升级、取消，以及试用管理和变更记录')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| Tab 面板 | `DxTabPanel` | 订阅列表 / 试用列表 / 订阅变更记录 三个 Tab |
| 查询区（订阅 Tab） | 自定义查询栏 | 关键词 + 状态筛选 |
| 查询区（试用 Tab） | 自定义查询栏 | 关键词 + 状态筛选 |
| 查询区（变更 Tab） | 自定义查询栏 | 关键词 + 租户筛选 |
| 工具栏（订阅 Tab） | `DxToolbar` | 新增订阅 |
| 工具栏（试用 Tab） | `DxToolbar` | 创建试用 |
| 表格区（订阅 Tab） | `DxDataGrid` + `CustomStore` | 订阅列表 |
| 表格区（试用 Tab） | `DxDataGrid` + `CustomStore` | 试用列表 |
| 表格区（变更 Tab） | `DxDataGrid` + `CustomStore` | 变更记录列表 |
| 分页 | `DxDataGrid` 内置 `DxPager` + `DxPaging` | 远程分页 |
| 新增订阅弹窗 | `DxPopup` + `DxForm` | 创建订阅表单 |
| 续费弹窗 | `DxPopup` + `DxForm` | 续费订阅表单 |
| 升级弹窗 | `DxPopup` + `DxForm` | 升级订阅表单 |
| 创建试用弹窗 | `DxPopup` + `DxForm` | 创建试用表单 |
| 详情抽屉 | `DxPopup`（只读展示） | 订阅详情 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### 订阅 Tab — 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入租户名称或套餐名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**订阅状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| active | `$t('生效中')` |
| expired | `$t('已过期')` |
| cancelled | `$t('已取消')` |

### 试用 Tab — 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入租户名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**试用状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| active | `$t('试用中')` |
| expired | `$t('已过期')` |
| converted | `$t('已转正')` |

### 变更记录 Tab — 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入关键词')` |
| 2 | TenantRefId | `$t('租户')` | DxTextBox | `''` | `$t('请输入租户 ID')` |

### 查询行为

| 行为 | 要求 |
|------|------|
| 回车搜索 | 在关键词输入框回车触发搜索 |
| 查询按钮 | `$t('查询')`，点击触发搜索 |
| 重置按钮 | `$t('重置')`，清空所有条件并重新加载 |
| 所有文本国际化 | 所有 label、placeholder、按钮文本均使用 `$t()` |

---

## 列表与分页

### 订阅列表 — 表格组件

使用 `DxDataGrid` + `CustomStore` 实现远程分页。

### 订阅列表 — 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | 固定宽度 |
| 2 | TenantName | `$t('租户名称')` | auto | 是 | - | |
| 3 | PackageName | `$t('套餐名称')` | 150px | 是 | - | |
| 4 | BillingCycle | `$t('计费周期')` | 100px | 否 | `billingCycleCell` | 月付/年付 |
| 5 | StartDate | `$t('开始日期')` | 150px | 是 | `dateCell` | `yyyy-MM-dd` |
| 6 | EndDate | `$t('结束日期')` | 150px | 是 | `dateCell` | `yyyy-MM-dd`，到期高亮 |
| 7 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 8 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 9 | - | `$t('操作')` | 400px | 否 | `actionCell` | 操作按钮列 |

**所有 caption 必须使用 `:caption="$t('...')"` 绑定，禁止硬编码。**

### 试用列表 — 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | TenantName | `$t('租户名称')` | auto | 是 | - | |
| 3 | TrialDays | `$t('试用天数')` | 100px | 否 | - | |
| 4 | StartDate | `$t('开始日期')` | 150px | 是 | `dateCell` | `yyyy-MM-dd` |
| 5 | EndDate | `$t('结束日期')` | 150px | 是 | `dateCell` | `yyyy-MM-dd` |
| 6 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 7 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |

### 订阅变更记录 — 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | TenantName | `$t('租户名称')` | auto | 否 | - | |
| 3 | ChangeType | `$t('变更类型')` | 120px | 否 | `changeTypeCell` | 续费/升级/取消 |
| 4 | FromPackage | `$t('原套餐')` | 150px | 否 | - | |
| 5 | ToPackage | `$t('目标套餐')` | 150px | 否 | - | |
| 6 | ChangedAt | `$t('变更时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |

### 分页配置（所有列表通用）

| 配置 | 值 |
|------|---|
| 支持分页 | 是 |
| 默认页大小 | 20 |
| 可选页大小 | `[10, 20, 50, 100]` |
| 显示总数 | 是（`showInfo: true`） |
| 显示页大小选择 | 是（`showPageSizeSelector: true`） |
| 显示导航按钮 | 是（`showNavigationButtons: true`） |
| 远程分页 | 是（CustomStore） |

### 订阅状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| active | `$t('生效中')` | 绿色 `#52c41a` | `status-active` |
| expired | `$t('已过期')` | 红色 `#f5222d` | `status-expired` |
| cancelled | `$t('已取消')` | 灰色 `#8c8c8c` | `status-cancelled` |

### 试用状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| active | `$t('试用中')` | 蓝色 `#1890ff` | `status-trial-active` |
| expired | `$t('已过期')` | 红色 `#f5222d` | `status-trial-expired` |
| converted | `$t('已转正')` | 绿色 `#52c41a` | `status-trial-converted` |

### 到期预警高亮

> 此规则是本页面的核心业务逻辑之一。

| 条件 | 样式 | 说明 |
|------|------|------|
| 结束日期 ≤ 今日 | 红色背景（`#fff1f0`） | 已过期 |
| 结束日期 ≤ 今日 + 7 天 | 黄色背景（`#fffbe6`） | 即将到期 |
| 其他 | 默认 | 正常 |

### 空状态与加载

| 状态 | 显示 |
|------|------|
| 空数据 | `:no-data-text="$t('暂无数据')"` |
| 加载中 | `DxLoadPanel`（`visible` 绑定 `pageLoading`） |

---

## 操作按钮

### 工具栏按钮（订阅 Tab）

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增订阅 | `$t('新增')` | `add` | `SUBSCRIPTION_CREATE` | 始终启用 | 打开新增订阅弹窗 | 无 |

### 工具栏按钮（试用 Tab）

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 创建试用 | `$t('创建试用')` | `add` | `SUBSCRIPTION_CREATE` | 始终启用 | 打开创建试用弹窗 | 无 |

### 订阅行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `SUBSCRIPTION_VIEW` | 始终 | 打开详情抽屉 | 无 |
| 续费 | `$t('续费')` | `refresh` | `SUBSCRIPTION_UPDATE` | `Status === 'active'` | 打开续费弹窗 | 无 |
| 升级 | `$t('升级')` | `arrowup` | `SUBSCRIPTION_UPDATE` | `Status === 'active'` | 打开升级弹窗 | 无 |
| 取消 | `$t('取消订阅')` | `close` | `SUBSCRIPTION_UPDATE` | `Status === 'active'` | 调用取消 API | `confirmAction('确认取消订阅 {name}，取消后不可退款')` |
| 变更记录 | `$t('变更记录')` | `event` | `SUBSCRIPTION_VIEW` | 始终 | 切换至变更记录 Tab 并筛选当前租户 | 无 |

### 状态流转按钮动态显隐规则

> 此规则是本页面的核心业务逻辑，一个租户仅有一个活跃订阅。

| 当前状态 | 允许的操作 | 不允许的操作 |
|---------|-----------|------------|
| active | 续费、升级、取消 | 无 |
| expired | 无（到期自动暂停租户） | 续费、升级、取消 |
| cancelled | 无 | 续费、升级、取消 |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const SUBSCRIPTION_VIEW = 'subscription.detail'
export const SUBSCRIPTION_CREATE = 'subscription.create'
export const SUBSCRIPTION_UPDATE = 'subscription.update'
```

### 确认框文案（全部国际化）

| 操作 | 文案 key | 说明 |
|------|---------|------|
| 取消订阅 | `'确认取消订阅 {name}，取消后不可退款'` | 需特别提示不可退款 |

### 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建订阅 | `notifySuccess('创建成功')` |
| 续费 | `notifySuccess('续费成功')` |
| 升级 | `notifySuccess('升级成功')` |
| 取消订阅 | `notifySuccess('取消成功')` |
| 创建试用 | `notifySuccess('创建成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 新增订阅表单

**标题**：`$t('新增订阅')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantRefId | `$t('租户')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 2 | PackageId | `$t('套餐')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 3 | BillingCycle | `$t('计费周期')` | DxSelectBox | 是 | - | - | - | `'monthly'` | - | - |
| 4 | StartDate | `$t('开始日期')` | DxDateBox | 是 | - | - | - | 今日 | - | - |

**计费周期选项**：

| 值 | 显示文本 |
|:--:|---------|
| monthly | `$t('月付')` |
| yearly | `$t('年付')` |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TenantRefId | required | - | `$t('请选择租户')` |
| PackageId | required | - | `$t('请选择套餐')` |
| BillingCycle | required | - | `$t('请选择计费周期')` |
| StartDate | required | - | `$t('请选择开始日期')` |

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

### 续费表单

**标题**：`$t('续费订阅')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantName | `$t('租户名称')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 2 | PackageName | `$t('当前套餐')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 3 | CurrentEndDate | `$t('当前到期日')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 4 | RenewMonths | `$t('续费月数')` | DxNumberBox | 是 | - | ≥ 1 整数 | - | `1` | - | - |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| RenewMonths | required | - | `$t('请输入续费月数')` |
| RenewMonths | range | min: 1 | `$t('续费月数至少为 1')` |

**提交行为**：同新增表单，成功提示为 `notifySuccess('续费成功')`。

### 升级表单

**标题**：`$t('升级订阅')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantName | `$t('租户名称')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 2 | CurrentPackage | `$t('当前套餐')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 3 | TargetPackageId | `$t('目标套餐')` | DxSelectBox | 是 | - | - | - | `null` | - | - |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TargetPackageId | required | - | `$t('请选择目标套餐')` |

**提交行为**：同新增表单，成功提示为 `notifySuccess('升级成功')`。

### 创建试用表单

**标题**：`$t('创建试用')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantRefId | `$t('租户')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 2 | TrialDays | `$t('试用天数')` | DxNumberBox | 是 | - | ≥ 1 整数 | - | `14` | - | - |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TenantRefId | required | - | `$t('请选择租户')` |
| TrialDays | required | - | `$t('请输入试用天数')` |
| TrialDays | range | min: 1 | `$t('试用天数至少为 1')` |

**提交行为**：同新增表单，成功提示为 `notifySuccess('创建成功')`。

### 详情展示

**标题**：`$t('订阅详情')`
**组件**：`DxPopup`（只读展示）

展示字段：

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | TenantName | `$t('租户名称')` | - |
| 3 | PackageName | `$t('套餐名称')` | - |
| 4 | BillingCycle | `$t('计费周期')` | 月付/年付 |
| 5 | StartDate | `$t('开始日期')` | `yyyy-MM-dd` |
| 6 | EndDate | `$t('结束日期')` | `yyyy-MM-dd`，到期高亮 |
| 7 | Status | `$t('状态')` | 状态标签（颜色） |
| 8 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 类型定义

```typescript
// src/types/subscriptions.ts

/** 创建订阅请求 */
export interface CreateSubscriptionReqDTO {
  TenantRefId: number
  PackageId: number
  BillingCycle: string
  StartDate: string
}

/** 续费请求 */
export interface RenewSubscriptionReqDTO {
  RenewMonths: number
}

/** 升级请求 */
export interface UpgradeSubscriptionReqDTO {
  TargetPackageId: number
}

/** 创建试用请求 */
export interface CreateTrialReqDTO {
  TenantRefId: number
  TrialDays: number
}

/** 订阅响应 */
export interface TenantSubscriptionRepDTO {
  Id: number
  TenantRefId: number
  TenantName: string
  PackageId: number
  PackageName: string
  BillingCycle: string
  StartDate: string
  EndDate: string
  Status: string
  CreatedAt: string
}

/** 试用响应 */
export interface TenantTrialRepDTO {
  Id: number
  TenantRefId: number
  TenantName: string
  TrialDays: number
  StartDate: string
  EndDate: string
  Status: string
  CreatedAt: string
}

/** 订阅变更记录响应 */
export interface TenantSubscriptionChangeRepDTO {
  Id: number
  TenantRefId: number
  TenantName: string
  ChangeType: string
  FromPackage?: string
  ToPackage?: string
  ChangedAt: string
}
```

---

## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| columns.ts | 订阅 / 试用 / 变更记录表格列定义数组 | 页面组件级语言文件 |
| query-form.ts | 各 Tab 查询表单字段配置 | 页面组件级语言文件 |
| status.ts | 订阅状态字典（active / expired / cancelled）、试用状态字典（active / expired / converted）、变更类型字典 | 页面组件级语言文件 |

---

## 国际化要求

### 组件级 key（放入 `SubscriptionsView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 订阅管理 | 订阅管理 | Subscription Management | サブスクリプション管理 | Pengurusan Langganan | 訂閱管理 |
| 管理租户订阅生命周期，包括创建、续费、升级、取消，以及试用管理和变更记录 | (同key) | Manage tenant subscription lifecycle including create, renew, upgrade, cancel, as well as trial management and change records | テナントサブスクリプションのライフサイクル管理：作成、更新、アップグレード、キャンセル、トライアル管理、変更履歴 | Urus kitaran hayat langganan penyewa termasuk cipta, perbaharui, naik taraf, batal, pengurusan percubaan dan rekod perubahan | 管理租戶訂閱生命週期，包括建立、續費、升級、取消，以及試用管理和變更記錄 |
| 请输入租户名称或套餐名称 | 请输入租户名称或套餐名称 | Enter tenant name or package name | テナント名またはパッケージ名を入力 | Masukkan nama penyewa atau nama pakej | 請輸入租戶名稱或套餐名稱 |
| 请输入租户名称 | 请输入租户名称 | Enter tenant name | テナント名を入力 | Masukkan nama penyewa | 請輸入租戶名稱 |
| 请输入关键词 | 请输入关键词 | Enter keyword | キーワードを入力 | Masukkan kata kunci | 請輸入關鍵詞 |
| 请输入租户 ID | 请输入租户 ID | Enter tenant ID | テナントIDを入力 | Masukkan ID penyewa | 請輸入租戶 ID |
| 租户名称 | 租户名称 | Tenant Name | テナント名 | Nama Penyewa | 租戶名稱 |
| 套餐名称 | 套餐名称 | Package Name | パッケージ名 | Nama Pakej | 套餐名稱 |
| 计费周期 | 计费周期 | Billing Cycle | 課金サイクル | Kitaran Bil | 計費週期 |
| 月付 | 月付 | Monthly | 月払い | Bulanan | 月付 |
| 年付 | 年付 | Yearly | 年払い | Tahunan | 年付 |
| 开始日期 | 开始日期 | Start Date | 開始日 | Tarikh Mula | 開始日期 |
| 结束日期 | 结束日期 | End Date | 終了日 | Tarikh Tamat | 結束日期 |
| 生效中 | 生效中 | Active | 有効 | Aktif | 生效中 |
| 已过期 | 已过期 | Expired | 期限切れ | Tamat Tempoh | 已過期 |
| 已取消 | 已取消 | Cancelled | キャンセル済み | Dibatalkan | 已取消 |
| 试用中 | 试用中 | Trial | トライアル中 | Percubaan | 試用中 |
| 已转正 | 已转正 | Converted | 正式化済み | Ditukarkan | 已轉正 |
| 试用天数 | 试用天数 | Trial Days | トライアル日数 | Hari Percubaan | 試用天數 |
| 试用列表 | 试用列表 | Trial List | トライアル一覧 | Senarai Percubaan | 試用列表 |
| 订阅变更记录 | 订阅变更记录 | Subscription Changes | サブスクリプション変更履歴 | Rekod Perubahan Langganan | 訂閱變更記錄 |
| 变更类型 | 变更类型 | Change Type | 変更タイプ | Jenis Perubahan | 變更類型 |
| 原套餐 | 原套餐 | Previous Package | 元パッケージ | Pakej Asal | 原套餐 |
| 目标套餐 | 目标套餐 | Target Package | 目標パッケージ | Pakej Sasaran | 目標套餐 |
| 变更时间 | 变更时间 | Changed At | 変更日時 | Masa Perubahan | 變更時間 |
| 新增订阅 | 新增订阅 | Create Subscription | サブスクリプション作成 | Cipta Langganan | 新增訂閱 |
| 续费订阅 | 续费订阅 | Renew Subscription | サブスクリプション更新 | Perbaharui Langganan | 續費訂閱 |
| 升级订阅 | 升级订阅 | Upgrade Subscription | サブスクリプションアップグレード | Naik Taraf Langganan | 升級訂閱 |
| 续费 | 续费 | Renew | 更新 | Perbaharui | 續費 |
| 升级 | 升级 | Upgrade | アップグレード | Naik Taraf | 升級 |
| 取消订阅 | 取消订阅 | Cancel Subscription | サブスクリプションキャンセル | Batal Langganan | 取消訂閱 |
| 创建试用 | 创建试用 | Create Trial | トライアル作成 | Cipta Percubaan | 建立試用 |
| 订阅详情 | 订阅详情 | Subscription Details | サブスクリプション詳細 | Butiran Langganan | 訂閱詳情 |
| 当前套餐 | 当前套餐 | Current Package | 現在のパッケージ | Pakej Semasa | 當前套餐 |
| 当前到期日 | 当前到期日 | Current End Date | 現在の終了日 | Tarikh Tamat Semasa | 當前到期日 |
| 续费月数 | 续费月数 | Renew Months | 更新月数 | Bulan Pembaharuan | 續費月數 |
| 请选择租户 | 请选择租户 | Select tenant | テナントを選択 | Pilih penyewa | 請選擇租戶 |
| 请选择套餐 | 请选择套餐 | Select package | パッケージを選択 | Pilih pakej | 請選擇套餐 |
| 请选择计费周期 | 请选择计费周期 | Select billing cycle | 課金サイクルを選択 | Pilih kitaran bil | 請選擇計費週期 |
| 请选择开始日期 | 请选择开始日期 | Select start date | 開始日を選択 | Pilih tarikh mula | 請選擇開始日期 |
| 请输入续费月数 | 请输入续费月数 | Enter renew months | 更新月数を入力 | Masukkan bulan pembaharuan | 請輸入續費月數 |
| 续费月数至少为 1 | 续费月数至少为 1 | Renew months must be at least 1 | 更新月数は1以上 | Bulan pembaharuan mestilah sekurang-kurangnya 1 | 續費月數至少為 1 |
| 请选择目标套餐 | 请选择目标套餐 | Select target package | 目標パッケージを選択 | Pilih pakej sasaran | 請選擇目標套餐 |
| 请输入试用天数 | 请输入试用天数 | Enter trial days | トライアル日数を入力 | Masukkan hari percubaan | 請輸入試用天數 |
| 试用天数至少为 1 | 试用天数至少为 1 | Trial days must be at least 1 | トライアル日数は1以上 | Hari percubaan mestilah sekurang-kurangnya 1 | 試用天數至少為 1 |
| 确认取消订阅 {name}，取消后不可退款 | 确认取消订阅 {name}，取消后不可退款 | Confirm cancel subscription for {name}, cancellation is non-refundable | {name} のサブスクリプションをキャンセルしますか（返金不可） | Sahkan batalkan langganan {name}, pembatalan tidak boleh dikembalikan | 確認取消訂閱 {name}，取消後不可退款 |
| 续费成功 | 续费成功 | Renewal successful | 更新成功 | Pembaharuan berjaya | 續費成功 |
| 升级成功 | 升级成功 | Upgrade successful | アップグレード成功 | Naik taraf berjaya | 升級成功 |
| 取消成功 | 取消成功 | Cancellation successful | キャンセル成功 | Pembatalan berjaya | 取消成功 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`删除`、`查看`、`确定`、`取消`、`操作`、`ID`、`状态`、`创建时间`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`请选择状态`、`关键词`、`租户`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"订阅管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **Tab 面板**包含：订阅列表、试用列表、订阅变更记录
- [ ] **订阅查询区**包含：关键词输入框、状态下拉（3 个状态选项 + 全部）
- [ ] **试用查询区**包含：关键词输入框、状态下拉（3 个状态选项 + 全部）
- [ ] **变更记录查询区**包含：关键词输入框、租户 ID 输入
- [ ] **订阅表格列**包含：ID、租户名称、套餐名称、计费周期、开始日期、结束日期、状态、创建时间、操作
- [ ] **试用表格列**包含：ID、租户名称、试用天数、开始日期、结束日期、状态、创建时间
- [ ] **变更记录表格列**包含：ID、租户名称、变更类型、原套餐、目标套餐、变更时间
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 所有列表远程分页通过 CustomStore 实现
- [ ] **工具栏（订阅 Tab）**包含：新增订阅按钮
- [ ] **工具栏（试用 Tab）**包含：创建试用按钮
- [ ] **订阅行操作**包含：查看、续费、升级、取消订阅、变更记录
- [ ] **新增订阅弹窗**包含字段：租户、套餐、计费周期、开始日期
- [ ] **续费弹窗**包含字段：租户名称（只读）、当前套餐（只读）、当前到期日（只读）、续费月数
- [ ] **升级弹窗**包含字段：租户名称（只读）、当前套餐（只读）、目标套餐
- [ ] **创建试用弹窗**包含字段：租户、试用天数
- [ ] **详情抽屉**展示：ID、租户名称、套餐名称、计费周期、开始日期、结束日期、状态、创建时间
- [ ] **到期预警高亮**：已过期红色、即将到期（7 天内）黄色

### P1 — 业务规则完整性

- [ ] 一个租户仅有一个活跃订阅
- [ ] 续费 / 升级 / 取消按钮仅在 `Status === 'active'` 时显示
- [ ] 已过期和已取消的订阅无操作按钮（除查看和变更记录外）
- [ ] 取消订阅确认框包含"不可退款"提示
- [ ] 升级立即生效
- [ ] 租户 `required` 验证（新增订阅、创建试用）
- [ ] 套餐 `required` 验证（新增订阅）
- [ ] 计费周期 `required` 验证
- [ ] 开始日期 `required` 验证
- [ ] 续费月数 `required` + ≥ 1 验证
- [ ] 目标套餐 `required` 验证
- [ ] 试用天数 `required` + ≥ 1 验证
- [ ] 每个操作按钮有权限码控制
- [ ] 取消订阅有 `confirmAction` 确认
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' SubscriptionsView.vue | grep -v ':caption'` 结果为 0
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] `grep -rn "notifySuccess(t(" SubscriptionsView.vue` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有状态显示值已国际化（3 种订阅状态 + 3 种试用状态）
- [ ] 所有确认框文案已国际化
- [ ] 所有成功提示已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
