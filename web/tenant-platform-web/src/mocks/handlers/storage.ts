import { http, HttpResponse } from 'msw'
import {
  mockStorageStrategies,
  mockTenantFiles,
  mockFileAccessPolicies,
} from '../data/storage'
import { ok, paged, getPageParams } from '../data/common'

export const storageHandlers = [
  http.get('/api/storage-strategies', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockStorageStrategies, page, pageSize))
  }),

  http.get('/api/storage-strategies/:id', ({ params }) => {
    const s = mockStorageStrategies.find((s) => s.id === Number(params['id']))
    if (!s)
      return HttpResponse.json(
        { success: false, message: '存储策略不存在', data: null, traceId: '' },
        { status: 404 },
      )
    return HttpResponse.json(ok(s))
  }),

  http.post('/api/storage-strategies', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newStrategy = {
      id: mockStorageStrategies.length + 1,
      strategyCode: body['strategyCode'] as string,
      strategyName: body['strategyName'] as string,
      providerType: body['providerType'] as string,
      bucketName: body['bucketName'] as string,
      basePath: body['basePath'] as string,
      status: 'Active',
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newStrategy, '创建成功'))
  }),

  http.put('/api/storage-strategies/:id', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    return HttpResponse.json(ok(body, '更新成功'))
  }),

  http.put('/api/storage-strategies/:id/enable', () => {
    return HttpResponse.json(ok(null, '启用成功'))
  }),

  http.put('/api/storage-strategies/:id/disable', () => {
    return HttpResponse.json(ok(null, '禁用成功'))
  }),

  http.get('/api/tenant-files', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockTenantFiles, page, pageSize))
  }),

  http.get('/api/tenant-files/:id', ({ params }) => {
    const f = mockTenantFiles.find((f) => f.id === Number(params['id']))
    if (!f)
      return HttpResponse.json(
        { success: false, message: '文件不存在', data: null, traceId: '' },
        { status: 404 },
      )
    return HttpResponse.json(ok(f))
  }),

  http.delete('/api/tenant-files/:id', () => {
    return HttpResponse.json(ok(null, '删除成功'))
  }),

  http.get('/api/file-access-policies', ({ request }) => {
    const url = new URL(request.url)
    const { page, pageSize } = getPageParams(url)
    return HttpResponse.json(paged(mockFileAccessPolicies, page, pageSize))
  }),

  http.post('/api/file-access-policies', async ({ request }) => {
    const body = (await request.json()) as Record<string, unknown>
    const newPolicy = {
      id: mockFileAccessPolicies.length + 1,
      fileId: body['fileId'] as number,
      subjectType: body['subjectType'] as string,
      subjectId: body['subjectId'] as number,
      permissionCode: body['permissionCode'] as string,
      createdAt: new Date().toISOString(),
    }
    return HttpResponse.json(ok(newPolicy, '创建成功'))
  }),
]
