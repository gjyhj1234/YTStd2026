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

/** User response (matches actual backend API) */
export interface PlatformUserRepDTO {
  Id: number
  Username: string
  DisplayName: string
  Email: string | null
  Phone: string | null
  Status: string
  MfaEnabled: boolean
  LastLoginAt: string | null
  CreatedAt: string
  Remark: string | null
}

/** Reset password response */
export interface ResetPasswordRepDTO {
  GeneratedPassword: string | null
}

/** User role (from /platform-roles/all) */
export interface PlatformUserRoleDTO {
  Id: number
  Code: string
  Name: string
  Status: string
}
