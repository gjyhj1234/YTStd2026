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
- `.ai/prompts/03-frontend/09-production-defaults.md` — 前端生产级默认能力补全
- `.ai/prompts/03-frontend/10-component-asset-catalog.md` — 前端组件资产目录与复用决策
- `.ai/prompts/03-frontend/11-delivery-workflow.md` — 前端任务快照、缓存与交付流程
- `.ai/prompts/03-frontend/12-github-automation-workflow.md` — GitHub 顺序执行 workflow
- `.ai/prompts/03-frontend/13-prompt-evolution-workflow.md` — 前端缺陷沉淀与提示词迭代

### 任务执行

- 使用 `.ai/templates/task-template.md` 定义任务
- 前端 slice / GitHub issue 优先使用 `.ai/templates/frontend-slice-task-template.md`
- 前端任务快照与缓存优先使用 `.ai/templates/frontend-task-cache-template.md`
- 按 `.ai/system/execution-policy.md` 执行
- 按 `.ai/system/session-handoff.md` 续接
- **编码完成后必须执行** `.ai/system/self-review-protocol.md` 中定义的自动化代码审查
- **前端编码完成后必须执行** `.ai/system/e2e-testing-workflow.md` 中定义的 Playwright E2E 测试迭代
- **每次迭代结束后必须执行** `.ai/system/readme-sync-protocol.md` 中定义的 README 同步检查
- **前端每轮结束后必须更新** `.ai/tasks/...` 的任务快照与 `.ai/workspace/session-summary-*.md`

### E2E 测试（前端任务必读）

- `.ai/system/e2e-testing-workflow.md` — E2E 测试工作流协议（环境检查、迭代修复闭环）
- `.ai/prompts/03-frontend/08-playwright-e2e.md` — Playwright 测试规范
- `.ai/prompts/08-platform/frontend/0040_e2e-testing-protocol.md` — 各模块测试要点
- **环境已预配置**：`copilot-setup-steps.yml` 已配置 PostgreSQL 服务、.NET 10、Node.js、Playwright
- **每次前端会话启动时**：必须按 e2e-testing-workflow.md 第一节执行环境预检

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

**官方 dxdocs 工作流（必须严格遵循）：**

1. **调用 `devexpress_docs_search`** 获取与问题相关的帮助主题列表（每个问题仅调用一次，避免冗余查询）
2. **调用 `devexpress_docs_get_content`** 获取并阅读最相关的帮助主题全文
3. **反思获取到的内容**，思考其与当前编码需求的关系
4. **基于检索到的信息编码**，而非凭记忆或猜测

**dxdocs 使用约束：**

- 每个问题仅调用一次 `devexpress_docs_search`，避免冗余查询
- **必须基于从 dxdocs 获取的信息编码**，禁止凭猜测或过时记忆
- 如果文档中有相关代码示例，**必须参考这些代码示例**
- 必须引用文档中提到的**具体 DevExtreme 控件和属性名称**
- 调用时使用 `technologies: ["Vue"]` 限定 Vue 相关文档

```
// 查阅示例
devexpress_docs_search(technologies: ["Vue"], question: "DxForm label-mode floating static browser autofill difference")
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeView item selection selectByClick focusStateEnabled CSS style control")
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid CustomStore remote paging load function skip take")
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

**组件特有的翻译 key 必须放在组件级语言文件中**，禁止放在主语言文件 `src/locales/{locale}.json` 中。`locales/index.ts` 使用 `import.meta.glob` 自动加载组件级文件并以最高优先级合并。

### 12. 侧边栏 DxTreeView 必须禁止选中态样式偏移

DxTreeView 用于侧边栏时，必须确保点击子菜单后不出现靠左对齐偏移。需使用 dxdocs 查阅 DxTreeView 的 `selectByClick`、`focusStateEnabled`、CSS 覆盖方案。

### 13. 组件级语言文件自动加载（import.meta.glob）

`src/locales/index.ts` 使用 Vite 的 `import.meta.glob` 自动收集 `views/**`、`components/**`、`layouts/**` 下的 `*.vue.{locale}.json` 文件。非 null 值以最高优先级合并到 vue-i18n 全局消息中。

```typescript
// ✅ 正确 — 修改组件级语言文件后界面直接生效
// PlatformRolesView.vue.zh-CN.json 中: "平台角色管理": "平台角色管理"
// 由 import.meta.glob 自动加载，优先级最高

// ❌ 错误 — 将组件特有 key 放在主语言文件中
// src/locales/zh-CN.json 中: "平台角色管理": "平台角色管理" ← 禁止
```

---

## ⚠️ 强制代码审查（编译通过 ≠ 任务完成）

**编译和测试通过不是最终验收标准。** Agent 在标记任务完成之前，必须执行 `.ai/system/self-review-protocol.md` 中定义的代码搜索验证，确保所有编码约束被严格遵守。

验收闭环：**分析 → 计划 → 实现 → 编译 → E2E 测试迭代 → 代码搜索审查 → 修复违规 → 再次编译 → 收尾**

---

## ⚠️ 前端 E2E 测试（前端任务零容忍）

**前端页面开发完成后，必须编写 Playwright E2E 测试并迭代通过。** 仅通过编译不代表页面可用（历史问题：编译通过但页面空白）。

### 14. 每个前端模块必须有对应的 E2E 测试

```
# 前端模块与 E2E 测试文件的对应关系
F1-2 登录页    → e2e/tests/login/login.noauth.spec.ts
F2-1 仪表盘    → e2e/tests/dashboard/dashboard.spec.ts
F2-2 平台用户  → e2e/tests/platform-users/platform-users.spec.ts
... 以此类推，每个模块一个测试文件
```

### 15. 前端会话启动必须执行环境预检

```bash
# 必须按顺序检查：
# 1. PostgreSQL 连接
PGPASSWORD=gjwq1234 psql -h localhost -U postgres -d test1 -c "SELECT 1;"
# 2. 后端 health check
curl -s http://127.0.0.1:5000/api/health/
# 3. Playwright 浏览器
npx playwright --version
# 4. 前端 dev server
curl -s http://localhost:5173/
```

### 16. E2E 测试迭代（最多 5 次）

```
编码 → 写测试 → 运行测试 → 失败 → 分析原因 → 修复 → 再运行 → ... → 全部通过
```

**不允许删除测试用例来"通过"测试。必须修复前端代码或测试逻辑。**

### 17. 前端任务必须填写需求补全矩阵（零容忍）

业务提示词和执行中的 slice 必须显式写出：显式需求、生产默认补全、待确认项、明确不做项。**禁止**因为用户没写某项就默认不实现。

### 18. 前端任务必须先做组件复用决策

实现页面前，必须先确认是否复用现有共享资产；同类交互出现第 2 次时，必须评估提升为共享资产。**禁止**在多个模块重复复制搜索区、操作列、状态标签、表单壳。

### 19. 使用 DxDataGrid / DxForm 时必须写能力矩阵

必须显式声明排序、列宽、列隐藏、固定列、分页、页大小、焦点行、唯一性校验、提交 loading 等能力是启用、禁用还是不适用。**禁止**只写“使用 DxDataGrid / DxForm 实现”。

### 20. 权限测试必须覆盖入口可见性

不能只测按钮显隐。必须显式验证：菜单入口是否可见、URL 是否可达、无权限时是否显示 403/无权限提示。**禁止**出现“有查询权限但入口整体消失”的情况。

### 21. E2E 结果必须提供证据，不得口头宣称“已测试”

前端任务结束时必须给出：实际运行命令、测试文件、通过/失败数量、覆盖矩阵。**未运行命令 = 未测试。**
