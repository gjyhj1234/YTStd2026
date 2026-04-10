/** 仪表盘统计数据 */
export interface DashboardStats {
  TotalTenants: number
  ActiveTenants: number
  TotalSubscriptions: number
  TotalUsers: number
  ExpiringTenants: number
  TrialTenants: number
}

/** 租户增长趋势项 */
export interface TenantTrendItem {
  Date: string
  Count: number
}

/** 订阅分布项 */
export interface SubscriptionDistItem {
  PackageName: string
  Count: number
}
