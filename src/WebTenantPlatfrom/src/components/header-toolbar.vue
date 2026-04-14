<template>
  <header class="header-component">
    <dx-toolbar class="header-toolbar">
      <dx-item
        :visible="menuToggleEnabled"
        location="before"
        css-class="menu-button"
      >
        <template #default>
          <dx-button
            icon="menu"
            styling-mode="text"
            @click="toggleMenuFunc"
          />
        </template>
      </dx-item>

      <dx-item
        location="before"
        css-class="header-title dx-toolbar-label"
      >
        <div>{{ $t('租户管理平台') }}</div>
      </dx-item>

      <dx-item location="after">
        <template #default>
          <dx-select-box
            :items="languageOptions"
            display-expr="label"
            value-expr="value"
            :value="currentLocale"
            :width="140"
            styling-mode="underlined"
            @value-changed="onLanguageChanged"
          />
        </template>
      </dx-item>

      <dx-item location="after">
        <div><theme-switcher /></div>
      </dx-item>

      <dx-item
        location="after"
        locate-in-menu="auto"
        menu-item-template="menuUserItem"
      >
        <template #default>
          <dx-drop-down-button
            :text="displayName"
            icon="user"
            styling-mode="text"
            :items="userMenuItems"
            display-expr="text"
            key-expr="id"
            :show-arrow-icon="false"
            :drop-down-options="{ width: 180 }"
            @item-click="onUserMenuItemClick"
          />
        </template>
      </dx-item>

      <template #menuUserItem>
        <dx-list
          :items="userMenuItems"
          display-expr="text"
          key-expr="id"
          :width="150"
        />
      </template>

    </dx-toolbar>
  </header>
</template>

<script setup lang="ts">
import DxButton from 'devextreme-vue/button'
import DxToolbar, { DxItem } from 'devextreme-vue/toolbar'
import { DxSelectBox } from 'devextreme-vue/select-box'
import { DxDropDownButton } from 'devextreme-vue/drop-down-button'
import { DxList } from 'devextreme-vue/list'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { computed } from 'vue'
import auth from '../auth'
import { useAuthStore } from '../store/auth'
import ThemeSwitcher from './theme-switcher.vue'

defineProps<{
  menuToggleEnabled: boolean
  title?: string
  toggleMenuFunc?: (e: { event: PointerEvent }) => void
}>()

import { locale as dxLocale } from 'devextreme/localization'

const router = useRouter()
const route = useRoute()
const { t, locale } = useI18n()
const authStore = useAuthStore()

// DevExtreme locale mapping
const dxLocaleMap: Record<string, string> = {
  'zh-CN': 'zh',
  'zh-TW': 'zh-TW',
  'ja-JP': 'ja',
  'en-US': 'en',
  'ms-MY': 'en'
}

const currentLocale = computed(() => locale.value)

const displayName = computed(() => {
  return authStore.user?.DisplayName || authStore.user?.Username || t('用户')
})

const languageOptions = [
  { value: 'zh-CN', label: '简体中文' },
  { value: 'en-US', label: 'English' },
  { value: 'ja-JP', label: '日本語' },
  { value: 'ms-MY', label: 'Bahasa Melayu' },
  { value: 'zh-TW', label: '繁體中文' }
]

const userMenuItems = computed(() => [
  { id: 'change-password', text: t('修改密码'), icon: 'key' },
  { id: 'logout', text: t('退出登录'), icon: 'runner' }
])

function onLanguageChanged(e: { value: string }) {
  locale.value = e.value
  localStorage.setItem('locale', e.value)
  // Sync DevExtreme locale for DxDateRangeBox, dialog buttons, etc.
  dxLocale(dxLocaleMap[e.value] || 'en')
}

function onUserMenuItemClick(e: { itemData: { id: string } }) {
  const itemId = e.itemData.id
  if (itemId === 'logout') {
    auth.logOut()
    authStore.clearAuth()
    router.push({
      path: '/login-form',
      query: { redirect: route.path }
    })
  } else if (itemId === 'change-password') {
    // TODO: open change password dialog
  }
}
</script>

<style lang="scss">
@use "../dx-styles.scss" as *;

header {
  background-color: var(--base-bg);
}

.header-component {
  flex: 0 0 auto;
  z-index: 1;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24);
}

.dx-toolbar.header-toolbar .dx-toolbar-items-container .dx-toolbar-after {
  padding: 0 40px;

  .screen-x-small & {
    padding: 0 20px;
  }
}

.dx-toolbar .dx-toolbar-item.dx-toolbar-button.menu-button {
  width: $side-panel-min-width;
  text-align: center;
  padding: 0;
}

.header-title .dx-item-content {
  padding: 0;
  margin: 0;
}

.dx-theme-generic {
  .header-toolbar {
    padding: 10px 0;
  }
}
</style>
