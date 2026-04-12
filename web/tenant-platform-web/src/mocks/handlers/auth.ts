import { http, HttpResponse } from 'msw'
import { mockLoginResult, mockCurrentUser } from '../data/auth'
import { ok, fail } from '../data/common'

export const authHandlers = [
  http.post('/api/auth/login', async ({ request }) => {
    const body = (await request.json()) as { Username: string; Password: string }
    if (body.Username === 'admin' && body.Password === 'admin123') {
      return HttpResponse.json(ok(mockLoginResult))
    }
    return HttpResponse.json(fail(10002), { status: 401 })
  }),

  http.post('/api/auth/refresh-token', () => {
    return HttpResponse.json(ok(mockLoginResult))
  }),

  http.get('/api/auth/me', () => {
    return HttpResponse.json(ok(mockCurrentUser))
  }),

  http.post('/api/auth/change-password', async ({ request }) => {
    const body = (await request.json()) as { OldPassword: string; NewPassword: string }
    if (body.OldPassword === 'admin123') {
      return HttpResponse.json(ok(null))
    }
    return HttpResponse.json(fail(10003))
  }),
]
