# API 设计规范

## 目标

统一后端 API 的路径设计、请求格式、响应格式、错误处理和权限控制。

---

## 适用范围

所有后端 WebAPI 端点。

---

## 路径设计

### 基本规则

- 使用 RESTful 风格
- 路径使用 kebab-case（小写 + 连字符）
- 资源名使用复数形式
- 路径前缀统一为 `/api/`
- 路径层级不超过 3 层

### 路径模式

| 操作 | HTTP 方法 | 路径 | 说明 |
|------|----------|------|------|
| 列表 | GET | `/api/{resources}` | 支持分页、筛选、排序 |
| 详情 | GET | `/api/{resources}/{id}` | 按 ID 获取 |
| 创建 | POST | `/api/{resources}` | 创建新资源 |
| 更新 | PUT | `/api/{resources}/{id}` | 全量更新 |
| 删除 | DELETE | `/api/{resources}/{id}` | 删除资源 |
| 启用 | PUT | `/api/{resources}/{id}/enable` | 状态操作 |
| 禁用 | PUT | `/api/{resources}/{id}/disable` | 状态操作 |
| 批量操作 | POST | `/api/{resources}/batch-{action}` | 批量操作 |
| 存在性检查 | GET | `/api/{resources}/check-{field}-exists` | 唯一性检查 |
| 统计 | GET | `/api/{resources}/statistics` | 统计数据 |

### 路径示例

```
GET    /api/platform-users              # 用户列表
GET    /api/platform-users/123          # 用户详情
POST   /api/platform-users              # 创建用户
PUT    /api/platform-users/123          # 编辑用户
DELETE /api/platform-users/123          # 删除用户
PUT    /api/platform-users/123/enable   # 启用用户
PUT    /api/platform-users/123/disable  # 禁用用户
POST   /api/platform-users/batch-enable # 批量启用
GET    /api/platform-users/check-username-exists?username=xxx  # 检查用户名
```

---

## 请求格式

### 查询参数

列表接口的分页和筛选使用查询参数：

```
GET /api/platform-users?page=1&pageSize=20&keyword=admin&status=1
```

标准分页参数：
- `page`：页码，从 1 开始
- `pageSize`：每页条数，默认 20，最大 100
- `keyword`：模糊搜索关键词
- `status`：状态筛选
- `sortField`：排序字段
- `sortOrder`：排序方向（`asc` / `desc`）

### 请求体

创建和更新使用 JSON 请求体：

```json
{
  "Username": "admin",
  "DisplayName": "管理员",
  "Email": "admin@example.com"
}
```

---

## 响应格式

### 统一响应结构

所有接口统一返回 `ApiResult<T>`：

```json
{
  "Code": 0,
  "Message": 0,
  "Data": { ... }
}
```

### 字段说明

| 字段 | 类型 | 说明 |
|------|------|------|
| `Code` | `int` | 0=成功，非零=错误码 |
| `Message` | `int` | 整形 Code（前端根据此 Code 翻译） |
| `Data` | `T` | 业务数据（可选） |

### 分页响应

```json
{
  "Code": 0,
  "Message": 0,
  "Data": {
    "Items": [...],
    "Total": 100,
    "Page": 1,
    "PageSize": 20
  }
}
```

### JSON 格式

- 属性名使用 PascalCase
- 不配置 `PropertyNamingPolicy`
- 日期格式：ISO 8601
- 空集合返回 `[]` 而非 `null`
- 空对象返回 `null` 而非 `{}`

---

## 错误码体系

### 错误码分段

| 范围 | 用途 | 示例 |
|------|------|------|
| `0` | 成功 | — |
| `10xxx` | 认证错误 | `10001` 无效凭证 |
| `11xxx` | 权限错误 | `11001` 权限不足 |
| `12xxx` | 输入验证错误 | `12001` 参数缺失 |
| `18xxx` | 唯一性冲突 | `18001` 用户名已存在 |
| `19xxx` | 业务规则错误 | `19001` 状态不允许操作 |
| `50xxx` | 系统错误 | `50001` 内部错误 |

### 错误码定义

所有错误码必须定义在 `Application/Constants/ErrorCodes.cs` 中：

```csharp
public static class ErrorCodes
{
    public const int Success = 0;
    public const int InvalidCredentials = 10001;
    public const int PermissionDenied = 11001;
    public const int UsernameExists = 18001;
}
```

### 错误处理

`ApiResult.Fail()` 仅接受 `ErrorCodes.XXX` 单个参数，不传 message：

```csharp
// ✅ 正确
return ApiResult.Fail(ErrorCodes.UsernameExists);

// ❌ 禁止 — 不再允许 message 参数
return ApiResult.Fail(ErrorCodes.UsernameExists, xxx);
```

后端不存在 Messages 类，所有错误/提示统一通过 ErrorCodes 整形常量表达。

---

## 权限控制

### 权限码命名

格式：`{模块}.{资源}.{操作}`

```
platform.user.list
platform.user.create
platform.user.update
platform.user.delete
platform.user.enable
platform.user.disable
tenant.lifecycle.create
tenant.lifecycle.suspend
```

### 权限检查

- 所有非公开端点必须有权限码标注
- 权限检查通过权限中间件执行
- 超级管理员跳过权限检查

### 存在性检查端点

所有有唯一索引的实体必须提供 check-exists 端点：

```
GET /api/platform-users/check-username-exists?username=xxx
```

响应：`{ "Code": 0, "Data": true/false }`

---

## 认证

### Token 传递

- 使用 Bearer Token
- 通过 `Authorization` Header 传递
- Token 中包含：userId、roles、permissions、isSuperAdmin

### 公开端点

以下端点不需要认证：
- `/api/auth/login`
- `/api/health`
- `/api/health/ready`

---

## 跨模块 API 前端类型契约（零容忍）

当前端模块调用其他模块的后端 API 时，前端 TypeScript 类型必须与后端实际返回的 DTO 字段精确一一对应。

### 规则

1. 后端返回 `PlatformRoleSimpleRepDTO { Id, Code, Name, Status }` 时，前端不得用 `PlatformRoleRepDTO { Id, Code, Name, Description, Status, CreatedAt }` 接收。必须新建 `PlatformRoleSimpleRepDTO` 类型。
2. 每个业务提示词的"API 端点"章节后面必须有"跨模块 API 依赖"小节，列出本模块依赖的其他模块 API 及其精确返回类型。
3. 前端 API 函数的 TypeScript 泛型参数必须与后端 DTO 精确对应：
   - `httpGet<PlatformRoleSimpleRepDTO[]>('/platform-roles/all')` ✅
   - `httpGet<PlatformRoleRepDTO[]>('/platform-roles/all')` ❌

### 批量操作 API

批量操作 API 必须传 `{ preventDuplicate: false }` 以禁用 axios 拦截器的重复请求防护。同时在 UI 层使用 `batchOperating` ref 锁定按钮。
