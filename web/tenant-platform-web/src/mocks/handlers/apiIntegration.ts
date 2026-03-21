import { http, HttpResponse } from 'msw'
import {
  mockTenantApiKeys,
  mockTenantWebhooks,
  mockWebhookDeliveryLogs,
} from '../data/apiIntegration'
import { ok, paged, getPageParams } from '../data/common'

export const apiIntegrationHandlers = [
  http.get('/api/tenant-api-keys', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantApiKeys, page, pageSize))
  }),

  http.post('/api/tenant-api-keys', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const result = {
      id: mockTenantApiKeys.length + 1,
      tenantRefId: body['tenantRefId'] as number,
      keyName: body['keyName'] as string,
      accessKey: `ak_new_${Date.now()}`,
      secretKey: `sk_new_${Date.now()}`,
      status: 'Active',
      expiresAt: '2026-12-31T00:00:00Z',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(result, '创建成功'))
  }),

  http.put('/api/tenant-api-keys/:id/disable', () => {
    return HttpResponse.json(ok(null, '禁用成功'))
  }),

  http.get('/api/tenant-api-usage-stats', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    const stats = [
      {
        id: 1,
        tenantRefId: 1,
        apiKeyId: 1,
        statDate: '2025-06-10',
        callCount: 1580,
        errorCount: 12,
        createdAt: '2025-06-11T00:05:00Z',
      },
    ]
    return HttpResponse.json(paged(stats, page, pageSize))
  }),

  http.get('/api/webhook-events', () => {
    const events = [
      { id: 1, eventCode: 'subscription.created', eventName: '订阅创建', createdAt: '2025-01-01T00:00:00Z' },
      { id: 2, eventCode: 'invoice.paid', eventName: '账单已支付', createdAt: '2025-01-01T00:00:00Z' },
      { id: 3, eventCode: 'payment.success', eventName: '支付成功', createdAt: '2025-01-01T00:00:00Z' },
    ]
    return HttpResponse.json(ok(events))
  }),

  http.get('/api/tenant-webhooks', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantWebhooks, page, pageSize))
  }),

  http.post('/api/tenant-webhooks', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newWebhook = {
      id: mockTenantWebhooks.length + 1,
      tenantRefId: body['tenantRefId'] as number,
      webhookUrl: body['webhookUrl'] as string,
      eventTypes: body['eventTypes'] as string,
      secret: `whsec_${Date.now()}`,
      status: 'Active',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newWebhook, '创建成功'))
  }),

  http.put('/api/tenant-webhooks/:id', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '更新成功'))
  }),

  http.put('/api/tenant-webhooks/:id/enable', () => {
    return HttpResponse.json(ok(null, '启用成功'))
  }),

  http.put('/api/tenant-webhooks/:id/disable', () => {
    return HttpResponse.json(ok(null, '禁用成功'))
  }),

  http.get('/api/webhook-delivery-logs', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockWebhookDeliveryLogs, page, pageSize))
  }),
]
