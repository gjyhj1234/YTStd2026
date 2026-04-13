import { httpPost, httpGet } from './api/http'
import type { CurrentUser } from './store/auth'
import type { LoginRepDTO } from './types/auth'

export interface AuthResult<T = unknown> {
  isOk: boolean
  data?: T
  message?: string
}

let _user: CurrentUser | null = null

export default {
  loggedIn(): boolean {
    return !!localStorage.getItem('auth_token')
  },

  async logIn(username: string, password: string): Promise<AuthResult<LoginRepDTO>> {
    try {
      const data = await httpPost<LoginRepDTO>('/auth/login', {
        Username: username,
        Password: password
      }, { skipErrorHandler: true })

      return {
        isOk: true,
        data
      }
    } catch {
      return {
        isOk: false,
        message: 'Authentication failed'
      }
    }
  },

  async logOut(): Promise<void> {
    _user = null
    localStorage.removeItem('auth_token')
  },

  async getUser(): Promise<AuthResult<CurrentUser>> {
    try {
      if (_user) {
        return { isOk: true, data: _user }
      }

      const token = localStorage.getItem('auth_token')
      if (!token) {
        return { isOk: false }
      }

      const data = await httpGet<CurrentUser>('/auth/me', undefined, { skipErrorHandler: true })
      _user = data

      return {
        isOk: true,
        data: _user
      }
    } catch {
      return {
        isOk: false
      }
    }
  },

  async resetPassword(email: string): Promise<AuthResult> {
    try {
      void email
      return { isOk: true }
    } catch {
      return {
        isOk: false,
        message: 'Failed to reset password'
      }
    }
  },

  async changePassword(password: string, recoveryCode: string): Promise<AuthResult> {
    try {
      void password
      void recoveryCode
      return { isOk: true }
    } catch {
      return {
        isOk: false,
        message: 'Failed to change password'
      }
    }
  },

  async createAccount(email: string, password: string): Promise<AuthResult> {
    try {
      void email
      void password
      return { isOk: true }
    } catch {
      return {
        isOk: false,
        message: 'Failed to create account'
      }
    }
  }
}
