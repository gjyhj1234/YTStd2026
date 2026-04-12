# 租户平台 — 主布局与登录页

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 本模块包含主布局（MainLayout）和所有基础布局组件。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F1-1 |
| 所属阶段 | 层级 1：布局层 |
| 依赖任务 | F0-1 脚手架, F0-2 axios, F0-3 i18n, F0-4 路由, F0-5 状态管理 |
| 预计文件数 | 15+ 个（含语言文件） |
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

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDrawer | `DxDrawer opened-state-mode shrink reveal overlap position left Vue` | 侧边栏抽屉 |
| DxTreeView | `DxTreeView item template customization selectByClick focusStateEnabled activeStateEnabled CSS` | 侧边栏菜单树 |
| DxToolbar | `DxToolbar items location widget DxButton` | 顶栏工具条 |
| DxSelectBox | `DxSelectBox value-changed event items display-expr value-expr` | 语言切换下拉 |
| DxDropDownButton | `DxDropDownButton items showArrow text dropDownOptions` | 用户下拉菜单 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 布局组件使用的 API 端点（认证相关）：

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 获取当前用户 | GET | `/api/auth/me` | - | `ApiResult<CurrentUserRepDTO>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/layouts/MainLayout.vue` | 主布局组件 |
| 2 | `src/WebTenantPlatfrom/src/layouts/MainLayout.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/layouts/MainLayout.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/layouts/MainLayout.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/layouts/MainLayout.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/layouts/MainLayout.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue` | 功能说明卡片 |
| 8 | `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue` | 操作指南抽屉 |
| 9 | `src/WebTenantPlatfrom/src/styles/layout.css` | 布局样式 |
| 10 | `src/WebTenantPlatfrom/src/styles/sidebar.css` | 侧边栏样式（含 DxTreeView 反偏移 CSS） |

---

## 页面结构

### 整体布局

```
+------------------------------------------------------------+
|  Topbar（Logo, 搜索, 通知, 用户下拉, 语言切换）            |
+----------------+-------------------------------------------+
|                |  Breadcrumb                                |
|   Sidebar      +-------------------------------------------+
|   (DxDrawer    |                                           |
|    + DxTree-   |  Main Content                             |
|    View)       |  (FunctionDescriptionCard +               |
|                |   <router-view /> +                       |
|                |   OperationGuideDrawer)                   |
|                |                                           |
+----------------+-------------------------------------------+
```

### 顶栏（Topbar）

| 区域 | 组件 | 内容 |
|------|------|------|
| 左侧 Logo | `<img>` + `<span>` | 平台 Logo + `$t('租户管理平台')` |
| 侧边栏折叠按钮 | `DxButton` | 图标按钮，切换侧边栏展开/折叠 |
| 搜索框 | `DxTextBox` | 全局搜索（可选，初版可省略） |
| 通知图标 | `DxButton` | 通知铃铛图标（可选，初版可省略） |
| 用户下拉 | `DxDropDownButton` | 显示名 + 头像，下拉菜单：个人设置、修改密码、退出登录 |
| 语言切换 | `DxSelectBox` | 5 种语言切换 |

### 语言切换选项

| 值 | 显示文本 |
|:--:|---------|
| zh-CN | 简体中文 |
| en-US | English |
| ja-JP | 日本語 |
| ms-MY | Bahasa Melayu |
| zh-TW | 繁體中文 |

### 侧边栏（Sidebar）

**组件**：`DxDrawer` + `DxTreeView`

**DxDrawer 配置**：

| 属性 | 值 |
|------|---|
| opened-state-mode | `shrink` |
| position | `left` |
| reveal-mode | `expand` |
| min-size | 56（折叠时仅显示图标） |
| max-size | 240（展开时完整宽度） |

**DxTreeView 配置**：

| 属性 | 值 |
|------|---|
| items | 从菜单配置生成（按权限过滤） |
| display-expr | `text`（响应式，通过 `$t()` 获取） |
| :select-by-click | true |
| :focus-state-enabled | false（防止选中态偏移） |

### 侧边栏菜单结构

菜单数据需从静态配置生成，通过 `authStore.hasAnyPermission()` 按权限过滤：

```
├── 首页/仪表盘
├── 平台管理
│   ├── 用户管理
│   ├── 角色管理
│   └── 权限管理
├── 租户管理
│   ├── 租户列表
│   ├── 租户信息
│   ├── 资源配额
│   └── 配置与开关
├── SaaS 运营
│   ├── 套餐管理
│   ├── 订阅管理
│   └── 账单管理
├── API 集成
└── 系统管理
    ├── 审计日志
    ├── 通知管理
    └── 文件管理
```

### DxTreeView 选中态 CSS（关键约束）

**必须确保点击子菜单后不出现靠左对齐偏移。** 需使用以下 CSS 覆盖策略：

```css
/* sidebar.css — 关键：防止 DxTreeView 选中态偏移 */
.dx-treeview-item-content {
  padding-left: 20px !important;  /* 固定 padding，不随选中态变化 */
}
.dx-treeview .dx-state-focused,
.dx-treeview .dx-state-active,
.dx-treeview .dx-state-selected,
.dx-treeview .dx-state-hover {
  /* 使用 text-shadow 替代 font-weight 避免布局变化 */
  font-weight: normal !important;
}
.dx-treeview .dx-state-selected .dx-treeview-item-content {
  text-shadow: 0 0 0.5px currentColor;  /* 模拟加粗效果 */
}
```

### 折叠态行为

| 状态 | 表现 |
|------|------|
| 展开 | 显示图标 + 文本 |
| 折叠 | 仅显示图标，文本隐藏 |
| 切换 | 平滑过渡动画 |

### 语言切换行为

切换语言后：
1. 所有菜单文本实时更新（通过 `locale.value` 触发响应式）
2. 面包屑文本更新
3. 页面内容文本更新
4. 语言偏好保存到 localStorage

### 面包屑（Breadcrumb）

- 根据当前路由自动生成
- 每一级使用 `$t()` 翻译
- 首页不可点击，其他层级可点击跳转

### 用户下拉菜单

| 选项 | 图标 | 行为 |
|------|------|------|
| `$t('个人设置')` | `preferences` | 跳转设置页（可选，初版可省略） |
| `$t('修改密码')` | `key` | 打开修改密码弹窗 |
| `$t('退出登录')` | `runner` | 清除 token + 跳转登录页 |

---

## 国际化要求

### 组件级 key（放入 `MainLayout.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 租户管理平台 | 租户管理平台 | Tenant Management Platform | テナント管理プラットフォーム | Platform Pengurusan Penyewa | 租戶管理平台 |
| 首页 | 首页 | Home | ホーム | Laman Utama | 首頁 |
| 平台管理 | 平台管理 | Platform Management | プラットフォーム管理 | Pengurusan Platform | 平台管理 |
| 用户管理 | 用户管理 | User Management | ユーザー管理 | Pengurusan Pengguna | 使用者管理 |
| 角色管理 | 角色管理 | Role Management | ロール管理 | Pengurusan Peranan | 角色管理 |
| 权限管理 | 权限管理 | Permission Management | 権限管理 | Pengurusan Kebenaran | 權限管理 |
| 租户管理 | 租户管理 | Tenant Management | テナント管理 | Pengurusan Penyewa | 租戶管理 |
| 租户列表 | 租户列表 | Tenant List | テナント一覧 | Senarai Penyewa | 租戶列表 |
| 租户信息 | 租户信息 | Tenant Info | テナント情報 | Maklumat Penyewa | 租戶資訊 |
| 资源配额 | 资源配额 | Resource Quota | リソースクォータ | Kuota Sumber | 資源配額 |
| 配置与开关 | 配置与开关 | Config & Switches | 設定とスイッチ | Konfigurasi & Suis | 配置與開關 |
| SaaS 运营 | SaaS 运营 | SaaS Operations | SaaS運営 | Operasi SaaS | SaaS 營運 |
| 套餐管理 | 套餐管理 | Package Management | パッケージ管理 | Pengurusan Pakej | 套餐管理 |
| 订阅管理 | 订阅管理 | Subscription Management | サブスクリプション管理 | Pengurusan Langganan | 訂閱管理 |
| 账单管理 | 账单管理 | Billing Management | 請求書管理 | Pengurusan Bil | 帳單管理 |
| API 集成 | API 集成 | API Integration | API統合 | Integrasi API | API 整合 |
| 系统管理 | 系统管理 | System Management | システム管理 | Pengurusan Sistem | 系統管理 |
| 审计日志 | 审计日志 | Audit Logs | 監査ログ | Log Audit | 稽核日誌 |
| 通知管理 | 通知管理 | Notification Management | 通知管理 | Pengurusan Pemberitahuan | 通知管理 |
| 文件管理 | 文件管理 | File Management | ファイル管理 | Pengurusan Fail | 檔案管理 |
| 个人设置 | 个人设置 | Profile Settings | 個人設定 | Tetapan Peribadi | 個人設定 |
| 修改密码 | 修改密码 | Change Password | パスワード変更 | Tukar Kata Laluan | 修改密碼 |
| 退出登录 | 退出登录 | Logout | ログアウト | Log Keluar | 登出 |

### common key（在组件级文件中值为 `null`）

`功能说明`、`操作指南`

---

## 验收标准

### P0 — 功能点完整性

- [ ] 主布局 MainLayout.vue 正确渲染
- [ ] 顶栏包含：Logo、侧边栏折叠按钮、用户下拉、语言切换
- [ ] 侧边栏使用 DxDrawer + DxTreeView
- [ ] 侧边栏菜单结构与后端模块一致（14+ 个菜单项）
- [ ] 菜单按权限过滤（authStore.hasAnyPermission）
- [ ] 侧边栏可折叠，折叠时仅显示图标
- [ ] 面包屑根据路由自动生成
- [ ] 语言切换下拉包含 5 种语言
- [ ] 用户下拉包含：修改密码、退出登录
- [ ] 退出登录清除 token 并跳转登录页
- [ ] 内容区域渲染 `<router-view />`
- [ ] FunctionDescriptionCard 通用组件存在
- [ ] OperationGuideDrawer 通用组件存在

### P1 — 业务规则完整性

- [ ] **点击子菜单后不出现靠左对齐偏移**（DxTreeView CSS 已覆盖）
- [ ] DxTreeView `focusStateEnabled` 设为 false
- [ ] 语言切换后所有菜单文本实时更新
- [ ] 语言偏好保存到 localStorage
- [ ] 折叠态图标正确显示
- [ ] 展开态图标 + 文本正确显示
- [ ] 面包屑每一级使用 $t() 翻译

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] 所有菜单文本使用 $t()
- [ ] 所有顶栏文本使用 $t()
- [ ] 所有面包屑使用 $t()
- [ ] 用户下拉菜单选项使用 $t()

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过
- [ ] 无 fetch 调用（使用 axios）
- [ ] DxTreeView 选中态 CSS 覆盖已应用
