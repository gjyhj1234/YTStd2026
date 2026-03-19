import { http, HttpResponse } from 'msw'
import {
  mockSaasPackages,
  mockSaasPackageVersions,
  mockSaasPackageCapabilities,
} from '../data/packages'
import { ok, paged, getPageParams } from '../data/common'

export const packagesHandlers = [
  http.get('/api/saas-packages', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockSaasPackages, page, pageSize))
  }),

  http.get('/api/saas-packages/:id', ({ params }) => {
    const pkg = mockSaasPackages.find((p) => p.id === Number(params['id']))
    if (!pkg)
      return HttpResponse.json(
        { success: false, message: '套餐不存在', data: null, traceId: '' },
        { status: 404 },
      )
    return HttpResponse.json(ok(pkg))
  }),

  http.post('/api/saas-packages', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newPkg = {
      id: mockSaasPackages.length + 1,
      packageCode: body['packageCode'] as string,
      packageName: body['packageName'] as string,
      description: (body['description'] as string) ?? '',
      status: 'Active',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newPkg, '创建成功'))
  }),

  http.put('/api/saas-packages/:id', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '更新成功'))
  }),

  http.put('/api/saas-packages/:id/enable', () => {
    return HttpResponse.json(ok(null, '启用成功'))
  }),

  http.put('/api/saas-packages/:id/disable', () => {
    return HttpResponse.json(ok(null, '禁用成功'))
  }),

  http.get('/api/saas-packages/:packageId/versions', ({ params, request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    const versions = mockSaasPackageVersions.filter(
      (v) => v.packageId === Number(params['packageId']),
    )
    return HttpResponse.json(paged(versions, page, pageSize))
  }),

  http.post('/api/saas-packages/:packageId/versions', async ({ request, params }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newVersion = {
      id: mockSaasPackageVersions.length + 1,
      packageId: Number(params['packageId']),
      versionCode: body['versionCode'] as string,
      versionName: body['versionName'] as string,
      editionType: body['editionType'] as string,
      billingCycle: body['billingCycle'] as string,
      price: body['price'] as number,
      currencyCode: (body['currencyCode'] as string) ?? 'CNY',
      trialDays: (body['trialDays'] as number) ?? 0,
      isDefault: false,
      enabled: true,
      effectiveFrom: new Date().toISOString(),
      effectiveTo: '2026-12-31T23:59:59Z',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newVersion, '创建成功'))
  }),

  http.get(
    '/api/saas-package-versions/:packageVersionId/capabilities',
    ({ params, request }) => {
      const url = new URL(request.url)
      const { page, pageSize } = getPageParams(url)
      const caps = mockSaasPackageCapabilities.filter(
        (c) => c.packageVersionId === Number(params['packageVersionId']),
      )
      return HttpResponse.json(paged(caps, page, pageSize))
    },
  ),

  http.post(
    '/api/saas-package-versions/:packageVersionId/capabilities',
    async ({ request, params }) => {
      const body = (await request.json()) as Record<string, unknown>
      const newCap = {
        id: mockSaasPackageCapabilities.length + 1,
        packageVersionId: Number(params['packageVersionId']),
        capabilityKey: body['capabilityKey'] as string,
        capabilityName: body['capabilityName'] as string,
        capabilityType: body['capabilityType'] as string,
        capabilityValue: body['capabilityValue'] as string,
        createdAt: new Date().toISOString(),
      }
      return HttpResponse.json(ok(newCap, '创建成功'))
    },
  ),
]
