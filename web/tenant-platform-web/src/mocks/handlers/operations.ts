import { http, HttpResponse } from 'msw'
import {
  mockTenantDailyStats,
  mockPlatformMonitorMetrics,
} from '../data/operations'
import { paged, getPageParams } from '../data/common'

export const operationsHandlers = [
  http.get('/api/tenant-daily-stats', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantDailyStats, page, pageSize))
  }),

  http.get('/api/platform-monitor-metrics', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockPlatformMonitorMetrics, page, pageSize))
  }),
]
