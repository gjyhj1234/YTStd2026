import { AUTH_ACCOUNT_DISABLED, AUTH_ACCOUNT_LOCKED, AUTH_TOKEN_INVALID, FORBIDDEN } from '@/constants/errorCodes'

/** API 错误封装 */
export class ApiError extends Error {
  code: number
  messageCode: number

  constructor(code: number, messageCode: number) {
    super(String(code))
    this.code = code
    this.messageCode = messageCode
  }
}

/** 根据错误码执行特殊处理 */
export function handleApiError(error: ApiError): void {
  switch (error.code) {
    case AUTH_ACCOUNT_DISABLED:
    case AUTH_ACCOUNT_LOCKED:
    case AUTH_TOKEN_INVALID:
      localStorage.removeItem('platform_token')
      localStorage.removeItem('platform_user')
      window.location.href = '/login'
      break
    case FORBIDDEN:
      // 权限不足 - 可以在具体视图中处理
      break
    default:
      break
  }
}
