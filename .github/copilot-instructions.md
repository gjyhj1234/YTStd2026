# Copilot Instructions

## 提示词体系入口

本仓库使用 `.ai/` 目录下的结构化提示词体系指导 AI 开发。

### 首次执行任务前必须阅读

1. `.ai/README.md` — 提示词体系总览与项目整体结构说明
2. `.ai/system/agent-contract.md` — Agent 协作契约
3. `.ai/rules/global.md` — 全局开发规范
4. `.ai/context/tech-stack.md` — 技术栈约束

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
- **编码完成后必须执行** `.ai/system/self-review-protocol.md` 中定义的自动化代码审查

### 底层框架维护

- 底层工程（YTStdSqlBuilder、YTStdLogger、YTStdAdo、YTStdEntity、YTStdI18n）的代码审查与优化提示词位于 `.ai/prompts/11-base-library/`
- 这些底层工程除非有独立的明确调整任务提示词，否则禁止修改其代码
- 调整底层工程的提示词同样必须放置在 `.ai/prompts/11-base-library/` 目录下

---

## ⚠️ 关键编码约束（内联摘要 — 必须遵守）

以下规则从 `.ai/rules/global.md` 和 `.ai/rules/backend.md` 中提取，**在每次编码任务中必须严格执行**。之所以在此内联，是因为这些规则在历史任务中曾被遗漏，造成代码质量问题。

### 1. InsertAsync 前必须获取 ID（零容忍）

```csharp
// ✅ 正确 — 每个 InsertAsync 之前必须有 GetNextLongIdAsync
entity.Id = await DB.GetNextLongIdAsync();
await DB.InsertAsync(entity);

// ❌ 错误 — 禁止在未设置 Id 的情况下调用 InsertAsync
await DB.InsertAsync(entity); // entity.Id 为 0，严重错误
```

**此规则无例外。所有实体（包括关联表、中间表、日志表）在 InsertAsync 之前必须调用 GetNextLongIdAsync。**

### 2. ApiResult.Fail() 仅传 ErrorCodes 常量

```csharp
// ✅ 正确
return ApiResult.Fail(ErrorCodes.UsernameExists);

// ❌ 错误 — 禁止传字符串消息
return ApiResult.Fail(ErrorCodes.UsernameExists, "用户名已存在");
```

### 3. Logger.Debug 必须使用 Func<string> 延迟求值

```csharp
// ✅ 正确
Logger.Debug(tenantId, userId, () => $"[方法名] 参数: {param}");

// ❌ 错误
Logger.Debug(tenantId, userId, $"[方法名] 参数: {param}");
```

### 4. 禁止使用反射、dynamic、System.Linq

### 5. Postman 集合必须与实际端点一致

Postman 集合中的每个请求的 HTTP 方法和路径，必须在对应的 Endpoints 代码中有精确匹配的 `MapGet`/`MapPost`/`MapPut`/`MapDelete` 注册。不允许 Postman 中存在代码中不存在的路由。

---

## ⚠️ 强制代码审查（编译通过 ≠ 任务完成）

**编译和测试通过不是最终验收标准。** Agent 在标记任务完成之前，必须执行 `.ai/system/self-review-protocol.md` 中定义的代码搜索验证，确保所有编码约束被严格遵守。

验收闭环：**分析 → 计划 → 实现 → 编译 → 代码搜索审查 → 修复违规 → 再次编译 → 收尾**