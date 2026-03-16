# 面向 Claude Opus 4.6 的租户平台后端服务工程实现提示词

## 单体 WebAPI + YTStdEntity 实体驱动 + NativeAOT 高性能实现

你是一名**顶级 .NET 基础架构工程师 / ASP.NET Core NativeAOT 专家 / PostgreSQL & Npgsql 专家 / 高性能框架作者**。
请基于现有仓库中的基础框架，为“租户平台”生成一套**单体主程序架构**的后端服务工程。

这不是 demo，不是玩具项目，而是**可直接落地的生产级平台后端**。
请严格按“**AOT 友好、高性能、低内存、零反射优先、实体驱动建模**”标准完成完整代码、测试与设计说明。

---

## 依赖项目参考

> **重要**：本模块依赖以下已有项目。完整的类型定义、API 签名和使用模式，
> 请查阅 [已有项目参考文档](../../docs/existing-projects-reference.md)。
>
> 租户平台的业务建模来源于以下文档：
> - [租户平台架构说明](../../docs/TenantPlatform/architecture.md)
> - [租户平台实体数据字典](../../docs/TenantPlatform/database_dictionary.md)
>
> AI 在实现时，必须遵循参考文档和业务文档中的约束，不得自行发明与现有框架冲突的实现模式。

### 本模块使用的已有类型

| 类型 | 来源项目 | 用途 |
|------|---------|------|
| `DB` | YTStdAdo | 数据库访问与 DDL / CRUD 执行入口 |
| `EntityAttribute` / `ColumnAttribute` / `IndexAttribute` / `DetailOfAttribute` | YTStdEntity | 实体建模特性 |
| `DbNullable<T>` / `DBNULL` | YTStdEntity | Update 三态可空值 |
| `Logger` | YTStdLogger | 日志记录 |
| `I18n` | YTStdI18n | 国际化错误消息 |

---

## 1. 最终目标

生成一个完整的租户平台后端工程，至少包含：

1. **实体层**：基于 `docs/TenantPlatform/database_dictionary.md` 生成平台实体。
2. **应用层**：围绕平台用户、角色、权限、租户主档、资源、套餐、订阅、账单、通知、日志等模块实现应用服务。
3. **WebAPI 层**：输出适合前端调用的 HTTP API。
4. **权限系统**：平台 RBAC + 菜单/API/操作权限判断。
5. **初始化与运维能力**：应用启动建表、基础数据初始化、缓存刷新、健康检查、审计日志。

---

## 2. 核心架构硬约束

### 2.1 部署模式

- **必须是单体主程序**。
- 所有 WebAPI、权限判断、后台任务、配置刷新、审计记录都在**同一个主程序进程**中运行。
- **禁止**拆分为微服务、禁止引入服务发现、禁止为了“看起来高级”而设计分布式治理组件。
- 如果考虑横向扩展，只允许在入口层增加**负载均衡**，业务代码仍保持单体部署模型。

### 2.2 性能与 AOT 约束

- 目标框架：`net10.0`
- 发布方式：`NativeAOT`
- 数据库：PostgreSQL（Npgsql）
- **禁止**反射驱动控制器发现、禁止 `dynamic`、禁止 Expression Tree、禁止重量级运行时代理。
- WebAPI 优先使用**AOT 友好的 Minimal API / RouteGroup** 方式，不依赖 MVC 反射式 Controller 激活机制。
- 字符串处理、SQL 生成、日志记录遵循现有仓库的低分配模式。

### 2.3 实体建模约束

- 数据表结构由实体维护，而不是手写 SQL 文件。
- **严禁**在租户平台实体中定义裸 `TenantId` 属性或 `tenant_id` 字段。
- 如果需要引用租户主档，必须使用具备业务语义的名字，例如：
  - `TenantRefId`
  - `OwnerTenantId`
  - `SourceTenantId`
  - `TargetTenantId`
- 原因：现有框架会把裸 `TenantId` 识别为租户业务分区字段，从而触发分区表语义；租户平台表不允许这样建模。

### 2.4 缓存与权限约束

- 权限判断必须使用**Local Cache（进程内缓存）**。
- 缓存对象至少包括：
  - 用户-角色关系
  - 角色-权限关系
  - 权限码与菜单树
  - 功能开关 / 基础配置
- **禁止**引入 Redis、分布式缓存、分布式锁作为首选方案。
- 缓存刷新策略采用：**启动预热 + 显式失效 + 定时轻量刷新**。

---

## 3. 项目结构

生成的代码应放置在以下位置：

| 项目 | 路径 | 说明 |
|------|------|------|
| 主项目 | `src/YTStdTenantPlatform/` | 租户平台单体 WebAPI 主程序 |
| 测试项目 | `tests/YTStdTenantPlatform.Tests/` | 单元测试 / 集成测试 |
| 示例或演示 | `samples/YTStdTenantPlatform.Sample/` | 启动示例与初始化演示（可选） |

建议的主项目结构：

```text
src/YTStdTenantPlatform/
├── Program.cs
├── Bootstrap/
│   ├── ServiceRegistration.cs
│   ├── RouteRegistration.cs
│   └── StartupInitialization.cs
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── ValueObjects/
│   └── Descriptors/
├── Application/
│   ├── Services/
│   ├── Commands/
│   ├── Queries/
│   └── Dtos/
├── Infrastructure/
│   ├── Auth/
│   ├── Cache/
│   ├── Scheduling/
│   ├── Persistence/
│   └── Initialization/
└── Endpoints/
    ├── PlatformAdminEndpoints.cs
    ├── TenantLifecycleEndpoints.cs
    ├── TenantInfoEndpoints.cs
    ├── PackageEndpoints.cs
    └── BillingEndpoints.cs
```

---

## 4. 模块设计要求

### 4.1 实体来源

- 所有实体以 `docs/TenantPlatform/database_dictionary.md` 为唯一业务数据来源。
- 所有菜单、权限、模块划分以 `docs/TenantPlatform/architecture.md` 为唯一功能来源。
- 生成实体时必须输出：
  - 实体类
  - 枚举
  - 索引声明
  - 描述类 / 元数据
  - 必要的输入输出 DTO

### 4.2 WebAPI 设计

- API 必须按模块分组，例如：
  - `/api/platform-users`
  - `/api/platform-roles`
  - `/api/platform-permissions`
  - `/api/tenants`
  - `/api/tenant-subscriptions`
  - `/api/billing-invoices`
- 必须覆盖：列表、详情、新增、编辑、启用/禁用、授权、状态流转、审计查询等操作。
- 所有接口都要考虑分页、筛选、状态过滤、审计信息返回。

### 4.3 权限与 Local Cache

至少设计以下缓存：

1. `PermissionSnapshotCache`
2. `RolePermissionCache`
3. `UserRoleCache`
4. `FeatureFlagCache`
5. `TenantPlatformConfigCache`

缓存设计要求：
- 进程内只读快照结构优先。
- 刷新时构建新快照，再原子替换旧引用。
- 权限判断路径避免频繁数据库查询。
- 写操作成功后，必须触发对应缓存失效或局部刷新。

### 4.4 后台任务

后台任务与异步处理必须仍在单体主程序中实现，例如：
- 租户初始化任务
- 到期提醒任务
- 账单生成任务
- Webhook 投递重试任务

禁止把这些任务拆分为独立微服务。

---

## 5. 初始化要求

- 应用启动时调用实体生成的建表/索引维护能力。
- 基础数据初始化逻辑必须与实体模型配套。
- 初始化数据的生成与代码结构，请继续参考独立提示词：
  - `tenant-platform-initdata-prompt.md`

---

## 6. 日志与安全要求

- 所有公开 API 和应用服务要接入 `Logger.Debug / Info / Error / Fatal`。
- 用户可见错误信息应支持 `I18n`。
- 密码字段只保存摘要，不保存明文。
- API Key、Webhook Secret、MFA 等敏感字段必须使用摘要或密文。
- 权限相关接口必须输出审计日志。

---

## 7. 最终质量标准

若设计上有取舍，请遵循以下优先级：

**正确性 > 与现有框架一致 > 安全性 > AOT 兼容 > 部署简单 > 性能 > 内存占用 > 可维护性 > 可读性 > 技巧炫技**

---

## 8. 最终指令

请现在开始实现，直接交付：

- 完整源码
- 完整测试
- 完整设计说明
- 实体建模说明
- Local Cache 设计说明
- 路由清单

不要只给思路。
不要输出微服务方案。
不要引入分布式缓存。
必须保持**单体主程序 + 实体驱动 + Local Cache 权限判断**。
