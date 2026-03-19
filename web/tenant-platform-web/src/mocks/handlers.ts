import { authHandlers } from './handlers/auth'
import { platformUsersHandlers } from './handlers/platformUsers'
import { platformRolesHandlers } from './handlers/platformRoles'
import { platformPermissionsHandlers } from './handlers/platformPermissions'
import { tenantsHandlers } from './handlers/tenants'
import { tenantGroupsHandlers } from './handlers/tenantGroups'
import { tenantDomainsHandlers } from './handlers/tenantDomains'
import { tenantTagsHandlers } from './handlers/tenantTags'
import { tenantResourcesHandlers } from './handlers/tenantResources'
import { tenantConfigHandlers } from './handlers/tenantConfig'
import { packagesHandlers } from './handlers/packages'
import { subscriptionsHandlers } from './handlers/subscriptions'
import { billingHandlers } from './handlers/billing'
import { apiIntegrationHandlers } from './handlers/apiIntegration'
import { notificationsHandlers } from './handlers/notifications'
import { storageHandlers } from './handlers/storage'
import { logsHandlers } from './handlers/logs'
import { operationsHandlers } from './handlers/operations'

export const handlers = [
  ...authHandlers,
  ...platformUsersHandlers,
  ...platformRolesHandlers,
  ...platformPermissionsHandlers,
  ...tenantsHandlers,
  ...tenantGroupsHandlers,
  ...tenantDomainsHandlers,
  ...tenantTagsHandlers,
  ...tenantResourcesHandlers,
  ...tenantConfigHandlers,
  ...packagesHandlers,
  ...subscriptionsHandlers,
  ...billingHandlers,
  ...apiIntegrationHandlers,
  ...notificationsHandlers,
  ...storageHandlers,
  ...logsHandlers,
  ...operationsHandlers,
]
