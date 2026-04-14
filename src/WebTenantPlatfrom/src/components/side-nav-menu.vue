<template>
  <div
    :class="[swatchClassName, 'side-navigation-menu']"
    @click="forwardClick"
  >
    <slot />
    <div class="menu-container">
      <dx-tree-view
        ref="treeViewRef"
        :items="filteredNavItems"
        key-expr="path"
        selection-mode="single"
        :select-by-click="true"
        :focus-state-enabled="false"
        expand-event="click"
        @item-click="handleItemClick"
        width="100%"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import DxTreeView from 'devextreme-vue/tree-view'
import { sizes } from '../utils/media-query'
import { getNavigationItems, filterNavigationByPermission } from '../app-navigation'
import type { NavigationItem } from '../app-navigation'
import { onMounted, ref, watch, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { themeService } from '../theme-service'
import { useAuthStore } from '../store/auth'

const props = defineProps<{
  compactMode: boolean
}>()

const emit = defineEmits<{
  (e: 'click', ...args: unknown[]): void
}>()

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const authStore = useAuthStore()

const isLargeScreen = sizes()['screen-large']

const allNavItems = computed(() => {
  return getNavigationItems(t).map((item: NavigationItem) => {
    if (item.path && !(/^\//.test(item.path))) {
      item.path = `/${item.path}`
    }
    return { ...item, expanded: isLargeScreen }
  })
})

const filteredNavItems = computed(() => {
  return filterNavigationByPermission(
    allNavItems.value,
    (code: string) => authStore.hasPermission(code)
  )
})

const treeViewRef = ref<InstanceType<typeof DxTreeView> | null>(null)
const swatchClassName = ref('')

function forwardClick(...args: unknown[]) {
  emit('click', ...args)
}

function handleItemClick(e: { itemData: NavigationItem; event: PointerEvent }) {
  if (!e.itemData.path || props.compactMode) {
    return
  }
  router.push(e.itemData.path)
  e.event.stopPropagation()
}

function updateSelection() {
  if (!treeViewRef.value || !(treeViewRef.value as unknown as { instance: { selectItem: (path: string) => void; expandItem: (path: string) => void } }).instance) {
    return
  }
  const instance = (treeViewRef.value as unknown as { instance: { selectItem: (path: string) => void; expandItem: (path: string) => void } }).instance
  instance.selectItem(route.path)
  instance.expandItem(route.path)
}

onMounted(() => {
  updateSelection()
  if (props.compactMode && treeViewRef.value) {
    const instance = (treeViewRef.value as unknown as { instance: { collapseAll: () => void } }).instance
    instance.collapseAll()
  }
})

watch(
  () => route.path,
  () => {
    updateSelection()
  }
)

watch(
  () => themeService.isDark,
  () => {
    swatchClassName.value = themeService.isDark.value ? 'dx-swatch-additional-dark' : 'dx-swatch-additional'
  },
  { immediate: true }
)

watch(
  () => props.compactMode,
  () => {
    if (props.compactMode && treeViewRef.value) {
      const instance = (treeViewRef.value as unknown as { instance: { collapseAll: () => void } }).instance
      instance.collapseAll()
    } else {
      updateSelection()
    }
  }
)
</script>

<style lang="scss">
@use "../variables.scss" as *;
@use "../dx-styles.scss" as *;

.dx-swatch-additional, .dx-swatch-additional-dark {
  &.side-navigation-menu {
    display: flex;
    flex-direction: column;
    min-height: 100%;
    height: 100%;
    width: 250px !important;
    background-color: var(--base-bg);

    .menu-container {
      min-height: 100%;
      display: flex;
      flex: 1;

      .dx-treeview {
        // ## Long text positioning
        white-space: nowrap;
        // ##

        .dx-treeview-node-container:empty {
          display: none;
        }

        // ## Icon width customization
        .dx-treeview-item {
          padding-left: 0;
          border-radius: 0;
          flex-direction: row-reverse;

          .dx-icon {
            width: $side-panel-min-width !important;
            margin: 0 !important;
          }
        }

        // ##

        // ## Arrow customization
        .dx-treeview-node {
          padding: 0 0 !important;
        }

        .dx-treeview-toggle-item-visibility {
          right: 10px;
          left: auto;
        }

        .dx-rtl .dx-treeview-toggle-item-visibility {
          left: 10px;
          right: auto;
        }
        // ##

        // ## Item levels customization
        .dx-treeview-node {
          &[aria-level="1"] {
            font-weight: bold;
          }

          &[aria-level="2"] .dx-treeview-item-content {
            font-weight: normal;
            padding: 0 $side-panel-min-width;
          }
        }
        // ##

        // ## Anti-shift CSS: prevent layout changes on selection
        .dx-state-focused,
        .dx-state-active,
        .dx-state-selected,
        .dx-state-hover {
          font-weight: normal !important;
        }

        .dx-state-selected .dx-treeview-item-content {
          text-shadow: 0 0 0.5px currentColor;
        }
        // ##
      }
    }
  }
}
</style>
