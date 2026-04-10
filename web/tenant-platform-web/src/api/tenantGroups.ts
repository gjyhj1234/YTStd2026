/** API — 租户分组 */
import { get, post, put, del } from '@/utils/http'
import type { TenantGroupRepDTO, CreateTenantGroupReqDTO, UpdateTenantGroupReqDTO } from '@/types/tenantInfo'

export type { TenantGroupRepDTO, CreateTenantGroupReqDTO }

export function getTenantGroupTree() {
  return get<TenantGroupRepDTO[]>('/api/tenant-groups/tree')
}

export function getTenantGroups() {
  return get<TenantGroupRepDTO[]>('/api/tenant-groups')
}

export function createTenantGroup(data: CreateTenantGroupReqDTO) {
  return post<void>('/api/tenant-groups', data)
}

export function updateTenantGroup(id: number, data: UpdateTenantGroupReqDTO) {
  return put<void>(`/api/tenant-groups/${id}`, data)
}

export function deleteTenantGroup(id: number) {
  return del<void>(`/api/tenant-groups/${id}`)
}
