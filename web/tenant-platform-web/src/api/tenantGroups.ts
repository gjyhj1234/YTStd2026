/** API — 租户分组 */
import { get, post } from '@/utils/http'

export interface TenantGroupDto {
  id: number
  groupCode: string
  groupName: string
  description: string
  parentId: number | null
  children: TenantGroupDto[]
  createdAt: string
}

export interface CreateTenantGroupRequest {
  groupCode: string
  groupName: string
  description: string
  parentId?: number
}

export function getTenantGroupTree() {
  return get<TenantGroupDto[]>('/api/tenant-groups/tree')
}

export function getTenantGroups() {
  return get<TenantGroupDto[]>('/api/tenant-groups')
}

export function createTenantGroup(data: CreateTenantGroupRequest) {
  return post<{ id: number }>('/api/tenant-groups', data)
}
