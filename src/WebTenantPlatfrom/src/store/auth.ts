import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface CurrentUser {
  Id: number
  Username: string
  DisplayName: string
  Email: string
  Permissions: string[]
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('auth_token'))
  const tenantId = ref<number | null>(null)
  const user = ref<CurrentUser | null>(null)

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
  }

  function clearAuth(): void {
    token.value = null
    tenantId.value = null
    user.value = null
    localStorage.removeItem('auth_token')
  }

  function hasPermission(code: string): boolean {
    return permissions.value.includes(code)
  }

  function hasAnyPermission(...codes: string[]): boolean {
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
