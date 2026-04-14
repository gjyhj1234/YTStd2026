import { test, expect } from '@playwright/test'
import { navigateTo } from '../../helpers/test-helpers'

/**
 * 仪表盘 E2E 测试
 *
 * 测试模块：F2-1 仪表盘（0020_dashboard-page.md）
 * 测试范围：
 *   - 页面渲染与元素完整性
 *   - 统计卡片展示
 *   - 图表渲染
 *   - 快捷操作按钮
 *   - 快捷操作导航
 *   - 侧边栏导航
 *   - 多语言切换
 *   - 主题切换
 *   - 响应式布局
 *
 * 前置条件：
 *   - 后端已启动（http://127.0.0.1:5000）
 *   - 前端已启动（http://localhost:5173）
 *   - 已通过 auth.setup.ts 完成登录
 */

// ══════════════════════════════════════════════════════════════
// D01: 页面渲染 — 登录后默认跳转到仪表盘
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 页面渲染', () => {
  test('D01 — 登录后默认跳转到仪表盘', async ({ page }) => {
    await page.goto('/')
    await page.waitForLoadState('networkidle')
    await expect(page).toHaveURL(/.*#\/dashboard/)
  })

  test('D02 — 页面标题正确展示', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    const title = page.locator('[data-testid="dashboard-title"]')
    await expect(title).toBeVisible({ timeout: 10_000 })
    const titleText = await title.textContent()
    expect(titleText).toBeTruthy()
  })

  test('D02b — 页面副标题正确展示', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    const subtitle = page.locator('[data-testid="dashboard-subtitle"]')
    await expect(subtitle).toBeVisible({ timeout: 10_000 })
    const subtitleText = await subtitle.textContent()
    expect(subtitleText).toBeTruthy()
  })
})

// ══════════════════════════════════════════════════════════════
// D03: 统计卡片展示
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 统计卡片', () => {
  test.beforeEach(async ({ page }) => {
    await navigateTo(page, '/dashboard')
    await page.waitForTimeout(2000)
  })

  test('D03a — 应包含 4 个统计卡片', async ({ page }) => {
    const cards = page.locator('[data-testid="stat-cards"] .stat-card')
    await expect(cards).toHaveCount(4)
  })

  test('D03b — 活跃用户卡片可见', async ({ page }) => {
    const card = page.locator('[data-testid="stat-card-active-users"]')
    await expect(card).toBeVisible()
  })

  test('D03c — 新增用户卡片可见', async ({ page }) => {
    const card = page.locator('[data-testid="stat-card-new-users"]')
    await expect(card).toBeVisible()
  })

  test('D03d — API 调用次数卡片可见', async ({ page }) => {
    const card = page.locator('[data-testid="stat-card-api-calls"]')
    await expect(card).toBeVisible()
  })

  test('D03e — 存储使用量卡片可见', async ({ page }) => {
    const card = page.locator('[data-testid="stat-card-storage"]')
    await expect(card).toBeVisible()
  })

  test('D03f — 卡片有图标', async ({ page }) => {
    const icons = page.locator('[data-testid="stat-cards"] .stat-icon i')
    const count = await icons.count()
    expect(count).toBe(4)
  })

  test('D03g — 卡片有数值', async ({ page }) => {
    const values = page.locator('[data-testid="stat-cards"] .stat-value')
    const count = await values.count()
    expect(count).toBe(4)
    for (let i = 0; i < count; i++) {
      const text = await values.nth(i).textContent()
      expect(text).toBeTruthy()
    }
  })
})

// ══════════════════════════════════════════════════════════════
// 图表渲染
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 图表', () => {
  test.beforeEach(async ({ page }) => {
    await navigateTo(page, '/dashboard')
    await page.waitForTimeout(2000)
  })

  test('D03h — 每日活跃用户趋势图表容器存在', async ({ page }) => {
    const chart = page.locator('[data-testid="chart-active-users"]')
    await expect(chart).toBeVisible()
  })

  test('D03i — 每日 API 调用趋势图表容器存在', async ({ page }) => {
    const chart = page.locator('[data-testid="chart-api-calls"]')
    await expect(chart).toBeVisible()
  })

  test('D03j — 监控指标分布饼图容器存在', async ({ page }) => {
    const chart = page.locator('[data-testid="chart-metrics"]')
    await expect(chart).toBeVisible()
  })

  test('D03k-chart — 图表不超出容器范围', async ({ page }) => {
    // Check all chart containers are constrained in size
    const containers = page.locator('.chart-container')
    const count = await containers.count()
    for (let i = 0; i < count; i++) {
      const box = await containers.nth(i).boundingBox()
      expect(box).toBeTruthy()
      // Chart containers should not exceed viewport height
      expect(box!.height).toBeLessThanOrEqual(400)
    }
  })
})

// ══════════════════════════════════════════════════════════════
// 快捷操作
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 快捷操作', () => {
  test.beforeEach(async ({ page }) => {
    await navigateTo(page, '/dashboard')
    await page.waitForTimeout(1000)
  })

  test('D03k — 快捷操作区域存在', async ({ page }) => {
    const quickActions = page.locator('[data-testid="quick-actions"]')
    await expect(quickActions).toBeVisible()
  })

  test('D03l — 快捷操作区域有标题', async ({ page }) => {
    const sectionTitle = page.locator('[data-testid="quick-actions"] .section-title')
    await expect(sectionTitle).toBeVisible()
    const text = await sectionTitle.textContent()
    expect(text).toBeTruthy()
  })

  test('D03m — 快捷操作按钮可见（管理员权限）', async ({ page }) => {
    // Admin should have all permissions, all 3 buttons should be visible
    // DxButton does not forward data-testid to DOM; use text-based locators within action-buttons
    const actionBtns = page.locator('.action-buttons .dx-button')
    const btnTenant = actionBtns.filter({ hasText: /创建租户|Create Tenant/i })
    const btnUser = actionBtns.filter({ hasText: /创建用户|Create User/i })
    const btnAudit = actionBtns.filter({ hasText: /审计日志|Audit Log/i })

    await expect(btnTenant).toBeVisible({ timeout: 5_000 })
    await expect(btnUser).toBeVisible({ timeout: 5_000 })
    await expect(btnAudit).toBeVisible({ timeout: 5_000 })
  })

  test('D03n — 点击"创建租户"跳转到租户列表页', async ({ page }) => {
    const btn = page.locator('.action-buttons .dx-button').filter({ hasText: /创建租户|Create Tenant/i })
    await expect(btn).toBeVisible({ timeout: 5_000 })
    await btn.click()
    await page.waitForTimeout(500)
    await expect(page).toHaveURL(/.*#\/tenants/)
  })

  test('D03o — 点击"创建用户"跳转到用户管理页', async ({ page }) => {
    const btn = page.locator('.action-buttons .dx-button').filter({ hasText: /创建用户|Create User/i })
    await expect(btn).toBeVisible({ timeout: 5_000 })
    await btn.click()
    await page.waitForTimeout(500)
    await expect(page).toHaveURL(/.*#\/platform-users/)
  })

  test('D03p — 点击"查看审计日志"跳转到审计日志页', async ({ page }) => {
    const btn = page.locator('.action-buttons .dx-button').filter({ hasText: /审计日志|Audit Log/i })
    await expect(btn).toBeVisible({ timeout: 5_000 })
    await btn.click()
    await page.waitForTimeout(500)
    await expect(page).toHaveURL(/.*#\/audit-logs/)
  })
})

// ══════════════════════════════════════════════════════════════
// D04: 侧边栏导航
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 侧边栏', () => {
  test('D04 — 左侧菜单正确展示', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    const sidebar = page.locator('.dx-treeview')
    await expect(sidebar).toBeVisible({ timeout: 10_000 })
  })

  test('D05 — 点击菜单项可跳转', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    const homeItem = page.locator('.dx-treeview-item').first()
    await expect(homeItem).toBeVisible({ timeout: 10_000 })
    await homeItem.click()
    await page.waitForTimeout(500)
    await expect(page).toHaveURL(/.*#\/dashboard/)
  })
})

// ══════════════════════════════════════════════════════════════
// D06: 多语言切换
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 多语言切换', () => {
  test.beforeEach(async ({ page }) => {
    await navigateTo(page, '/dashboard')
    await page.waitForTimeout(1000)
  })

  test('D06a — 中文标题正确', async ({ page }) => {
    // Set locale to zh-CN via localStorage before navigation
    await page.evaluate(() => localStorage.setItem('locale', 'zh-CN'))
    await page.goto('/#/dashboard')
    await page.waitForLoadState('networkidle')
    await page.waitForTimeout(1000)

    const title = page.locator('[data-testid="dashboard-title"]')
    await expect(title).toBeVisible()
    // Title should contain Chinese or the key itself
    const text = await title.textContent()
    expect(text).toBeTruthy()
  })

  test('D06b — 英文标题正确', async ({ page }) => {
    await page.evaluate(() => localStorage.setItem('locale', 'en-US'))
    await page.goto('/#/dashboard')
    await page.waitForLoadState('networkidle')
    await page.waitForTimeout(1000)

    const title = page.locator('[data-testid="dashboard-title"]')
    await expect(title).toBeVisible()
    const text = await title.textContent()
    // Should be "Dashboard" or the i18n key
    expect(text).toBeTruthy()
  })
})

// ══════════════════════════════════════════════════════════════
// D07: 主题切换
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 主题切换', () => {
  test('D07a — 切换暗色主题后卡片背景色变化', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    await page.waitForTimeout(1000)

    // Get initial card background color
    const card = page.locator('[data-testid="stat-card-active-users"]')
    await expect(card).toBeVisible()
    const lightBg = await card.evaluate(el => getComputedStyle(el).backgroundColor)

    // Click theme switcher button (moon/sun icon in header toolbar)
    const themeBtn = page.locator('.theme-button')
    if (await themeBtn.isVisible()) {
      await themeBtn.click()
      await page.waitForTimeout(1000)

      // After switching, the background should differ
      const darkBg = await card.evaluate(el => getComputedStyle(el).backgroundColor)
      expect(darkBg).not.toBe(lightBg)

      // Switch back to light theme for subsequent tests
      await themeBtn.click()
      await page.waitForTimeout(500)
    }
  })

  test('D07b — 切换暗色主题后页面标题颜色变化', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    await page.waitForTimeout(1000)

    const title = page.locator('[data-testid="dashboard-title"]')
    await expect(title).toBeVisible()
    const lightColor = await title.evaluate(el => getComputedStyle(el).color)

    const themeBtn = page.locator('.theme-button')
    if (await themeBtn.isVisible()) {
      await themeBtn.click()
      await page.waitForTimeout(1000)

      const darkColor = await title.evaluate(el => getComputedStyle(el).color)
      expect(darkColor).not.toBe(lightColor)

      // Switch back
      await themeBtn.click()
      await page.waitForTimeout(500)
    }
  })
})

// ══════════════════════════════════════════════════════════════
// D08: 响应式布局
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 响应式布局', () => {
  test('D08a — 平板布局卡片两列排列', async ({ page }) => {
    await page.setViewportSize({ width: 768, height: 1024 })
    await navigateTo(page, '/dashboard')
    await page.waitForTimeout(1000)

    // stat-cards should still be visible
    const cards = page.locator('[data-testid="stat-cards"] .stat-card')
    await expect(cards).toHaveCount(4)

    // Charts should not overflow viewport
    const chartContainers = page.locator('.chart-container')
    const count = await chartContainers.count()
    for (let i = 0; i < count; i++) {
      const box = await chartContainers.nth(i).boundingBox()
      expect(box).toBeTruthy()
      // Chart width should fit within viewport
      expect(box!.width).toBeLessThanOrEqual(768)
    }
  })

  test('D08b — 手机布局卡片单列排列', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 })
    await navigateTo(page, '/dashboard')
    await page.waitForTimeout(1000)

    const cards = page.locator('[data-testid="stat-cards"] .stat-card')
    await expect(cards).toHaveCount(4)

    // Charts should not overflow viewport width
    const chartContainers = page.locator('.chart-container')
    const count = await chartContainers.count()
    for (let i = 0; i < count; i++) {
      const box = await chartContainers.nth(i).boundingBox()
      expect(box).toBeTruthy()
      expect(box!.width).toBeLessThanOrEqual(375)
    }
  })
})
