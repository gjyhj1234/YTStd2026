import 'devextreme/dist/css/dx.common.css'
import './themes/generated/theme.base.dark.css'
import './themes/generated/theme.base.css'
import './themes/generated/theme.additional.dark.css'
import './themes/generated/theme.additional.css'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import { i18n } from './locales'
import themes from 'devextreme/ui/themes'
import config from "devextreme/core/config";
import { licenseKey } from './devextreme-license';
import { locale as dxLocale, loadMessages } from 'devextreme/localization'
import zhMessages from 'devextreme/localization/messages/zh.json'
import zhTwMessages from 'devextreme/localization/messages/zh-tw.json'
import jaMessages from 'devextreme/localization/messages/ja.json'
import enMessages from 'devextreme/localization/messages/en.json'
import App from './App.vue'
import appInfo from './app-info'

// Load DevExtreme locale dictionaries
loadMessages(zhMessages)
loadMessages(zhTwMessages)
loadMessages(jaMessages)
loadMessages(enMessages)

// Map vue-i18n locales to DevExtreme locales
const dxLocaleMap: Record<string, string> = {
  'zh-CN': 'zh',
  'zh-TW': 'zh-TW',
  'ja-JP': 'ja',
  'en-US': 'en',
  'ms-MY': 'en' // fallback to English for Malay
}

// Set initial DevExtreme locale from saved preference
const savedLocale = localStorage.getItem('locale') || 'zh-CN'
dxLocale(dxLocaleMap[savedLocale] || 'zh')

config({
    forceIsoDateParsing: true,
    licenseKey
})
themes.initialized(() => {
  const app = createApp(App)
  const pinia = createPinia()

  app.use(pinia)
  app.use(router)
  app.use(i18n)
  app.config.globalProperties.$appInfo = appInfo

  // Provide dxLocaleMap for use by components that need to sync locale
  app.provide('dxLocaleMap', dxLocaleMap)

  app.mount('#app')
})
