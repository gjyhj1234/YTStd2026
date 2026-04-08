# 命名规范总则

## 目标

统一项目中所有命名规则，确保前端、后端、数据库、API、文件的命名一致。

---

## 适用范围

项目中的所有代码、文件、目录、数据库对象。

---

## 命名风格对照表

| 位置 | 风格 | 示例 |
|------|------|------|
| C# 类名 | PascalCase | `PlatformUser`, `TenantLifecycleAppService` |
| C# 方法名 | PascalCase | `CreateAsync`, `GetListAsync` |
| C# 属性名 | PascalCase | `Username`, `DisplayName` |
| C# 私有字段 | _camelCase | `_cache`, `_logger` |
| C# 局部变量 | camelCase | `userId`, `roleList` |
| C# 常量 | PascalCase | `ErrorCodes.UsernameExists` |
| C# 命名空间 | PascalCase | `YTStdTenantPlatform.Application.Services` |
| 数据库表名 | snake_case | `sys_user`, `sys_tenant` |
| 数据库列名 | snake_case | `username`, `display_name`, `created_at` |
| 数据库索引名 | snake_case | `idx_sys_user_username` |
| API 路径 | kebab-case | `/api/platform-users`, `/api/tenant-lifecycle-events` |
| JSON 属性名 | PascalCase | `Code`, `Message`, `Data`, `Items` |
| TypeScript 类型名 | PascalCase | `PlatformUser`, `ApiResult` |
| TypeScript 变量名 | camelCase | `userId`, `pageSize` |
| TypeScript 常量 | UPPER_SNAKE_CASE | `PLATFORM_USER_LIST` |
| Vue 文件名 | PascalCase | `ListView.vue`, `CreateView.vue` |
| Vue 组件名 | PascalCase | `FunctionDescriptionCard` |
| TS/JS 文件名 | camelCase | `platformUser.ts`, `http.ts` |
| CSS 类名 | kebab-case | `user-list`, `form-container` |
| 目录名 | kebab-case | `platform-users/`, `tenant-lifecycle/` |
| 国际化 key | 中文原文 | `用户列表`, `保存`, `用户名` |
| 权限码 | dot.separated | `platform.user.list`, `tenant.lifecycle.create` |
| 错误码常量 | PascalCase | `ErrorCodes.UsernameExists`（`const int`） |
| 配置键 | PascalCase | `ConnectionStrings:Default` |

---

## 后端命名详则

### 类命名

| 类型 | 后缀 | 示例 |
|------|------|------|
| 实体 | 无后缀 | `PlatformUser` |
| 请求 DTO | `ReqDTO` | `CreatePlatformUserReqDTO` |
| 响应 DTO | `RepDTO` | `PlatformUserRepDTO` |
| 应用服务 | `AppService` | `PlatformUserAppService` |
| 端点 | `Endpoints` | `PlatformUserEndpoints` |
| 中间件 | `Middleware` | `PermissionMiddleware` |
| 缓存 | `Cache` | `PermissionSnapshotCache` |
| 枚举 | 按语义 | `UserStatus`, `TenantStatus` |
| 初始化贡献者 | `SeedContributor` | `PermissionSeedContributor` |

### 方法命名

| 操作 | 命名 | 示例 |
|------|------|------|
| 查询单个 | `GetAsync` | `GetAsync(long id)` |
| 查询列表 | `GetListAsync` | `GetListAsync(query)` |
| 创建 | `CreateAsync` | `CreateAsync(dto)` |
| 更新 | `UpdateAsync` | `UpdateAsync(id, dto)` |
| 删除 | `DeleteAsync` | `DeleteAsync(id)` |
| 启用 | `EnableAsync` | `EnableAsync(id)` |
| 禁用 | `DisableAsync` | `DisableAsync(id)` |
| 检查存在 | `CheckExistsAsync` | `CheckUsernameExistsAsync(username)` |
| 批量操作 | `Batch{Action}Async` | `BatchEnableAsync(ids)` |

---

## 前端命名详则

### 文件命名

| 类型 | 命名 | 示例 |
|------|------|------|
| 页面视图 | PascalCase + View | `ListView.vue`, `DetailView.vue` |
| 通用组件 | PascalCase | `FunctionDescriptionCard.vue` |
| API 模块 | camelCase | `platformUser.ts` |
| 类型模块 | camelCase | `platformUser.ts` |
| 工具函数 | camelCase | `http.ts`, `format.ts` |
| 状态模块 | camelCase | `auth.ts`, `app.ts` |

### 目录命名

- 视图目录使用 kebab-case：`views/platform-users/`
- 其他目录使用 camelCase 或 kebab-case（保持项目一致）

---

## 国际化 key 命名

### 格式

前端使用中文作为 key（最终方案）。

### 示例

```
用户列表                        # 页面标题
用户名                          # 字段标签
保存                            # 按钮
确认删除                        # 操作提示
```

### 规则

- key 使用中文原文
- 同文本只允许一个 key（禁止同词多义）
- 禁止使用 dot.separated 英文 key（如 `user.username_exists`）
- 禁止歧义词

---

## 权限码命名

### 格式

`{域}.{资源}.{操作}`

### 示例

```
platform.user.list
platform.user.create
platform.user.update
platform.user.delete
platform.role.list
platform.role.authorize
tenant.lifecycle.create
tenant.lifecycle.suspend
tenant.lifecycle.terminate
saas.package.list
saas.subscription.create
```

---

## 禁止的命名

| 禁止项 | 原因 |
|-------|------|
| 单字母变量（循环变量除外） | 可读性差 |
| 含义模糊的缩写 | 歧义 |
| 拼音命名 | 非标准 |
| 数字开头 | 语法限制 |
| 下划线开头的公开成员 | 与私有字段混淆 |
| `temp`、`data`、`info` 等无具体含义的名称 | 语义不明 |
