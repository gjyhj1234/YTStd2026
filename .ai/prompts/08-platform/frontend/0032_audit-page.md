# 租户平台 — 审计日志页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块为审计日志查询，包含操作日志、审计日志、登录日志三个标签页。所有日志均为只读，不支持增删改。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-13 |
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
- `src/YTStdTenantPlatform/Endpoints/AuditEndpoints.cs` — 后端 API 端点定义
- `src/YTStdTenantPlatform/Application/Dtos/Audit/AuditRepDTO.cs` — 后端 DTO 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take totalCount` | 三个标签页列表远程分页 |
| DxTabPanel | `DxTabPanel items selectedIndex onSelectionChanged` | 三标签页切换（操作日志 / 审计日志 / 登录日志） |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 日志详情弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 关键词搜索 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 状态筛选 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 页面加载 |
| DxToolbar | `DxToolbar items location widget DxButton` | 查询工具栏 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `AuditEndpoints.cs` 中的路由注册。

### 操作日志端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 操作日志列表 | GET | `/api/operation-logs?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<OperationLogRepDTO>>` |
| 操作日志详情 | GET | `/api/operation-logs/{id}` | - | `ApiResult<OperationLogRepDTO>` |

### 审计日志端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 审计日志列表 | GET | `/api/audit-logs?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<AuditLogRepDTO>>` |
| 审计日志详情 | GET | `/api/audit-logs/{id}` | - | `ApiResult<AuditLogRepDTO>` |

### 登录日志端点

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 登录日志列表 | GET | `/api/login-logs?page=1&pageSize=20&keyword=&status=` | - | `ApiResult<PagedResult<SystemLogRepDTO>>` |
| 登录日志详情 | GET | `/api/login-logs/{id}` | - | `ApiResult<SystemLogRepDTO>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/audit/AuditView.vue` | 主页面（含 3 个标签页） |
| 2 | `src/WebTenantPlatfrom/src/views/audit/AuditView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/audit/AuditView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/audit/AuditView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/audit/AuditView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/audit/AuditView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/audit.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/audit.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('审计日志')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('查看操作日志、审计日志和登录日志')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| 标签页容器 | `DxTabPanel` | 3 个标签页切换 |
| Tab 1: 操作日志 | DxDataGrid + CustomStore | 操作日志列表（只读） |
| Tab 2: 审计日志 | DxDataGrid + CustomStore | 审计日志列表（只读） |
| Tab 3: 登录日志 | DxDataGrid + CustomStore | 登录日志列表（只读） |
| 详情弹窗 | DxPopup（只读展示） | 日志详情展示 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## 查询功能

### Tab 1: 操作日志 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入操作动作或资源类型')` |
| 2 | Status | `$t('操作结果')` | DxSelectBox | `null`（全部） | `$t('请选择操作结果')` |

**操作结果下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| success | `$t('成功')` |
| failure | `$t('失败')` |

### Tab 2: 审计日志 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入审计类型或主体类型')` |
| 2 | Status | `$t('严重级别')` | DxSelectBox | `null`（全部） | `$t('请选择严重级别')` |

**严重级别下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| info | `$t('信息')` |
| warning | `$t('警告')` |
| error | `$t('错误')` |
| critical | `$t('严重')` |

### Tab 3: 登录日志 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入服务名称或日志消息')` |
| 2 | Status | `$t('日志级别')` | DxSelectBox | `null`（全部） | `$t('请选择日志级别')` |

**日志级别下拉选项**：

| 值 | 显示文本 |
|:--:|---------|
| null | `$t('全部')` |
| info | `$t('信息')` |
| warning | `$t('警告')` |
| error | `$t('错误')` |

### 查询行为

| 行为 | 要求 |
|------|------|
| 回车搜索 | 在关键词输入框回车触发搜索 |
| 查询按钮 | `$t('查询')`，点击触发搜索 |
| 重置按钮 | `$t('重置')`，清空所有条件并重新加载 |
| 所有文本国际化 | 所有 label、placeholder、按钮文本均使用 `$t()` |

---

## 列表与分页

### Tab 1: 操作日志列表

**表格组件**：DxDataGrid + CustomStore

#### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | 固定宽度 |
| 2 | TenantRefId | `$t('租户ID')` | 100px | 否 | - | |
| 3 | OperatorType | `$t('操作者类型')` | 100px | 否 | - | |
| 4 | OperatorId | `$t('操作者ID')` | 100px | 否 | - | |
| 5 | Action | `$t('操作动作')` | auto | 是 | - | |
| 6 | ResourceType | `$t('资源类型')` | 120px | 否 | - | |
| 7 | ResourceId | `$t('资源ID')` | 100px | 否 | - | |
| 8 | IpAddress | `$t('IP 地址')` | 140px | 否 | - | |
| 9 | OperationResult | `$t('操作结果')` | 100px | 否 | `resultCell` | 颜色标签 |
| 10 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 11 | - | `$t('操作')` | 100px | 否 | `actionCell` | 查看详情按钮 |

### Tab 2: 审计日志列表

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | TenantRefId | `$t('租户ID')` | 100px | 否 | - | |
| 3 | AuditType | `$t('审计类型')` | 120px | 是 | - | |
| 4 | Severity | `$t('严重级别')` | 100px | 否 | `severityCell` | 颜色标签 |
| 5 | SubjectType | `$t('主体类型')` | 120px | 否 | - | |
| 6 | SubjectId | `$t('主体ID')` | 100px | 否 | - | |
| 7 | ComplianceTag | `$t('合规标签')` | 120px | 否 | - | |
| 8 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 9 | - | `$t('操作')` | 100px | 否 | `actionCell` | 查看详情按钮 |

### Tab 3: 登录日志列表

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|--------|------|
| 1 | Id | `$t('ID')` | 80px | 否 | - | |
| 2 | ServiceName | `$t('服务名称')` | 150px | 是 | - | |
| 3 | LogLevel | `$t('日志级别')` | 100px | 否 | `logLevelCell` | 颜色标签 |
| 4 | TraceId | `$t('链路ID')` | 200px | 否 | - | |
| 5 | Message | `$t('日志消息')` | auto | 否 | - | 文本过长截断 |
| 6 | CreatedAt | `$t('创建时间')` | 180px | 是 | `dateCell` | `yyyy-MM-dd HH:mm` |
| 7 | - | `$t('操作')` | 100px | 否 | `actionCell` | 查看详情按钮 |

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

### 操作结果颜色

| 状态值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| success | `$t('成功')` | 绿色 `#52c41a` | `result-success` |
| failure | `$t('失败')` | 红色 `#f5222d` | `result-failure` |

### 严重级别颜色

| 级别值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| info | `$t('信息')` | 蓝色 `#1890ff` | `severity-info` |
| warning | `$t('警告')` | 黄色 `#faad14` | `severity-warning` |
| error | `$t('错误')` | 红色 `#f5222d` | `severity-error` |
| critical | `$t('严重')` | 深红 `#cf1322` | `severity-critical` |

### 日志级别颜色

| 级别值 | 显示文本 | 标签颜色 | CSS class |
|:------:|---------|---------|-----------|
| info | `$t('信息')` | 蓝色 `#1890ff` | `level-info` |
| warning | `$t('警告')` | 黄色 `#faad14` | `level-warning` |
| error | `$t('错误')` | 红色 `#f5222d` | `level-error` |

### 空状态与加载

| 状态 | 显示 |
|------|------|
| 空数据 | `:no-data-text="$t('暂无数据')"` |
| 加载中 | `DxLoadPanel`（`visible` 绑定 `pageLoading`） |

---

## 操作按钮

> 本模块所有日志为只读，无工具栏增删改按钮。

### 行操作按钮（三个标签页通用）

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | `$t('查看')` | `search` | `AUDIT_VIEW` | 始终 | 打开详情弹窗 | 无 |

### 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const AUDIT_VIEW = 'audit.detail'
```

### 成功提示

> 本模块为只读，无增删改操作，无成功提示。

---

## 表单功能

> 本模块所有日志为只读，不包含新增/编辑表单。

### 操作日志详情展示

**标题**：`$t('操作日志详情')`
**组件**：`DxPopup`（只读展示，`width: 700`，`height: auto`）

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | TenantRefId | `$t('租户ID')` | - |
| 3 | OperatorType | `$t('操作者类型')` | - |
| 4 | OperatorId | `$t('操作者ID')` | - |
| 5 | Action | `$t('操作动作')` | - |
| 6 | ResourceType | `$t('资源类型')` | - |
| 7 | ResourceId | `$t('资源ID')` | - |
| 8 | IpAddress | `$t('IP 地址')` | - |
| 9 | OperationResult | `$t('操作结果')` | 颜色标签 |
| 10 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 审计日志详情展示

**标题**：`$t('审计日志详情')`
**组件**：`DxPopup`（只读展示，`width: 700`，`height: auto`）

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | TenantRefId | `$t('租户ID')` | - |
| 3 | AuditType | `$t('审计类型')` | - |
| 4 | Severity | `$t('严重级别')` | 颜色标签 |
| 5 | SubjectType | `$t('主体类型')` | - |
| 6 | SubjectId | `$t('主体ID')` | - |
| 7 | ComplianceTag | `$t('合规标签')` | - |
| 8 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

### 登录日志详情展示

**标题**：`$t('登录日志详情')`
**组件**：`DxPopup`（只读展示，`width: 700`，`height: auto`）

| 序号 | 字段名 | 标签 | 格式化 |
|:----:|--------|------|--------|
| 1 | Id | `$t('ID')` | - |
| 2 | ServiceName | `$t('服务名称')` | - |
| 3 | LogLevel | `$t('日志级别')` | 颜色标签 |
| 4 | TraceId | `$t('链路ID')` | - |
| 5 | Message | `$t('日志消息')` | 长文本展示（pre-wrap） |
| 6 | CreatedAt | `$t('创建时间')` | `yyyy-MM-dd HH:mm:ss` |

**弹窗按钮**：

| 按钮 | 文本 | 位置 | 行为 |
|------|------|------|------|
| 关闭 | `$t('关闭')` | 弹窗底部右侧 | 关闭弹窗 |

### 类型定义

```typescript
// src/types/audit.ts

/** 操作日志响应 */
export interface OperationLogRepDTO {
  Id: number
  TenantRefId?: number
  OperatorType: string
  OperatorId?: number
  Action: string
  ResourceType?: string
  ResourceId?: string
  IpAddress?: string
  OperationResult: string
  CreatedAt: string
}

/** 审计日志响应 */
export interface AuditLogRepDTO {
  Id: number
  TenantRefId?: number
  AuditType: string
  Severity: string
  SubjectType?: string
  SubjectId?: string
  ComplianceTag?: string
  CreatedAt: string
}

/** 登录日志（系统日志）响应 */
export interface SystemLogRepDTO {
  Id: number
  ServiceName: string
  LogLevel: string
  TraceId?: string
  Message: string
  CreatedAt: string
}
```

---

## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| 无独立静态配置文件 | 状态字典、列定义均内联在页面组件中 | 页面组件级语言文件 |

> 本模块为只读日志查询，状态选项简单，不需要独立配置文件。

---

## 国际化要求

### 组件级 key（放入 `AuditView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 审计日志 | 审计日志 | Audit Logs | 監査ログ | Log Audit | 稽核日誌 |
| 查看操作日志、审计日志和登录日志 | (同key) | View operation logs, audit logs, and login logs | 操作ログ、監査ログ、ログインログの表示 | Lihat log operasi, log audit dan log log masuk | 查看操作日誌、稽核日誌和登入日誌 |
| 操作日志 | 操作日志 | Operation Logs | 操作ログ | Log Operasi | 操作日誌 |
| 审计日志详情 | 审计日志详情 | Audit Log Details | 監査ログ詳細 | Butiran Log Audit | 稽核日誌詳情 |
| 登录日志 | 登录日志 | Login Logs | ログインログ | Log Log Masuk | 登入日誌 |
| 操作日志详情 | 操作日志详情 | Operation Log Details | 操作ログ詳細 | Butiran Log Operasi | 操作日誌詳情 |
| 登录日志详情 | 登录日志详情 | Login Log Details | ログインログ詳細 | Butiran Log Log Masuk | 登入日誌詳情 |
| 请输入操作动作或资源类型 | 请输入操作动作或资源类型 | Enter action or resource type | 操作またはリソースタイプを入力 | Masukkan tindakan atau jenis sumber | 請輸入操作動作或資源類型 |
| 请输入审计类型或主体类型 | 请输入审计类型或主体类型 | Enter audit type or subject type | 監査タイプまたは主体タイプを入力 | Masukkan jenis audit atau jenis subjek | 請輸入稽核類型或主體類型 |
| 请输入服务名称或日志消息 | 请输入服务名称或日志消息 | Enter service name or log message | サービス名またはログメッセージを入力 | Masukkan nama perkhidmatan atau mesej log | 請輸入服務名稱或日誌訊息 |
| 操作者类型 | 操作者类型 | Operator Type | 操作者タイプ | Jenis Pengendali | 操作者類型 |
| 操作者ID | 操作者ID | Operator ID | 操作者ID | ID Pengendali | 操作者ID |
| 操作动作 | 操作动作 | Action | 操作 | Tindakan | 操作動作 |
| 资源类型 | 资源类型 | Resource Type | リソースタイプ | Jenis Sumber | 資源類型 |
| 资源ID | 资源ID | Resource ID | リソースID | ID Sumber | 資源ID |
| IP 地址 | IP 地址 | IP Address | IPアドレス | Alamat IP | IP 地址 |
| 操作结果 | 操作结果 | Operation Result | 操作結果 | Keputusan Operasi | 操作結果 |
| 请选择操作结果 | 请选择操作结果 | Select operation result | 操作結果を選択 | Pilih keputusan operasi | 請選擇操作結果 |
| 审计类型 | 审计类型 | Audit Type | 監査タイプ | Jenis Audit | 稽核類型 |
| 严重级别 | 严重级别 | Severity | 重大度 | Keterukan | 嚴重級別 |
| 请选择严重级别 | 请选择严重级别 | Select severity | 重大度を選択 | Pilih keterukan | 請選擇嚴重級別 |
| 主体类型 | 主体类型 | Subject Type | 主体タイプ | Jenis Subjek | 主體類型 |
| 主体ID | 主体ID | Subject ID | 主体ID | ID Subjek | 主體ID |
| 合规标签 | 合规标签 | Compliance Tag | コンプライアンスタグ | Tag Pematuhan | 合規標籤 |
| 服务名称 | 服务名称 | Service Name | サービス名 | Nama Perkhidmatan | 服務名稱 |
| 日志级别 | 日志级别 | Log Level | ログレベル | Tahap Log | 日誌級別 |
| 请选择日志级别 | 请选择日志级别 | Select log level | ログレベルを選択 | Pilih tahap log | 請選擇日誌級別 |
| 链路ID | 链路ID | Trace ID | トレースID | ID Jejak | 鏈路ID |
| 日志消息 | 日志消息 | Log Message | ログメッセージ | Mesej Log | 日誌訊息 |
| 信息 | 信息 | Info | 情報 | Maklumat | 資訊 |
| 警告 | 警告 | Warning | 警告 | Amaran | 警告 |
| 错误 | 错误 | Error | エラー | Ralat | 錯誤 |
| 严重 | 严重 | Critical | 重大 | Kritikal | 嚴重 |
| 成功 | 成功 | Success | 成功 | Berjaya | 成功 |
| 失败 | 失败 | Failed | 失敗 | Gagal | 失敗 |
| 租户ID | 租户ID | Tenant ID | テナントID | ID Penyewa | 租戶ID |
| 关闭 | 关闭 | Close | 閉じる | Tutup | 關閉 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`查看`、`操作`、`ID`、`状态`、`创建时间`、`全部`、`暂无数据`、`功能说明`、`操作指南`、`关键词`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"审计日志"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **DxTabPanel** 包含 3 个标签页：操作日志、审计日志、登录日志
- [ ] **Tab 1 表格列**包含：ID、租户ID、操作者类型、操作者ID、操作动作、资源类型、资源ID、IP 地址、操作结果、创建时间、操作
- [ ] **Tab 2 表格列**包含：ID、租户ID、审计类型、严重级别、主体类型、主体ID、合规标签、创建时间、操作
- [ ] **Tab 3 表格列**包含：ID、服务名称、日志级别、链路ID、日志消息、创建时间、操作
- [ ] **分页**包含：页大小选择（10/20/50/100）、总数显示、导航按钮
- [ ] 远程分页通过 CustomStore 实现
- [ ] **行操作**仅包含：查看（三个标签页通用）
- [ ] **无工具栏增删改按钮**（全部只读）
- [ ] **操作日志详情弹窗**展示所有字段
- [ ] **审计日志详情弹窗**展示所有字段
- [ ] **登录日志详情弹窗**展示所有字段

### P1 — 业务规则完整性

- [ ] 所有日志为只读，无创建/编辑/删除操作
- [ ] Tab 1 支持按关键词和操作结果筛选
- [ ] Tab 2 支持按关键词和严重级别筛选
- [ ] Tab 3 支持按关键词和日志级别筛选
- [ ] 操作结果列颜色区分（成功绿色 / 失败红色）
- [ ] 严重级别列颜色区分（信息蓝 / 警告黄 / 错误红 / 严重深红）
- [ ] 日志级别列颜色区分（信息蓝 / 警告黄 / 错误红）
- [ ] 登录日志消息列长文本截断显示
- [ ] 详情弹窗消息字段完整展示（pre-wrap）
- [ ] 每个操作按钮有权限码控制

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `grep -rn 'caption="' AuditView.vue | grep -v ':caption'` 结果为 0
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有查询字段 label / placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有状态/级别显示值已国际化
- [ ] 所有标签页标题已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
