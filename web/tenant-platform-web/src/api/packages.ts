/** API — SaaS 套餐 */
import { get, post, put, type PagedResult } from '@/utils/http'

/* ---------- 套餐 ---------- */

export interface SaasPackageDto {
  id: number
  packageCode: string
  packageName: string
  description: string
  status: string
  createdAt: string
}

export interface CreateSaasPackageRequest {
  packageCode: string
  packageName: string
  description: string
}

export interface UpdateSaasPackageRequest {
  packageName: string
  description: string
}

export function getPackages(params: Record<string, string | number | undefined>) {
  return get<PagedResult<SaasPackageDto>>('/api/saas-packages', params)
}

export function getPackage(id: number) {
  return get<SaasPackageDto>(`/api/saas-packages/${id}`)
}

export function createPackage(data: CreateSaasPackageRequest) {
  return post<{ id: number }>('/api/saas-packages', data)
}

export function updatePackage(id: number, data: UpdateSaasPackageRequest) {
  return put<void>(`/api/saas-packages/${id}`, data)
}

export function enablePackage(id: number) {
  return put<void>(`/api/saas-packages/${id}/enable`)
}

export function disablePackage(id: number) {
  return put<void>(`/api/saas-packages/${id}/disable`)
}

/* ---------- 版本 ---------- */

export interface SaasPackageVersionDto {
  id: number
  packageId: number
  versionCode: string
  versionName: string
  editionType: string
  billingCycle: string
  price: number
  currencyCode: string
  trialDays: number
  isDefault: boolean
  enabled: boolean
  effectiveFrom: string | null
  effectiveTo: string | null
  createdAt: string
}

export interface CreateSaasPackageVersionRequest {
  packageId: number
  versionCode: string
  versionName: string
  editionType: string
  billingCycle: string
  price: number
  currencyCode: string
  trialDays: number
  isDefault: boolean
}

export function getPackageVersions(packageId: number, params: Record<string, string | number | undefined>) {
  return get<PagedResult<SaasPackageVersionDto>>(`/api/saas-packages/${packageId}/versions`, params)
}

export function createPackageVersion(packageId: number, data: CreateSaasPackageVersionRequest) {
  return post<{ id: number }>(`/api/saas-packages/${packageId}/versions`, data)
}

/* ---------- 能力 ---------- */

export interface SaasPackageCapabilityDto {
  id: number
  packageVersionId: number
  capabilityKey: string
  capabilityName: string
  capabilityType: string
  capabilityValue: string
  createdAt: string
}

export interface SaveSaasPackageCapabilityRequest {
  packageVersionId: number
  capabilityKey: string
  capabilityName: string
  capabilityType: string
  capabilityValue: string
}

export function getPackageCapabilities(packageVersionId: number, params: Record<string, string | number | undefined>) {
  return get<PagedResult<SaasPackageCapabilityDto>>(`/api/saas-package-versions/${packageVersionId}/capabilities`, params)
}

export function savePackageCapability(packageVersionId: number, data: SaveSaasPackageCapabilityRequest) {
  return post<{ id: number }>(`/api/saas-package-versions/${packageVersionId}/capabilities`, data)
}
