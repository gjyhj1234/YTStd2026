/** API — 仪表盘 */
import { get } from '@/utils/http'
import type { DashboardStats, TenantTrendItem, SubscriptionDistItem } from '@/types/dashboard'

export type { DashboardStats, TenantTrendItem, SubscriptionDistItem }

/** 获取仪表盘统计数据 */
export function getDashboardStats() {
  return get<DashboardStats>('/api/platform-operations/dashboard')
}

/** 获取租户增长趋势（最近 30 天） */
export function getTenantTrend() {
  return get<TenantTrendItem[]>('/api/platform-operations/tenant-trend')
}

/** 获取订阅分布 */
export function getSubscriptionDist() {
  return get<SubscriptionDistItem[]>('/api/platform-operations/subscription-dist')
}
