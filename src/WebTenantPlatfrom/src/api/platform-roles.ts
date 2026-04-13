import { httpGet, httpPost, httpPut, httpDelete } from './http'
import type { PagedResult } from './http.types'
import type { PlatformRoleRepDTO, CreatePlatformRoleReqDTO, UpdatePlatformRoleReqDTO } from '../types/platform-roles'

/** Get role list (paged) */
export function getPlatformRolesApi(params: {
  Page: number
  PageSize: number
  Keyword?: string
  Status?: string | null
}): Promise<PagedResult<PlatformRoleRepDTO>> {
  const query: Record<string, unknown> = {
    Page: params.Page,
    PageSize: params.PageSize
  }
  if (params.Keyword) query.Keyword = params.Keyword
  if (params.Status !== null && params.Status !== undefined) query.Status = params.Status
  return httpGet<PagedResult<PlatformRoleRepDTO>>('/platform-roles', query)
}

/** Get role detail */
export function getPlatformRoleApi(id: number): Promise<PlatformRoleRepDTO> {
  return httpGet<PlatformRoleRepDTO>(`/platform-roles/${id}`)
}

/** Create role */
export function createPlatformRoleApi(data: CreatePlatformRoleReqDTO): Promise<number> {
  return httpPost<number>('/platform-roles', data)
}

/** Update role */
export function updatePlatformRoleApi(id: number, data: UpdatePlatformRoleReqDTO): Promise<void> {
  return httpPut<void>(`/platform-roles/${id}`, data)
}

/** Delete role */
export function deletePlatformRoleApi(id: number): Promise<void> {
  return httpDelete<void>(`/platform-roles/${id}`)
}

/** Enable role */
export function enablePlatformRoleApi(id: number): Promise<void> {
  return httpPut<void>(`/platform-roles/${id}/enable`)
}

/** Disable role */
export function disablePlatformRoleApi(id: number): Promise<void> {
  return httpPut<void>(`/platform-roles/${id}/disable`)
}

/** Check code exists */
export function checkRoleCodeExistsApi(code: string): Promise<boolean> {
  return httpGet<boolean>('/platform-roles/check-code-exists', { code })
}

/** Get role permissions */
export function getRolePermissionsApi(id: number): Promise<number[]> {
  return httpGet<number[]>(`/platform-roles/${id}/permissions`)
}

/** Bind permissions to role */
export function bindRolePermissionsApi(id: number, permissionIds: number[]): Promise<void> {
  return httpPost<void>(`/platform-roles/${id}/permissions`, { PermissionIds: permissionIds })
}

/** Bind members to role */
export function bindRoleMembersApi(id: number, userIds: number[]): Promise<void> {
  return httpPost<void>(`/platform-roles/${id}/members`, { UserIds: userIds })
}

/** Get all roles (for select dropdown) */
export function getAllPlatformRolesApi(): Promise<PlatformRoleRepDTO[]> {
  return httpGet<PlatformRoleRepDTO[]>('/platform-roles/all')
}
