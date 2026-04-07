# Copilot Instructions

## 提示词体系入口

本仓库使用 `.ai/` 目录下的结构化提示词体系指导 AI 开发。

### 首次执行任务前必须阅读

1. `.ai/system/agent-contract.md` — Agent 协作契约
2. `.ai/rules/global.md` — 全局开发规范
3. `.ai/context/tech-stack.md` — 技术栈约束

### 按需阅读

- `.ai/rules/backend.md` — 后端开发规范
- `.ai/rules/frontend.md` — 前端开发规范
- `.ai/rules/database.md` — 数据库命名规范
- `.ai/rules/api-design.md` — API 设计规范
- `.ai/rules/naming.md` — 命名规范总则
- `.ai/rules/i18n.md` — 国际化规范
- `.ai/rules/generator.md` — 源代码生成器规范
- `.ai/rules/testing.md` — 测试规范
- `.ai/rules/postman.md` — Postman 测试规范
- `.ai/rules/security.md` — 安全规范

### 任务执行

- 使用 `.ai/templates/task-template.md` 定义任务
- 按 `.ai/system/execution-policy.md` 执行
- 按 `.ai/system/session-handoff.md` 续接

---

## Project Guidelines

- 在该仓库中，主键 ID 不能由 CRUD 生成器自动分配；事务插入常依赖主表 ID，业务代码必须显式获取并设置 ID。
- 在该仓库中，优先考虑正确性。代码必须保持 AOT 友好，适用于 .NET 10 NativeAOT，且不使用反射、动态或 LINQ；优化以实现高性能和低 GC；仅针对 PostgreSQL/Npgsql；生成与现有实体和逻辑一致的生产级可编译代码。
- 在该仓库中，`Logger.Debug` 必须仅使用委托重载，绝不使用直接字符串参数或字符串插值/连接。优先传递一个 lambda，并尽量减少字符串连接，以避免在 Debug 被禁用时的内存分配。
- 在该仓库中，手动 JSON 字符串连接应替换为基于 `Utf8JsonWriter` 的构造，以实现 AOT 友好的低分配 JSON 生成。对于 AOT 友好的 JSON，优先使用 `JsonSerializerContext` 和 `JsonSerializable` 源生成，而不是手动元数据注册，以保持代码简洁和可读。
- 在该仓库中，所有 `ApiResult.Fail()` 的消息参数必须使用 `Messages.XXX` 常量（i18n key），禁止硬编码中文字符串。
- 在该仓库中，所有实体中引用租户的字段禁止使用裸 `TenantId` / `tenant_id`，必须使用语义化名称（如 `TenantRefId`、`OwnerTenantId`）。
- 在该仓库中，中间件错误响应必须使用 PascalCase JSON 属性名（`Code`、`Message`、`Data`），与 `ApiResult` DTO 保持一致。
- 在该仓库中，CRUD 结果（`DbInsResult`、`DbUdqResult`）使用 `.Success` (bool) 属性判断成功，只有 `ApiResult` 使用 `.Code` 属性。