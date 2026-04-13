/** Backend unified response format */
export interface ApiResult<T = void> {
  Code: number
  Data: T
  Message: string | null
}

/** Paged response format */
export interface PagedResult<T> {
  Items: T[]
  Total: number
  Page: number
  PageSize: number
}

/** Paged query parameters */
export interface PagedQuery {
  Page: number
  PageSize: number
  Keyword?: string
}

/** HTTP request configuration extension */
export interface RequestOptions {
  /** Skip unified error handling (default false) */
  skipErrorHandler?: boolean
  /** Skip loading management (default false) */
  skipLoading?: boolean
  /** Prevent duplicate submissions (default true) */
  preventDuplicate?: boolean
  /** Request cancel signal (for cancellation on unmount) */
  signal?: AbortSignal
}
