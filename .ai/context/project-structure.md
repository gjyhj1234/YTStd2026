# 项目结构说明

## 概述

本文件描述仓库的目录结构和各项目的位置，Agent 在创建或修改文件时必须遵循此结构。

---

## 仓库根目录

```
YTStd2026/
├── .ai/                          # AI 提示词体系（本目录）
├── .github/                      # GitHub 配置
│   ├── copilot-instructions.md   # Copilot 全局指令
│   ├── prompts/                  # 旧提示词（历史参考）
│   └── workflows/                # CI/CD 工作流
├── docs/                         # 项目文档
│   ├── architecture.md           # 仓库架构说明
│   ├── existing-projects-reference.md # 已有项目 API 参考
│   ├── prompt-authoring-guide.md # 提示词编写指南（旧）
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
│       │   ├── Constants/       # 常量（ErrorCodes, Messages）
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
├── web/                          # 前端项目
│   └── tenant-platform-web/     # 租户平台前端
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
| 常量 | `src/{Project}/Application/Constants/*.cs` | `ErrorCodes.cs`, `Messages.cs` |
| 测试 | `tests/{Project}.Tests/*.cs` | `{Module}Tests.cs` |

### 前端

| 文件类型 | 目录 | 命名规范 |
|---------|------|---------|
| 页面 | `src/views/{module}/` | `{Name}View.vue` |
| API | `src/api/{module}.ts` | camelCase |
| 类型 | `src/types/{module}.ts` | camelCase |
| 组件 | `src/components/{Name}.vue` | PascalCase |
| 路由 | `src/router/index.ts` | — |
| 状态 | `src/store/{module}.ts` | camelCase |
| 工具 | `src/utils/{name}.ts` | camelCase |
| 国际化 | `src/locales/{locale}.json` | 如 `zh-CN.json` |
