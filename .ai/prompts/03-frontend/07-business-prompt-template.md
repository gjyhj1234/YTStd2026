# 极细化业务实施提示词模板

> 本文件定义业务模块前端提示词的标准模板和写作规范。
> 所有 `.ai/prompts/08-platform/frontend/` 下的业务页面提示词必须按本模板格式撰写。
> 不允许写成笼统描述。

---

## 一、模板结构（必须严格遵循）

每个业务模块的前端提示词必须包含以下完整章节：

```markdown
# 租户平台 — {模块名称}页面

## 任务信息
## 前置阅读
## DevExpress 文档查阅
## API 端点
## 必须产出的文件
## 页面结构
## 查询功能
## 列表与分页
## 操作按钮
## 表单功能
## 静态配置文件
## 国际化要求
## 验收标准
```

---

## 二、各章节详细要求

### 2.1 任务信息

```markdown
## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F2-{N} |
| 所属阶段 | 层级 2：业务页面层 |
| 依赖任务 | F1-1 主布局 |
| 预计文件数 | {N} 个 |
```

### 2.2 前置阅读

必须列出所有需要阅读的文件，不能省略。

```markdown
## 前置阅读

- `.ai/prompts/03-frontend/00-governance.md` — 前端总治理
- `.ai/prompts/03-frontend/04-devextreme-templates.md` — DevExtreme 规范
- `.ai/prompts/03-frontend/05-axios-standard.md` — axios 规范
- `.ai/prompts/03-frontend/06-i18n-execution.md` — i18n 规范
- `.ai/prompts/03-frontend/03-anti-patterns.md` — 反模式清单
- `.ai/rules/frontend.md` — 前端开发规范
- `.ai/rules/i18n.md` — 国际化规范
- `.github/copilot-instructions.md` — 关键编码约束
- {后端 API 文档路径}
```

### 2.3 DevExpress 文档查阅

必须列出本模块需要查阅的 dxdocs 问题（具体到组件 + 属性）。

```markdown
## DevExpress 文档查阅（强制前置步骤）

**工作流**：详见 `04-devextreme-templates.md` 第二节

**本模块必须查阅的组件**：

| 组件 | 查阅问题 | 用途 |
|------|---------|------|
| DxDataGrid | CustomStore remote paging load function | 列表分页 |
| DxForm | validation rules required stringLength async | 表单验证 |
| DxPopup | content template slot visible | 弹窗 |
```

### 2.4 API 端点

必须列出**完整精确**的 API 端点表，包括 HTTP 方法、URL、请求体、响应体。

```markdown
## API 端点（精确匹配）

| 操作 | HTTP 方法 | URL | 请求体 | 响应体 |
|------|----------|-----|--------|--------|
| 列表 | GET | /api/{module}?Page=1&PageSize=20&Keyword=&Status= | - | ApiResult<PagedResult<{DTO}>> |
| 详情 | GET | /api/{module}/{id} | - | ApiResult<{DTO}> |
| 创建 | POST | /api/{module} | { Field1, Field2, ... } | ApiResult<long> |
| ... | ... | ... | ... | ... |
```

### 2.5 必须产出的文件

完整列出所有需要创建/修改的文件。

```markdown
## 必须产出的文件

| 序号 | 文件路径 | 用途 |
|:----:|---------|------|
| 1 | `src/views/{module}/{PageName}View.vue` | 主页面 |
| 2 | `src/views/{module}/{PageName}View.vue.zh-CN.json` | 中文语言 |
| 3 | `src/views/{module}/{PageName}View.vue.en-US.json` | 英文语言 |
| 4 | `src/views/{module}/{PageName}View.vue.ja-JP.json` | 日文语言 |
| 5 | `src/views/{module}/{PageName}View.vue.ms-MY.json` | 马来文语言 |
| 6 | `src/views/{module}/{PageName}View.vue.zh-TW.json` | 繁中语言 |
| 7 | `src/api/{module}.ts` | API 封装 |
| 8 | `src/types/{module}.ts` | 类型定义 |
| 9 | `src/router/index.ts`（追加） | 路由注册 |
| 10 | `src/constants/permissions.ts`（追加） | 权限码 |
```

### 2.6 页面结构

必须逐项明确页面包含的区域。

```markdown
## 页面结构

| 区域 | 组件 | 内容 |
|------|------|------|
| 页面标题 | `<h2>` + `$t()` | {标题文本} |
| 页面副标题 | `<p>` + `$t()` | {副标题文本} |
| 功能说明区 | `FunctionDescriptionCard` | {说明内容} |
| 查询区 | DxForm / 自定义 | 见"查询功能"章节 |
| 高级查询区 | 折叠面板 | 见"查询功能"章节 |
| 工具栏 | DxToolbar | 见"操作按钮"章节 |
| 表格区 | DxDataGrid | 见"列表与分页"章节 |
| 分页 | DxDataGrid 内置 | 见"列表与分页"章节 |
| 新增弹窗 | DxPopup + DxForm | 见"表单功能"章节 |
| 编辑弹窗 | DxPopup + DxForm | 见"表单功能"章节 |
| 详情抽屉 | DxPopup / DxDrawer | 只读展示 |
| 操作指南 | OperationGuideDrawer | {指南内容} |
```

### 2.7 查询功能

必须逐项列出所有查询字段。

```markdown
## 查询功能

### 通用查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | Keyword | 关键词 | DxTextBox | '' | '请输入关键词' |
| 2 | Status | 状态 | DxSelectBox | null（全部） | '请选择状态' |

### 高级查询条件

| 序号 | 字段名 | 标签 | 类型 | 默认值 | placeholder |
|:----:|--------|------|------|--------|-------------|
| 1 | RoleId | 角色 | DxSelectBox | null | '请选择角色' |
| 2 | CreatedAtRange | 创建时间 | DxDateRangeBox | null | '起始日期 - 结束日期' |

### 查询行为
- 支持回车搜索：是
- 支持重置：是
- 支持展开/收起高级查询：是
- 所有查询文本已国际化：是
```

### 2.8 列表与分页

必须列出每一列和分页配置。

```markdown
## 列表与分页

### 表格组件：DxDataGrid + CustomStore

### 列定义

| 序号 | data-field | caption | 宽度 | 排序 | 格式化 | 说明 |
|:----:|-----------|---------|:----:|:----:|--------|------|
| 1 | Id | ID | 80 | 否 | - | |
| 2 | Username | 用户名 | auto | 是 | - | |
| 3 | DisplayName | 显示名 | auto | 否 | - | |
| 4 | Status | 状态 | 100 | 否 | statusCell | 颜色区分 |
| 5 | CreatedAt | 创建时间 | 180 | 是 | dateCell | yyyy-MM-dd HH:mm |
| 6 | - | 操作 | 380 | 否 | actionCell | 操作按钮 |

### 分页配置
- 支持分页：是
- 默认页大小：20
- 可选页大小：[10, 20, 50, 100]
- 显示总数：是
- 支持跳转：是
- 远程分页：是（CustomStore）

### 状态列颜色

| 状态值 | 显示文本 | 颜色 |
|:------:|---------|------|
| 1 | 已启用 | 绿色 (#52c41a) |
| 2 | 已禁用 | 红色 (#f5222d) |

### 空状态与加载
- 空数据文本：`$t('暂无数据')`
- 加载中：DxLoadPanel
```

### 2.9 操作按钮

必须逐个列出每个操作按钮的完整信息。

```markdown
## 操作按钮

### 工具栏按钮

| 按钮 | 文本 | 图标 | 权限码 | 启用条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 新增 | $t('新增') | add | PLATFORM_USER_CREATE | 始终 | 打开新增弹窗 | 无 |
| 批量启用 | $t('批量启用') | check | PLATFORM_USER_ENABLE | 选中行 > 0 | 批量启用 API | confirmAction |
| 批量禁用 | $t('批量禁用') | close | PLATFORM_USER_DISABLE | 选中行 > 0 | 批量禁用 API | confirmAction |

### 行操作按钮

| 按钮 | 文本 | 图标 | 权限码 | 显示条件 | 点击行为 | 确认框 |
|------|------|------|--------|---------|---------|--------|
| 查看 | $t('查看') | search | PLATFORM_USER_VIEW | 始终 | 打开详情抽屉 | 无 |
| 编辑 | $t('编辑') | edit | PLATFORM_USER_UPDATE | 始终 | 打开编辑弹窗 | 无 |
| 启用 | $t('启用') | check | PLATFORM_USER_ENABLE | Status === 禁用 | 调用启用 API | confirmAction |
| 禁用 | $t('禁用') | close | PLATFORM_USER_DISABLE | Status === 启用 | 调用禁用 API | confirmAction |
| 重置密码 | $t('重置密码') | key | PLATFORM_USER_RESET_PWD | 始终 | 调用重置 API | confirmAction |
| 删除 | $t('删除') | trash | PLATFORM_USER_DELETE | 始终 | 调用删除 API | confirmDelete |

### 确认框文案

| 操作 | 文案 | 国际化 |
|------|------|:------:|
| 启用 | '确认启用用户 {name}' | ✅ |
| 禁用 | '确认禁用用户 {name}' | ✅ |
| 重置密码 | '确认重置用户 {name} 的密码' | ✅ |
| 删除 | confirmDelete(item.DisplayName) | ✅ |

### 成功提示

| 操作 | 提示 | 国际化 |
|------|------|:------:|
| 创建 | notifySuccess('创建成功') | ✅ |
| 更新 | notifySuccess('更新成功') | ✅ |
| 启用 | notifySuccess('启用成功') | ✅ |
| 禁用 | notifySuccess('禁用成功') | ✅ |
| 重置密码 | notifySuccess('重置密码成功') | ✅ |
| 删除 | notifySuccess('删除成功') | ✅ |
```

### 2.10 表单功能

必须逐字段列出完整信息。

```markdown
## 表单功能

### 新增表单

**标题**：`$t('新增{实体名}')`
**组件**：DxPopup + DxForm

| 序号 | 字段名 | 标签 | 类型 | 必填 | 长度 | 格式 | 唯一性 | 默认值 | 禁用条件 | 显隐条件 |
|:----:|--------|------|------|:----:|------|------|:------:|--------|---------|---------|
| 1 | Username | 用户名 | DxTextBox | ✅ | 3-50 | 字母数字下划线 | ✅ async | '' | - | - |
| 2 | Password | 密码 | DxTextBox(mode:password) | ✅ | 6-100 | - | - | '' | - | - |
| 3 | DisplayName | 显示名 | DxTextBox | ✅ | 2-50 | - | - | '' | - | - |
| ... | ... | ... | ... | ... | ... | ... | ... | ... | ... | ... |

**唯一性校验**：
- Username：调用 `GET /api/platform-users/check-username-exists?username=xxx`
- 编辑时排除当前 Id：`GET /api/platform-users/check-username-exists?username=xxx&excludeId=123`

**提交行为**：
- 提交前：调用 `formInstance.validate()`，不通过则阻止提交
- 提交时：`submitting = true`，禁用提交按钮
- 提交成功：关闭弹窗 → notifySuccess('创建成功') → 刷新列表
- 提交失败：显示错误提示（axios 拦截器自动处理） → 不关闭弹窗

**按钮**：
- 提交按钮：`$t('确定')`，`submitting` 时 disabled + loading
- 取消按钮：`$t('取消')`，关闭弹窗
```

### 2.11 静态配置文件

如果有静态配置需要拆分，必须明确列出。

```markdown
## 静态配置文件

| 文件 | 内容 | 翻译归属 |
|------|------|---------|
| columns.ts | 表格列定义数组 | 页面组件级语言文件 |
| query-form.ts | 查询表单字段配置 | 页面组件级语言文件 |
| status.ts | 状态字典 | common 或 enums |
```

### 2.12 国际化要求

明确列出本模块所有需要国际化的 key。

```markdown
## 国际化要求

### 组件级 key（放入 *.vue.{locale}.json）

| key | zh-CN | en-US | ja-JP | ms-MY | zh-TW |
|-----|-------|-------|-------|-------|-------|
| 平台用户管理 | 平台用户管理 | Platform Users | プラットフォームユーザー管理 | Pengurusan Pengguna Platform | 平台使用者管理 |
| ... | ... | ... | ... | ... | ... |

### common key（放入 common/{locale}.json，值为 null）

已有的 common key 无需重复定义，仅在组件级文件中写 null 即可：
- 查询、重置、新增、编辑、删除、确定、取消、创建成功、更新成功、删除成功 等
```

### 2.13 验收标准

**必须是逐项功能点 checklist，禁止笼统描述。**

```markdown
## 验收标准

### P0 — 功能点完整性

- [ ] 页面标题 "{模块名}" 存在
- [ ] 功能说明卡片存在
- [ ] 查询区包含：关键词、状态
- [ ] 高级查询包含：角色、创建时间范围
- [ ] 表格列包含：ID、用户名、显示名、状态、创建时间、操作
- [ ] 分页包含：页大小选择、总数显示、页码跳转
- [ ] 工具栏包含：新增按钮
- [ ] 行操作包含：查看、编辑、启用/禁用、重置密码、删除
- [ ] 新增弹窗包含字段：用户名、密码、显示名、邮箱、手机、角色
- [ ] 编辑弹窗包含字段：显示名、邮箱、手机、角色（无用户名、无密码）
- [ ] 详情抽屉展示所有关键字段

### P1 — 业务规则完整性

- [ ] 用户名必填验证
- [ ] 用户名长度 3-50 验证
- [ ] 用户名唯一性异步验证（新增）
- [ ] 用户名唯一性异步验证排除当前 Id（编辑）
- [ ] 密码必填验证
- [ ] 密码长度 ≥ 6 验证
- [ ] 显示名必填验证
- [ ] 每个操作按钮权限码存在且正确
- [ ] 每个操作按钮显隐逻辑正确
- [ ] 启用/禁用按钮根据当前状态显隐
- [ ] 删除有 confirmDelete 确认
- [ ] 危险操作有 confirmAction 确认

### P2 — 国际化完整性

- [ ] 5 个语言文件已创建且 key 一致
- [ ] DxColumn caption 全部使用 `:caption="$t()"`
- [ ] notifySuccess/confirmAction 不双重 t()
- [ ] 组件特有 key 在组件级文件（非主语言文件）
- [ ] null key 在 common 中有对应翻译
- [ ] 所有按钮文本已国际化
- [ ] 所有查询字段 label/placeholder 已国际化
- [ ] 所有弹窗标题已国际化
- [ ] 所有验证提示已国际化
- [ ] 所有状态显示值已国际化

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符
- [ ] Code Review 自检全部通过
```

---

## 三、禁止事项

1. **禁止**验收标准只写"实现 XX 功能""编译通过"
2. **禁止**省略查询条件详情
3. **禁止**省略表格列定义
4. **禁止**省略操作按钮的图标、权限码、确认文案
5. **禁止**省略表单字段的类型、长度、唯一性校验
6. **禁止**省略国际化 key 清单
7. **禁止**使用"等""等等""以此类推"代替具体列表
