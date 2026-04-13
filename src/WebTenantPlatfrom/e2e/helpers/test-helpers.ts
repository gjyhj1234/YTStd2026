import { Page, expect } from '@playwright/test'

/**
 * E2E 测试通用工具函数
 *
 * 提供可复用的页面操作、断言、等待函数。
 * 所有 E2E 测试均应使用此文件中的工具函数，确保测试行为一致。
 */

// ──────────────────────────────────────────────────────────────
// 导航工具
// ──────────────────────────────────────────────────────────────

/** 导航到指定路由并等待页面加载 */
export async function navigateTo(page: Page, route: string): Promise<void> {
  await page.goto(`/#${route}`)
  // 等待 DevExtreme 组件渲染完成
  await page.waitForLoadState('networkidle')
  // 额外等待 300ms 确保 Vue 组件渲染完成
  await page.waitForTimeout(300)
}

/** 等待 DxDataGrid 加载完成 */
export async function waitForGridLoaded(page: Page): Promise<void> {
  // 等待 loading panel 消失
  await page.locator('.dx-loadpanel-content').waitFor({ state: 'hidden', timeout: 15_000 }).catch(() => {
    // loading panel 可能已经消失或不存在
  })
  // 等待 grid 出现
  await expect(page.locator('.dx-datagrid')).toBeVisible({ timeout: 10_000 })
}

/** 等待 DxForm 加载完成 */
export async function waitForFormLoaded(page: Page): Promise<void> {
  await expect(page.locator('.dx-form')).toBeVisible({ timeout: 10_000 })
}

// ──────────────────────────────────────────────────────────────
// DevExtreme 组件交互工具
// ──────────────────────────────────────────────────────────────

/** 在 DxTextBox 中输入文本（通过 label 或 placeholder 定位） */
export async function fillDxTextBox(page: Page, label: string, value: string): Promise<void> {
  // 尝试通过 label 定位
  const formItem = page.locator('.dx-field-item, .dx-form-group-content .dx-item').filter({ hasText: label })
  const input = formItem.locator('input').first()

  if (await input.isVisible()) {
    await input.clear()
    await input.fill(value)
  } else {
    // 回退：通过 placeholder 定位
    const fallbackInput = page.locator(`input[placeholder*="${label}"]`)
    await fallbackInput.clear()
    await fallbackInput.fill(value)
  }
}

/** 点击 DxButton（通过按钮文本定位） */
export async function clickDxButton(page: Page, text: string): Promise<void> {
  const button = page.locator('.dx-button').filter({ hasText: text })
  await expect(button).toBeVisible({ timeout: 5_000 })
  await button.click()
}

/** 在 DxSelectBox 中选择选项 */
export async function selectDxOption(page: Page, label: string, optionText: string): Promise<void> {
  const formItem = page.locator('.dx-field-item, .dx-form-group-content .dx-item').filter({ hasText: label })
  const selectBox = formItem.locator('.dx-selectbox, .dx-lookup')
  await selectBox.click()

  // 等待下拉列表出现
  const overlay = page.locator('.dx-overlay-content .dx-list, .dx-popup-content .dx-list')
  await expect(overlay).toBeVisible({ timeout: 5_000 })

  // 点击选项
  const option = overlay.locator('.dx-list-item').filter({ hasText: optionText })
  await option.click()
}

/** 在 DxDataGrid 工具栏中点击按钮 */
export async function clickGridToolbarButton(page: Page, text: string): Promise<void> {
  const toolbar = page.locator('.dx-datagrid-header-panel .dx-toolbar')
  const button = toolbar.locator('.dx-button').filter({ hasText: text })
  await expect(button).toBeVisible({ timeout: 5_000 })
  await button.click()
}

/** 获取 DxDataGrid 行数 */
export async function getGridRowCount(page: Page): Promise<number> {
  const rows = page.locator('.dx-datagrid-rowsview .dx-row.dx-data-row')
  return await rows.count()
}

/** 点击 DxDataGrid 指定行的操作按钮 */
export async function clickGridRowAction(page: Page, rowIndex: number, actionText: string): Promise<void> {
  const row = page.locator('.dx-datagrid-rowsview .dx-row.dx-data-row').nth(rowIndex)
  const actionButton = row.locator('.dx-button, .dx-link').filter({ hasText: actionText })
  await actionButton.click()
}

// ──────────────────────────────────────────────────────────────
// 通知与确认框工具
// ──────────────────────────────────────────────────────────────

/** 等待并验证 DevExtreme 通知消息 */
export async function expectNotification(page: Page, messagePattern: string | RegExp, type: 'success' | 'error' | 'warning' = 'success'): Promise<void> {
  const toast = page.locator(`.dx-toast-${type}, .dx-toast-message`).filter({
    hasText: typeof messagePattern === 'string' ? messagePattern : undefined
  })
  await expect(toast).toBeVisible({ timeout: 5_000 })
}

/** 确认 DevExtreme 确认对话框 */
export async function confirmDialog(page: Page): Promise<void> {
  const dialog = page.locator('.dx-dialog, .dx-popup-content').filter({ hasText: /确认|确定|Confirm|OK/i })
  const confirmBtn = dialog.locator('.dx-button').filter({ hasText: /确认|确定|是|Yes|OK/i })
  await confirmBtn.click()
}

/** 取消 DevExtreme 确认对话框 */
export async function cancelDialog(page: Page): Promise<void> {
  const dialog = page.locator('.dx-dialog, .dx-popup-content').filter({ hasText: /取消|Cancel/i })
  const cancelBtn = dialog.locator('.dx-button').filter({ hasText: /取消|否|No|Cancel/i })
  await cancelBtn.click()
}

// ──────────────────────────────────────────────────────────────
// DxPopup 工具
// ──────────────────────────────────────────────────────────────

/** 等待 DxPopup 弹窗出现 */
export async function waitForPopup(page: Page, titleText?: string): Promise<void> {
  if (titleText) {
    const popup = page.locator('.dx-popup-content, .dx-overlay-content').filter({ hasText: titleText })
    await expect(popup).toBeVisible({ timeout: 5_000 })
  } else {
    await expect(page.locator('.dx-popup-content')).toBeVisible({ timeout: 5_000 })
  }
}

/** 关闭 DxPopup 弹窗 */
export async function closePopup(page: Page): Promise<void> {
  const closeBtn = page.locator('.dx-closebutton, .dx-popup-title .dx-button').last()
  if (await closeBtn.isVisible()) {
    await closeBtn.click()
  }
}

// ──────────────────────────────────────────────────────────────
// 侧边栏导航工具
// ──────────────────────────────────────────────────────────────

/** 通过侧边栏导航到指定菜单 */
export async function navigateViaMenu(page: Page, ...menuTexts: string[]): Promise<void> {
  for (const text of menuTexts) {
    const menuItem = page.locator('.dx-treeview-item').filter({ hasText: text })
    await menuItem.click()
    await page.waitForTimeout(300)
  }
  await page.waitForLoadState('networkidle')
}

// ──────────────────────────────────────────────────────────────
// API 请求拦截工具
// ──────────────────────────────────────────────────────────────

/** 等待指定 API 请求完成 */
export async function waitForApiResponse(page: Page, urlPattern: string | RegExp): Promise<void> {
  await page.waitForResponse(
    response => {
      const url = response.url()
      if (typeof urlPattern === 'string') {
        return url.includes(urlPattern)
      }
      return urlPattern.test(url)
    },
    { timeout: 15_000 }
  )
}
