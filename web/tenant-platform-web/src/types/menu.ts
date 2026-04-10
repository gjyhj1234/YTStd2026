/** 菜单树节点 */
export interface MenuRepDTO {
  Id: number
  ParentId: number
  Code: string
  Name: string
  Icon?: string
  RoutePath?: string
  ComponentPath?: string
  PermissionCode?: string
  MenuType: number
  IsEnabled: boolean
  IsExternal: boolean
  IsVisible: boolean
  SortOrder: number
  Remark?: string
  CreatedAt: string
  Children?: MenuRepDTO[]
}

/** 创建菜单请求 */
export interface CreateMenuReqDTO {
  ParentId: number
  Code: string
  Name: string
  Icon?: string
  RoutePath?: string
  ComponentPath?: string
  PermissionCode?: string
  MenuType: number
  IsExternal?: boolean
  IsVisible?: boolean
  SortOrder?: number
  Remark?: string
}

/** 更新菜单请求 */
export interface UpdateMenuReqDTO {
  Name: string
  Icon?: string
  RoutePath?: string
  ComponentPath?: string
  PermissionCode?: string
  IsExternal?: boolean
  IsVisible?: boolean
  Remark?: string
}
