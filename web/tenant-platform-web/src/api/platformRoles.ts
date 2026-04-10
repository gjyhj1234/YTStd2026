/** API — 平台角色 */
import { get, post, put, del } from '@/utils/http'
import type { PagedResult } from '@/types/base'
import type { PlatformRoleRepDTO, CreatePlatformRoleReqDTO, UpdatePlatformRoleReqDTO, RolePermissionBindReqDTO, RoleMemberBindReqDTO } from '@/types/platformRole'

export type { PlatformRoleRepDTO, CreatePlatformRoleReqDTO }

export function getPlatformRoles(params: Record<string, string | number | undefined>) {
  return get<PagedResult<PlatformRoleRepDTO>>('/api/platform-roles', params)
}

export function getAllPlatformRoles() {
  return get<PlatformRoleRepDTO[]>('/api/platform-roles/all')
}

export function getPlatformRole(id: number) {
  return get<PlatformRoleRepDTO>(`/api/platform-roles/${id}`)
}

export function createPlatformRole(data: CreatePlatformRoleReqDTO) {
  return post<number>('/api/platform-roles', data)
}

export function updatePlatformRole(id: number, data: UpdatePlatformRoleReqDTO) {
  return put<void>(`/api/platform-roles/${id}`, data)
}

export function deletePlatformRole(id: number) {
  return del<void>(`/api/platform-roles/${id}`)
}

export function enablePlatformRole(id: number) {
  return put<void>(`/api/platform-roles/${id}/enable`)
}

export function disablePlatformRole(id: number) {
  return put<void>(`/api/platform-roles/${id}/disable`)
}

export function getRolePermissions(id: number) {
  return get<number[]>(`/api/platform-roles/${id}/permissions`)
}

export function bindRolePermissions(id: number, data: RolePermissionBindReqDTO) {
  return post<void>(`/api/platform-roles/${id}/permissions`, data)
}

export function bindRoleMembers(id: number, data: RoleMemberBindReqDTO) {
  return post<void>(`/api/platform-roles/${id}/members`, data)
}

export function checkRoleCodeExists(code: string) {
  return get<boolean>('/api/platform-roles/check-code-exists', { Code: code })
}
