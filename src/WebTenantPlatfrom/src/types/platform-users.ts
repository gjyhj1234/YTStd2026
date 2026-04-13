/** User create request */
export interface PlatformUserCreateReqDTO {
  Username: string
  Password: string
  DisplayName: string
  Email: string
  Phone: string
  RoleIds: number[]
}

/** User update request */
export interface PlatformUserUpdateReqDTO {
  DisplayName: string
  Email: string
  Phone: string
  RoleIds: number[]
}

/** User response */
export interface PlatformUserRepDTO {
  Id: number
  Username: string
  DisplayName: string
  Email: string
  Phone: string
  Status: number
  Roles: PlatformUserRoleDTO[]
  CreatedAt: string
  UpdatedAt: string
}

/** User role */
export interface PlatformUserRoleDTO {
  Id: number
  Name: string
}
