export const mockTenantResourceQuotas = [
  {
    id: 1,
    tenantRefId: 1,
    quotaType: 'MaxUsers',
    quotaLimit: 100,
    warningThreshold: 80,
    resetCycle: 'None',
    createdAt: '2025-01-15T10:00:00Z',
  },
  {
    id: 2,
    tenantRefId: 1,
    quotaType: 'StorageGB',
    quotaLimit: 50,
    warningThreshold: 40,
    resetCycle: 'Monthly',
    createdAt: '2025-01-15T10:00:00Z',
  },
  {
    id: 3,
    tenantRefId: 2,
    quotaType: 'ApiCallsPerDay',
    quotaLimit: 10000,
    warningThreshold: 8000,
    resetCycle: 'Daily',
    createdAt: '2025-02-01T08:30:00Z',
  },
]
