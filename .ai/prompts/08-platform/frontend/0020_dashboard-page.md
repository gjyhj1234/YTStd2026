# 租户平台 — 首页仪表盘

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 仪表盘为**只读展示**页面，展示运营统计数据。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-1 |
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
| DxChart | `DxChart data-source series type line bar area Vue` | 图表展示 |
| DxPieChart | `DxPieChart data-source series palette label connector Vue` | 饼图展示 |
| DxLoadPanel | `DxLoadPanel visible position shading` | 加载状态 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `PlatformOperationEndpoints.cs` 中的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 租户每日统计列表 | GET | `/api/platform-operations/tenant-statistics?page=1&pageSize=20&keyword=&tenantRefId=0` | - | `ApiResult<PagedResult<TenantDailyStatRepDTO>>` |
| 平台监控指标列表 | GET | `/api/platform-operations/monitor-metrics?page=1&pageSize=20&keyword=` | - | `ApiResult<PagedResult<PlatformMonitorMetricRepDTO>>` |

> **注意**：当前后端尚未实现 `/api/platform-operations/dashboard` 聚合接口。仪表盘应使用上述两个分页列表接口获取最新数据，或在前端聚合展示。如后续后端新增仪表盘聚合接口，再做适配。

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue` | 主页面 |
| 2 | `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/dashboard/DashboardView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/platform-operations.ts` | API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/platform-operations.ts` | 类型定义 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t('仪表盘')` | 页面主标题 |
| 页面副标题 | `<p>` + `$t('平台运营数据概览')` | 页面说明 |
| 统计卡片区 | 自定义卡片组 | 核心运营指标卡片 |
| 图表区 | `DxChart` / `DxPieChart` | 趋势图表 |
| 快捷操作区 | `DxButton` 组 | 常用操作快捷入口 |
| 加载状态 | `DxLoadPanel` | 数据加载中 |

---

## 统计卡片

### 卡片列表

| 序号 | 卡片标题 | 图标 | 数据来源 | 颜色 |
|:----:|---------|------|---------|------|
| 1 | `$t('活跃用户')` | `group` | 最近一天 TenantDailyStatRepDTO.ActiveUserCount 合计 | 蓝色 `#1890ff` |
| 2 | `$t('新增用户')` | `add` | 最近一天 TenantDailyStatRepDTO.NewUserCount 合计 | 绿色 `#52c41a` |
| 3 | `$t('API调用次数')` | `globe` | 最近一天 TenantDailyStatRepDTO.ApiCallCount 合计 | 橙色 `#fa8c16` |
| 4 | `$t('存储使用量')` | `folder` | 最近一天 TenantDailyStatRepDTO.StorageBytes 合计 | 紫色 `#722ed1` |

### 卡片样式

每个卡片需包含：
- 图标（左侧，使用上述颜色背景圆）
- 标题（使用 `$t()`）
- 数值（大字体加粗）
- 格式化：API 调用次数千分位逗号分隔，存储使用量转为 MB/GB 显示

---

## 图表

### 图表 1：每日活跃用户趋势

| 配置 | 值 |
|------|---|
| 组件 | `DxChart` |
| 类型 | 折线图 (`line`) |
| 数据源 | `TenantDailyStatRepDTO` 列表，按 `StatDate` 排序 |
| X 轴 | `StatDate`（日期格式 `MM-dd`） |
| Y 轴 | `ActiveUserCount` |
| 标题 | `$t('每日活跃用户趋势')` |

### 图表 2：每日 API 调用趋势

| 配置 | 值 |
|------|---|
| 组件 | `DxChart` |
| 类型 | 柱状图 (`bar`) |
| 数据源 | `TenantDailyStatRepDTO` 列表，按 `StatDate` 排序 |
| X 轴 | `StatDate`（日期格式 `MM-dd`） |
| Y 轴 | `ApiCallCount` |
| 标题 | `$t('每日API调用趋势')` |

### 图表 3：监控指标分布

| 配置 | 值 |
|------|---|
| 组件 | `DxPieChart` |
| 数据源 | `PlatformMonitorMetricRepDTO` 列表，按 `MetricType` 分组 |
| 系列字段 | `MetricType` |
| 值字段 | `MetricValue` |
| 标题 | `$t('监控指标分布')` |

---

## 快捷操作

| 序号 | 按钮 | 文本 | 图标 | 权限码 | 点击行为 |
|:----:|------|------|------|--------|---------|
| 1 | 创建租户 | `$t('创建租户')` | `add` | `tenant.create` | 路由跳转到租户列表页 |
| 2 | 创建用户 | `$t('创建用户')` | `add` | `platform.user.create` | 路由跳转到用户管理页 |
| 3 | 查看审计日志 | `$t('查看审计日志')` | `description` | `audit.list` | 路由跳转到审计日志页 |

---

## 类型定义

```typescript
// src/types/platform-operations.ts

/** 租户每日统计 */
export interface TenantDailyStatRepDTO {
  Id: number
  TenantRefId: number
  StatDate: string
  ActiveUserCount: number
  NewUserCount: number
  ApiCallCount: number
  StorageBytes: number
  ResourceScore: number
  CreatedAt: string
}

/** 平台监控指标 */
export interface PlatformMonitorMetricRepDTO {
  Id: number
  ComponentName: string
  MetricType: string
  MetricKey: string
  MetricValue: number
  MetricUnit?: string
  CollectedAt: string
}
```

---

## 国际化要求

### 组件级 key（放入 `DashboardView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 仪表盘 | 仪表盘 | Dashboard | ダッシュボード | Papan Pemuka | 儀表盤 |
| 平台运营数据概览 | 平台运营数据概览 | Platform Operations Overview | プラットフォーム運営データ概要 | Gambaran Keseluruhan Data Operasi Platform | 平台營運資料概覽 |
| 活跃用户 | 活跃用户 | Active Users | アクティブユーザー | Pengguna Aktif | 活躍使用者 |
| 新增用户 | 新增用户 | New Users | 新規ユーザー | Pengguna Baharu | 新增使用者 |
| API调用次数 | API调用次数 | API Call Count | API呼び出し回数 | Bilangan Panggilan API | API呼叫次數 |
| 存储使用量 | 存储使用量 | Storage Usage | ストレージ使用量 | Penggunaan Storan | 儲存使用量 |
| 每日活跃用户趋势 | 每日活跃用户趋势 | Daily Active Users Trend | 日次アクティブユーザー推移 | Trend Pengguna Aktif Harian | 每日活躍使用者趨勢 |
| 每日API调用趋势 | 每日API调用趋势 | Daily API Call Trend | 日次API呼び出し推移 | Trend Panggilan API Harian | 每日API呼叫趨勢 |
| 监控指标分布 | 监控指标分布 | Monitor Metrics Distribution | 監視指標分布 | Taburan Metrik Pemantauan | 監控指標分佈 |
| 快捷操作 | 快捷操作 | Quick Actions | クイック操作 | Tindakan Pantas | 快捷操作 |
| 创建租户 | 创建租户 | Create Tenant | テナント作成 | Cipta Penyewa | 建立租戶 |
| 创建用户 | 创建用户 | Create User | ユーザー作成 | Cipta Pengguna | 建立使用者 |
| 查看审计日志 | 查看审计日志 | View Audit Logs | 監査ログを見る | Lihat Log Audit | 查看稽核日誌 |

### common key（在组件级文件中值为 `null`）

`功能说明`、`操作指南`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题"仪表盘"存在
- [ ] 页面副标题存在
- [ ] **统计卡片**包含：活跃用户、新增用户、API调用次数、存储使用量（4 个卡片）
- [ ] 统计卡片有图标和颜色区分
- [ ] 数值格式化正确（千分位、MB/GB）
- [ ] **图表 1**：每日活跃用户趋势折线图存在
- [ ] **图表 2**：每日 API 调用趋势柱状图存在
- [ ] **图表 3**：监控指标分布饼图存在
- [ ] **快捷操作**包含：创建租户、创建用户、查看审计日志（3 个按钮）
- [ ] 加载中显示 DxLoadPanel

### P1 — 业务规则完整性

- [ ] 统计数据从正确 API 获取
- [ ] 图表数据从正确 API 获取
- [ ] 快捷操作按钮有权限码控制
- [ ] 快捷操作跳转路由正确
- [ ] 数据为空时图表友好展示

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] 所有卡片标题使用 $t()
- [ ] 所有图表标题使用 $t()
- [ ] 所有按钮文本使用 $t()
- [ ] 组件特有 key 在组件级语言文件中
- [ ] common key 在组件级文件中值为 null

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpGet`
