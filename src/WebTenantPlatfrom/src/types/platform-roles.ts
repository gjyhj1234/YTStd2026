/** Role create request */
export interface CreatePlatformRoleReqDTO {
  Code: string
  Name: string
  Description?: string
}

/** Role update request */
export interface UpdatePlatformRoleReqDTO {
  Name?: string
  Description?: string
}

/** Role response */
export interface PlatformRoleRepDTO {
  Id: number
  Code: string
  Name: string
  Description?: string
  Status: string
  CreatedAt: string
}

/** Role permission bind request */
export interface RolePermissionBindReqDTO {
  PermissionIds: number[]
}

/** Role member bind request */
export interface RoleMemberBindReqDTO {
  UserIds: number[]
}
