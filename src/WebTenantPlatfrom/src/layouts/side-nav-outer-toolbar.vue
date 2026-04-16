<template>
  <div class="side-nav-outer-toolbar">
    <header-toolbar
      class="layout-header"
      :menu-toggle-enabled="true"
      :toggle-menu-func="toggleMenu"
      :title="$t('租户管理平台')"
    />
    <dx-drawer
      class="layout-body"
      position="before"
      template="menuTemplate"
      v-model:opened="menuOpened"
      :opened-state-mode="drawerOptions.menuMode"
      :reveal-mode="drawerOptions.menuRevealMode"
      :min-size="drawerOptions.minMenuSize"
      :max-size="drawerOptions.maxMenuSize"
      :shading="drawerOptions.shaderEnabled"
      :close-on-outside-click="drawerOptions.closeOnOutsideClick"
    >
      <dx-scroll-view ref="scrollViewRef" class="with-footer">
        <div class="content">
          <slot />
        </div>
        <slot name="footer" />
      </dx-scroll-view>
      <template #menuTemplate>
        <side-nav-menu
          :compact-mode="!menuOpened"
          @click="handleSideBarClick"
        />
      </template>
    </dx-drawer>
  </div>
</template>

<script setup lang="ts">
import DxDrawer from 'devextreme-vue/drawer'
import DxScrollView from 'devextreme-vue/scroll-view'

import HeaderToolbar from '../components/header-toolbar.vue'
import SideNavMenu from '../components/side-nav-menu.vue'
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'

const props = defineProps<{
  title?: string
  isXSmall: boolean
  isLarge: boolean
}>()

const route = useRoute()

const scrollViewRef = ref<InstanceType<typeof DxScrollView> | null>(null)
const menuOpened = ref(props.isLarge)
const menuTemporaryOpened = ref(false)

function toggleMenu(e: { event: PointerEvent }) {
  e.event.stopPropagation()
  if (menuOpened.value) {
    menuTemporaryOpened.value = false
  }
  menuOpened.value = !menuOpened.value
}

function handleSideBarClick() {
  if (menuOpened.value === false) {
    menuTemporaryOpened.value = true
  }
  menuOpened.value = true
}

const drawerOptions = computed(() => {
  const shaderEnabled = !props.isLarge

  return {
    menuMode: props.isLarge ? 'shrink' : 'overlap',
    menuRevealMode: props.isXSmall ? 'slide' : 'expand',
    minMenuSize: props.isXSmall ? 0 : 60,
    maxMenuSize: props.isXSmall ? 250 : undefined,
    closeOnOutsideClick: shaderEnabled,
    shaderEnabled
  }
})

watch(
  () => props.isLarge,
  () => {
    if (!menuTemporaryOpened.value) {
      menuOpened.value = props.isLarge
    }
  }
)

watch(
  () => route.path,
  () => {
    if (menuTemporaryOpened.value || !props.isLarge) {
      menuOpened.value = false
      menuTemporaryOpened.value = false
    }
    if (scrollViewRef.value) {
      const instance = (scrollViewRef.value as unknown as { instance: { scrollTo: (pos: number) => void } }).instance
      instance.scrollTo(0)
    }
  }
)
</script>

<style lang="scss">
.side-nav-outer-toolbar {
  flex-direction: column;
  display: flex;
  height: 100%;
  width: 100%;
}

.layout-header {
  z-index: 1505;
}

/* Mobile: hide footer to save vertical space */
@media (max-width: 768px) {
  .side-nav-outer-toolbar .content-block:has(.footer) {
    display: none;
  }
}
</style>
