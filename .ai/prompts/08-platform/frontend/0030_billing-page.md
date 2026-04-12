# 租户平台 — 计费账单页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块为计费账单管理，包含账单 CRUD、支付/作废操作、账单明细查看、支付单管理和退款管理。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-11 |
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
- `BillingEndpoints.cs` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 账单 / 支付单 / 退款列表远程分页 |
| DxDataGrid | `DxDataGrid master-detail row expand collapse template` | 账单明细展开行 |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 创建账单 / 创建支付单 / 创建退款表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 创建 / 详情 / 明细弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 状态筛选 |
| DxNumberBox | `DxNumberBox min max step format placeholder` | 金额输入 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |
| DxTabPanel | `DxTabPanel items selectedIndex` | 账单 / 支付单 / 退款 Tab 切换 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `BillingEndpoints.cs` 中的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 账单列表 | GET | `/api/billings?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<BillingInvoiceRepDTO>>` |
| 账单详情 | GET | `/api/billings/{id}` | - | `ApiResult<BillingInvoiceRepDTO>` |
| 创建账单 | POST | `/api/billings` | `CreateBillingInvoiceReqDTO` | `ApiResult<long>` |
| 支付账单 | PUT | `/api/billings/{id}/pay` | - | `ApiResult` |
| 作废账单 | PUT | `/api/billings/{id}/void` | - | `ApiResult` |
| 账单明细 | GET | `/api/billings/{invoiceId}/items?page=1&pageSize=20` | - | `ApiResult<PagedResult<BillingInvoiceItemRepDTO>>` |
| 租户账单 | GET | `/api/tenants/{id}/billings?page=1&pageSize=20` | - | `ApiResult<PagedResult<BillingInvoiceRepDTO>>` |
| 支付单列表 | GET | `/api/payment-orders?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<PaymentOrderRepDTO>>` |
| 创建支付单 | POST | `/api/payment-orders` | `CreatePaymentOrderReqDTO` | `ApiResult<long>` |
| 退款列表 | GET | `/api/payment-refunds?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<PaymentRefundRepDTO>>` |
| 创建退款 | POST | `/api/payment-refunds` | `CreateRefundReqDTO` | `ApiResult<long>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/billings/BillingsView.vue` | 主页面 |
| 2 | `src/WebTenantPlatfrom/src/views/billings/BillingsView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/billings/BillingsView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/billings/BillingsView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/billings/BillingsView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/billings/BillingsView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/billings.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/billings.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('计费账单')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理账单、支付单和退款，包括账单生成、支付、作废及退款处理')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| Tab 面板 | `DxTabPanel` | 账单列表 / 支付单列表 / 退款列表 三个 Tab |
| 查询区（账单 Tab） | 自定义查询栏 | 关键词 + 状态筛选 |
| 查询区（支付单 Tab） | 自定义查询栏 | 关键词 + 状态筛选 |
| 查询区（退款 Tab） | 自定义查询栏 | 关键词 + 状态筛选 |
| 工具栏（账单 Tab） | `DxToolbar` | 新增账单 |
| 工具栏（支付单 Tab） | `DxToolbar` | 创建支付单 |
| 工具栏（退款 Tab） | `DxToolbar` | 创建退款 |
| 表格区（账单 Tab） | `DxDataGrid` + `CustomStore` | 账单列表 |
| 表格区（支付单 Tab） | `DxDataGrid` + `CustomStore` | 支付单列表 |
| 表格区（退款 Tab） | `DxDataGrid` + `CustomStore` | 退款列表 |
| 分页 | `DxDataGrid` 内置 `DxPager` + `DxPaging` | 远程分页 |
| 新增账单弹窗 | `DxPopup` + `DxForm` | 创建账单表单 |
| 创建支付单弹窗 | `DxPopup` + `DxForm` | 创建支付单表单 |
| 创建退款弹窗 | `DxPopup` + `DxForm` | 创建退款表单 |
| 详情抽屉 | `DxPopup`（只读展示） | 账单详情 |
| 账单明细弹窗 | `DxPopup` + `DxDataGrid` | 账单明细项列表 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### 账单 Tab — 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入账单号或租户名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**账单状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| pending | `$t('待支付')` |
| paid | `$t('已支付')` |
| voided | `$t('已作废')` |

### 支付单 Tab — 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入支付单号或租户名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**支付单状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| pending | `$t('待处理')` |
| completed | `$t('已完成')` |
| failed | `$t('已失败')` |

### 退款 Tab — 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入退款单号或租户名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**退款状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| pending | `$t('待处理')` |
| approved | `$t('已批准')` |
| rejected | `$t('已驳回')` |
| completed | `$t('已完成')` |

### 查询行为

| 行为 | 要求 |
|------|------|
| 回车搜索 | 在关键词输入框回车触发搜索 |
| 查询按钮 | `$t('查询')`，点击触发搜索 |
| 重置按钮 | `$t('重置')`，清空所有条件并重新加载 |
| 所有文本国际化 | 所有 label、placeholder、按钮文本均使用 `$t()` |

---

## 列表与分页

### 账单列表 — 表格组件

使用 `DxDataGrid` + `CustomStore` 实现远程分页。

### 账单列表 — 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | 固定宽度 |
| 2 | InvoiceNo | `$t('账单号')` | 180px | 是 | - | |
| 3 | TenantName | `$t('租户名称')` | auto | 是 | - | |
| 4 | TotalAmount | `$t('总金额')` | 120px | 否 | `currencyCell` | 货币格式 |
| 5 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 6 | DueDate | `$t('到期日')` | 150px | 是 | `dateCell` | `yyyy-MM-dd` |
| 7 | PaidAt | `$t('支付时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 8 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 9 | - | `$t('操作')` | 400px | 否 | `actionCell` | 操作按钮列 |

**所有 caption 必须使用 `:caption="$t('...')"` 绑定，禁止硬编码。**

### 支付单列表 — 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | OrderNo | `$t('支付单号')` | 180px | 是 | - | |
| 3 | TenantName | `$t('租户名称')` | auto | 是 | - | |
| 4 | Amount | `$t('支付金额')` | 120px | 否 | `currencyCell` | 货币格式 |
| 5 | PaymentMethod | `$t('支付方式')` | 120px | 否 | `paymentMethodCell` | |
| 6 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 7 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |

### 退款列表 — 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | RefundNo | `$t('退款单号')` | 180px | 是 | - | |
| 3 | TenantName | `$t('租户名称')` | auto | 是 | - | |
| 4 | RefundAmount | `$t('退款金额')` | 120px | 否 | `currencyCell` | 货币格式 |
| 5 | Reason | `$t('退款原因')` | auto | 否 | - | |
| 6 | Status | `$t('状态')` | 100px | 否 | `statusCell` | 颜色标签 |
| 7 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |

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

### 账单状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| pending | `$t('待支付')` | 黄色 `#faad14` | `status-pending` |
| paid | `$t('已支付')` | 绿色 `#52c41a` | `status-paid` |
| voided | `$t('已作废')` | 灰色 `#8c8c8c` | `status-voided` |

### 支付单状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| pending | `$t('待处理')` | 黄色 `#faad14` | `status-pay-pending` |
| completed | `$t('已完成')` | 绿色 `#52c41a` | `status-pay-completed` |
| failed | `$t('已失败')` | 红色 `#f5222d` | `status-pay-failed` |

### 退款状态列颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| pending | `$t('待处理')` | 黄色 `#faad14` | `status-refund-pending` |
| approved | `$t('已批准')` | 蓝色 `#1890ff` | `status-refund-approved` |
| rejected | `$t('已驳回')` | 红色 `#f5222d` | `status-refund-rejected` |
| completed | `$t('已完成')` | 绿色 `#52c41a` | `status-refund-completed` |

### 空状态与加载

| 状态 | 显示 |
|------|------|
| 空数据 | `:no-data-text="$t('暂无数据')"` |
| 加载中 | `DxLoadPanel`（`visible` 绑定 `pageLoading`） |

---

## 操作按钮

### 工具栏按钮（账单 Tab）

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增账单 | `$t('新增')` | `add` | `BILLING_CREATE` | 始终启用 | 打开新增账单弹窗 | 无 |

### 工具栏按钮（支付单 Tab）

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 创建支付单 | `$t('创建支付单')` | `add` | `BILLING_CREATE` | 始终启用 | 打开创建支付单弹窗 | 无 |

### 工具栏按钮（退款 Tab）

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 创建退款 | `$t('创建退款')` | `add` | `BILLING_CREATE` | 始终启用 | 打开创建退款弹窗 | 无 |

### 账单行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `BILLING_VIEW` | 始终 | 打开详情抽屉 | 无 |
| 明细 | `$t('账单明细')` | `detailslayout` | `BILLING_VIEW` | 始终 | 打开账单明细弹窗 | 无 |
| 支付 | `$t('支付')` | `money` | `BILLING_UPDATE` | `Status === 'pending'` | 调用支付 API | `confirmAction('确认标记账单 {invoiceNo} 为已支付')` |
| 作废 | `$t('作废')` | `close` | `BILLING_UPDATE` | `Status === 'pending'` | 调用作废 API | `confirmAction('确认作废账单 {invoiceNo}')` |

### 状态流转按钮动态显隐规则

> 此规则是本页面的核心业务逻辑。

| 当前状态 | 允许的操作 | 不允许的操作 |
|---------|-----------|------------|
| pending | 支付、作废 | 无 |
| paid | 无 | 支付、作废 |
| voided | 无 | 支付、作废 |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const BILLING_VIEW = 'billing.detail'
export const BILLING_CREATE = 'billing.create'
export const BILLING_UPDATE = 'billing.update'
```

### 确认框文案（全部国际化）

| 操作 | 文案 key | 说明 |
|------|---------|------|
| 支付 | `'确认标记账单 {invoiceNo} 为已支付'` | 标记支付完成 |
| 作废 | `'确认作废账单 {invoiceNo}'` | 作废后不可恢复 |

### 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建账单 | `notifySuccess('创建成功')` |
| 支付 | `notifySuccess('支付成功')` |
| 作废 | `notifySuccess('作废成功')` |
| 创建支付单 | `notifySuccess('创建成功')` |
| 创建退款 | `notifySuccess('创建成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 新增账单表单

**标题**：`$t('新增账单')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantRefId | `$t('租户')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 2 | TotalAmount | `$t('总金额')` | DxNumberBox | 是 | - | > 0 | - | `0` | - | - |
| 3 | DueDate | `$t('到期日')` | DxDateBox | 是 | - | - | - | 30天后 | - | - |
| 4 | Description | `$t('账单说明')` | DxTextArea | 否 | 0-500 | - | - | `''` | - | - |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TenantRefId | required | - | `$t('请选择租户')` |
| TotalAmount | required | - | `$t('请输入总金额')` |
| TotalAmount | range | min: 0.01 | `$t('总金额必须大于 0')` |
| DueDate | required | - | `$t('请选择到期日')` |
| Description | stringLength | max: 500 | `$t('账单说明长度不超过 500 个字符')` |

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

### 创建支付单表单

**标题**：`$t('创建支付单')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | InvoiceId | `$t('关联账单')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 2 | Amount | `$t('支付金额')` | DxNumberBox | 是 | - | > 0 | - | `0` | - | - |
| 3 | PaymentMethod | `$t('支付方式')` | DxSelectBox | 是 | - | - | - | `null` | - | - |

**支付方式选项**：

| 值 | 显示文本 |
|:--:|---------|
| bank_transfer | `$t('银行转账')` |
| credit_card | `$t('信用卡')` |
| alipay | `$t('支付宝')` |
| wechat_pay | `$t('微信支付')` |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| InvoiceId | required | - | `$t('请选择关联账单')` |
| Amount | required | - | `$t('请输入支付金额')` |
| Amount | range | min: 0.01 | `$t('支付金额必须大于 0')` |
| PaymentMethod | required | - | `$t('请选择支付方式')` |

**提交行为**：同新增账单表单，成功提示为 `notifySuccess('创建成功')`。

### 创建退款表单

**标题**：`$t('创建退款')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | PaymentOrderId | `$t('关联支付单')` | DxSelectBox | 是 | - | - | - | `null` | - | - |
| 2 | RefundAmount | `$t('退款金额')` | DxNumberBox | 是 | - | > 0 | - | `0` | - | - |
| 3 | Reason | `$t('退款原因')` | DxTextArea | 是 | 1-500 | - | - | `''` | - | - |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| PaymentOrderId | required | - | `$t('请选择关联支付单')` |
| RefundAmount | required | - | `$t('请输入退款金额')` |
| RefundAmount | range | min: 0.01 | `$t('退款金额必须大于 0')` |
| Reason | required | - | `$t('请输入退款原因')` |
| Reason | stringLength | min: 1, max: 500 | `$t('退款原因长度 1-500 个字符')` |

**提交行为**：同新增账单表单，成功提示为 `notifySuccess('创建成功')`。

### 详情展示

**标题**：`$t('账单详情')`
**组件**：`DxPopup`（只读展示）

展示字段：

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | InvoiceNo | `$t('账单号')` | - |
| 3 | TenantName | `$t('租户名称')` | - |
| 4 | TotalAmount | `$t('总金额')` | 货币格式 |
| 5 | Status | `$t('状态')` | 状态标签（颜色） |
| 6 | DueDate | `$t('到期日')` | `yyyy-MM-dd` |
| 7 | PaidAt | `$t('支付时间')` | `yyyy-MM-dd HH:mm:ss` |
| 8 | Description | `$t('账单说明')` | - |
| 9 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 账单明细弹窗

**标题**：`$t('账单明细')`
**组件**：`DxPopup`（`width: 800`，`height: 500`）+ `DxDataGrid`

- 调用 `GET /api/billings/{invoiceId}/items?page=1&pageSize=20` 获取明细列表
- 明细列表列定义：

| 序号 | data-field | caption（i18n key） | 宽度 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|--------|------|
| 1 | Id | `$t('ID')` | 80px | - | |
| 2 | ItemName | `$t('项目名称')` | auto | - | |
| 3 | Quantity | `$t('数量')` | 100px | - | |
| 4 | UnitPrice | `$t('单价')` | 120px | `currencyCell` | |
| 5 | Amount | `$t('小计')` | 120px | `currencyCell` | |

### 类型定义

```typescript
// src/types/billings.ts

/** 创建账单请求 */
export interface CreateBillingInvoiceReqDTO {
  TenantRefId: number
  TotalAmount: number
  DueDate: string
  Description?: string
}

/** 账单响应 */
export interface BillingInvoiceRepDTO {
  Id: number
  InvoiceNo: string
  TenantRefId: number
  TenantName: string
  TotalAmount: number
  Status: string
  DueDate: string
  PaidAt?: string
  Description?: string
  CreatedAt: string
}

/** 账单明细项响应 */
export interface BillingInvoiceItemRepDTO {
  Id: number
  InvoiceRefId: number
  ItemName: string
  Quantity: number
  UnitPrice: number
  Amount: number
}

/** 创建支付单请求 */
export interface CreatePaymentOrderReqDTO {
  InvoiceId: number
  Amount: number
  PaymentMethod: string
}

/** 支付单响应 */
export interface PaymentOrderRepDTO {
  Id: number
  OrderNo: string
  InvoiceRefId: number
  TenantName: string
  Amount: number
  PaymentMethod: string
  Status: string
  CreatedAt: string
}

/** 创建退款请求 */
export interface CreateRefundReqDTO {
  PaymentOrderId: number
  RefundAmount: number
  Reason: string
}

/** 退款响应 */
export interface PaymentRefundRepDTO {
  Id: number
  RefundNo: string
  PaymentOrderRefId: number
  TenantName: string
  RefundAmount: number
  Reason: string
  Status: string
  CreatedAt: string
}
```

---

## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| columns.ts | 账单 / 支付单 / 退款表格列定义数组 | 页面组件级语言文件 |
| query-form.ts | 各 Tab 查询表单字段配置 | 页面组件级语言文件 |
| status.ts | 账单状态字典（pending / paid / voided）、支付单状态字典（pending / completed / failed）、退款状态字典（pending / approved / rejected / completed）、支付方式字典 | 页面组件级语言文件 |

---

## 国际化要求

### 组件级 key（放入 `BillingsView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 计费账单 | 计费账单 | Billing & Invoicing | 請求・課金 | Bil & Invois | 計費帳單 |
| 管理账单、支付单和退款，包括账单生成、支付、作废及退款处理 | (同key) | Manage invoices, payment orders and refunds, including invoice generation, payment, voiding and refund processing | 請求書、支払い、返金の管理：請求書の作成、支払い、無効化、返金処理 | Urus invois, pesanan pembayaran dan bayaran balik, termasuk penjanaan invois, pembayaran, pembatalan dan pemprosesan bayaran balik | 管理帳單、支付單和退款，包括帳單生成、支付、作廢及退款處理 |
| 请输入账单号或租户名称 | 请输入账单号或租户名称 | Enter invoice number or tenant name | 請求書番号またはテナント名を入力 | Masukkan nombor invois atau nama penyewa | 請輸入帳單號或租戶名稱 |
| 请输入支付单号或租户名称 | 请输入支付单号或租户名称 | Enter payment order number or tenant name | 支払い番号またはテナント名を入力 | Masukkan nombor pesanan pembayaran atau nama penyewa | 請輸入支付單號或租戶名稱 |
| 请输入退款单号或租户名称 | 请输入退款单号或租户名称 | Enter refund number or tenant name | 返金番号またはテナント名を入力 | Masukkan nombor bayaran balik atau nama penyewa | 請輸入退款單號或租戶名稱 |
| 账单号 | 账单号 | Invoice No. | 請求書番号 | No. Invois | 帳單號 |
| 租户名称 | 租户名称 | Tenant Name | テナント名 | Nama Penyewa | 租戶名稱 |
| 总金额 | 总金额 | Total Amount | 合計金額 | Jumlah Keseluruhan | 總金額 |
| 到期日 | 到期日 | Due Date | 期限日 | Tarikh Tamat | 到期日 |
| 支付时间 | 支付时间 | Paid At | 支払日時 | Masa Pembayaran | 支付時間 |
| 账单说明 | 账单说明 | Invoice Description | 請求書説明 | Penerangan Invois | 帳單說明 |
| 待支付 | 待支付 | Pending Payment | 未払い | Menunggu Pembayaran | 待支付 |
| 已支付 | 已支付 | Paid | 支払い済み | Dibayar | 已支付 |
| 已作废 | 已作废 | Voided | 無効 | Dibatalkan | 已作廢 |
| 支付 | 支付 | Pay | 支払い | Bayar | 支付 |
| 作废 | 作废 | Void | 無効にする | Batalkan | 作廢 |
| 账单明细 | 账单明细 | Invoice Items | 請求書明細 | Butiran Invois | 帳單明細 |
| 项目名称 | 项目名称 | Item Name | 項目名 | Nama Item | 項目名稱 |
| 数量 | 数量 | Quantity | 数量 | Kuantiti | 數量 |
| 单价 | 单价 | Unit Price | 単価 | Harga Seunit | 單價 |
| 小计 | 小计 | Subtotal | 小計 | Jumlah Kecil | 小計 |
| 新增账单 | 新增账单 | Create Invoice | 請求書作成 | Cipta Invois | 新增帳單 |
| 账单详情 | 账单详情 | Invoice Details | 請求書詳細 | Butiran Invois | 帳單詳情 |
| 支付单列表 | 支付单列表 | Payment Orders | 支払い一覧 | Senarai Pesanan Pembayaran | 支付單列表 |
| 支付单号 | 支付单号 | Order No. | 支払い番号 | No. Pesanan | 支付單號 |
| 支付金额 | 支付金额 | Payment Amount | 支払い金額 | Jumlah Pembayaran | 支付金額 |
| 支付方式 | 支付方式 | Payment Method | 支払い方法 | Kaedah Pembayaran | 支付方式 |
| 银行转账 | 银行转账 | Bank Transfer | 銀行振込 | Pindahan Bank | 銀行轉帳 |
| 信用卡 | 信用卡 | Credit Card | クレジットカード | Kad Kredit | 信用卡 |
| 支付宝 | 支付宝 | Alipay | Alipay | Alipay | 支付寶 |
| 微信支付 | 微信支付 | WeChat Pay | WeChat Pay | WeChat Pay | 微信支付 |
| 创建支付单 | 创建支付单 | Create Payment Order | 支払い作成 | Cipta Pesanan Pembayaran | 建立支付單 |
| 关联账单 | 关联账单 | Associated Invoice | 関連請求書 | Invois Berkaitan | 關聯帳單 |
| 退款列表 | 退款列表 | Refunds | 返金一覧 | Senarai Bayaran Balik | 退款列表 |
| 退款单号 | 退款单号 | Refund No. | 返金番号 | No. Bayaran Balik | 退款單號 |
| 退款金额 | 退款金额 | Refund Amount | 返金金額 | Jumlah Bayaran Balik | 退款金額 |
| 退款原因 | 退款原因 | Refund Reason | 返金理由 | Sebab Bayaran Balik | 退款原因 |
| 创建退款 | 创建退款 | Create Refund | 返金作成 | Cipta Bayaran Balik | 建立退款 |
| 关联支付单 | 关联支付单 | Associated Payment Order | 関連支払い | Pesanan Pembayaran Berkaitan | 關聯支付單 |
| 待处理 | 待处理 | Pending | 処理待ち | Menunggu | 待處理 |
| 已完成 | 已完成 | Completed | 完了 | Selesai | 已完成 |
| 已失败 | 已失败 | Failed | 失敗 | Gagal | 已失敗 |
| 已批准 | 已批准 | Approved | 承認済み | Diluluskan | 已批准 |
| 已驳回 | 已驳回 | Rejected | 却下 | Ditolak | 已駁回 |
| 请选择租户 | 请选择租户 | Select tenant | テナントを選択 | Pilih penyewa | 請選擇租戶 |
| 请输入总金额 | 请输入总金额 | Enter total amount | 合計金額を入力 | Masukkan jumlah keseluruhan | 請輸入總金額 |
| 总金额必须大于 0 | 总金额必须大于 0 | Total amount must be greater than 0 | 合計金額は0より大きくする必要があります | Jumlah keseluruhan mestilah lebih daripada 0 | 總金額必須大於 0 |
| 请选择到期日 | 请选择到期日 | Select due date | 期限日を選択 | Pilih tarikh tamat | 請選擇到期日 |
| 账单说明长度不超过 500 个字符 | 账单说明长度不超过 500 个字符 | Invoice description must not exceed 500 characters | 請求書説明は500文字以内 | Penerangan invois mestilah tidak melebihi 500 aksara | 帳單說明長度不超過 500 個字元 |
| 请选择关联账单 | 请选择关联账单 | Select associated invoice | 関連請求書を選択 | Pilih invois berkaitan | 請選擇關聯帳單 |
| 请输入支付金额 | 请输入支付金额 | Enter payment amount | 支払い金額を入力 | Masukkan jumlah pembayaran | 請輸入支付金額 |
| 支付金额必须大于 0 | 支付金额必须大于 0 | Payment amount must be greater than 0 | 支払い金額は0より大きくする必要があります | Jumlah pembayaran mestilah lebih daripada 0 | 支付金額必須大於 0 |
| 请选择支付方式 | 请选择支付方式 | Select payment method | 支払い方法を選択 | Pilih kaedah pembayaran | 請選擇支付方式 |
| 请选择关联支付单 | 请选择关联支付单 | Select associated payment order | 関連支払いを選択 | Pilih pesanan pembayaran berkaitan | 請選擇關聯支付單 |
| 请输入退款金额 | 请输入退款金额 | Enter refund amount | 返金金額を入力 | Masukkan jumlah bayaran balik | 請輸入退款金額 |
| 退款金额必须大于 0 | 退款金额必须大于 0 | Refund amount must be greater than 0 | 返金金額は0より大きくする必要があります | Jumlah bayaran balik mestilah lebih daripada 0 | 退款金額必須大於 0 |
| 请输入退款原因 | 请输入退款原因 | Enter refund reason | 返金理由を入力 | Masukkan sebab bayaran balik | 請輸入退款原因 |
| 退款原因长度 1-500 个字符 | 退款原因长度 1-500 个字符 | Refund reason must be 1-500 characters | 返金理由は1-500文字 | Sebab bayaran balik mestilah 1-500 aksara | 退款原因長度 1-500 個字元 |
| 确认标记账单 {invoiceNo} 为已支付 | 确认标记账单 {invoiceNo} 为已支付 | Confirm mark invoice {invoiceNo} as paid | 請求書 {invoiceNo} を支払い済みにしますか | Sahkan tandakan invois {invoiceNo} sebagai dibayar | 確認標記帳單 {invoiceNo} 為已支付 |
| 确认作废账单 {invoiceNo} | 确认作废账单 {invoiceNo} | Confirm void invoice {invoiceNo} | 請求書 {invoiceNo} を無効にしますか | Sahkan batalkan invois {invoiceNo} | 確認作廢帳單 {invoiceNo} |
| 支付成功 | 支付成功 | Payment successful | 支払い成功 | Pembayaran berjaya | 支付成功 |
| 作废成功 | 作废成功 | Voided successfully | 無効化成功 | Pembatalan berjaya | 作廢成功 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`删除`、`查看`、`确定`、`取消`、`操作`、`ID`、`状态`、`创建时间`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`请选择状态`、`关键词`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"计费账单"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **Tab 面板**包含：账单列表、支付单列表、退款列表
- [ ] **账单查询区**包含：关键词输入框、状态下拉（3 个状态选项 + 全部）
- [ ] **支付单查询区**包含：关键词输入框、状态下拉（3 个状态选项 + 全部）
- [ ] **退款查询区**包含：关键词输入框、状态下拉（4 个状态选项 + 全部）
- [ ] **账单表格列**包含：ID、账单号、租户名称、总金额、状态、到期日、支付时间、创建时间、操作
- [ ] **支付单表格列**包含：ID、支付单号、租户名称、支付金额、支付方式、状态、创建时间
- [ ] **退款表格列**包含：ID、退款单号、租户名称、退款金额、退款原因、状态、创建时间
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 所有列表远程分页通过 CustomStore 实现
- [ ] **工具栏（账单 Tab）**包含：新增账单按钮
- [ ] **工具栏（支付单 Tab）**包含：创建支付单按钮
- [ ] **工具栏（退款 Tab）**包含：创建退款按钮
- [ ] **账单行操作**包含：查看、账单明细、支付、作废
- [ ] **新增账单弹窗**包含字段：租户、总金额、到期日、账单说明
- [ ] **创建支付单弹窗**包含字段：关联账单、支付金额、支付方式
- [ ] **创建退款弹窗**包含字段：关联支付单、退款金额、退款原因
- [ ] **详情抽屉**展示：ID、账单号、租户名称、总金额、状态、到期日、支付时间、账单说明、创建时间
- [ ] **账单明细弹窗**展示明细列表（ID、项目名称、数量、单价、小计）

### P1 — 业务规则完整性

- [ ] 支付 / 作废按钮仅在 `Status === 'pending'` 时显示
- [ ] 已支付和已作废的账单无支付 / 作废按钮
- [ ] 支付有 `confirmAction` 确认
- [ ] 作废有 `confirmAction` 确认
- [ ] 租户 `required` 验证（新增账单）
- [ ] 总金额 `required` + > 0 验证
- [ ] 到期日 `required` 验证
- [ ] 关联账单 `required` 验证（创建支付单）
- [ ] 支付金额 `required` + > 0 验证
- [ ] 支付方式 `required` 验证
- [ ] 关联支付单 `required` 验证（创建退款）
- [ ] 退款金额 `required` + > 0 验证
- [ ] 退款原因 `required` + 长度 1-500 验证
- [ ] 每个操作按钮有权限码控制
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' BillingsView.vue | grep -v ':caption'` 结果为 0
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] `grep -rn "notifySuccess(t(" BillingsView.vue` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有状态显示值已国际化（3 种账单状态 + 3 种支付单状态 + 4 种退款状态 + 4 种支付方式）
- [ ] 所有确认框文案已国际化
- [ ] 所有成功提示已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
