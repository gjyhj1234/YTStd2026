import { httpPost } from './http'
import type { LoginReqDTO, LoginRepDTO } from '../types/auth'

/** 登录 */
export function loginApi(data: LoginReqDTO): Promise<LoginRepDTO> {
  return httpPost<LoginRepDTO>('/auth/login', data, { skipErrorHandler: true })
}
