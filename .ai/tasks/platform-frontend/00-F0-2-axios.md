# 子任务 F0-2 — axios 封装

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F0-2 |
| 模块名称 | axios HTTP 封装 |
| 并行组 | — （串行，基础设施） |
| 对应提示词 | `.ai/prompts/03-frontend/05-axios-standard.md` |
| 后端 API 提示词 | 无 |
| 依赖任务 | F0-1 脚手架搭建（✅ 已完成） |
| 完成会话 | `session-summary-20260413-09` |
| 状态 | ✅ 已完成 |

---

## 任务目标

实现标准化的 axios 封装，包括实例创建、请求/响应拦截器、Token 注入、ApiResult 拆包、错误处理、上传/下载、防重复提交等。为所有业务模块 API 提供统一的 HTTP 调用基础。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/03-frontend/05-axios-standard.md` — axios 标准化实现规范（可执行规范，非原则说明）

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/api/http.types.ts` | HTTP 相关类型定义（ApiResult、PagedResult、PagedQuery、RequestOptions） |
| `src/WebTenantPlatfrom/src/api/http.ts` | axios 实例与拦截器（核心文件） |

---

## 核心功能清单

| 功能 | 说明 |
|------|------|
| axios 实例 | baseURL、timeout、Content-Type 配置 |
| 请求拦截器 | 防重复提交、Bearer Token 注入 |
| 响应拦截器 | ApiResult 拆包、业务错误处理、HTTP 错误处理（401/403/500） |
| BusinessError | 自定义错误类，携带 Code 和 Message |
| 封装方法 | httpGet、httpPost、httpPut、httpDelete、httpUpload、httpDownload |
| 请求取消 | createCancelToken 用于组件卸载取消 |

---

## 验收标准

- [x] `http.types.ts` 包含 ApiResult、PagedResult、PagedQuery、RequestOptions 类型
- [x] `http.ts` 包含 axios 实例创建
- [x] 请求拦截器包含防重复提交、Token 注入
- [x] 响应拦截器包含 ApiResult 拆包、业务错误处理、HTTP 错误处理
- [x] 封装方法完整（httpGet、httpPost、httpPut、httpDelete、httpUpload、httpDownload）
- [x] BusinessError 错误类已定义
- [x] createCancelToken 已实现
- [x] `npm run build` 通过

---

## 已完成说明

本子任务已在 `session-summary-20260413-09` 中完成。主要产出：

1. `api/http.types.ts` — 类型定义（ApiResult、PagedResult、PagedQuery、RequestOptions）
2. `api/http.ts` — 完整 axios 封装：
   - axios 实例（baseURL、timeout、Content-Type）
   - 请求拦截器（防重复提交、Token 注入占位）
   - 响应拦截器（ApiResult 拆包、业务错误处理、HTTP 错误处理）
   - BusinessError 错误类
   - 封装方法：httpGet、httpPost、httpPut、httpDelete、httpUpload、httpDownload
   - createCancelToken 用于组件卸载取消

> **注意**：Token 注入在 F0-2 中为占位注释，在 F0-4（路由与权限守卫）中启用了实际的 Bearer Token 注入。
