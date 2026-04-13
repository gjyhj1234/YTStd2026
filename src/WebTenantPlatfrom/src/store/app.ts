import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAppStore = defineStore('app', () => {
  const loading = ref(false)

  function setLoading(value: boolean): void {
    loading.value = value
  }

  return {
    loading,
    setLoading
  }
})
