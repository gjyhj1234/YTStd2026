import { createI18n } from 'vue-i18n'
import { loadMessages, locale as setDevExtremeLocale } from 'devextreme/localization'
import devExtremeEn from 'devextreme/localization/messages/en.json'
import devExtremeZhCn from 'devextreme/localization/messages/zh.json'
import devExtremeZhTw from 'devextreme/localization/messages/zh-tw.json'
import zhCN from './zh-CN.json'
import enUS from './en-US.json'
import msMY from './ms-MY.json'
import zhTW from './zh-TW.json'
import jaJP from './ja-JP.json'
import generatedZhCN from './generated/zh-CN.json'
import generatedEnUS from './generated/en-US.json'
import generatedMsMY from './generated/ms-MY.json'
import generatedZhTW from './generated/zh-TW.json'
import generatedJaJP from './generated/ja-JP.json'
import commonZhCN from './common/zh-CN.json'
import commonEnUS from './common/en-US.json'
import commonMsMY from './common/ms-MY.json'
import commonZhTW from './common/zh-TW.json'
import commonJaJP from './common/ja-JP.json'
import enumsZhCN from './enums/zh-CN.json'
import enumsEnUS from './enums/en-US.json'
import enumsMsMY from './enums/ms-MY.json'
import enumsZhTW from './enums/zh-TW.json'
import enumsJaJP from './enums/ja-JP.json'

export type LocaleCode = 'zh-CN' | 'en-US' | 'ms-MY' | 'zh-TW' | 'ja-JP'

const LOCALE_STORAGE_KEY = 'platform_locale'
const DEFAULT_LOCALE: LocaleCode = 'zh-CN'

/**
 * 使用 import.meta.glob 自动加载所有组件级语言文件（*.vue.{locale}.json）
 * eager: true 表示编译时静态导入，确保所有文件在构建时被包含
 */
const componentZhCNModules = import.meta.glob<Record<string, string | null>>(
  ['../views/**/*.vue.zh-CN.json', '../components/**/*.vue.zh-CN.json', '../layouts/**/*.vue.zh-CN.json'],
  { eager: true, import: 'default' },
)
const componentEnUSModules = import.meta.glob<Record<string, string | null>>(
  ['../views/**/*.vue.en-US.json', '../components/**/*.vue.en-US.json', '../layouts/**/*.vue.en-US.json'],
  { eager: true, import: 'default' },
)
const componentMsMYModules = import.meta.glob<Record<string, string | null>>(
  ['../views/**/*.vue.ms-MY.json', '../components/**/*.vue.ms-MY.json', '../layouts/**/*.vue.ms-MY.json'],
  { eager: true, import: 'default' },
)
const componentZhTWModules = import.meta.glob<Record<string, string | null>>(
  ['../views/**/*.vue.zh-TW.json', '../components/**/*.vue.zh-TW.json', '../layouts/**/*.vue.zh-TW.json'],
  { eager: true, import: 'default' },
)
const componentJaJPModules = import.meta.glob<Record<string, string | null>>(
  ['../views/**/*.vue.ja-JP.json', '../components/**/*.vue.ja-JP.json', '../layouts/**/*.vue.ja-JP.json'],
  { eager: true, import: 'default' },
)

/**
 * 合并组件级语言文件：遍历所有模块，提取非 null 的键值对
 * null 值表示该 key 由 common 层提供翻译，不覆盖上层
 */
function mergeComponentMessages(modules: Record<string, Record<string, string | null>>): Record<string, string> {
  const result: Record<string, string> = {}
  for (const data of Object.values(modules)) {
    if (!data) continue
    for (const key of Object.keys(data)) {
      const value = data[key]
      if (value !== null && value !== undefined) {
        result[key] = value
      }
    }
  }
  return result
}

const componentZhCN = mergeComponentMessages(componentZhCNModules)
const componentEnUS = mergeComponentMessages(componentEnUSModules)
const componentMsMY = mergeComponentMessages(componentMsMYModules)
const componentZhTW = mergeComponentMessages(componentZhTWModules)
const componentJaJP = mergeComponentMessages(componentJaJPModules)

/**
 * 消息合并优先级（从低到高）：
 * 1. common — 全局复用翻译（最低）
 * 2. generated — Generator 生成的 ErrorCode/MessageCode 翻译
 * 3. enums — 枚举值翻译
 * 4. 主语言文件 — 应用级结构化翻译（app/menu/route 等嵌套键）
 * 5. 组件级语言文件 — 组件自有翻译（最高，修改后直接生效）
 */
const messages = {
  'zh-CN': { ...commonZhCN, ...generatedZhCN, ...enumsZhCN, ...zhCN, ...componentZhCN },
  'en-US': { ...commonEnUS, ...generatedEnUS, ...enumsEnUS, ...enUS, ...componentEnUS },
  'ms-MY': { ...commonMsMY, ...generatedMsMY, ...enumsMsMY, ...msMY, ...componentMsMY },
  'zh-TW': { ...commonZhTW, ...generatedZhTW, ...enumsZhTW, ...zhTW, ...componentZhTW },
  'ja-JP': { ...commonJaJP, ...generatedJaJP, ...enumsJaJP, ...jaJP, ...componentJaJP },
}

loadMessages(devExtremeEn)
loadMessages(devExtremeZhCn)
loadMessages(devExtremeZhTw)

export const i18n = createI18n({
  legacy: false,
  globalInjection: true,
  locale: resolveInitialLocale(),
  fallbackLocale: 'en-US',
  messages,
})

applyLocale(i18n.global.locale.value as LocaleCode)

export const localeOptions: Array<{ value: LocaleCode; labelKey: string }> = [
  { value: 'zh-CN', labelKey: 'languages.zh-CN' },
  { value: 'en-US', labelKey: 'languages.en-US' },
  { value: 'ms-MY', labelKey: 'languages.ms-MY' },
  { value: 'zh-TW', labelKey: 'languages.zh-TW' },
  { value: 'ja-JP', labelKey: 'languages.ja-JP' },
]

function resolveInitialLocale(): LocaleCode {
  return normalizeLocale(localStorage.getItem(LOCALE_STORAGE_KEY))
}

export function normalizeLocale(value?: string | null): LocaleCode {
  switch ((value ?? '').trim()) {
    case 'en':
    case 'en-US':
      return 'en-US'
    case 'ms':
    case 'ms-MY':
      return 'ms-MY'
    case 'zh-TW':
    case 'zh_HK':
    case 'zh-HK':
      return 'zh-TW'
    case 'ja':
    case 'ja-JP':
      return 'ja-JP'
    case 'zh':
    case 'zh-CN':
      return 'zh-CN'
    default:
      return DEFAULT_LOCALE
  }
}

function mapDevExtremeLocale(locale: LocaleCode): string {
  switch (locale) {
    case 'zh-CN':
      return 'zh'
    case 'zh-TW':
      return 'zh-tw'
    case 'ms-MY':
      return 'en'
    case 'ja-JP':
      return 'ja'
    case 'en-US':
    default:
      return 'en'
  }
}

function applyLocale(locale: LocaleCode) {
  i18n.global.locale.value = locale
  localStorage.setItem(LOCALE_STORAGE_KEY, locale)
  document.documentElement.lang = locale
  setDevExtremeLocale(mapDevExtremeLocale(locale))
}

export function setLocale(locale: string | null | undefined) {
  applyLocale(normalizeLocale(locale))
}

export function getCurrentLocale(): LocaleCode {
  return normalizeLocale(i18n.global.locale.value as string)
}

export function translateText(text?: string | null): string {
  if (!text) {
    return ''
  }

  const key = text.startsWith('i18n:') ? text.slice(5) : text
  const translated = i18n.global.t(key)
  return translated === key ? text : translated
}

export function translateList(items?: string[] | null): string[] {
  return (items ?? []).map(item => translateText(item))
}

export { LOCALE_STORAGE_KEY }
