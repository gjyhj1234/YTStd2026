# DevExtreme Vue 模板、dxdocs 与能力矩阵规范

> 本文件不只是提醒“要查文档”，而是要求每个业务提示词对 DevExtreme 关键组件逐项声明能力开关。未声明的能力不视为“自由发挥”，而视为“需求缺项”。

---

## 一、模板体系总约束

1. 应用骨架必须基于 **DevExtreme Vue Application Template**。
2. 页面结构必须优先参考 **DevExtreme Vue UI Templates**。
3. 表格、表单、弹窗、抽屉、树、分页、工具栏等标准能力优先使用 `devextreme-vue`。
4. 在没有查过 dxdocs 之前，禁止凭经验自写替代组件。

---

## 二、dxdocs 强制工作流

每个 DevExtreme 复杂组件都必须执行以下流程：

1. `devexpress_docs_search(technologies: ["Vue"], question: "...")`
2. `devexpress_docs_get_content(...)`
3. 提取将采用的组件、属性、事件、代码示例。
4. 把采用结果写入业务提示词或任务快照。

### 2.1 记录格式

| 组件 | 查询问题 | 采用的能力 |
| ------ | --------- | ----------- |
| DxDataGrid | `DxDataGrid remote operations column chooser fixed columns focused row` | `remoteOperations`、`columnChooser`、`focusedRowEnabled` |
| DxForm | `DxForm validation rules async validation label mode static` | `validationRules`、`async`、`label-mode` |

---

## 三、DxDataGrid 能力矩阵（强制逐项声明）

只要业务页面使用 `DxDataGrid`，提示词中必须明确以下能力是“启用 / 禁用 / 不适用”。

| 能力 | 必须声明 | 推荐默认 | 说明 |
| ------ | --------- | --------- | ------ |
| `remoteOperations` | ✅ | 启用 | 后台管理页默认远程分页 |
| 默认排序字段 | ✅ | 显式声明 | 未声明即缺项 |
| `columnAutoWidth` | ✅ | 启用 | 避免列拥挤 |
| 列宽策略 | ✅ | 每列显式声明 | `width` / `min-width` / `auto` |
| `allowColumnResizing` | ✅ | 启用 | 桌面端推荐 |
| `allowColumnReordering` | ✅ | 按需启用 | 若禁用必须说明 |
| `columnChooser` | ✅ | 评估后显式决策 | 用户需要显示/隐藏列时不能省略 |
| 固定列 | ✅ | 主键列或操作列评估是否固定 | 特别是操作列 |
| `columnHidingEnabled` | ✅ | 移动端启用 | 必须声明 `hidingPriority` |
| `focusedRowEnabled` | ✅ | 启用 | 创建/编辑后高亮回到当前行 |
| `selection` | ✅ | 明确单选/多选/无选择 | 不能隐式决定 |
| 分页页大小 | ✅ | `[10, 20, 50, 100]` | 若不同必须说明 |
| `showInfo` / `showNavigationButtons` | ✅ | 启用 | 分页体验基线 |
| 空态文案 | ✅ | 启用 | `no-data-text` |
| 加载面板 | ✅ | 启用 | `DxLoadPanel` |
| 行操作列布局 | ✅ | 直显前 2 个 + 更多下拉 | 对应 `0050` 规范 |

### 3.1 DataGrid 业务提示词写法模板

```markdown
## DevExtreme 能力矩阵

### DxDataGrid

| 能力 | 状态 | 配置/说明 |
| ------ | ------ | ---------- |
| 远程分页 | 启用 | `CustomStore + remoteOperations` |
| 默认排序 | 启用 | `CreatedAt desc` |
| 列显示/隐藏 | 禁用 | 本页面列数少，不提供 chooser |
| 固定列 | 启用 | `操作列 fixed right` |
| 移动端列隐藏 | 启用 | `Email=2, Phone=1, CreatedAt=4` |
| 焦点行 | 启用 | 创建/编辑成功后定位到当前记录 |
```

---

## 四、DxForm 能力矩阵（强制逐项声明）

| 能力 | 必须声明 | 推荐默认 | 说明 |
| ------ | --------- | --------- | ------ |
| `label-mode` | ✅ | 登录页 `static`，其他页显式决策 | 未声明即缺项 |
| 必填校验 | ✅ | 按字段声明 | 不能笼统写“有校验” |
| 长度校验 | ✅ | 按字段声明 | 最小值、最大值都写清 |
| 格式校验 | ✅ | 按字段声明 | email、手机号、pattern |
| 异步唯一性校验 | ✅ | 对唯一字段启用 | 编辑场景排除当前 Id |
| 帮助文本 | ✅ | 显式决策 | `helpText` 是否需要 |
| 默认值 | ✅ | 按字段声明 | 禁止隐式依赖组件默认值 |
| 显隐条件 | ✅ | 按字段声明 | 如密码字段仅新增时可见 |
| 禁用条件 | ✅ | 按字段声明 | 如用户名编辑时只读 |
| 提交 loading | ✅ | 启用 | `submitting` |
| 提交前校验 | ✅ | 启用 | `formInstance.validate()` |

---

## 五、DxPopup / DxDrawer 能力矩阵

| 能力 | 必须声明 | 说明 |
| ------ | --------- | ------ |
| 标题 | ✅ | 标题文案必须国际化 |
| 尺寸 | ✅ | `width` / `height` / `fullScreen` |
| 响应式宽度 | ✅ | 移动端必须使用 `computed(() => Math.min(maxWidth, windowWidth - 32))`，禁止固定 `:width="600"` |
| 最大高度 | ✅ | 弹窗必须设置 `:max-height="'90vh'"`，防止内容溢出屏幕 |
| 拖拽 | ✅ | 小屏幕（<768px）禁用 `:drag-enabled="!isSmallScreen"` |
| 内容插槽 | ✅ | 必须声明使用 `template #content` |
| 关闭方式 | ✅ | 关闭按钮、遮罩点击、ESC 是否允许 |
| unsaved 行为 | ✅ | 表单有未保存内容时是否提醒 |
| 提交/取消按钮 | ✅ | 文案、位置、loading |

---

## 六、DxTreeView / Drawer / 权限树能力矩阵

| 能力 | 必须声明 | 说明 |
| ------ | --------- | ------ |
| `selectByClick` | ✅ | 特别是侧边栏导航和权限分配 |
| `focusStateEnabled` | ✅ | 影响选中态样式和焦点 |
| 搜索方式 | ✅ | 内建搜索还是外部搜索框 |
| 选中递归 | ✅ | 权限分配必须明确 |
| 入口权限 | ✅ | 菜单点击、URL 直达、无权限提示 |
| 样式覆盖 | ✅ | 如果需要 CSS 覆盖，必须说明具体原因 |

---

## 七、禁止自写替代能力

未经 dxdocs 查证与人工确认，禁止自写以下能力替代 DevExtreme：

1. 表格。
2. 表单布局。
3. 抽屉与侧边导航。
4. 分页器。
5. 选择树。
6. 下拉框。
7. 加载面板。

---

## 八、业务提示词中的最小声明集

每个使用 DevExtreme 的业务提示词，至少必须包含以下 4 类声明：

1. dxdocs 查询记录。
2. 组件能力矩阵。
3. 为什么启用或禁用了关键能力。
4. 与测试相关的可定位点（`data-testid` 或明确的 helper 方案）。

---

## 九、版本

- 版本：2.1
- 更新日期：2026-04-16
- 更新重点：§五 DxPopup 能力矩阵新增响应式宽度、最大高度、拖拽禁用声明（修复 375px 弹窗溢出）
