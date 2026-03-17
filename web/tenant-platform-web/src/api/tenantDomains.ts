/** API — 租户域名 */
import { get, post, type PagedResult } from '@/utils/http'

export interface TenantDomainDto {
  id: number
  tenantRefId: number
  domain: string
  domainType: string
  isPrimary: boolean
  verificationStatus: string
  createdAt: string
}

export interface CreateTenantDomainRequest {
  tenantRefId: number
  domain: string
  domainType: string
}

export function getTenantDomains(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantDomainDto>>('/api/tenant-domains', params)
}

export function createTenantDomain(data: CreateTenantDomainRequest) {
  return post<{ id: number }>('/api/tenant-domains', data)
}
