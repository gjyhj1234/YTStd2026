import { http, HttpResponse } from 'msw'
import {
  mockTenantSystemConfigs,
  mockTenantFeatureFlags,
  mockTenantParameters,
} from '../data/tenantConfig'
import { ok, paged, getPageParams } from '../data/common'

export const tenantConfigHandlers = [
  http.get('/api/tenant-system-configs/:tenantRefId', ({ params }) => {
    const config = mockTenantSystemConfigs.find(
      (c) => c.tenantRefId === Number(params['tenantRefId']),
    )
    if (!config)
      return HttpResponse.json(
        { success: false, message: '配置不存在', data: null, traceId: '' },
        { status: 404 },
      )
    return HttpResponse.json(ok(config))
  }),

  http.put('/api/tenant-system-configs/:tenantRefId', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '更新成功'))
  }),

  http.get('/api/tenant-feature-flags', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantFeatureFlags, page, pageSize))
  }),

  http.post('/api/tenant-feature-flags', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const flag = {
      id: mockTenantFeatureFlags.length + 1,
      tenantRefId: body['tenantRefId'] as number,
      featureKey: body['featureKey'] as string,
      featureName: body['featureName'] as string,
      enabled: (body['enabled'] as boolean) ?? false,
      description: (body['description'] as string) ?? '',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(flag, '创建成功'))
  }),

  http.put('/api/tenant-feature-flags/:id/toggle', () => {
    return HttpResponse.json(ok(null, '切换成功'))
  }),

  http.get('/api/tenant-parameters', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantParameters, page, pageSize))
  }),

  http.post('/api/tenant-parameters', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const param = {
      id: mockTenantParameters.length + 1,
      tenantRefId: body['tenantRefId'] as number,
      paramKey: body['paramKey'] as string,
      paramValue: body['paramValue'] as string,
      paramType: body['paramType'] as string,
      description: (body['description'] as string) ?? '',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(param, '创建成功'))
  }),

  http.delete('/api/tenant-parameters/:id', () => {
    return HttpResponse.json(ok(null, '删除成功'))
  }),
]
