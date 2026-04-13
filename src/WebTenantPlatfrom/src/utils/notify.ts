import notify from 'devextreme/ui/notify'
import { confirm } from 'devextreme/ui/dialog'
import { i18n } from '../locales'

/** Show success notification (pass i18n key, NOT t()-wrapped) */
export function notifySuccess(messageKey: string): void {
  const { t } = i18n.global
  notify({ message: t(messageKey), type: 'success', displayTime: 2000 })
}

/** Show error notification (pass i18n key) */
export function notifyError(messageKey: string): void {
  const { t } = i18n.global
  notify({ message: t(messageKey), type: 'error', displayTime: 3000 })
}

/** Show confirm action dialog (pass i18n key, NOT t()-wrapped) */
export async function confirmAction(messageKey: string, params?: Record<string, string>): Promise<boolean> {
  const { t } = i18n.global
  let message = t(messageKey)
  if (params) {
    for (const [key, value] of Object.entries(params)) {
      message = message.replace(`{${key}}`, value)
    }
  }
  const result = await confirm(message, t('确定'))
  return result
}

/** Show delete confirmation dialog */
export async function confirmDelete(name: string): Promise<boolean> {
  const { t } = i18n.global
  const message = t('确认删除 {name}', { name })
  const result = await confirm(message, t('确定'))
  return result
}
