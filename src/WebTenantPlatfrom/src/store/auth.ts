import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const tenantId = ref<number | null>(null)

  function setToken(newToken: string): void {
    token.value = newToken
  }

  function setTenantId(id: number): void {
    tenantId.value = id
  }

  function clearAuth(): void {
    token.value = null
    tenantId.value = null
  }

  return {
    token,
    tenantId,
    setToken,
    setTenantId,
    clearAuth
  }
})
