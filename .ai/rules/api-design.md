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
  "Message": "success",
  "Data": { ... }
}
```

### 字段说明

| 字段 | 类型 | 说明 |
|------|------|------|
| `Code` | `int` | 0=成功，非零=错误码 |
| `Message` | `string` | 成功提示或 i18n 错误键 |
| `Data` | `T` | 业务数据（可选） |

### 分页响应

```json
{
  "Code": 0,
  "Message": "success",
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

### 错误消息

所有错误消息必须定义在 `Application/Constants/Messages.cs` 中，使用 i18n key：

```csharp
public static class Messages
{
    public const string Success = "common.success";
    public const string InvalidCredentials = "auth.invalid_credentials";
    public const string UsernameExists = "user.username_exists";
}
```

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
