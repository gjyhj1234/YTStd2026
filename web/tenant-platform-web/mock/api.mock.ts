import { defineMock } from 'vite-plugin-mock-dev-server'

const ok = <T>(data: T, message = '操作成功') => ({
  success: true, message, data, traceId: '',
})
const fail = (message = '操作失败') => ({
  success: false, message, data: null, traceId: '',
})

export default defineMock([
  /* ── Auth ── */
  {
    url: '/api/auth/login',
    method: 'POST',
    body: ok(
      {
        token: 'mock-token-eyJhbGciOiJIUzI1NiJ9',
        expiresIn: 7200,
        userId: 1,
        username: 'admin',
        displayName: '超级管理员',
        requirePasswordReset: false,
        roles: ['super_admin'],
        permissions: [
          'platform:user:list', 'platform:user:create', 'platform:user:update',
          'platform:user:enable', 'platform:user:disable',
          'platform:role:list', 'platform:role:create', 'platform:role:update',
          'platform:role:enable', 'platform:role:disable',
          'platform:role:bind-permissions', 'platform:role:bind-members',
          'platform:permission:list', 'platform:permission:get',
          'platform:tenant:list', 'platform:tenant:create',
          'platform:tenant:update', 'platform:tenant:status',
        ],
        isSuperAdmin: true,
      },
      '登录成功',
    ),
  },
  {
    url: '/api/auth/logout',
    method: 'POST',
    body: ok(null, '登出成功'),
  },
  {
    url: '/api/auth/me',
    method: 'GET',
    body: ok({ userId: 1, username: 'admin', displayName: '超级管理员', isSuperAdmin: true }),
  },

  /* ── Platform Users ── */
  {
    url: '/api/platform-users',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, username: 'admin', displayName: '超级管理员', email: 'admin@ytstd.com', phone: '13800000001', status: 'Active', isSuperAdmin: true, lastLoginAt: '2025-06-01T10:30:00Z', createdAt: '2025-01-01T00:00:00Z' },
        { id: 2, username: 'operator01', displayName: '运营主管', email: 'operator01@ytstd.com', phone: '13800000002', status: 'Active', isSuperAdmin: false, lastLoginAt: '2025-06-10T09:15:00Z', createdAt: '2025-02-15T08:00:00Z' },
      ],
      total: 2, page: 1, pageSize: 20, totalPages: 1,
    }),
  },
  {
    url: '/api/platform-users',
    method: 'POST',
    body: ok(null, '创建成功'),
  },

  /* ── Platform Roles ── */
  {
    url: '/api/platform-roles',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, code: 'super_admin', name: '超级管理员', description: '拥有平台所有权限', status: 'Active', createdAt: '2025-01-01T00:00:00Z' },
        { id: 2, code: 'operator', name: '运营管理员', description: '负责租户运营管理', status: 'Active', createdAt: '2025-01-15T08:00:00Z' },
      ],
      total: 2, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Platform Permissions ── */
  {
    url: '/api/platform-permissions/tree',
    method: 'GET',
    body: ok([
      {
        id: 1, code: 'platform:user', name: '用户管理', permissionType: 'Menu',
        parentId: null, path: '/platform/users', method: '',
        children: [
          { id: 2, code: 'platform:user:list', name: '用户列表', permissionType: 'Action', parentId: 1, path: '/api/platform-users', method: 'GET', children: [] },
        ],
      },
    ]),
  },

  /* ── Tenants ── */
  {
    url: '/api/tenants',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantCode: 'T20250101001', tenantName: '华夏科技', enterpriseName: '华夏科技有限公司', contactName: '李明', contactEmail: 'liming@huaxia-tech.com', lifecycleStatus: 'Active', isolationMode: 'Shared', enabled: true, openedAt: '2025-01-15T00:00:00Z', expiresAt: '2026-01-15T00:00:00Z', createdAt: '2025-01-15T10:00:00Z' },
        { id: 2, tenantCode: 'T20250201002', tenantName: '云海数据', enterpriseName: '云海数据服务有限公司', contactName: '王芳', contactEmail: 'wangfang@yunhai-data.cn', lifecycleStatus: 'Active', isolationMode: 'Shared', enabled: true, openedAt: '2025-02-01T00:00:00Z', expiresAt: '2026-02-01T00:00:00Z', createdAt: '2025-02-01T08:30:00Z' },
      ],
      total: 2, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Tenant Groups ── */
  {
    url: '/api/tenant-groups/tree',
    method: 'GET',
    body: ok([
      { id: 1, groupCode: 'GRP_ENTERPRISE', groupName: '企业客户', parentId: null, sortOrder: 1, description: '企业级客户分组', createdAt: '2025-01-01T00:00:00Z', children: [] },
    ]),
  },

  /* ── Tenant Domains ── */
  {
    url: '/api/tenant-domains',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, domain: 'huaxia.ytstd.com', domainType: 'SubDomain', isPrimary: true, verificationStatus: 'Verified', createdAt: '2025-01-15T10:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Tenant Tags ── */
  {
    url: '/api/tenant-tags',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tagCode: 'INDUSTRY_TECH', tagName: '科技行业', tagCategory: 'Industry', createdAt: '2025-01-01T00:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Tenant Resource Quotas ── */
  {
    url: '/api/tenant-resource-quotas',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, quotaType: 'MaxUsers', quotaLimit: 100, warningThreshold: 80, resetCycle: 'None', createdAt: '2025-01-15T10:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── SaaS Packages ── */
  {
    url: '/api/saas-packages',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, packageCode: 'PKG_BASIC', packageName: '基础版', description: '适合小型团队', status: 'Active', createdAt: '2025-01-01T00:00:00Z' },
        { id: 2, packageCode: 'PKG_PROFESSIONAL', packageName: '专业版', description: '适合中型企业', status: 'Active', createdAt: '2025-01-01T00:00:00Z' },
      ],
      total: 2, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Subscriptions ── */
  {
    url: '/api/tenant-subscriptions',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, packageVersionId: 2, subscriptionStatus: 'Active', subscriptionType: 'Paid', startedAt: '2025-01-15T00:00:00Z', expiresAt: '2026-01-15T00:00:00Z', autoRenew: true, cancelledAt: '', createdAt: '2025-01-15T10:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Billing Invoices ── */
  {
    url: '/api/billing-invoices',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, invoiceNo: 'INV-20250201-001', tenantRefId: 1, subscriptionId: 1, invoiceStatus: 'Paid', billingPeriodStart: '2025-02-01T00:00:00Z', billingPeriodEnd: '2025-02-28T23:59:59Z', subtotalAmount: 299, extraAmount: 0, discountAmount: 0, totalAmount: 299, currencyCode: 'CNY', issuedAt: '2025-02-01T00:00:00Z', dueAt: '2025-02-15T00:00:00Z', paidAt: '2025-02-05T14:30:00Z', createdAt: '2025-02-01T00:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Payment Orders ── */
  {
    url: '/api/payment-orders',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, orderNo: 'PAY-20250205-001', tenantRefId: 1, invoiceId: 1, paymentChannel: 'Alipay', paymentStatus: 'Success', amount: 299, currencyCode: 'CNY', thirdPartyTxnNo: 'ALI2025020514300001', paidAt: '2025-02-05T14:30:00Z', createdAt: '2025-02-05T14:29:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Payment Refunds ── */
  {
    url: '/api/payment-refunds',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, refundNo: 'REF-20250601-001', paymentOrderId: 1, refundStatus: 'Success', refundAmount: 149.5, refundReason: '订阅取消退还剩余费用', refundedAt: '2025-06-02T10:00:00Z', createdAt: '2025-06-01T16:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── API Keys ── */
  {
    url: '/api/tenant-api-keys',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, keyName: '生产环境密钥', accessKey: 'ak_huaxia_prod_xxxxxxxxxxxxx', status: 'Active', expiresAt: '2026-01-15T00:00:00Z', createdAt: '2025-01-20T00:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Webhooks ── */
  {
    url: '/api/tenant-webhooks',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, webhookUrl: 'https://hooks.huaxia-tech.com/callback', eventTypes: 'subscription.created,subscription.cancelled', secret: 'whsec_xxxxxxxxxxxxxxxx', status: 'Active', createdAt: '2025-02-01T00:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Notification Templates ── */
  {
    url: '/api/notification-templates',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, templateCode: 'TPL_WELCOME', templateName: '欢迎通知', channel: 'Email', subjectTemplate: '欢迎加入 {{systemName}}', bodyTemplate: '尊敬的{{contactName}}，欢迎！', status: 'Active', createdAt: '2025-01-01T00:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Notifications ── */
  {
    url: '/api/notifications',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, templateId: 1, channel: 'Email', recipient: 'liming@huaxia-tech.com', subject: '欢迎加入平台', body: '尊敬的李明，欢迎！', sendStatus: 'Sent', sentAt: '2025-01-15T10:01:00Z', readAt: '2025-01-15T11:00:00Z', createdAt: '2025-01-15T10:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Storage Strategies ── */
  {
    url: '/api/storage-strategies',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, strategyCode: 'LOCAL_DEFAULT', strategyName: '本地默认存储', providerType: 'Local', bucketName: 'default', basePath: '/data/uploads', status: 'Active', createdAt: '2025-01-01T00:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Tenant Files ── */
  {
    url: '/api/tenant-files',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, storageStrategyId: 2, fileName: '合同扫描件.pdf', filePath: 'tenant-files/1/contracts/合同扫描件.pdf', fileExt: '.pdf', mimeType: 'application/pdf', fileSize: 2048576, uploaderType: 'PlatformUser', uploaderId: 1, visibility: 'Private', downloadCount: 3, createdAt: '2025-02-15T14:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Operation Logs ── */
  {
    url: '/api/operation-logs',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 0, operatorType: 'PlatformUser', operatorId: 1, action: 'CreateTenant', resourceType: 'Tenant', resourceId: '1', ipAddress: '192.168.1.100', operationResult: 'Success', createdAt: '2025-01-15T10:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Audit Logs ── */
  {
    url: '/api/audit-logs',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 0, auditType: 'SecurityEvent', severity: 'High', subjectType: 'PlatformUser', subjectId: '1', complianceTag: 'SOC2', createdAt: '2025-01-15T10:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── System Logs ── */
  {
    url: '/api/system-logs',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, serviceName: 'TenantPlatformApi', logLevel: 'Information', traceId: 'trace-abc-001', message: '租户华夏科技创建成功', createdAt: '2025-01-15T10:00:01Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Tenant Daily Stats ── */
  {
    url: '/api/tenant-daily-stats',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, statDate: '2025-06-01', activeUserCount: 45, newUserCount: 3, apiCallCount: 12580, storageBytes: 5368709120, resourceScore: 85, createdAt: '2025-06-02T00:05:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Platform Monitor Metrics ── */
  {
    url: '/api/platform-monitor-metrics',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, componentName: 'TenantPlatformApi', metricType: 'Performance', metricKey: 'avg_response_time_ms', metricValue: 45.2, metricUnit: 'ms', collectedAt: '2025-06-10T12:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Tenant System Config ── */
  {
    url: '/api/tenant-system-configs/:tenantRefId',
    method: 'GET',
    body: ok({ id: 1, tenantRefId: 1, systemName: '华夏科技管理平台', logoUrl: '/uploads/logos/huaxia.png', systemTheme: 'light', defaultLanguage: 'zh-CN', defaultTimezone: 'Asia/Shanghai' }),
  },

  /* ── Feature Flags ── */
  {
    url: '/api/tenant-feature-flags',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, featureKey: 'enable_advanced_report', featureName: '高级报表', enabled: true, description: '启用高级数据分析报表功能', createdAt: '2025-01-20T00:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Tenant Parameters ── */
  {
    url: '/api/tenant-parameters',
    method: 'GET',
    body: ok({
      items: [
        { id: 1, tenantRefId: 1, paramKey: 'max_login_attempts', paramValue: '5', paramType: 'Integer', description: '最大登录尝试次数', createdAt: '2025-01-20T00:00:00Z' },
      ],
      total: 1, page: 1, pageSize: 20, totalPages: 1,
    }),
  },

  /* ── Catch-all fallbacks for write operations ── */
  { url: '/api/platform-users/:id/enable', method: 'PUT', body: ok(null, '启用成功') },
  { url: '/api/platform-users/:id/disable', method: 'PUT', body: ok(null, '禁用成功') },
  { url: '/api/platform-roles/:id/enable', method: 'PUT', body: ok(null, '启用成功') },
  { url: '/api/platform-roles/:id/disable', method: 'PUT', body: ok(null, '禁用成功') },
  { url: '/api/tenants/:id/status', method: 'PUT', body: ok(null, '状态变更成功') },
  { url: '/api/saas-packages/:id/enable', method: 'PUT', body: ok(null, '启用成功') },
  { url: '/api/saas-packages/:id/disable', method: 'PUT', body: ok(null, '禁用成功') },
  { url: '/api/tenant-subscriptions/:id/cancel', method: 'PUT', body: ok(null, '取消成功') },
  { url: '/api/billing-invoices/:id/void', method: 'PUT', body: ok(null, '作废成功') },
  { url: '/api/tenant-api-keys/:id/disable', method: 'PUT', body: ok(null, '禁用成功') },
  { url: '/api/tenant-webhooks/:id/enable', method: 'PUT', body: ok(null, '启用成功') },
  { url: '/api/tenant-webhooks/:id/disable', method: 'PUT', body: ok(null, '禁用成功') },
  { url: '/api/notification-templates/:id/enable', method: 'PUT', body: ok(null, '启用成功') },
  { url: '/api/notification-templates/:id/disable', method: 'PUT', body: ok(null, '禁用成功') },
  { url: '/api/notifications/:id/read', method: 'PUT', body: ok(null, '标记已读') },
  { url: '/api/storage-strategies/:id/enable', method: 'PUT', body: ok(null, '启用成功') },
  { url: '/api/storage-strategies/:id/disable', method: 'PUT', body: ok(null, '禁用成功') },
  { url: '/api/tenant-feature-flags/:id/toggle', method: 'PUT', body: ok(null, '切换成功') },
  { url: '/api/tenant-tags/bind', method: 'POST', body: ok(null, '绑定成功') },

  /* ── Generic fallback for unknown POST/PUT/DELETE ── */
  { url: '/api/:resource', method: 'POST', body: ok(null, '创建成功') },
  { url: '/api/:resource/:id', method: 'PUT', body: ok(null, '更新成功') },
  { url: '/api/:resource/:id', method: 'DELETE', body: ok(null, '删除成功') },
])

// suppress unused-variable warning
void fail
