/** API — 租户资源配额 */
import { get, post, type PagedResult } from '@/utils/http'

export interface TenantResourceQuotaDto {
  id: number
  tenantRefId: number
  quotaType: string
  quotaLimit: number
  warningThreshold: number
  resetCycle: string
  createdAt: string
}

export interface SaveTenantResourceQuotaRequest {
  tenantRefId: number
  quotaType: string
  quotaLimit: number
  warningThreshold: number
  resetCycle: string
}

export function getTenantResourceQuotas(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantResourceQuotaDto>>('/api/tenant-resource-quotas', params)
}

export function getTenantResourceQuota(id: number) {
  return get<TenantResourceQuotaDto>(`/api/tenant-resource-quotas/${id}`)
}

export function saveTenantResourceQuota(data: SaveTenantResourceQuotaRequest) {
  return post<{ id: number }>('/api/tenant-resource-quotas', data)
}
