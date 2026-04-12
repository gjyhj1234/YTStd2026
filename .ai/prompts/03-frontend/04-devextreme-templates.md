# DevExtreme Vue Application Template / UI Templates / dxdocs 使用规范

> 本文件定义前端开发中 DevExtreme Vue 模板体系和 dxdocs 文档查阅的强制规范。
> 所有涉及模板、布局、组件、交互模式、页面结构的前端任务必须遵循本文件。

---

## 一、模板体系定义

### 1.1 Application Template

**名称**：DevExtreme Vue Application Template

**用途**：定义整个应用的骨架结构，包括：

- 应用入口（App.vue）
- 主布局（侧边栏 + 顶栏 + 内容区）
- 导航结构（DxDrawer + DxTreeView）
- 路由集成
- 认证流程骨架
- 主题配置

**强制约束**：

1. 必须基于 DevExtreme Vue Application Template 的标准结构搭建应用骨架
2. 不得自行设计不兼容的布局方案
3. 布局组件必须使用 DevExtreme 提供的 `DxDrawer`、`DxToolbar` 等
4. 导航必须使用 `DxTreeView` 或 `DxList`

**dxdocs 查阅要求**：

```
devexpress_docs_search(technologies: ["Vue"], question: "DevExtreme Vue Application Template structure layout navigation sidebar")
devexpress_docs_search(technologies: ["Vue"], question: "DxDrawer component opened-state-mode reveal-mode position template")
```

### 1.2 UI Templates

**名称**：DevExtreme Vue UI Templates

**用途**：定义各类页面的标准布局模式，包括：

- 列表页模板（DataGrid + 工具栏 + 分页）
- 表单页模板（DxForm + 验证 + 提交）
- 详情页模板（只读展示 + 操作按钮）
- 仪表盘模板（统计卡片 + 图表）
- 登录页模板（认证表单）

**强制约束**：

1. 实现新页面前必须先确认是否有对应的 UI Template 可用
2. 如有可用的 UI Template，必须基于其结构实现，不得自行设计
3. 如没有完全匹配的 UI Template，必须以最接近的模板为基础进行扩展

**dxdocs 查阅要求**：

```
devexpress_docs_search(technologies: ["Vue"], question: "DevExtreme Vue UI Templates page layout patterns")
```

---

## 二、dxdocs 强制查阅规范

### 2.1 查阅工作流（每个 DevExtreme 组件必须执行）

```
步骤 1: 调用 devexpress_docs_search（每个问题仅调用一次）
         → technologies: ["Vue"]
         → question: 具体到组件名 + 属性/行为

步骤 2: 调用 devexpress_docs_get_content
         → 获取最相关帮助主题的全文

步骤 3: 反思获取到的内容
         → 提取可用的 API、属性、代码示例
         → 对比当前需求，判断是否适用

步骤 4: 基于检索到的信息编码
         → 引用具体控件和属性名称
         → 参考文档中的代码示例
```

### 2.2 必须查阅的场景

| 场景 | 是否必须查阅 | 说明 |
|------|:----------:|------|
| 首次使用某个 DevExtreme 组件 | ✅ | 必须了解完整 API |
| 组件行为异常 | ✅ | 可能是用法不当 |
| 配置高级功能 | ✅ | 如远程分页、异步验证 |
| 样式定制 | ✅ | CSS 变量、主题覆盖 |
| 已查阅且用法简单重复 | ❌ | 可复用上次查阅结果 |

### 2.3 查阅约束

1. **每个问题仅调用一次 `devexpress_docs_search`**，避免冗余查询
2. **必须基于从 dxdocs 获取的信息编码**，禁止凭猜测或过时记忆
3. **如果文档中有相关代码示例，必须参考**
4. **必须引用文档中提到的具体 DevExtreme 控件和属性名称**
5. **禁止在不查阅 dxdocs 的情况下猜测 DevExtreme 组件的 API 和行为**

### 2.4 常用查阅模板

```
# 数据表格
devexpress_docs_search(technologies: ["Vue"], question: "DxDataGrid CustomStore remote operations load function skip take totalCount")

# 表单
devexpress_docs_search(technologies: ["Vue"], question: "DxForm validation rules required stringLength async validationCallback")

# 弹窗
devexpress_docs_search(technologies: ["Vue"], question: "DxPopup content template slot visible showing hiding event")

# 树形列表
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeList parent-id-expr key-expr auto-expand-all selection recursive")

# 抽屉导航
devexpress_docs_search(technologies: ["Vue"], question: "DxDrawer opened-state-mode shrink reveal-mode expand position left")

# 树形导航
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeView item template selectByClick focusStateEnabled CSS customization")

# 下拉框
devexpress_docs_search(technologies: ["Vue"], question: "DxSelectBox data-source display-expr value-expr search-enabled")

# 标签页
devexpress_docs_search(technologies: ["Vue"], question: "DxTabPanel items selected-index animation swipe-enabled")

# 图表
devexpress_docs_search(technologies: ["Vue"], question: "DxChart series argument-field value-field type bar line")

# 主题/本地化
devexpress_docs_search(technologies: ["Vue"], question: "DevExtreme Vue theme CSS variables localization locale loadMessages")
```

---

## 三、组件使用决策流程

在实现任何 UI 功能前，必须按以下流程决策：

```
需要实现 UI 功能
  ↓
是否有 DevExtreme Vue 组件可用？
  ↓ 通过 dxdocs 查阅确认
  ├── 有 → 使用 DevExtreme 组件
  └── 不确定 → 再次查阅 dxdocs，扩大搜索范围
        ├── 确认有 → 使用 DevExtreme 组件
        └── 确认无 → 停下并请求人工确认
              ├── 人工确认可手写 → 手写组件（记录原因）
              └── 人工确认不可 → 等待指示
```

**禁止**直接跳过查阅步骤手写以下组件：

- 数据表格（必须用 DxDataGrid 或 DxTreeList）
- 表单（必须用 DxForm）
- 弹窗（必须用 DxPopup）
- 抽屉（必须用 DxDrawer）
- 分页器（必须用 DxDataGrid 内置分页或 DxPager）
- 树形视图（必须用 DxTreeView 或 DxTreeList）
- 工具栏（必须用 DxToolbar）
- 下拉框（必须用 DxSelectBox 或 DxDropDownBox）
- 日期选择器（必须用 DxDateBox）
- 按钮（必须用 DxButton）
- 加载面板（必须用 DxLoadPanel）

---

## 四、任务输出中的模板声明

每个前端任务完成后，必须在 session summary 中声明：

```markdown
## DevExtreme 模板与组件使用声明

### Application Template 能力
- 使用了: [列出使用的 Application Template 能力]

### UI Template 模式
- 使用了: [列出使用的 UI Template 或页面结构模式]

### dxdocs 查阅记录
| 查阅问题 | 采用的组件/API/属性 |
|---------|-------------------|
| DxDataGrid CustomStore... | CustomStore.load, remoteOperations, ... |
| ... | ... |
```

---

## 五、DevExtreme 版本约束

- **DevExtreme Vue 版本**：25.2+（见 `.ai/context/tech-stack.md`）
- 必须使用与该版本兼容的 API
- 如果 dxdocs 返回的 API 标注了版本要求，必须确认兼容性
