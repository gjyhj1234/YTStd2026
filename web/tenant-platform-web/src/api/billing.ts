/** API — 账单与支付 */
import { get, post, put, type PagedResult } from '@/utils/http'

/* ---------- 账单 ---------- */

export interface BillingInvoiceDto {
  id: number
  invoiceNo: string
  tenantRefId: number
  subscriptionId: number
  invoiceStatus: string
  billingPeriodStart: string
  billingPeriodEnd: string
  subtotalAmount: number
  extraAmount: number
  discountAmount: number
  totalAmount: number
  currencyCode: string
  issuedAt: string | null
  dueAt: string | null
  paidAt: string | null
  createdAt: string
}

export interface CreateBillingInvoiceRequest {
  tenantRefId: number
  subscriptionId: number
  billingPeriodStart: string
  billingPeriodEnd: string
  currencyCode: string
}

export interface BillingInvoiceItemDto {
  id: number
  invoiceId: number
  itemType: string
  itemName: string
  quantity: number
  unitPrice: number
  amount: number
  createdAt: string
}

export function getInvoices(params: Record<string, string | number | undefined>) {
  return get<PagedResult<BillingInvoiceDto>>('/api/billing-invoices', params)
}

export function getInvoice(id: number) {
  return get<BillingInvoiceDto>(`/api/billing-invoices/${id}`)
}

export function createInvoice(data: CreateBillingInvoiceRequest) {
  return post<{ id: number }>('/api/billing-invoices', data)
}

export function voidInvoice(id: number) {
  return put<void>(`/api/billing-invoices/${id}/void`)
}

export function getInvoiceItems(invoiceId: number, params: Record<string, string | number | undefined>) {
  return get<PagedResult<BillingInvoiceItemDto>>(`/api/billing-invoices/${invoiceId}/items`, params)
}

/* ---------- 支付订单 ---------- */

export interface PaymentOrderDto {
  id: number
  orderNo: string
  tenantRefId: number
  invoiceId: number
  paymentChannel: string
  paymentStatus: string
  amount: number
  currencyCode: string
  thirdPartyTxnNo: string | null
  paidAt: string | null
  createdAt: string
}

export interface CreatePaymentOrderRequest {
  tenantRefId: number
  invoiceId: number
  paymentChannel: string
  amount: number
  currencyCode: string
}

export function getPaymentOrders(params: Record<string, string | number | undefined>) {
  return get<PagedResult<PaymentOrderDto>>('/api/payment-orders', params)
}

export function createPaymentOrder(data: CreatePaymentOrderRequest) {
  return post<{ id: number }>('/api/payment-orders', data)
}

/* ---------- 退款 ---------- */

export interface PaymentRefundDto {
  id: number
  refundNo: string
  paymentOrderId: number
  refundStatus: string
  refundAmount: number
  refundReason: string
  refundedAt: string | null
  createdAt: string
}

export interface CreateRefundRequest {
  paymentOrderId: number
  refundAmount: number
  refundReason: string
}

export function getPaymentRefunds(params: Record<string, string | number | undefined>) {
  return get<PagedResult<PaymentRefundDto>>('/api/payment-refunds', params)
}

export function createRefund(data: CreateRefundRequest) {
  return post<{ id: number }>('/api/payment-refunds', data)
}
