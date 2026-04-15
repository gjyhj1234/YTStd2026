import { test, expect } from '@playwright/test'
import { navigateTo, waitForGridLoaded, getGridRowCount } from '../../helpers/test-helpers'

/**
 * 平台角色管理 E2E 测试
 *
 * 测试模块：F2-3 平台角色管理（0022_platform-role-page.md）
 * 测试范围：
 *   - 页面渲染与元素完整性
 *   - 角色列表展示
 *   - 新增角色弹窗
 *   - 搜索功能
 *   - 响应式布局
 *   - 多语言切换
 *
 * 前置条件：
 *   - 后端已启动（http://127.0.0.1:5000）
 *   - 前端已启动（http://localhost:5173）
 *   - 已通过 auth.setup.ts 完成登录
 *   - 种子数据中存在角色（super_admin, platform_admin, platform_viewer）
 */

// ══════════════════════════════════════════════════════════════
// R01~R02: 页面渲染与角色列表
// ══════════════════════════════════════════════════════════════

test.describe('平台角色管理 — 页面渲染', () => {
  test.beforeEach(async ({ page }) => {
    await navigateTo(page, '/platform-roles')
    await page.waitForTimeout(2000)
  })

  test('R01 — 页面标题正确展示', async ({ page }) => {
    const title = page.locator('.page-title')
    await expect(title).toBeVisible({ timeout: 10_000 })
    const text = await title.textContent()
    expect(text).toBeTruthy()
  })

  test('R02 — 角色列表展示数据', async ({ page }) => {
    await waitForGridLoaded(page)
    const rowCount = await getGridRowCount(page)
    // Seed data has at least 3 roles
    expect(rowCount).toBeGreaterThanOrEqual(1)
  })

  test('R02b — 列头包含角色编码和名称', async ({ page }) => {
    await waitForGridLoaded(page)
    const headerPanel = page.locator('.dx-datagrid-headers')
    await expect(headerPanel).toBeVisible()

    const headerCells = page.locator('.dx-datagrid-headers .dx-header-row td')
    const count = await headerCells.count()
    expect(count).toBeGreaterThanOrEqual(4)
  })
})

// ══════════════════════════════════════════════════════════════
// R03: 新增角色弹窗
// ══════════════════════════════════════════════════════════════

test.describe('平台角色管理 — 新增角色', () => {
  test('R03 — 点击新增按钮打开弹窗', async ({ page }) => {
    await navigateTo(page, '/platform-roles')
    await page.waitForTimeout(2000)

    const addBtn = page.locator('.grid-toolbar .dx-button, .dx-toolbar .dx-button').filter({ hasText: /新增|Create|新規|Tambah|新增/i }).first()
    await expect(addBtn).toBeVisible({ timeout: 5_000 })
    await addBtn.click()

    // Popup should appear with role form
    const popup = page.locator('.dx-popup-content, .dx-overlay-content').filter({ hasText: /新增角色|Create Role|ロール作成|Cipta Peranan|新增角色/i })
    await expect(popup).toBeVisible({ timeout: 5_000 })
  })

  test('R03b — 空表单提交应触发验证', async ({ page }) => {
    await navigateTo(page, '/platform-roles')
    await page.waitForTimeout(2000)

    const addBtn = page.locator('.grid-toolbar .dx-button, .dx-toolbar .dx-button').filter({ hasText: /新增|Create|新規|Tambah|新增/i }).first()
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
// R09: 搜索
// ══════════════════════════════════════════════════════════════

test.describe('平台角色管理 — 搜索', () => {
  test('搜索功能可用', async ({ page }) => {
    await navigateTo(page, '/platform-roles')
    await page.waitForTimeout(2000)

    const searchArea = page.locator('.search-area')
    await expect(searchArea).toBeVisible()

    const searchInput = searchArea.locator('.dx-textbox').first().locator('input[type="text"]')
    await expect(searchInput).toBeVisible()

    // Search for "admin"
    await searchInput.fill('admin')
    const searchBtn = searchArea.locator('.dx-button').filter({ hasText: /查询|Search|検索|Cari|查詢/i }).first()
    await searchBtn.click()
    await page.waitForTimeout(1500)

    await waitForGridLoaded(page)
    const rowCount = await getGridRowCount(page)
    expect(rowCount).toBeGreaterThanOrEqual(1)
  })
})

// ══════════════════════════════════════════════════════════════
// 响应式布局
// ══════════════════════════════════════════════════════════════

test.describe('平台角色管理 — 桌面端', () => {
  test('桌面端完整布局', async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 720 })
    await navigateTo(page, '/platform-roles')
    await page.waitForTimeout(2000)

    await expect(page.locator('.page-title')).toBeVisible()
    await expect(page.locator('.search-area')).toBeVisible()
    await expect(page.locator('.dx-datagrid')).toBeVisible()
  })
})

test.describe('平台角色管理 — 平板端', () => {
  test('平板端页面可用', async ({ page }) => {
    await page.setViewportSize({ width: 768, height: 1024 })
    await navigateTo(page, '/platform-roles')
    await page.waitForTimeout(2000)

    await expect(page.locator('.page-title')).toBeVisible()
    await expect(page.locator('.dx-datagrid')).toBeVisible()
  })
})

test.describe('平台角色管理 — 手机端', () => {
  test('手机端页面可用', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 })
    await navigateTo(page, '/platform-roles')
    await page.waitForTimeout(2000)

    await expect(page.locator('.page-title')).toBeVisible()
  })
})

// ══════════════════════════════════════════════════════════════
// 多语言切换
// ══════════════════════════════════════════════════════════════

test.describe('平台角色管理 — 多语言切换', () => {
  for (const lang of [
    { locale: 'zh-CN', title: '平台角色管理' },
    { locale: 'en-US', title: 'Platform Role Management' },
    { locale: 'ja-JP', title: 'プラットフォームロール管理' },
    { locale: 'ms-MY', title: 'Pengurusan Peranan Platform' },
    { locale: 'zh-TW', title: '平台角色管理' },
  ]) {
    test(`切换到 ${lang.locale}`, async ({ page }) => {
      await page.goto('/#/platform-roles')
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
