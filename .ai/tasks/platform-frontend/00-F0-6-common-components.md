# 子任务 F0-6 — 通用组件

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F0-6 |
| 模块名称 | 通用组件（FunctionDescriptionCard + OperationGuideDrawer） |
| 并行组 | — （串行，基础设施） |
| 对应提示词 | 包含在 `.ai/prompts/08-platform/frontend/0010_layout.md` 中 |
| 后端 API 提示词 | 无 |
| 依赖任务 | F0-1 脚手架搭建（✅ 已完成）、F0-3 i18n 基础设施（✅ 已完成） |
| 完成会话 | `session-summary-20260413-10` |
| 状态 | ✅ 已完成 |

---

## 任务目标

创建可跨模块复用的通用 UI 组件，供后续业务页面使用。当前包含功能说明卡片（FunctionDescriptionCard）和操作指南抽屉（OperationGuideDrawer）两个组件。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0010_layout.md` — 主布局提示词（通用组件部分）
- `.ai/prompts/03-frontend/04-devextreme-templates.md` — DevExtreme 组件规范

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue` | 功能说明卡片组件 |
| `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue` | 操作指南抽屉组件 |
| `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue.zh-CN.json` | 中文语言文件 |
| `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue.en-US.json` | 英文语言文件 |
| `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue.ja-JP.json` | 日文语言文件 |
| `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue.ms-MY.json` | 马来语言文件 |
| `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue.zh-TW.json` | 繁中语言文件 |
| `src/WebTenantPlatfrom/src/utils/notify.ts` | 通知工具函数 |

---

## 组件功能清单

### FunctionDescriptionCard

| 功能 | 说明 |
|------|------|
| v-model:visible | 双向绑定显示/隐藏状态 |
| 关闭按钮 | 点击关闭卡片 |
| slot 内容区域 | 通过默认 slot 传入功能说明文本 |
| 5 个语言文件 | 组件内文本国际化 |

### OperationGuideDrawer

| 功能 | 说明 |
|------|------|
| v-model:visible | 双向绑定显示/隐藏状态 |
| DxDrawer overlap 模式 | 从右侧滑出 |
| DxScrollView 内容区域 | 支持滚动的指南内容 |
| 5 个语言文件 | 组件内文本国际化 |

### notify.ts 工具函数

| 函数 | 说明 |
|------|------|
| notifySuccess(key) | 成功提示（接收 i18n key，内部调用 t()） |
| notifyError(key) | 错误提示 |
| confirmAction(key, params?) | 确认操作弹窗（支持 {name} 参数替换） |
| confirmDelete(name) | 确认删除弹窗 |

---

## 验收标准

- [x] FunctionDescriptionCard.vue 存在且支持 v-model:visible
- [x] FunctionDescriptionCard.vue 有 5 个对应语言文件，key 完全一致
- [x] OperationGuideDrawer.vue 存在且使用 DxDrawer overlap 模式
- [x] OperationGuideDrawer.vue 有 5 个对应语言文件，key 完全一致
- [x] notify.ts 包含 notifySuccess、notifyError、confirmAction、confirmDelete
- [x] notify.ts 仅接收 i18n key（内部调用 t()），外部禁止双重 t()
- [x] `npm run build` 通过

---

## 已完成说明

本子任务已在 `session-summary-20260413-10` 中完成（与 F1-1 主布局一起实现）。主要产出：

1. **FunctionDescriptionCard.vue**：
   - 功能说明卡片组件，支持 v-model:visible 双向绑定
   - 包含关闭按钮、slot 内容区域
   - 5 个语言文件（zh-CN/en-US/ja-JP/ms-MY/zh-TW）
2. **OperationGuideDrawer.vue**：
   - 操作指南抽屉组件，使用 DxDrawer overlap 模式从右侧滑出
   - 支持 v-model:visible、DxScrollView 内容区域
   - 5 个语言文件
3. **utils/notify.ts**：
   - notifySuccess(key)、notifyError(key)、confirmAction(key, params?)、confirmDelete(name)
   - 内部使用 `i18n.global.t()` 翻译，外部仅传 key
