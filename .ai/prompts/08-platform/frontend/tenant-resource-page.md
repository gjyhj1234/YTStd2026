# 租户平台 — 租户资源配额页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块管理租户的资源配额设置和资源使用量监控，包含配额列表、配额编辑和使用量进度条展示。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-7 |
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
- `.ai/prompts/08-platform/backend/tenant-resource-api.md` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 配额列表远程分页 |
| DxForm | `DxForm validation rules required range custom` | 配额编辑表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 编辑配额弹窗 |
| DxProgressBar | `DxProgressBar min max value status-format show-status` | 使用量进度条 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 租户选择、配额类型选择 |
| DxNumberBox | `DxNumberBox min max value format` | 配额限制数值输入 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `TenantResourceEndpoints.cs` 中的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 配额列表 | GET | `/api/tenants/{tenantRefId}/resource-quotas?page=1&pageSize=20&keyword=` | - | `ApiResult<PagedResult<TenantResourceQuotaRepDTO>>` |
| 创建/更新配额 | PUT | `/api/tenants/{tenantRefId}/resource-quotas` | `SaveTenantResourceQuotaReqDTO` | `ApiResult` |
| 使用量列表 | GET | `/api/tenants/{tenantRefId}/resource-usage` | - | `ApiResult<List<TenantResourceUsageRepDTO>>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/tenant-resources/TenantResourcesView.vue` | 主页面 |
| 2 | `src/WebTenantPlatfrom/src/views/tenant-resources/TenantResourcesView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/tenant-resources/TenantResourcesView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/tenant-resources/TenantResourcesView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/tenant-resources/TenantResourcesView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/tenant-resources/TenantResourcesView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/tenant-resources.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/tenant-resources.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('租户资源配额')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理租户资源配额设置和使用量监控')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| 租户选择器 | `DxSelectBox` | 选择目标租户 |
| 资源使用概览 | 卡片区域 + `DxProgressBar` | 各资源使用量进度条 |
| 配额列表 | `DxDataGrid` + `CustomStore` | 配额详细列表 |
| 编辑配额弹窗 | `DxPopup` + `DxForm` | 编辑配额设置 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 租户选择器

在页面顶部提供 `DxSelectBox` 用于选择目标租户：
- 数据来源：租户列表 API
- `display-expr`：`TenantName`（或 `TenantCode + ' - ' + TenantName`）
- `value-expr`：`Id`
- placeholder：`$t('请选择租户')`
- 选择后自动加载该租户的资源配额和使用量

---

## 资源使用概览

选择租户后，调用 `GET /api/tenants/{tenantRefId}/resource-usage` 获取使用量数据，以卡片形式展示每种资源的使用情况。

### 卡片内容

每个资源卡片包含：

| 元素 | 内容 |
|------|------|
| 标题 | 资源类型名称（`$t()` 国际化） |
| 进度条 | `DxProgressBar`，`min=0`，`max=QuotaLimit`，`value=CurrentUsage` |
| 使用量文本 | `CurrentUsage / QuotaLimit`（如 `150 / 1000`） |
| 百分比 | `UsagePercent%` |
| 颜色 | 根据百分比变色 |

### 进度条颜色规则

| 使用百分比 | 颜色 | CSS class |
|:--------:|------|-----------|
| 0-60% | 绿色 `#52c41a` | `usage-normal` |
| 61-80% | 黄色 `#faad14` | `usage-warning` |
| 81-100% | 红色 `#f5222d` | `usage-danger` |

---

## 配额列表

### 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入配额类型关键词')` |

### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | QuotaType | `$t('配额类型')` | 150px | 是 | `quotaTypeCell` | |
| 3 | QuotaLimit | `$t('配额上限')` | 120px | 是 | 数字格式化 | |
| 4 | WarningThreshold | `$t('预警阈值')` | 120px | 否 | 数字格式化 | |
| 5 | ResetCycle | `$t('重置周期')` | 120px | 否 | `resetCycleCell` | |
| 6 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 7 | - | `$t('操作')` | 120px | 否 | `actionCell` | 编辑 |

**所有 caption 必须使用 `:caption="$t('...')"` 绑定，禁止硬编码。**

### 分页配置

| 配置 | 值 |
|------|---|
| 支持分页 | 是 |
| 默认页大小 | 20 |
| 可选页大小 | `[10, 20, 50, 100]` |
| 显示总数 | 是 |
| 远程分页 | 是（CustomStore） |

### 配额类型说明

| 类型值 | 显示文本 |
|:------:|---------|
| max_users | `$t('用户数上限')` |
| storage_mb | `$t('存储空间(MB)')` |
| api_calls_daily | `$t('每日API调用数')` |
| api_calls_monthly | `$t('每月API调用数')` |

### 重置周期说明

| 周期值 | 显示文本 |
|:------:|---------|
| none | `$t('不重置')` |
| daily | `$t('每日')` |
| monthly | `$t('每月')` |
| yearly | `$t('每年')` |

---

## 操作按钮

### 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增配额 | `$t('新增配额')` | `add` | `TENANT_RESOURCE_UPDATE` | 已选择租户 | 打开新增/编辑弹窗（空表单） | 无 |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 | 确认框 |
|------|------|------|--------|---------|--------|
| 编辑 | `$t('编辑')` | `edit` | `TENANT_RESOURCE_UPDATE` | 打开编辑配额弹窗 | 无 |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const TENANT_RESOURCE_VIEW = 'tenant.detail'
export const TENANT_RESOURCE_UPDATE = 'tenant.update'
```

### 成功提示

| 操作 | 调用 |
|------|------|
| 保存配额 | `notifySuccess('保存成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 编辑配额表单

**标题**：`$t('编辑资源配额')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 验证规则 | 默认值 | 禁用条件 |
|:----:|--------|------|------|:----:|---------|--------|---------|
| 1 | QuotaType | `$t('配额类型')` | DxSelectBox | 是 | required | `''` | 编辑时禁用 |
| 2 | QuotaLimit | `$t('配额上限')` | DxNumberBox | 是 | required, min: 1 | `0` | - |
| 3 | WarningThreshold | `$t('预警阈值')` | DxNumberBox | 否 | custom: ≤ QuotaLimit | `null` | - |
| 4 | ResetCycle | `$t('重置周期')` | DxSelectBox | 否 | - | `'none'` | - |

**配额类型选项**（DxSelectBox）：

| 值 | 显示文本 |
|:--:|---------|
| max_users | `$t('用户数上限')` |
| storage_mb | `$t('存储空间(MB)')` |
| api_calls_daily | `$t('每日API调用数')` |
| api_calls_monthly | `$t('每月API调用数')` |

**重置周期选项**（DxSelectBox）：

| 值 | 显示文本 |
|:--:|---------|
| none | `$t('不重置')` |
| daily | `$t('每日')` |
| monthly | `$t('每月')` |
| yearly | `$t('每年')` |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| QuotaType | required | - | `$t('请选择配额类型')` |
| QuotaLimit | required | - | `$t('请输入配额上限')` |
| QuotaLimit | range | min: 1 | `$t('配额上限必须大于 0')` |
| WarningThreshold | custom | ≤ QuotaLimit | `$t('预警阈值不能超过配额上限')` |

**提交行为**：

1. 提交前：调用 `formInstance.validate()`，不通过则阻止提交
2. 提交时：`submitting.value = true`，禁用提交按钮并显示 loading
3. 调用 `PUT /api/tenants/{tenantRefId}/resource-quotas`
4. 提交成功：关闭弹窗 → `notifySuccess('保存成功')` → 刷新配额列表和使用量概览
5. 提交失败：axios 拦截器自动显示错误 → 不关闭弹窗 → `submitting.value = false`

**弹窗按钮**：

| 按钮 | 文本 | 位置 | 行为 |
|------|------|------|------|
| 提交 | `$t('确定')` | 弹窗底部右侧 | 提交表单，`submitting` 时 disabled + loading |
| 取消 | `$t('取消')` | 弹窗底部左侧 | 关闭弹窗，重置表单 |

---

## 类型定义

```typescript
// src/types/tenant-resources.ts

/** 资源配额响应 */
export interface TenantResourceQuotaRepDTO {
  Id: number
  TenantRefId: number
  QuotaType: string
  QuotaLimit: number
  WarningThreshold?: number
  ResetCycle?: string
  CreatedAt: string
}

/** 资源使用量响应 */
export interface TenantResourceUsageRepDTO {
  TenantRefId: number
  QuotaType: string
  QuotaLimit: number
  CurrentUsage: number
  UsagePercent: number
}

/** 保存配额请求 */
export interface SaveTenantResourceQuotaReqDTO {
  TenantRefId: number
  QuotaType: string
  QuotaLimit: number
  WarningThreshold?: number
  ResetCycle?: string
}
```

---

## 国际化要求

### 组件级 key（放入 `TenantResourcesView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 租户资源配额 | 租户资源配额 | Tenant Resource Quotas | テナントリソースクォータ | Kuota Sumber Penyewa | 租戶資源配額 |
| 管理租户资源配额设置和使用量监控 | 管理租户资源配额设置和使用量监控 | Manage tenant resource quota settings and usage monitoring | テナントリソースクォータ設定と使用量監視を管理 | Urus tetapan kuota sumber penyewa dan pemantauan penggunaan | 管理租戶資源配額設定和使用量監控 |
| 请选择租户 | 请选择租户 | Select tenant | テナントを選択 | Pilih penyewa | 請選擇租戶 |
| 资源使用概览 | 资源使用概览 | Resource Usage Overview | リソース使用状況概要 | Gambaran Keseluruhan Penggunaan Sumber | 資源使用概覽 |
| 配额类型 | 配额类型 | Quota Type | クォータタイプ | Jenis Kuota | 配額類型 |
| 配额上限 | 配额上限 | Quota Limit | クォータ上限 | Had Kuota | 配額上限 |
| 预警阈值 | 预警阈值 | Warning Threshold | 警告しきい値 | Ambang Amaran | 預警閾值 |
| 重置周期 | 重置周期 | Reset Cycle | リセットサイクル | Kitaran Set Semula | 重置週期 |
| 用户数上限 | 用户数上限 | Max Users | 最大ユーザー数 | Had Pengguna | 使用者數上限 |
| 存储空间(MB) | 存储空间(MB) | Storage (MB) | ストレージ (MB) | Storan (MB) | 儲存空間(MB) |
| 每日API调用数 | 每日API调用数 | Daily API Calls | 日次API呼び出し数 | Panggilan API Harian | 每日API呼叫數 |
| 每月API调用数 | 每月API调用数 | Monthly API Calls | 月次API呼び出し数 | Panggilan API Bulanan | 每月API呼叫數 |
| 不重置 | 不重置 | No Reset | リセットなし | Tiada Set Semula | 不重置 |
| 每日 | 每日 | Daily | 毎日 | Harian | 每日 |
| 每月 | 每月 | Monthly | 毎月 | Bulanan | 每月 |
| 每年 | 每年 | Yearly | 毎年 | Tahunan | 每年 |
| 新增配额 | 新增配额 | Add Quota | クォータ追加 | Tambah Kuota | 新增配額 |
| 编辑资源配额 | 编辑资源配额 | Edit Resource Quota | リソースクォータ編集 | Edit Kuota Sumber | 編輯資源配額 |
| 请选择配额类型 | 请选择配额类型 | Select quota type | クォータタイプを選択 | Pilih jenis kuota | 請選擇配額類型 |
| 请输入配额上限 | 请输入配额上限 | Enter quota limit | クォータ上限を入力 | Masukkan had kuota | 請輸入配額上限 |
| 配额上限必须大于 0 | 配额上限必须大于 0 | Quota limit must be greater than 0 | クォータ上限は0より大きくする必要があります | Had kuota mestilah lebih besar daripada 0 | 配額上限必須大於 0 |
| 预警阈值不能超过配额上限 | 预警阈值不能超过配额上限 | Warning threshold cannot exceed quota limit | 警告しきい値はクォータ上限を超えることはできません | Ambang amaran tidak boleh melebihi had kuota | 預警閾值不能超過配額上限 |
| 请输入配额类型关键词 | 请输入配额类型关键词 | Enter quota type keyword | クォータタイプキーワードを入力 | Masukkan kata kunci jenis kuota | 請輸入配額類型關鍵詞 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`编辑`、`确定`、`取消`、`操作`、`ID`、`创建时间`、`暂无数据`、`功能说明`、`操作指南`、`保存成功`、`关键词`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"租户资源配额"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **租户选择器**存在且可选择租户
- [ ] **资源使用概览**展示各资源的进度条（使用量/配额）
- [ ] 进度条根据百分比变色（绿/黄/红）
- [ ] **配额列表**包含：ID、配额类型、配额上限、预警阈值、重置周期、创建时间、操作
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示
- [ ] **编辑弹窗**包含字段：配额类型、配额上限、预警阈值、重置周期

### P1 — 业务规则完整性

- [ ] 选择租户后自动加载配额列表和使用量
- [ ] 配额类型 `required` 验证
- [ ] 配额上限 `required` + `range`（min: 1）验证
- [ ] 预警阈值不超过配额上限（自定义验证）
- [ ] 编辑时配额类型禁用
- [ ] 每个操作按钮有权限码控制
- [ ] 新增/编辑按钮需已选择租户
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗、刷新配额列表和使用量概览
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `notifySuccess` 不双重 t()
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有配额类型/重置周期显示值已国际化
- [ ] 进度条状态文本已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPut`
