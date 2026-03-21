import { http, HttpResponse } from 'msw'
import {
  mockTenantSubscriptions,
  mockTenantTrials,
  mockTenantSubscriptionChanges,
} from '../data/subscriptions'
import { ok, paged, getPageParams } from '../data/common'

export const subscriptionsHandlers = [
  http.get('/api/tenant-subscriptions', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantSubscriptions, page, pageSize))
  }),

  http.get('/api/tenant-subscriptions/:id', ({ params }) => {
    const sub = mockTenantSubscriptions.find((s) => s.id === Number(params['id']))
    if (!sub)
      return HttpResponse.json(
        { success: false, message: '订阅不存在', data: null, traceId: '' },
        { status: 404 },
      )
    return HttpResponse.json(ok(sub))
  }),

  http.post('/api/tenant-subscriptions', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newSub = {
      id: mockTenantSubscriptions.length + 1,
      tenantRefId: body['tenantRefId'] as number,
      packageVersionId: body['packageVersionId'] as number,
      subscriptionStatus: 'Active',
      subscriptionType: 'Paid',
      startedAt: new Date().toISOString(),
      expiresAt: '2026-12-31T23:59:59Z',
      autoRenew: (body['autoRenew'] as boolean) ?? false,
      cancelledAt: '',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newSub, '创建成功'))
  }),

  http.put('/api/tenant-subscriptions/:id/cancel', () => {
    return HttpResponse.json(ok(null, '取消成功'))
  }),

  http.get('/api/tenant-trials', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantTrials, page, pageSize))
  }),

  http.post('/api/tenant-trials', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newTrial = {
      id: mockTenantTrials.length + 1,
      tenantRefId: body['tenantRefId'] as number,
      packageVersionId: body['packageVersionId'] as number,
      status: 'Active',
      startedAt: new Date().toISOString(),
      expiresAt: '2025-12-31T23:59:59Z',
      convertedSubscriptionId: null,
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newTrial, '创建成功'))
  }),

  http.get('/api/tenant-subscription-changes', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantSubscriptionChanges, page, pageSize))
  }),
]
