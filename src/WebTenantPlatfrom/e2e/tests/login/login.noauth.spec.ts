import { test, expect, Page } from '@playwright/test'

/**
 * 登录页 E2E 测试（无需预认证状态）
 */

/** 获取登录表单中的用户名输入框 */
function getUsernameInput(page: Page) {
  return page.locator('.login-card .dx-form .dx-textbox').first().locator('input[type="text"]')
}

/** 获取密码输入框 */
function getPasswordInput(page: Page) {
  return page.locator('.login-card input[type="password"]')
}

/** 获取登录按钮 */
function getLoginBtn(page: Page) {
  return page.locator('.login-card .dx-button').filter({ hasText: /登录|Sign In|Login|ログイン|Log Masuk|登入/i })
}

/** 等待登录页加载完成 */
async function waitForLoginPage(page: Page) {
  await page.waitForLoadState('networkidle')
  await page.waitForTimeout(500)
  await expect(page.locator('.login-card')).toBeVisible({ timeout: 10_000 })
}

async function getLayoutMetrics(page: Page) {
  return page.evaluate(() => {
    const pageEl = document.querySelector('.login-page')
    const containerEl = document.querySelector('.login-container')
    const cardEl = document.querySelector('.login-card')
    const footerEl = document.querySelector('.login-footer')
    const rect = (el: Element | null) => el ? (el as HTMLElement).getBoundingClientRect() : null
    const pageRect = rect(pageEl)
    const containerRect = rect(containerEl)
    const cardRect = rect(cardEl)
    const footerStyle = footerEl ? getComputedStyle(footerEl) : null

    return {
      viewportWidth: window.innerWidth,
      viewportHeight: window.innerHeight,
      pageRect,
      containerRect,
      cardRect,
      footerDisplay: footerStyle?.display ?? null
    }
  })
}

// ══════════════════════════════════════════════════════════════
// 桌面端渲染 (1280×720)
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 桌面端渲染', () => {
  test.beforeEach(async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 720 })
    await page.goto('/#/login-form')
    await waitForLoginPage(page)
  })

  test('L01 — 应展示登录页标题', async ({ page }) => {
    await expect(page.locator('.login-title')).toBeVisible({ timeout: 10_000 })
  })

  test('L02 — 应展示登录表单', async ({ page }) => {
    await expect(page.locator('.login-card .dx-form')).toBeVisible()
  })

  test('L03 — 桌面端应展示左侧品牌区', async ({ page }) => {
    await expect(page.locator('.login-branding')).toBeVisible()
    const metrics = await getLayoutMetrics(page)
    expect(Math.round(metrics.pageRect?.width ?? 0)).toBe(1280)
    expect(Math.round(metrics.containerRect?.width ?? 0)).toBe(860)
    const centeredOffset = Math.abs(((metrics.viewportWidth - (metrics.containerRect?.width ?? 0)) / 2) - (metrics.containerRect?.left ?? 0))
    expect(centeredOffset).toBeLessThanOrEqual(2)
  })

  test('L04 — 应包含用户名输入框', async ({ page }) => {
    await expect(getUsernameInput(page)).toBeVisible()
  })

  test('L05 — 应包含密码输入框', async ({ page }) => {
    await expect(getPasswordInput(page)).toBeVisible()
  })

  test('L06 — 应包含登录按钮', async ({ page }) => {
    await expect(getLoginBtn(page)).toBeVisible()
  })

  test('L07 — 应包含语言切换下拉', async ({ page }) => {
    await expect(page.locator('.login-lang-switcher .dx-selectbox')).toBeVisible()
  })

  test('L08 — DxForm label-mode 应为 static', async ({ page }) => {
    await expect(page.locator('.dx-field-item-label-location-floating')).toHaveCount(0)
  })
})

// ══════════════════════════════════════════════════════════════
// 平板端渲染 (768×1024)
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 平板端渲染', () => {
  test('L09 — 平板端左侧品牌区应隐藏', async ({ page }) => {
    await page.setViewportSize({ width: 768, height: 1024 })
    await page.goto('/#/login-form')
    await waitForLoginPage(page)
    await expect(page.locator('.login-branding')).toBeHidden()
    await expect(page.locator('.login-card')).toBeVisible()
    const metrics = await getLayoutMetrics(page)
    expect(metrics.containerRect?.width ?? 0).toBeLessThanOrEqual(480)
    const centeredOffset = Math.abs(((metrics.viewportWidth - (metrics.containerRect?.width ?? 0)) / 2) - (metrics.containerRect?.left ?? 0))
    expect(centeredOffset).toBeLessThanOrEqual(2)
  })
})

// ══════════════════════════════════════════════════════════════
// 手机端渲染 (375×812)
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 手机端渲染', () => {
  test('L10 — 手机端登录卡片全屏展示', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 })
    await page.goto('/#/login-form')
    await waitForLoginPage(page)
    await expect(page.locator('.login-branding')).toBeHidden()
    await expect(page.locator('.login-card .dx-form')).toBeVisible()
    await expect(getLoginBtn(page)).toBeVisible()
    const metrics = await getLayoutMetrics(page)
    expect(Math.round(metrics.containerRect?.width ?? 0)).toBe(375)
    expect(Math.round(metrics.cardRect?.width ?? 0)).toBe(375)
    expect(metrics.footerDisplay).toBe('none')
  })
})

// ══════════════════════════════════════════════════════════════
// 表单验证
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 表单验证', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/#/login-form')
    await waitForLoginPage(page)
  })

  test('L11 — 空表单提交应触发验证', async ({ page }) => {
    await getLoginBtn(page).click()
    await page.waitForTimeout(500)
    // DevExtreme adds .dx-invalid class to invalid textboxes
    const invalidFields = page.locator('.login-card .dx-textbox.dx-invalid')
    const count = await invalidFields.count()
    expect(count).toBeGreaterThan(0)
  })

  test('L12 — 仅填写用户名提交应提示密码必填', async ({ page }) => {
    await getUsernameInput(page).fill('testuser')
    await getLoginBtn(page).click()
    await page.waitForTimeout(500)
    const invalidFields = page.locator('.login-card .dx-textbox.dx-invalid')
    const count = await invalidFields.count()
    expect(count).toBeGreaterThan(0)
  })

  test('L13 — 密码少于 6 位应显示长度验证错误', async ({ page }) => {
    await getUsernameInput(page).fill('testuser')
    await getPasswordInput(page).fill('12345')
    await getLoginBtn(page).click()
    await page.waitForTimeout(500)
    const invalidFields = page.locator('.login-card .dx-textbox.dx-invalid')
    const count = await invalidFields.count()
    expect(count).toBeGreaterThan(0)
  })
})

// ══════════════════════════════════════════════════════════════
// 登录流程
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 登录流程', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/#/login-form')
    await page.evaluate(() => localStorage.removeItem('auth_token'))
    await page.goto('/#/login-form')
    await waitForLoginPage(page)
  })

  test('L14 — 使用正确凭据应登录成功并跳转', async ({ page }) => {
    await getUsernameInput(page).fill('admin')
    await getPasswordInput(page).fill('gjwq1234')
    await getLoginBtn(page).click()
    await page.waitForURL(/.*#\/(dashboard|change-password)/, { timeout: 20_000 })
  })

  test('L15 — 使用错误密码应留在登录页', async ({ page }) => {
    const usernameInput = getUsernameInput(page)
    await usernameInput.fill('not-exists-user')
    await getPasswordInput(page).fill('wrongpassword')
    await getLoginBtn(page).click()
    await page.waitForTimeout(3000)
    expect(page.url()).toContain('login-form')
    expect(await usernameInput.inputValue()).toBe('not-exists-user')
  })

  test('L16 — 回车键应能提交表单', async ({ page }) => {
    await getUsernameInput(page).fill('admin')
    const pwdInput = getPasswordInput(page)
    await pwdInput.fill('gjwq1234')
    await pwdInput.press('Enter')
    await page.waitForURL(/.*#\/(dashboard|change-password)/, { timeout: 20_000 })
  })
})

// ══════════════════════════════════════════════════════════════
// 滑块验证码
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 滑块验证码', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/#/login-form')
    await page.evaluate(() => localStorage.removeItem('auth_token'))
    await page.goto('/#/login-form')
    await waitForLoginPage(page)
  })

  test('L17 — 初始状态不显示滑块验证', async ({ page }) => {
    await expect(page.locator('[data-testid="slider-captcha"]')).toHaveCount(0)
  })

  test('L18 — 连续 3 次登录失败后应显示滑块验证', async ({ page }) => {
    for (let i = 0; i < 3; i++) {
      await getUsernameInput(page).fill('not-exists-user')
      await getPasswordInput(page).fill('wrong123')
      await getLoginBtn(page).click()
      // Wait for the API call to complete and error notification
      await page.waitForTimeout(3000)
    }
    await expect(page.locator('[data-testid="slider-captcha"]')).toBeVisible({ timeout: 5_000 })
  })
})

// ══════════════════════════════════════════════════════════════
// 多语言切换
// ══════════════════════════════════════════════════════════════

test.describe('登录页 — 多语言切换', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/#/login-form')
    await waitForLoginPage(page)
  })

  test('L19 — 切换到英文', async ({ page }) => {
    await page.locator('.login-lang-switcher .dx-selectbox').click()
    await page.locator('.dx-list-item').filter({ hasText: 'English' }).click()
    await page.waitForTimeout(500)
    await expect(page.locator('.login-card .dx-button').filter({ hasText: 'Sign In' })).toBeVisible({ timeout: 3_000 })
    await expect(page.locator('.login-title')).toContainText('Tenant Management Platform')
  })

  test('L20 — 切换到日本語', async ({ page }) => {
    await page.locator('.login-lang-switcher .dx-selectbox').click()
    await page.locator('.dx-list-item').filter({ hasText: '日本語' }).click()
    await page.waitForTimeout(500)
    await expect(page.locator('.login-card .dx-button').filter({ hasText: 'ログイン' })).toBeVisible({ timeout: 3_000 })
    await expect(page.locator('.login-title')).toContainText('テナント管理プラットフォーム')
  })

  test('L21 — 切换到 Bahasa Melayu', async ({ page }) => {
    await page.locator('.login-lang-switcher .dx-selectbox').click()
    await page.locator('.dx-list-item').filter({ hasText: 'Bahasa Melayu' }).click()
    await page.waitForTimeout(500)
    await expect(page.locator('.login-card .dx-button').filter({ hasText: 'Log Masuk' })).toBeVisible({ timeout: 3_000 })
    await expect(page.locator('.login-title')).toContainText('Platform Pengurusan Penyewa')
  })

  test('L22 — 切换到繁體中文', async ({ page }) => {
    await page.locator('.login-lang-switcher .dx-selectbox').click()
    await page.locator('.dx-list-item').filter({ hasText: '繁體中文' }).click()
    await page.waitForTimeout(500)
    await expect(page.locator('.login-card .dx-button').filter({ hasText: '登入' })).toBeVisible({ timeout: 3_000 })
    await expect(page.locator('.login-title')).toContainText('租戶管理平台')
  })

  test('L23 — 切换回简体中文', async ({ page }) => {
    await page.locator('.login-lang-switcher .dx-selectbox').click()
    await page.locator('.dx-list-item').filter({ hasText: 'English' }).click()
    await page.waitForTimeout(500)
    await page.locator('.login-lang-switcher .dx-selectbox').click()
    await page.locator('.dx-list-item').filter({ hasText: '简体中文' }).click()
    await page.waitForTimeout(500)
    await expect(page.locator('.login-card .dx-button').filter({ hasText: '登录' })).toBeVisible({ timeout: 3_000 })
    await expect(page.locator('.login-title')).toContainText('租户管理平台')
  })
})
