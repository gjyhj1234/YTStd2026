/** API — 审计与日志 */
import { get, type PagedResult } from '@/utils/http'

/* ---------- 操作日志 ---------- */

export interface OperationLogDto {
  id: number
  tenantRefId: number
  operatorType: string
  operatorId: number
  action: string
  resourceType: string
  resourceId: string
  ipAddress: string
  operationResult: string
  createdAt: string
}

export function getOperationLogs(params: Record<string, string | number | undefined>) {
  return get<PagedResult<OperationLogDto>>('/api/operation-logs', params)
}

export function getOperationLog(id: number) {
  return get<OperationLogDto>(`/api/operation-logs/${id}`)
}

/* ---------- 审计日志 ---------- */

export interface AuditLogDto {
  id: number
  tenantRefId: number
  auditType: string
  severity: string
  subjectType: string
  subjectId: string
  complianceTag: string
  createdAt: string
}

export function getAuditLogs(params: Record<string, string | number | undefined>) {
  return get<PagedResult<AuditLogDto>>('/api/audit-logs', params)
}

export function getAuditLog(id: number) {
  return get<AuditLogDto>(`/api/audit-logs/${id}`)
}

/* ---------- 系统日志 ---------- */

export interface SystemLogDto {
  id: number
  serviceName: string
  logLevel: string
  traceId: string
  message: string
  createdAt: string
}

export function getSystemLogs(params: Record<string, string | number | undefined>) {
  return get<PagedResult<SystemLogDto>>('/api/system-logs', params)
}

export function getSystemLog(id: number) {
  return get<SystemLogDto>(`/api/system-logs/${id}`)
}
