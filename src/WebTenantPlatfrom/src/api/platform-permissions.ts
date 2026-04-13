import { httpGet } from './http'
import type { PlatformPermissionRepDTO } from '../types/platform-permissions'

/** Get permission tree */
export function getPermissionTreeApi(): Promise<PlatformPermissionRepDTO[]> {
  return httpGet<PlatformPermissionRepDTO[]>('/platform-permissions/tree')
}

/** Get permission list (flat) */
export function getPermissionListApi(): Promise<PlatformPermissionRepDTO[]> {
  return httpGet<PlatformPermissionRepDTO[]>('/platform-permissions')
}

/** Get permission detail */
export function getPermissionDetailApi(id: number): Promise<PlatformPermissionRepDTO> {
  return httpGet<PlatformPermissionRepDTO>(`/platform-permissions/${id}`)
}
