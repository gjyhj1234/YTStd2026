# 技术栈与框架约束

## 概述

本文件定义项目的技术栈选型和不可变约束，Agent 不得修改这些决策。

---

## 后端技术栈

| 项目 | 选型 | 版本 | 备注 |
|------|------|------|------|
| 运行时 | .NET | 10.0 | NativeAOT 发布 |
| 语言 | C# | 最新 | 与 .NET 10 匹配 |
| 数据库 | PostgreSQL | 最新 | Npgsql 驱动 |
| 发布方式 | NativeAOT | — | 禁止反射、dynamic、LINQ |
| 部署模式 | 单体主程序 | — | 禁止微服务 |
| WebAPI | Minimal API | — | 禁止 MVC Controller |
| 缓存 | Local Cache | — | 禁止 Redis/分布式缓存 |
| 日志 | YTStdLogger | 自研 | 高性能零分配 |
| SQL | YTStdSqlBuilder | 自研 | 编译期生成 |
| ORM | YTStdEntity + YTStdAdo | 自研 | Source Generator 驱动 |
| 国际化 | YTStdI18n | 自研 | 数组索引零分配 |
| 解决方案文件 | YTStd.slnx | — | 所有项目包含在内 |

## 前端技术栈

| 项目 | 选型 | 版本 | 备注 |
|------|------|------|------|
| 框架 | Vue 3 | 最新 | Composition API |
| 语言 | TypeScript | 最新 | 严格模式 |
| 构建 | Vite | 最新 | — |
| UI 组件库 | DevExtreme Vue | 25.2+ | devextreme-vue |
| 路由 | vue-router | 最新 | — |
| 状态管理 | Pinia | 最新 | — |
| HTTP | 原生 fetch 封装 | — | 不引入 axios |
| 国际化 | vue-i18n | 最新 | — |
| Mock | MSW 2.x | — | 开发环境 Mock |

## 硬性禁止

### 后端禁止项

1. 禁止 `System.Reflection`
2. 禁止 `dynamic`
3. 禁止 `System.Linq`
4. 禁止 Expression Tree
5. 禁止 JSON 序列化框架（除 `System.Text.Json` 源生成）
6. 禁止 Redis / 分布式缓存
7. 禁止微服务拆分
8. 禁止 MVC 反射式 Controller
9. 禁止 Entity Framework / Dapper / 其他 ORM
10. 禁止 `ResourceManager` / `.resx` 文件

### 前端禁止项

1. 禁止引入与 DevExtreme 重复定位的 UI 组件库（如 Element Plus、Ant Design Vue）
2. 禁止引入重量级低收益依赖
3. 禁止使用 jQuery

## 自研框架模块

```
src/
├── YTStdSqlBuilder/          # SQL 构建器运行时
├── YTStdSqlBuilder.Generator/ # SQL 构建器源代码生成器
├── YTStdLogger/              # 日志系统
├── YTStdAdo/                 # 数据库访问层
├── YTStdEntity/              # 实体框架
├── YTStdEntity.Generator/    # 实体源代码生成器
├── YTStdI18n/                # 国际化
└── YTStdI18n.Generator/      # 国际化源代码生成器
```

## 构建与测试命令

| 操作 | 命令 |
|------|------|
| 后端构建 | `dotnet build YTStd.slnx` |
| 后端测试 | `dotnet test YTStd.slnx` |
| 前端安装依赖 | `cd web/tenant-platform-web && npm install` |
| 前端构建 | `cd web/tenant-platform-web && npm run build` |
| 前端开发 | `cd web/tenant-platform-web && npm run dev` |
