export const mockTenantApiKeys = [
  {
    id: 1,
    tenantRefId: 1,
    keyName: '生产环境密钥',
    accessKey: 'ak_huaxia_prod_xxxxxxxxxxxxx',
    status: 'Active',
    expiresAt: '2026-01-15T00:00:00Z',
    createdAt: '2025-01-20T00:00:00Z',
  },
  {
    id: 2,
    tenantRefId: 1,
    keyName: '测试环境密钥',
    accessKey: 'ak_huaxia_test_yyyyyyyyyyyyy',
    status: 'Active',
    expiresAt: '2025-12-31T00:00:00Z',
    createdAt: '2025-01-20T00:00:00Z',
  },
  {
    id: 3,
    tenantRefId: 2,
    keyName: '默认密钥',
    accessKey: 'ak_yunhai_default_zzzzzzzzzzz',
    status: 'Disabled',
    expiresAt: '2025-08-01T00:00:00Z',
    createdAt: '2025-02-05T00:00:00Z',
  },
]

export const mockTenantWebhooks = [
  {
    id: 1,
    tenantRefId: 1,
    webhookUrl: 'https://hooks.huaxia-tech.com/callback',
    eventTypes: 'subscription.created,subscription.cancelled',
    secret: 'whsec_xxxxxxxxxxxxxxxx',
    status: 'Active',
    createdAt: '2025-02-01T00:00:00Z',
  },
  {
    id: 2,
    tenantRefId: 2,
    webhookUrl: 'https://api.yunhai-data.cn/webhook',
    eventTypes: 'invoice.paid,payment.success',
    secret: 'whsec_yyyyyyyyyyyyyyyy',
    status: 'Active',
    createdAt: '2025-02-10T00:00:00Z',
  },
]

export const mockWebhookDeliveryLogs = [
  {
    id: 1,
    webhookId: 1,
    eventType: 'subscription.created',
    requestPayload: '{"event":"subscription.created","data":{"subscriptionId":1}}',
    responseStatus: 200,
    responseBody: '{"received":true}',
    deliveredAt: '2025-02-01T10:00:01Z',
    createdAt: '2025-02-01T10:00:00Z',
  },
  {
    id: 2,
    webhookId: 2,
    eventType: 'invoice.paid',
    requestPayload: '{"event":"invoice.paid","data":{"invoiceId":3}}',
    responseStatus: 200,
    responseBody: '{"ok":true}',
    deliveredAt: '2025-02-10T09:01:00Z',
    createdAt: '2025-02-10T09:00:59Z',
  },
  {
    id: 3,
    webhookId: 1,
    eventType: 'subscription.cancelled',
    requestPayload: '{"event":"subscription.cancelled","data":{"subscriptionId":3}}',
    responseStatus: 500,
    responseBody: 'Internal Server Error',
    deliveredAt: '2025-06-01T16:01:00Z',
    createdAt: '2025-06-01T16:00:59Z',
  },
]
