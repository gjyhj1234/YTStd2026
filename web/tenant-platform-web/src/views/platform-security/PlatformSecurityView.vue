<template>
  <div>
    <div class="page-header">
      <h2>{{ $t('平台安全中心') }}</h2>
      <div class="page-header-actions">
        <PageHelpEntry @click="showGuide = true" />
      </div>
    </div>

    <FunctionDescriptionCard
      :purpose="$t('安全中心集中展示安全策略配置')"
      :data-scope="$t('平台全局安全策略配置')"
      :permission-note="$t('需要安全查看权限')"
      :risk-note="$t('安全策略变更将影响所有管理员')"
      :collapsible="true"
    />

    <DxLoadPanel :visible="isLoading" :position="{ of: '.security-dashboard' }" />

    <div class="security-dashboard">
      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-key" />
          <h3>{{ $t('密码策略') }}</h3>
        </div>
        <div class="security-card-body">
          <div class="security-item">
            <span class="security-label">{{ $t('最小长度') }}</span>
            <span class="security-value">{{ securityInfo.passwordPolicy.minLength }} {{ $t('位') }}</span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('要求大写字母') }}</span>
            <span class="security-value">{{ securityInfo.passwordPolicy.requireUppercase ? $t('是') : $t('否') }}</span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('要求小写字母') }}</span>
            <span class="security-value">{{ securityInfo.passwordPolicy.requireLowercase ? $t('是') : $t('否') }}</span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('要求数字') }}</span>
            <span class="security-value">{{ securityInfo.passwordPolicy.requireDigit ? $t('是') : $t('否') }}</span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('要求特殊字符') }}</span>
            <span class="security-value">{{ securityInfo.passwordPolicy.requireSpecialChar ? $t('是') : $t('否') }}</span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('密码过期天数') }}</span>
            <span class="security-value">{{ securityInfo.passwordPolicy.expirationDays }} {{ $t('天') }}</span>
          </div>
        </div>
      </div>

      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-globe" />
          <h3>{{ $t('IP白名单') }}</h3>
        </div>
        <div class="security-card-body">
          <div class="security-item">
            <span class="security-label">{{ $t('启用状态') }}</span>
            <StatusTag :status="securityInfo.ipWhitelist.Enabled ? 'Active' : 'Disabled'" />
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('白名单条目数') }}</span>
            <span class="security-value">{{ securityInfo.ipWhitelist.entryCount }} {{ $t('条') }}</span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('最近更新') }}</span>
            <span class="security-value">{{ formatDateTime(securityInfo.ipWhitelist.lastUpdatedAt) }}</span>
          </div>
        </div>
      </div>

      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-lock" />
          <h3>{{ $t('多因素认证MFA') }}</h3>
        </div>
        <div class="security-card-body">
          <div class="security-item">
            <span class="security-label">{{ $t('启用状态') }}</span>
            <StatusTag :status="securityInfo.mfa.Enabled ? 'Active' : 'Disabled'" />
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('强制要求') }}</span>
            <span class="security-value">{{ securityInfo.mfa.enforced ? $t('是') : $t('否') }}</span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('支持方式') }}</span>
            <span class="security-value">{{ securityInfo.mfa.supportedMethods.join('、') || $t('未配置') }}</span>
          </div>
        </div>
      </div>
    </div>

    <OperationGuideDrawer
      v-model:visible="showGuide"
      :title="$t('安全中心操作指引')"
      :entry-path="$t('安全中心入口路径')"
      :steps="guideSteps"
      :field-notes="guideFieldNotes"
      :error-notes="guideErrorNotes"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { DxLoadPanel } from 'devextreme-vue/load-panel'
import StatusTag from '@/components/StatusTag.vue'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import { formatDateTime } from '@/utils/format'

interface SecurityInfo {
  passwordPolicy: {
    minLength: number
    requireUppercase: boolean
    requireLowercase: boolean
    requireDigit: boolean
    requireSpecialChar: boolean
    expirationDays: number
  }
  ipWhitelist: {
    Enabled: boolean
    entryCount: number
    lastUpdatedAt: string
  }
  mfa: {
    Enabled: boolean
    enforced: boolean
    supportedMethods: string[]
  }
}

const showGuide = ref(false)
const isLoading = ref(false)
const { t } = useI18n()

const securityInfo = reactive<SecurityInfo>({
  passwordPolicy: {
    minLength: 8,
    requireUppercase: true,
    requireLowercase: true,
    requireDigit: true,
    requireSpecialChar: false,
    expirationDays: 90,
  },
  ipWhitelist: {
    Enabled: false,
    entryCount: 0,
    lastUpdatedAt: '',
  },
  mfa: {
    Enabled: false,
    enforced: false,
    supportedMethods: [],
  },
})

async function loadData() {
  isLoading.value = true
  try {
    // Security API will be integrated in a future phase
  } finally {
    isLoading.value = false
  }
}

const guideSteps = computed(() => [
  t('进入安全中心查看安全策略概览'),
  t('查看密码策略了解复杂度和过期要求'),
  t('查看IP白名单状态了解访问控制'),
  t('查看MFA配置了解多因素认证要求'),
])
const guideFieldNotes = computed(() => [
  t('密码策略控制密码复杂度和过期时间'),
  t('IP白名单启用后仅允许白名单IP访问'),
  t('MFA多因素认证提供额外安全保护'),
])
const guideErrorNotes = computed(() => [
  t('安全策略变更后需所有在线用户重新登录'),
  t('启用IP白名单前请确保添加管理员IP'),
])

onMounted(loadData)
</script>

<style scoped>
.security-dashboard {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 16px;
}
.security-card {
  padding: 20px;
}
.security-card-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 16px;
}
.security-card-header h3 {
  font-size: 16px;
  margin: 0;
  color: var(--dx-color-text, #333);
}
.security-card-header i {
  font-size: 20px;
  color: var(--dx-color-primary, #1976d2);
}
.security-card-body {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.security-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 13px;
  line-height: 1.6;
}
.security-label {
  color: var(--dx-color-text-secondary, #666);
}
.security-value {
  color: var(--dx-color-text, #333);
  font-weight: 500;
}
</style>
