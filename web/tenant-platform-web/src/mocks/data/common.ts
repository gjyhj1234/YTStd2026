export function ok<T>(data: T) {
  return { Code: 0, Message: 0, Data: data }
}

export function fail(code = 50002) {
  return { Code: code, Message: code, Data: null }
}

export function paged<T>(items: T[], page: number, pageSize: number) {
  const start = (page - 1) * pageSize
  const slice = items.slice(start, start + pageSize)
  return ok({
    Items: slice,
    Total: items.length,
    Page: page,
    PageSize: pageSize,
    TotalPages: Math.ceil(items.length / pageSize),
  })
}

export function getPageParams(url: URL) {
  const page = Number(url.searchParams.get('Page') ?? url.searchParams.get('page') ?? '1')
  const pageSize = Number(url.searchParams.get('PageSize') ?? url.searchParams.get('pageSize') ?? '20')
  return { page, pageSize }
}
