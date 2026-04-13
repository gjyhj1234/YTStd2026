import { httpGet } from './http'
import type { PagedResult } from './http.types'
import type { TenantDailyStatRepDTO, PlatformMonitorMetricRepDTO } from '../types/platform-operations'

/** 获取租户每日统计列表 */
export function getTenantStatisticsApi(
  page: number = 1,
  pageSize: number = 20,
  keyword: string = '',
  tenantRefId: number = 0
): Promise<PagedResult<TenantDailyStatRepDTO>> {
  return httpGet<PagedResult<TenantDailyStatRepDTO>>(
    '/platform-operations/tenant-statistics',
    { page, pageSize, keyword, tenantRefId }
  )
}

/** 获取平台监控指标列表 */
export function getMonitorMetricsApi(
  page: number = 1,
  pageSize: number = 20,
  keyword: string = ''
): Promise<PagedResult<PlatformMonitorMetricRepDTO>> {
  return httpGet<PagedResult<PlatformMonitorMetricRepDTO>>(
    '/platform-operations/monitor-metrics',
    { page, pageSize, keyword }
  )
}
