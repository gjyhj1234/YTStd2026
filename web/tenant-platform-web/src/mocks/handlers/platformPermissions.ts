import { http, HttpResponse } from 'msw'
import { mockPlatformPermissions, mockFlatPermissions } from '../data/platformPermissions'
import { ok } from '../data/common'

export const platformPermissionsHandlers = [
  http.get('/api/platform-permissions/tree', () => {
    return HttpResponse.json(ok(mockPlatformPermissions))
  }),

  http.get('/api/platform-permissions/by-code/:code', ({ params }) => {
    const perm = mockFlatPermissions.find((p) => p.code === params['code'])
    if (!perm) return HttpResponse.json({ success: false, message: '权限不存在', data: null, traceId: '' }, { status: 404 })
    return HttpResponse.json(ok(perm))
  }),

  http.get('/api/platform-permissions/:id', ({ params }) => {
    const perm = mockFlatPermissions.find((p) => p.id === Number(params['id']))
    if (!perm) return HttpResponse.json({ success: false, message: '权限不存在', data: null, traceId: '' }, { status: 404 })
    return HttpResponse.json(ok(perm))
  }),

  http.get('/api/platform-permissions', () => {
    return HttpResponse.json(ok(mockFlatPermissions))
  }),
]
