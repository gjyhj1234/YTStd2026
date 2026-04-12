import { http, HttpResponse } from 'msw'
import { mockPlatformRoles } from '../data/platformRoles'
import { ok, fail, paged, getPageParams } from '../data/common'

export const platformRolesHandlers = [
  http.get('/api/platform-roles/all', () => {
    return HttpResponse.json(ok(mockPlatformRoles))
  }),

  http.get('/api/platform-roles/check-code-exists', ({ request }) => {
    const url = new URL(request.url)
    const code = url.searchParams.get('Code') ?? ''
    const exists = mockPlatformRoles.some((r) => r.Code === code)
    return HttpResponse.json(ok(exists))
  }),

  http.get('/api/platform-roles/:id/permissions', () => {
    // 模拟已绑定权限 IDs
    return HttpResponse.json(ok([2, 3]))
  }),

  http.get('/api/platform-roles/:id/members', () => {
    // 模拟已绑定成员 IDs
    return HttpResponse.json(ok([1]))
  }),

  http.get('/api/platform-roles', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockPlatformRoles, page, pageSize))
  }),

  http.get('/api/platform-roles/:id', ({ params }) => {
    const role = mockPlatformRoles.find((r) => r.Id === Number(params['id']))
    if (!role) return HttpResponse.json(fail(), { status: 404 })
    return HttpResponse.json(ok(role))
  }),

  http.post('/api/platform-roles', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newRole = {
      Id: mockPlatformRoles.length + 1,
      Code: body['Code'] as string,
      Name: body['Name'] as string,
      Description: body['Description'] as string,
      Status: 'Active',
      CreatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newRole))
  }),

  http.put('/api/platform-roles/:id', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body))
  }),

  http.delete('/api/platform-roles/:id', () => {
    return HttpResponse.json(ok(null))
  }),

  http.put('/api/platform-roles/:id/enable', () => {
    return HttpResponse.json(ok(null))
  }),

  http.put('/api/platform-roles/:id/disable', () => {
    return HttpResponse.json(ok(null))
  }),

  http.post('/api/platform-roles/:id/permissions', () => {
    return HttpResponse.json(ok(null))
  }),

  http.post('/api/platform-roles/:id/members', () => {
    return HttpResponse.json(ok(null))
  }),
]
