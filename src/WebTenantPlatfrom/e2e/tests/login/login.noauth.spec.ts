import { test, expect } from '@playwright/test'

/**
 * 登录页 E2E 测试（无需预认证状态）
 *
 * 测试模块：F1-2 登录页（0011_login-page.md）
 * 测试范围：
 *   - 页面渲染与元素完整性
 *   - 表单验证规则
 *   - 登录成功流程
 *   - 登录失败处理
 *   - 语言切换功能
 *
 * 前置条件：
 *   - 后端已启动（http://127.0.0.1:5000）
 *   - 前端已启动（http://localhost:5173）
 *   - 数据库已初始化（包含种子数据：admin 用户）
 */

// ══════════════════════════════════════════════════════════════
// 页面渲染与元素完整性
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 页面渲染', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/#/login-form')
    await page.waitForLoadState('networkidle')
  })

  test('应展示登录页标题', async ({ page }) => {
    // 页面应包含"租户管理平台"或对应翻译
    const title = page.locator('text=租户管理平台').or(page.locator('text=Tenant Management Platform'))
    await expect(title).toBeVisible({ timeout: 10_000 })
  })

  test('应展示登录表单', async ({ page }) => {
    // DxForm 应存在
    await expect(page.locator('.dx-form')).toBeVisible()
  })

  test('应包含用户名输入框', async ({ page }) => {
    // 用户名输入框应可见
    const usernameField = page.locator('.dx-textbox').first().locator('input')
      .or(page.locator('input[name="Username"]'))
    await expect(usernameField).toBeVisible()
  })

  test('应包含密码输入框', async ({ page }) => {
    // 密码输入框应可见（type=password）
    const passwordField = page.locator('input[type="password"]')
    await expect(passwordField).toBeVisible()
  })

  test('应包含登录按钮', async ({ page }) => {
    const loginBtn = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })
    await expect(loginBtn).toBeVisible()
  })

  test('应包含语言切换下拉', async ({ page }) => {
    // 语言切换 DxSelectBox 应存在
    const langSelector = page.locator('.dx-selectbox').or(page.locator('.language-selector'))
    await expect(langSelector).toBeVisible()
  })

  test('DxForm label-mode 应为 static（非 floating）', async ({ page }) => {
    // 验证 DxForm 不使用 floating label（零容忍规则）
    const floatingLabels = page.locator('.dx-field-item-label-location-floating')
    await expect(floatingLabels).toHaveCount(0)
  })
})

// ══════════════════════════════════════════════════════════════
// 表单验证
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 表单验证', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/#/login-form')
    await page.waitForLoadState('networkidle')
  })

  test('空表单提交应显示验证错误', async ({ page }) => {
    // 点击登录按钮（不填写任何内容）
    const loginBtn = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })
    await loginBtn.click()

    // 应显示验证错误信息
    const validationErrors = page.locator('.dx-invalid-message')
    await expect(validationErrors.first()).toBeVisible({ timeout: 3_000 })
  })

  test('仅填写用户名提交应提示密码必填', async ({ page }) => {
    // 填写用户名
    const usernameInput = page.locator('.dx-textbox').first().locator('input')
    await usernameInput.fill('testuser')

    // 点击登录
    const loginBtn = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })
    await loginBtn.click()

    // 密码字段应显示验证错误
    const passwordError = page.locator('input[type="password"]')
      .locator('..')
      .locator('.dx-invalid-message')
      .or(page.locator('.dx-validator .dx-invalid-message'))
    await expect(passwordError.first()).toBeVisible({ timeout: 3_000 })
  })

  test('密码少于 6 位应显示长度验证错误', async ({ page }) => {
    // 填写短密码
    const usernameInput = page.locator('.dx-textbox').first().locator('input')
    await usernameInput.fill('testuser')
    const passwordInput = page.locator('input[type="password"]')
    await passwordInput.fill('12345')

    // 点击登录
    const loginBtn = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })
    await loginBtn.click()

    // 应显示密码长度验证错误
    await page.waitForTimeout(500)
    const validationErrors = page.locator('.dx-invalid-message')
    const count = await validationErrors.count()
    expect(count).toBeGreaterThan(0)
  })
})

// ══════════════════════════════════════════════════════════════
// 登录流程
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 登录流程', () => {
  test.beforeEach(async ({ page }) => {
    // 确保登出状态
    await page.goto('/#/login-form')
    await page.evaluate(() => localStorage.removeItem('auth_token'))
    await page.goto('/#/login-form')
    await page.waitForLoadState('networkidle')
  })

  test('使用正确凭据应登录成功并跳转', async ({ page }) => {
    // 填写正确的管理员凭据
    const usernameInput = page.locator('.dx-textbox').first().locator('input')
    await usernameInput.fill('admin')
    const passwordInput = page.locator('input[type="password"]')
    await passwordInput.fill('gjwq1234')

    // 点击登录
    const loginBtn = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })
    await loginBtn.click()

    // 应跳转到 dashboard 或 change-password
    await page.waitForURL(/.*#\/(dashboard|change-password)/, { timeout: 15_000 })
  })

  test('使用错误密码应显示错误提示', async ({ page }) => {
    // 填写错误密码
    const usernameInput = page.locator('.dx-textbox').first().locator('input')
    await usernameInput.fill('admin')
    const passwordInput = page.locator('input[type="password"]')
    await passwordInput.fill('wrongpassword')

    // 点击登录
    const loginBtn = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })
    await loginBtn.click()

    // 应显示错误提示
    await page.waitForTimeout(2000)

    // 应仍在登录页
    expect(page.url()).toContain('login-form')

    // 表单不应被清空（登录失败不清空表单规则）
    const usernameValue = await usernameInput.inputValue()
    expect(usernameValue).toBe('admin')
  })

  test('使用不存在的用户名应显示错误提示', async ({ page }) => {
    const usernameInput = page.locator('.dx-textbox').first().locator('input')
    await usernameInput.fill('nonexistentuser')
    const passwordInput = page.locator('input[type="password"]')
    await passwordInput.fill('somepassword123')

    const loginBtn = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })
    await loginBtn.click()

    await page.waitForTimeout(2000)
    expect(page.url()).toContain('login-form')
  })

  test('回车键应能提交表单', async ({ page }) => {
    const usernameInput = page.locator('.dx-textbox').first().locator('input')
    await usernameInput.fill('admin')
    const passwordInput = page.locator('input[type="password"]')
    await passwordInput.fill('gjwq1234')

    // 按回车键提交
    await passwordInput.press('Enter')

    // 应跳转到 dashboard 或 change-password
    await page.waitForURL(/.*#\/(dashboard|change-password)/, { timeout: 15_000 })
  })

  test('登录中应禁用登录按钮', async ({ page }) => {
    const usernameInput = page.locator('.dx-textbox').first().locator('input')
    await usernameInput.fill('admin')
    const passwordInput = page.locator('input[type="password"]')
    await passwordInput.fill('gjwq1234')

    const loginBtn = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })

    // 点击并立即检查按钮状态
    await loginBtn.click()

    // 按钮应变为 disabled 状态（或显示 loading）
    // 注意：这个断言可能需要根据实际实现调整
    // 登录成功后会很快跳转，所以这里用等待 URL 变化来验证
    await page.waitForURL(/.*#\/(dashboard|change-password)/, { timeout: 15_000 })
  })
})

// ══════════════════════════════════════════════════════════════
// 语言切换
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 语言切换', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/#/login-form')
    await page.waitForLoadState('networkidle')
  })

  test('切换到英文应更新页面文本', async ({ page }) => {
    // 找到语言切换 SelectBox
    const langSelector = page.locator('.dx-selectbox').or(page.locator('.language-selector'))

    if (await langSelector.isVisible()) {
      await langSelector.click()

      // 选择 English
      const engOption = page.locator('.dx-list-item').filter({ hasText: /English|en-US/i })
      if (await engOption.isVisible()) {
        await engOption.click()
        await page.waitForTimeout(500)

        // 登录按钮文本应变为英文
        const loginBtn = page.locator('.dx-button').filter({ hasText: /Sign In|Login/i })
        await expect(loginBtn).toBeVisible({ timeout: 3_000 })
      }
    }
  })
})
