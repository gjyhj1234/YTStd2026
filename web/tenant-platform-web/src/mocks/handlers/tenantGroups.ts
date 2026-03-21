import { http, HttpResponse } from 'msw'
import { mockTenantGroups, mockFlatTenantGroups } from '../data/tenantGroups'
import { ok, paged, getPageParams } from '../data/common'

export const tenantGroupsHandlers = [
  http.get('/api/tenant-groups/tree', () => {
    return HttpResponse.json(ok(mockTenantGroups))
  }),

  http.get('/api/tenant-groups', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockFlatTenantGroups, page, pageSize))
  }),

  http.post('/api/tenant-groups', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newGroup = {
      id: mockFlatTenantGroups.length + 1,
      groupCode: body['groupCode'] as string,
      groupName: body['groupName'] as string,
      parentId: (body['parentId'] as number | null) ?? null,
      sortOrder: (body['sortOrder'] as number) ?? 0,
      description: (body['description'] as string) ?? '',
      createdAt: new Date().toISOString(),
      children: [],
    }
    return HttpResponse.json(ok(newGroup, '创建成功'))
  }),
]
