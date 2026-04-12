# 租户平台 — 前端全面工程化重构总提示词

> **⚠️ 已归档 — 本文件已被新提示词体系取代**
>
> 本文件的内容已分别吸收进以下新文件：
> - dxdocs 工作流 → `03-frontend/04-devextreme-templates.md`
> - 工程化标准 → `03-frontend/00-governance.md`
> - 反模式清单 → `03-frontend/03-anti-patterns.md`
> - 模块清单 → `08-platform/frontend/0000_overview.md`
> - axios 规范 → `03-frontend/05-axios-standard.md`
> - i18n 规范 → `03-frontend/06-i18n-execution.md`
> - 业务模板 → `03-frontend/07-business-prompt-template.md`
>
> **新 Agent 请勿以本文件为执行依据。** 仅作为历史缺陷分析的参考文档保留。
> 新任务应从 `00-governance.md` 和 `0000_overview.md` 入口开始。

---

## 任务背景

后端已基于统一架构完整生成（阶段 A-D 全部完成，462 测试通过），前端已有初始实现（阶段 E1-E5 + F1），但当前前端代码仅为 Demo 级别，存在大量严重问题。本文档为前端系统性重构的总纲，所有子任务模块必须以此为标准执行。

---

## 前置阅读（强制）

- `.ai/rules/frontend.md` — 前端开发规范
- `.ai/rules/i18n.md` — 国际化规范
- `.ai/rules/naming.md` — 命名规范
- `.ai/context/tech-stack.md` — 技术栈约束
- `.ai/prompts/03-frontend/page-module.md` — 页面模块通用提示词
- `.ai/prompts/08-platform/frontend/refactoring-master.md` — 本文件
- `.github/copilot-instructions.md` — 关键编码约束内联摘要

---

## 零、DevExpress MCP Server（dxdocs）— 强制使用

本仓库已配置 DevExpress MCP Server（工具名：`dxdocs`），提供 `devexpress_docs_search` 和 `devexpress_docs_get_content` 两个工具。

### 官方 dxdocs 工作流（必须严格遵循）

对于**每一个** DevExtreme 组件相关的编码任务，必须按以下 4 步执行：

1. **调用 `devexpress_docs_search`** — 获取与当前组件/问题相关的帮助主题列表（每个问题仅调用一次，避免冗余查询）
2. **调用 `devexpress_docs_get_content`** — 获取并阅读最相关的帮助主题全文内容
3. **反思获取到的内容** — 分析文档内容与当前编码需求的关系，提取可用的 API、属性、代码示例
4. **基于检索到的信息编码** — 使用文档中的具体控件名称、属性名称和代码范例，禁止凭猜测

### dxdocs 使用约束

- 每个问题仅调用一次 `devexpress_docs_search`，避免冗余查询
- **必须基于从 dxdocs 获取的信息编码**，禁止凭记忆或猜测 DevExtreme API
- 如果文档中有相关代码示例，**必须参考这些代码示例**
- 必须引用文档中提到的**具体 DevExtreme 控件和属性名称**
- 调用时使用 `technologies: ["Vue"]` 限定 Vue 相关文档

### 强制调用场景

Agent 在以下场景**必须**先执行 dxdocs 工作流（search → get_content → reflect → code），再进行编码：

1. **首次使用任何 DevExtreme 组件时**（DxDataGrid、DxTreeView、DxForm、DxDrawer、DxPopup 等）
2. **遇到 DevExtreme 组件行为异常时**（如 floating label 与浏览器自动填充冲突、DxTreeView 选中偏移）
3. **配置 DevExtreme 组件的高级功能时**（远程分页 CustomStore、表单验证 validationRules、DxTreeList 树形选择等）
4. **处理 DevExtreme 组件的样式定制时**（CSS 变量、主题切换、DxDrawer 侧边栏布局）

### 调用示例

```
// 第 1 步：搜索相关帮助主题
devexpress_docs_search(technologies: ["Vue"], question: "DxForm label-mode floating vs static browser autofill")

// 第 2 步：获取最相关主题的全文（URL 从搜索结果中获取）
devexpress_docs_get_content(url: "https://docs.devexpress.com/...")

// 第 3 步：反思 — 分析文档中的 API 说明和代码示例
// 第 4 步：基于文档信息编码
```

更多搜索示例：

```
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeView selectByClick focusStateEnabled CSS customization")
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid CustomStore remote paging load function")
devexpress_docs_search(technologies: ["Vue"], question: "DxPopup content template slot usage")
```

### 禁止行为

- 禁止在不执行 dxdocs 工作流的情况下猜测 DevExtreme 组件的 API 和行为
- 禁止使用过时的 DevExtreme API（必须通过 dxdocs 获取最新信息）
- 当 DevExtreme 组件行为与预期不符时，必须先通过 dxdocs 查阅已知问题和最佳实践

---

## 一、现有前端全面缺陷分析

### 1. 国际化（i18n）严重缺陷

| 缺陷类别 | 问题描述 | 严重等级 |
|----------|---------|---------|
| 组件级语言文件完全缺失 | `.ai/rules/i18n.md` 要求每个 `.vue` 文件必须配套 `.vue.zh-CN.json` + `.vue.en-US.json` 等组件级语言文件，当前 37 个视图文件无一创建 | 🔴 致命 |
| 混用 i18n key 风格 | 部分使用嵌套结构 key（`$t('common.save')`），部分使用中文 key（`$t('平台用户管理')`），违反规范统一性要求 | 🔴 致命 |
| FunctionDescriptionCard 硬编码中文 | 所有页面的 FunctionDescriptionCard 组件 `purpose`、`data-scope`、`permission-note`、`risk-note` 属性均为硬编码中文字符串，未使用 `t()` | 🔴 致命 |
| OperationGuideDrawer 硬编码中文 | 所有页面的 `title`、`entry-path`、`steps`、`field-notes`、`error-notes` 均为硬编码中文数组，未使用 `t()` | 🔴 致命 |
| 枚举显示名翻译不完整 | 后端有 56 个枚举类型，前端仅在个别页面对少量枚举做了 computed 翻译（如 statusOptions），大量枚举缺少前端翻译映射 | 🟡 严重 |
| ms-MY/zh-TW 翻译不完整 | ms-MY.json（446 键）和 zh-TW.json（445 键）明显少于 zh-CN.json（599 键），翻译缺失 | 🟡 严重 |
| ja-JP 语言完全缺失 | `.ai/rules/i18n.md` 要求支持 5 种语言（含 ja-JP），当前仅实现 4 种 | 🟡 严重 |
| 状态标签翻译混乱 | StatusTag 组件直接渲染英文枚举值（如 "Active"），未进行 i18n 翻译 | 🟡 严重 |

### 2. 表单验证完全缺失

| 缺陷类别 | 问题描述 | 严重等级 |
|----------|---------|---------|
| DxForm 无任何验证规则 | 所有创建/编辑表单的 DxSimpleItem 均未配置 `validationRules`，必填字段无 required 验证、邮箱无格式验证、密码无长度验证 | 🔴 致命 |
| 唯一性实时检查完全缺失 | 后端已提供 `check-username-exists`、`check-code-exists` 等唯一性检查 API，前端表单未调用任何唯一性检查 | 🔴 致命 |
| 提交前无表单验证 | 所有 `handleCreate`/`handleEdit` 方法直接调用 API，未先执行 DxForm 的 `validate()` 方法 | 🔴 致命 |

### 3. 操作反馈与错误处理缺陷

| 缺陷类别 | 问题描述 | 严重等级 |
|----------|---------|---------|
| 操作成功无反馈 | 创建、编辑、删除、状态变更等操作成功后无任何弹窗提示（应使用 DevExtreme `notify` 或 `DxToast`） | 🔴 致命 |
| 删除无确认弹窗 | 所有删除操作直接调用 API，无二次确认弹窗（应使用 DevExtreme `confirm` 或 `DxPopup`） | 🔴 致命 |
| 状态变更无确认弹窗 | 启用/禁用/暂停/关闭等状态变更操作无二次确认 | 🟡 严重 |
| 错误提示不可见 | `http.ts` 中错误由 `handleApiError` 处理，但该函数仅做 localStorage 清理和重定向，不展示任何用户可见的错误消息 | 🔴 致命 |
| catch 块静默吞错 | 所有视图中 catch 块均为空或仅有注释 `// 错误由 http 层统一处理`，实际上 http 层未做 Toast 提示 | 🔴 致命 |

### 4. 业务功能残缺

| 缺陷类别 | 问题描述 | 严重等级 |
|----------|---------|---------|
| 分页未实现远程分页 | DxDataGrid 使用本地 `data-source` 数组，分页组件仅为客户端分页；loadData 固定请求 Page=1, PageSize=20 不响应分页切换 | 🔴 致命 |
| 详情页/详情弹窗完全缺失 | 所有模块均无 DetailView，点击行无法查看完整详情 | 🟡 严重 |
| 批量操作未实现 | 后端已提供 batch-enable/batch-disable API（平台用户），前端未实现批量选择与批量操作 | 🟡 严重 |
| 租户生命周期操作不完整 | 后端提供 initialize/suspend/resume/terminate/convert-trial 五个状态流转 API，前端仅实现了部分（激活/暂停/关闭）且调用错误的 API 路径 | 🟡 严重 |
| 角色权限绑定未完整实现 | PlatformRolesView 角色权限绑定弹窗虽有但可能未正确获取并展示当前已绑定权限 | 🟡 严重 |
| 角色成员管理未实现 | 后端有 `POST /api/platform-roles/{id}/members` 接口，前端未实现成员绑定功能 | 🟡 严重 |
| 套餐版本与能力管理缺失 | 后端有 package-versions 和 package-capabilities API，前端 PackageVersionsView 缺少能力管理入口 | 🟡 严重 |
| 订阅续费/升级未实现 | 后端有 renew/upgrade API，前端 SubscriptionsView 缺少续费和升级操作 | 🟡 严重 |
| 发票明细查看缺失 | 后端有 `GET /api/billings/{invoiceId}/items`，前端 InvoicesView 无发票明细查看入口 | 🟡 严重 |
| 文件访问策略管理缺失 | 后端有 file-access-policies API，前端无对应管理页面 | 🟡 严重 |
| 菜单排序功能缺失 | 后端有 `PUT /api/menus/{id}/sort`，前端菜单管理未实现排序 | 🟡 严重 |
| 安全策略页面为占位 | PlatformSecurityView 虽有但功能不完整 | 🟡 严重 |

### 5. 架构与组件规范缺陷

| 缺陷类别 | 问题描述 | 严重等级 |
|----------|---------|---------|
| 自定义 CSS 样式大量使用 | `global.css` 中 `.status-tag`、`.menu-item`、`.card` 等应使用 DevExtreme 组件替代，避免主题切换样式错乱 | 🟡 严重 |
| 原生 HTML 标签滥用 | `<h2>`、`<p>`、`<span>`、`<div class="card">` 等应替换为 DevExtreme 对应组件（如 `<DxBox>`、`<DxTileView>` 等） | 🟡 严重 |
| StatusTag 自定义组件 | 应使用 DevExtreme 内置 Badge/Tag 样式或 CSS class 机制，不应自定义 `.status-tag` CSS | 🟡 严重 |
| MainLayout 侧边栏手写 | 使用原生 HTML/CSS 实现侧边栏菜单，应改用 DevExtreme `DxDrawer` + `DxTreeView` 或 `DxList` | 🟡 严重 |
| 全局样式硬编码颜色 | `global.css` 中 `#1976d2`、`#1e1e2d`、`#f5f7fa` 等颜色硬编码，不随 DevExtreme 主题切换 | 🟡 严重 |
| 登录页按钮手写样式 | 登录页 toggle-btn、logout-btn 使用自定义 CSS，应使用 DxButton | 🟡 严重 |

### 6. 数据加载与异常状态缺陷

| 缺陷类别 | 问题描述 | 严重等级 |
|----------|---------|---------|
| 无加载状态指示 | 数据加载时无 loading 指示器（应使用 DxLoadPanel 或 DxDataGrid 的 loadPanel） | 🟡 严重 |
| 无空数据状态 | 列表为空时无"暂无数据"提示 | 🟡 一般 |
| 无错误重试机制 | 加载失败时无重试按钮 | 🟡 一般 |
| API 无竞态处理 | 快速切换筛选条件时可能出现请求竞态 | 🟡 一般 |

### 7. API 路径与后端不一致

| 前端 API 路径 | 后端实际路径 | 说明 |
|---------------|-------------|------|
| `/api/payments/orders` | `/api/payment-orders/` | 支付订单路径不匹配 |
| `/api/payments/refunds` | `/api/payment-refunds/` | 退款路径不匹配 |
| `/api/tenants/{id}/status` (changeTenantStatus) | 后端有 initialize/suspend/resume/terminate/convert-trial 等独立端点 | 状态变更应调用具体端点而非统一的 status 端点 |
| `/api/platform-operations/dashboard` 等 | 后端无此端点 | Dashboard 数据 API 未实现 |
| 部分 API 函数参数类型 | 后端 DTO 字段 | 可能存在类型不完全匹配 |

### 8. 权限控制不完整

| 缺陷类别 | 问题描述 | 严重等级 |
|----------|---------|---------|
| 部分操作按钮缺少权限控制 | 多个页面的编辑、删除按钮未用 `v-if="perm.has()"` 包裹 | 🟡 严重 |
| 无 403 禁止访问页面 | 路由守卫拦截后无专门的 403 页面展示 | 🟡 严重 |
| 权限码不完整 | permissions.ts 有 159 个权限码，但部分新增功能缺少对应权限码定义 | 🟡 一般 |

---

## 二、重构标准与工程化要求

### 2.1 i18n 规范（核心要求）

1. **每个 `.vue` 文件必须创建对应的组件级语言文件**：
   - `{Component}.vue.zh-CN.json`
   - `{Component}.vue.en-US.json`
   - `{Component}.vue.zh-TW.json`
   - `{Component}.vue.ms-MY.json`
   - `{Component}.vue.ja-JP.json`（新增语言支持）

2. **组件语言文件内容规范**：
   - 组件专有文案直接写翻译值
   - 通用文案（编辑、删除、保存等）值设为 `null`，由 `common/` 提供
   - 所有语言文件的 key 必须完全一致

3. **严禁在模板或脚本中使用硬编码中文**：
   - 所有用户可见文本必须使用 `t()` 或 `$t()`
   - 包括但不限于：页面标题、表单标签、按钮文本、提示文案、确认弹窗文案、操作指引文案、FunctionDescriptionCard 内容

4. **枚举翻译规范**：
   - 后端 56 个枚举类型的所有值必须在前端有完整翻译
   - 使用 `enum.{EnumType}.{Value}` 格式的 key
   - 示例：`t('enum.PlatformUserStatus.Active')` → "已启用"

5. **i18n key 风格统一为中文 key**：
   - 遵循 `.ai/rules/i18n.md` 规范，使用中文作为 key
   - 组件级文件中使用中文 key，如 `"平台用户管理": "Platform User Management"`
   - 但可保留 `common.xxx` / `enum.xxx` / `status.xxx` / `error.xxx` 的嵌套结构 key 用于全局资源

### 2.2 DevExtreme 组件化要求

1. **严禁使用原生 HTML 替代 DevExtreme 组件**：
   - `<button>` → `<DxButton>`
   - `<select>` → `<DxSelectBox>`
   - `<input>` → `<DxTextBox>` / `<DxNumberBox>` / `<DxDateBox>`
   - 状态标签 → DevExtreme CSS class 或自定义 DxTagBox 样式
   - 卡片容器 → 保留 `.card` 但颜色应使用 CSS 变量或 DevExtreme 主题变量
   - 确认弹窗 → `import { confirm } from 'devextreme/ui/dialog'`
   - Toast 通知 → `import notify from 'devextreme/ui/notify'`
   - 加载面板 → `<DxLoadPanel>`

2. **主题联动**：
   - 所有颜色必须使用 DevExtreme CSS 变量（`--dx-color-*`）或语义化 class
   - 禁止硬编码颜色值如 `#1976d2`
   - 侧边栏应使用 `<DxDrawer>` 组件
   - 菜单应使用 `<DxTreeView>` 或 `<DxList>`

### 2.3 表单验证要求

1. **必填字段验证**：所有标记为必填的字段必须配置 `{ type: 'required', message: t('xxx不能为空') }`
2. **格式验证**：邮箱字段配置 `{ type: 'email' }`，手机号配置正则验证
3. **长度验证**：用户名 2-50 字符，密码 6-128 字符，编码 1-100 字符等
4. **唯一性异步验证**：用户名、租户编码、角色编码、套餐编码等唯一字段配置 `{ type: 'async', validationCallback }` 调用后端 check-exists API
5. **提交前验证**：所有表单提交前必须调用 `formInstance.validate()`，验证不通过禁止提交

### 2.4 操作反馈要求

1. **操作成功**：使用 `notify({ message: t('操作成功'), type: 'success' }, 3000)` 显示成功 Toast
2. **操作失败**：使用 `notify({ message: errorMessage, type: 'error' }, 5000)` 显示后端返回的本地化错误信息
3. **删除确认**：使用 `confirm(t('确认删除'), t('确认'))` 弹出确认对话框，用户确认后才执行删除
4. **危险操作确认**：状态变更（禁用、暂停、关闭、终止等）使用 confirm 二次确认
5. **加载状态**：数据加载时显示 DxLoadPanel 或 DataGrid 内置 loadPanel

### 2.5 远程分页要求

1. **DxDataGrid 使用 CustomStore**：通过 DevExtreme CustomStore 实现远程分页、排序、筛选
2. **分页参数传递**：将 DataGrid 的 loadOptions 中的 skip/take 转换为 Page/PageSize 参数
3. **总数响应**：CustomStore 的 load 方法返回 `{ data: items, totalCount: total }`
4. **筛选联动**：搜索关键词和状态筛选变化时自动刷新 DataGrid

### 2.6 错误处理增强

1. **http.ts 统一错误 Toast**：在 `request` 函数中，API 返回非 0 Code 时自动显示 `notify` 错误 Toast
2. **视图层错误处理**：catch 块中可添加额外的视图级处理（如关闭弹窗），但不应静默吞错
3. **网络错误处理**：fetch 失败时显示"网络连接失败，请检查网络"等提示
4. **超时处理**：可配置请求超时时间

### 2.7 安全与权限要求

1. **按钮级权限控制**：所有 CRUD 操作按钮必须使用 `v-if="perm.has(PERMISSION_CODE)"` 控制
2. **403 页面**：创建 ForbiddenView 页面，路由守卫拦截无权限访问时展示
3. **Token 过期处理**：401 响应自动跳转登录页并清理本地存储

---

## 三、模块拆分与子任务清单

### 子任务 G0：基础设施重构（全局影响）

**范围**：
- i18n 架构升级（添加 ja-JP 语言支持、组件级语言文件加载机制）
- `locales/index.ts` 重构（支持组件级语言文件的按需加载）
- `utils/http.ts` 增强（统一错误 Toast 通知、网络错误处理）
- `utils/errorHandler.ts` 增强（集成 DevExtreme notify）
- `global.css` 重构（移除硬编码颜色，使用 DevExtreme 主题变量）
- 创建 403 ForbiddenView 页面
- 补齐 `locales/common/` 和 `locales/generated/` 的 ja-JP 语言文件
- 补齐 ms-MY.json 和 zh-TW.json 缺失的翻译 key

### 子任务 G1：布局与导航重构

**范围**：
- `MainLayout.vue` — 使用 `DxDrawer` + `DxTreeView` 替代手写侧边栏
- `LoginView.vue` — 使用全 DevExtreme 组件、添加表单验证、增强错误处理
- `DashboardView.vue` — 使用 DxDataGrid/DxChart 统一渲染、添加加载状态

### 子任务 G2：平台管理模块（用户/角色/权限/安全）

**范围**：
- `PlatformUsersView.vue` — 远程分页、表单验证、唯一性检查、操作确认、批量操作、详情弹窗
- `PlatformRolesView.vue` — 远程分页、表单验证、权限绑定完善、成员绑定、操作确认
- `PlatformPermissionsView.vue` — 权限树完善、CRUD 功能
- `PlatformSecurityView.vue` — 安全策略管理完善
- 每个组件创建 5 个语言文件

### 子任务 G3：租户生命周期模块

**范围**：
- `TenantsView.vue` — 远程分页、完整生命周期操作（initialize/suspend/resume/terminate/convert-trial）、详情弹窗、表单验证
- `TenantGroupsView.vue` — 远程分页、表单验证、操作确认
- `TenantDomainsView.vue` — 远程分页、操作确认、域名格式验证
- `TenantTagsView.vue` — 远程分页、操作确认、租户标签绑定
- 每个组件创建 5 个语言文件

### 子任务 G4：租户配置模块

**范围**：
- `TenantSystemConfigView.vue` — 配置管理完善、编辑确认
- `TenantFeatureFlagsView.vue` — 功能开关管理、启用/禁用操作
- `TenantParametersView.vue` — 参数管理、CRUD 完善
- 每个组件创建 5 个语言文件

### 子任务 G5：SaaS 套餐与订阅模块

**范围**：
- `PackagesView.vue` — 远程分页、发布/下架操作、状态流转
- `PackageVersionsView.vue` — 版本管理、能力管理入口
- `SubscriptionsView.vue` — 远程分页、续费/升级操作
- `TrialsView.vue` — 试用管理
- 每个组件创建 5 个语言文件

### 子任务 G6：计费与支付模块

**范围**：
- `InvoicesView.vue` — 远程分页、发票明细查看、支付标记、作废操作
- `PaymentsView.vue` — 远程分页、API 路径修正
- `RefundsView.vue` — 远程分页、API 路径修正
- 每个组件创建 5 个语言文件

### 子任务 G7：API 集成与 Webhook 模块

**范围**：
- `ApiKeysView.vue` — 远程分页、操作确认、详情查看
- `WebhooksView.vue` — 远程分页、操作确认、启用/禁用
- `WebhookDeliveryLogsView.vue` — 远程分页、日志详情查看
- 每个组件创建 5 个语言文件

### 子任务 G8：运营与日志模块

**范围**：
- `OperationsView.vue` — 运营统计展示
- `OperationLogsView.vue` — 远程分页、日志详情
- `AuditLogsView.vue` — 远程分页、审计详情
- `SystemLogsView.vue` — 远程分页、日志详情
- 每个组件创建 5 个语言文件

### 子任务 G9：通知与存储模块

**范围**：
- `NotificationTemplatesView.vue` — 远程分页、模板创建/编辑、启用/禁用
- `NotificationsView.vue` — 远程分页、标记已读
- `StorageStrategiesView.vue` — 远程分页、策略管理
- `TenantFilesView.vue` — 远程分页、文件删除、访问策略管理
- 每个组件创建 5 个语言文件

### 子任务 G10：基础设施与系统设置模块

**范围**：
- `InfrastructureView.vue` — 基础设施组件管理
- `RateLimitPoliciesView.vue` — 限流策略管理
- `SystemMenusView.vue` — 菜单树管理、排序功能
- `SystemDictionariesView.vue` — 字典类型与项管理
- 每个组件创建 5 个语言文件

---

## 四、单模块验收标准模板

每个子任务完成后必须输出如下结构化验证清单：

```markdown
## 模块验收清单：{模块名称}

### 1. 功能完整性
| 功能点 | 完成状态 | 说明 |
|--------|---------|------|
| 列表展示 | ✅/❌ | |
| 远程分页 | ✅/❌ | |
| 关键词搜索 | ✅/❌ | |
| 状态筛选 | ✅/❌ | |
| 创建功能 | ✅/❌ | |
| 编辑功能 | ✅/❌ | |
| 删除功能 | ✅/❌ | |
| 详情查看 | ✅/❌ | |
| 状态变更 | ✅/❌ | |
| 批量操作 | ✅/❌（如适用）| |

### 2. 接口调用完整性
| API 端点 | 前端是否调用 | 调用位置 |
|----------|-------------|---------|
| GET /api/xxx | ✅/❌ | loadData |
| POST /api/xxx | ✅/❌ | handleCreate |
| PUT /api/xxx/{id} | ✅/❌ | handleEdit |
| DELETE /api/xxx/{id} | ✅/❌ | handleDelete |

### 3. 表单校验完整性
| 字段 | 必填 | 格式校验 | 长度限制 | 唯一性检查 |
|------|------|---------|---------|-----------|
| 用户名 | ✅/❌ | ✅/❌/N/A | ✅/❌/N/A | ✅/❌/N/A |

### 4. 国际化完整性
| 检查项 | 状态 |
|--------|------|
| 组件级语言文件已创建（5 个语言） | ✅/❌ |
| 页面标题使用 t() | ✅/❌ |
| 表单标签使用 t() | ✅/❌ |
| 操作按钮使用 t() | ✅/❌ |
| 提示文案使用 t() | ✅/❌ |
| 确认弹窗文案使用 t() | ✅/❌ |
| FunctionDescriptionCard 内容使用 t() | ✅/❌ |
| OperationGuideDrawer 内容使用 t() | ✅/❌ |
| 枚举值显示已翻译 | ✅/❌ |
| 无硬编码中文 | ✅/❌ |

### 5. 操作反馈
| 操作 | 成功提示 | 失败提示 | 确认弹窗 |
|------|---------|---------|---------|
| 创建 | ✅/❌ | ✅/❌ | N/A |
| 编辑 | ✅/❌ | ✅/❌ | N/A |
| 删除 | ✅/❌ | ✅/❌ | ✅/❌ |
| 状态变更 | ✅/❌ | ✅/❌ | ✅/❌ |

### 6. 界面与交互规范
| 检查项 | 状态 |
|--------|------|
| 全部使用 DevExtreme 组件 | ✅/❌ |
| 无原生 HTML 替代组件 | ✅/❌ |
| 权限控制到按钮级 | ✅/❌ |
| 加载状态指示 | ✅/❌ |
| 空数据状态处理 | ✅/❌ |

### 7. 编译验证
| 检查项 | 状态 |
|--------|------|
| npm run build 通过 | ✅/❌ |
| TypeScript 无类型错误 | ✅/❌ |
| 无 ESLint 警告 | ✅/❌ |
```

---

## 五、执行顺序与依赖

```
G0（基础设施）
  ↓
G1（布局导航）
  ↓
G2（平台管理） → G3（租户生命周期） → G4（租户配置）
                                        ↓
G5（套餐订阅） → G6（计费支付）
  ↓
G7（API 集成） → G8（运营日志） → G9（通知存储） → G10（基础设施系统设置）
```

每个子任务完成后必须执行 `npm run build` 验证通过后才可进入下一个模块。

---

## 六、禁止事项

1. 禁止在组件中使用硬编码中文文本
2. 禁止省略表单验证规则
3. 禁止省略操作确认弹窗
4. 禁止省略操作成功/失败反馈
5. 禁止使用原生 HTML 组件替代 DevExtreme 组件
6. 禁止使用硬编码颜色值
7. 禁止在 catch 块中静默吞错
8. 禁止预留未实现逻辑或占位代码
9. 禁止生成 TODO/FIXME 注释
10. 禁止使用 Element Plus、Ant Design 或其他非 DevExtreme 组件库
11. 禁止使用 jQuery
12. 禁止添加新的 npm 依赖（除非绝对必要且经确认）

---

## 七、后端枚举完整映射清单

以下 56 个枚举类型需在前端完整映射翻译（每个枚举值需在各语言文件中有对应翻译）：

1. ActiveDisabledStatus
2. AuditSeverity
3. BillingCycle
4. DataIsolationType
5. DataJobStatus
6. DataJobType
7. DomainType
8. DomainVerificationStatus
9. EditionType
10. FeatureFlagRolloutType
11. FileAccessSubjectType
12. FilePermissionCode
13. FileUploaderType
14. FileVisibility
15. InfrastructureComponentStatus
16. InfrastructureComponentType
17. InitializationTaskStatus
18. InitializationTaskType
19. InvoiceItemType
20. InvoiceStatus
21. IpWhitelistSubjectType
22. LifecycleEventType
23. LoginStatus
24. LoginType
25. MenuType
26. MfaProviderType
27. MfaSettingStatus
28. MonitorMetricType
29. NotificationChannel
30. NotificationSendStatus
31. OperationResult
32. OperatorType
33. ParamType
34. PaymentChannel
35. PaymentStatus
36. PermissionType
37. PlatformRoleStatus
38. PlatformUserStatus
39. QuotaResetCycle
40. QuotaType
41. RateLimitSubjectType
42. RefundStatus
43. SaasPackageStatus
44. StorageProviderType
45. SubscriptionChangeType
46. SubscriptionStatus
47. SubscriptionType
48. SystemLogLevel
49. TagType
50. TenantApiKeyStatus
51. TenantIsolationMode
52. TenantLifecycleStatus
53. TenantSourceType
54. TrialStatus
55. WebhookDeliveryStatus
56. SubscriptionChangeType

---

## 八、质量检查清单（每模块完成后 Agent 强制自检）

### 8.1 编译验证
```bash
cd web/tenant-platform-web && npm run build
```

### 8.2 硬编码中文检测（caption 属性）
```bash
# 搜索 .vue 文件中硬编码的 caption 属性（未使用 $t()）
grep -rn 'caption="' web/tenant-platform-web/src/ --include="*.vue" | grep -v ':caption'
# 结果必须为 0（所有 caption 必须使用 :caption="$t()"）
```

### 8.3 notifySuccess / confirmAction 参数检查
```bash
# 检查是否有双重 t() 调用
grep -rn "notifySuccess(t(" web/tenant-platform-web/src/ --include="*.vue"
grep -rn "confirmAction(t(" web/tenant-platform-web/src/ --include="*.vue"
grep -rn "confirmDelete(t(" web/tenant-platform-web/src/ --include="*.vue"
# 结果必须为 0（useNotify 内部已有 t()，调用方仅传 key）
```

### 8.4 表单验证规则存在性
```bash
grep -rn "validationRules\|validation-rules" web/tenant-platform-web/src/ --include="*.vue"
# 每个包含 DxForm 的视图必须有 validationRules 定义
```

### 8.5 权限控制存在性
```bash
grep -rn "perm.has" web/tenant-platform-web/src/ --include="*.vue"
# 所有 CRUD 操作按钮必须有权限控制
```

### 8.6 语言文件完整性
```bash
# 检查每个 .vue 文件是否有 5 个对应的语言文件
for f in $(find web/tenant-platform-web/src/views -name "*.vue" -not -name "PlaceholderView.vue"); do
  for lang in zh-CN en-US ja-JP ms-MY zh-TW; do
    [ ! -f "${f}.${lang}.json" ] && echo "MISSING: ${f}.${lang}.json"
  done
done
# 结果必须为空
```

### 8.7 语言文件 key 一致性
```bash
# 对比每个视图的 zh-CN 和 en-US 的 key 是否一致
for f in $(find web/tenant-platform-web/src/views -name "*.vue.zh-CN.json"); do
  en="${f/zh-CN/en-US}"
  if [ -f "$en" ]; then
    diff <(python3 -c "import json; print(sorted(json.load(open('$f')).keys()))" 2>/dev/null) \
         <(python3 -c "import json; print(sorted(json.load(open('$en')).keys()))" 2>/dev/null) \
    && echo "OK: $f" || echo "MISMATCH: $f vs $en"
  fi
done
```

### 8.8 操作反馈检查
```bash
grep -rn "confirm\|notify" web/tenant-platform-web/src/ --include="*.vue"
# 确认所有 CRUD 操作有反馈：创建/编辑成功 → notifySuccess，删除 → confirmDelete，危险操作 → confirmAction
```

### 8.9 DxDataGrid 远程分页检查
```bash
grep -rn "CustomStore" web/tenant-platform-web/src/ --include="*.vue"
# 每个使用 DxDataGrid 的列表页必须使用 CustomStore 实现远程分页
```

**如发现任何违规项，必须立即修复，然后重新执行 8.1 编译验证。**

---

## 九、可执行代码模板（Agent 必须参照）

### 9.1 DxDataGrid 列表页标准模板

```vue
<DxDataGrid
  ref="gridRef"
  :data-source="gridStore"
  :show-borders="true"
  :column-auto-width="true"
  :hover-state-enabled="true"
  :remote-operations="true"
  key-expr="Id"
>
  <!-- ✅ 所有 caption 必须使用 :caption="$t()" -->
  <DxColumn data-field="Id" :caption="$t('ID')" :width="80" :allow-sorting="false" />
  <DxColumn data-field="Code" :caption="$t('编码')" />
  <DxColumn data-field="Name" :caption="$t('名称')" />
  <DxColumn data-field="Status" :caption="$t('状态')" cell-template="statusCell" :width="100" />
  <DxColumn :caption="$t('操作')" cell-template="actionCell" :width="280" :allow-sorting="false" />
  <DxPaging :page-size="20" />
  <DxPager :show-page-size-selector="true" :allowed-page-sizes="[10, 20, 50]" :show-info="true" />
</DxDataGrid>
```

### 9.2 CustomStore 远程分页标准模板

```typescript
const gridStore = new CustomStore({
  key: 'Id',
  load: async (loadOptions) => {
    const page = ((loadOptions.skip ?? 0) / (loadOptions.take ?? 20)) + 1
    const pageSize = loadOptions.take ?? 20
    const res = await getXxxList({
      Page: page,
      PageSize: pageSize,
      Keyword: filterKeyword.value || undefined,
      Status: filterStatus.value || undefined,
    })
    return {
      data: res.Data!.Items,
      totalCount: res.Data!.Total,
    }
  },
})
```

### 9.3 操作反馈标准调用方式

```typescript
// ✅ 正确 — notifySuccess / confirmAction / confirmDelete 仅传 i18n key
// useNotify.ts 内部会调用 t() 翻译
notifySuccess('创建成功')
notifySuccess('更新成功')
notifySuccess('删除成功')
notifySuccess('启用成功')
notifySuccess('禁用成功')
notifySuccess('保存成功')

const confirmed = await confirmDelete(item.Name)
const confirmed = await confirmAction('确认启用此角色')
const confirmed = await confirmAction('确认禁用此用户')

// ❌ 错误 — 禁止双重 t()
notifySuccess(t('创建成功'))  // 双重翻译
confirmAction(t('确认启用'))  // 双重翻译
```

### 9.4 DxForm 表单验证标准模板

```typescript
const codeRules = computed(() => [
  { type: 'required' as const, message: t('编码不能为空') },
  { type: 'stringLength' as const, min: 1, max: 100, message: t('编码最长100字') },
  {
    type: 'async' as const,
    validationCallback: async (params: { value?: string }) => {
      if (!params.value) return true
      const res = await checkCodeExists(params.value)
      if (res.Data === true) throw new Error(t('编码已存在'))
      return true
    },
  },
])

// 提交前必须验证
async function handleSubmit() {
  const formInstance = formRef.value?.instance
  if (formInstance) {
    const result = formInstance.validate()
    if (!result.isValid) return
  }
  // ... 调用 API
}
```

### 9.5 登录页 label-mode 规范

```vue
<!-- ✅ 正确 — 使用 static 避免浏览器自动填充时 floating label 与值重叠 -->
<DxForm
  ref="formRef"
  :form-data="formData"
  :col-count="1"
  label-mode="static"
  :disabled="isLoading"
>
```

---

## 十、Mock 数据完整性要求

每个模块的 Mock handler 必须满足以下要求：

1. **至少返回 3 条以上合理测试数据**，覆盖不同状态（如 Active、Disabled）
2. **数据结构与后端 DTO 完全一致**（PascalCase 字段名）
3. **权限树 Mock 必须包含至少 3 级层级**，每级至少 2 个节点
4. **分页 API Mock 必须正确响应 Page 和 PageSize 参数**
5. **唯一性检查 API Mock 必须根据参数返回合理的 true/false**
