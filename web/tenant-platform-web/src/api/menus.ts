/** API — 菜单管理 */
import { get, post, put, del } from '@/utils/http'
import type { MenuRepDTO, CreateMenuReqDTO, UpdateMenuReqDTO } from '@/types/menu'

export type { MenuRepDTO, CreateMenuReqDTO, UpdateMenuReqDTO }

/** 获取菜单树 */
export function getMenuTree() {
  return get<MenuRepDTO[]>('/api/menus/tree')
}

/** 创建菜单 */
export function createMenu(data: CreateMenuReqDTO) {
  return post<number>('/api/menus', data)
}

/** 更新菜单 */
export function updateMenu(id: number, data: UpdateMenuReqDTO) {
  return put<void>(`/api/menus/${id}`, data)
}

/** 删除菜单 */
export function deleteMenu(id: number) {
  return del<void>(`/api/menus/${id}`)
}

/** 启用菜单 */
export function enableMenu(id: number) {
  return put<void>(`/api/menus/${id}/enable`)
}

/** 禁用菜单 */
export function disableMenu(id: number) {
  return put<void>(`/api/menus/${id}/disable`)
}

/** 调整排序 */
export function setMenuSort(id: number, sortOrder: number) {
  return put<void>(`/api/menus/${id}/sort`, { SortOrder: sortOrder })
}
