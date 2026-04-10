<template>
  <div class="login-page">
    <div class="login-card">
      <h1>{{ $t('app.title') }}</h1>
      <DxForm
        ref="formRef"
        :form-data="formData"
        :col-count="1"
        label-mode="floating"
        :disabled="isLoading"
      >
        <DxSimpleItem
          data-field="Username"
          :editor-options="{ placeholder: $t('请输入用户名') }"
          :validation-rules="usernameRules"
        >
          <DxLabel :text="$t('用户名')" />
        </DxSimpleItem>
        <DxSimpleItem
          data-field="Password"
          :editor-options="{ mode: 'password', placeholder: $t('请输入密码') }"
          :validation-rules="passwordRules"
        >
          <DxLabel :text="$t('密码')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions
            :text="isLoading ? $t('登录中') : $t('登 录')"
            type="default"
            :use-submit-behavior="false"
            width="100%"
            :disabled="isLoading"
            @click="handleLogin"
          />
        </DxButtonItem>
      </DxForm>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/store/auth'
import { DxForm, DxSimpleItem, DxLabel, DxButtonItem, DxButtonOptions } from 'devextreme-vue/form'
import { notifyError } from '@/composables/useNotify'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const { t } = useI18n()

const formRef = ref<InstanceType<typeof DxForm> | null>(null)
const formData = reactive({ Username: '', Password: '' })
const isLoading = ref(false)

const usernameRules = computed(() => [
  { type: 'required' as const, message: t('用户名不能为空') },
])

const passwordRules = computed(() => [
  { type: 'required' as const, message: t('密码不能为空') },
  { type: 'stringLength' as const, min: 6, message: t('密码长度至少6位') },
])

async function handleLogin() {
  const formInstance = formRef.value?.instance
  if (formInstance) {
    const result = formInstance.validate()
    if (!result.isValid) return
  }

  isLoading.value = true
  try {
    await authStore.login(formData.Username, formData.Password)
    const redirect = (route.query.redirect as string) || '/dashboard'
    router.push(redirect)
  } catch (e: unknown) {
    notifyError(e instanceof Error ? e.message : t('登录失败'))
  } finally {
    isLoading.value = false
  }
}
</script>
