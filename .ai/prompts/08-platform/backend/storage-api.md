# 租户平台 — 文件存储 API

## 目标

重构文件存储 API。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/security.md`
- `backend/error-codes.md`

---

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| POST | `/api/files/upload` | 上传文件 |
| GET | `/api/files/{id}` | 获取文件信息 |
| GET | `/api/files/{id}/download` | 下载文件 |
| DELETE | `/api/files/{id}` | 删除文件 |
| GET | `/api/files` | 文件列表 |

---

## 业务规则

1. 文件存储在本地磁盘
2. 文件元信息存储在 `sys_file` 表
3. 限制上传文件大小
4. 限制允许的文件类型
5. 文件路径不暴露给前端

---

## 验收标准

- [ ] 上传/下载/删除正确
- [ ] 文件类型检查
- [ ] 文件大小限制
- [ ] 编译通过
