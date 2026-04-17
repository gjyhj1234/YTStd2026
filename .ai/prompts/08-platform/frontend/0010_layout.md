# 租户平台 — 主布局（基于 DevExtreme Vue Application Template）

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> **核心约束**：布局必须基于 **DevExtreme Vue Application Template** 的标准架构创建，
> 使用其内置的 `side-nav-outer-toolbar`（或 `side-nav-inner-toolbar`）布局组件，
> 而非从零手写布局结构。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F1-1 |
| 所属阶段 | 层级 1：布局层 |
| 依赖任务 | F0-1 脚手架, F0-2 axios, F0-3 i18n, F0-4 路由, F0-5 状态管理 |
| 预计文件数 | 20+ 个（含语言文件） |
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

**本模块必须查阅的文档**：

| 主题 | dxdocs 查阅问题 | 用途 |
|------|----------------|------|
| Application Template | `DevExtreme Vue Application Template structure layout navigation sidebar setup` | 了解模板整体架构 |
| Application Template Layouts | `DevExtreme Vue Application Template side-nav-outer-toolbar side-nav-inner-toolbar layout` | 选择和定制布局变体 |
| Application Template Navigation | `DevExtreme Vue Application Template app-navigation configure menu items path icon` | 配置导航菜单 |
| Application Template Toolbar | `DevExtreme Vue Application Template header-toolbar custom toolbar item DxItem` | 定制顶栏工具条 |
| Application Template Authentication | `DevExtreme Vue Application Template authentication auth integrate back end` | 集成认证流程 |
| Application Template Themes | `DevExtreme Vue Application Template themes theme-service color swatch switch` | 配置主题和颜色方案 |
| DxDrawer | `DxDrawer opened-state-mode shrink reveal overlap position left Vue` | 侧边栏抽屉（模板内置） |
| DxTreeView | `DxTreeView item template customization selectByClick focusStateEnabled CSS` | 侧边栏菜单树（模板内置） |
| DxToolbar | `DxToolbar items location widget DxButton DxItem` | 顶栏工具条（模板内置） |
| DxSelectBox | `DxSelectBox value-changed event items display-expr value-expr` | 语言切换下拉 |
| DxDropDownButton | `DxDropDownButton items showArrow text dropDownOptions` | 用户下拉菜单 |

每个主题查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## 一、Application Template 架构说明

### 1.1 模板生成方式

项目脚手架阶段（`0001_scaffold.md`）应使用 DevExtreme CLI 生成基础项目结构：

```bash
# 生成 Vue 3 应用（使用 outer toolbar 布局）
npx devextreme-cli new vue-app WebTenantPlatfrom --version=3

# 或指定 inner toolbar 布局
npx devextreme-cli new vue-app WebTenantPlatfrom --version=3 --layout=side-nav-inner-toolbar
```

> **注意**：如果 `devextreme-cli` 无法使用或生成的模板版本不匹配，
> 需手动创建与 Application Template 同构的文件结构（见下方 1.2 节）。

### 1.2 Application Template 标准目录结构

生成后的项目应包含以下 Application Template 标准结构（需适配到 `src/WebTenantPlatfrom` 路径）：

```
src/WebTenantPlatfrom/
├── src/
│   ├── main.ts                          # 应用入口
│   ├── App.vue                          # 根组件
│   ├── app-info.ts                      # 应用标题等元信息
│   ├── app-navigation.ts               # 导航菜单配置（Application Template 核心文件）
│   ├── auth.ts                          # 认证模块（Application Template 核心文件）
│   ├── router.ts                        # 路由配置（引入布局组件）
│   ├── theme-service.ts                 # 主题切换服务（Application Template 核心文件）
│   ├── layouts/
│   │   ├── side-nav-outer-toolbar.vue   # Outer toolbar 布局（Application Template 核心组件）
│   │   ├── side-nav-inner-toolbar.vue   # Inner toolbar 布局（备选）
│   │   └── single-card.vue             # 登录页等独立卡片布局
│   ├── components/
│   │   ├── header-toolbar.vue           # 顶栏工具条组件（Application Template 核心组件）
│   │   ├── side-navigation-menu.vue     # 侧边栏菜单组件（Application Template 核心组件）
│   │   ├── login-form.vue              # 登录表单（Application Template 提供）
│   │   ├── FunctionDescriptionCard.vue  # 功能说明卡片（自定义扩展）
│   │   └── OperationGuideDrawer.vue     # 操作指南抽屉（自定义扩展）
│   ├── views/                           # 页面视图
│   ├── api/                             # API 封装（axios）
│   ├── store/                           # Pinia 状态管理
│   ├── types/                           # TypeScript 类型
│   ├── locales/                         # 国际化资源
│   ├── styles/                          # 全局和组件样式
│   └── themes/                          # DevExtreme 主题配置
│       ├── metadata.base.json           # 基础主题元数据
│       ├── metadata.additional.json     # 导航栏颜色方案（color swatch）
│       └── generated/                   # 编译后的主题 CSS
├── public/
└── package.json
```

### 1.3 布局选择

| 布局类型 | 文件 | 特点 | 推荐场景 |
|---------|------|------|---------|
| **side-nav-outer-toolbar**（推荐） | `layouts/side-nav-outer-toolbar.vue` | 工具栏在侧边栏外部，横跨全宽 | 管理后台（本项目使用） |
| side-nav-inner-toolbar | `layouts/side-nav-inner-toolbar.vue` | 工具栏仅在内容区域内 | 备选 |
| single-card | `layouts/single-card.vue` | 无侧边栏，居中卡片 | 登录页、重置密码页 |

**本项目默认使用 `side-nav-outer-toolbar` 布局。**

---

## 二、Application Template 核心组件定制

### 2.1 导航菜单配置（app-navigation.ts）

Application Template 使用 `app-navigation.ts`（原模板为 `.js`，本项目使用 `.ts`）配置导航菜单。每个菜单项的配置字段：

| 字段 | 类型 | 说明 |
|------|------|------|
| text | string | 菜单文本（需通过 `$t()` 国际化） |
| icon | string | DevExtreme 图标名称 |
| path | string | 路由路径（叶子节点必须有） |
| items | array | 子菜单（父节点不能同时有 `path` 和 `items`） |
| permission | string | 权限码（自定义扩展，用于按权限过滤） |

**菜单数据定义示例**：

```typescript
// src/app-navigation.ts
export interface NavigationItem {
  text: string
  icon?: string
  path?: string
  items?: NavigationItem[]
  permission?: string   // 自定义扩展：权限过滤
}

export function getNavigationItems(t: (key: string) => string): NavigationItem[] {
  return [
    { text: t('首页'), icon: 'home', path: '/dashboard' },
    {
      text: t('平台管理'), icon: 'group', items: [
        { text: t('用户管理'), path: '/platform-users', permission: 'PLATFORM_USER_VIEW' },
        { text: t('角色管理'), path: '/platform-roles', permission: 'PLATFORM_ROLE_VIEW' },
        { text: t('权限管理'), path: '/platform-permissions', permission: 'PLATFORM_PERMISSION_VIEW' },
      ]
    },
    // ... 其他菜单项见下方菜单结构
  ]
}
```

> **关键**：Application Template 要求菜单项"要么有 `path`，要么有 `items`，不能两者都有"。

### 2.2 侧边栏菜单结构

```
├── 首页/仪表盘          (icon: home,    path: /dashboard)
├── 平台管理              (icon: group)
│   ├── 用户管理          (path: /platform-users)
│   ├── 角色管理          (path: /platform-roles)
│   └── 权限管理          (path: /platform-permissions)
├── 租户管理              (icon: box)
│   ├── 租户列表          (path: /tenants)
│   ├── 租户信息          (path: /tenant-info)
│   ├── 资源配额          (path: /tenant-resources)
│   └── 配置与开关        (path: /tenant-config)
├── SaaS 运营             (icon: money)
│   ├── 套餐管理          (path: /packages)
│   ├── 订阅管理          (path: /subscriptions)
│   └── 账单管理          (path: /billing)
├── API 集成              (icon: globe,   path: /api-integration)
└── 系统管理              (icon: preferences)
    ├── 审计日志          (path: /audit-logs)
    ├── 通知管理          (path: /notifications)
    └── 文件管理          (path: /storage)
```

菜单数据通过 `authStore.hasAnyPermission()` 按权限过滤后传给 `side-navigation-menu.vue`。

> **⚠️ 超级管理员权限旁路（零容忍）**：`store/auth.ts` 中的 `hasPermission()` 和 `hasAnyPermission()` 必须先检查 `user.IsSuperAdmin`，若为 true 则直接返回 true，与后端 `CurrentUser.HasPermission` 逻辑一致。否则超级管理员登录后侧边栏菜单全部消失（后端不为超管分配具体权限码，`Permissions` 数组为空）。`CurrentUser` 接口必须包含 `IsSuperAdmin: boolean` 字段，登录时从 `LoginRepDTO.IsSuperAdmin` 赋值。

### 2.3 顶栏工具条定制（header-toolbar.vue）

Application Template 的 `header-toolbar.vue` 使用 `DxToolbar` + `DxItem` 组织顶栏。本项目需要在标准模板基础上扩展以下自定义项：

| 区域 | DxItem location | 组件 | 内容 |
|------|:---------------:|------|------|
| 菜单按钮 | before | `DxButton`（模板内置） | 切换侧边栏展开/折叠 |
| Logo + 标题 | before | `<img>` + `<span>` | 平台 Logo + `$t('租户管理平台')` |
| 语言切换 | after | `DxSelectBox` | 5 种语言切换（自定义扩展） |
| 用户下拉 | after | `DxDropDownButton` | 用户菜单（自定义扩展） |

**语言切换选项**：

| 值 | 显示文本 |
|:--:|---------|
| zh-CN | 简体中文 |
| en-US | English |
| ja-JP | 日本語 |
| ms-MY | Bahasa Melayu |
| zh-TW | 繁體中文 |

**用户下拉菜单选项**：

| 选项 | 图标 | 行为 |
|------|------|------|
| `$t('个人设置')` | `preferences` | 跳转设置页（可选，初版可省略） |
| `$t('修改密码')` | `key` | 打开修改密码弹窗 |
| `$t('退出登录')` | `runner` | 清除 token + 跳转登录页 |

### 2.4 侧边栏（side-navigation-menu.vue）

Application Template 内置 `side-navigation-menu.vue`，使用 `DxTreeView` 渲染菜单。本项目需定制：

**DxTreeView 配置（在 side-navigation-menu.vue 中）**：

| 属性 | 值 | 说明 |
|------|---|------|
| :items | filteredNavItems | 按权限过滤后的菜单数据 |
| display-expr | `text` | 响应式，通过 `$t()` 获取 |
| key-expr | `path` | 路由路径作为唯一标识 |
| :select-by-click | true | 点击即选中 |
| :focus-state-enabled | false | **关键：防止选中态偏移** |
| selection-mode | `single` | 单选模式 |
| @item-click | onItemClick | 导航到对应路由 |

**DxDrawer 配置（在 side-nav-outer-toolbar.vue 中）**：

| 属性 | 值 | 说明 |
|------|---|------|
| opened-state-mode | `shrink` | 内容区域收缩让出空间 |
| position | `left` | 侧边栏在左侧 |
| reveal-mode | `expand` | 侧边栏展开方式 |
| :min-size | 56 | 折叠时仅显示图标 |
| :max-size | 240 | 展开时完整宽度 |
| :opened | drawerOpened | 响应式控制 |
| template | `navigation` | 使用具名插槽渲染菜单 |

### 2.5 DxTreeView 选中态 CSS（关键约束）

**必须确保点击子菜单后不出现靠左对齐偏移。** 需使用以下 CSS 覆盖策略：

```css
/* styles/sidebar.css — 关键：防止 DxTreeView 选中态偏移 */
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

### 2.6 认证集成（auth.ts）

Application Template 内置认证流程，通过 `src/auth.ts`（原模板为 `.js`）管理。本项目需要：

1. 将模板中的 stub 函数替换为真实 API 调用（使用 axios）
2. 认证相关 API 端点：

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 登录 | POST | `/api/auth/login` | `{ Username, Password }` | `ApiResult<LoginRepDTO>` |
| 获取当前用户 | GET | `/api/auth/me` | - | `ApiResult<CurrentUserRepDTO>` |

3. `auth.ts` 导出的函数约定（与 Application Template 兼容）：

| 函数 | 返回值 | 说明 |
|------|--------|------|
| `logIn(username, password)` | `{ isOk, data?, message? }` | 调用登录 API |
| `logOut()` | void | 清除 token，跳转登录页 |
| `getUser()` | `{ isOk, data? }` | 调用 /api/auth/me |
| `createAccount(email, password)` | `{ isOk }` | 可选，初版可省略 |
| `changePassword(password, recoveryCode)` | `{ isOk }` | 可选，初版可省略 |
| `resetPassword(email)` | `{ isOk }` | 可选，初版可省略 |

### 2.7 主题配置（theme-service.ts）

Application Template 内置主题切换逻辑。本项目需保留并配置：

| 文件 | 用途 |
|------|------|
| `src/themes/metadata.base.json` | 基础主题元数据 |
| `src/themes/metadata.additional.json` | 导航栏颜色方案（color swatch） |
| `src/themes/metadata.additional.dark.json` | 暗色导航栏颜色方案 |
| `src/theme-service.ts` | 主题切换服务 |
| `src/variables.scss` | 全局 CSS 变量 |

导航栏使用 `dx-swatch-additional` CSS 类应用颜色方案（Application Template 标准做法）。

---

## 三、折叠态与响应式行为

| 状态 | 表现 |
|------|------|
| 展开 | 显示图标 + 文本 |
| 折叠 | 仅显示图标，文本隐藏 |
| 切换 | 平滑过渡动画（DxDrawer 内置） |
| 响应式 | 窗口宽度 < 768px 时自动折叠侧边栏 |

---

## 四、语言切换行为

切换语言后：
1. 所有菜单文本实时更新（`app-navigation.ts` 中使用 `t()` 获取文本，切换 `locale.value` 触发响应式）
2. 面包屑文本更新
3. 页面内容文本更新
4. 语言偏好保存到 localStorage
5. DevExtreme 组件本地化消息同步更新

---

## 五、面包屑（Breadcrumb）

- 根据当前路由自动生成
- 每一级使用 `$t()` 翻译
- 首页不可点击，其他层级可点击跳转

---

## API 端点（精确匹配）

> 布局组件使用的 API 端点（认证相关）：

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 登录 | POST | `/api/auth/login` | `{ Username, Password }` | `ApiResult<LoginRepDTO>` |
| 获取当前用户 | GET | `/api/auth/me` | - | `ApiResult<CurrentUserRepDTO>` |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 | 来源 |
|:----:|---------|------|------|
| 1 | `src/WebTenantPlatfrom/src/layouts/side-nav-outer-toolbar.vue` | 主布局组件（Application Template 核心） | 模板生成 + 定制 |
| 2 | `src/WebTenantPlatfrom/src/layouts/side-nav-inner-toolbar.vue` | 备选布局组件 | 模板生成 |
| 3 | `src/WebTenantPlatfrom/src/layouts/single-card.vue` | 登录页卡片布局 | 模板生成 |
| 4 | `src/WebTenantPlatfrom/src/components/header-toolbar.vue` | 顶栏工具条（Application Template 核心） | 模板生成 + 定制 |
| 5 | `src/WebTenantPlatfrom/src/components/side-navigation-menu.vue` | 侧边栏菜单（Application Template 核心） | 模板生成 + 定制 |
| 6 | `src/WebTenantPlatfrom/src/app-navigation.ts` | 导航菜单配置 | 模板生成 + 定制 |
| 7 | `src/WebTenantPlatfrom/src/auth.ts` | 认证模块 | 模板生成 + 对接后端 |
| 8 | `src/WebTenantPlatfrom/src/theme-service.ts` | 主题切换服务 | 模板生成 |
| 9 | `src/WebTenantPlatfrom/src/app-info.ts` | 应用元信息 | 模板生成 + 定制 |
| 10 | `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue` | 功能说明卡片 | 自定义扩展 |
| 11 | `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue` | 操作指南抽屉 | 自定义扩展 |
| 12 | `src/WebTenantPlatfrom/src/styles/sidebar.css` | 侧边栏样式（含 DxTreeView 反偏移 CSS） | 自定义 |
| 13 | `src/WebTenantPlatfrom/src/themes/metadata.base.json` | 基础主题元数据 | 模板生成 |
| 14 | `src/WebTenantPlatfrom/src/themes/metadata.additional.json` | 导航栏颜色方案 | 模板生成 |
| 15 | `src/WebTenantPlatfrom/src/variables.scss` | 全局 CSS 变量 | 模板生成 |
| 16-20 | 布局组件语言文件 `.vue.{locale}.json` × 5 | 国际化 | 自定义 |

**语言文件详情**（以 `side-nav-outer-toolbar.vue` 或统一布局语言文件为例）：

| 文件 | 说明 |
|------|------|
| `*.vue.zh-CN.json` | 简体中文 |
| `*.vue.en-US.json` | 英文 |
| `*.vue.ja-JP.json` | 日文 |
| `*.vue.ms-MY.json` | 马来文 |
| `*.vue.zh-TW.json` | 繁体中文 |

---

## 国际化要求

### 组件级 key（放入布局组件对应的 `.vue.{locale}.json`）

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

### P0 — 功能点完整性（Application Template 架构）

- [ ] 项目基于 DevExtreme Vue Application Template 生成或同构创建
- [ ] `side-nav-outer-toolbar.vue` 布局组件存在且作为默认布局
- [ ] `single-card.vue` 卡片布局存在（供登录页使用）
- [ ] `header-toolbar.vue` 顶栏组件存在
- [ ] `side-navigation-menu.vue` 侧边栏菜单组件存在
- [ ] `app-navigation.ts` 导航菜单配置存在
- [ ] `auth.ts` 认证模块存在（对接后端 API）
- [ ] `theme-service.ts` 主题服务存在
- [ ] 顶栏包含：菜单按钮、Logo、语言切换、用户下拉
- [ ] 侧边栏使用 DxDrawer + DxTreeView（Application Template 内置方式）
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
- [ ] 导航栏使用 `dx-swatch-additional` 颜色方案
- [ ] 窗口宽度 < 768px 时侧边栏自动折叠

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] 所有菜单文本使用 $t()（通过 app-navigation.ts 中的 t 函数）
- [ ] 所有顶栏文本使用 $t()
- [ ] 所有面包屑使用 $t()
- [ ] 用户下拉菜单选项使用 $t()

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过
- [ ] 无 fetch 调用（使用 axios）
- [ ] DxTreeView 选中态 CSS 覆盖已应用
- [ ] Application Template 标准文件结构完整
