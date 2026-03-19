import { http, HttpResponse } from 'msw'
import { mockPlatformRoles } from '../data/platformRoles'
import { ok, paged, getPageParams } from '../data/common'

export const platformRolesHandlers = [
  http.get('/api/platform-roles', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockPlatformRoles, page, pageSize))
  }),

  http.get('/api/platform-roles/:id', ({ params }) => {
    const role = mockPlatformRoles.find((r) => r.id === Number(params['id']))
    if (!role) return HttpResponse.json({ success: false, message: '角色不存在', data: null, traceId: '' }, { status: 404 })
    return HttpResponse.json(ok(role))
  }),

  http.post('/api/platform-roles', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newRole = {
      id: mockPlatformRoles.length + 1,
      code: body['code'] as string,
      name: body['name'] as string,
      description: body['description'] as string,
      status: 'Active',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newRole, '创建成功'))
  }),

  http.put('/api/platform-roles/:id', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '更新成功'))
  }),

  http.put('/api/platform-roles/:id/enable', () => {
    return HttpResponse.json(ok(null, '启用成功'))
  }),

  http.put('/api/platform-roles/:id/disable', () => {
    return HttpResponse.json(ok(null, '禁用成功'))
  }),

  http.put('/api/platform-roles/:id/permissions', () => {
    return HttpResponse.json(ok(null, '权限绑定成功'))
  }),

  http.put('/api/platform-roles/:id/members', () => {
    return HttpResponse.json(ok(null, '成员绑定成功'))
  }),
]
