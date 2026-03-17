/** API — API 密钥与 Webhook */
import { get, post, put, type PagedResult } from '@/utils/http'

/* ---------- API 密钥 ---------- */

export interface TenantApiKeyDto {
  id: number
  tenantRefId: number
  keyName: string
  accessKey: string
  status: string
  quotaLimit: number
  rateLimit: number
  lastUsedAt: string | null
  expiresAt: string | null
  createdAt: string
}

export interface CreateApiKeyRequest {
  tenantRefId: number
  keyName: string
  expiresAt: string
}

export interface ApiKeyCreatedResult {
  id: number
  accessKey: string
  secretKey: string
}

export function getApiKeys(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantApiKeyDto>>('/api/tenant-api-keys', params)
}

export function createApiKey(data: CreateApiKeyRequest) {
  return post<ApiKeyCreatedResult>('/api/tenant-api-keys', data)
}

export function disableApiKey(id: number) {
  return put<void>(`/api/tenant-api-keys/${id}/disable`)
}

/* ---------- API 使用统计 ---------- */

export interface TenantApiUsageStatDto {
  id: number
  tenantRefId: number
  apiKeyId: number
  statDate: string
  apiPath: string
  requestCount: number
  successCount: number
  errorCount: number
  averageLatencyMs: number
  createdAt: string
}

export function getApiUsageStats(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantApiUsageStatDto>>('/api/tenant-api-usage-stats', params)
}

/* ---------- Webhook 事件 ---------- */

export interface WebhookEventDto {
  id: number
  eventCode: string
  eventName: string
  description: string
  createdAt: string
}

export function getWebhookEvents(params: Record<string, string | number | undefined>) {
  return get<PagedResult<WebhookEventDto>>('/api/webhook-events', params)
}

/* ---------- Webhook ---------- */

export interface TenantWebhookDto {
  id: number
  tenantRefId: number
  webhookName: string
  targetUrl: string
  status: string
  createdAt: string
}

export interface CreateWebhookRequest {
  tenantRefId: number
  webhookName: string
  targetUrl: string
}

export interface UpdateWebhookRequest {
  webhookName?: string
  targetUrl?: string
}

export function getWebhooks(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantWebhookDto>>('/api/tenant-webhooks', params)
}

export function createWebhook(data: CreateWebhookRequest) {
  return post<{ id: number }>('/api/tenant-webhooks', data)
}

export function updateWebhook(id: number, data: UpdateWebhookRequest) {
  return put<void>(`/api/tenant-webhooks/${id}`, data)
}

export function enableWebhook(id: number) {
  return put<void>(`/api/tenant-webhooks/${id}/enable`)
}

export function disableWebhook(id: number) {
  return put<void>(`/api/tenant-webhooks/${id}/disable`)
}

/* ---------- Webhook 投递日志 ---------- */

export interface WebhookDeliveryLogDto {
  id: number
  webhookId: number
  eventId: number
  deliveryStatus: string
  responseStatusCode: number
  retryCount: number
  deliveredAt: string | null
  createdAt: string
}

export function getWebhookDeliveryLogs(params: Record<string, string | number | undefined>) {
  return get<PagedResult<WebhookDeliveryLogDto>>('/api/webhook-delivery-logs', params)
}
