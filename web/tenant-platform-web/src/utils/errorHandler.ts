import { AUTH_ACCOUNT_DISABLED, AUTH_ACCOUNT_LOCKED, AUTH_TOKEN_INVALID, FORBIDDEN } from '@/constants/errorCodes'
import { notifyError } from '@/composables/useNotify'

/** API 错误封装 */
export class ApiError extends Error {
  code: number
  messageCode: number
  localizedMessage: string

  constructor(code: number, messageCode: number, localizedMessage?: string) {
    super(localizedMessage || String(code))
    this.code = code
    this.messageCode = messageCode
    this.localizedMessage = localizedMessage || String(code)
  }
}

/** 根据错误码执行特殊处理并显示错误通知 */
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
      notifyError(error.localizedMessage)
      break
    default:
      notifyError(error.localizedMessage)
      break
  }
}
