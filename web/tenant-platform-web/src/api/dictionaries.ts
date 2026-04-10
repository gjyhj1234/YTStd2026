/** API — 数据字典 */
import { get, post, put, del } from '@/utils/http'
import type { DictionaryRepDTO, DictionaryTypeRepDTO, CreateDictionaryReqDTO, UpdateDictionaryReqDTO } from '@/types/dictionary'

export type { DictionaryRepDTO, DictionaryTypeRepDTO, CreateDictionaryReqDTO, UpdateDictionaryReqDTO }

/** 获取字典类型列表 */
export function getDictionaryTypes() {
  return get<DictionaryTypeRepDTO[]>('/api/dictionaries/types')
}

/** 按类型获取字典项 */
export function getDictionaryByType(typeCode: string) {
  return get<DictionaryRepDTO[]>(`/api/dictionaries/types/${encodeURIComponent(typeCode)}`)
}

/** 创建字典项 */
export function createDictionary(data: CreateDictionaryReqDTO) {
  return post<number>('/api/dictionaries', data)
}

/** 更新字典项 */
export function updateDictionary(id: number, data: UpdateDictionaryReqDTO) {
  return put<void>(`/api/dictionaries/${id}`, data)
}

/** 删除字典项 */
export function deleteDictionary(id: number) {
  return del<void>(`/api/dictionaries/${id}`)
}
