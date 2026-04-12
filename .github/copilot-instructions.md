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

### 6. Create/Save 方法必须遵循唯一性双重校验模式（零容忍）

所有包含唯一索引字段的实体，在 `InsertAsync` 前**必须**进行唯一性前置校验；同时在 `InsertAsync` 失败时**必须**进行唯一性后置复核，返回精确的唯一性冲突错误码（`ErrorCodes.XxxExists`），而非笼统的创建失败错误码（`ErrorCodes.XxxCreateFailed`）。

```csharp
// ✅ 正确 — 唯一性双重校验模式
// 第一重：前置校验
var (chkResult, existing) = await XxxCRUD.GetListAsync(tenantId, operatorId);
if (chkResult.Success && existing != null)
{
    foreach (var item in existing)
    {
        if (string.Equals(item.Code, req.Code.Trim(), StringComparison.OrdinalIgnoreCase))
            return ApiResult<long>.Fail(ErrorCodes.XxxCodeExists);
    }
}

// ... 构建实体并设置 Id ...

// 第二重：后置复核（防止并发竞争导致唯一索引冲突时返回笼统错误）
var insResult = await XxxCRUD.InsertAsync(tenantId, operatorId, entity);
if (!insResult.Success)
{
    // InsertAsync 失败时重新检查唯一性，若冲突则返回精确错误码
    var (rechkResult, rechkData) = await XxxCRUD.GetListAsync(tenantId, operatorId);
    if (rechkResult.Success && rechkData != null)
    {
        foreach (var item in rechkData)
        {
            if (string.Equals(item.Code, entity.Code, StringComparison.OrdinalIgnoreCase))
                return ApiResult<long>.Fail(ErrorCodes.XxxCodeExists);
        }
    }
    return ApiResult<long>.Fail(ErrorCodes.XxxCreateFailed);
}

// ❌ 错误 — 缺少前置校验
var insResult = await XxxCRUD.InsertAsync(tenantId, operatorId, entity);
if (!insResult.Success)
    return ApiResult<long>.Fail(ErrorCodes.XxxCreateFailed); // 用户无法知道是重复还是其他原因

// ❌ 错误 — 有前置校验但无后置复核
// 前置校验通过后，在并发场景下另一请求可能已插入相同值
// InsertAsync 因唯一索引失败，但返回笼统的 CreateFailed 错误码
```

**此规则适用于所有包含唯一索引的实体的 Create/Save 方法。每个唯一字段必须有对应的 `ErrorCodes.XxxExists` 错误码。**

---

## ⚠️ 前端关键编码约束（内联摘要 — 前端任务必须遵守）

以下规则从 `.ai/rules/frontend.md` 和 `.ai/rules/i18n.md` 中提取，**在每次前端编码任务中必须严格执行**。之所以在此内联，是因为这些规则在多轮前端重构中反复被遗漏。

### 7. 前端开发必须使用 DevExpress MCP Server（dxdocs）

本仓库已配置 DevExpress MCP Server（工具名：`dxdocs`），提供 `devexpress_docs_search` 和 `devexpress_docs_get_content` 两个工具。

**强制调用场景**（必须先查阅文档再编码）：
1. 首次使用任何 DevExtreme 组件时（DxDataGrid、DxTreeView、DxForm、DxDrawer、DxPopup 等）
2. 遇到 DevExtreme 组件行为异常时（如 label 重叠、样式错位、事件不触发）
3. 配置 DevExtreme 组件的高级功能时（远程分页 CustomStore、表单验证 validationRules、树形选择等）
4. 处理 DevExtreme 组件的样式定制时（CSS 变量、主题切换、DxDrawer 布局）

```
// 查阅示例
devexpress_docs_search(technologies: ["Vue"], question: "DxForm label-mode floating static difference")
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeView item selection CSS style control")
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid CustomStore remote paging")
```

**禁止在不查阅 dxdocs 文档的情况下猜测 DevExtreme 组件的 API 和行为。**

### 8. 登录页 DxForm label-mode 必须使用 static

```vue
// ✅ 正确 — 避免浏览器自动填充与 floating label 重叠
<DxForm label-mode="static">

// ❌ 错误 — 浏览器自动填充时 floating label 与值重叠（DevExtreme 已知行为）
<DxForm label-mode="floating">
```

### 9. DxColumn caption 必须使用 $t() 绑定

```vue
// ✅ 正确
<DxColumn data-field="Code" :caption="$t('角色编码')" />

// ❌ 错误 — 硬编码 caption 不随语言切换
<DxColumn data-field="Code" caption="角色编码" />
```

### 10. notifySuccess / confirmAction 仅传 i18n key（不双重 t()）

```typescript
// ✅ 正确 — useNotify 内部会调用 t() 翻译
notifySuccess('创建成功')
confirmAction('确认启用此角色')

// ❌ 错误 — 双重 t() 调用
notifySuccess(t('创建成功'))
confirmAction(t('确认启用此角色'))
```

### 11. 每个 .vue 文件必须有 5 个对应语言文件

```
ComponentName.vue
ComponentName.vue.zh-CN.json  ← 必须
ComponentName.vue.en-US.json  ← 必须
ComponentName.vue.ja-JP.json  ← 必须
ComponentName.vue.ms-MY.json  ← 必须
ComponentName.vue.zh-TW.json  ← 必须
```

### 12. 侧边栏 DxTreeView 必须禁止选中态样式偏移

DxTreeView 用于侧边栏时，必须确保点击子菜单后不出现靠左对齐偏移。需使用 dxdocs 查阅 DxTreeView 的 `selectByClick`、`focusStateEnabled`、CSS 覆盖方案。

---

## ⚠️ 强制代码审查（编译通过 ≠ 任务完成）

**编译和测试通过不是最终验收标准。** Agent 在标记任务完成之前，必须执行 `.ai/system/self-review-protocol.md` 中定义的代码搜索验证，确保所有编码约束被严格遵守。

验收闭环：**分析 → 计划 → 实现 → 编译 → 代码搜索审查 → 修复违规 → 再次编译 → 收尾**