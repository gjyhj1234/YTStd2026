import { test as setup, expect } from '@playwright/test'

/**
 * 认证设置（Authentication Setup）
 *
 * 此文件在所有需要登录的测试之前运行，通过 API 登录并保存浏览器状态。
 * 后续测试通过 storageState 复用此登录状态，避免每个测试重复登录。
 *
 * 默认管理员账号：admin / gjwq1234
 */

const AUTH_FILE = './e2e/.auth/user.json'

setup('authenticate as admin', async ({ page, request }) => {
  // 1. 通过 API 登录获取 Token
  const loginResponse = await request.post('/api/auth/login', {
    data: { Username: 'admin', Password: 'gjwq1234' }
  })
  expect(loginResponse.ok()).toBeTruthy()

  const result = await loginResponse.json()
  const token = result.Data?.Token
  expect(token).toBeTruthy()

  // 2. 导航到页面并注入 Token 和用户信息到 localStorage
  await page.goto('/')
  await page.evaluate((loginData: { Token: string; UserId: number; Username: string; DisplayName: string; Permissions: string[] }) => {
    localStorage.setItem('auth_token', loginData.Token)
    localStorage.setItem('auth_user', JSON.stringify({
      Id: loginData.UserId,
      Username: loginData.Username,
      DisplayName: loginData.DisplayName,
      Email: '',
      Permissions: loginData.Permissions
    }))
  }, {
    Token: result.Data.Token,
    UserId: result.Data.UserId,
    Username: result.Data.Username,
    DisplayName: result.Data.DisplayName,
    Permissions: result.Data.Permissions || []
  })

  // 3. Reload page to ensure Pinia store initializes from localStorage
  //    (hash navigation alone may not trigger a full app re-initialization)
  await page.reload()
  await page.waitForLoadState('networkidle')

  // 4. Navigate to dashboard and verify authenticated state
  await page.goto('/#/dashboard')
  await page.waitForLoadState('networkidle')
  await page.waitForTimeout(2000)

  // 5. Save authenticated state (includes localStorage with Token and user info)
  await page.context().storageState({ path: AUTH_FILE })
})
