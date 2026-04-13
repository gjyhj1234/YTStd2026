<template>
  <dx-drawer
    :opened="visible"
    opened-state-mode="overlap"
    position="right"
    :reveal-mode="'slide'"
    :close-on-outside-click="true"
    @update:opened="onOpenedChanged"
  >
    <template #default>
      <slot name="content" />
    </template>
    <template #panel>
      <div class="operation-guide-panel">
        <div class="guide-header">
          <span class="guide-title">{{ $t('操作指南') }}</span>
          <dx-button
            icon="close"
            styling-mode="text"
            @click="onClose"
          />
        </div>
        <dx-scroll-view class="guide-content">
          <slot />
        </dx-scroll-view>
      </div>
    </template>
  </dx-drawer>
</template>

<script setup lang="ts">
import DxDrawer from 'devextreme-vue/drawer'
import DxScrollView from 'devextreme-vue/scroll-view'
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

function onOpenedChanged(value: boolean) {
  emit('update:visible', value)
}
</script>

<style scoped>
.operation-guide-panel {
  width: 360px;
  height: 100%;
  background-color: var(--base-bg);
  border-left: 1px solid var(--dx-color-border);
  display: flex;
  flex-direction: column;
}

.guide-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  border-bottom: 1px solid var(--dx-color-border);
}

.guide-title {
  font-weight: bold;
  font-size: 16px;
}

.guide-content {
  flex: 1;
  padding: 16px;
}
</style>
