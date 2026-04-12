<template>
  <DxDrawer
    :opened="!appStore.sidebarCollapsed"
    :min-size="64"
    :max-size="260"
    opened-state-mode="shrink"
    position="left"
    reveal-mode="expand"
    template="sidebarPanel"
    height="100%"
  >
    <template #sidebarPanel>
      <div class="app-sidebar">
        <div class="sidebar-logo">
          <template v-if="!appStore.sidebarCollapsed">{{ $t('app.title') }}</template>
          <template v-else>{{ $t('app.shortTitle') }}</template>
        </div>
        <DxTreeView
          :items="treeMenuItems"
          key-expr="id"
          display-expr="text"
          items-expr="items"
          :select-by-click="false"
          :focus-state-enabled="false"
          :active-state-enabled="false"
          :hover-state-enabled="false"
          :search-enabled="false"
          no-data-text=""
          @item-click="onMenuItemClick"
        >
          <template #item="{ data }">
            <div class="sidebar-tree-item" :class="{ active: isItemActive(data) }">
              <i v-if="data.icon" :class="`dx-icon dx-icon-${data.icon}`" />
              <span v-if="!appStore.sidebarCollapsed" class="sidebar-tree-text">{{ data.text }}</span>
            </div>
          </template>
        </DxTreeView>
      </div>
    </template>

    <div class="app-main">
      <header class="app-header">
        <div class="header-left">
          <DxButton
            icon="menu"
            styling-mode="text"
            @click="appStore.toggleSidebar"
          />
          <span class="breadcrumb">{{ currentTitle }}</span>
        </div>
        <div class="header-right">
          <DxSelectBox
            :items="languageOptions"
            display-expr="label"
            value-expr="value"
            :value="appStore.locale"
            :width="140"
            :input-attr="{ 'aria-label': $t('app.language') }"
            @value-changed="onLocaleChange"
          />
          <span class="user-info">{{ authStore.displayName }}</span>
          <DxButton
            :text="$t('app.logout')"
            type="normal"
            styling-mode="outlined"
            @click="handleLogout"
          />
        </div>
      </header>
      <main class="app-content">
        <router-view />
      </main>
    </div>
  </DxDrawer>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { DxDrawer } from 'devextreme-vue/drawer'
import { DxTreeView } from 'devextreme-vue/tree-view'
import { DxButton } from 'devextreme-vue/button'
import { DxSelectBox } from 'devextreme-vue/select-box'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/store/app'
import { useAuthStore } from '@/store/auth'
import { menuItems, type MenuItem } from '@/constants/menus'
import { localeOptions, type LocaleCode } from '@/locales'

const appStore = useAppStore()
const authStore = useAuthStore()
const route = useRoute()
const router = useRouter()
const { t, locale } = useI18n()

/** DxTreeView 节点数据类型 */
interface TreeItem {
  id: string
  text: string
  icon?: string
  path?: string
  expanded?: boolean
  items?: TreeItem[]
}

/** 将菜单数据转换为 DxTreeView 格式，折叠时不显示子菜单 */
const treeMenuItems = computed<TreeItem[]>(() => {
  // 访问 locale.value 以触发语言切换时的响应式更新
  locale.value
  return visibleMenuItems.value.map(item => {
    const node: TreeItem = {
      id: item.key,
      text: t(item.label),
      icon: item.icon,
      path: item.path,
    }
    if (item.children && !appStore.sidebarCollapsed) {
      node.expanded = true
      node.items = getVisibleChildren(item).map(child => ({
        id: child.key,
        text: t(child.label),
        path: child.path,
      }))
    }
    return node
  })
})

const languageOptions = computed(() => {
  // 访问 locale.value 以触发语言切换时的响应式更新
  locale.value
  return localeOptions.map(item => ({
    value: item.value,
    label: t(item.labelKey),
  }))
})

/** 根据权限过滤可见菜单 */
const visibleMenuItems = computed(() => {
  return menuItems.filter(item => {
    if (item.children) {
      return getVisibleChildren(item).length > 0
    }
    return isPermissionVisible(item)
  })
})

function getVisibleChildren(item: MenuItem): MenuItem[] {
  return (item.children ?? []).filter(child => isPermissionVisible(child))
}

function isPermissionVisible(item: MenuItem): boolean {
  if (!item.permissions || item.permissions.length === 0) return true
  return authStore.hasAnyPermission(item.permissions)
}

function isItemActive(data: TreeItem): boolean {
  return !!data.path && route.path === data.path
}

const currentTitle = computed(() => {
  return t((route.meta.title as string) || 'route.dashboard')
})

function onMenuItemClick(e: { itemData?: TreeItem }) {
  if (e.itemData?.path) {
    router.push(e.itemData.path)
  }
}

function onLocaleChange(event: { value?: unknown }) {
  if (event.value) {
    appStore.updateLocale(event.value as LocaleCode)
  }
}

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>
