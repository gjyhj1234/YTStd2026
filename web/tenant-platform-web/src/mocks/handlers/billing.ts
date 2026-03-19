import { http, HttpResponse } from 'msw'
import {
  mockBillingInvoices,
  mockPaymentOrders,
  mockPaymentRefunds,
} from '../data/billing'
import { ok, paged, getPageParams } from '../data/common'

export const billingHandlers = [
  http.get('/api/billing-invoices', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockBillingInvoices, page, pageSize))
  }),

  http.get('/api/billing-invoices/:id', ({ params }) => {
    const inv = mockBillingInvoices.find((i) => i.id === Number(params['id']))
    if (!inv)
      return HttpResponse.json(
        { success: false, message: '账单不存在', data: null, traceId: '' },
        { status: 404 },
      )
    return HttpResponse.json(ok(inv))
  }),

  http.get('/api/billing-invoices/:id/items', () => {
    return HttpResponse.json(ok([]))
  }),

  http.post('/api/billing-invoices', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '创建成功'))
  }),

  http.put('/api/billing-invoices/:id/void', () => {
    return HttpResponse.json(ok(null, '作废成功'))
  }),

  http.get('/api/payment-orders', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockPaymentOrders, page, pageSize))
  }),

  http.post('/api/payment-orders', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '创建成功'))
  }),

  http.get('/api/payment-refunds', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockPaymentRefunds, page, pageSize))
  }),

  http.post('/api/payment-refunds', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '创建成功'))
  }),
]
