export const mockTenantDomains = [
  {
    id: 1,
    tenantRefId: 1,
    domain: 'huaxia.ytstd.com',
    domainType: 'SubDomain',
    isPrimary: true,
    verificationStatus: 'Verified',
    createdAt: '2025-01-15T10:00:00Z',
  },
  {
    id: 2,
    tenantRefId: 1,
    domain: 'www.huaxia-tech.com',
    domainType: 'CustomDomain',
    isPrimary: false,
    verificationStatus: 'Pending',
    createdAt: '2025-02-01T08:00:00Z',
  },
  {
    id: 3,
    tenantRefId: 2,
    domain: 'yunhai.ytstd.com',
    domainType: 'SubDomain',
    isPrimary: true,
    verificationStatus: 'Verified',
    createdAt: '2025-02-01T08:30:00Z',
  },
]
