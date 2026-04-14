<template>
  <div>
    <DxButton
      icon="help"
      styling-mode="text"
      :hint="$t('操作指南')"
      @click="onToggle"
    />
    <DxPopup
      :visible="visible"
      :title="$t('操作指南')"
      :width="600"
      :height="'auto'"
      :show-close-button="true"
      :drag-enabled="true"
      @hiding="onClose"
    >
      <template #content>
        <DxScrollView>
          <div class="operation-guide-content">
            <slot />
          </div>
        </DxScrollView>
      </template>
    </DxPopup>
  </div>
</template>

<script setup lang="ts">
import { DxButton } from 'devextreme-vue/button'
import { DxPopup } from 'devextreme-vue/popup'
import DxScrollView from 'devextreme-vue/scroll-view'

defineProps<{
  visible?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

function onToggle() {
  emit('update:visible', true)
}

function onClose() {
  emit('update:visible', false)
}
</script>

<style scoped>
.operation-guide-content {
  padding: 8px 0;
  font-size: 13px;
  line-height: 1.8;
  color: var(--base-text-color-alpha-7);
}
</style>
