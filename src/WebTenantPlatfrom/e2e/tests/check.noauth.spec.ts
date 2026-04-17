import { test, expect } from '@playwright/test'

test('sidebar shows all menu items after login', async ({ page }) => {
  await page.goto('/#/login-form')
  await page.waitForTimeout(2000)
  
  // Fill login form inside .login-card
  const formInputs = page.locator('.login-card .dx-texteditor-input')
  await formInputs.nth(0).fill('admin')
  await formInputs.nth(1).fill('gjwq1234')
  
  // Click login
  await page.locator('.login-submit').click()
  await page.waitForTimeout(5000)
  
  // Should be on dashboard
  expect(page.url()).toContain('dashboard')
  
  // Check sidebar shows all top-level menu groups
  const menuItems = await page.locator('.dx-treeview-item').allTextContents()
  console.log('Menu items:', JSON.stringify(menuItems))
  expect(menuItems.length).toBeGreaterThan(5)
  
  // Verify key menu groups exist
  const menuText = menuItems.join('|')
  expect(menuText).toContain('首页')
  expect(menuText).toContain('平台管理')
  expect(menuText).toContain('租户管理')
  expect(menuText).toContain('SaaS 运营')
  expect(menuText).toContain('系统管理')
})

test('collapsed sidebar icons are centered', async ({ page, request }) => {
  // Setup auth via API
  const loginResponse = await request.post('/api/auth/login', {
    data: { Username: 'admin', Password: 'gjwq1234' }
  })
  const result = await loginResponse.json()
  
  await page.goto('/')
  await page.evaluate((loginData) => {
    localStorage.setItem('auth_token', loginData.Token)
    localStorage.setItem('auth_user', JSON.stringify({
      Id: loginData.UserId,
      Username: loginData.Username,
      DisplayName: loginData.DisplayName,
      Email: '',
      Permissions: loginData.Permissions,
      IsSuperAdmin: loginData.IsSuperAdmin
    }))
  }, result.Data)
  
  await page.reload()
  await page.waitForTimeout(3000)
  
  // Click menu toggle to collapse sidebar
  await page.locator('.menu-button .dx-button').click()
  await page.waitForTimeout(1000)
  
  // Check icon positions are properly aligned (icon x should be near 0)
  const iconInfo = await page.evaluate(() => {
    const icons = document.querySelectorAll('.dx-treeview-item .dx-icon')
    const results: { idx: number; iconLeft: number; iconWidth: number; drawerWidth: number }[] = []
    const drawer = document.querySelector('.dx-drawer-panel-content')
    const drawerWidth = drawer?.getBoundingClientRect().width || 0
    icons.forEach((icon, idx) => {
      const rect = icon.getBoundingClientRect()
      results.push({
        idx,
        iconLeft: Math.round(rect.left),
        iconWidth: Math.round(rect.width),
        drawerWidth: Math.round(drawerWidth),
      })
    })
    return results
  })
  
  console.log('Icon positions:', JSON.stringify(iconInfo))
  
  // Drawer should be 60px wide
  expect(iconInfo[0].drawerWidth).toBe(60)
  
  // Each icon should start at x=0 (or very close) to be centered in the 60px visible area
  for (const info of iconInfo) {
    expect(info.iconLeft).toBeLessThanOrEqual(5)
    expect(info.iconWidth).toBe(60)
  }
})

test('stale auth_token without auth_user redirects to login', async ({ page }) => {
  // Set only token, no user data
  await page.goto('/')
  await page.evaluate(() => {
    localStorage.setItem('auth_token', 'stale-token-without-user')
    localStorage.removeItem('auth_user')
  })
  
  // Try to navigate to protected route
  await page.goto('/#/dashboard')
  await page.waitForTimeout(3000)
  
  // Should be redirected to login page
  expect(page.url()).toContain('login-form')
})
