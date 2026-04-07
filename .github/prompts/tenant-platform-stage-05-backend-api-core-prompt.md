# 租户平台阶段 05：核心后端 API 与应用服务提示词

## 目标

在实体、初始化、基础设施稳定后，本阶段聚焦**核心业务域**的应用服务与 WebAPI：

1. 平台管理体系
2. 租户生命周期体系
3. 租户信息体系
4. 租户资源管理
5. 租户配置中心

请优先完成这些高频、强依赖前端的核心模块。

---

## 必须阅读的文件

- `.github/prompts/tenant-platform-backend-prompt.md`
- `docs/TenantPlatform/architecture.md`
- `docs/TenantPlatform/database_dictionary.md`
- 已完成的实体、初始化、基础设施代码

---

## 本阶段范围

### 应用服务
至少考虑：
- PlatformUserAppService
- PlatformRoleAppService
- PlatformPermissionAppService
- PlatformSecurityAppService
- TenantLifecycleAppService
- TenantInfoAppService
- TenantResourceAppService
- TenantConfigAppService

### Endpoint 分组
至少考虑：
- `/api/platform-users`
- `/api/platform-roles`
- `/api/platform-permissions`
- `/api/platform-security`
- `/api/tenants`
- `/api/tenant-lifecycle-events`
- `/api/tenant-groups`
- `/api/tenant-domains`
- `/api/tenant-tags`
- `/api/tenant-resource-quotas`
- `/api/tenant-system-configs`
- `/api/tenant-feature-flags`

### API 能力要求
- 列表 / 分页 / 筛选 / 状态过滤
- 详情
- 新增 / 编辑
- 启用 / 禁用
- 授权 / 绑定
- 状态流转
- 审计查询入口（如适合）

---

## 特别要求

1. 所有公开 DTO、应用服务方法、Endpoint 注册方法都必须有中文 XML 注释。
2. 要为前端提供稳定语义：
   - 请求用途
   - 关键参数
   - 返回结构
   - 分页字段
   - 状态字段含义
3. 写操作成功后应联动缓存失效 / 刷新。
4. 权限、限流、审计、日志链路必须接入。
5. **主键 ID 显式生成**：所有创建操作必须在 `InsertAsync` 之前调用 `entity.Id = await DB.GetNextLongIdAsync();`，引入 `using YTStdAdo;`。禁止依赖数据库自动分配 ID。
6. **唯一性校验**：所有包含唯一索引的实体，创建和更新时必须进行唯一性校验；同时提供 `GET /api/{resource}/check-{field}-exists` 接口供前端调用。详见 `tenant-platform-backend-prompt.md` 第 4.2.1 和 4.2.2 节。
7. **i18n 要求**：所有 `ApiResult.Fail(...)` 的 `message` 必须使用 `Messages.XXX` 常量（i18n 键），禁止硬编码中文字符串。`Logger.Debug` 必须使用 `Func<string>` 委托重载。

---

## 本阶段交付物

- 核心业务域 Application Services
- 核心业务域 DTO
- 核心业务域 Endpoint / RouteGroup
- 对应接口测试（核心增删改查、权限拒绝、缓存联动）

---

## 验证要求

至少执行并报告：
- `dotnet build YTStd.slnx`
- 与新增 API 相关的定向测试

---

## 明确禁止

- 不要在本阶段把所有扩展域也一次性塞进来；
- 不要遗漏接口注释与 DTO 注释；
- 不要只实现 Service 不挂接 RouteGroup。
