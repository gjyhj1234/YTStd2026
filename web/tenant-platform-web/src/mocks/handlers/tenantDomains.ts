import { http, HttpResponse } from 'msw'
import { mockTenantDomains } from '../data/tenantDomains'
import { ok, paged, getPageParams } from '../data/common'

export const tenantDomainsHandlers = [
  http.get('/api/tenant-domains', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantDomains, page, pageSize))
  }),

  http.post('/api/tenant-domains', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newDomain = {
      id: mockTenantDomains.length + 1,
      tenantRefId: body['tenantRefId'] as number,
      domain: body['domain'] as string,
      domainType: body['domainType'] as string,
      isPrimary: false,
      verificationStatus: 'Pending',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newDomain, '创建成功'))
  }),
]
