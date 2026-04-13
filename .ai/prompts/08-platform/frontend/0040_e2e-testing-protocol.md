# 租户平台 — E2E 测试协议

> 本文件定义每个前端模块的 E2E 测试要点、前置条件和验收标准。
> Agent 在完成前端模块编码后，必须按此协议编写并执行 Playwright 测试。

---

## 一、前置阅读

执行 E2E 测试前，必须先阅读：

| 序号 | 文件 | 用途 |
|:----:|------|------|
| 1 | `.ai/system/e2e-testing-workflow.md` | 环境检查与迭代工作流 |
| 2 | `.ai/prompts/03-frontend/08-playwright-e2e.md` | Playwright 测试规范 |
| 3 | 本文件 | 各模块测试要点 |
| 4 | 对应模块的前端提示词（如 `0021_platform-user-page.md`） | 功能需求 |

---

## 二、测试执行流程（每个模块）

Agent 在完成一个前端模块后，**必须**按以下步骤执行：

### 步骤 1：环境预检

按 `.ai/system/e2e-testing-workflow.md` 第一节执行环境检查。

### 步骤 2：编写测试

在 `e2e/tests/{模块名}/` 下创建测试文件，覆盖本文件定义的该模块所有测试要点。

### 步骤 3：运行测试

```bash
cd src/WebTenantPlatfrom && npx playwright test e2e/tests/{模块名}/
```

### 步骤 4：分析结果并迭代

- 测试全部通过 → 继续 self-review-protocol
- 有失败 → 分析原因、修复代码、重新运行（最多 5 次迭代）

### 步骤 5：记录结果

在会话总结中记录测试结果。

---

## 二.5、通用测试维度（每个模块都必须覆盖）

Agent 在编写每个模块的 E2E 测试时，除了该模块特有的功能测试外，**必须额外覆盖以下通用测试维度**：

### 维度 1：响应式布局验证（桌面/平板/手机）

每个页面必须在 3 种视口尺寸下验证可用性：

| 视口 | 分辨率 | 验证要点 |
|------|:------:|---------|
| 桌面端 | 1280×720 | 完整布局展示、侧边栏可见、所有功能可操作 |
| 平板端 | 768×1024 | 布局自适应、核心功能可操作、无溢出 |
| 手机端 | 375×812 | 全屏布局、表单可填写、按钮可点击、无截断 |

**测试模板：**

```typescript
test.describe('页面名 — 桌面端', () => {
  test.beforeEach(async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 720 })
    // ... navigate
  })
  // 桌面端特有验证
})

test.describe('页面名 — 平板端', () => {
  test('平板端页面可用', async ({ page }) => {
    await page.setViewportSize({ width: 768, height: 1024 })
    // 验证核心元素可见、可操作
  })
})

test.describe('页面名 — 手机端', () => {
  test('手机端页面可用', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 })
    // 验证核心元素可见、可操作
  })
})
```

### 维度 2：多语言切换验证（5 种语言）

每个页面必须验证切换语言后，**前端 UI 文本**（页面标题、按钮文字、表单标签等）正确翻译。

**注意：** 数据库值（如用户名、租户名等后端返回数据）不需要验证翻译。仅验证 `$t()` 绑定的前端文本。

| 语言 | locale | 验证要点 |
|------|--------|---------|
| 简体中文 | zh-CN | 默认语言，页面标题、按钮文字为中文 |
| English | en-US | 页面标题、按钮文字为英文 |
| 日本語 | ja-JP | 页面标题、按钮文字为日文 |
| Bahasa Melayu | ms-MY | 页面标题、按钮文字为马来文 |
| 繁體中文 | zh-TW | 页面标题、按钮文字为繁体中文 |

**测试模板：**

```typescript
test.describe('页面名 — 多语言切换', () => {
  for (const lang of [
    { locale: 'en-US', title: 'Dashboard', btn: 'Sign In' },
    { locale: 'ja-JP', title: 'ダッシュボード', btn: 'ログイン' },
    { locale: 'ms-MY', title: 'Papan Pemuka', btn: 'Log Masuk' },
    { locale: 'zh-TW', title: '儀表盤', btn: '登入' },
  ]) {
    test(`切换到 ${lang.locale}`, async ({ page }) => {
      // 通过语言切换器或 localStorage 切换语言
      // 验证页面标题和关键按钮文本已翻译
    })
  }
})
```

---

## 三、各模块测试要点

### F1-2 登录页（`tests/login/login.noauth.spec.ts`）

**已实现 ✅** — 响应式 + 验证码 + 多语言全覆盖。

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| L01 | 页面标题 | 标题".login-title"可见 |
| L02 | 表单元素 | DxForm、用户名、密码输入框、登录按钮可见 |
| L03 | 桌面端品牌区 | 桌面端(1280×720)左侧品牌区`.login-branding`可见 |
| L04 | 用户名输入框 | 使用 `.login-card .dx-form .dx-textbox input[type="text"]` 定位 |
| L05 | 密码输入框 | `.login-card input[type="password"]` 可见 |
| L06 | 登录按钮 | `.login-card .dx-button` 过滤多语言文本 |
| L07 | 语言切换 | `.login-lang-switcher .dx-selectbox` 可见 |
| L08 | label-mode | DxForm 使用 static（无 `.dx-field-item-label-location-floating`） |
| L09 | 平板端响应 | 768×1024 下品牌区隐藏，登录卡片居中 |
| L10 | 手机端响应 | 375×812 下全屏展示，表单和按钮可见 |
| L11 | 空表单验证 | 提交后 `.dx-textbox.dx-invalid` 出现（注意：不是检查 `.dx-invalid-message` 可见性） |
| L12 | 密码必填 | 仅填用户名提交，密码框出现 `.dx-invalid` |
| L13 | 密码长度 | 密码少于 6 位提交后出现验证错误 |
| L14 | 正确凭据 | admin/gjwq1234 登录成功跳转 dashboard 或 change-password |
| L15 | 错误凭据 | 错误密码留在登录页，不清空表单 |
| L16 | 回车键提交 | 密码框按回车触发登录 |
| L17 | 验证码初始 | 初始状态不显示滑块验证 `[data-testid="slider-captcha"]` |
| L18 | 验证码触发 | 连续 3 次登录失败后显示滑块验证 |
| L19-L23 | 多语言 | 分别切换到 en-US / ja-JP / ms-MY / zh-TW / zh-CN，验证标题和按钮文本 |

**前置条件：** 无（不需要预登录）
**数据库要求：** 需要种子数据中的 admin 用户

---

### F2-1 仪表盘（`tests/dashboard/dashboard.spec.ts`）

**已实现 ✅** — 页面渲染 + 统计卡片 + 图表 + 快捷操作 + 侧边栏 + 多语言全覆盖。

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| D01 | 默认跳转 | 登录后默认跳转到 `/#/dashboard` |
| D02 | 页面标题 | `[data-testid="dashboard-title"]` 可见且有文本 |
| D02b | 页面副标题 | `[data-testid="dashboard-subtitle"]` 可见且有文本 |
| D03a | 统计卡片数 | `[data-testid="stat-cards"] .stat-card` 有 4 个 |
| D03b-e | 各卡片可见 | active-users / new-users / api-calls / storage 4 个卡片 |
| D03f | 卡片图标 | 每个卡片有 `.stat-icon i` 图标 |
| D03g | 卡片数值 | 每个卡片有 `.stat-value` 且有文本内容 |
| D03h-j | 图表容器 | chart-active-users / chart-api-calls / chart-metrics 3 个图表容器可见 |
| D03k-l | 快捷操作 | `[data-testid="quick-actions"]` 可见，有 `.section-title` |
| D04 | 侧边栏 | `.dx-treeview` 可见 |
| D05 | 菜单跳转 | 点击菜单项可跳转 |
| D06a | 中文标题 | locale=zh-CN 时标题正确 |
| D06b | 英文标题 | locale=en-US 时标题正确 |

**前置条件：** 已登录
**数据库要求：** 种子数据即可

---

### F2-2 平台用户管理（`tests/platform-users/platform-users.spec.ts`）

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| U01 | 页面渲染 | 导航到 /platform-users，页面标题正确 |
| U02 | 用户列表 | DxDataGrid 展示用户数据（至少有种子管理员） |
| U03 | 列头检查 | 列头包含：用户名、显示名、邮箱、状态等 |
| U04 | 新增用户 | 点击新增 → 填写表单 → 提交成功 → 列表刷新 |
| U05 | 用户名唯一 | 创建重复用户名显示"已存在"错误 |
| U06 | 编辑用户 | 点击编辑 → 表单回填 → 修改 → 提交成功 |
| U07 | 禁用用户 | 操作后状态变更 |
| U08 | 启用用户 | 操作后状态变更 |
| U09 | 重置密码 | 重置密码操作（按钮可见、确认弹窗） |
| U10 | 搜索功能 | 输入关键字搜索用户 |
| U11 | 必填验证 | 用户名/密码为空时显示验证错误 |

**前置条件：** 已登录
**数据库要求：** 种子数据（admin 用户）

---

### F2-3 平台角色管理（`tests/platform-roles/platform-roles.spec.ts`）

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| R01 | 页面渲染 | 导航到 /platform-roles，页面标题正确 |
| R02 | 角色列表 | DxDataGrid 展示角色数据（至少有种子角色） |
| R03 | 新增角色 | 填写角色名称、编码 → 提交成功 |
| R04 | 角色编码唯一 | 创建重复编码显示"已存在"错误 |
| R05 | 编辑角色 | 表单回填 → 修改 → 提交成功 |
| R06 | 分配权限 | 打开权限分配弹窗，勾选权限，保存 |
| R07 | 分配成员 | 打开成员分配弹窗，添加/移除用户 |
| R08 | 删除角色 | 确认弹窗 → 删除成功 |
| R09 | 启用/禁用 | 状态切换操作 |

**前置条件：** 已登录，权限数据已存在
**数据库要求：** 种子数据

---

### F2-4 平台权限管理（`tests/platform-permissions/platform-permissions.spec.ts`）

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| P01 | 页面渲染 | 导航到 /platform-permissions，展示权限树 |
| P02 | 权限列表 | 权限数据正确展示（树形或列表） |
| P03 | 新增权限 | 填写权限名称、编码 → 提交成功 |
| P04 | 编辑权限 | 修改权限信息 → 提交成功 |
| P05 | 删除权限 | 确认弹窗 → 删除成功 |

**前置条件：** 已登录
**数据库要求：** 种子权限数据

---

### F2-5 租户管理（`tests/tenants/tenants.spec.ts`）

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| T01 | 页面渲染 | 导航到 /tenants，DxDataGrid 展示租户列表 |
| T02 | 创建租户 | 填写租户信息（名称、编码、类型等）→ 提交成功 |
| T03 | 租户编码唯一 | 创建重复编码显示"已存在"错误 |
| T04 | 编辑租户 | 表单回填 → 修改 → 提交成功 |
| T05 | 租户状态变更 | 启用/禁用/冻结操作 |
| T06 | 租户详情 | 查看租户详细信息 |
| T07 | 搜索筛选 | 按名称/状态搜索租户 |

**前置条件：** 已登录
**数据库要求：** 种子数据即可

---

### F2-6 租户信息管理（`tests/tenant-info/tenant-info.spec.ts`）

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| TI01 | 页面渲染 | 导航到 /tenant-info，包含分组、域名、标签标签页 |
| TI02 | 租户分组 | CRUD 操作（创建/编辑/删除分组） |
| TI03 | 分组树形展示 | 分组以树形结构展示 |
| TI04 | 租户域名 | CRUD 操作（绑定/解绑域名） |
| TI05 | 租户标签 | CRUD 操作（创建/编辑/删除标签） |
| TI06 | 标签绑定 | 将标签绑定到租户 |

**前置条件：** 已登录，至少有 1 个租户
**数据库要求：** 需要先创建租户数据

---

### F2-7 租户资源管理（`tests/tenant-resources/tenant-resources.spec.ts`）

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| TR01 | 页面渲染 | 导航到 /tenant-resources，DxDataGrid 展示资源配额 |
| TR02 | 设置配额 | 为租户设置资源配额（存储、API 调用数等） |
| TR03 | 修改配额 | 修改已有配额 |
| TR04 | 使用统计 | 查看资源使用统计 |

**前置条件：** 已登录，至少有 1 个租户
**数据库要求：** 需要先创建租户数据

---

### F2-8 租户配置管理（`tests/tenant-config/tenant-config.spec.ts`）

**测试要点：**

| 编号 | 测试场景 | 验证点 |
|:----:|---------|--------|
| TC01 | 页面渲染 | 导航到 /tenant-config，包含系统配置、功能开关、参数标签页 |
| TC02 | 系统配置 | 查看/修改系统配置 |
| TC03 | 功能开关 | 开启/关闭功能开关 |
| TC04 | 参数管理 | CRUD 操作（创建/编辑/删除参数） |

**前置条件：** 已登录
**数据库要求：** 种子数据

---

### F2-9 至 F2-16（套餐/订阅/账单/API 集成/审计/通知/文件/平台运营）

这些模块的测试要点遵循相同模式：

1. **页面渲染** — 导航到对应路由，验证页面标题和主要元素
2. **数据列表** — DxDataGrid 正确加载和展示数据
3. **CRUD 操作** — 创建、编辑、删除（按模块需求）
4. **搜索筛选** — 关键字搜索、状态筛选
5. **表单验证** — 必填字段、格式验证

**依赖链提醒：**
- 订阅管理 → 依赖租户 + 套餐
- 账单管理 → 依赖订阅
- API 集成 → 依赖租户
- 租户信息/资源/配置 → 依赖租户

---

## 四、测试执行优先级

| 优先级 | 模块 | 说明 |
|:-----:|------|------|
| P0 | 登录页 | 所有功能的入口 |
| P0 | 仪表盘 | 登录后首页 |
| P1 | 平台用户/角色/权限 | 核心管理功能 |
| P1 | 租户管理 | 核心业务功能 |
| P2 | 租户信息/资源/配置 | 依赖租户数据 |
| P2 | 套餐管理 | 独立模块 |
| P3 | 订阅/账单 | 依赖链较长 |
| P3 | 其他模块 | 辅助功能 |

---

## 五、数据库重置策略

### 何时需要全量重置

```
以下任一情况需要全量重置：
1. 测试模块的 CRUD 操作产生了无法预测的脏数据
2. 上一轮测试中断导致数据状态不确定
3. 需要测试的模块依赖其他模块的干净种子数据
4. 后端代码修改了实体结构或种子数据逻辑
```

### 重置后的依赖操作链

如果某模块依赖前置数据，重置后需要按以下顺序重建：

```bash
# 1. 重置数据库 + 重启后端（自动种子数据）
# 2. 如需租户数据：通过 API 创建测试租户
# 3. 如需套餐数据：通过 API 创建测试套餐
# 4. 如需订阅数据：通过 API 创建测试订阅
# 5. 运行目标模块测试
```

**推荐方式：** 在测试文件的 `test.beforeAll` 中通过 API 创建前置数据，而非通过 UI 操作。

---

## 六、会话总结中的 E2E 报告模板

```markdown
## E2E 测试结果

### 测试概要

| 指标 | 值 |
|------|---|
| 总测试数 | N |
| 通过 | N |
| 失败 | 0 |
| 跳过 | 0 |
| 迭代次数 | M |

### 测试覆盖

| 模块 | 文件 | 测试数 | 状态 |
|------|------|:------:|:----:|
| 登录页 | `login.noauth.spec.ts` | X | ✅ |
| {当前模块} | `{module}.spec.ts` | Y | ✅ |

### 迭代记录（如有）

| 迭代 | 失败数 | 失败原因 | 修复内容 |
|:----:|:------:|---------|---------|
| 1 | 3 | DxTextBox 定位错误 | 修改 test-helpers 定位逻辑 |
| 2 | 1 | API 返回格式不匹配 | 修复前端 DTO 字段名 |
| 3 | 0 | — | — |
```

---

## 版本

- 版本：1.1
- 更新日期：2026-04-13
- 更新内容：v1.1 新增通用测试维度（响应式+多语言）、DevExtreme 定位陷阱、调试经验总结
- 创建日期：2026-04-13
- 创建原因：为每个前端模块定义 E2E 测试的验收标准，实现自动化的功能验证闭环

---

## 附录 A：DevExtreme 组件 Playwright 定位陷阱与经验总结

> **目的**：记录历次 E2E 测试迭代中发现的 DevExtreme 组件定位问题与正确解决方案，避免后续 Agent 重复踩坑。
> **每次 Agent 调试 E2E 测试时，必须先阅读本节，在遇到定位问题时优先查阅此处的解决方案。**

### A.1 DxTextBox 定位：避开 DxSelectBox 的隐藏 input

**问题描述**：使用 `.dx-textbox` 定位时，DxSelectBox 也是 DxTextBox 的子类，会产生多个匹配。特别是 DxSelectBox 内部有一个 `<input type="hidden">` 存储选中值，导致 `locator('.dx-textbox').first().locator('input')` 解析为多个元素（hidden input + 可见 input）。

**❌ 错误定位（会 resolve 到多个元素）**：

```typescript
// DxSelectBox 的 hidden input 也会被匹配
page.locator('.dx-textbox').first().locator('input')
```

**✅ 正确定位**：

```typescript
// 方案 1：限定容器 + 指定 input type
page.locator('.login-card .dx-form .dx-textbox').first().locator('input[type="text"]')

// 方案 2：用密码框的 type 天然唯一
page.locator('.login-card input[type="password"]')

// 方案 3：用 data-field 属性定位（DxForm SimpleItem）
page.locator('[data-field="Username"] input')
```

### A.2 DxForm 验证错误检测：使用 `.dx-invalid` 而非 `.dx-invalid-message`

**问题描述**：DevExtreme 的验证错误消息 (`.dx-invalid-message`) 是通过 overlay 机制渲染的，默认为 `visibility: hidden`，只有在 hover/focus 时才显示。因此 Playwright 的 `toBeVisible()` 对 `.dx-invalid-message` 经常返回 false。

**❌ 错误断言**：

```typescript
// overlay 默认 hidden，即使表单验证失败也会断言失败
const errors = page.locator('.dx-invalid-message')
await expect(errors.first()).toBeVisible()
```

**✅ 正确断言**：

```typescript
// 检查 .dx-textbox 是否带有 .dx-invalid 类（不依赖 overlay 可见性）
const invalidFields = page.locator('.login-card .dx-textbox.dx-invalid')
const count = await invalidFields.count()
expect(count).toBeGreaterThan(0)
```

### A.3 DxButton 定位：多语言按钮必须使用正则匹配

**问题描述**：登录按钮文本随语言切换变化（登录/Sign In/ログイン/Log Masuk/登入），用硬编码文本匹配在非中文环境下会失败。

**✅ 正确定位**：

```typescript
// 用正则匹配所有语言的按钮文本
page.locator('.login-card .dx-button')
  .filter({ hasText: /登录|Sign In|Login|ログイン|Log Masuk|登入/i })
```

### A.4 后端启动：必须使用 `--no-launch-profile` 和明确端口

**问题描述**：`dotnet run --project src/YTStdTenantPlatform` 默认使用 launchSettings.json 中的端口配置，可能不是 5000。E2E 测试和 Vite proxy 都依赖 `http://127.0.0.1:5000`。

**✅ 正确启动命令**：

```bash
ASPNETCORE_URLS="http://0.0.0.0:5000" dotnet run --project src/YTStdTenantPlatform -c Release --no-launch-profile &
# 等待 health check 通过
for i in $(seq 1 30); do
  curl -s http://127.0.0.1:5000/api/health/ | grep -q '"Code":0' && break
  sleep 2
done
```

### A.5 auth.setup.ts：检查 loginResponse.ok() 前确认后端连通

**问题描述**：如果后端在长时间测试运行过程中崩溃或重启，auth.setup.ts 的 `request.post('/api/auth/login')` 会因连接拒绝而失败，导致所有依赖 auth-setup 的测试全部跳过。

**建议**：auth.setup.ts 应在登录前先做一次 health check，确认后端可达。

### A.6 DxSelectBox 语言切换：使用 `.dx-list-item` 过滤文本

**问题描述**：DxSelectBox 的下拉列表在 DOM 中使用 `.dx-list-item` 渲染（不在 DxSelectBox 容器内，而是在 body 层的 overlay 中）。

**✅ 正确操作**：

```typescript
// 1. 点击 DxSelectBox 打开下拉
await page.locator('.login-lang-switcher .dx-selectbox').click()
// 2. 在全局 overlay 中按文本过滤选项
await page.locator('.dx-list-item').filter({ hasText: 'English' }).click()
// 3. 等待语言切换生效
await page.waitForTimeout(500)
```

### A.7 waitForTimeout 使用建议

| 场景 | 推荐等待时间 | 说明 |
|------|:----------:|------|
| 语言切换后 | 500ms | vue-i18n 响应式更新需要时间 |
| 登录 API 调用后 | 2000-3000ms | 网络请求 + 路由跳转 |
| 页面导航后 | 1000ms | Vue 组件渲染 + API 数据加载 |
| DxForm 验证后 | 500ms | 验证结果渲染到 DOM |

### A.8 后端长时间运行可能崩溃

**问题描述**：在 CI 环境中，后端使用 `nohup dotnet run ... &` 后台运行。如果 E2E 测试运行时间超过后端进程存活时间（或被 OOM killer 终止），后续测试全部失败。

**建议**：
1. 测试开始前始终执行环境预检
2. 长时间运行的测试套件中间检查后端是否存活
3. 如发现后端崩溃，先重启后端再继续测试
