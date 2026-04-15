import { httpGet, httpPost, httpPut, httpDelete } from './http'
import type { PagedResult } from './http.types'
import type { PlatformRoleRepDTO, PlatformRoleSimpleRepDTO, CreatePlatformRoleReqDTO, UpdatePlatformRoleReqDTO } from '../types/platform-roles'

/** Get role list (paged) */
export function getPlatformRolesApi(params: {
  Page: number
  PageSize: number
  Keyword?: string
  Status?: string | null
  SortField?: string
  SortOrder?: string
}): Promise<PagedResult<PlatformRoleRepDTO>> {
  const query: Record<string, unknown> = {
    Page: params.Page,
    PageSize: params.PageSize
  }
  if (params.Keyword) query.Keyword = params.Keyword
  if (params.Status !== null && params.Status !== undefined) query.Status = params.Status
  if (params.SortField) query.SortField = params.SortField
  if (params.SortOrder) query.SortOrder = params.SortOrder
  return httpGet<PagedResult<PlatformRoleRepDTO>>('/platform-roles', query)
}

/** Get role detail */
export function getPlatformRoleApi(id: string | number): Promise<PlatformRoleRepDTO> {
  return httpGet<PlatformRoleRepDTO>(`/platform-roles/${id}`)
}

/** Create role */
export function createPlatformRoleApi(data: CreatePlatformRoleReqDTO): Promise<string | number> {
  return httpPost<string | number>('/platform-roles', data)
}

/** Update role */
export function updatePlatformRoleApi(id: string | number, data: UpdatePlatformRoleReqDTO): Promise<void> {
  return httpPut<void>(`/platform-roles/${id}`, data)
}

/** Delete role */
export function deletePlatformRoleApi(id: string | number): Promise<void> {
  return httpDelete<void>(`/platform-roles/${id}`)
}

/** Enable role */
export function enablePlatformRoleApi(id: string | number): Promise<void> {
  return httpPut<void>(`/platform-roles/${id}/enable`)
}

/** Disable role */
export function disablePlatformRoleApi(id: string | number): Promise<void> {
  return httpPut<void>(`/platform-roles/${id}/disable`)
}

/** Check code exists */
export function checkRoleCodeExistsApi(code: string): Promise<boolean> {
  return httpGet<boolean>('/platform-roles/check-code-exists', { code })
}

/** Get role permissions */
export function getRolePermissionsApi(id: string | number): Promise<Array<string | number>> {
  return httpGet<Array<string | number>>(`/platform-roles/${id}/permissions`)
}

/** Bind permissions to role */
export function bindRolePermissionsApi(id: string | number, permissionIds: Array<string | number>): Promise<void> {
  return httpPost<void>(`/platform-roles/${id}/permissions`, { PermissionIds: permissionIds })
}

/** Get role members (user IDs) */
export function getRoleMembersApi(id: string | number): Promise<Array<string | number>> {
  return httpGet<Array<string | number>>(`/platform-roles/${id}/members`)
}

/** Bind members to role */
export function bindRoleMembersApi(id: string | number, userIds: Array<string | number>): Promise<void> {
  return httpPost<void>(`/platform-roles/${id}/members`, { UserIds: userIds })
}

/** Get all roles (for select dropdown) */
export function getAllPlatformRolesApi(): Promise<PlatformRoleSimpleRepDTO[]> {
  return httpGet<PlatformRoleSimpleRepDTO[]>('/platform-roles/all')
}
