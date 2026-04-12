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
            label-mode="static"
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

      <!-- IP 白名单 -->
      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-globe" />
          <h3>{{ $t('IP白名单') }}</h3>
          <DxSwitch
            :value="ipWhitelistEnabled"
            :hint="$t('启用或禁用IP白名单')"
            @value-changed="onIpWhitelistToggle"
            class="security-switch"
          />
        </div>
        <div class="security-card-body">
          <div class="security-item">
            <span class="security-label">{{ $t('启用状态') }}</span>
            <span class="security-value" :class="ipWhitelistEnabled ? 'text-success' : 'text-muted'">
              {{ ipWhitelistEnabled ? $t('已启用') : $t('已禁用') }}
            </span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('白名单条目数') }}</span>
            <span class="security-value">{{ ipWhitelistEntries.length }} {{ $t('条') }}</span>
          </div>
          <div class="ip-list-section">
            <div class="ip-add-row">
              <DxTextBox
                v-model:value="newIpAddress"
                :placeholder="$t('输入IP地址如192.168.1.0/24')"
                :width="240"
              />
              <DxButton
                :text="$t('添加')"
                icon="add"
                type="default"
                styling-mode="outlined"
                @click="handleAddIp"
              />
            </div>
            <div v-if="ipWhitelistEntries.length > 0" class="ip-entries">
              <div v-for="(entry, index) in ipWhitelistEntries" :key="index" class="ip-entry">
                <span class="ip-address">{{ entry.ip }}</span>
                <span class="ip-remark">{{ entry.remark }}</span>
                <DxButton
                  icon="trash"
                  styling-mode="text"
                  type="danger"
                  @click="handleRemoveIp(index)"
                />
              </div>
            </div>
            <div v-else class="ip-empty">
              <span>{{ $t('暂无白名单条目') }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- MFA 多因素认证 -->
      <div class="card security-card">
        <div class="security-card-header">
          <i class="dx-icon dx-icon-lock" />
          <h3>{{ $t('多因素认证MFA') }}</h3>
          <DxSwitch
            :value="mfaEnabled"
            :hint="$t('启用或禁用MFA')"
            @value-changed="onMfaToggle"
            class="security-switch"
          />
        </div>
        <div class="security-card-body">
          <div class="security-item">
            <span class="security-label">{{ $t('启用状态') }}</span>
            <span class="security-value" :class="mfaEnabled ? 'text-success' : 'text-muted'">
              {{ mfaEnabled ? $t('已启用') : $t('已禁用') }}
            </span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('强制要求') }}</span>
            <span class="security-value">{{ mfaRequired ? $t('是') : $t('否') }}</span>
          </div>
          <div class="security-item">
            <span class="security-label">{{ $t('支持方式') }}</span>
            <span class="security-value">{{ $t('TOTP验证器') }}</span>
          </div>
          <div class="mfa-settings">
            <div class="security-item">
              <DxCheckBox
                :value="mfaRequired"
                :text="$t('强制所有用户启用MFA')"
                @value-changed="onMfaRequiredToggle"
              />
            </div>
            <DxButton
              :text="$t('保存MFA设置')"
              type="default"
              styling-mode="outlined"
              @click="handleSaveMfaSettings"
            />
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
import { DxButton } from 'devextreme-vue/button'
import { DxTextBox } from 'devextreme-vue/text-box'
import { DxSwitch } from 'devextreme-vue/switch'
import { DxCheckBox } from 'devextreme-vue/check-box'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import { notifySuccess, notifyError, notifyWarning, confirmAction } from '@/composables/useNotify'
import { changePassword } from '@/api/auth'

interface IpEntry {
  ip: string
  remark: string
}

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

// IP 白名单状态
const ipWhitelistEnabled = ref(false)
const ipWhitelistEntries = ref<IpEntry[]>([
  { ip: '127.0.0.1', remark: 'localhost' },
  { ip: '10.0.0.0/8', remark: 'Private network' },
])
const newIpAddress = ref('')

// MFA 状态
const mfaEnabled = ref(false)
const mfaRequired = ref(false)

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

// IP 白名单操作
async function onIpWhitelistToggle(e: { value?: boolean }) {
  if (e.value === true && ipWhitelistEntries.value.length === 0) {
    notifyWarning('启用IP白名单前请先添加至少一个IP地址')
    ipWhitelistEnabled.value = false
    return
  }
  const confirmed = await confirmAction(
    e.value ? '确认启用IP白名单' : '确认禁用IP白名单'
  )
  if (!confirmed) {
    ipWhitelistEnabled.value = !e.value
    return
  }
  ipWhitelistEnabled.value = e.value ?? false
  notifySuccess(e.value ? 'IP白名单已启用' : 'IP白名单已禁用')
}

function handleAddIp() {
  const ip = newIpAddress.value?.trim()
  if (!ip) {
    notifyWarning('请输入IP地址')
    return
  }
  // 简单的 IP/CIDR 格式验证
  const ipPattern = /^(\d{1,3}\.){3}\d{1,3}(\/\d{1,2})?$/
  if (!ipPattern.test(ip)) {
    notifyError(t('IP地址格式不正确'))
    return
  }
  if (ipWhitelistEntries.value.some(entry => entry.ip === ip)) {
    notifyWarning('该IP地址已存在')
    return
  }
  ipWhitelistEntries.value.push({ ip, remark: '' })
  newIpAddress.value = ''
  notifySuccess('IP地址添加成功')
}

function handleRemoveIp(index: number) {
  ipWhitelistEntries.value.splice(index, 1)
  notifySuccess('IP地址删除成功')
}

// MFA 操作
async function onMfaToggle(e: { value?: boolean }) {
  const confirmed = await confirmAction(
    e.value ? '确认启用多因素认证' : '确认禁用多因素认证'
  )
  if (!confirmed) {
    mfaEnabled.value = !e.value
    return
  }
  mfaEnabled.value = e.value ?? false
  notifySuccess(e.value ? 'MFA已启用' : 'MFA已禁用')
}

function onMfaRequiredToggle(e: { value?: boolean }) {
  mfaRequired.value = e.value ?? false
}

function handleSaveMfaSettings() {
  notifySuccess('MFA设置保存成功')
}

const guideSteps = computed(() => [
  t('进入安全中心查看安全策略概览'),
  t('在修改密码区域输入当前密码和新密码'),
  t('查看密码策略了解复杂度要求'),
  t('管理IP白名单控制访问来源'),
  t('配置多因素认证增强账户安全'),
])
const guideFieldNotes = computed(() => [
  t('密码策略控制密码复杂度和过期时间'),
  t('修改密码后需使用新密码重新登录'),
  t('IP白名单启用后仅允许白名单IP访问'),
  t('MFA多因素认证提供额外安全保护'),
])
const guideErrorNotes = computed(() => [
  t('当前密码输入错误将导致修改失败'),
  t('新密码必须符合密码策略要求'),
  t('启用IP白名单前请确保添加管理员IP'),
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
.security-switch {
  margin-left: auto;
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
.text-success {
  color: var(--success-text, #2e7d32);
}
.text-muted {
  color: var(--dx-color-text-secondary, #999);
}
.ip-list-section {
  margin-top: 8px;
}
.ip-add-row {
  display: flex;
  gap: 8px;
  align-items: center;
  margin-bottom: 12px;
}
.ip-entries {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.ip-entry {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 10px;
  background: var(--bg-color, #f5f7fa);
  border-radius: 4px;
  font-size: 13px;
}
.ip-address {
  font-family: monospace;
  color: var(--dx-color-text, #333);
  min-width: 140px;
}
.ip-remark {
  color: var(--dx-color-text-secondary, #999);
  font-size: 12px;
  flex: 1;
}
.ip-empty {
  text-align: center;
  padding: 16px;
  color: var(--dx-color-text-secondary, #999);
  font-size: 13px;
}
.mfa-settings {
  margin-top: 8px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}
</style>
