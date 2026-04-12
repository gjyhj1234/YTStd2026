# 租户平台 — 租户管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块为租户生命周期管理，包含 CRUD、状态流转（初始化/暂停/恢复/终止/转正）、生命周期事件查看。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-5 |
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
- `.ai/prompts/08-platform/backend/tenant-lifecycle-api.md` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 租户列表远程分页 |
| DxForm | `DxForm validation rules required stringLength async validationCallback` | 创建/编辑租户表单验证 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 创建/编辑/详情/生命周期事件弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 状态筛选、隔离模式选择 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |
| DxList | `DxList item-template data-source` | 生命周期事件时间线展示 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `TenantEndpoints.cs` 中的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 租户列表 | GET | `/api/tenants?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<TenantRepDTO>>` |
| 租户详情 | GET | `/api/tenants/{id}` | - | `ApiResult<TenantRepDTO>` |
| 创建租户 | POST | `/api/tenants` | `CreateTenantReqDTO` | `ApiResult` |
| 更新租户 | PUT | `/api/tenants/{id}` | `UpdateTenantReqDTO` | `ApiResult` |
| 删除租户 | DELETE | `/api/tenants/{id}` | - | `ApiResult` |
| 状态变更 | PUT | `/api/tenants/{id}/status` | `{ TargetStatus, Reason? }` | `ApiResult` |
| 初始化 | PUT | `/api/tenants/{id}/initialize` | - | `ApiResult` |
| 暂停 | PUT | `/api/tenants/{id}/suspend` | - | `ApiResult` |
| 恢复 | PUT | `/api/tenants/{id}/resume` | - | `ApiResult` |
| 终止 | PUT | `/api/tenants/{id}/terminate` | - | `ApiResult` |
| 试用转正 | PUT | `/api/tenants/{id}/convert-trial` | - | `ApiResult` |
| 检查编码 | GET | `/api/tenants/check-code-exists?code=xxx` | - | `ApiResult<bool>` |
| 生命周期事件 | GET | `/api/tenants/{id}/lifecycle-events?page=1&pageSize=20` | - | `ApiResult<PagedResult<TenantLifecycleEventRepDTO>>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue` | 主页面 |
| 2 | `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/tenants/TenantsView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/tenants.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/tenants.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('租户管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理租户生命周期，包括创建、编辑、状态流转和生命周期事件查看')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| 查询区 | 自定义查询栏 | 关键词 + 状态筛选 |
| 工具栏 | `DxToolbar` | 新增 |
| 表格区 | `DxDataGrid` + `CustomStore` | 租户列表 |
| 分页 | `DxDataGrid` 内置 `DxPager` + `DxPaging` | 远程分页 |
| 新增弹窗 | `DxPopup` + `DxForm` | 创建租户表单 |
| 编辑弹窗 | `DxPopup` + `DxForm` | 编辑租户表单 |
| 详情抽屉 | `DxPopup`（只读展示） | 租户详情 |
| 生命周期事件弹窗 | `DxPopup` + `DxList` / 时间线组件 | 生命周期事件列表 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入租户编码或名称')` |
| 2 | Status | `$t('状态')` | DxSelectBox | `null`（全部） | `$t('请选择状态')` |

**状态下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| pending | `$t('待初始化')` |
| active | `$t('活跃')` |
| suspended | `$t('暂停')` |
| trial | `$t('试用中')` |
| terminated | `$t('已终止')` |

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
| 2 | TenantCode | `$t('租户编码')` | 150px | 是 | - | |
| 3 | TenantName | `$t('租户名称')` | auto | 是 | - | |
| 4 | EnterpriseName | `$t('企业名称')` | auto | 否 | - | |
| 5 | LifecycleStatus | `$t('生命周期状态')` | 120px | 否 | `statusCell` | 颜色标签 |
| 6 | IsolationMode | `$t('隔离模式')` | 100px | 否 | `isolationCell` | |
| 7 | ContactName | `$t('联系人')` | 100px | 否 | - | |
| 8 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 9 | - | `$t('操作')` | 520px | 否 | `actionCell` | 操作按钮列 |

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
| pending | `$t('待初始化')` | 灰色 `#8c8c8c` | `status-pending` |
| active | `$t('活跃')` | 绿色 `#52c41a` | `status-active` |
| suspended | `$t('暂停')` | 黄色 `#faad14` | `status-suspended` |
| trial | `$t('试用中')` | 蓝色 `#1890ff` | `status-trial` |
| terminated | `$t('已终止')` | 红色 `#f5222d` | `status-terminated` |

### 隔离模式列

| 模式值 | 显示文本 |
|:------:|---------|
| shared | `$t('共享模式')` |
| isolated | `$t('隔离模式')` |

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
| 新增 | `$t('新增')` | `add` | `TENANT_CREATE` | 始终启用 | 打开新增弹窗 | 无 |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `TENANT_VIEW` | 始终 | 打开详情抽屉 | 无 |
| 编辑 | `$t('编辑')` | `edit` | `TENANT_UPDATE` | `LifecycleStatus !== 'terminated'` | 打开编辑弹窗 | 无 |
| 初始化 | `$t('初始化')` | `runner` | `TENANT_UPDATE` | `LifecycleStatus === 'pending'` | 调用初始化 API | `confirmAction('确认初始化租户 {name}')` |
| 暂停 | `$t('暂停')` | `pause` | `TENANT_UPDATE` | `LifecycleStatus === 'active' \|\| LifecycleStatus === 'trial'` | 调用暂停 API | `confirmAction('确认暂停租户 {name}')` |
| 恢复 | `$t('恢复')` | `revert` | `TENANT_UPDATE` | `LifecycleStatus === 'suspended'` | 调用恢复 API | `confirmAction('确认恢复租户 {name}')` |
| 终止 | `$t('终止')` | `clearsquare` | `TENANT_UPDATE` | `LifecycleStatus !== 'terminated' && LifecycleStatus !== 'pending'` | 调用终止 API | `confirmAction('确认终止租户 {name}，此操作不可逆')` |
| 转正 | `$t('转正')` | `todo` | `TENANT_UPDATE` | `LifecycleStatus === 'trial'` | 调用转正 API | `confirmAction('确认将试用租户 {name} 转为正式')` |
| 事件 | `$t('生命周期事件')` | `event` | `TENANT_VIEW` | 始终 | 打开生命周期事件弹窗 | 无 |
| 删除 | `$t('删除')` | `trash` | `TENANT_DELETE` | 始终 | 调用删除 API | `confirmDelete(row.TenantName)` |

### 状态流转按钮动态显隐规则

> 此规则是本页面的核心业务逻辑，必须严格对应后端状态机。

| 当前状态 | 允许的操作按钮 | 不允许的操作按钮 |
|---------|--------------|----------------|
| pending | 初始化 | 暂停、恢复、终止、转正 |
| active | 暂停、终止 | 初始化、恢复、转正 |
| suspended | 恢复、终止 | 初始化、暂停、转正 |
| trial | 暂停、终止、转正 | 初始化、恢复 |
| terminated | 无 | 初始化、暂停、恢复、终止、转正（所有状态操作均禁用） |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const TENANT_VIEW = 'tenant.detail'
export const TENANT_CREATE = 'tenant.create'
export const TENANT_UPDATE = 'tenant.update'
export const TENANT_DELETE = 'tenant.delete'
```

### 确认框文案（全部国际化）

| 操作 | 文案 key | 说明 |
|------|---------|------|
| 初始化 | `'确认初始化租户 {name}'` | 需在 common 语言文件中有对应翻译 |
| 暂停 | `'确认暂停租户 {name}'` | 同上 |
| 恢复 | `'确认恢复租户 {name}'` | 同上 |
| 终止 | `'确认终止租户 {name}，此操作不可逆'` | 需特别提示不可逆 |
| 转正 | `'确认将试用租户 {name} 转为正式'` | 同上 |
| 删除 | `confirmDelete(row.TenantName)` | 使用通用删除确认 |

### 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 创建 | `notifySuccess('创建成功')` |
| 更新 | `notifySuccess('更新成功')` |
| 初始化 | `notifySuccess('初始化成功')` |
| 暂停 | `notifySuccess('暂停成功')` |
| 恢复 | `notifySuccess('恢复成功')` |
| 终止 | `notifySuccess('终止成功')` |
| 转正 | `notifySuccess('转正成功')` |
| 删除 | `notifySuccess('删除成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 表单功能

### 新增表单

**标题**：`$t('新增租户')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantCode | `$t('租户编码')` | DxTextBox | 是 | 2-50 | `^[a-zA-Z0-9_-]+$` 字母数字下划线连字符 | 是 async | `''` | - | - |
| 2 | TenantName | `$t('租户名称')` | DxTextBox | 是 | 2-100 | - | - | `''` | - | - |
| 3 | EnterpriseName | `$t('企业名称')` | DxTextBox | 否 | 0-200 | - | - | `''` | - | - |
| 4 | ContactName | `$t('联系人')` | DxTextBox | 否 | 0-50 | - | - | `''` | - | - |
| 5 | ContactPhone | `$t('联系电话')` | DxTextBox | 否 | 0-20 | - | - | `''` | - | - |
| 6 | ContactEmail | `$t('联系邮箱')` | DxTextBox | 否 | 0-100 | email 格式 | - | `''` | - | - |
| 7 | SourceType | `$t('来源类型')` | DxSelectBox | 是 | - | - | - | `'manual'` | - | - |
| 8 | IsolationMode | `$t('隔离模式')` | DxSelectBox | 是 | - | - | - | `'shared'` | - | - |
| 9 | DefaultLanguage | `$t('默认语言')` | DxSelectBox | 是 | - | - | - | `'zh-CN'` | - | - |
| 10 | DefaultTimezone | `$t('默认时区')` | DxSelectBox | 是 | - | - | - | `'Asia/Shanghai'` | - | - |

**来源类型选项**：

| 值 | 显示文本 |
|:--:|---------|
| manual | `$t('手动创建')` |
| registration | `$t('注册')` |
| import | `$t('导入')` |

**隔离模式选项**：

| 值 | 显示文本 |
|:--:|---------|
| shared | `$t('共享模式')` |
| isolated | `$t('隔离模式')` |

**默认语言选项**：

| 值 | 显示文本 |
|:--:|---------|
| zh-CN | 简体中文 |
| en-US | English |
| ja-JP | 日本語 |
| ms-MY | Bahasa Melayu |
| zh-TW | 繁體中文 |

**唯一性校验**：

- TenantCode：调用 `GET /api/tenants/check-code-exists?code=xxx`
- DxForm async validation：

```typescript
{
  type: 'async',
  validationCallback: async (params) => {
    const exists = await checkCodeExists(params.value)
    return !exists
  },
  message: t('租户编码已存在')
}
```

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| TenantCode | required | - | `$t('请输入租户编码')` |
| TenantCode | stringLength | min: 2, max: 50 | `$t('租户编码长度 2-50 个字符')` |
| TenantCode | pattern | `^[a-zA-Z0-9_-]+$` | `$t('租户编码仅允许字母、数字、下划线和连字符')` |
| TenantCode | async | checkCodeExists | `$t('租户编码已存在')` |
| TenantName | required | - | `$t('请输入租户名称')` |
| TenantName | stringLength | min: 2, max: 100 | `$t('租户名称长度 2-100 个字符')` |
| EnterpriseName | stringLength | max: 200 | `$t('企业名称长度不超过 200 个字符')` |
| ContactName | stringLength | max: 50 | `$t('联系人长度不超过 50 个字符')` |
| ContactPhone | stringLength | max: 20 | `$t('联系电话长度不超过 20 个字符')` |
| ContactEmail | email | - | `$t('请输入有效的邮箱地址')` |
| ContactEmail | stringLength | max: 100 | `$t('联系邮箱长度不超过 100 个字符')` |

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

**标题**：`$t('编辑租户')`
**组件**：`DxPopup`（`width: 700`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 格式校验 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|---------|---------|:------:|--------|---------|---------|
| 1 | TenantCode | `$t('租户编码')` | DxTextBox | - | - | - | - | 从数据加载 | **始终禁用** | 显示（只读） |
| 2 | TenantName | `$t('租户名称')` | DxTextBox | 否 | 2-100 | - | - | 从数据加载 | - | - |
| 3 | EnterpriseName | `$t('企业名称')` | DxTextBox | 否 | 0-200 | - | - | 从数据加载 | - | - |
| 4 | ContactName | `$t('联系人')` | DxTextBox | 否 | 0-50 | - | - | 从数据加载 | - | - |
| 5 | ContactPhone | `$t('联系电话')` | DxTextBox | 否 | 0-20 | - | - | 从数据加载 | - | - |
| 6 | ContactEmail | `$t('联系邮箱')` | DxTextBox | 否 | 0-100 | email 格式 | - | 从数据加载 | - | - |

**注意**：编辑时 TenantCode 字段为只读（`disabled: true`），不可修改。编辑接口 `UpdateTenantReqDTO` 不含 TenantCode、SourceType、IsolationMode、DefaultLanguage、DefaultTimezone 字段。

**提交行为**：同新增表单，成功提示为 `notifySuccess('更新成功')`。

### 详情展示

**标题**：`$t('租户详情')`
**组件**：`DxPopup`（只读展示）

展示字段：

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | TenantCode | `$t('租户编码')` | - |
| 3 | TenantName | `$t('租户名称')` | - |
| 4 | EnterpriseName | `$t('企业名称')` | - |
| 5 | ContactName | `$t('联系人')` | - |
| 6 | ContactEmail | `$t('联系邮箱')` | - |
| 7 | LifecycleStatus | `$t('生命周期状态')` | 状态标签（颜色） |
| 8 | IsolationMode | `$t('隔离模式')` | - |
| 9 | Enabled | `$t('是否启用')` | 布尔值显示 |
| 10 | OpenedAt | `$t('开通时间')` | `yyyy-MM-dd HH:mm:ss` |
| 11 | ExpiresAt | `$t('过期时间')` | `yyyy-MM-dd HH:mm:ss` |
| 12 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 生命周期事件弹窗

**标题**：`$t('生命周期事件')`
**组件**：`DxPopup`（`width: 700`，`height: 500`）

- 调用 `GET /api/tenants/{id}/lifecycle-events?page=1&pageSize=50` 获取事件列表
- 按时间倒序展示，每条事件包含：

| 字段 | 标签 | 格式化 |
|------|------|--------|
| EventType | `$t('事件类型')` | 事件类型标签 |
| FromStatus | `$t('原状态')` | 状态标签 |
| ToStatus | `$t('目标状态')` | 状态标签 |
| Reason | `$t('原因')` | - |
| OccurredAt | `$t('发生时间')` | `yyyy-MM-dd HH:mm:ss` |

---

## 类型定义

```typescript
// src/types/tenants.ts

/** 创建租户请求 */
export interface CreateTenantReqDTO {
  TenantCode: string
  TenantName: string
  EnterpriseName?: string
  ContactName?: string
  ContactPhone?: string
  ContactEmail?: string
  SourceType: string
  IsolationMode: string
  DefaultLanguage: string
  DefaultTimezone: string
}

/** 更新租户请求 */
export interface UpdateTenantReqDTO {
  TenantName?: string
  EnterpriseName?: string
  ContactName?: string
  ContactPhone?: string
  ContactEmail?: string
}

/** 状态变更请求 */
export interface TenantStatusChangeReqDTO {
  TargetStatus: string
  Reason?: string
}

/** 租户响应 */
export interface TenantRepDTO {
  Id: number
  TenantCode: string
  TenantName: string
  EnterpriseName?: string
  ContactName?: string
  ContactEmail?: string
  LifecycleStatus: string
  IsolationMode: string
  Enabled: boolean
  OpenedAt?: string
  ExpiresAt?: string
  CreatedAt: string
}

/** 生命周期事件响应 */
export interface TenantLifecycleEventRepDTO {
  Id: number
  TenantRefId: number
  EventType: string
  FromStatus?: string
  ToStatus?: string
  Reason?: string
  OccurredAt: string
}
```

---

## 国际化要求

### 组件级 key（放入 `TenantsView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 租户管理 | 租户管理 | Tenant Management | テナント管理 | Pengurusan Penyewa | 租戶管理 |
| 管理租户生命周期，包括创建、编辑、状态流转和生命周期事件查看 | (同key) | Manage tenant lifecycle including create, edit, status transitions, and lifecycle event viewing | テナントのライフサイクル管理：作成、編集、ステータス遷移、イベント表示 | Urus kitaran hayat penyewa termasuk cipta, edit, peralihan status dan paparan acara | 管理租戶生命週期，包括建立、編輯、狀態流轉和生命週期事件查看 |
| 请输入租户编码或名称 | 请输入租户编码或名称 | Enter tenant code or name | テナントコードまたは名前を入力 | Masukkan kod penyewa atau nama | 請輸入租戶編碼或名稱 |
| 租户编码 | 租户编码 | Tenant Code | テナントコード | Kod Penyewa | 租戶編碼 |
| 租户名称 | 租户名称 | Tenant Name | テナント名 | Nama Penyewa | 租戶名稱 |
| 企业名称 | 企业名称 | Enterprise Name | 企業名 | Nama Perusahaan | 企業名稱 |
| 联系人 | 联系人 | Contact Person | 担当者 | Orang Hubungan | 聯絡人 |
| 联系电话 | 联系电话 | Contact Phone | 連絡先電話 | Telefon Hubungan | 聯絡電話 |
| 联系邮箱 | 联系邮箱 | Contact Email | 連絡先メール | E-mel Hubungan | 聯絡郵箱 |
| 生命周期状态 | 生命周期状态 | Lifecycle Status | ライフサイクルステータス | Status Kitaran Hayat | 生命週期狀態 |
| 隔离模式 | 隔离模式 | Isolation Mode | アイソレーションモード | Mod Pengasingan | 隔離模式 |
| 共享模式 | 共享模式 | Shared Mode | 共有モード | Mod Dikongsi | 共享模式 |
| 来源类型 | 来源类型 | Source Type | ソースタイプ | Jenis Sumber | 來源類型 |
| 手动创建 | 手动创建 | Manual | 手動作成 | Manual | 手動建立 |
| 注册 | 注册 | Registration | 登録 | Pendaftaran | 註冊 |
| 导入 | 导入 | Import | インポート | Import | 匯入 |
| 默认语言 | 默认语言 | Default Language | デフォルト言語 | Bahasa Lalai | 預設語言 |
| 默认时区 | 默认时区 | Default Timezone | デフォルトタイムゾーン | Zon Masa Lalai | 預設時區 |
| 待初始化 | 待初始化 | Pending | 初期化待ち | Menunggu Permulaan | 待初始化 |
| 活跃 | 活跃 | Active | アクティブ | Aktif | 活躍 |
| 暂停 | 暂停 | Suspended | 停止中 | Digantung | 暫停 |
| 试用中 | 试用中 | Trial | トライアル中 | Percubaan | 試用中 |
| 已终止 | 已终止 | Terminated | 終了 | Ditamatkan | 已終止 |
| 新增租户 | 新增租户 | Create Tenant | テナント作成 | Cipta Penyewa | 新增租戶 |
| 编辑租户 | 编辑租户 | Edit Tenant | テナント編集 | Edit Penyewa | 編輯租戶 |
| 租户详情 | 租户详情 | Tenant Details | テナント詳細 | Butiran Penyewa | 租戶詳情 |
| 生命周期事件 | 生命周期事件 | Lifecycle Events | ライフサイクルイベント | Acara Kitaran Hayat | 生命週期事件 |
| 初始化 | 初始化 | Initialize | 初期化 | Permulaan | 初始化 |
| 恢复 | 恢复 | Resume | 復元 | Pulihkan | 恢復 |
| 终止 | 终止 | Terminate | 終了 | Tamatkan | 終止 |
| 转正 | 转正 | Convert to Formal | 正式化 | Tukar ke Formal | 轉正 |
| 是否启用 | 是否启用 | Enabled | 有効 | Diaktifkan | 是否啟用 |
| 开通时间 | 开通时间 | Opened At | 開通日時 | Masa Pembukaan | 開通時間 |
| 过期时间 | 过期时间 | Expires At | 有効期限 | Masa Tamat Tempoh | 過期時間 |
| 事件类型 | 事件类型 | Event Type | イベントタイプ | Jenis Acara | 事件類型 |
| 原状态 | 原状态 | From Status | 元ステータス | Status Asal | 原狀態 |
| 目标状态 | 目标状态 | To Status | 目標ステータス | Status Sasaran | 目標狀態 |
| 原因 | 原因 | Reason | 理由 | Sebab | 原因 |
| 发生时间 | 发生时间 | Occurred At | 発生日時 | Masa Berlaku | 發生時間 |
| 请输入租户编码 | 请输入租户编码 | Enter tenant code | テナントコードを入力 | Masukkan kod penyewa | 請輸入租戶編碼 |
| 请输入租户名称 | 请输入租户名称 | Enter tenant name | テナント名を入力 | Masukkan nama penyewa | 請輸入租戶名稱 |
| 租户编码长度 2-50 个字符 | 租户编码长度 2-50 个字符 | Tenant code must be 2-50 characters | テナントコードは2-50文字 | Kod penyewa mestilah 2-50 aksara | 租戶編碼長度 2-50 個字元 |
| 租户编码仅允许字母、数字、下划线和连字符 | 租户编码仅允许字母、数字、下划线和连字符 | Tenant code can only contain letters, numbers, underscores and hyphens | テナントコードは英数字、アンダースコア、ハイフンのみ | Kod penyewa hanya boleh mengandungi huruf, nombor, garis bawah dan sempang | 租戶編碼僅允許字母、數字、底線和連字符 |
| 租户编码已存在 | 租户编码已存在 | Tenant code already exists | テナントコードは既に存在します | Kod penyewa sudah wujud | 租戶編碼已存在 |
| 租户名称长度 2-100 个字符 | 租户名称长度 2-100 个字符 | Tenant name must be 2-100 characters | テナント名は2-100文字 | Nama penyewa mestilah 2-100 aksara | 租戶名稱長度 2-100 個字元 |
| 企业名称长度不超过 200 个字符 | 企业名称长度不超过 200 个字符 | Enterprise name must not exceed 200 characters | 企業名は200文字以内 | Nama perusahaan mestilah tidak melebihi 200 aksara | 企業名稱長度不超過 200 個字元 |
| 请输入有效的邮箱地址 | 请输入有效的邮箱地址 | Please enter a valid email address | 有効なメールアドレスを入力してください | Sila masukkan alamat e-mel yang sah | 請輸入有效的郵箱地址 |
| 确认初始化租户 {name} | 确认初始化租户 {name} | Confirm initialize tenant {name} | テナント {name} を初期化しますか | Sahkan permulaan penyewa {name} | 確認初始化租戶 {name} |
| 确认暂停租户 {name} | 确认暂停租户 {name} | Confirm suspend tenant {name} | テナント {name} を停止しますか | Sahkan gantung penyewa {name} | 確認暫停租戶 {name} |
| 确认恢复租户 {name} | 确认恢复租户 {name} | Confirm resume tenant {name} | テナント {name} を復元しますか | Sahkan pulihkan penyewa {name} | 確認恢復租戶 {name} |
| 确认终止租户 {name}，此操作不可逆 | 确认终止租户 {name}，此操作不可逆 | Confirm terminate tenant {name}, this action is irreversible | テナント {name} を終了しますか（この操作は取り消せません） | Sahkan tamatkan penyewa {name}, tindakan ini tidak boleh ditarik balik | 確認終止租戶 {name}，此操作不可逆 |
| 确认将试用租户 {name} 转为正式 | 确认将试用租户 {name} 转为正式 | Confirm convert trial tenant {name} to formal | テナント {name} を正式化しますか | Sahkan tukar penyewa percubaan {name} ke formal | 確認將試用租戶 {name} 轉為正式 |
| 初始化成功 | 初始化成功 | Initialization successful | 初期化成功 | Permulaan berjaya | 初始化成功 |
| 暂停成功 | 暂停成功 | Suspension successful | 停止成功 | Penggantungan berjaya | 暫停成功 |
| 恢复成功 | 恢复成功 | Resume successful | 復元成功 | Pemulihan berjaya | 恢復成功 |
| 终止成功 | 终止成功 | Termination successful | 終了成功 | Penamatan berjaya | 終止成功 |
| 转正成功 | 转正成功 | Conversion successful | 正式化成功 | Penukaran berjaya | 轉正成功 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`编辑`、`删除`、`查看`、`确定`、`取消`、`操作`、`ID`、`状态`、`创建时间`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`创建成功`、`更新成功`、`删除成功`、`请选择状态`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"租户管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **查询区**包含：关键词输入框、状态下拉（6 个状态选项 + 全部）
- [ ] **表格列**包含：ID、租户编码、租户名称、企业名称、生命周期状态、隔离模式、联系人、创建时间、操作
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 远程分页通过 CustomStore 实现
- [ ] **工具栏**包含：新增按钮
- [ ] **行操作**包含：查看、编辑、初始化、暂停、恢复、终止、转正、生命周期事件、删除
- [ ] **新增弹窗**包含字段：租户编码、租户名称、企业名称、联系人、联系电话、联系邮箱、来源类型、隔离模式、默认语言、默认时区
- [ ] **编辑弹窗**包含字段：租户编码（只读）、租户名称、企业名称、联系人、联系电话、联系邮箱
- [ ] **详情抽屉**展示：ID、租户编码、租户名称、企业名称、联系人、联系邮箱、生命周期状态、隔离模式、是否启用、开通时间、过期时间、创建时间
- [ ] **生命周期事件弹窗**展示事件列表（事件类型、原状态、目标状态、原因、发生时间）

### P1 — 业务规则完整性

- [ ] 租户编码 `required` 验证
- [ ] 租户编码 `stringLength`（2-50）验证
- [ ] 租户编码 `pattern`（字母数字下划线连字符）验证
- [ ] 租户编码 `async` 唯一性验证（新增时）
- [ ] 编辑时租户编码字段 disabled（不可修改）
- [ ] 租户名称 `required` 验证
- [ ] 租户名称 `stringLength`（2-100）验证
- [ ] 联系邮箱 `email` 格式验证
- [ ] 每个操作按钮有权限码控制
- [ ] **状态流转按钮动态显隐**严格对应后端状态机：
  - pending：仅显示 初始化
  - active：仅显示 暂停、终止
  - suspended：仅显示 恢复、终止
  - trial：仅显示 暂停、终止、转正
  - terminated：不显示任何状态操作按钮
- [ ] 终止操作确认框包含"此操作不可逆"提示
- [ ] 每个状态操作有 `confirmAction` 确认
- [ ] 删除有 `confirmDelete` 确认
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' TenantsView.vue | grep -v ':caption'` 结果为 0
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] `grep -rn "notifySuccess(t(" TenantsView.vue` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有状态显示值已国际化（5 种生命周期状态 + 2 种隔离模式）
- [ ] 所有确认框文案已国际化
- [ ] 所有成功提示已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
