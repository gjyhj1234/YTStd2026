/**
 * Shared DevExtreme locale mapping.
 * Maps vue-i18n locale codes to DevExtreme locale codes.
 * Used by main.ts (init) and header-toolbar.vue (runtime switch).
 */
export const dxLocaleMap: Record<string, string> = {
  'zh-CN': 'zh',
  'zh-TW': 'zh-TW',
  'ja-JP': 'ja',
  'en-US': 'en',
  'ms-MY': 'en' // fallback to English for Malay
}
