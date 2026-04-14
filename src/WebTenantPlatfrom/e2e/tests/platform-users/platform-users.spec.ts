import { test, expect } from '@playwright/test'
import { navigateTo, waitForGridLoaded, getGridRowCount } from '../../helpers/test-helpers'

/**
 * 平台用户管理 E2E 测试
 *
 * 测试模块：F2-2 平台用户管理（0021_platform-user-page.md）
 * 测试范围：
 *   - 页面渲染与元素完整性
 *   - 数据列表展示
 *   - 搜索功能
 *   - 列头检查
 *   - 表单验证
 *   - 响应式布局
 *   - 多语言切换
 *
 * 前置条件：
 *   - 后端已启动（http://127.0.0.1:5000）
 *   - 前端已启动（http://localhost:5173）
 *   - 已通过 auth.setup.ts 完成登录
 *   - 种子数据中存在 admin 用户
 */

// ══════════════════════════════════════════════════════════════
// U01~U03: 页面渲染与数据列表
// ══════════════════════════════════════════════════════════════

test.describe('平台用户管理 — 页面渲染', () => {
  test.beforeEach(async ({ page }) => {
    await navigateTo(page, '/platform-users')
    await page.waitForTimeout(2000)
  })

  test('U01 — 页面标题正确展示', async ({ page }) => {
    const title = page.locator('.page-title')
    await expect(title).toBeVisible({ timeout: 10_000 })
    const text = await title.textContent()
    expect(text).toBeTruthy()
  })

  test('U02 — 用户列表展示数据', async ({ page }) => {
    await waitForGridLoaded(page)
    const rowCount = await getGridRowCount(page)
    // Seed data has at least the admin user
    expect(rowCount).toBeGreaterThanOrEqual(1)
  })

  test('U03 — 列头包含关键字段', async ({ page }) => {
    await waitForGridLoaded(page)
    // Check header cells for key columns
    const headerPanel = page.locator('.dx-datagrid-headers')
    await expect(headerPanel).toBeVisible()

    // The grid should contain columns like ID, Username, DisplayName, Status, etc.
    // Check that at least the grid header is rendered
    const headerCells = page.locator('.dx-datagrid-headers .dx-header-row td')
    const count = await headerCells.count()
    expect(count).toBeGreaterThanOrEqual(4)
  })
})

// ══════════════════════════════════════════════════════════════
// U10: 搜索功能
// ══════════════════════════════════════════════════════════════

test.describe('平台用户管理 — 搜索', () => {
  test('U10 — 搜索功能可用', async ({ page }) => {
    await navigateTo(page, '/platform-users')
    await page.waitForTimeout(2000)

    // Search bar should contain DxTextBox and DxSelectBox
    const searchBar = page.locator('.search-bar')
    await expect(searchBar).toBeVisible()

    // The search keyword input
    const searchInput = searchBar.locator('.dx-textbox').first().locator('input[type="text"]')
    await expect(searchInput).toBeVisible()

    // Type a search term
    await searchInput.fill('admin')

    // Click search button
    const searchBtn = searchBar.locator('.dx-button').filter({ hasText: /查询|Search|検索|Cari|查詢/i }).first()
    await expect(searchBtn).toBeVisible()
    await searchBtn.click()
    await page.waitForTimeout(1500)

    // Should still show admin user
    await waitForGridLoaded(page)
    const rowCount = await getGridRowCount(page)
    expect(rowCount).toBeGreaterThanOrEqual(1)
  })
})

// ══════════════════════════════════════════════════════════════
// U04: 新增用户弹窗
// ══════════════════════════════════════════════════════════════

test.describe('平台用户管理 — 新增用户', () => {
  test('U04 — 点击新增按钮打开弹窗', async ({ page }) => {
    await navigateTo(page, '/platform-users')
    await page.waitForTimeout(2000)

    // Click "新增" button in toolbar
    const addBtn = page.locator('.grid-toolbar .dx-button, .toolbar-buttons .dx-button').filter({ hasText: /新增|Create|新規|Tambah|新增/i }).first()
    await expect(addBtn).toBeVisible({ timeout: 5_000 })
    await addBtn.click()

    // Popup should appear
    const popup = page.locator('.dx-popup-content, .dx-overlay-content').filter({ hasText: /新增用户|Create User|ユーザー作成|Cipta Pengguna|新增用戶/i })
    await expect(popup).toBeVisible({ timeout: 5_000 })
  })

  test('U11 — 空表单提交应触发验证', async ({ page }) => {
    await navigateTo(page, '/platform-users')
    await page.waitForTimeout(2000)

    // Open create dialog
    const addBtn = page.locator('.grid-toolbar .dx-button, .toolbar-buttons .dx-button').filter({ hasText: /新增|Create|新規|Tambah|新增/i }).first()
    await addBtn.click()
    await page.waitForTimeout(500)

    // Click save without filling form
    const saveBtn = page.locator('.dx-popup-content .dx-button, .dx-overlay-content .dx-button').filter({ hasText: /确定|Save|OK|保存|Submit/i }).first()
    await expect(saveBtn).toBeVisible({ timeout: 5_000 })
    await saveBtn.click()
    await page.waitForTimeout(500)

    // Validation errors should appear
    const invalidFields = page.locator('.dx-popup-content .dx-textbox.dx-invalid, .dx-overlay-content .dx-textbox.dx-invalid')
    const count = await invalidFields.count()
    expect(count).toBeGreaterThan(0)
  })
})

// ══════════════════════════════════════════════════════════════
// 响应式布局
// ══════════════════════════════════════════════════════════════

test.describe('平台用户管理 — 桌面端', () => {
  test('桌面端完整布局', async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 720 })
    await navigateTo(page, '/platform-users')
    await page.waitForTimeout(2000)

    // Page title visible
    await expect(page.locator('.page-title')).toBeVisible()
    // Search bar visible
    await expect(page.locator('.search-bar')).toBeVisible()
    // Grid visible
    await expect(page.locator('.dx-datagrid')).toBeVisible()
    // Sidebar visible
    await expect(page.locator('.dx-treeview')).toBeVisible()
  })
})

test.describe('平台用户管理 — 平板端', () => {
  test('平板端页面可用', async ({ page }) => {
    await page.setViewportSize({ width: 768, height: 1024 })
    await navigateTo(page, '/platform-users')
    await page.waitForTimeout(2000)

    // Core elements visible
    await expect(page.locator('.page-title')).toBeVisible()
    await expect(page.locator('.dx-datagrid')).toBeVisible()
  })
})

test.describe('平台用户管理 — 手机端', () => {
  test('手机端页面可用', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 })
    await navigateTo(page, '/platform-users')
    await page.waitForTimeout(2000)

    // Core elements visible
    await expect(page.locator('.page-title')).toBeVisible()
  })
})

// ══════════════════════════════════════════════════════════════
// 多语言切换
// ══════════════════════════════════════════════════════════════

test.describe('平台用户管理 — 多语言切换', () => {
  for (const lang of [
    { locale: 'zh-CN', title: '平台用户管理' },
    { locale: 'en-US', title: 'Platform User Management' },
    { locale: 'ja-JP', title: 'プラットフォームユーザー管理' },
    { locale: 'ms-MY', title: 'Pengurusan Pengguna Platform' },
    { locale: 'zh-TW', title: '平台使用者管理' },
  ]) {
    test(`切换到 ${lang.locale}`, async ({ page }) => {
      // Navigate first to establish page context, then set locale and reload
      await page.goto('/#/platform-users')
      await page.waitForLoadState('networkidle')
      await page.evaluate((l: string) => localStorage.setItem('locale', l), lang.locale)
      await page.reload()
      await page.waitForLoadState('networkidle')
      await page.waitForTimeout(2000)

      const title = page.locator('.page-title')
      await expect(title).toBeVisible()
      await expect(title).toContainText(lang.title)
    })
  }
})
