export const mockTenantSystemConfigs = [
  {
    id: 1,
    tenantRefId: 1,
    systemName: '华夏科技管理平台',
    logoUrl: '/uploads/logos/huaxia.png',
    systemTheme: 'light',
    defaultLanguage: 'zh-CN',
    defaultTimezone: 'Asia/Shanghai',
  },
  {
    id: 2,
    tenantRefId: 2,
    systemName: '云海数据服务中心',
    logoUrl: '/uploads/logos/yunhai.png',
    systemTheme: 'dark',
    defaultLanguage: 'zh-CN',
    defaultTimezone: 'Asia/Shanghai',
  },
]

export const mockTenantFeatureFlags = [
  {
    id: 1,
    tenantRefId: 1,
    featureKey: 'enable_advanced_report',
    featureName: '高级报表',
    enabled: true,
    description: '启用高级数据分析报表功能',
    createdAt: '2025-01-20T00:00:00Z',
  },
  {
    id: 2,
    tenantRefId: 1,
    featureKey: 'enable_api_access',
    featureName: 'API接入',
    enabled: true,
    description: '允许通过API访问平台服务',
    createdAt: '2025-01-20T00:00:00Z',
  },
  {
    id: 3,
    tenantRefId: 2,
    featureKey: 'enable_advanced_report',
    featureName: '高级报表',
    enabled: false,
    description: '启用高级数据分析报表功能',
    createdAt: '2025-02-05T00:00:00Z',
  },
]

export const mockTenantParameters = [
  {
    id: 1,
    tenantRefId: 1,
    paramKey: 'max_login_attempts',
    paramValue: '5',
    paramType: 'Integer',
    description: '最大登录尝试次数',
    createdAt: '2025-01-20T00:00:00Z',
  },
  {
    id: 2,
    tenantRefId: 1,
    paramKey: 'session_timeout_minutes',
    paramValue: '30',
    paramType: 'Integer',
    description: '会话超时时间（分钟）',
    createdAt: '2025-01-20T00:00:00Z',
  },
  {
    id: 3,
    tenantRefId: 2,
    paramKey: 'max_login_attempts',
    paramValue: '3',
    paramType: 'Integer',
    description: '最大登录尝试次数',
    createdAt: '2025-02-05T00:00:00Z',
  },
]
