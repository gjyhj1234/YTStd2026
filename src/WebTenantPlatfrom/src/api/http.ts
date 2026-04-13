import axios from 'axios'
import type {
  AxiosInstance,
  AxiosRequestConfig,
  InternalAxiosRequestConfig,
  AxiosResponse,
  AxiosError
} from 'axios'
import type { ApiResult, RequestOptions } from './http.types'

type ExtendedConfig = AxiosRequestConfig & { _options?: RequestOptions }

const http: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Pending requests map for duplicate prevention
const pendingRequests = new Map<string, AbortController>()

function getRequestKey(config: InternalAxiosRequestConfig): string {
  return `${config.method}:${config.url}:${JSON.stringify(config.params)}:${JSON.stringify(config.data)}`
}

// Request interceptor
http.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Token injection (placeholder — will be integrated with auth store in F0-4)
    // const authStore = useAuthStore()
    // const token = authStore.token
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`
    // }

    // Duplicate prevention
    const options = (config as ExtendedConfig)._options
    if (options?.preventDuplicate !== false) {
      const key = getRequestKey(config)
      if (pendingRequests.has(key)) {
        const controller = pendingRequests.get(key)!
        controller.abort()
      }
      const controller = new AbortController()
      config.signal = options?.signal || controller.signal
      pendingRequests.set(key, controller)
    }

    return config
  },
  (error: AxiosError) => {
    return Promise.reject(error)
  }
)

// Response interceptor
http.interceptors.response.use(
  (response: AxiosResponse<ApiResult>) => {
    const config = response.config
    const key = getRequestKey(config as InternalAxiosRequestConfig)
    pendingRequests.delete(key)

    const result = response.data

    // ApiResult unwrap: Code === 0 means success
    if (result.Code === 0) {
      return response
    }

    // Business error handling
    const options = (config as ExtendedConfig)._options
    if (!options?.skipErrorHandler) {
      handleBusinessError(result.Code, result.Message)
    }

    return Promise.reject(new BusinessError(result.Code, result.Message))
  },
  (error: AxiosError) => {
    if (error.config) {
      const key = getRequestKey(error.config as InternalAxiosRequestConfig)
      pendingRequests.delete(key)
    }

    // Cancelled requests are not handled
    if (axios.isCancel(error)) {
      return Promise.reject(error)
    }

    const options = (error.config as ExtendedConfig)?._options
    if (!options?.skipErrorHandler) {
      handleHttpError(error)
    }

    return Promise.reject(error)
  }
)

/** Business error class */
export class BusinessError extends Error {
  code: number
  constructor(code: number, message: string | null) {
    super(message || `Business error: ${code}`)
    this.code = code
    this.name = 'BusinessError'
  }
}

/** Business error code translation and i18n notification */
function handleBusinessError(code: number, _message: string | null): void {
  const errorKey = `error.${code}`

  import('../locales').then(({ i18n }) => {
    const { t } = i18n.global
    const translated = t(errorKey)
    const displayMessage = translated !== errorKey ? translated : (_message || t('未知错误'))

    import('devextreme/ui/notify').then(({ default: notify }) => {
      notify({ message: displayMessage, type: 'error', displayTime: 3000 })
    }).catch(() => {
      console.error('[handleBusinessError]', displayMessage)
    })
  })
}

/** HTTP error handling */
function handleHttpError(error: AxiosError): void {
  const status = error.response?.status

  import('../locales').then(({ i18n }) => {
    const { t } = i18n.global
    let message: string

    switch (status) {
      case 401:
        // Token expired or unauthenticated → redirect to login (handled in F0-4)
        message = t('请求失败')
        break
      case 403:
        message = t('无访问权限')
        break
      case 500:
        message = t('服务器错误，请稍后重试')
        break
      default:
        message = !error.response ? t('网络连接异常，请检查网络') : t('请求失败')
    }

    import('devextreme/ui/notify').then(({ default: notify }) => {
      notify({ message, type: 'error', displayTime: 3000 })
    }).catch(() => {
      console.error('[handleHttpError]', message)
    })
  })
}

/** Generic GET request */
export async function httpGet<T>(url: string, params?: Record<string, unknown>, options?: RequestOptions): Promise<T> {
  const response = await http.get<ApiResult<T>>(url, {
    params,
    _options: options
  } as ExtendedConfig)
  return response.data.Data
}

/** Generic POST request */
export async function httpPost<T>(url: string, data?: unknown, options?: RequestOptions): Promise<T> {
  const response = await http.post<ApiResult<T>>(url, data, {
    _options: options
  } as ExtendedConfig)
  return response.data.Data
}

/** Generic PUT request */
export async function httpPut<T>(url: string, data?: unknown, options?: RequestOptions): Promise<T> {
  const response = await http.put<ApiResult<T>>(url, data, {
    _options: options
  } as ExtendedConfig)
  return response.data.Data
}

/** Generic DELETE request */
export async function httpDelete<T>(url: string, options?: RequestOptions): Promise<T> {
  const response = await http.delete<ApiResult<T>>(url, {
    _options: options
  } as ExtendedConfig)
  return response.data.Data
}

/** File upload */
export async function httpUpload<T>(url: string, file: File, fieldName: string = 'file', options?: RequestOptions): Promise<T> {
  const formData = new FormData()
  formData.append(fieldName, file)

  const response = await http.post<ApiResult<T>>(url, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    timeout: 120000,
    _options: options
  } as ExtendedConfig)
  return response.data.Data
}

/** File download */
export async function httpDownload(url: string, filename: string, params?: Record<string, unknown>): Promise<void> {
  const response = await http.get(url, {
    params,
    responseType: 'blob',
    _options: { skipErrorHandler: true }
  } as ExtendedConfig)

  const blob = new Blob([response.data])
  const link = document.createElement('a')
  link.href = URL.createObjectURL(blob)
  link.download = filename
  link.click()
  URL.revokeObjectURL(link.href)
}

/** Create cancellable request (for component unmount cancellation) */
export function createCancelToken(): { signal: AbortSignal; cancel: () => void } {
  const controller = new AbortController()
  return {
    signal: controller.signal,
    cancel: () => controller.abort()
  }
}

export default http
