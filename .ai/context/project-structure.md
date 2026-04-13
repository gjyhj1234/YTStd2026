# 项目结构说明

## 概述

本文件描述仓库的目录结构和各项目的位置，Agent 在创建或修改文件时必须遵循此结构。

---

## 仓库根目录

```
YTStd2026/
├── .ai/                          # AI 提示词体系（本目录）
├── .github/                      # GitHub 配置
│   ├── copilot-instructions.md   # Copilot 全局指令（指向 .ai/ 体系）
│   └── workflows/                # CI/CD 工作流
├── docs/                         # 项目文档
│   ├── architecture.md           # 仓库架构说明
│   ├── existing-projects-reference.md # 已有项目 API 参考
│   ├── reconstruction/           # 重建文档
│   ├── standards/                # 标准文档
│   └── TenantPlatform/           # 租户平台文档
│       ├── architecture.md       # 租户平台架构
│       ├── database_dictionary.md # 数据字典
│       ├── API.md                # API 文档
│       └── ...
├── src/                          # 源代码
│   ├── YTStdSqlBuilder/         # SQL 构建器
│   ├── YTStdSqlBuilder.Generator/ # SQL 源代码生成器
│   ├── YTStdLogger/             # 日志系统
│   ├── YTStdAdo/                # 数据库访问层
│   ├── YTStdEntity/             # 实体框架
│   ├── YTStdEntity.Generator/   # 实体源代码生成器
│   ├── YTStdI18n/               # 国际化
│   ├── YTStdI18n.Generator/     # 国际化源代码生成器
│   └── YTStdTenantPlatform/     # 租户平台主程序
│       ├── Program.cs
│       ├── Bootstrap/           # 启动引导
│       ├── Application/         # 应用层
│       │   ├── Constants/       # 常量（ErrorCodes）
│       │   ├── Dtos/            # 数据传输对象
│       │   └── Services/        # 应用服务
│       ├── Domain/              # 领域层
│       │   ├── Enums/           # 枚举
│       │   └── ValueObjects/    # 值对象
│       ├── Endpoints/           # API 端点
│       ├── Infrastructure/      # 基础设施
│       │   ├── Auth/            # 认证
│       │   ├── Cache/           # 缓存
│       │   ├── Initialization/  # 初始化
│       │   ├── Middleware/      # 中间件
│       │   ├── Scheduling/      # 后台任务
│       │   └── Serialization/   # JSON 序列化
│       └── entity/              # 实体
│           └── TenantPlatform/  # 租户平台实体
│   └── WebTenantPlatfrom/       # 租户平台前端（新，当前活跃开发）
│       ├── src/
│       │   ├── main.ts          # 应用入口
│       │   ├── App.vue          # 根组件
│       │   ├── api/             # API 封装（axios）
│       │   │   ├── http.ts      # axios 实例与拦截器
│       │   │   ├── http.types.ts # HTTP 类型定义
│       │   │   └── {module}.ts  # 业务模块 API
│       │   ├── assets/          # 静态资源
│       │   ├── components/      # 通用组件
│       │   │   ├── FunctionDescriptionCard.vue
│       │   │   └── OperationGuideDrawer.vue
│       │   ├── composables/     # 组合式函数
│       │   │   └── useNotify.ts # 通知与确认
│       │   ├── constants/       # 常量
│       │   │   └── permissions.ts # 权限码
│       │   ├── layouts/         # 布局
│       │   │   ├── MainLayout.vue
│       │   │   └── side-nav-outer-toolbar/ # DevExtreme Application Template 布局
│       │   ├── locales/         # 国际化资源
│       │   │   ├── index.ts     # i18n 初始化 + import.meta.glob
│       │   │   ├── common/      # 全局复用翻译
│       │   │   ├── enums/       # 枚举翻译
│       │   │   ├── generated/   # Generator 生成
│       │   │   ├── zh-CN.json   # 主语言文件
│       │   │   ├── en-US.json
│       │   │   ├── ja-JP.json
│       │   │   ├── ms-MY.json
│       │   │   └── zh-TW.json
│       │   ├── router/          # 路由
│       │   │   └── index.ts     # 路由配置 + 权限守卫
│       │   ├── store/           # 状态管理（Pinia）
│       │   │   ├── auth.ts      # 认证状态
│       │   │   └── app.ts       # 应用状态
│       │   ├── types/           # TypeScript 类型
│       │   │   └── {module}.ts  # 业务模块 DTO 类型
│       │   ├── utils/           # 工具函数
│       │   │   └── theme-service.ts # 主题服务
│       │   └── views/           # 页面视图
│       │       ├── login/       # 登录页
│       │       ├── dashboard/   # 仪表盘
│       │       ├── platform-users/    # 用户管理
│       │       ├── platform-roles/    # 角色管理
│       │       ├── platform-permissions/ # 权限管理
│       │       ├── tenants/           # 租户管理
│       │       ├── tenant-info/       # 租户信息
│       │       ├── tenant-resources/  # 租户资源
│       │       ├── tenant-config/     # 租户配置
│       │       ├── packages/          # 套餐管理
│       │       ├── subscriptions/     # 订阅管理
│       │       ├── billing/           # 账单管理
│       │       ├── api-integration/   # API 集成
│       │       ├── audit/             # 审计日志
│       │       ├── notifications/     # 通知管理
│       │       ├── storage/           # 文件管理
│       │       └── platform-operations/ # 平台运营
│       ├── public/
│       ├── package.json
│       ├── vite.config.ts
│       └── tsconfig.json
├── tests/                        # 测试项目
│   ├── YTStdSqlBuilder.Tests/
│   ├── YTStdSqlBuilder.Generator.Tests/
│   ├── YTStdLogger.Tests/
│   ├── YTStdAdo.Tests/
│   ├── YTStdEntity.Tests/
│   ├── YTStdI18n.Tests/
│   └── YTStdTenantPlatform.Tests/
├── benchmarks/                   # 性能基准测试
├── samples/                      # 示例项目
├── web/                          # 前端项目（旧，已冻结，仅供参考和回滚）
│   └── tenant-platform-web/     # 租户平台前端（旧）
│       ├── src/
│       │   ├── main.ts
│       │   ├── App.vue
│       │   ├── api/             # API 封装
│       │   ├── assets/          # 静态资源
│       │   ├── components/      # 通用组件
│       │   ├── composables/     # 组合式函数
│       │   ├── constants/       # 常量
│       │   ├── layouts/         # 布局
│       │   ├── locales/         # 国际化资源
│       │   ├── mocks/           # Mock 数据
│       │   ├── router/          # 路由
│       │   ├── store/           # 状态管理
│       │   ├── types/           # TypeScript 类型
│       │   ├── utils/           # 工具函数
│       │   └── views/           # 页面视图
│       ├── public/
│       └── package.json
└── YTStd.slnx                   # 解决方案文件
```

---

## 文件放置规则

### 后端

| 文件类型 | 目录 | 命名规范 |
|---------|------|---------|
| 实体 | `src/{Project}/entity/{Module}/*.cs` | PascalCase，与表名对应 |
| 枚举 | `src/{Project}/Domain/Enums/*.cs` | PascalCase + 后缀 Enum 可选 |
| DTO | `src/{Project}/Application/Dtos/{Module}/*.cs` | `{Business}ReqDTO` / `{Business}RepDTO` |
| 应用服务 | `src/{Project}/Application/Services/*.cs` | `{Module}AppService.cs` |
| 端点 | `src/{Project}/Endpoints/*.cs` | `{Module}Endpoints.cs` |
| 中间件 | `src/{Project}/Infrastructure/Middleware/*.cs` | `{Name}Middleware.cs` |
| 缓存 | `src/{Project}/Infrastructure/Cache/*.cs` | `{Name}Cache.cs` |
| 初始化 | `src/{Project}/Infrastructure/Initialization/*.cs` | 按功能命名 |
| 常量 | `src/{Project}/Application/Constants/*.cs` | `ErrorCodes.cs` |
| 测试 | `tests/{Project}.Tests/*.cs` | `{Module}Tests.cs` |

### 前端（新项目：`src/WebTenantPlatfrom`）

> 注意：路径名称中 `WebTenantPlatfrom` 为项目约定拼写，不得自行修正为 `WebTenantPlatform`。

| 文件类型 | 目录 | 命名规范 |
|---------|------|---------|
| 页面 | `src/WebTenantPlatfrom/src/views/{module}/` | `{Name}View.vue` |
| API | `src/WebTenantPlatfrom/src/api/{module}.ts` | camelCase |
| 类型 | `src/WebTenantPlatfrom/src/types/{module}.ts` | camelCase |
| 组件 | `src/WebTenantPlatfrom/src/components/{Name}.vue` | PascalCase |
| 路由 | `src/WebTenantPlatfrom/src/router/index.ts` | — |
| 状态 | `src/WebTenantPlatfrom/src/store/{module}.ts` | camelCase |
| 工具 | `src/WebTenantPlatfrom/src/utils/{name}.ts` | camelCase |
| 国际化 | `src/WebTenantPlatfrom/src/locales/` | generated/ + common/ + enums/ + 主语言文件 + 组件级 `.vue.{locale}.json` |
| 语言文件 | 与 `.vue` 同目录 | `{Name}View.vue.{locale}.json`（zh-CN / en-US / ja-JP / ms-MY / zh-TW） |

### 前端（旧项目：`web/tenant-platform-web`，已冻结）

> 旧项目仅供参考和回滚，不再新增功能。

| 文件类型 | 目录 | 命名规范 |
|---------|------|---------|
| 页面 | `web/tenant-platform-web/src/views/{module}/` | `{Name}View.vue` |
| API | `web/tenant-platform-web/src/api/{module}.ts` | camelCase |
| 类型 | `web/tenant-platform-web/src/types/{module}.ts` | camelCase |
| 组件 | `web/tenant-platform-web/src/components/{Name}.vue` | PascalCase |
| 路由 | `web/tenant-platform-web/src/router/index.ts` | — |
| 状态 | `web/tenant-platform-web/src/store/{module}.ts` | camelCase |
| 工具 | `web/tenant-platform-web/src/utils/{name}.ts` | camelCase |
| 国际化 | `web/tenant-platform-web/src/locales/` | generated/ + common/ + runtime/ + 组件级 `.vue.{locale}.json` |
