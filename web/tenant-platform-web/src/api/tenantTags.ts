/** API — 租户标签 */
import { get, post, del } from '@/utils/http'
import type { PagedResult } from '@/types/base'
import type { TenantTagRepDTO, CreateTenantTagReqDTO, TagBindReqDTO } from '@/types/tenantInfo'

export type { TenantTagRepDTO, CreateTenantTagReqDTO, TagBindReqDTO }

export function getTenantTags(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantTagRepDTO>>('/api/tenant-tags', params)
}

export function createTenantTag(data: CreateTenantTagReqDTO) {
  return post<void>('/api/tenant-tags', data)
}

export function deleteTenantTag(id: number) {
  return del<void>(`/api/tenant-tags/${id}`)
}

export function bindTags(data: TagBindReqDTO) {
  return post<void>('/api/tenant-tags/bind', data)
}
