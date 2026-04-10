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

    <div class="security-dashboard">
      <!-- 修改密码 -->
      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-key" />
          <h3>{{ $t('修改密码') }}</h3>
        </div>
        <div class="security-card-body">
          <DxForm
            ref="changePwdFormRef"
            :form-data="changePwdForm"
            :col-count="1"
            label-mode="floating"
          >
            <DxSimpleItem data-field="OldPassword" :editor-options="{ mode: 'password' }" :validation-rules="oldPasswordRules">
              <DxLabel :text="$t('当前密码')" />
            </DxSimpleItem>
            <DxSimpleItem data-field="NewPassword" :editor-options="{ mode: 'password' }" :validation-rules="newPasswordRules">
              <DxLabel :text="$t('新密码')" />
            </DxSimpleItem>
            <DxSimpleItem data-field="ConfirmPassword" :editor-options="{ mode: 'password' }" :validation-rules="confirmPasswordRules">
              <DxLabel :text="$t('确认新密码')" />
            </DxSimpleItem>
            <DxButtonItem>
              <DxButtonOptions
                :text="$t('修改密码')"
                type="default"
                :use-submit-behavior="false"
                @click="handleChangePassword"
              />
            </DxButtonItem>
          </DxForm>
        </div>
      </div>

      <!-- 密码策略（只读展示） -->
      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-info" />
          <h3>{{ $t('密码策略') }}</h3>
          <span class="security-badge">{{ $t('系统默认配置') }}</span>
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

      <!-- IP 白名单（开发中） -->
      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-globe" />
          <h3>{{ $t('IP白名单') }}</h3>
          <span class="security-badge developing">{{ $t('功能开发中') }}</span>
        </div>
        <div class="security-card-body">
          <div class="security-placeholder">
            <i class="dx-icon dx-icon-clock" />
            <p>{{ $t('IP白名单功能正在开发中请稍后关注更新') }}</p>
          </div>
        </div>
      </div>

      <!-- MFA（开发中） -->
      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-lock" />
          <h3>{{ $t('多因素认证MFA') }}</h3>
          <span class="security-badge developing">{{ $t('功能开发中') }}</span>
        </div>
        <div class="security-card-body">
          <div class="security-placeholder">
            <i class="dx-icon dx-icon-clock" />
            <p>{{ $t('多因素认证功能正在开发中请稍后关注更新') }}</p>
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
import { ref, reactive, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { DxForm, DxSimpleItem, DxLabel, DxButtonItem, DxButtonOptions } from 'devextreme-vue/form'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import { notifySuccess, notifyError } from '@/composables/useNotify'
import { changePassword } from '@/api/auth'

const showGuide = ref(false)
const { t } = useI18n()
const changePwdFormRef = ref<InstanceType<typeof DxForm> | null>(null)

const changePwdForm = reactive({
  OldPassword: '',
  NewPassword: '',
  ConfirmPassword: '',
})

const securityInfo = reactive({
  passwordPolicy: {
    minLength: 8,
    requireUppercase: true,
    requireLowercase: true,
    requireDigit: true,
    requireSpecialChar: false,
    expirationDays: 90,
  },
})

const oldPasswordRules = computed(() => [
  { type: 'required' as const, message: t('请输入当前密码') },
])

const newPasswordRules = computed(() => [
  { type: 'required' as const, message: t('请输入新密码') },
  { type: 'stringLength' as const, min: 6, max: 128, message: t('密码长度至少6位') },
])

const confirmPasswordRules = computed(() => [
  { type: 'required' as const, message: t('请确认新密码') },
  {
    type: 'compare' as const,
    comparisonTarget: () => changePwdForm.NewPassword,
    message: t('两次输入的密码不一致'),
  },
])

async function handleChangePassword() {
  const formInstance = changePwdFormRef.value?.instance
  if (formInstance) {
    const result = formInstance.validate()
    if (!result.isValid) return
  }
  try {
    await changePassword({
      OldPassword: changePwdForm.OldPassword,
      NewPassword: changePwdForm.NewPassword,
    })
    notifySuccess('密码修改成功')
    Object.assign(changePwdForm, { OldPassword: '', NewPassword: '', ConfirmPassword: '' })
  } catch (e: unknown) {
    notifyError(e instanceof Error ? e.message : t('密码修改失败'))
  }
}

const guideSteps = computed(() => [
  t('进入安全中心查看安全策略概览'),
  t('在修改密码区域输入当前密码和新密码'),
  t('查看密码策略了解复杂度要求'),
])
const guideFieldNotes = computed(() => [
  t('密码策略控制密码复杂度和过期时间'),
  t('修改密码后需使用新密码重新登录'),
])
const guideErrorNotes = computed(() => [
  t('当前密码输入错误将导致修改失败'),
  t('新密码必须符合密码策略要求'),
])
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
.security-badge {
  font-size: 11px;
  padding: 2px 8px;
  border-radius: 10px;
  background: var(--info-bg, #e3f2fd);
  color: var(--info-text, #1565c0);
  margin-left: auto;
}
.security-badge.developing {
  background: var(--warning-bg, #fff3e0);
  color: var(--warning-text, #e65100);
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
.security-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 24px 0;
  color: var(--dx-color-text-secondary, #999);
}
.security-placeholder .dx-icon {
  font-size: 32px;
  margin-bottom: 8px;
}
.security-placeholder p {
  font-size: 13px;
  text-align: center;
}
</style>
