import { test as setup, expect } from '@playwright/test'

/**
 * 认证设置（Authentication Setup）
 *
 * 此文件在所有需要登录的测试之前运行，执行登录操作并保存浏览器状态。
 * 后续测试通过 storageState 复用此登录状态，避免每个测试重复登录。
 *
 * 默认管理员账号：admin / gjwq1234
 */

const AUTH_FILE = './e2e/.auth/user.json'

setup('authenticate as admin', async ({ page }) => {
  // 1. 导航到登录页
  await page.goto('/#/login-form')

  // 2. 等待登录表单加载
  await expect(page.locator('.dx-form')).toBeVisible({ timeout: 15_000 })

  // 3. 填写登录表单
  //    注意：DevExtreme DxTextBox 需要通过 input 元素填写
  const usernameInput = page.locator('input[name="Username"]').or(
    page.locator('.dx-textbox').first().locator('input')
  )
  const passwordInput = page.locator('input[name="Password"]').or(
    page.locator('input[type="password"]')
  )

  await usernameInput.fill('admin')
  await passwordInput.fill('gjwq1234')

  // 4. 点击登录按钮
  const loginButton = page.locator('.dx-button').filter({ hasText: /登录|Sign In|Login/i })
  await loginButton.click()

  // 5. 等待导航到仪表盘（登录成功标志）
  //    注意：RequirePasswordReset 可能为 true，需要处理
  await page.waitForURL(/.*#\/(dashboard|change-password)/, { timeout: 15_000 })

  // 6. 如果被重定向到修改密码页面，说明 RequirePasswordReset=true
  //    在测试环境中，可以先跳过或处理
  if (page.url().includes('change-password')) {
    console.warn('[auth.setup] Admin requires password reset - this may affect tests')
  }

  // 7. 保存认证状态
  await page.context().storageState({ path: AUTH_FILE })
})
