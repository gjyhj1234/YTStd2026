<template>
  <div class="login-page">
    <!-- 语言切换 - 右上角 -->
    <div class="login-lang-switcher">
      <DxSelectBox
        :items="languageOptions"
        display-expr="label"
        value-expr="value"
        :value="currentLocale"
        :width="140"
        styling-mode="underlined"
        @value-changed="onLanguageChanged"
      />
    </div>

    <!-- 登录卡片 -->
    <div class="login-card">
      <div class="login-header">
        <h1 class="login-title">{{ $t('租户管理平台') }}</h1>
        <p class="login-subtitle">{{ $t('请登录您的账号') }}</p>
      </div>

      <form @submit.prevent="onSubmit">
        <DxForm
          ref="formRef"
          :form-data="formData"
          :disabled="logging"
          label-mode="static"
          :show-colon-after-label="false"
        >
          <DxSimpleItem
            data-field="Username"
            editor-type="dxTextBox"
            :label="{ text: $t('用户名') }"
            :editor-options="usernameEditorOptions"
          >
            <DxRequiredRule :message="$t('请输入用户名')" />
          </DxSimpleItem>

          <DxSimpleItem
            data-field="Password"
            editor-type="dxTextBox"
            :label="{ text: $t('密码') }"
            :editor-options="passwordEditorOptions"
          >
            <DxRequiredRule :message="$t('请输入密码')" />
            <DxStringLengthRule
              :min="6"
              :message="$t('密码长度至少 6 个字符')"
            />
          </DxSimpleItem>

          <DxButtonItem>
            <DxButtonOptions
              width="100%"
              type="default"
              styling-mode="contained"
              template="loginBtnTemplate"
              :use-submit-behavior="true"
            />
          </DxButtonItem>

          <template #loginBtnTemplate>
            <div>
              <span class="dx-button-text">
                <DxLoadIndicator
                  v-if="logging"
                  width="24px"
                  height="24px"
                  :visible="true"
                />
                <span v-if="logging">{{ $t('登录中') }}</span>
                <span v-if="!logging">{{ $t('登录') }}</span>
              </span>
            </div>
          </template>
        </DxForm>
      </form>
    </div>

    <!-- 底部版权 -->
    <div class="login-footer">
      &copy; 2026 YTStd
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import DxForm, {
  DxSimpleItem,
  DxRequiredRule,
  DxStringLengthRule,
  DxButtonItem,
  DxButtonOptions
} from 'devextreme-vue/form'
import { DxSelectBox } from 'devextreme-vue/select-box'
import DxLoadIndicator from 'devextreme-vue/load-indicator'
import notify from 'devextreme/ui/notify'

import { loginApi } from '../../api/auth'
import { useAuthStore } from '../../store/auth'
import type { LoginReqDTO } from '../../types/auth'

const router = useRouter()
const route = useRoute()
const { t, locale } = useI18n()
const authStore = useAuthStore()

const formRef = ref<InstanceType<typeof DxForm> | null>(null)
const logging = ref(false)

const formData = reactive<LoginReqDTO>({
  Username: '',
  Password: ''
})

const usernameEditorOptions = computed(() => ({
  placeholder: t('请输入用户名'),
  stylingMode: 'filled' as const,
  onEnterKey: onSubmit
}))

const passwordEditorOptions = computed(() => ({
  placeholder: t('请输入密码'),
  stylingMode: 'filled' as const,
  mode: 'password' as const,
  onEnterKey: onSubmit
}))

const currentLocale = computed(() => locale.value)

const languageOptions = [
  { value: 'zh-CN', label: '简体中文' },
  { value: 'en-US', label: 'English' },
  { value: 'ja-JP', label: '日本語' },
  { value: 'ms-MY', label: 'Bahasa Melayu' },
  { value: 'zh-TW', label: '繁體中文' }
]

function onLanguageChanged(e: { value: string }) {
  locale.value = e.value
  localStorage.setItem('locale', e.value)
}

async function onSubmit() {
  const formInstance = formRef.value?.instance
  if (!formInstance) return

  const validationResult = formInstance.validate()
  if (!validationResult.isValid) return

  logging.value = true

  try {
    const data = await loginApi({
      Username: formData.Username,
      Password: formData.Password
    })

    // 存储 Token 和用户信息
    authStore.setToken(data.Token)
    authStore.setUser({
      Id: data.UserId,
      Username: data.Username,
      DisplayName: data.DisplayName,
      Email: '',
      Permissions: data.Permissions
    })

    // RequirePasswordReset 检查
    if (data.RequirePasswordReset) {
      router.push('/change-password')
      return
    }

    // 跳转到 redirect 参数或 dashboard
    const redirect = route.query.redirect as string | undefined
    router.push(redirect || '/dashboard')
  } catch (error: unknown) {
    logging.value = false

    // 从 BusinessError 或 AxiosError 中提取错误码
    const bizError = error as { code?: number; response?: { status?: number } }

    if (bizError.code) {
      // BusinessError — 后端返回的业务错误码
      const errorCodeMap: Record<string, string> = {
        AuthCredentialsRequired: '请输入用户名',
        AuthInvalidCredentials: '用户名或密码错误',
        AuthAccountDisabled: '账户已禁用',
        AuthAccountLocked: '账户已锁定'
      }

      // 后端 Code 为数字，Message 为错误码字符串
      const errMsg = (error as { message?: string }).message || ''
      let matched = false
      for (const [key, i18nKey] of Object.entries(errorCodeMap)) {
        if (errMsg.includes(key)) {
          notify({ message: t(i18nKey), type: 'error', displayTime: 3000 })
          matched = true
          break
        }
      }

      if (!matched) {
        notify({ message: t('用户名或密码错误'), type: 'error', displayTime: 3000 })
      }
    } else if (bizError.response?.status === 401) {
      notify({ message: t('用户名或密码错误'), type: 'error', displayTime: 3000 })
    } else if (bizError.response?.status === 403) {
      notify({ message: t('账户已禁用'), type: 'error', displayTime: 3000 })
    } else if (bizError.response?.status === 423) {
      notify({ message: t('账户已锁定'), type: 'error', displayTime: 3000 })
    } else {
      notify({ message: t('用户名或密码错误'), type: 'error', displayTime: 3000 })
    }
  }
}
</script>

<style>
@import '../../styles/login.css';
</style>
