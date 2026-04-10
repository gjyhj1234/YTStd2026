/** 字典项 */
export interface DictionaryRepDTO {
  Id: number
  TypeCode: string
  TypeName: string
  ItemCode: string
  ItemName: string
  ItemValue?: string
  IsEnabled: boolean
  SortOrder: number
  Remark?: string
  CreatedAt: string
}

/** 字典类型摘要 */
export interface DictionaryTypeRepDTO {
  TypeCode: string
  TypeName: string
  ItemCount: number
}

/** 创建字典项请求 */
export interface CreateDictionaryReqDTO {
  TypeCode: string
  TypeName: string
  ItemCode: string
  ItemName: string
  ItemValue?: string
  SortOrder?: number
  Remark?: string
}

/** 更新字典项请求 */
export interface UpdateDictionaryReqDTO {
  ItemName: string
  ItemValue?: string
  SortOrder?: number
  Remark?: string
}
