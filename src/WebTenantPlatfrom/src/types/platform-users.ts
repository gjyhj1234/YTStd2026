/** User create request */
export interface PlatformUserCreateReqDTO {
  Username: string
  Password: string
  DisplayName: string
  Email: string
  Phone: string
  RoleIds: Array<string | number>
}

/** User update request */
export interface PlatformUserUpdateReqDTO {
  DisplayName: string
  Email: string
  Phone: string
  RoleIds: Array<string | number>
}

/** User response (matches actual backend API) */
export interface PlatformUserRepDTO {
  Id: string | number
  Username: string
  DisplayName: string
  Email: string | null
  Phone: string | null
  Status: string
  MfaEnabled: boolean
  LastLoginAt: string | null
  CreatedAt: string
  Remark: string | null
  RoleIds?: Array<string | number> | null
  RoleNames?: string[] | null
}

/** Reset password response */
export interface ResetPasswordRepDTO {
  GeneratedPassword: string | null
}

/** User role (from /platform-roles/all) */
export interface PlatformUserRoleDTO {
  Id: string | number
  Code: string
  Name: string
  Status: string
}
