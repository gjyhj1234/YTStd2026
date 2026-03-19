import { http, HttpResponse } from 'msw'
import { mockTenants } from '../data/tenants'
import { ok, paged, getPageParams } from '../data/common'

export const tenantsHandlers = [
  http.get('/api/tenants', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenants, page, pageSize))
  }),

  http.post('/api/tenants', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newTenant = {
      id: mockTenants.length + 1,
      tenantCode: `T${Date.now()}`,
      tenantName: body['tenantName'] as string,
      enterpriseName: body['enterpriseName'] as string,
      contactName: body['contactName'] as string,
      contactEmail: body['contactEmail'] as string,
      lifecycleStatus: 'Active',
      isolationMode: body['isolationMode'] as string ?? 'Shared',
      enabled: true,
      openedAt: new Date().toISOString(),
      expiresAt: '2026-12-31T23:59:59Z',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newTenant, '创建成功'))
  }),

  http.put('/api/tenants/:id', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '更新成功'))
  }),

  http.put('/api/tenants/:id/status', () => {
    return HttpResponse.json(ok(null, '状态变更成功'))
  }),
]
