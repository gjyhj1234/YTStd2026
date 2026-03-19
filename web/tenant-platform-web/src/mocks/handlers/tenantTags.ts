import { http, HttpResponse } from 'msw'
import { mockTenantTags } from '../data/tenantTags'
import { ok, paged, getPageParams } from '../data/common'

export const tenantTagsHandlers = [
  http.get('/api/tenant-tags', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantTags, page, pageSize))
  }),

  http.post('/api/tenant-tags', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newTag = {
      id: mockTenantTags.length + 1,
      tagCode: body['tagCode'] as string,
      tagName: body['tagName'] as string,
      tagCategory: body['tagCategory'] as string,
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newTag, '创建成功'))
  }),

  http.post('/api/tenant-tags/bind', () => {
    return HttpResponse.json(ok(null, '绑定成功'))
  }),
]
