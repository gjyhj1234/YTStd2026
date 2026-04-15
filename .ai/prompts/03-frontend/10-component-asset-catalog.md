# 前端组件资产目录与复用决策规范

> 本文件的目标是把“相同功能反复重写”变成“共享资产优先复用、达到阈值即提升为资产”。

---

## 一、总原则

1. **先查资产，再写页面。**
2. **同类交互出现第 2 次，就必须评估提升为共享资产。**
3. **禁止把共享交互复制到多个页面里再分别修改。**
4. **业务提示词必须写清每个关键功能点复用了什么资产。**

---

## 二、现有共享资产（当前仓库已知）

| 资产 | 路径/位置 | 用途 | 复用要求 |
| ------ | ----------- | ------ | --------- |
| FunctionDescriptionCard | `src/WebTenantPlatfrom/src/components/FunctionDescriptionCard.vue` | 功能说明入口 | 页面级必须优先复用 |
| OperationGuideDrawer | `src/WebTenantPlatfrom/src/components/OperationGuideDrawer.vue` | 操作指引入口 | 页面级必须优先复用 |
| notify / confirm | `src/WebTenantPlatfrom/src/utils/notify.ts` | 成功提示、确认弹窗 | 通知和确认逻辑必须复用 |
| dx locale 映射 | `src/WebTenantPlatfrom/src/utils/dx-locale.ts` | DevExtreme locale 同步 | locale 切换逻辑必须复用 |
| 通用组件与交互规范 | `08-platform/frontend/0050_common-components-standard.md` | 平台前端交互标准 | 页面设计必须遵守 |

---

## 三、必须优先组件化的交互类型

以下交互一旦在两个模块中重复出现，默认必须抽象成共享资产或共享配置：

| 类型 | 典型内容 |
| ------ | --------- |
| 页面头部 | 标题 + 功能说明 + 操作指引 |
| 搜索区 | 基础查询 + 高级查询 + 操作按钮 |
| Grid 操作列 | 查看/编辑直显 + 更多下拉 |
| 状态标签 | 启用/禁用/生命周期状态等颜色标签 |
| 实体表单弹窗 | 标题、底部按钮、提交/取消、loading |
| 唯一性校验 | async validator、排除当前 Id、错误提示 |
| 权限显隐 | 菜单、页面、按钮三级控制 |
| 密码结果展示 | 重置密码后结果弹窗 |
| E2E 选择器 | data-testid 命名规则和 helper |

---

## 四、推荐资产分层

| 分层 | 示例 | 说明 |
| ------ | ------ | ------ |
| 共享组件 | PageHeaderActions、SearchPanel、EntityFormDialog | 结构和样式都复用 |
| 共享配置 | `columns.ts`、`toolbar-actions.ts`、`status-options.ts` | 组件不变，配置复用 |
| 共享 composable | `useGridActions`、`usePermissionGate`、`useAsyncUniqueValidator` | 行为逻辑复用 |
| 共享测试 helper | `openEntityDialog`、`assertGridHeaders`、`assertPermissionEntryVisible` | 测试行为复用 |

> 如果还没有现成共享资产，先在业务提示词中写明“需要创建新共享资产”，再编码，不得直接复制粘贴页面代码。

---

## 五、复用决策表（强制）

每个业务提示词和每个实际 slice 都必须填写以下表：

| 功能点 | 应优先复用的资产 | 仓库现状 | 决策 | 理由 |
| -------- | ---------------- | --------- | ------ | ------ |
| 页面头部 | FunctionDescriptionCard + OperationGuideDrawer | 已存在 | 复用 | 已满足 |
| 搜索区 | SearchPanel 或共享样式规范 | 部分存在 | 扩展/新建 | 现有仅有样式，无统一组件 |
| Grid 操作列 | 0050 中的溢出规范 | 已存在 | 复用 | 避免每页重写按钮布局 |
| 状态标签 | StatusTag/共享 renderer | 不足 | 新建共享资产 | 第二个模块开始重复 |

---

## 六、资产提升触发条件

出现以下任一情况，必须从“页面内部实现”升级为“共享资产”：

1. 同类结构在第 2 个模块出现。
2. 同一交互已经在 2 个以上页面手工维护。
3. 同一类 bug 在多个页面重复出现。
4. E2E 测试需要对同类交互反复写定位逻辑。

---

## 七、禁止事项

1. 禁止把同一个搜索区布局复制到多个页面再各自微调。
2. 禁止每个模块自己写一套操作列“更多”逻辑。
3. 禁止每个模块自己写一套状态颜色映射。
4. 禁止在页面里硬编码复杂的权限判定表达式而不抽取。
5. 禁止共享资产已经存在却继续在页面本地重写。

---

## 八、版本

- 版本：1.0
- 创建日期：2026-04-15
- 创建原因：解决相同组件和交互模式反复重写、缺少复用约定的问题
