# Playwright E2E 测试规范

## 目标

定义前端 Playwright E2E 测试的编写标准、命名约定、最佳实践和反模式，确保所有 E2E 测试一致、可靠、可维护。

---

## 适用范围

`src/WebTenantPlatfrom/e2e/` 下的所有 Playwright 测试文件。

---

## 一、技术栈

| 项目 | 选型 | 版本 |
|------|------|------|
| 测试框架 | Playwright Test | ^1.52.0 |
| 浏览器 | Chromium | 仅 Chromium |
| 语言 | TypeScript | 与项目一致 |
| 配置文件 | `src/WebTenantPlatfrom/playwright.config.ts` | — |

---

## 二、目录结构

```
src/WebTenantPlatfrom/e2e/
├── helpers/                    # 共享工具
│   ├── auth.setup.ts          # 认证设置（Playwright setup project）
│   ├── test-helpers.ts        # 页面操作工具函数
│   └── db-helpers.ts          # 数据库操作工具
├── tests/                      # 测试文件
│   ├── login/                 # 模块目录
│   │   └── login.noauth.spec.ts
│   ├── dashboard/
│   │   └── dashboard.spec.ts
│   ├── platform-users/
│   │   └── platform-users.spec.ts
│   └── ...
├── .auth/                      # 认证状态（.gitignore）
│   └── user.json
├── test-results/              # 测试结果（.gitignore）
└── playwright-report/         # HTML 报告（.gitignore）
```

---

## 三、测试文件命名规则

| 模式 | 说明 | Playwright 项目 |
|------|------|----------------|
| `{module}.spec.ts` | 需要预登录的标准测试 | `chromium`（依赖 auth-setup） |
| `{module}.noauth.spec.ts` | 不需要登录的测试 | `no-auth` |

每个前端模块对应一个测试目录和至少一个测试文件：

| 模块编号 | 模块名 | 测试目录 | 测试文件 |
|:-------:|--------|---------|---------|
| F1-2 | 登录页 | `tests/login/` | `login.noauth.spec.ts` |
| F2-1 | 仪表盘 | `tests/dashboard/` | `dashboard.spec.ts` |
| F2-2 | 平台用户管理 | `tests/platform-users/` | `platform-users.spec.ts` |
| F2-3 | 平台角色管理 | `tests/platform-roles/` | `platform-roles.spec.ts` |
| F2-4 | 平台权限管理 | `tests/platform-permissions/` | `platform-permissions.spec.ts` |
| F2-5 | 租户管理 | `tests/tenants/` | `tenants.spec.ts` |
| F2-6 | 租户信息管理 | `tests/tenant-info/` | `tenant-info.spec.ts` |
| F2-7 | 租户资源管理 | `tests/tenant-resources/` | `tenant-resources.spec.ts` |
| F2-8 | 租户配置管理 | `tests/tenant-config/` | `tenant-config.spec.ts` |
| F2-9 | 套餐管理 | `tests/packages/` | `packages.spec.ts` |
| F2-10 | 订阅管理 | `tests/subscriptions/` | `subscriptions.spec.ts` |
| F2-11 | 账单管理 | `tests/billing/` | `billing.spec.ts` |
| F2-12 | API 集成 | `tests/api-integration/` | `api-integration.spec.ts` |
| F2-13 | 审计日志 | `tests/audit/` | `audit.spec.ts` |
| F2-14 | 通知管理 | `tests/notifications/` | `notifications.spec.ts` |
| F2-15 | 文件管理 | `tests/storage/` | `storage.spec.ts` |
| F2-16 | 平台运营 | `tests/platform-operations/` | `platform-operations.spec.ts` |

---

## 四、测试编写规范

### 4.1 使用 test-helpers.ts 中的工具函数

```typescript
// ✅ 正确 — 使用共享工具函数
import { navigateTo, waitForGridLoaded, fillDxTextBox } from '../../helpers/test-helpers'
await navigateTo(page, '/platform-users')
await waitForGridLoaded(page)

// ❌ 错误 — 直接写底层操作
await page.goto('/#/platform-users')
await page.waitForSelector('.dx-datagrid')
```

### 4.2 使用 describe 分组

```typescript
// ✅ 正确 — 按功能分组
test.describe('平台用户管理 — 页面渲染', () => { ... })
test.describe('平台用户管理 — CRUD 操作', () => { ... })
test.describe('平台用户管理 — 搜索筛选', () => { ... })
test.describe('平台用户管理 — 表单验证', () => { ... })

// ❌ 错误 — 所有测试在顶层
test('test1', () => { ... })
test('test2', () => { ... })
```

### 4.3 测试用例命名

```typescript
// ✅ 正确 — 中文描述，清晰明确
test('应正确展示用户列表', async ({ page }) => { ... })
test('创建用户 — 填写完整信息应成功', async ({ page }) => { ... })
test('创建用户 — 用户名为空应显示验证错误', async ({ page }) => { ... })

// ❌ 错误 — 模糊命名
test('test create', async ({ page }) => { ... })
test('it works', async ({ page }) => { ... })
```

### 4.4 等待策略

```typescript
// ✅ 正确 — 使用语义化等待
await expect(page.locator('.dx-datagrid')).toBeVisible({ timeout: 10_000 })
await page.waitForResponse(resp => resp.url().includes('/api/platform-users'))

// ❌ 错误 — 使用固定时间等待
await page.waitForTimeout(5000)  // 仅在确实需要等待渲染时使用，且需注释原因
```

### 4.5 DevExtreme 组件交互

DevExtreme 组件的 DOM 结构与原生 HTML 不同，需要特殊处理：

```typescript
// DxTextBox 输入 — 需要定位内部 input
const input = page.locator('.dx-textbox').first().locator('input')
await input.fill('value')

// DxSelectBox 选择 — 需要点击打开下拉再选择
const selectBox = page.locator('.dx-selectbox').first()
await selectBox.click()
const option = page.locator('.dx-list-item').filter({ hasText: '选项文本' })
await option.click()

// DxButton 点击 — 通过文本过滤
const button = page.locator('.dx-button').filter({ hasText: '保存' })
await button.click()

// DxDataGrid 行操作 — 通过行索引和操作文本
const row = page.locator('.dx-data-row').nth(0)
const editBtn = row.locator('.dx-link').filter({ hasText: '编辑' })
await editBtn.click()
```

### 4.6 断言风格

```typescript
// ✅ 正确 — 使用 Playwright 内置断言（自带重试）
await expect(page.locator('.page-title')).toHaveText('用户管理')
await expect(page.locator('.dx-data-row')).toHaveCount(10)
await expect(page.locator('.error-message')).toBeVisible()

// ❌ 错误 — 使用非重试断言
const text = await page.locator('.page-title').textContent()
expect(text).toBe('用户管理')  // 没有重试机制，可能因时序失败
```

---

## 五、测试覆盖标准

每个业务模块的测试**必须覆盖**以下场景：

### 5.1 页面渲染（必须）

- [ ] 页面标题正确展示
- [ ] DxDataGrid 存在且加载数据
- [ ] 工具栏按钮可见（新增、搜索等）
- [ ] 分页器可见（如有）

### 5.2 列表展示（必须）

- [ ] 数据行正确渲染
- [ ] 列头与设计一致（使用 $t() 的 caption）
- [ ] 空数据时显示空状态提示

### 5.3 创建操作（必须）

- [ ] 点击新增按钮打开弹窗/表单
- [ ] 填写有效数据提交成功
- [ ] 提交后列表刷新显示新数据
- [ ] 成功通知消息展示

### 5.4 编辑操作（必须）

- [ ] 点击编辑按钮打开弹窗/表单
- [ ] 表单正确回填现有数据
- [ ] 修改数据提交成功
- [ ] 提交后列表刷新显示更新数据

### 5.5 删除操作（必须）

- [ ] 点击删除按钮弹出确认对话框
- [ ] 确认删除后记录消失
- [ ] 取消删除记录保留

### 5.6 表单验证（必须）

- [ ] 必填字段为空时显示验证错误
- [ ] 格式验证（如邮箱、手机号）
- [ ] 唯一性冲突提示（如用户名已存在）

### 5.7 搜索/筛选（必须）

- [ ] 输入关键字搜索，列表正确筛选
- [ ] 清空搜索条件，列表恢复

---

## 六、反模式清单

| 编号 | 反模式 | 正确做法 |
|:----:|--------|---------|
| E1 | 硬编码等待时间 `waitForTimeout(5000)` | 使用 `expect().toBeVisible()` 或 `waitForResponse()` |
| E2 | 直接操作 DOM 属性而非使用 Playwright API | 使用 `locator.fill()`、`locator.click()` |
| E3 | 测试之间有隐式数据依赖 | 每个 describe 块独立，使用 beforeEach 准备数据 |
| E4 | 在测试中写复杂的页面操作逻辑 | 提取到 test-helpers.ts 中 |
| E5 | 删除失败的测试 | 修复前端代码或修正测试逻辑 |
| E6 | 测试名使用英文或模糊描述 | 使用中文描述，清晰明确 |
| E7 | 不等待 API 响应就断言 | 使用 `waitForResponse()` 或 `networkidle` |
| E8 | 忽略 DevExtreme 组件的特殊 DOM 结构 | 使用 test-helpers.ts 中的工具函数 |

---

## 七、与 CI 集成

测试在 Agent 环境中运行，不需要额外 CI 配置。运行命令：

```bash
# 在项目目录下
cd src/WebTenantPlatfrom

# 运行全部测试
npx playwright test

# 运行指定模块
npx playwright test e2e/tests/{module}/

# 运行无需认证的测试
npx playwright test --project=no-auth

# 运行需要认证的测试
npx playwright test --project=chromium
```

---

## 版本

- 版本：1.0
- 创建日期：2026-04-13
- 创建原因：建立 Playwright E2E 测试编写标准，确保前端功能验证的一致性和完整性
