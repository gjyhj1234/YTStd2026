/** API — 租户配置（系统配置、功能开关、参数） */
import { get, post, put, del, type PagedResult } from '@/utils/http'

/* ---------- 系统配置 ---------- */

export interface TenantSystemConfigDto {
  id: number
  tenantRefId: number
  systemName: string
  logoUrl: string
  systemTheme: string
  defaultLanguage: string
  defaultTimezone: string
  updatedAt: string
}

export interface UpdateTenantSystemConfigRequest {
  systemName?: string
  logoUrl?: string
  systemTheme?: string
  defaultLanguage?: string
  defaultTimezone?: string
}

export function getTenantSystemConfig(tenantRefId: number) {
  return get<TenantSystemConfigDto>(`/api/tenant-system-configs/${tenantRefId}`)
}

export function updateTenantSystemConfig(tenantRefId: number, data: UpdateTenantSystemConfigRequest) {
  return put<void>(`/api/tenant-system-configs/${tenantRefId}`, data)
}

/* ---------- 功能开关 ---------- */

export interface TenantFeatureFlagDto {
  id: number
  tenantRefId: number
  featureKey: string
  featureName: string
  enabled: boolean
  rolloutType: string
  updatedAt: string
}

export interface SaveTenantFeatureFlagRequest {
  tenantRefId: number
  featureKey: string
  featureName: string
  enabled: boolean
  rolloutType: string
}

export function getTenantFeatureFlags(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantFeatureFlagDto>>('/api/tenant-feature-flags', params)
}

export function saveTenantFeatureFlag(data: SaveTenantFeatureFlagRequest) {
  return post<{ id: number }>('/api/tenant-feature-flags', data)
}

export function toggleFeatureFlag(id: number, enabled: boolean) {
  return put<void>(`/api/tenant-feature-flags/${id}/toggle`, { enabled })
}

/* ---------- 参数 ---------- */

export interface TenantParameterDto {
  id: number
  tenantRefId: number
  paramKey: string
  paramName: string
  paramType: string
  paramValue: string
  updatedAt: string
}

export interface SaveTenantParameterRequest {
  tenantRefId: number
  paramKey: string
  paramName: string
  paramType: string
  paramValue: string
}

export function getTenantParameters(params: Record<string, string | number | undefined>) {
  return get<PagedResult<TenantParameterDto>>('/api/tenant-parameters', params)
}

export function saveTenantParameter(data: SaveTenantParameterRequest) {
  return post<{ id: number }>('/api/tenant-parameters', data)
}

export function deleteTenantParameter(id: number) {
  return del<void>(`/api/tenant-parameters/${id}`)
}
