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
import App from './App.vue'
import appInfo from './app-info'
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
  app.mount('#app')
})
