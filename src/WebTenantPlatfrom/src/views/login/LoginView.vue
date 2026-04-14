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

    <div class="login-container">
      <!-- 左侧品牌区域（桌面端可见） -->
      <div class="login-branding">
        <div class="branding-content">
          <i class="dx-icon-globe branding-icon"></i>
          <h2 class="branding-title">{{ $t('租户管理平台') }}</h2>
          <p class="branding-desc">{{ $t('请登录您的账号') }}</p>
        </div>
      </div>

      <!-- 右侧登录卡片 -->
      <div class="login-card">
        <div class="login-header">
          <h1 class="login-title">{{ $t('租户管理平台') }}</h1>
          <p class="login-subtitle">{{ $t('请登录您的账号') }}</p>
        </div>

        <form @submit.prevent="onSubmit">
          <DxForm
            ref="formRef"
            :key="`login-form-${showCaptcha ? 'captcha' : 'plain'}`"
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

            <!-- 滑动验证 - 多次失败后显示 -->
            <DxEmptyItem v-if="showCaptcha">
              <template #default>
                <div class="captcha-wrapper" data-testid="slider-captcha">
                  <p class="captcha-label">{{ $t('请拖动滑块完成验证') }}</p>
                  <div
                    class="captcha-track"
                    data-testid="captcha-track"
                  >
                    <div
                      class="captcha-fill"
                      :style="{ width: captchaProgress + '%' }"
                    />
                    <div
                      class="captcha-thumb"
                      :style="{ left: captchaProgress + '%' }"
                      data-testid="captcha-thumb"
                      @mousedown="onCaptchaMouseDown"
                      @touchstart.prevent="onCaptchaTouchStart"
                    >
                      <i class="dx-icon-chevrondoubleright"></i>
                    </div>
                    <span v-if="captchaProgress < 5" class="captcha-hint">{{ $t('向右拖动滑块') }}</span>
                    <span v-if="captchaVerified" class="captcha-success">{{ $t('验证通过') }}</span>
                  </div>
                </div>
              </template>
            </DxEmptyItem>

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
    </div>

    <!-- 底部版权 -->
    <div class="login-footer">
      &copy; 2026 YTStd
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onBeforeUnmount } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import DxForm, {
  DxSimpleItem,
  DxEmptyItem,
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

const CAPTCHA_THRESHOLD = 3 // 连续失败次数阈值
const CAPTCHA_COMPLETE_PCT = 92 // 滑块到达此百分比视为完成

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

// 滑块验证状态
const failedAttempts = ref(0)
const showCaptcha = computed(() => failedAttempts.value >= CAPTCHA_THRESHOLD)
const captchaProgress = ref(0)
const captchaVerified = ref(false)
let captchaDragging = false
let captchaStartX = 0
let captchaTrackWidth = 0

function onCaptchaMouseDown(e: MouseEvent) {
  if (captchaVerified.value) return
  captchaDragging = true
  captchaStartX = e.clientX - (captchaProgress.value / 100) * captchaTrackWidth
  const track = (e.target as HTMLElement).closest('.captcha-track') as HTMLElement | null
  if (track) captchaTrackWidth = track.clientWidth
  document.addEventListener('mousemove', onCaptchaMouseMove)
  document.addEventListener('mouseup', onCaptchaMouseUp)
}

function onCaptchaTouchStart(e: TouchEvent) {
  if (captchaVerified.value) return
  captchaDragging = true
  const touch = e.touches[0]
  const track = (e.target as HTMLElement).closest('.captcha-track') as HTMLElement | null
  if (track) captchaTrackWidth = track.clientWidth
  captchaStartX = touch.clientX - (captchaProgress.value / 100) * captchaTrackWidth
  document.addEventListener('touchmove', onCaptchaTouchMove, { passive: false })
  document.addEventListener('touchend', onCaptchaTouchEnd)
}

function onCaptchaMouseMove(e: MouseEvent) {
  if (!captchaDragging) return
  updateCaptchaProgress(e.clientX)
}

function onCaptchaTouchMove(e: TouchEvent) {
  if (!captchaDragging) return
  e.preventDefault()
  updateCaptchaProgress(e.touches[0].clientX)
}

function updateCaptchaProgress(clientX: number) {
  if (captchaTrackWidth <= 0) return
  const offset = clientX - captchaStartX
  let pct = (offset / captchaTrackWidth) * 100
  if (pct < 0) pct = 0
  if (pct > 100) pct = 100
  captchaProgress.value = pct
  if (pct >= CAPTCHA_COMPLETE_PCT) {
    captchaProgress.value = 100
    captchaVerified.value = true
    captchaDragging = false
    removeCaptchaListeners()
  }
}

function onCaptchaMouseUp() {
  if (!captchaVerified.value) {
    captchaProgress.value = 0
  }
  captchaDragging = false
  removeCaptchaListeners()
}

function onCaptchaTouchEnd() {
  if (!captchaVerified.value) {
    captchaProgress.value = 0
  }
  captchaDragging = false
  removeCaptchaListeners()
}

function removeCaptchaListeners() {
  document.removeEventListener('mousemove', onCaptchaMouseMove)
  document.removeEventListener('mouseup', onCaptchaMouseUp)
  document.removeEventListener('touchmove', onCaptchaTouchMove)
  document.removeEventListener('touchend', onCaptchaTouchEnd)
}

onBeforeUnmount(() => {
  removeCaptchaListeners()
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

  // 如果需要验证码但尚未通过
  if (showCaptcha.value && !captchaVerified.value) {
    notify({ message: t('请先完成滑块验证'), type: 'warning', displayTime: 3000 })
    return
  }

  logging.value = true

  try {
    const data = await loginApi({
      Username: formData.Username,
      Password: formData.Password
    })

    // 重置失败计数
    failedAttempts.value = 0
    captchaProgress.value = 0
    captchaVerified.value = false

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
    failedAttempts.value++

    // 失败后重置验证码状态（需要重新验证）
    if (captchaVerified.value) {
      captchaVerified.value = false
      captchaProgress.value = 0
    }

    // Import BusinessError type for instanceof check
    const { BusinessError } = await import('../../api/http')

    if (error instanceof BusinessError) {
      const msg = error.message || ''
      if (msg.indexOf('AuthCredentialsRequired') !== -1) {
        notify({ message: t('请输入用户名'), type: 'error', displayTime: 3000 })
      } else if (msg.indexOf('AuthInvalidCredentials') !== -1) {
        notify({ message: t('用户名或密码错误'), type: 'error', displayTime: 3000 })
      } else if (msg.indexOf('AuthAccountDisabled') !== -1) {
        notify({ message: t('账户已禁用'), type: 'error', displayTime: 3000 })
      } else if (msg.indexOf('AuthAccountLocked') !== -1) {
        notify({ message: t('账户已锁定'), type: 'error', displayTime: 3000 })
      } else {
        notify({ message: t('用户名或密码错误'), type: 'error', displayTime: 3000 })
      }
    } else {
      const axiosErr = error as { response?: { status?: number } }
      const status = axiosErr.response?.status
      if (status === 401) {
        notify({ message: t('用户名或密码错误'), type: 'error', displayTime: 3000 })
      } else if (status === 403) {
        notify({ message: t('账户已禁用'), type: 'error', displayTime: 3000 })
      } else if (status === 423) {
        notify({ message: t('账户已锁定'), type: 'error', displayTime: 3000 })
      } else {
        notify({ message: t('用户名或密码错误'), type: 'error', displayTime: 3000 })
      }
    }
  }
}
</script>

<style>
@import '../../styles/login.css';
</style>
