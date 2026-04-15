/** Permission tree node (nested from backend) */
export interface PlatformPermissionRepDTO {
  Id: string | number
  Code: string
  Name: string
  PermissionType: string
  ParentId: string | number | null
  Path?: string
  Method?: string
  Children?: PlatformPermissionRepDTO[]
}

/** Flat permission node (for DxTreeList) */
export interface FlatPermission {
  Id: string | number
  Code: string
  Name: string
  PermissionType: string
  ParentId: string | number | null
  Path?: string
  Method?: string
}
