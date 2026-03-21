import { http, HttpResponse } from 'msw'
import { mockPlatformUsers } from '../data/platformUsers'
import { ok, paged, getPageParams } from '../data/common'

export const platformUsersHandlers = [
  http.get('/api/platform-users', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockPlatformUsers, page, pageSize))
  }),

  http.post('/api/platform-users', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newUser = {
      id: mockPlatformUsers.length + 1,
      username: body['username'] as string,
      displayName: body['displayName'] as string,
      email: body['email'] as string,
      phone: body['phone'] as string,
      status: 'Active',
      isSuperAdmin: false,
      lastLoginAt: '',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newUser, '创建成功'))
  }),

  http.put('/api/platform-users/:id', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '更新成功'))
  }),

  http.put('/api/platform-users/:id/enable', () => {
    return HttpResponse.json(ok(null, '启用成功'))
  }),

  http.put('/api/platform-users/:id/disable', () => {
    return HttpResponse.json(ok(null, '禁用成功'))
  }),
]
