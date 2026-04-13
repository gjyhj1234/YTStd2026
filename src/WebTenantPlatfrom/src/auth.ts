export interface UserInfo {
  email: string
  avatarUrl: string
}

export interface AuthResult<T = unknown> {
  isOk: boolean
  data?: T
  message?: string
}

const defaultUser: UserInfo = {
  email: 'sandra@example.com',
  avatarUrl: 'https://js.devexpress.com/Demos/WidgetsGallery/JSDemos/images/employees/06.png'
}

export default {
  _user: defaultUser as UserInfo | null,

  loggedIn(): boolean {
    return !!this._user
  },

  async logIn(email: string, password: string): Promise<AuthResult<UserInfo>> {
    try {
      // Send request
      console.log(email, password)
      this._user = { ...defaultUser, email }

      return {
        isOk: true,
        data: this._user
      }
    } catch {
      return {
        isOk: false,
        message: 'Authentication failed'
      }
    }
  },

  async logOut(): Promise<void> {
    this._user = null
  },

  async getUser(): Promise<AuthResult<UserInfo>> {
    try {
      // Send request
      return {
        isOk: true,
        data: this._user!
      }
    } catch {
      return {
        isOk: false
      }
    }
  },

  async resetPassword(email: string): Promise<AuthResult> {
    try {
      // Send request
      console.log(email)

      return {
        isOk: true
      }
    } catch {
      return {
        isOk: false,
        message: 'Failed to reset password'
      }
    }
  },

  async changePassword(email: string, recoveryCode: string): Promise<AuthResult> {
    try {
      // Send request
      console.log(email, recoveryCode)

      return {
        isOk: true
      }
    } catch {
      return {
        isOk: false,
        message: 'Failed to change password'
      }
    }
  },

  async createAccount(email: string, password: string): Promise<AuthResult> {
    try {
      // Send request
      console.log(email, password)

      return {
        isOk: true
      }
    } catch {
      return {
        isOk: false,
        message: 'Failed to create account'
      }
    }
  }
}
