<template>
  <div v-if="visible" class="function-description-card">
    <div class="card-header">
      <span class="card-title">{{ $t('功能说明') }}</span>
      <dx-button
        icon="close"
        styling-mode="text"
        @click="onClose"
      />
    </div>
    <div class="card-content">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import DxButton from 'devextreme-vue/button'

defineProps<{
  visible?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

function onClose() {
  emit('update:visible', false)
}
</script>

<style scoped>
.function-description-card {
  background-color: var(--base-bg);
  border: 1px solid var(--dx-color-border);
  border-radius: 4px;
  padding: 16px;
  margin-bottom: 16px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.card-title {
  font-weight: bold;
  font-size: 14px;
}

.card-content {
  font-size: 13px;
  line-height: 1.6;
  color: var(--base-text-color-alpha-7);
}
</style>
