import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface CurrentUser {
  Id: number
  Username: string
  DisplayName: string
  Email: string
  Permissions: string[]
  IsSuperAdmin: boolean
}

const USER_STORAGE_KEY = 'auth_user'

function loadUserFromStorage(): CurrentUser | null {
  try {
    const raw = localStorage.getItem(USER_STORAGE_KEY)
    if (raw) {
      return JSON.parse(raw) as CurrentUser
    }
  } catch {
    // ignore parse errors
  }
  return null
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('auth_token'))
  const tenantId = ref<number | null>(null)
  const user = ref<CurrentUser | null>(loadUserFromStorage())

  const isAuthenticated = computed(() => !!token.value)
  const permissions = computed(() => user.value?.Permissions || [])

  function setToken(newToken: string): void {
    token.value = newToken
    localStorage.setItem('auth_token', newToken)
  }

  function setTenantId(id: number): void {
    tenantId.value = id
  }

  function setUser(newUser: CurrentUser): void {
    user.value = newUser
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(newUser))
  }

  function clearAuth(): void {
    token.value = null
    tenantId.value = null
    user.value = null
    localStorage.removeItem('auth_token')
    localStorage.removeItem(USER_STORAGE_KEY)
  }

  function hasPermission(code: string): boolean {
    if (user.value?.IsSuperAdmin) return true
    return permissions.value.includes(code)
  }

  function hasAnyPermission(...codes: string[]): boolean {
    if (user.value?.IsSuperAdmin) return true
    return codes.some(code => permissions.value.includes(code))
  }

  return {
    token,
    tenantId,
    user,
    isAuthenticated,
    permissions,
    setToken,
    setTenantId,
    setUser,
    clearAuth,
    hasPermission,
    hasAnyPermission
  }
})
