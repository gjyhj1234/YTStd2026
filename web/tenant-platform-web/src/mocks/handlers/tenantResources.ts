import { http, HttpResponse } from 'msw'
import { mockTenantResourceQuotas } from '../data/tenantResources'
import { ok, paged, getPageParams } from '../data/common'

export const tenantResourcesHandlers = [
  http.get('/api/tenant-resource-quotas', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantResourceQuotas, page, pageSize))
  }),

  http.get('/api/tenant-resource-quotas/:id', ({ params }) => {
    const q = mockTenantResourceQuotas.find((r) => r.id === Number(params['id']))
    if (!q) return HttpResponse.json({ success: false, message: '资源配额不存在', data: null, traceId: '' }, { status: 404 })
    return HttpResponse.json(ok(q))
  }),

  http.post('/api/tenant-resource-quotas', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newQuota = {
      id: mockTenantResourceQuotas.length + 1,
      tenantRefId: body['tenantRefId'] as number,
      quotaType: body['quotaType'] as string,
      quotaLimit: body['quotaLimit'] as number,
      warningThreshold: body['warningThreshold'] as number,
      resetCycle: body['resetCycle'] as string,
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newQuota, '创建成功'))
  }),
]
