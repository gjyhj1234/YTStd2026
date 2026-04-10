<template>
  <div class="forbidden-page">
    <div class="forbidden-content">
      <i class="dx-icon-lock forbidden-icon"></i>
      <h1>403</h1>
      <p class="forbidden-message">{{ t('权限不足，无法访问此页面') }}</p>
      <DxButton
        :text="t('返回首页')"
        type="default"
        styling-mode="contained"
        @click="goHome"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { DxButton } from 'devextreme-vue/button'

const { t } = useI18n()
const router = useRouter()

function goHome() {
  router.push('/dashboard')
}
</script>

<style scoped>
.forbidden-page {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  min-height: 400px;
}

.forbidden-content {
  text-align: center;
}

.forbidden-icon {
  font-size: 64px;
  color: var(--dx-color-danger, #d9534f);
  margin-bottom: 16px;
}

.forbidden-content h1 {
  font-size: 72px;
  font-weight: 700;
  color: var(--dx-color-danger, #d9534f);
  margin: 0 0 8px;
}

.forbidden-message {
  font-size: 16px;
  color: var(--dx-color-text, #333);
  margin-bottom: 24px;
}
</style>
