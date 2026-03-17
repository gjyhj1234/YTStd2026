/** API — 租户标签 */
import { get, post, type PagedResult } from '@/utils/http'

export interface TenantTagDto {
  id: number
  tagKey: string
  tagValue: string
  tagType: string
  description: string
  createdAt: string
}

export interface CreateTenantTagRequest {
  tagKey: string
  tagValue: string
  tagType: string
  description: string
}

export interface TagBindRequest {
  tenantRefId: number
  tagIds: number[]
}

export function getTenantTags(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantTagDto>>('/api/tenant-tags', params)
}

export function createTenantTag(data: CreateTenantTagRequest) {
  return post<{ id: number }>('/api/tenant-tags', data)
}

export function bindTags(data: TagBindRequest) {
  return post<void>('/api/tenant-tags/bind', data)
}
