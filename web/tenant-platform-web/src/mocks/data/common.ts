export function ok<T>(data: T, message = '操作成功') {
  return { success: true, message, data, traceId: '' }
}

export function fail(message = '操作失败') {
  return { success: false, message, data: null, traceId: '' }
}

export function paged<T>(items: T[], page: number, pageSize: number) {
  const start = (page - 1) * pageSize
  const slice = items.slice(start, start + pageSize)
  return ok({
    items: slice,
    total: items.length,
    page,
    pageSize,
    totalPages: Math.ceil(items.length / pageSize),
  })
}

export function getPageParams(url: URL) {
  const page = Number(url.searchParams.get('page') ?? '1')
  const pageSize = Number(url.searchParams.get('pageSize') ?? '20')
  return { page, pageSize }
}
