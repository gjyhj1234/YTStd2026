import { http, HttpResponse } from 'msw'
import { mockLoginResult, mockCurrentUser } from '../data/auth'
import { ok, fail } from '../data/common'

export const authHandlers = [
  http.post('/api/auth/login', async ({ request }) => {
    const body = (await request.json()) as { username: string; password: string }
    if (body.username === 'admin' && body.password === 'admin123') {
      return HttpResponse.json(ok(mockLoginResult, '登录成功'))
    }
    return HttpResponse.json(fail('用户名或密码错误'), { status: 401 })
  }),

  http.post('/api/auth/logout', () => {
    return HttpResponse.json(ok(null, '登出成功'))
  }),

  http.get('/api/auth/me', () => {
    return HttpResponse.json(ok(mockCurrentUser))
  }),
]
