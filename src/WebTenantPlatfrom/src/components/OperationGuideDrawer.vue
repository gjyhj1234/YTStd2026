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
      :width="popupWidth"
      :height="'auto'"
      :max-height="'90vh'"
      :show-close-button="true"
      :drag-enabled="!isSmallScreen"
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
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { DxButton } from 'devextreme-vue/button'
import { DxPopup } from 'devextreme-vue/popup'
import DxScrollView from 'devextreme-vue/scroll-view'

defineProps<{
  visible?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const windowWidth = ref(typeof window !== 'undefined' ? window.innerWidth : 1280)
const popupWidth = computed(() => Math.min(600, windowWidth.value - 32))
const isSmallScreen = computed(() => windowWidth.value < 768)

function onWindowResize(): void { windowWidth.value = window.innerWidth }

onMounted(() => {
  if (typeof window !== 'undefined') {
    window.addEventListener('resize', onWindowResize)
  }
})

onUnmounted(() => {
  if (typeof window !== 'undefined') {
    window.removeEventListener('resize', onWindowResize)
  }
})

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
