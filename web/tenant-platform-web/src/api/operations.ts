/** API — 平台运营 */
import { get, type PagedResult } from '@/utils/http'

export interface TenantDailyStatDto {
  id: number
  tenantRefId: number
  statDate: string
  activeUserCount: number
  newUserCount: number
  apiCallCount: number
  storageBytes: number
  resourceScore: number
  createdAt: string
}

export interface PlatformMonitorMetricDto {
  id: number
  componentName: string
  metricType: string
  metricKey: string
  metricValue: number
  metricUnit: string
  collectedAt: string
}

export function getDailyStats(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantDailyStatDto>>('/api/tenant-daily-stats', params)
}

export function getMonitorMetrics(params: Record<string, string | number | undefined>) {
  return get<PagedResult<PlatformMonitorMetricDto>>('/api/platform-monitor-metrics', params)
}
