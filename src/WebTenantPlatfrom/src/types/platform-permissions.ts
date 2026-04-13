/** Permission tree node (nested from backend) */
export interface PlatformPermissionRepDTO {
  Id: number
  Code: string
  Name: string
  PermissionType: string
  ParentId: number | null
  Path?: string
  Method?: string
  Children?: PlatformPermissionRepDTO[]
}

/** Flat permission node (for DxTreeList) */
export interface FlatPermission {
  Id: number
  Code: string
  Name: string
  PermissionType: string
  ParentId: number | null
  Path?: string
  Method?: string
}
