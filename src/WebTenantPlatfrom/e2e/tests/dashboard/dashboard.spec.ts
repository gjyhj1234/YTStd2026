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
 *   - 侧边栏导航
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
    // Hash router redirects / to /dashboard
    await expect(page).toHaveURL(/.*#\/dashboard/)
  })

  test('D02 — 页面标题正确展示', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    const title = page.locator('[data-testid="dashboard-title"]')
    await expect(title).toBeVisible({ timeout: 10_000 })
    // Should show "仪表盘" or its translation
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
    // Wait for loading to complete
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
    // Each value should have text content
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
})

// ══════════════════════════════════════════════════════════════
// D04: 侧边栏导航
// ══════════════════════════════════════════════════════════════

test.describe('仪表盘 — 侧边栏', () => {
  test('D04 — 左侧菜单正确展示', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    // Sidebar should be visible with menu items
    const sidebar = page.locator('.dx-treeview')
    await expect(sidebar).toBeVisible({ timeout: 10_000 })
  })

  test('D05 — 点击菜单项可跳转', async ({ page }) => {
    await navigateTo(page, '/dashboard')
    // Find a clickable menu item (e.g. "首页" / "Home")
    const homeItem = page.locator('.dx-treeview-item').first()
    await expect(homeItem).toBeVisible({ timeout: 10_000 })
    await homeItem.click()
    await page.waitForTimeout(500)
    // Should still be on dashboard (home menu points to dashboard)
    await expect(page).toHaveURL(/.*#\/dashboard/)
  })
})
