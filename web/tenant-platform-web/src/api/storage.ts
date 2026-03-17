/** API — 存储管理 */
import { get, post, put, del, type PagedResult } from '@/utils/http'

/* ---------- 存储策略 ---------- */

export interface StorageStrategyDto {
  id: number
  strategyCode: string
  strategyName: string
  providerType: string
  bucketName: string
  basePath: string
  status: string
  createdAt: string
}

export interface CreateStorageStrategyRequest {
  strategyCode: string
  strategyName: string
  providerType: string
  bucketName: string
  basePath: string
}

export interface UpdateStorageStrategyRequest {
  strategyName?: string
  bucketName?: string
  basePath?: string
}

export function getStorageStrategies(params: Record<string, string | number | undefined>) {
  return get<PagedResult<StorageStrategyDto>>('/api/storage-strategies', params)
}

export function getStorageStrategy(id: number) {
  return get<StorageStrategyDto>(`/api/storage-strategies/${id}`)
}

export function createStorageStrategy(data: CreateStorageStrategyRequest) {
  return post<{ id: number }>('/api/storage-strategies', data)
}

export function updateStorageStrategy(id: number, data: UpdateStorageStrategyRequest) {
  return put<void>(`/api/storage-strategies/${id}`, data)
}

export function enableStorageStrategy(id: number) {
  return put<void>(`/api/storage-strategies/${id}/enable`)
}

export function disableStorageStrategy(id: number) {
  return put<void>(`/api/storage-strategies/${id}/disable`)
}

/* ---------- 租户文件 ---------- */

export interface TenantFileDto {
  id: number
  tenantRefId: number
  storageStrategyId: number
  fileName: string
  filePath: string
  fileExt: string
  mimeType: string
  fileSize: number
  uploaderType: string
  uploaderId: number
  visibility: string
  downloadCount: number
  createdAt: string
}

export function getTenantFiles(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantFileDto>>('/api/tenant-files', params)
}

export function getTenantFile(id: number) {
  return get<TenantFileDto>(`/api/tenant-files/${id}`)
}

export function deleteTenantFile(id: number) {
  return del<void>(`/api/tenant-files/${id}`)
}

/* ---------- 文件访问策略 ---------- */

export interface FileAccessPolicyDto {
  id: number
  fileId: number
  subjectType: string
  subjectId: number
  permissionCode: string
  createdAt: string
}

export interface SaveFileAccessPolicyRequest {
  fileId: number
  subjectType: string
  subjectId: number
  permissionCode: string
}

export function getFileAccessPolicies(params: Record<string, string | number | undefined>) {
  return get<PagedResult<FileAccessPolicyDto>>('/api/file-access-policies', params)
}

export function saveFileAccessPolicy(data: SaveFileAccessPolicyRequest) {
  return post<{ id: number }>('/api/file-access-policies', data)
}
