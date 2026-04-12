# 租户平台 — 平台运营页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 平台运营页面为**只读展示**页面，展示租户每日统计和平台监控指标两个数据视图。
> 与仪表盘（`0020_dashboard-page.md`）的区别：仪表盘展示概览摘要，本页面展示详细的历史统计数据和监控指标列表。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-16 |
| 所属阶段 | 层级 2：业务页面层 |
| 依赖任务 | F1-1 主布局 |
| 预计文件数 | 8 个（含语言文件） |
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
- `.ai/prompts/08-platform/backend/platform-operation-api.md` — 后端 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | `DxDataGrid CustomStore remote paging load function skip take Vue` | 统计列表分页 |
| DxTabPanel | `DxTabPanel tabs items selectedIndex onSelectionChanged Vue` | 标签页切换 |
| DxDateBox | `DxDateBox type date format displayFormat onValueChanged Vue` | 筛选日期 |
| DxSelectBox | `DxSelectBox dataSource displayExpr valueExpr placeholder Vue` | 筛选租户 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

```
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid CustomStore remote paging load function skip take")
devexpress_docs_search(technologies: ["Vue"], question: "DxTabPanel tabs items selectedIndex onSelectionChanged")
```

---

## API 端点（精确匹配）

以下端点精确匹配 `PlatformOperationEndpoints.cs` 中的路由注册：

| 操作 | HTTP 方法 | URL | 请求参数 | 响应体 |
|------|----------|-----|---------|--------|
| 租户每日统计列表 | GET | `/api/platform-operations/tenant-statistics?page=1&pageSize=20&keyword=&tenantRefId=` | Query: page, pageSize, keyword, tenantRefId | `ApiResult<PagedResult<TenantDailyStatRepDTO>>` |
| 监控指标列表 | GET | `/api/platform-operations/monitor-metrics?page=1&pageSize=20&keyword=` | Query: page, pageSize, keyword | `ApiResult<PagedResult<PlatformMonitorMetricRepDTO>>` |

### TenantDailyStatRepDTO 字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| Id | long | 统计 ID |
| TenantRefId | long | 关联租户 ID |
| StatDate | DateTime | 统计日期 |
| ActiveUserCount | int | 活跃用户数 |
| NewUserCount | int | 新增用户数 |
| ApiCallCount | long | API 调用次数 |
| StorageBytes | long | 存储字节数 |
| ResourceScore | decimal | 资源评分 |
| CreatedAt | DateTime | 创建时间 |

### PlatformMonitorMetricRepDTO 字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| Id | long | 指标 ID |
| ComponentName | string | 组件名称 |
| MetricType | string | 指标类型 |
| MetricKey | string | 指标键 |
| MetricValue | decimal | 指标值 |
| MetricUnit | string? | 指标单位 |
| CollectedAt | DateTime | 采集时间 |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/views/platform-operations/PlatformOperationsView.vue` | 主页面（双标签页） |
| 2 | `src/views/platform-operations/PlatformOperationsView.vue.zh-CN.json` | 中文语言 |
| 3 | `src/views/platform-operations/PlatformOperationsView.vue.en-US.json` | 英文语言 |
| 4 | `src/views/platform-operations/PlatformOperationsView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/views/platform-operations/PlatformOperationsView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/views/platform-operations/PlatformOperationsView.vue.zh-TW.json` | 繁中语言 |
| 7 | `src/api/platform-operation.ts` | API 封装 |
| 8 | `src/types/platform-operation.ts` | 类型定义 |
| 9 | `src/router/index.ts`（追加） | 路由注册 |

---

## 页面结构

本页面采用 **DxTabPanel** 双标签页布局，两个标签页各包含一个独立的 DxDataGrid 列表。

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t()` | 平台运营 |
| 页面副标题 | `<p>` + `$t()` | 查看租户每日统计和平台监控指标 |
| 标签页容器 | DxTabPanel | 两个标签页 |
| 标签页 1 | 租户每日统计 | DxDataGrid + 查询筛选 |
| 标签页 2 | 监控指标 | DxDataGrid + 查询筛选 |

---

## 查询功能

### 标签页 1：租户每日统计 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | tenantRefId | 租户 | DxSelectBox（数据源来自租户列表接口） | null（全部） | '请选择租户' |
| 2 | keyword | 关键词 | DxTextBox | '' | '请输入关键词' |

### 标签页 2：监控指标 — 查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | keyword | 关键词 | DxTextBox | '' | '请输入指标类型关键词' |

### 查询行为

- 支持回车搜索：是
- 支持重置：是
- 所有查询文本已国际化：是

---

## 列表与分页

### 标签页 1：租户每日统计 — DxDataGrid + CustomStore

#### 列定义

| 序号 | data-field | caption | 宽度 | 排序 | 格式化 | 说明 |
|:----:|-----------|---------|:----:|:----:|--------|------|
| 1 | Id | ID | 80 | 否 | - | |
| 2 | TenantRefId | 租户 ID | 120 | 否 | - | |
| 3 | StatDate | 统计日期 | 120 | 是 | dateCell | yyyy-MM-dd |
| 4 | ActiveUserCount | 活跃用户数 | 120 | 是 | numberCell | 千分位 |
| 5 | NewUserCount | 新增用户数 | 120 | 是 | numberCell | 千分位 |
| 6 | ApiCallCount | API 调用次数 | 140 | 是 | numberCell | 千分位 |
| 7 | StorageBytes | 存储用量 | 120 | 是 | bytesCell | 自动转换 KB/MB/GB |
| 8 | ResourceScore | 资源评分 | 100 | 是 | decimalCell | 保留 2 位 |
| 9 | CreatedAt | 创建时间 | 180 | 否 | dateTimeCell | yyyy-MM-dd HH:mm |

#### 分页配置

- 支持分页：是
- 默认页大小：20
- 可选页大小：[10, 20, 50, 100]
- 显示总数：是
- 远程分页：是（CustomStore）

### 标签页 2：监控指标 — DxDataGrid + CustomStore

#### 列定义

| 序号 | data-field | caption | 宽度 | 排序 | 格式化 | 说明 |
|:----:|-----------|---------|:----:|:----:|--------|------|
| 1 | Id | ID | 80 | 否 | - | |
| 2 | ComponentName | 组件名称 | 150 | 是 | - | |
| 3 | MetricType | 指标类型 | 120 | 是 | - | |
| 4 | MetricKey | 指标键 | 150 | 否 | - | |
| 5 | MetricValue | 指标值 | 120 | 是 | decimalCell | 保留 4 位 |
| 6 | MetricUnit | 单位 | 80 | 否 | - | 可为空 |
| 7 | CollectedAt | 采集时间 | 180 | 是 | dateTimeCell | yyyy-MM-dd HH:mm:ss |

#### 分页配置

- 支持分页：是
- 默认页大小：20
- 可选页大小：[10, 20, 50, 100]
- 显示总数：是
- 远程分页：是（CustomStore）

### 空状态与加载

- 空数据文本：`$t('暂无数据')`
- 加载中：DxLoadPanel

---

## 操作按钮

本页面为**只读展示**页面，无新增/编辑/删除操作。

### 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 点击行为 |
|------|------|------|--------|---------|
| 刷新 | $t('刷新') | refresh | PLATFORM_OPERATION_VIEW | 重新加载当前标签页数据 |
| 导出 | $t('导出') | export | PLATFORM_OPERATION_EXPORT | 导出当前标签页数据为 Excel（可选功能） |

---

## 表单功能

本页面为只读展示页面，无表单。

---

## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| `columns-daily-stat.ts`（可选） | 租户每日统计列定义数组 | 页面组件级语言文件 |
| `columns-monitor-metric.ts`（可选） | 监控指标列定义数组 | 页面组件级语言文件 |

---

## 国际化要求

### 组件级 key（放入 `*.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 平台运营 | 平台运营 | Platform Operations | プラットフォーム運営 | Operasi Platform | 平台營運 |
| 查看租户每日统计和平台监控指标 | 查看租户每日统计和平台监控指标 | View tenant daily statistics and platform monitoring metrics | テナント日次統計とプラットフォーム監視指標を表示 | Lihat statistik harian penyewa dan metrik pemantauan platform | 查看租戶每日統計和平台監控指標 |
| 租户每日统计 | 租户每日统计 | Tenant Daily Statistics | テナント日次統計 | Statistik Harian Penyewa | 租戶每日統計 |
| 监控指标 | 监控指标 | Monitor Metrics | 監視指標 | Metrik Pemantauan | 監控指標 |
| 租户 ID | 租户 ID | Tenant ID | テナントID | ID Penyewa | 租戶 ID |
| 统计日期 | 统计日期 | Stat Date | 統計日 | Tarikh Statistik | 統計日期 |
| 活跃用户数 | 活跃用户数 | Active Users | アクティブユーザー数 | Pengguna Aktif | 活躍用戶數 |
| 新增用户数 | 新增用户数 | New Users | 新規ユーザー数 | Pengguna Baharu | 新增用戶數 |
| API 调用次数 | API 调用次数 | API Calls | API呼び出し回数 | Panggilan API | API 調用次數 |
| 存储用量 | 存储用量 | Storage Usage | ストレージ使用量 | Penggunaan Storan | 儲存用量 |
| 资源评分 | 资源评分 | Resource Score | リソーススコア | Skor Sumber | 資源評分 |
| 组件名称 | 组件名称 | Component Name | コンポーネント名 | Nama Komponen | 元件名稱 |
| 指标类型 | 指标类型 | Metric Type | 指標タイプ | Jenis Metrik | 指標類型 |
| 指标键 | 指标键 | Metric Key | 指標キー | Kunci Metrik | 指標鍵 |
| 指标值 | 指标值 | Metric Value | 指標値 | Nilai Metrik | 指標值 |
| 单位 | 单位 | Unit | 単位 | Unit | 單位 |
| 采集时间 | 采集时间 | Collected At | 採集時刻 | Masa Dikumpul | 採集時間 |
| 请选择租户 | 请选择租户 | Select tenant | テナントを選択 | Pilih penyewa | 請選擇租戶 |
| 请输入指标类型关键词 | 请输入指标类型关键词 | Enter metric type keyword | 指標タイプキーワードを入力 | Masukkan kata kunci jenis metrik | 請輸入指標類型關鍵詞 |

### common key（放入 common/{locale}.json，值为 null）

已有的 common key 无需重复定义，仅在组件级文件中写 null 即可：

- ID、关键词、请输入关键词、创建时间、查询、重置、刷新、导出、暂无数据

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"平台运营"存在
- [ ] 页面副标题描述文字存在
- [ ] DxTabPanel 包含两个标签页："租户每日统计""监控指标"
- [ ] 标签页 1 — 租户每日统计表格列包含：ID、租户 ID、统计日期、活跃用户数、新增用户数、API 调用次数、存储用量、资源评分、创建时间
- [ ] 标签页 1 — 租户选择筛选器存在且数据来源正确
- [ ] 标签页 1 — 关键词搜索存在
- [ ] 标签页 2 — 监控指标表格列包含：ID、组件名称、指标类型、指标键、指标值、单位、采集时间
- [ ] 标签页 2 — 关键词搜索存在
- [ ] 两个标签页均支持分页（页大小选择、总数显示）
- [ ] 刷新按钮存在且功能正确

### P1 — 业务规则完整性

- [ ] 租户每日统计列表按 tenantRefId 筛选数据
- [ ] 监控指标列表按 keyword 筛选 MetricType
- [ ] 存储字节数正确格式化为 KB/MB/GB
- [ ] 数值列正确显示千分位
- [ ] 日期列正确格式化
- [ ] CustomStore 远程分页参数正确传递

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建且 key 一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] 标签页标题使用 `$t()`
- [ ] 查询字段 label/placeholder 使用 `$t()`
- [ ] 组件特有 key 在组件级文件（非主语言文件）
- [ ] null key 在 common 中有对应翻译

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符
- [ ] Code Review 自检全部通过
