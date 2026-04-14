import { test, expect } from '@playwright/test'

test('debug collapsed sidebar DOM structure', async ({ page, request }) => {
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
      Permissions: loginData.Permissions
    }))
  }, result.Data)
  
  await page.reload()
  await page.waitForTimeout(3000)
  
  // Click menu toggle to collapse sidebar
  await page.locator('.menu-button .dx-button').click()
  await page.waitForTimeout(1000)
  
  // Get the DOM structure of a tree item
  const domInfo = await page.evaluate(() => {
    const firstItem = document.querySelector('.dx-treeview-item')
    if (!firstItem) return { error: 'no treeview item' }
    
    const html = firstItem.outerHTML.substring(0, 500)
    
    // Get computed styles
    const itemStyle = window.getComputedStyle(firstItem)
    const icon = firstItem.querySelector('.dx-icon')
    const content = firstItem.querySelector('.dx-treeview-item-content')
    
    // Check parent node
    const node = firstItem.closest('.dx-treeview-node')
    const nodeStyle = node ? window.getComputedStyle(node) : null
    
    // Check if icon is inside or outside content
    const iconParent = icon?.parentElement?.className
    
    return {
      html,
      itemPadding: itemStyle.padding,
      itemPaddingLeft: itemStyle.paddingLeft,
      itemFlexDirection: itemStyle.flexDirection,
      itemDisplay: itemStyle.display,
      iconParent,
      iconRect: icon?.getBoundingClientRect(),
      contentRect: content?.getBoundingClientRect(),
      contentPaddingLeft: content ? window.getComputedStyle(content).paddingLeft : null,
      nodePadding: nodeStyle?.padding,
      nodePaddingLeft: nodeStyle?.paddingLeft,
      nodeMarginLeft: nodeStyle?.marginLeft,
      nodeRect: node?.getBoundingClientRect(),
    }
  })
  console.log('DOM debug:', JSON.stringify(domInfo, null, 2))
})
