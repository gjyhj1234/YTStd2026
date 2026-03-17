/** API — 平台角色 */
import { get, post, put, type PagedResult } from '@/utils/http'

export interface PlatformRoleDto {
  id: number
  code: string
  name: string
  description: string
  status: string
  createdAt: string
}

export interface CreatePlatformRoleRequest {
  code: string
  name: string
  description: string
}

export interface UpdatePlatformRoleRequest {
  name?: string
  description?: string
}

export interface RolePermissionBindRequest {
  permissionIds: number[]
}

export interface RoleMemberBindRequest {
  userIds: number[]
}

export function getPlatformRoles(params: Record<string, string | number | undefined>) {
  return get<PagedResult<PlatformRoleDto>>('/api/platform-roles', params)
}

export function getPlatformRole(id: number) {
  return get<PlatformRoleDto>(`/api/platform-roles/${id}`)
}

export function createPlatformRole(data: CreatePlatformRoleRequest) {
  return post<{ id: number }>('/api/platform-roles', data)
}

export function updatePlatformRole(id: number, data: UpdatePlatformRoleRequest) {
  return put<void>(`/api/platform-roles/${id}`, data)
}

export function enablePlatformRole(id: number) {
  return put<void>(`/api/platform-roles/${id}/enable`)
}

export function disablePlatformRole(id: number) {
  return put<void>(`/api/platform-roles/${id}/disable`)
}

export function bindRolePermissions(id: number, data: RolePermissionBindRequest) {
  return put<void>(`/api/platform-roles/${id}/permissions`, data)
}

export function bindRoleMembers(id: number, data: RoleMemberBindRequest) {
  return put<void>(`/api/platform-roles/${id}/members`, data)
}
