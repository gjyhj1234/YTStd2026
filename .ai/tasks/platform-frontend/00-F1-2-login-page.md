# 子任务 F1-2 — 登录页

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F1-2 |
| 模块名称 | 登录页（响应式布局 + 滑块验证码） |
| 并行组 | — （串行，布局层） |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0011_login-page.md` |
| 后端 API 提示词 | `.ai/prompts/08-platform/backend/auth-api.md` |
| 依赖任务 | F1-1 主布局（✅ 已完成） |
| 完成会话 | `session-summary-20260413-11`（基础）、`session-summary-20260413-12`（响应式 + 验证码增强） |
| 状态 | ✅ 已完成 |

---

## 任务目标

实现独立全屏登录页面，包含：DxForm `label-mode="static"` 登录表单（用户名 + 密码）、响应式三端布局（桌面双栏/平板居中/手机全屏）、连续失败 3 次触发滑块验证码、5 种语言切换、对接 auth.logIn() API。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0011_login-page.md` — 登录页功能定义（含响应式布局规范、滑块验证码规范）
- `.ai/prompts/08-platform/backend/auth-api.md` — 认证 API 端点定义

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/types/auth.ts` | 认证类型（LoginReqDTO、LoginRepDTO、CurrentUserRepDTO） |
| `src/WebTenantPlatfrom/src/api/auth.ts` | 认证 API 封装（loginApi） |
| `src/WebTenantPlatfrom/src/views/login/LoginView.vue` | 登录页面组件 |
| `src/WebTenantPlatfrom/src/views/login/LoginView.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/views/login/LoginView.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/views/login/LoginView.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/views/login/LoginView.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/views/login/LoginView.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/styles/login.css` | 登录页样式（响应式 + 验证码） |
| `src/WebTenantPlatfrom/src/auth.ts`（修改） | 使用 LoginRepDTO 类型 |
| `src/WebTenantPlatfrom/src/router.ts`（修改） | login-form 路由指向 LoginView.vue |
| `src/WebTenantPlatfrom/e2e/tests/login/login.noauth.spec.ts` | 登录页 E2E 测试（23 个用例） |

---

## 核心功能清单

| 功能 | 说明 |
|------|------|
| DxForm label-mode | **必须使用 `"static"`**（零容忍，禁止 floating） |
| 表单字段 | Username（用户名） + Password（密码，mode: password） |
| 验证规则 | required + stringLength(min: 6) for Password |
| 回车提交 | onEnterKey 绑定到 onSubmit |
| 登录中状态 | `logging` ref → 按钮 disabled + DxLoadIndicator |
| 登录成功 | Token + 用户信息存入 authStore → 跳转 redirect 或 /dashboard |
| RequirePasswordReset | 为 true 时跳转 /change-password |
| 登录失败 | 根据 BusinessError code / HTTP status 显示精确错误提示，不清空表单 |
| 响应式布局 | 桌面端（≥1024px）双栏、平板端（768~1024px）居中、手机端（<600px）全屏 |
| 滑块验证码 | failedAttempts ≥ 3 时触发，DxEmptyItem 嵌入，鼠标+触摸双支持，92% 阈值吸附 |
| 语言切换 | 右上角 DxSelectBox，5 种语言，偏好保存 localStorage |
| 16 个 i18n key | 基础 12 + 验证码 4 |

---

## 验收标准

- [x] 登录页为独立全屏页面（不使用 MainLayout）
- [x] **DxForm label-mode="static"**（零容忍）
- [x] 表单字段：用户名 + 密码
- [x] 用户名 required、密码 required + stringLength(min:6)
- [x] 回车键提交 + 登录中 loading 状态
- [x] 登录成功存储 Token 并跳转
- [x] 登录失败根据错误码显示不同提示
- [x] RequirePasswordReset 检查
- [x] 响应式三端布局正确
- [x] 滑块验证码 3 次失败后显示
- [x] 语言切换 5 种语言
- [x] 5 个语言文件 key 完全一致（16 个 key）
- [x] `npm run build` 通过
- [x] E2E 测试编写完成（23 个用例）

---

## 已完成说明

本子任务分两轮完成：

### 第一轮（session-summary-20260413-11）— 基础登录页

1. **types/auth.ts** — LoginReqDTO、LoginRepDTO、CurrentUserRepDTO
2. **api/auth.ts** — loginApi(data) via httpPost `/auth/login`
3. **LoginView.vue** — DxForm label-mode="static"、验证规则、回车提交、登录流程
4. **styles/login.css** — 全屏渐变背景、居中卡片
5. **5 个语言文件** — 12 个 key

### 第二轮（session-summary-20260413-12）— 响应式 + 验证码增强

1. **响应式布局重构** — 桌面双栏（品牌区+登录卡片）、平板居中、手机全屏
2. **滑块验证码** — DxEmptyItem + 鼠标/触摸事件、92% 阈值吸附
3. **新增 4 个 i18n key** × 5 语言文件
4. **E2E 测试** — login.noauth.spec.ts 23 个用例（渲染 + 响应式 + 验证 + 登录流 + 验证码 + 多语言）
