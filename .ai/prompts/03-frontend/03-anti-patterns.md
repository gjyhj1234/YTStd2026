# 前端反模式清单（持续扩充）

> 本文件用于沉淀“已经真实发生过、以后必须避免”的问题。每次出现新的可复用教训，必须更新本文件或 `13-prompt-evolution-workflow.md` 指向的其他文件。

---

## 一、需求与任务反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| A1 | 只实现用户描述里直接写出来的功能 | 生产默认能力缺失 | 先填写“需求补全矩阵”，按 `09-production-defaults.md` 补齐默认能力 |
| A2 | 业务提示词没有写清字段/按钮/状态流转就直接编码 | 页面看似完成，实际漏项 | 使用 `07-business-prompt-template.md` 完整列出所有字段、按钮、能力矩阵 |
| A3 | 一个模块一次性做完 | 超时、中断、质量失控 | 先按 `01-task-splitting.md` 拆成 slice |
| A4 | 只输出聊天进度，不更新 `.ai/tasks/` 和 `.ai/workspace/` | 下一轮无法续接 | 每轮必须写任务快照和 session summary |
| A5 | 修了 bug 但不更新提示词 | 下个任务重复犯错 | 按 `13-prompt-evolution-workflow.md` 回写相应提示词 |

---

## 二、DevExtreme 与页面能力反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| D1 | 登录页 `DxForm` 使用 `floating` label-mode | 浏览器自动填充时 label 与值重叠 | 登录页必须使用 `label-mode="static"` |
| D2 | `DxColumn caption` 硬编码 | 语言切换后列头不更新 | 必须使用 `:caption="$t('...')"` |
| D3 | 未查 dxdocs，凭记忆写 DevExtreme API | 属性名错误、行为失效 | 每个复杂组件先查 `dxdocs` |
| D4 | 使用了 DataGrid，但没有声明排序、列宽、分页、列隐藏、固定列 | 页面“能用但不好用”，且模块间能力不一致 | 每个业务提示词都必须填写 DataGrid 能力矩阵 |
| D5 | 侧边栏 `DxTreeView` 选中态引起布局偏移 | 菜单点击后错位 | 参考 dxdocs + CSS 覆盖方案 |
| D6 | 弹窗/抽屉只实现 happy path，不考虑空态、加载态、关闭行为 | 交互不完整 | Popup/Drawer 必须写清 visible、close、loading、unsaved 行为 |
| D7 | 在 DxDataGrid / DxTreeList / 详情弹窗中显示 ID 字段 | ID 是系统内部字段，用户不应看到 | 禁止在列表、树、详情弹窗中展示 ID 字段。`key-expr="Id"` 可保留但不渲染为 DxColumn |
| D8 | 弹窗在移动端使用固定宽度而非全屏 | 小屏设备内容溢出或显示不全 | 移动端（<768px）弹窗必须使用 `width: windowWidth`、`height: '100vh'`，同时确保关闭按钮可见 |
| D9 | DxTreeView `selection-changed` 处理函数每次都更新 reactive 状态 | 展开/收起节点时触发不必要的 Vue 重渲染，导致树状态重置（展开后无法收起） | 必须先比较新旧选中 key 集合，仅在选中项实际变化时才更新 reactive ref |

---

## 三、组件复用反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| C1 | 每个模块自己写一套搜索区布局 | 维护困难、样式漂移 | 使用统一搜索区规范，必要时提升为共享资产 |
| C2 | 每个模块自己写一套“更多操作”下拉 | 操作列风格不一致 | 复用 `0050_common-components-standard.md` 中的操作列规范 |
| C3 | 同样的状态标签每页自己映射颜色 | 颜色和语义不一致 | 提升为共享状态资产或共享 renderer |
| C4 | 共享资产已经存在，仍在页面中复制代码 | 缺陷重复 | 先做组件复用决策表，禁止直接复制 |

---

## 四、国际化反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| I1 | `notifySuccess(t('创建成功'))` | 双重翻译或翻译失败 | `notifySuccess('创建成功')` |
| I2 | 组件特有 key 放主语言文件 | key 归属混乱 | 组件特有 key 放 `*.vue.{locale}.json` |
| I3 | 漏掉 4 个语言文件或 key 不一致 | 编译能过但切换语言出错 | 每个 `.vue` 必须有 5 个语言文件且 key 完全一致 |
| I4 | 静态配置文件中的文本没有翻译归属 | columns/query-form/form 容易漏翻 | 在业务提示词中显式写静态配置翻译归属 |
| I5 | 组件 JSON 中 `null` key 没有对应 common 翻译 | 运行时出现 key 原文 | `null` 只能用于已有 common key |

---

## 五、权限与导航反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| P1 | 按钮权限做了，菜单入口也一并隐藏 | 用户拥有子权限却找不到入口 | 必须输出“权限入口可见性矩阵” |
| P2 | 只有路由守卫，没有菜单显隐规则 | 看得到点不开，体验割裂 | 菜单、路由、按钮三层分别声明 |
| P3 | 父菜单权限比子页面权限更严 | 页面理论可访问，实际入口消失 | 父菜单默认由最小可读权限驱动 |
| P4 | 无权限时直接白屏或跳首页 | 难以诊断权限问题 | 显示 403 或明确无权限提示 |
| P5 | 前端 `hasPermission` 未处理超级管理员（`IsSuperAdmin`）旁路 | 超级管理员登录后菜单全部消失（后端不为超管分配具体权限码） | `hasPermission` / `hasAnyPermission` 必须先检查 `IsSuperAdmin`，与后端 `CurrentUser.HasPermission` 逻辑一致 |

---

## 六、测试与验收反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| T1 | 口头说“已测试”，没有命令和结果 | 伪通过 | 未运行命令就视为未测试 |
| T2 | E2E 选择器依赖 CSS 类名或 DevExtreme 内部结构 | 稍改样式就全挂 | 核心元素必须使用 `data-testid` |
| T3 | 只测成功路径，不测表单验证、权限、空态 | 实际 bug 大量漏出 | E2E 证据矩阵必须覆盖负路径 |
| T4 | 同一表单字段标签重复或绑定错位，测试未发现 | 用户看到两个相同 label 或字段错位 | 增加“标签-字段映射”测试维度 |
| T5 | 删掉失败测试来“通过” | 验收失真 | 只能修代码或修正错误测试，禁止删除用例 |
| T6 | 只跑 build，不跑 E2E/self-review | 质量失真 | build、E2E、自审必须都有证据 |

---

## 七、项目结构与状态反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| S1 | 误删旧前端 `web/tenant-platform-web` | 丢失参考与回滚依据 | 旧前端必须保留 |
| S2 | 静态配置全部内联到 `.vue` | 文件过大，难复用 | 拆为 `columns.ts`、`query-form.ts`、`form.ts` 等 |
| S3 | 提示词/JSON/代码出现乱码 | Agent 误解需求，界面文本异常 | 每轮都做乱码检查 |

---

## 八、API 类型与网络反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| N1 | 跨模块 API 用错误的 DTO 类型接收响应 | 字段缺失或类型不对，组件绑定失败 | 每个前端 TypeScript 类型必须与后端 DTO 字段精确一一对应。不同 DTO 不能混用，必要时新建专用类型 |
| N2 | 批量操作未禁用 `preventDuplicate` | 用户连续点击时前一个请求被 abort，仅最后一次生效 | 批量操作 API 传入 `{ preventDuplicate: false }`，并在 UI 层用 `batchOperating` ref 锁定按钮 |
| N3 | 批量操作按钮无 loading/disabled 锁 | 用户可连续点击发起多次请求 | 批量按钮必须在操作期间 disabled，函数入口检查 `batchOperating.value` |
| N4 | 业务提示词只列本模块 API，未列跨模块依赖 | Agent 凭猜测选择类型，导致字段不匹配 | 业务提示词必须显式列出"跨模块 API 依赖"表，包含 URL、响应体类型、前端 TypeScript 类型 |

---

## 九、移动端适配反模式

| 编号 | 反模式 | 风险 | 正确做法 |
| :----: | -------- | ------ | --------- |
| M1 | 弹窗（DxPopup）使用固定宽度 `:width="600"` | 375px 手机屏幕下弹窗超出屏幕，内容被裁剪 | 弹窗宽度必须用 `computed(() => Math.min(600, windowWidth - 32))`，同时设置 `:max-height="'90vh'"`。FunctionDescriptionCard 和 OperationGuideDrawer 已内置此逻辑 |
| M2 | DataGrid 使用固定高度或未禁止内部滚动 | 手机页面出现双滚动条（布局 DxScrollView + DataGrid 内滚动） | 移动端 DataGrid 必须 `height: auto`，通过 CSS `overflow: hidden` 禁止内部滚动 |
| M3 | 工具栏显示超过 2 个按钮 | 375px 下按钮挤压变形或换行混乱 | 移动端工具栏最多 2 个按钮，其余使用 DxDropDownButton("更多") 收纳 |
| M4 | 搜索区域不支持折叠 | 搜索条件占满屏幕，用户看不到数据 | 搜索区域必须支持折叠（showAdvanced toggle），默认仅显示基础搜索 |
| M5 | 未设置列隐藏优先级（hiding-priority） | 移动端 DataGrid 出现横向滚动条 | 所有非核心列必须设置 `hiding-priority`，核心列（名称、状态、操作）永不隐藏 |
| M6 | 页面出现横向滚动 | 移动端体验极差，用户误触横滑 | 页面容器 + DataGrid 都必须 `overflow-x: hidden` |
| M7 | Header Toolbar 的 DxList 模板缺少 `@item-click` | 移动端 toolbar 溢出菜单中的用户菜单（退出登录等）点击无反应 | `menuUserItem` 模板的 DxList 必须绑定 `@item-click` 处理函数 |

---

## 十、缺陷回写路径

发现反模式后，不要只修代码，还要按下表回写：

| 问题类型 | 首选回写文件 |
| --------- | ------------- |
| 需求漏项 | `09-production-defaults.md` |
| 复用缺失 | `10-component-asset-catalog.md` |
| DevExtreme 能力遗漏 | `04-devextreme-templates.md` |
| 业务模板过粗 | `07-business-prompt-template.md` |
| E2E 漏测/伪通过 | `08-playwright-e2e.md` |
| 执行状态丢失 | `11-delivery-workflow.md` |
| GitHub 队列不合理 | `12-github-automation-workflow.md` |
| API 类型/网络问题 | `03-anti-patterns.md` 第八节 |
| 移动端适配问题 | `03-anti-patterns.md` 第九节 + `0050_common-components-standard.md` 第十一节 |

---

## 版本

- 版本：2.2
- 更新日期：2026-04-16
- 更新重点：新增移动端适配反模式（M1-M7：弹窗固定宽度、双滚动、工具栏溢出、搜索折叠、列隐藏优先级、横向滚动、Header DxList 缺 click handler）
