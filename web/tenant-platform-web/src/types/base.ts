/** 统一响应结构 */
export interface ApiResult<T = void> {
  Code: number
  Message: string
  Data?: T
}

/** 分页请求参数 */
export interface PagedRequest {
  Page?: number
  PageSize?: number
  Keyword?: string
  Status?: string
}

/** 分页响应结构 */
export interface PagedResult<T> {
  Items: T[]
  Total: number
  Page: number
  PageSize: number
  TotalPages: number
}
