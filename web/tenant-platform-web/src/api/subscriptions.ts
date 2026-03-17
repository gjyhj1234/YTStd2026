/** API — 订阅管理 */
import { get, post, put, type PagedResult } from '@/utils/http'

/* ---------- 订阅 ---------- */

export interface TenantSubscriptionDto {
  id: number
  tenantRefId: number
  packageVersionId: number
  subscriptionStatus: string
  subscriptionType: string
  startedAt: string
  expiresAt: string | null
  autoRenew: boolean
  cancelledAt: string | null
  createdAt: string
}

export interface CreateSubscriptionRequest {
  tenantRefId: number
  packageVersionId: number
  subscriptionType: string
  autoRenew: boolean
}

export function getSubscriptions(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantSubscriptionDto>>('/api/tenant-subscriptions', params)
}

export function getSubscription(id: number) {
  return get<TenantSubscriptionDto>(`/api/tenant-subscriptions/${id}`)
}

export function createSubscription(data: CreateSubscriptionRequest) {
  return post<{ id: number }>('/api/tenant-subscriptions', data)
}

export function cancelSubscription(id: number) {
  return put<void>(`/api/tenant-subscriptions/${id}/cancel`)
}

/* ---------- 试用 ---------- */

export interface TenantTrialDto {
  id: number
  tenantRefId: number
  packageVersionId: number
  status: string
  startedAt: string
  expiresAt: string | null
  convertedSubscriptionId: number | null
  createdAt: string
}

export interface CreateTrialRequest {
  tenantRefId: number
  packageVersionId: number
}

export function getTrials(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantTrialDto>>('/api/tenant-trials', params)
}

export function createTrial(data: CreateTrialRequest) {
  return post<{ id: number }>('/api/tenant-trials', data)
}

/* ---------- 变更记录 ---------- */

export interface TenantSubscriptionChangeDto {
  id: number
  tenantRefId: number
  subscriptionId: number
  changeType: string
  fromPackageVersionId: number
  toPackageVersionId: number
  effectiveAt: string
  remark: string
  createdAt: string
}

export function getSubscriptionChanges(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantSubscriptionChangeDto>>('/api/tenant-subscription-changes', params)
}
