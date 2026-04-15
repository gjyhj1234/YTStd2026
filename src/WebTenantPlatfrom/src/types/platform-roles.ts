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
  Id: string | number
  Code: string
  Name: string
  Description?: string
  Status: string
  CreatedAt: string
}

/** Role permission bind request */
export interface RolePermissionBindReqDTO {
  PermissionIds: Array<string | number>
}

/** Role member bind request */
export interface RoleMemberBindReqDTO {
  UserIds: Array<string | number>
}

/** Role simple response (from /platform-roles/all, for dropdown/select) */
export interface PlatformRoleSimpleRepDTO {
  Id: string | number
  Code: string
  Name: string
  Status: string
}
