import notify from 'devextreme/ui/notify'
import { confirm } from 'devextreme/ui/dialog'
import { i18n } from '@/locales'

function t(key: string): string {
  const result = i18n.global.t(key)
  return result === key ? key : result
}

/** 成功提示 */
export function notifySuccess(messageKey?: string): void {
  notify({
    message: t(messageKey ?? '操作成功'),
    type: 'success',
    displayTime: 3000,
    position: { at: 'top center', my: 'top center', offset: '0 20' },
  })
}

/** 错误提示 */
export function notifyError(message: string): void {
  notify({
    message,
    type: 'error',
    displayTime: 5000,
    position: { at: 'top center', my: 'top center', offset: '0 20' },
  })
}

/** 警告提示 */
export function notifyWarning(messageKey: string): void {
  notify({
    message: t(messageKey),
    type: 'warning',
    displayTime: 4000,
    position: { at: 'top center', my: 'top center', offset: '0 20' },
  })
}

/** 信息提示 */
export function notifyInfo(messageKey: string): void {
  notify({
    message: t(messageKey),
    type: 'info',
    displayTime: 3000,
    position: { at: 'top center', my: 'top center', offset: '0 20' },
  })
}

/** 确认弹窗（删除操作） */
export function confirmDelete(itemName?: string): Promise<boolean> {
  const message = itemName
    ? `${t('确认删除')} "${itemName}" ?`
    : `${t('确认删除')}?`
  return confirm(message, t('确认'))
}

/** 确认弹窗（危险操作） */
export function confirmAction(messageKey: string): Promise<boolean> {
  return confirm(t(messageKey), t('确认'))
}

/** 封装 composable */
export function useNotify() {
  return {
    success: notifySuccess,
    error: notifyError,
    warning: notifyWarning,
    info: notifyInfo,
    confirmDelete,
    confirmAction,
  }
}
