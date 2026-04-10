/** API — 平台用户 */
import { get, post, put, del } from '@/utils/http'
import type { PagedResult } from '@/types/base'
import type { PlatformUserRepDTO, CreatePlatformUserReqDTO, UpdatePlatformUserReqDTO } from '@/types/platformUser'

export type { PlatformUserRepDTO, CreatePlatformUserReqDTO, UpdatePlatformUserReqDTO }

export function getPlatformUsers(params: Record<string, string | number | undefined>) {
  return get<PagedResult<PlatformUserRepDTO>>('/api/platform-users', params)
}

export function getPlatformUser(id: number) {
  return get<PlatformUserRepDTO>(`/api/platform-users/${id}`)
}

export function createPlatformUser(data: CreatePlatformUserReqDTO) {
  return post<number>('/api/platform-users', data)
}

export function updatePlatformUser(id: number, data: UpdatePlatformUserReqDTO) {
  return put<void>(`/api/platform-users/${id}`, data)
}

export function deletePlatformUser(id: number) {
  return del<void>(`/api/platform-users/${id}`)
}

export function enablePlatformUser(id: number) {
  return put<void>(`/api/platform-users/${id}/enable`)
}

export function disablePlatformUser(id: number) {
  return put<void>(`/api/platform-users/${id}/disable`)
}

export function resetPlatformUserPassword(id: number, data: { NewPassword: string }) {
  return put<void>(`/api/platform-users/${id}/reset-password`, data)
}

export function checkUsernameExists(username: string) {
  return get<boolean>('/api/platform-users/check-username-exists', { Username: username })
}

export function batchEnablePlatformUsers(ids: number[]) {
  return put<void>('/api/platform-users/batch-enable', { Ids: ids })
}

export function batchDisablePlatformUsers(ids: number[]) {
  return put<void>('/api/platform-users/batch-disable', { Ids: ids })
}
