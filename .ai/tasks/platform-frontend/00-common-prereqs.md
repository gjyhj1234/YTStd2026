# 通用前置阅读与环境约束

> **每个子任务执行前必须阅读本文件。**
> 本文件提取了所有子任务共用的前置阅读、环境约束、编码规则和验收流程。

---

## 一、必读文件清单

### 1.1 系统级（每次会话必读）

| 序号 | 文件 | 用途 |
| :----: | ------ | ------ |
| 1 | `.github/copilot-instructions.md` | 关键编码约束（第 7-16 条为前端约束） |
| 2 | `.ai/system/agent-contract.md` | Agent 协作契约 |
| 3 | `.ai/system/e2e-testing-workflow.md` | E2E 测试工作流 |
| 4 | `.ai/system/self-review-protocol.md` | 自动化代码审查协议 |

### 1.2 前端规范（每次会话必读）

| 序号 | 文件 | 用途 |
| :----: | ------ | ------ |
| 1 | `.ai/prompts/03-frontend/00-governance.md` | 前端总治理 |
| 2 | `.ai/prompts/03-frontend/04-devextreme-templates.md` | DevExtreme 模板与组件规范 |
| 3 | `.ai/prompts/03-frontend/05-axios-standard.md` | axios 标准化实现规范 |
| 4 | `.ai/prompts/03-frontend/06-i18n-execution.md` | 前端国际化执行规范 |
| 5 | `.ai/prompts/03-frontend/03-anti-patterns.md` | 反模式清单 |
| 6 | `.ai/prompts/03-frontend/08-playwright-e2e.md` | Playwright E2E 测试规范 |
| 7 | `.ai/prompts/03-frontend/09-production-defaults.md` | 生产级默认能力补全 |
| 8 | `.ai/prompts/03-frontend/10-component-asset-catalog.md` | 组件资产目录与复用决策 |
| 9 | `.ai/prompts/03-frontend/11-delivery-workflow.md` | 任务快照与交付流程 |
| 10 | `.ai/prompts/03-frontend/12-github-automation-workflow.md` | GitHub 顺序执行 workflow |
| 11 | `.ai/prompts/03-frontend/13-prompt-evolution-workflow.md` | 缺陷沉淀与提示词迭代 |
| 12 | `.ai/rules/frontend.md` | 前端开发规范 |
| 13 | `.ai/rules/i18n.md` | 国际化规范 |

### 1.3 业务总览（每次会话必读）

| 序号 | 文件 | 用途 |
| :----: | ------ | ------ |
| 1 | `.ai/prompts/08-platform/frontend/0000_overview.md` | 前端总览与模块清单 |
| 2 | `.ai/prompts/08-platform/frontend/0040_e2e-testing-protocol.md` | 各模块 E2E 测试要点 |
| 3 | `.ai/prompts/08-platform/frontend/0050_common-components-standard.md` | 通用组件与交互规范（功能说明/操作指引/搜索区域/操作列溢出/国际化/行高亮/密码展示） |

### 1.4 子任务特定（见各子任务文件 "前置阅读" 节）

每个子任务文件会额外指定需要阅读的后端 API 提示词。

---

## 二、项目环境

| 属性 | 值 |
| ------ | --- |
| 前端项目路径 | `src/WebTenantPlatfrom` |
| 技术栈 | Vue 3 + TypeScript + Vite + DevExtreme Vue 25.2+ + axios + Pinia + vue-i18n |
| 构建命令 | `cd src/WebTenantPlatfrom && npm run build` |
| E2E 测试命令 | `cd src/WebTenantPlatfrom && npx playwright test` |
| 后端地址 | `http://127.0.0.1:5000` |
| 前端 dev server | `http://localhost:5173` |
| 管理员账号 | `admin / gjwq1234` |
| 路由模式 | Hash 模式（`createWebHashHistory()`） |
| 旧项目（仅参考，不修改） | `web/tenant-platform-web` |

---

## 三、环境预检（每次会话启动必须执行）

```bash
# 1. PostgreSQL 连接
PGPASSWORD=gjwq1234 psql -h localhost -U postgres -d test1 -c "SELECT 1;" 2>/dev/null

# 2. 后端 health check
curl -s http://127.0.0.1:5000/api/health/ | head -c 200

# 3. Playwright 浏览器
cd src/WebTenantPlatfrom && npx playwright --version

# 4. 前端 dev server
curl -s http://localhost:5173/ | head -c 200
```

如果任何一项失败，按 `.ai/system/e2e-testing-workflow.md` 中的启动流程处理。

---

## 四、单个模块的标准交付流程

每个业务模块子任务必须按以下顺序完成：

```text
1. 阅读前置文件（本文件 + 子任务文件 + 后端 API 提示词）
2. 填写需求补全矩阵、组件复用决策表、权限入口可见性矩阵
3. 查阅 DevExpress 文档（dxdocs）并写能力矩阵
4. 编码实现
   - 创建 Vue 组件（views/{module}/）
   - 创建 API 文件（api/{module}.ts）
   - 创建类型定义（types/{module}.ts）
   - 创建 5 个语言文件（*.vue.{locale}.json）
   - 更新路由（router.ts）
   - 更新权限常量（constants/permissions.ts，如需要）
5. 构建验证：npm run build
6. E2E 测试编写与迭代（最多 5 次），当迭代 5 次仍未完成，必须写明阻塞点并输出 session-summary
7. 自审（self-review-protocol F1-F7）
8. 输出 session-summary 至 `.ai/workspace/`
9. 更新所执行的子任务文档中的“任务快照”与状态
10. 如果发现新的通用问题，更新相应提示词文件
```

---

## 五、关键编码约束（内联提醒）

以下规则在 `copilot-instructions.md` 中有完整定义，此处仅做重点提醒：

### 5.1 DxColumn caption 必须使用 $t() 绑定

```vue
<!-- ✅ 正确 -->
<DxColumn data-field="Code" :caption="$t('角色编码')" />

<!-- ❌ 错误 -->
<DxColumn data-field="Code" caption="角色编码" />
```

### 5.2 notifySuccess / confirmAction 仅传 i18n key

```typescript
// ✅ 正确
notifySuccess('创建成功')

// ❌ 错误
notifySuccess(t('创建成功'))
```

### 5.3 每个 .vue 文件必须有 5 个语言文件

```text
ComponentName.vue
ComponentName.vue.zh-CN.json
ComponentName.vue.en-US.json
ComponentName.vue.ja-JP.json
ComponentName.vue.ms-MY.json
ComponentName.vue.zh-TW.json
```

### 5.4 DevExpress 文档查阅（dxdocs）

首次使用任何 DevExtreme 组件时，必须先调用 `devexpress_docs_search` + `devexpress_docs_get_content` 查阅文档，禁止凭猜测编码。

### 5.5 登录页 DxForm label-mode 使用 static

```vue
<DxForm label-mode="static">
```

### 5.6 通用组件标准（每个模块必须遵循）

每个业务模块必须遵循 `.ai/prompts/08-platform/frontend/0050_common-components-standard.md` 中定义的规范：

- **功能说明 & 操作指引**：页面标题区域必须包含 FunctionDescriptionCard（info 图标）和 OperationGuideDrawer（help 图标），内容详细完整
- **统一搜索区域**：所有查询条件统一为一个区域，高级查询展开追加在当前区域中，操作按钮始终在末尾
- **Grid 操作列溢出**：前 2 个常用操作直接显示，超过部分使用 DxDropDownButton（"更多"下拉菜单），操作列宽度控制在 200px
- **枚举/下拉数据源**：必须使用 `computed` 绑定 i18n，禁止静态缓存
- **DevExtreme 国际化**：main.ts 已加载语言包，切换语言时 header-toolbar 自动同步 DevExtreme locale
- **DxDateRangeBox**：必须配置 `start-date-label` 和 `end-date-label` i18n
- **新增/编辑后行高亮**：使用 `focused-row-enabled` + `focused-row-key`
- **重置密码展示**：重置密码后必须展示系统生成的新密码弹窗

---

## 六、验收闭环

```text
编码 → npm run build → E2E 测试迭代 → 代码搜索审查（F1-F7）→ 修复违规 → 再次编译 → session-summary
```

**编译和测试通过不是最终验收标准。** 必须执行 self-review-protocol 中的代码搜索验证。

---

## 七、会话结束输出

每个子任务完成后，Agent 必须：

1. 在 `.ai/workspace/` 创建 `session-summary-{YYYYMMDD}-{seq}.md`
2. 更新 `.ai/tasks/platform-frontend/README.md` 中的执行状态
3. 更新 `.ai/prompts/08-platform/frontend/0000_overview.md` 中的模块状态
4. 更新所执行的子任务文档中的“任务快照”区块
5. 如果发现新通用问题，按 `.ai/prompts/03-frontend/13-prompt-evolution-workflow.md` 更新提示词沉淀

---

## 版本

- 版本：1.0
- 创建日期：2026-04-13
- 用途：消除子任务间重复的前置阅读内容，确保每个子任务文件简洁聚焦
