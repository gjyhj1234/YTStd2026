import { createI18n } from 'vue-i18n'
import type { I18nOptions } from 'vue-i18n'

import zhCN from './zh-CN.json'
import enUS from './en-US.json'
import jaJP from './ja-JP.json'
import msMY from './ms-MY.json'
import zhTW from './zh-TW.json'

export type SupportedLocale = 'zh-CN' | 'en-US' | 'ja-JP' | 'ms-MY' | 'zh-TW'

const mainMessages: Record<SupportedLocale, Record<string, string>> = {
  'zh-CN': zhCN,
  'en-US': enUS,
  'ja-JP': jaJP,
  'ms-MY': msMY,
  'zh-TW': zhTW
}

/**
 * Auto-load component-level language files via import.meta.glob
 * Pattern: **\/*.vue.{locale}.json
 * Priority (low → high): main locale files → component-level files
 */
function mergeComponentMessages(messages: Record<string, Record<string, string>>): void {
  const localePatterns: Record<SupportedLocale, string> = {
    'zh-CN': '.vue.zh-CN.json',
    'en-US': '.vue.en-US.json',
    'ja-JP': '.vue.ja-JP.json',
    'ms-MY': '.vue.ms-MY.json',
    'zh-TW': '.vue.zh-TW.json'
  }

  const allFiles = import.meta.glob<Record<string, string | null>>([
    '../views/**/*.vue.*.json',
    '../components/**/*.vue.*.json',
    '../layouts/**/*.vue.*.json'
  ], { eager: true })

  for (const [filePath, module] of Object.entries(allFiles)) {
    for (const [locale, suffix] of Object.entries(localePatterns)) {
      if (filePath.endsWith(suffix)) {
        const localeMessages = messages[locale]
        if (localeMessages && module) {
          const exported = (module as Record<string, unknown>).default || module
          for (const [key, value] of Object.entries(exported as Record<string, string | null>)) {
            if (value !== null && value !== undefined) {
              localeMessages[key] = value
            }
          }
        }
        break
      }
    }
  }
}

const messages: Record<string, Record<string, string>> = { ...mainMessages }
mergeComponentMessages(messages)

const i18nOptions: I18nOptions = {
  legacy: false,
  locale: 'zh-CN',
  fallbackLocale: 'en-US',
  messages
}

export const i18n = createI18n(i18nOptions)
