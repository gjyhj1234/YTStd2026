/** API — 平台权限 */
import { get } from '@/utils/http'

export interface PlatformPermissionDto {
  id: number
  code: string
  name: string
  permissionType: string
  parentId: number | null
  path: string
  method: string
  children: PlatformPermissionDto[]
}

export function getPermissionTree() {
  return get<PlatformPermissionDto[]>('/api/platform-permissions/tree')
}

export function getPermissions() {
  return get<PlatformPermissionDto[]>('/api/platform-permissions')
}

export function getPermission(id: number) {
  return get<PlatformPermissionDto>(`/api/platform-permissions/${id}`)
}

export function getPermissionByCode(code: string) {
  return get<PlatformPermissionDto>(`/api/platform-permissions/by-code/${code}`)
}
