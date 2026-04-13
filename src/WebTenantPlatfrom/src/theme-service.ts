import { ref } from 'vue'
import type { Ref } from 'vue'

class ThemeService {
  themes: string[] = ['light', 'dark']
  themeClassNamePrefix: string = 'dx-swatch-'
  currentTheme: Ref<string> = ref('')
  isDark: Ref<boolean> = ref(false)

  constructor() {
    const appEl = document.getElementById('app')
    if (appEl && !appEl.className.includes(this.themeClassNamePrefix)) {
      this.currentTheme.value = this.themes[0]
      appEl.classList.add(this.themeClassNamePrefix + this.currentTheme.value)
    }
  }

  switchAppTheme(): void {
    const prevTheme = this.currentTheme.value
    const isCurrentThemeDark = prevTheme === 'dark'

    this.currentTheme.value = this.themes[prevTheme === this.themes[0] ? 1 : 0]

    const appEl = document.getElementById('app')
    if (!appEl) return

    appEl.classList.replace(
      this.themeClassNamePrefix + prevTheme,
      this.themeClassNamePrefix + this.currentTheme.value
    )

    const additionalClassNamePrefix = this.themeClassNamePrefix + 'additional'
    const additionalClassNamePostfix = isCurrentThemeDark ? '-' + prevTheme : ''
    const additionalClassName = `${additionalClassNamePrefix}${additionalClassNamePostfix}`

    appEl.querySelector(`.${additionalClassName}`)?.classList
      .replace(additionalClassName, additionalClassNamePrefix + (isCurrentThemeDark ? '' : '-dark'))

    this.isDark.value = this.currentTheme.value === 'dark'
  }
}

export const themeService = new ThemeService()
