# 租户平台 — 租户配置管理页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块包含三个子模块：系统配置、功能开关、租户参数。建议使用 Tab 页切换。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-8 |
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
- `.ai/prompts/08-platform/backend/tenant-config-api.md` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxTabPanel | `DxTabPanel items selected-index title template slot` | 三子模块 Tab 切换 |
| DxForm | `DxForm validation rules required custom group items` | 系统配置/参数编辑表单 |
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take` | 功能开关列表、参数列表 |
| DxSwitch | `DxSwitch value on-value-changed` | 功能开关开关切换 |
| DxPopup | `DxPopup content template slot visible showing hiding event` | 编辑弹窗 |
| DxTextBox | `DxTextBox placeholder value-changed mode` | 表单输入 |
| DxSelectBox | `DxSelectBox data-source display-expr value-expr placeholder` | 租户选择、参数类型选择 |
| DxToolbar | `DxToolbar items location widget DxButton` | 工具栏 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `TenantConfigEndpoints.cs` 中的路由注册。

### 系统配置

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 全部配置 | GET | `/api/system-configs` | - | `ApiResult<List<TenantSystemConfigRepDTO>>` |
| 按 key 获取 | GET | `/api/system-configs/{key}` | - | `ApiResult<TenantSystemConfigRepDTO>` |
| 更新配置 | PUT | `/api/system-configs/{key}` | `UpdateTenantSystemConfigReqDTO` | `ApiResult` |

### 功能开关

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 开关列表 | GET | `/api/feature-flags?page=1&pageSize=20&keyword=&tenantRefId=` | - | `ApiResult<List<TenantFeatureFlagRepDTO>>` |
| 按 key 获取 | GET | `/api/feature-flags/{key}` | - | `ApiResult<TenantFeatureFlagRepDTO>` |
| 更新开关 | PUT | `/api/feature-flags/{key}` | `SaveTenantFeatureFlagReqDTO` | `ApiResult` |
| 启用 | PUT | `/api/feature-flags/{key}/enable` | - | `ApiResult` |
| 禁用 | PUT | `/api/feature-flags/{key}/disable` | - | `ApiResult` |

### 租户参数

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 参数列表 | GET | `/api/tenant-parameters?page=1&pageSize=20&keyword=&tenantRefId=` | - | `ApiResult<PagedResult<TenantParameterRepDTO>>` |
| 创建/更新参数 | POST | `/api/tenant-parameters` | `SaveTenantParameterReqDTO` | `ApiResult` |
| 删除参数 | DELETE | `/api/tenant-parameters/{id}` | - | `ApiResult` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/tenant-config/TenantConfigView.vue` | 主页面（Tab 容器） |
| 2 | `src/WebTenantPlatfrom/src/views/tenant-config/TenantConfigView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/tenant-config/TenantConfigView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/tenant-config/TenantConfigView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/tenant-config/TenantConfigView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/tenant-config/TenantConfigView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/tenant-config.ts` | API 封装（配置 + 开关 + 参数） |
| 8 | `src/WebTenantPlatfrom/src/types/tenant-config.ts` | 类型定义 |
| 9 | `src/WebTenantPlatfrom/src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/WebTenantPlatfrom/src/constants/permissions.ts`（追加） | 权限码 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('租户配置管理')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('管理系统配置、功能开关和租户参数')` | 页面说明 |
| 功能说明区 | `FunctionDescriptionCard` | 说明本页面提供的核心能力 |
| Tab 容器 | `DxTabPanel` | 三个 Tab：系统配置、功能开关、租户参数 |
| Tab 1 — 系统配置 | `DxForm`（表单模式） | 系统级配置编辑 |
| Tab 2 — 功能开关 | `DxDataGrid` + `DxSwitch` | 功能开关列表与快速切换 |
| Tab 3 — 租户参数 | `DxDataGrid` + 新增/编辑弹窗 | 参数键值对管理 |
| 操作指南 | `OperationGuideDrawer` | 操作步骤说明 |

---

## Tab 1：系统配置

### 配置表单

**组件**：`DxForm`（直接编辑模式，非弹窗）

- 调用 `GET /api/system-configs` 获取全部配置项
- 以表单形式展示和编辑配置

**配置字段**：

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 默认值 |
|:----:|--------|------|------|:----:|---------|--------|
| 1 | SystemName | `$t('系统名称')` | DxTextBox | 否 | 0-100 | 从配置加载 |
| 2 | LogoUrl | `$t('Logo URL')` | DxTextBox | 否 | 0-500 | 从配置加载 |
| 3 | SystemTheme | `$t('系统主题')` | DxSelectBox | 否 | - | 从配置加载 |
| 4 | DefaultLanguage | `$t('默认语言')` | DxSelectBox | 否 | - | 从配置加载 |
| 5 | DefaultTimezone | `$t('默认时区')` | DxSelectBox | 否 | - | 从配置加载 |

**系统主题选项**：

| 值 | 显示文本 |
|:--:|---------|
| light | `$t('浅色主题')` |
| dark | `$t('深色主题')` |

**默认语言选项**：同租户管理页面。

**验证规则**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| SystemName | stringLength | max: 100 | `$t('系统名称长度不超过 100 个字符')` |
| LogoUrl | stringLength | max: 500 | `$t('Logo URL 长度不超过 500 个字符')` |

**保存行为**：

- 底部放置保存按钮 `$t('保存配置')`
- 点击后逐个调用 `PUT /api/system-configs/{key}` 更新各配置项
- 保存成功：`notifySuccess('保存成功')`
- 保存失败：axios 拦截器自动显示错误

---

## Tab 2：功能开关

### 租户选择器

在功能开关 Tab 顶部提供可选的 `DxSelectBox` 用于筛选特定租户的功能开关：
- placeholder：`$t('全局（不选择租户）')`
- 不选择时显示全局开关

### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 格式化 | 说明 |
|:----:|-----------|---------------------|:----:|--------|------|
| 1 | FeatureKey | `$t('功能键')` | 200px | - | |
| 2 | FeatureName | `$t('功能名称')` | auto | - | |
| 3 | Enabled | `$t('状态')` | 120px | `switchCell` | DxSwitch 快速切换 |
| 4 | RolloutType | `$t('发布类型')` | 120px | `rolloutCell` | |
| 5 | UpdatedAt | `$t('更新时间')` | 180px | `dateCell` | `yyyy-MM-dd HH:mm` |
| 6 | - | `$t('操作')` | 120px | `actionCell` | 编辑 |

### DxSwitch 行内切换

- 列表中 Enabled 列直接使用 `DxSwitch` 组件
- 切换时调用 `PUT /api/feature-flags/{key}/enable` 或 `/disable`
- 切换前弹出确认：`confirmAction('确认{action}功能 {name}')`
- 切换成功：`notifySuccess('操作成功')` → 刷新列表
- 切换失败：回滚开关状态

### 发布类型标签

| 类型值 | 显示文本 | 颜色 |
|:------:|---------|------|
| all | `$t('全部用户')` | 绿色 `#52c41a` |
| percentage | `$t('百分比')` | 蓝色 `#1890ff` |
| whitelist | `$t('白名单')` | 黄色 `#faad14` |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 | 确认框 |
|------|------|------|--------|---------|--------|
| 编辑 | `$t('编辑')` | `edit` | `TENANT_CONFIG_UPDATE` | 打开编辑开关弹窗 | 无 |

### 编辑功能开关表单

**标题**：`$t('编辑功能开关')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 默认值 | 禁用条件 |
|:----:|--------|------|------|:----:|--------|---------|
| 1 | FeatureKey | `$t('功能键')` | DxTextBox | - | 从数据加载 | **始终禁用** |
| 2 | FeatureName | `$t('功能名称')` | DxTextBox | 是 | 从数据加载 | - |
| 3 | Enabled | `$t('是否启用')` | DxSwitch | - | 从数据加载 | - |
| 4 | RolloutType | `$t('发布类型')` | DxSelectBox | 是 | 从数据加载 | - |

**验证规则**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| FeatureName | required | - | `$t('请输入功能名称')` |

---

## Tab 3：租户参数

### 租户选择器

在参数 Tab 顶部提供 `DxSelectBox` 用于选择目标租户：
- placeholder：`$t('请选择租户')`
- 选择后加载该租户的参数列表

### 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | `$t('关键词')` | DxTextBox | `''` | `$t('请输入参数关键词')` |

### 列定义

| 序号 | data-field | caption（i18n key） | 宽度 | 可排序 | 说明 |
|:----:|-----------|---------------------|:----:|:------:|------|
| 1 | Id | `$t('ID')` | 80px | 否 | |
| 2 | ParamKey | `$t('参数键')` | 150px | 是 | |
| 3 | ParamName | `$t('参数名称')` | auto | 是 | |
| 4 | ParamType | `$t('参数类型')` | 100px | 否 | |
| 5 | ParamValue | `$t('参数值')` | auto | 否 | |
| 6 | UpdatedAt | `$t('更新时间')` | 180px | 是 | `yyyy-MM-dd HH:mm` |
| 7 | - | `$t('操作')` | 200px | 否 | 编辑、删除 |

### 分页配置

| 配置 | 值 |
|------|---|
| 支持分页 | 是 |
| 默认页大小 | 20 |
| 可选页大小 | `[10, 20, 50, 100]` |
| 远程分页 | 是（CustomStore） |

### 参数类型标签

| 类型值 | 显示文本 |
|:------:|---------|
| string | `$t('字符串')` |
| number | `$t('数字')` |
| boolean | `$t('布尔值')` |
| json | `$t('JSON')` |

### 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 |
|------|------|------|--------|---------|---------|
| 新增参数 | `$t('新增参数')` | `add` | `TENANT_CONFIG_UPDATE` | 已选择租户 | 打开新增参数弹窗 |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 | 确认框 |
|------|------|------|--------|---------|--------|
| 编辑 | `$t('编辑')` | `edit` | `TENANT_CONFIG_UPDATE` | 打开编辑参数弹窗 | 无 |
| 删除 | `$t('删除')` | `trash` | `TENANT_CONFIG_DELETE` | 调用删除 API | `confirmDelete(row.ParamName)` |

### 新增/编辑参数表单

**标题**：新增 `$t('新增参数')` / 编辑 `$t('编辑参数')`
**组件**：`DxPopup`（`width: 600`，`height: auto`）+ `DxForm`

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 默认值 | 禁用条件 |
|:----:|--------|------|------|:----:|---------|--------|---------|
| 1 | ParamKey | `$t('参数键')` | DxTextBox | 是 | 2-50 | `''` | 编辑时禁用 |
| 2 | ParamName | `$t('参数名称')` | DxTextBox | 是 | 2-100 | `''` | - |
| 3 | ParamType | `$t('参数类型')` | DxSelectBox | 是 | - | `'string'` | - |
| 4 | ParamValue | `$t('参数值')` | DxTextBox / DxTextArea | 是 | 1-2000 | `''` | - |

**验证规则**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| ParamKey | required | - | `$t('请输入参数键')` |
| ParamKey | stringLength | min: 2, max: 50 | `$t('参数键长度 2-50 个字符')` |
| ParamName | required | - | `$t('请输入参数名称')` |
| ParamName | stringLength | min: 2, max: 100 | `$t('参数名称长度 2-100 个字符')` |
| ParamValue | required | - | `$t('请输入参数值')` |
| ParamValue | stringLength | min: 1, max: 2000 | `$t('参数值长度 1-2000 个字符')` |

---

## 权限码定义

```typescript
// src/constants/permissions.ts（追加）
export const TENANT_CONFIG_VIEW = 'tenant.detail'
export const TENANT_CONFIG_UPDATE = 'tenant.update'
export const TENANT_CONFIG_DELETE = 'tenant.delete'
```

---

## 成功提示（全部国际化）

| 操作 | 调用 |
|------|------|
| 保存系统配置 | `notifySuccess('保存成功')` |
| 更新功能开关 | `notifySuccess('保存成功')` |
| 启用/禁用功能 | `notifySuccess('操作成功')` |
| 创建参数 | `notifySuccess('创建成功')` |
| 更新参数 | `notifySuccess('更新成功')` |
| 删除参数 | `notifySuccess('删除成功')` |

**注意**：`notifySuccess` 仅传 i18n key，不用 `t()` 包裹。

---

## 类型定义

```typescript
// src/types/tenant-config.ts

/** 系统配置响应 */
export interface TenantSystemConfigRepDTO {
  Id: number
  TenantRefId: number
  SystemName?: string
  LogoUrl?: string
  SystemTheme?: string
  DefaultLanguage?: string
  DefaultTimezone?: string
  UpdatedAt: string
}

/** 更新系统配置请求 */
export interface UpdateTenantSystemConfigReqDTO {
  SystemName?: string
  LogoUrl?: string
  SystemTheme?: string
  DefaultLanguage?: string
  DefaultTimezone?: string
}

/** 功能开关响应 */
export interface TenantFeatureFlagRepDTO {
  Id: number
  TenantRefId: number
  FeatureKey: string
  FeatureName: string
  Enabled: boolean
  RolloutType: string
  UpdatedAt: string
}

/** 保存功能开关请求 */
export interface SaveTenantFeatureFlagReqDTO {
  TenantRefId: number
  FeatureKey: string
  FeatureName: string
  Enabled: boolean
  RolloutType: string
}

/** 租户参数响应 */
export interface TenantParameterRepDTO {
  Id: number
  TenantRefId: number
  ParamKey: string
  ParamName: string
  ParamType: string
  ParamValue: string
  UpdatedAt: string
}

/** 保存参数请求 */
export interface SaveTenantParameterReqDTO {
  TenantRefId: number
  ParamKey: string
  ParamName: string
  ParamType: string
  ParamValue: string
}
```

---

## 国际化要求

### 组件级 key（放入 `TenantConfigView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 租户配置管理 | 租户配置管理 | Tenant Configuration | テナント設定管理 | Pengurusan Konfigurasi Penyewa | 租戶配置管理 |
| 管理系统配置、功能开关和租户参数 | 管理系统配置、功能开关和租户参数 | Manage system configurations, feature flags, and tenant parameters | システム設定、機能フラグ、テナントパラメータを管理 | Urus konfigurasi sistem, suis ciri dan parameter penyewa | 管理系統配置、功能開關和租戶參數 |
| 系统配置 | 系统配置 | System Configuration | システム設定 | Konfigurasi Sistem | 系統配置 |
| 功能开关 | 功能开关 | Feature Flags | 機能フラグ | Suis Ciri | 功能開關 |
| 租户参数 | 租户参数 | Tenant Parameters | テナントパラメータ | Parameter Penyewa | 租戶參數 |
| 系统名称 | 系统名称 | System Name | システム名 | Nama Sistem | 系統名稱 |
| Logo URL | Logo URL | Logo URL | Logo URL | Logo URL | Logo URL |
| 系统主题 | 系统主题 | System Theme | システムテーマ | Tema Sistem | 系統主題 |
| 浅色主题 | 浅色主题 | Light Theme | ライトテーマ | Tema Cerah | 淺色主題 |
| 深色主题 | 深色主题 | Dark Theme | ダークテーマ | Tema Gelap | 深色主題 |
| 保存配置 | 保存配置 | Save Configuration | 設定を保存 | Simpan Konfigurasi | 儲存配置 |
| 系统名称长度不超过 100 个字符 | 系统名称长度不超过 100 个字符 | System name must not exceed 100 characters | システム名は100文字以内 | Nama sistem mestilah tidak melebihi 100 aksara | 系統名稱長度不超過 100 個字元 |
| Logo URL 长度不超过 500 个字符 | Logo URL 长度不超过 500 个字符 | Logo URL must not exceed 500 characters | Logo URLは500文字以内 | Logo URL mestilah tidak melebihi 500 aksara | Logo URL 長度不超過 500 個字元 |
| 功能键 | 功能键 | Feature Key | 機能キー | Kunci Ciri | 功能鍵 |
| 功能名称 | 功能名称 | Feature Name | 機能名 | Nama Ciri | 功能名稱 |
| 发布类型 | 发布类型 | Rollout Type | ロールアウトタイプ | Jenis Pelancaran | 發佈類型 |
| 全部用户 | 全部用户 | All Users | 全ユーザー | Semua Pengguna | 全部使用者 |
| 百分比 | 百分比 | Percentage | パーセンテージ | Peratusan | 百分比 |
| 白名单 | 白名单 | Whitelist | ホワイトリスト | Senarai Putih | 白名單 |
| 编辑功能开关 | 编辑功能开关 | Edit Feature Flag | 機能フラグ編集 | Edit Suis Ciri | 編輯功能開關 |
| 请输入功能名称 | 请输入功能名称 | Enter feature name | 機能名を入力 | Masukkan nama ciri | 請輸入功能名稱 |
| 全局（不选择租户） | 全局（不选择租户） | Global (no tenant selected) | グローバル（テナント未選択） | Global (tiada penyewa dipilih) | 全域（不選擇租戶） |
| 确认启用功能 {name} | 确认启用功能 {name} | Confirm enable feature {name} | 機能 {name} を有効にしますか | Sahkan aktifkan ciri {name} | 確認啟用功能 {name} |
| 确认禁用功能 {name} | 确认禁用功能 {name} | Confirm disable feature {name} | 機能 {name} を無効にしますか | Sahkan nyahaktifkan ciri {name} | 確認停用功能 {name} |
| 参数键 | 参数键 | Parameter Key | パラメータキー | Kunci Parameter | 參數鍵 |
| 参数名称 | 参数名称 | Parameter Name | パラメータ名 | Nama Parameter | 參數名稱 |
| 参数类型 | 参数类型 | Parameter Type | パラメータタイプ | Jenis Parameter | 參數類型 |
| 参数值 | 参数值 | Parameter Value | パラメータ値 | Nilai Parameter | 參數值 |
| 字符串 | 字符串 | String | 文字列 | Rentetan | 字串 |
| 数字 | 数字 | Number | 数値 | Nombor | 數字 |
| 布尔值 | 布尔值 | Boolean | ブール値 | Boolean | 布林值 |
| JSON | JSON | JSON | JSON | JSON | JSON |
| 新增参数 | 新增参数 | Add Parameter | パラメータ追加 | Tambah Parameter | 新增參數 |
| 编辑参数 | 编辑参数 | Edit Parameter | パラメータ編集 | Edit Parameter | 編輯參數 |
| 请输入参数关键词 | 请输入参数关键词 | Enter parameter keyword | パラメータキーワードを入力 | Masukkan kata kunci parameter | 請輸入參數關鍵詞 |
| 请输入参数键 | 请输入参数键 | Enter parameter key | パラメータキーを入力 | Masukkan kunci parameter | 請輸入參數鍵 |
| 参数键长度 2-50 个字符 | 参数键长度 2-50 个字符 | Parameter key must be 2-50 characters | パラメータキーは2-50文字 | Kunci parameter mestilah 2-50 aksara | 參數鍵長度 2-50 個字元 |
| 请输入参数名称 | 请输入参数名称 | Enter parameter name | パラメータ名を入力 | Masukkan nama parameter | 請輸入參數名稱 |
| 参数名称长度 2-100 个字符 | 参数名称长度 2-100 个字符 | Parameter name must be 2-100 characters | パラメータ名は2-100文字 | Nama parameter mestilah 2-100 aksara | 參數名稱長度 2-100 個字元 |
| 请输入参数值 | 请输入参数值 | Enter parameter value | パラメータ値を入力 | Masukkan nilai parameter | 請輸入參數值 |
| 参数值长度 1-2000 个字符 | 参数值长度 1-2000 个字符 | Parameter value must be 1-2000 characters | パラメータ値は1-2000文字 | Nilai parameter mestilah 1-2000 aksara | 參數值長度 1-2000 個字元 |
| 操作成功 | 操作成功 | Operation successful | 操作成功 | Operasi berjaya | 操作成功 |

### common key（在组件级文件中值为 `null`）

以下 key 已存在于 common 语言文件中，组件级文件中写 `null`：

`查询`、`重置`、`新增`、`编辑`、`删除`、`确定`、`取消`、`操作`、`ID`、`状态`、`更新时间`、`暂无数据`、`功能说明`、`操作指南`、`保存成功`、`创建成功`、`更新成功`、`删除成功`、`是否启用`、`请选择租户`、`关键词`、`默认语言`、`默认时区`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"租户配置管理"存在
- [ ] 页面副标题存在
- [ ] 功能说明卡片（FunctionDescriptionCard）存在
- [ ] 操作指南入口（OperationGuideDrawer）存在
- [ ] **Tab 面板**包含三个 Tab：系统配置、功能开关、租户参数
- [ ] **系统配置**：表单展示并可编辑系统名称、Logo URL、系统主题、默认语言、默认时区
- [ ] **系统配置**：保存按钮功能正常
- [ ] **功能开关**：列表展示功能键、功能名称、状态（DxSwitch）、发布类型、更新时间、操作
- [ ] **功能开关**：DxSwitch 行内切换功能正常
- [ ] **功能开关**：编辑弹窗功能正常
- [ ] **租户参数**：租户选择器存在
- [ ] **租户参数**：列表展示参数键、参数名称、参数类型、参数值、更新时间、操作
- [ ] **租户参数**：新增/编辑/删除参数功能完整

### P1 — 业务规则完整性

- [ ] 系统名称 `stringLength`（max: 100）验证
- [ ] Logo URL `stringLength`（max: 500）验证
- [ ] 功能开关切换有 `confirmAction` 确认
- [ ] 功能开关切换失败回滚开关状态
- [ ] 参数键 `required` + `stringLength`（2-50）验证
- [ ] 参数名称 `required` + `stringLength`（2-100）验证
- [ ] 参数值 `required` + `stringLength`（1-2000）验证
- [ ] 编辑时参数键/功能键 disabled
- [ ] 删除参数有 `confirmDelete` 确认
- [ ] 每个操作按钮有权限码控制
- [ ] 租户参数新增按钮需已选择租户
- [ ] 提交时有 `submitting` loading 状态
- [ ] 提交成功后关闭弹窗并刷新列表
- [ ] 提交失败后不关闭弹窗

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] `notifySuccess` / `confirmAction` 不双重 t()
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null
- [ ] 所有按钮文本已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示消息已国际化
- [ ] 所有参数类型/发布类型/主题显示值已国际化
- [ ] Tab 标题已国际化
- [ ] 所有确认框文案已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过（见 `00-governance.md` 第四节）
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet` / `httpPost` / `httpPut` / `httpDelete`
