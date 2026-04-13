# axios 标准化实现规范

> 本文件是 axios HTTP 封装的可执行实现规范，不是原则说明。
> 新前端项目（`src/WebTenantPlatfrom`）统一使用 axios，禁止使用原生 fetch。
> Agent 必须按照本文件的结构和代码模板实现 HTTP 封装。

---

## 一、依赖安装

```bash
cd src/WebTenantPlatfrom
npm install axios
```

---

## 二、文件结构

```
src/WebTenantPlatfrom/src/
├── api/
│   ├── http.ts                  ← axios 实例与拦截器（核心文件）
│   ├── http.types.ts            ← HTTP 相关类型定义
│   ├── platform-users.ts       ← 业务模块 API（示例）
│   └── platform-roles.ts       ← 业务模块 API（示例）
```

---

## 三、类型定义 — `http.types.ts`

```typescript
/** 后端统一返回格式 */
export interface ApiResult<T = void> {
  Code: number
  Data: T
  Message: string | null
}

/** 分页返回格式 */
export interface PagedResult<T> {
  Items: T[]
  Total: number
  Page: number
  PageSize: number
}

/** 分页查询参数 */
export interface PagedQuery {
  Page: number
  PageSize: number
  Keyword?: string
}

/** HTTP 请求配置扩展 */
export interface RequestOptions {
  /** 是否跳过统一错误处理（默认 false） */
  skipErrorHandler?: boolean
  /** 是否跳过 loading 管理（默认 false） */
  skipLoading?: boolean
  /** 是否防重复提交（默认 true） */
  preventDuplicate?: boolean
  /** 请求取消 token（用于组件卸载时取消） */
  signal?: AbortSignal
}
```

---

## 四、axios 实例创建 — `http.ts`

### 4.1 实例创建

```typescript
import axios from 'axios'
import type { AxiosInstance, AxiosRequestConfig, InternalAxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'
import type { ApiResult, RequestOptions } from './http.types'
import { useAuthStore } from '@/store/auth'
import { useAppStore } from '@/store/app'
import router from '@/router'
import { i18n } from '@/locales'

const http: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
})
```

### 4.2 baseURL 管理

```typescript
// 通过 Vite 环境变量管理
// .env.development
VITE_API_BASE_URL=/api

// .env.production
VITE_API_BASE_URL=/api

// .env.staging（如需要）
VITE_API_BASE_URL=https://staging-api.example.com/api
```

### 4.3 请求拦截器

```typescript
// 防重复提交：记录进行中的请求
const pendingRequests = new Map<string, AbortController>()

function getRequestKey(config: InternalAxiosRequestConfig): string {
  return `${config.method}:${config.url}:${JSON.stringify(config.params)}:${JSON.stringify(config.data)}`
}

http.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const authStore = useAuthStore()

    // Token 注入
    const token = authStore.token
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    // 租户标识注入（如适用）
    const tenantId = authStore.tenantId
    if (tenantId) {
      config.headers['X-Tenant-Id'] = String(tenantId)
    }

    // 语言标识注入
    const locale = i18n.global.locale.value
    if (locale) {
      config.headers['Accept-Language'] = locale
    }

    // 防重复提交
    const options = (config as AxiosRequestConfig & { _options?: RequestOptions })._options
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
```

### 4.4 响应拦截器

```typescript
http.interceptors.response.use(
  (response: AxiosResponse<ApiResult>) => {
    const config = response.config
    const key = getRequestKey(config as InternalAxiosRequestConfig)
    pendingRequests.delete(key)

    const result = response.data

    // ApiResult 拆包：Code === 0 表示成功
    if (result.Code === 0) {
      return response
    }

    // 业务错误处理
    const options = (config as AxiosRequestConfig & { _options?: RequestOptions })._options
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

    // 取消请求不处理
    if (axios.isCancel(error)) {
      return Promise.reject(error)
    }

    const options = ((error.config as AxiosRequestConfig & { _options?: RequestOptions })?._options)
    if (!options?.skipErrorHandler) {
      handleHttpError(error)
    }

    return Promise.reject(error)
  }
)
```

### 4.5 错误处理

```typescript
/** 业务错误类 */
export class BusinessError extends Error {
  code: number
  constructor(code: number, message: string | null) {
    super(message || `Business error: ${code}`)
    this.code = code
    this.name = 'BusinessError'
  }
}

/** 业务错误码翻译与国际化提示 */
function handleBusinessError(code: number, _message: string | null): void {
  const { t } = i18n.global

  // 通过错误码查找国际化 key
  // 错误码在 src/locales/generated/ 中由 Generator 生成
  const errorKey = `error.${code}`
  const translated = t(errorKey)

  // 如果找到翻译则使用翻译文本，否则使用后端返回的 message
  const displayMessage = translated !== errorKey ? translated : (_message || t('未知错误'))

  // 显示错误提示（使用 DevExtreme notify）
  import('devextreme/ui/notify').then(({ default: notify }) => {
    notify({ message: displayMessage, type: 'error', displayTime: 3000 })
  })
}

/** HTTP 错误处理 */
function handleHttpError(error: AxiosError): void {
  const { t } = i18n.global
  const status = error.response?.status

  let message: string
  switch (status) {
    case 401:
      // Token 过期或未认证 → 跳转登录
      handleUnauthorized()
      return
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
  })
}

/** 401 处理 */
function handleUnauthorized(): void {
  const authStore = useAuthStore()
  authStore.clearAuth()
  router.push({
    path: '/login',
    query: { redirect: router.currentRoute.value.fullPath }
  })
}
```

---

## 五、封装导出方法

```typescript
/** 通用 GET 请求 */
export async function httpGet<T>(url: string, params?: Record<string, unknown>, options?: RequestOptions): Promise<T> {
  const response = await http.get<ApiResult<T>>(url, {
    params,
    _options: options,
  } as AxiosRequestConfig & { _options?: RequestOptions })
  return response.data.Data
}

/** 通用 POST 请求 */
export async function httpPost<T>(url: string, data?: unknown, options?: RequestOptions): Promise<T> {
  const response = await http.post<ApiResult<T>>(url, data, {
    _options: options,
  } as AxiosRequestConfig & { _options?: RequestOptions })
  return response.data.Data
}

/** 通用 PUT 请求 */
export async function httpPut<T>(url: string, data?: unknown, options?: RequestOptions): Promise<T> {
  const response = await http.put<ApiResult<T>>(url, data, {
    _options: options,
  } as AxiosRequestConfig & { _options?: RequestOptions })
  return response.data.Data
}

/** 通用 DELETE 请求 */
export async function httpDelete<T>(url: string, options?: RequestOptions): Promise<T> {
  const response = await http.delete<ApiResult<T>>(url, {
    _options: options,
  } as AxiosRequestConfig & { _options?: RequestOptions })
  return response.data.Data
}
```

---

## 六、上传与下载

### 6.1 文件上传

```typescript
/** 文件上传 */
export async function httpUpload<T>(url: string, file: File, fieldName: string = 'file', options?: RequestOptions): Promise<T> {
  const formData = new FormData()
  formData.append(fieldName, file)

  const response = await http.post<ApiResult<T>>(url, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    timeout: 120000, // 上传超时 2 分钟
    _options: options,
  } as AxiosRequestConfig & { _options?: RequestOptions })
  return response.data.Data
}
```

### 6.2 文件下载

```typescript
/** 文件下载 */
export async function httpDownload(url: string, filename: string, params?: Record<string, unknown>): Promise<void> {
  const response = await http.get(url, {
    params,
    responseType: 'blob',
    _options: { skipErrorHandler: true },
  } as AxiosRequestConfig & { _options?: RequestOptions })

  const blob = new Blob([response.data])
  const link = document.createElement('a')
  link.href = URL.createObjectURL(blob)
  link.download = filename
  link.click()
  URL.revokeObjectURL(link.href)
}
```

---

## 七、请求取消

```typescript
/** 创建可取消的请求（用于组件 onUnmounted 时取消） */
export function createCancelToken(): { signal: AbortSignal; cancel: () => void } {
  const controller = new AbortController()
  return {
    signal: controller.signal,
    cancel: () => controller.abort(),
  }
}

// 使用示例（在 Vue 组件中）：
// const { signal, cancel } = createCancelToken()
// onUnmounted(() => cancel())
// const data = await httpGet('/api/users', { Page: 1 }, { signal })
```

---

## 八、按钮 loading / 页面 loading / 表单 submitting 状态协作

### 8.1 设计原则

| 状态 | 管理位置 | 作用范围 |
|------|---------|---------|
| `pageLoading` | 页面组件 `ref<boolean>` | 控制 `DxLoadPanel` 显示 |
| `submitting` | 表单组件 `ref<boolean>` | 控制提交按钮 disabled + loading |
| `buttonLoading` | 行操作 `ref<Record<string, boolean>>` | 控制单行操作按钮 loading |

### 8.2 实现模板

```typescript
// 页面级 loading
const pageLoading = ref(false)

async function loadData() {
  pageLoading.value = true
  try {
    const data = await httpGet<PagedResult<UserDTO>>('/api/platform-users', queryParams)
    // ...
  } finally {
    pageLoading.value = false
  }
}

// 表单级 submitting
const submitting = ref(false)

async function handleSubmit() {
  if (submitting.value) return  // 防重复
  submitting.value = true
  try {
    await httpPost('/api/platform-users', formData)
    notifySuccess('创建成功')
  } finally {
    submitting.value = false
  }
}

// 行操作 loading
const rowLoading = ref<Record<string, boolean>>({})

async function handleEnable(id: number) {
  const key = `enable-${id}`
  if (rowLoading.value[key]) return
  rowLoading.value[key] = true
  try {
    await httpPut(`/api/platform-users/${id}/enable`)
    notifySuccess('启用成功')
    await loadData()
  } finally {
    rowLoading.value[key] = false
  }
}
```

---

## 九、业务 API 封装示例

```typescript
// src/api/platform-users.ts
import { httpGet, httpPost, httpPut, httpDelete } from './http'
import type { PagedResult, PagedQuery } from './http.types'
import type { PlatformUserRepDTO, PlatformUserCreateReqDTO, PlatformUserUpdateReqDTO } from '@/types/platform-users'

/** 查询用户列表 */
export function getUserList(params: PagedQuery & { Status?: number }) {
  return httpGet<PagedResult<PlatformUserRepDTO>>('/platform-users', params)
}

/** 获取用户详情 */
export function getUserDetail(id: number) {
  return httpGet<PlatformUserRepDTO>(`/platform-users/${id}`)
}

/** 创建用户 */
export function createUser(data: PlatformUserCreateReqDTO) {
  return httpPost<number>('/platform-users', data)
}

/** 更新用户 */
export function updateUser(id: number, data: PlatformUserUpdateReqDTO) {
  return httpPut<void>(`/platform-users/${id}`, data)
}

/** 删除用户 */
export function deleteUser(id: number) {
  return httpDelete<void>(`/platform-users/${id}`)
}

/** 启用用户 */
export function enableUser(id: number) {
  return httpPut<void>(`/platform-users/${id}/enable`)
}

/** 禁用用户 */
export function disableUser(id: number) {
  return httpPut<void>(`/platform-users/${id}/disable`)
}

/** 重置密码 */
export function resetUserPassword(id: number) {
  return httpPut<void>(`/platform-users/${id}/reset-password`)
}

/** 检查用户名是否存在 */
export function checkUsernameExists(username: string) {
  return httpGet<boolean>('/platform-users/check-username-exists', { username })
}
```

---

## 十、Code Review 检查点

```bash
# 1. 确认无 fetch 调用（结果必须为 0）
grep -rn 'fetch(' src/WebTenantPlatfrom/src/ | grep -v 'node_modules' | grep -v 'import.meta.glob'

# 2. 确认 axios 实例存在
grep -rn 'axios.create' src/WebTenantPlatfrom/src/api/http.ts

# 3. 确认请求拦截器存在
grep -rn 'interceptors.request.use' src/WebTenantPlatfrom/src/api/http.ts

# 4. 确认响应拦截器存在
grep -rn 'interceptors.response.use' src/WebTenantPlatfrom/src/api/http.ts

# 5. 确认 Token 注入存在
grep -rn 'Authorization' src/WebTenantPlatfrom/src/api/http.ts

# 6. 确认 401 处理存在
grep -rn '401' src/WebTenantPlatfrom/src/api/http.ts

# 7. 确认 ApiResult 拆包
grep -rn 'Code === 0' src/WebTenantPlatfrom/src/api/http.ts

# 8. 确认业务 API 使用封装方法（不直接使用 axios 实例）
grep -rn 'http.get\|http.post\|http.put\|http.delete' src/WebTenantPlatfrom/src/api/ | grep -v 'http.ts'
# 结果应为 0，业务 API 文件应使用 httpGet/httpPost/httpPut/httpDelete
```
