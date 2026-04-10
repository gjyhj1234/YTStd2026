/** API — 租户域名 */
import { get, post, del } from '@/utils/http'
import type { TenantDomainRepDTO, CreateTenantDomainReqDTO } from '@/types/tenantInfo'

export type { TenantDomainRepDTO, CreateTenantDomainReqDTO }

export function getTenantDomains() {
  return get<TenantDomainRepDTO[]>('/api/tenant-domains')
}

export function createTenantDomain(data: CreateTenantDomainReqDTO) {
  return post<void>('/api/tenant-domains', data)
}

export function deleteTenantDomain(tenantRefId: number, domainId: number) {
  return del<void>(`/api/tenant-groups/${tenantRefId}/domains/${domainId}`)
}
