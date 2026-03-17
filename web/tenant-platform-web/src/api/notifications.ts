/** API — 通知管理 */
import { get, post, put, type PagedResult } from '@/utils/http'

/* ---------- 通知模板 ---------- */

export interface NotificationTemplateDto {
  id: number
  templateCode: string
  templateName: string
  channel: string
  subjectTemplate: string
  bodyTemplate: string
  status: string
  createdAt: string
}

export interface CreateNotificationTemplateRequest {
  templateCode: string
  templateName: string
  channel: string
  subjectTemplate: string
  bodyTemplate: string
}

export interface UpdateNotificationTemplateRequest {
  templateName?: string
  subjectTemplate?: string
  bodyTemplate?: string
}

export function getNotificationTemplates(params: Record<string, string | number | undefined>) {
  return get<PagedResult<NotificationTemplateDto>>('/api/notification-templates', params)
}

export function getNotificationTemplate(id: number) {
  return get<NotificationTemplateDto>(`/api/notification-templates/${id}`)
}

export function createNotificationTemplate(data: CreateNotificationTemplateRequest) {
  return post<{ id: number }>('/api/notification-templates', data)
}

export function updateNotificationTemplate(id: number, data: UpdateNotificationTemplateRequest) {
  return put<void>(`/api/notification-templates/${id}`, data)
}

export function enableNotificationTemplate(id: number) {
  return put<void>(`/api/notification-templates/${id}/enable`)
}

export function disableNotificationTemplate(id: number) {
  return put<void>(`/api/notification-templates/${id}/disable`)
}

/* ---------- 通知 ---------- */

export interface NotificationDto {
  id: number
  tenantRefId: number
  templateId: number
  channel: string
  recipient: string
  subject: string
  body: string
  sendStatus: string
  sentAt: string | null
  readAt: string | null
  createdAt: string
}

export interface CreateNotificationRequest {
  tenantRefId: number
  templateId: number
  channel: string
  recipient: string
  subject: string
  body: string
}

export function getNotifications(params: Record<string, string | number | undefined>) {
  return get<PagedResult<NotificationDto>>('/api/notifications', params)
}

export function getNotification(id: number) {
  return get<NotificationDto>(`/api/notifications/${id}`)
}

export function createNotification(data: CreateNotificationRequest) {
  return post<{ id: number }>('/api/notifications', data)
}

export function markNotificationRead(id: number) {
  return put<void>(`/api/notifications/${id}/read`)
}
