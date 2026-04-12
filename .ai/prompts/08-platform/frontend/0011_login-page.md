# 租户平台 — 登录页面

> 本文件是"极细化业务实施提示词"，按照 `03-frontend/07-business-prompt-template.md` 模板编写。
> 登录页为独立全屏页面，不使用主布局（MainLayout）。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F1-2 |
| 所属阶段 | 层级 1：布局层 |
| 依赖任务 | F0-1 脚手架, F0-2 axios, F0-3 i18n, F0-4 路由, F0-5 状态管理 |
| 预计文件数 | 9 个（含语言文件） |
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
- `.github/copilot-instructions.md` — 关键编码约束（第 7-13 条为前端约束，**第 8 条 label-mode 必须为 static**）
- `.ai/prompts/08-platform/backend/auth-api.md` — 后端认证 API 定义

---

## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `03-frontend/04-devextreme-templates.md` 第二节。

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxForm | `DxForm label-mode static floating browser autofill difference` | 登录表单（**必须使用 static**） |
| DxForm | `DxForm validation rules required stringLength validationCallback` | 表单验证 |
| DxTextBox | `DxTextBox mode password placeholder value-changed` | 密码输入框 |
| DxButton | `DxButton type default success danger text icon styling-mode` | 登录按钮 |
| DxSelectBox | `DxSelectBox value-changed items display-expr value-expr` | 语言切换 |
| DxLoadIndicator | `DxLoadIndicator visible` | 登录加载状态 |

每个组件查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读代码示例。

---

## API 端点（精确匹配）

> 以下端点精确对应 `RouteRegistration.cs` 中 MapAuthEndpoints 的路由注册。

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 登录 | POST | `/api/auth/login` | `{ Username, Password }` | `ApiResult<LoginRepDTO>` |

### 登录响应字段

```typescript
interface LoginRepDTO {
  Token: string
  ExpiresIn: number
  UserId: number
  Username: string
  DisplayName: string
  RequirePasswordReset: boolean
  Roles: string[]
  Permissions: string[]
  IsSuperAdmin: boolean
}
```

### 登录失败错误码

| 错误码 | 说明 | HTTP 状态码 |
|:------:|------|:----------:|
| AuthCredentialsRequired | 用户名或密码为空 | 400 |
| AuthInvalidCredentials | 用户名或密码错误 | 401 |
| AuthAccountDisabled | 账户已禁用 | 403 |
| AuthAccountLocked | 账户已锁定 | 423 |

---

## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/WebTenantPlatfrom/src/views/login/LoginView.vue` | 登录页面 |
| 2 | `src/WebTenantPlatfrom/src/views/login/LoginView.vue.zh-CN.json` | 简体中文语言 |
| 3 | `src/WebTenantPlatfrom/src/views/login/LoginView.vue.en-US.json` | 英文语言 |
| 4 | `src/WebTenantPlatfrom/src/views/login/LoginView.vue.ja-JP.json` | 日文语言 |
| 5 | `src/WebTenantPlatfrom/src/views/login/LoginView.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/WebTenantPlatfrom/src/views/login/LoginView.vue.zh-TW.json` | 繁体中文语言 |
| 7 | `src/WebTenantPlatfrom/src/api/auth.ts` | 认证 API 封装 |
| 8 | `src/WebTenantPlatfrom/src/types/auth.ts` | 认证类型定义 |
| 9 | `src/WebTenantPlatfrom/src/styles/login.css` | 登录页样式 |

---

## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面背景 | 全屏背景 | 渐变色或背景图 |
| 登录卡片 | 居中卡片容器 | 包含 Logo + 标题 + 表单 + 语言切换 |
| Logo | `<img>` | 平台 Logo 图片 |
| 标题 | `<h1>` + `$t('租户管理平台')` | 登录标题 |
| 副标题 | `<p>` + `$t('请登录您的账号')` | 登录副标题 |
| 登录表单 | `DxForm` (`label-mode="static"`) | 用户名 + 密码 |
| 登录按钮 | `DxButton` | 登录提交 |
| 语言切换 | `DxSelectBox` | 5 种语言切换（放在卡片底部或右上角） |

### 页面布局

```
+--------------------------------------------+
|                                            |
|          [语言切换 - 右上角]                |
|                                            |
|          +--------------------+            |
|          |      [Logo]        |            |
|          |  租户管理平台       |            |
|          |  请登录您的账号     |            |
|          |                    |            |
|          |  用户名 [________] |            |
|          |  密码   [________] |            |
|          |                    |            |
|          |  [    登 录    ]   |            |
|          +--------------------+            |
|                                            |
|          © 2026 YTStd                      |
+--------------------------------------------+
```

---

## 表单功能

### 登录表单

**组件**：`DxForm`（`label-mode="static"`）

> **关键约束（零容忍）**：DxForm label-mode **必须使用 `"static"`**，**禁止使用 `"floating"`**。
> 原因：浏览器自动填充用户名/密码时，DevExtreme floating label 不感知填充值，导致 label 与值重叠。
> 这是 DevExtreme 的已知行为，无法通过 CSS 修复。

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度限制 | 默认值 | 说明 |
|:----:|--------|------|------|:----:|---------|--------|------|
| 1 | Username | `$t('用户名')` | DxTextBox | 是 | - | `''` | placeholder: `$t('请输入用户名')` |
| 2 | Password | `$t('密码')` | DxTextBox (`mode: 'password'`) | 是 | min: 6 | `''` | placeholder: `$t('请输入密码')` |

**验证规则汇总**：

| 字段 | 规则类型 | 参数 | 验证消息 |
|------|---------|------|---------|
| Username | required | - | `$t('请输入用户名')` |
| Password | required | - | `$t('请输入密码')` |
| Password | stringLength | min: 6 | `$t('密码长度至少 6 个字符')` |

**提交行为**：

1. 提交前：调用 `formInstance.validate()`，不通过则阻止提交
2. 支持回车键提交
3. 提交时：`logging.value = true`，禁用登录按钮并显示 DxLoadIndicator
4. 登录成功：
   - 将 Token、用户信息、角色、权限存入 authStore
   - 将 Token 存入 localStorage（给 axios 拦截器使用）
   - 如果 `RequirePasswordReset === true`，跳转到修改密码页
   - 否则跳转到 `route.query.redirect` 参数对应路径，默认 `/dashboard`
5. 登录失败：
   - 根据后端返回的错误码显示对应提示
   - `AuthInvalidCredentials` → `notifyError('用户名或密码错误')`
   - `AuthAccountDisabled` → `notifyError('账户已禁用')`
   - `AuthAccountLocked` → `notifyError('账户已锁定')`
   - 其他错误 → axios 拦截器自动处理
   - 不清空表单，方便用户重新输入
   - `logging.value = false`

### 登录按钮

| 按钮 | 文本 | 类型 | 宽度 | 行为 |
|------|------|------|------|------|
| 登录 | `$t('登录')` | `default`（type="default"） | 100% | 提交登录表单 |

- 登录中时按钮 disabled + 显示 DxLoadIndicator
- 按钮使用 `styling-mode="contained"` 突出显示

---

## 语言切换

放在登录页右上角或卡片底部。

| 值 | 显示文本 |
|:--:|---------|
| zh-CN | 简体中文 |
| en-US | English |
| ja-JP | 日本語 |
| ms-MY | Bahasa Melayu |
| zh-TW | 繁體中文 |

切换语言后：
1. 登录页所有文本实时更新
2. 语言偏好保存到 localStorage

---

## 类型定义

```typescript
// src/types/auth.ts

/** 登录请求 */
export interface LoginReqDTO {
  Username: string
  Password: string
}

/** 登录响应 */
export interface LoginRepDTO {
  Token: string
  ExpiresIn: number
  UserId: number
  Username: string
  DisplayName: string
  RequirePasswordReset: boolean
  Roles: string[]
  Permissions: string[]
  IsSuperAdmin: boolean
}

/** 当前用户信息 */
export interface CurrentUserRepDTO {
  UserId: number
  Username: string
  DisplayName: string
  IsSuperAdmin: boolean
}
```

---

## 国际化要求

### 组件级 key（放入 `LoginView.vue.{locale}.json`）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 租户管理平台 | 租户管理平台 | Tenant Management Platform | テナント管理プラットフォーム | Platform Pengurusan Penyewa | 租戶管理平台 |
| 请登录您的账号 | 请登录您的账号 | Please sign in to your account | アカウントにログインしてください | Sila log masuk ke akaun anda | 請登入您的帳號 |
| 用户名 | 用户名 | Username | ユーザー名 | Nama Pengguna | 使用者名稱 |
| 密码 | 密码 | Password | パスワード | Kata Laluan | 密碼 |
| 请输入用户名 | 请输入用户名 | Enter username | ユーザー名を入力 | Masukkan nama pengguna | 請輸入使用者名稱 |
| 请输入密码 | 请输入密码 | Enter password | パスワードを入力 | Masukkan kata laluan | 請輸入密碼 |
| 登录 | 登录 | Sign In | ログイン | Log Masuk | 登入 |
| 密码长度至少 6 个字符 | 密码长度至少 6 个字符 | Password must be at least 6 characters | パスワードは6文字以上 | Kata laluan mestilah sekurang-kurangnya 6 aksara | 密碼長度至少 6 個字元 |
| 用户名或密码错误 | 用户名或密码错误 | Invalid username or password | ユーザー名またはパスワードが正しくありません | Nama pengguna atau kata laluan tidak sah | 使用者名稱或密碼錯誤 |
| 账户已禁用 | 账户已禁用 | Account is disabled | アカウントが無効です | Akaun telah dinyahaktifkan | 帳號已停用 |
| 账户已锁定 | 账户已锁定 | Account is locked | アカウントがロックされています | Akaun telah dikunci | 帳號已鎖定 |
| 登录中 | 登录中 | Signing in... | ログイン中... | Sedang log masuk... | 登入中 |

### common key（在组件级文件中值为 `null`）

无（登录页 key 均为特有 key）

---

## 验收标准

### P0 — 功能点完整性

- [ ] 登录页为独立全屏页面（不使用 MainLayout）
- [ ] 页面包含：Logo、标题"租户管理平台"、副标题"请登录您的账号"
- [ ] 登录表单包含字段：用户名、密码
- [ ] 登录按钮存在且文本为"登录"
- [ ] 语言切换下拉存在且包含 5 种语言
- [ ] 登录成功跳转到 /dashboard（或 redirect 参数）
- [ ] 登录成功存储 Token 和用户信息到 authStore
- [ ] 登录失败显示对应错误提示

### P1 — 业务规则完整性

- [ ] **DxForm label-mode 使用 `"static"`**（零容忍，禁止 `"floating"`）
- [ ] `grep -rn 'label-mode="floating"' LoginView.vue` 结果为 0
- [ ] 用户名 `required` 验证
- [ ] 密码 `required` 验证
- [ ] 密码 `stringLength`（min: 6）验证
- [ ] 支持回车键提交
- [ ] 登录中按钮 disabled + loading 状态
- [ ] 登录失败不清空表单
- [ ] 登录失败根据错误码显示不同提示
- [ ] `RequirePasswordReset` 为 true 时跳转修改密码
- [ ] 语言切换实时更新页面文本
- [ ] 语言偏好保存到 localStorage

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建
- [ ] 5 个语言文件 key 完全一致
- [ ] 所有文本使用 $t()
- [ ] 所有验证提示使用 $t()
- [ ] 所有错误提示使用 i18n key
- [ ] 组件特有 key 在组件级语言文件中

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符（`grep -rn $'\xEF\xBF\xBD'` 结果为 0）
- [ ] Code Review 自检全部通过
- [ ] 无 fetch 调用（使用 axios）
- [ ] API 封装使用 `httpPost`
