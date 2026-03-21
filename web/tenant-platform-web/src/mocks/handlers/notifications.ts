import { http, HttpResponse } from 'msw'
import {
  mockNotificationTemplates,
  mockNotifications,
} from '../data/notifications'
import { ok, paged, getPageParams } from '../data/common'

export const notificationsHandlers = [
  http.get('/api/notification-templates', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockNotificationTemplates, page, pageSize))
  }),

  http.get('/api/notification-templates/:id', ({ params }) => {
    const tpl = mockNotificationTemplates.find((t) => t.id === Number(params['id']))
    if (!tpl)
      return HttpResponse.json(
        { success: false, message: '模板不存在', data: null, traceId: '' },
        { status: 404 },
      )
    return HttpResponse.json(ok(tpl))
  }),

  http.post('/api/notification-templates', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newTpl = {
      id: mockNotificationTemplates.length + 1,
      templateCode: body['templateCode'] as string,
      templateName: body['templateName'] as string,
      channel: body['channel'] as string,
      subjectTemplate: body['subjectTemplate'] as string,
      bodyTemplate: body['bodyTemplate'] as string,
      status: 'Active',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newTpl, '创建成功'))
  }),

  http.put('/api/notification-templates/:id', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '更新成功'))
  }),

  http.put('/api/notification-templates/:id/enable', () => {
    return HttpResponse.json(ok(null, '启用成功'))
  }),

  http.put('/api/notification-templates/:id/disable', () => {
    return HttpResponse.json(ok(null, '禁用成功'))
  }),

  http.get('/api/notifications', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockNotifications, page, pageSize))
  }),

  http.get('/api/notifications/:id', ({ params }) => {
    const n = mockNotifications.find((n) => n.id === Number(params['id']))
    if (!n)
      return HttpResponse.json(
        { success: false, message: '通知不存在', data: null, traceId: '' },
        { status: 404 },
      )
    return HttpResponse.json(ok(n))
  }),

  http.post('/api/notifications', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '创建成功'))
  }),

  http.put('/api/notifications/:id/read', () => {
    return HttpResponse.json(ok(null, '标记已读'))
  }),
]
