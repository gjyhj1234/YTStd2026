import {
  PLATFORM_USER_VIEW,
  PLATFORM_ROLE_VIEW
} from './constants/permissions'

/**
 * Permission constants for navigation items.
 *
 * For modules with existing constants in permissions.ts, import directly.
 * For modules not yet implemented, define inline constants matching backend permission codes.
 */
const PLATFORM_PERMISSION_VIEW = 'platform:permission:view'
const TENANT_LIST_VIEW = 'tenant:list:view'
const TENANT_GROUP_VIEW = 'tenant:group:view'
const TENANT_CONFIG_VIEW = 'tenant:config:view'
const PACKAGE_LIST_VIEW = 'package:list:view'
const SUBSCRIPTION_LIST_VIEW = 'subscription:list:view'
const BILLING_INVOICE_VIEW = 'billing:invoice:view'
const LOG_OPERATION_VIEW = 'log:operation:view'
const INFRA_COMPONENT_VIEW = 'infra:component:view'
const LOG_AUDIT_VIEW = 'log:audit:view'
const NOTIFICATION_TEMPLATE_VIEW = 'notification:template:view'

export interface NavigationItem {
  text: string
  path?: string
  icon?: string
  items?: NavigationItem[]
  permission?: string
}

export function getNavigationItems(t: (key: string) => string): NavigationItem[] {
  return [
    { text: t('首页'), icon: 'home', path: '/dashboard' },
    {
      text: t('平台管理'), icon: 'group', items: [
        { text: t('用户管理'), path: '/platform-users', permission: PLATFORM_USER_VIEW },
        { text: t('角色管理'), path: '/platform-roles', permission: PLATFORM_ROLE_VIEW },
        { text: t('权限管理'), path: '/platform-permissions', permission: PLATFORM_PERMISSION_VIEW }
      ]
    },
    {
      text: t('租户管理'), icon: 'box', items: [
        { text: t('租户列表'), path: '/tenants', permission: TENANT_LIST_VIEW },
        { text: t('租户信息'), path: '/tenant-info', permission: TENANT_GROUP_VIEW },
        { text: t('资源配额'), path: '/tenant-resources', permission: TENANT_CONFIG_VIEW },
        { text: t('配置与开关'), path: '/tenant-config', permission: TENANT_CONFIG_VIEW }
      ]
    },
    {
      text: t('SaaS 运营'), icon: 'money', items: [
        { text: t('套餐管理'), path: '/packages', permission: PACKAGE_LIST_VIEW },
        { text: t('订阅管理'), path: '/subscriptions', permission: SUBSCRIPTION_LIST_VIEW },
        { text: t('账单管理'), path: '/billing', permission: BILLING_INVOICE_VIEW },
        { text: t('平台运营'), path: '/platform-operations', permission: LOG_OPERATION_VIEW }
      ]
    },
    { text: t('API 集成'), icon: 'globe', path: '/api-integration', permission: INFRA_COMPONENT_VIEW },
    {
      text: t('系统管理'), icon: 'preferences', items: [
        { text: t('审计日志'), path: '/audit-logs', permission: LOG_AUDIT_VIEW },
        { text: t('通知管理'), path: '/notifications', permission: NOTIFICATION_TEMPLATE_VIEW },
        { text: t('文件管理'), path: '/storage', permission: INFRA_COMPONENT_VIEW }
      ]
    }
  ]
}

/**
 * Filter navigation items by permission.
 * Parent items with no visible children after filtering are removed.
 */
export function filterNavigationByPermission(
  items: NavigationItem[],
  hasPermission: (code: string) => boolean
): NavigationItem[] {
  const result: NavigationItem[] = []

  for (const item of items) {
    if (item.items) {
      const filteredChildren = item.items.filter(
        child => !child.permission || hasPermission(child.permission)
      )
      if (filteredChildren.length > 0) {
        result.push({ ...item, items: filteredChildren })
      }
    } else {
      if (!item.permission || hasPermission(item.permission)) {
        result.push(item)
      }
    }
  }

  return result
}

const defaultNavigation: NavigationItem[] = [
  { text: '首页', icon: 'home', path: '/dashboard' }
]

export default defaultNavigation
