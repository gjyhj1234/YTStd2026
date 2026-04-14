import { httpGet, httpPost, httpPut, httpDelete } from './http'
import type { PagedResult } from './http.types'
import type { PlatformUserRepDTO, PlatformUserCreateReqDTO, PlatformUserUpdateReqDTO } from '../types/platform-users'

/** Get user list (paged) */
export function getPlatformUsersApi(params: {
  Page: number
  PageSize: number
  Keyword?: string
  Status?: string | null
  RoleId?: number | null
  CreatedAtStart?: string | null
  CreatedAtEnd?: string | null
}): Promise<PagedResult<PlatformUserRepDTO>> {
  const query: Record<string, unknown> = {
    Page: params.Page,
    PageSize: params.PageSize
  }
  if (params.Keyword) query.Keyword = params.Keyword
  if (params.Status !== null && params.Status !== undefined) query.Status = params.Status
  if (params.RoleId !== null && params.RoleId !== undefined) query.RoleId = params.RoleId
  if (params.CreatedAtStart) query.CreatedAtStart = params.CreatedAtStart
  if (params.CreatedAtEnd) query.CreatedAtEnd = params.CreatedAtEnd
  return httpGet<PagedResult<PlatformUserRepDTO>>('/platform-users', query)
}

/** Get user detail */
export function getPlatformUserApi(id: number): Promise<PlatformUserRepDTO> {
  return httpGet<PlatformUserRepDTO>(`/platform-users/${id}`)
}

/** Create user */
export function createPlatformUserApi(data: PlatformUserCreateReqDTO): Promise<number> {
  return httpPost<number>('/platform-users', data)
}

/** Update user */
export function updatePlatformUserApi(id: number, data: PlatformUserUpdateReqDTO): Promise<void> {
  return httpPut<void>(`/platform-users/${id}`, data)
}

/** Delete user */
export function deletePlatformUserApi(id: number): Promise<void> {
  return httpDelete<void>(`/platform-users/${id}`)
}

/** Enable user */
export function enablePlatformUserApi(id: number): Promise<void> {
  return httpPut<void>(`/platform-users/${id}/enable`)
}

/** Disable user */
export function disablePlatformUserApi(id: number): Promise<void> {
  return httpPut<void>(`/platform-users/${id}/disable`)
}

/** Reset password */
export function resetPasswordApi(id: number): Promise<void> {
  return httpPut<void>(`/platform-users/${id}/reset-password`)
}

/** Check username exists */
export function checkUsernameExistsApi(username: string): Promise<boolean> {
  return httpGet<boolean>('/platform-users/check-username-exists', { username })
}

/** Batch enable users */
export function batchEnableUsersApi(ids: number[]): Promise<void> {
  return httpPut<void>('/platform-users/batch-enable', { Ids: ids })
}

/** Batch disable users */
export function batchDisableUsersApi(ids: number[]): Promise<void> {
  return httpPut<void>('/platform-users/batch-disable', { Ids: ids })
}
