# 任务：租户平台前端全面工程化重构

## 任务信息

- **任务编号**：TASK-PLATFORM-FE-001
- **所属阶段**：阶段 G（前端全面重构）
- **优先级**：高
- **预估时间**：每子任务 30-60 分钟
- **前置任务**：TASK-PLATFORM-001（阶段 A-F 全部完成）
- **总纲提示词**：`.ai/prompts/08-platform/frontend/refactoring-master.md`

## 任务目标

对 `web/tenant-platform-web/` 前端工程进行系统性重构，使其达到生产级质量标准。重构范围包括：i18n 全量化、DevExtreme 组件化、表单验证完善、远程分页实现、操作反馈增强、错误处理优化、API 路径修正、权限控制完善等。

---

## 前置阅读

- `.ai/system/agent-contract.md`
- `.ai/rules/global.md`
- `.ai/rules/frontend.md`
- `.ai/rules/i18n.md`
- `.ai/rules/naming.md`
- `.ai/context/tech-stack.md`
- `.ai/prompts/08-platform/frontend/refactoring-master.md`（总纲）

---

## 子任务拆分

### G0：基础设施重构（全局影响，最高优先级）

**目标**：为所有后续模块重构奠定基础

**具体输出**：
1. `src/locales/index.ts` — 升级 i18n 架构，支持 ja-JP 语言、组件级语言文件按需加载
2. `src/locales/ja-JP.json` — 新增日语主语言文件
3. `src/locales/common/ja-JP.json` — 新增日语公共翻译文件
4. `src/locales/generated/ja-JP.json` — 新增日语错误码翻译文件
5. `src/locales/ms-MY.json` — 补齐缺失翻译 key（当前 446 → 599）
6. `src/locales/zh-TW.json` — 补齐缺失翻译 key（当前 445 → 599）
7. `src/utils/http.ts` — 增强：请求失败自动 notify Toast、网络错误处理
8. `src/utils/errorHandler.ts` — 集成 DevExtreme notify
9. `src/styles/global.css` — 移除硬编码颜色，使用 CSS 变量/DevExtreme 主题变量
10. `src/views/error/ForbiddenView.vue` — 403 禁止访问页面
11. `src/router/index.ts` — 添加 403 路由、完善路由守卫
12. `src/locales/enums/` — 创建枚举翻译文件目录（56 个枚举类型的 5 种语言翻译）
13. `src/composables/useNotify.ts` — 封装 DevExtreme notify 操作反馈 composable

**验收标准**：
- [x] ja-JP 语言完整可用（554 keys 主文件 + 61 common + 177 generated + 213 enum）
- [x] ms-MY、zh-TW 翻译 key 数量与 zh-CN 一致（均为 554 keys）
- [x] API 错误自动显示 notify Toast（errorHandler 集成 DevExtreme notify）
- [x] 403 页面可正常访问（/forbidden 路由 + ForbiddenView.vue）
- [x] `npm run build` 通过

---

### G1：布局与导航重构

**目标**：使用 DevExtreme 组件替代手写 HTML 布局

**具体输出**：
1. `src/layouts/MainLayout.vue` — 使用 DxDrawer + DxTreeView/DxList 重构侧边栏
2. `src/views/login/LoginView.vue` — 表单验证、错误处理增强、Loading 状态
3. `src/views/dashboard/DashboardView.vue` — 加载状态、空数据处理
4. 每个组件的 5 个语言文件

**验收标准**：
- [x] 侧边栏使用 DevExtreme 组件（DxDrawer + DxTreeView）
- [x] 登录表单有必填验证（validationRules + stringLength）
- [x] 仪表盘有加载状态指示（DxLoadPanel + isLoading）
- [x] 所有文本使用 t()（包括 FunctionDescriptionCard 和 OperationGuideDrawer）
- [x] `npm run build` 通过

---

### G2：平台管理模块（用户/角色/权限/安全）

**目标**：完善平台管理核心模块的全部功能

**具体输出**：
1. `PlatformUsersView.vue` — 远程分页、表单验证（用户名必填+唯一性检查、邮箱格式、密码长度）、操作确认、批量启用/禁用、详情弹窗、成功/失败 Toast
2. `PlatformRolesView.vue` — 远程分页、表单验证（编码必填+唯一性检查、名称必填）、权限绑定完善（展示当前已绑定权限）、成员绑定功能、操作确认
3. `PlatformPermissionsView.vue` — 权限树展示完善、创建/编辑权限（编码唯一性检查）
4. `PlatformSecurityView.vue` — 安全策略管理完善（密码策略、登录策略展示与编辑）
5. 每个组件的 5 个语言文件（共 20 个语言文件）

**API 对应**：
| 操作 | API 端点 |
|------|---------|
| 用户列表 | GET `/api/platform-users/` |
| 创建用户 | POST `/api/platform-users/` |
| 更新用户 | PUT `/api/platform-users/{id}` |
| 删除用户 | DELETE `/api/platform-users/{id}` |
| 启用/禁用用户 | PUT `/api/platform-users/{id}/enable\|disable` |
| 重置密码 | PUT `/api/platform-users/{id}/reset-password` |
| 检查用户名 | GET `/api/platform-users/check-username-exists` |
| 批量启用/禁用 | PUT `/api/platform-users/batch-enable\|batch-disable` |
| 角色列表 | GET `/api/platform-roles/` |
| 全部角色 | GET `/api/platform-roles/all` |
| 创建角色 | POST `/api/platform-roles/` |
| 更新角色 | PUT `/api/platform-roles/{id}` |
| 删除角色 | DELETE `/api/platform-roles/{id}` |
| 角色权限绑定 | POST `/api/platform-roles/{id}/permissions` |
| 获取角色权限 | GET `/api/platform-roles/{id}/permissions` |
| 角色成员绑定 | POST `/api/platform-roles/{id}/members` |
| 检查角色编码 | GET `/api/platform-roles/check-code-exists` |
| 权限树 | GET `/api/platform-permissions/tree` |
| 创建权限 | POST `/api/platform-permissions/` |
| 更新权限 | PUT `/api/platform-permissions/{id}` |
| 删除权限 | DELETE `/api/platform-permissions/{id}` |

**验收标准**：
- [x] 远程分页使用 CustomStore（PlatformUsersView + PlatformRolesView）
- [x] 表单验证（required + stringLength + email + password + async 唯一性检查）
- [x] 操作确认弹窗（删除 confirmDelete、启用/禁用 confirmAction）
- [x] 操作成功 Toast（notifySuccess）
- [x] 批量启用/禁用功能（PlatformUsersView）
- [x] 详情弹窗（PlatformUsersView）
- [x] 权限绑定完善 — 预加载已绑定权限（PlatformRolesView）
- [x] 成员绑定功能实现（PlatformRolesView）
- [x] 所有文本使用 $t() / t()（无硬编码中文）
- [x] CSS 硬编码颜色替换为 CSS 变量
- [x] DxLoadPanel 加载状态指示（Permissions + Security）
- [x] 20 个组件语言文件（4 视图 × 5 语言）
- [x] `npm run build` 通过

---

### G3：租户生命周期模块

**目标**：完善租户管理全生命周期功能

**具体输出**：
1. `TenantsView.vue` — 远程分页、完整生命周期操作（initialize/suspend/resume/terminate/convert-trial）、详情弹窗、表单验证（租户编码唯一性检查）、生命周期事件列表
2. `TenantGroupsView.vue` — 远程分页、树形展示、CRUD 完善
3. `TenantDomainsView.vue` — 远程分页、域名格式验证、删除确认
4. `TenantTagsView.vue` — 远程分页、租户标签绑定、删除确认
5. 每个组件的 5 个语言文件（共 20 个语言文件）

**API 对应**：
| 操作 | API 端点 |
|------|---------|
| 租户列表 | GET `/api/tenants/` |
| 创建租户 | POST `/api/tenants/` |
| 更新租户 | PUT `/api/tenants/{id}` |
| 删除租户 | DELETE `/api/tenants/{id}` |
| 初始化租户 | PUT `/api/tenants/{id}/initialize` |
| 暂停租户 | PUT `/api/tenants/{id}/suspend` |
| 恢复租户 | PUT `/api/tenants/{id}/resume` |
| 终止租户 | PUT `/api/tenants/{id}/terminate` |
| 试用转正式 | PUT `/api/tenants/{id}/convert-trial` |
| 检查编码 | GET `/api/tenants/check-code-exists` |
| 生命周期事件 | GET `/api/tenants/{id}/lifecycle-events` |
| 分组树 | GET `/api/tenant-groups/tree` |
| 创建分组 | POST `/api/tenant-groups/` |
| 更新分组 | PUT `/api/tenant-groups/{id}` |
| 删除分组 | DELETE `/api/tenant-groups/{id}` |
| 域名列表 | GET `/api/tenants/{tenantRefId}/domains` |
| 创建域名 | POST `/api/tenants/{tenantRefId}/domains` |
| 删除域名 | DELETE `/api/tenants/{tenantRefId}/domains/{domainId}` |
| 标签列表 | GET `/api/tenant-tags/` |
| 创建标签 | POST `/api/tenant-tags/` |
| 删除标签 | DELETE `/api/tenant-tags/{id}` |
| 设置租户标签 | PUT `/api/tenants/{id}/tags` |

---

### G4：租户配置模块

**具体输出**：
1. `TenantSystemConfigView.vue` — 配置列表展示、按键编辑配置值
2. `TenantFeatureFlagsView.vue` — 功能开关列表、启用/禁用切换、编辑
3. `TenantParametersView.vue` — 参数列表、CRUD 操作、删除确认
4. 每个组件的 5 个语言文件

---

### G5：SaaS 套餐与订阅模块

**具体输出**：
1. `PackagesView.vue` — 远程分页、发布/下架状态流转、表单验证（编码唯一性）
2. `PackageVersionsView.vue` — 版本列表、创建版本、能力管理入口
3. `SubscriptionsView.vue` — 远程分页、续费/升级操作、取消订阅
4. `TrialsView.vue` — 远程分页、创建试用
5. 每个组件的 5 个语言文件

---

### G6：计费与支付模块

**具体输出**：
1. `InvoicesView.vue` — 远程分页、发票明细查看、标记支付、作废、API 路径修正
2. `PaymentsView.vue` — 远程分页、API 路径修正为 `/api/payment-orders/`
3. `RefundsView.vue` — 远程分页、API 路径修正为 `/api/payment-refunds/`
4. 每个组件的 5 个语言文件

---

### G7：API 集成与 Webhook 模块

**具体输出**：
1. `ApiKeysView.vue` — 远程分页、详情查看、删除确认
2. `WebhooksView.vue` — 远程分页、CRUD 完善、启用/禁用
3. `WebhookDeliveryLogsView.vue` — 远程分页、日志详情查看
4. 每个组件的 5 个语言文件

---

### G8：运营与日志模块

**具体输出**：
1. `OperationsView.vue` — 运营统计展示完善
2. `OperationLogsView.vue` — 远程分页、日志详情弹窗
3. `AuditLogsView.vue` — 远程分页、审计详情弹窗
4. `SystemLogsView.vue` — 远程分页、日志详情弹窗
5. 每个组件的 5 个语言文件

---

### G9：通知与存储模块

**具体输出**：
1. `NotificationTemplatesView.vue` — 远程分页、模板 CRUD、启用/禁用
2. `NotificationsView.vue` — 远程分页、标记已读、详情查看
3. `StorageStrategiesView.vue` — 远程分页、策略 CRUD、启用/禁用
4. `TenantFilesView.vue` — 远程分页、文件删除确认、访问策略管理
5. 每个组件的 5 个语言文件

---

### G10：基础设施与系统设置模块

**具体输出**：
1. `InfrastructureView.vue` — 基础设施组件管理
2. `RateLimitPoliciesView.vue` — 限流策略 CRUD
3. `SystemMenusView.vue` — 菜单树管理、排序功能（调用 sort API）
4. `SystemDictionariesView.vue` — 字典类型管理、字典项 CRUD
5. 每个组件的 5 个语言文件

---

## 执行约束

1. 每个子任务按 G0 → G1 → G2 → ... → G10 顺序执行
2. 每个子任务完成后必须执行 `npm run build` 验证
3. 每个子任务完成后必须输出结构化验证清单（格式见 `refactoring-master.md` 第四节）
4. 验证清单通过后才可进入下一个子任务
5. 所有代码变更遵循 `.ai/rules/frontend.md` 和 `.ai/rules/i18n.md`

## 禁止事项

（详见 `refactoring-master.md` 第六节）

---

## 历史进度

| 阶段 | 状态 | 说明 |
|------|------|------|
| A — 数据库 | ✅ 完成 | 56 实体表 |
| B — 核心后端 | ✅ 完成 | 用户/角色/权限/安全/认证 |
| C — 扩展后端 | ✅ 完成 | 95+ 端点 |
| D — 测试 + Postman | ✅ 完成 | 329 platform tests，176 Postman requests |
| E — 前端初版 | ✅ 完成 | Demo 级别，质量不达标 |
| F — 国际化初版 | ✅ 完成 | 基础 i18n 架构，但不完整 |
| **G — 前端全面重构** | 🔄 进行中 | 本任务 |

### 子任务进度

| 子任务 | 状态 | 说明 |
|--------|------|------|
| G0 — 基础设施重构 | ✅ 完成 | useNotify composable、errorHandler增强、http网络错误处理、ja-JP全量翻译、ms-MY/zh-TW补齐554键、55枚举×5语言翻译、403页面、CSS变量化 |
| G1 — 布局导航重构 | ✅ 完成 | DxDrawer+DxTreeView侧边栏、DxButton替代原生按钮、登录表单验证+Loading、仪表盘DxLoadPanel+空数据、15组件语言文件、common+主语言文件新增key |
| G2 — 平台管理模块 | ✅ 完成 | CustomStore远程分页、表单验证(required+stringLength+email+async唯一性)、确认弹窗、成功Toast、批量操作、详情弹窗、权限/成员绑定完善、CSS变量化、DxLoadPanel、20组件语言文件、99+9新locale key |
| **G1-FIX — 登录页+导航修复** | ✅ 完成 | Issue #1 移除 placeholder 叠加 + Issue #2 修复侧边栏偏移 CSS |
| **G2-FIX — 平台管理模块修复** | ✅ 完成 | Issue #3~#8 弹窗 #content 插槽、Remark 回显、角色分配、权限树加载、安全中心密码修改 |
| G3 — 租户生命周期模块 | ⬜ 待执行 | |
| G4 — 租户配置模块 | ⬜ 待执行 | |
| G5 — SaaS 套餐订阅 | ⬜ 待执行 | |
| G6 — 计费支付模块 | ⬜ 待执行 | |
| G7 — API 集成模块 | ⬜ 待执行 | |
| G8 — 运营日志模块 | ⬜ 待执行 | |
| G9 — 通知存储模块 | ⬜ 待执行 | |
| G10 — 基础设施系统设置 | ⬜ 待执行 | |

---

## 人工检查发现的问题（2026-04-10 验收反馈）

以下问题在 G0-G2 完成后的人工验收中发现，必须在继续 G3 之前全部修复。
按菜单功能拆分为独立子任务，逐一实现并逐一人工验证。

---

### G1-FIX：登录页 + 侧边栏导航修复

**优先级**：最高（影响所有页面的基础交互）

#### Issue #1：登录页默认值与 placeholder 叠加

**现象**：当登录表单有默认账户信息（如开发环境预填 admin/password）时，默认值文本与 placeholder 文本产生视觉叠加，用户体验差。

**根因分析**：
- `LoginView.vue` 使用 `label-mode="floating"` + `placeholder`，当 formData 有默认值时，DxForm floating label 不会自动上浮，导致 placeholder 和默认值重叠。
- 当前 `formData` 初始为空字符串 `{ Username: '', Password: '' }`，但若环境配置或浏览器自动填充提供默认值，浮动标签仍在原位。

**修复方案**：
1. 移除 `placeholder` 属性 — `label-mode="floating"` 本身已提供浮动标签效果，不需要额外 placeholder
2. 如需要开发环境预填默认值，确保 formData 初始值非空时 floating label 正确上浮
3. 或改用 `label-mode="static"` + placeholder 二选一方案，避免两者同时出现

**涉及文件**：
- `src/views/login/LoginView.vue`

**验收标准**：
- [ ] 无默认值时：显示浮动标签，输入框无文字叠加
- [ ] 有默认值时：浮动标签上浮，默认值清晰可见
- [ ] 浏览器自动填充时：无叠加现象
- [ ] `npm run build` 通过

---

#### Issue #2：侧边栏子菜单点击后向左偏移

**现象**：登录成功后进入后台，左侧菜单点击某个子菜单后，该子菜单文本向左发生位移，且不会恢复原位。

**根因分析**：
- `MainLayout.vue` 中 DxTreeView 的 `@item-click` 触发路由导航后，可能引起组件重新渲染
- `.sidebar-tree-item.active` CSS 样式中 `font-weight: 600` 变化导致文本宽度改变，引起偏移
- DxTreeView 的 `select-by-click` 为 false，但内置选中状态 CSS 可能仍在影响布局
- 可能存在 DxTreeView 内部 `.dx-state-focused` / `.dx-state-active` 样式与自定义样式冲突

**修复方案**：
1. 检查 DxTreeView 的内置选中/聚焦 CSS 样式（`.dx-treeview-item-content`、`.dx-state-focused`）是否引起布局偏移
2. 对 `.sidebar-tree-item` 添加固定 `padding-left` 确保子菜单缩进一致
3. 使用 `font-weight: 600` 时配合 `letter-spacing` 或固定宽度容器避免文本宽度变化引起的偏移
4. 在 `global.css` 中覆盖 DxTreeView 的 `.dx-treeview-node` padding/margin 确保一致

**涉及文件**：
- `src/layouts/MainLayout.vue`
- `src/styles/global.css`

**验收标准**：
- [ ] 点击子菜单前后，菜单项位置不发生偏移
- [ ] 切换不同子菜单时，所有菜单项对齐一致
- [ ] 侧边栏折叠/展开后菜单项仍正常对齐
- [ ] `npm run build` 通过

---

### G2-FIX：平台管理模块修复

**优先级**：高（平台核心管理功能）

按菜单页面拆分为独立修复单元：

---

#### G2-FIX-A：用户管理页面修复（PlatformUsersView）

##### Issue #3：用户详情弹窗未在弹窗中显示 + 备注字段无法保存/回显

**现象**：
- 用户详情内容未展示在 DxPopup 弹窗中，而是直接显示在页面底部
- 编辑用户设置备注后保存，再次打开编辑弹窗时备注为空
- 新增用户填写备注后，后续编辑也看不到备注

**根因分析**：
1. **详情弹窗**：`DxPopup` 的内容可能未正确包裹在 popup 容器内。检查 `DxPopup` 的使用方式 — 当前代码中 `<DxPopup :visible="showDetailPopup">` 后面的 `<div v-if="detailUser">` 是直接子元素，可能需要使用 `<template #content>` 具名插槽或确保 DxPopup 版本支持默认插槽渲染。
2. **备注不回显**：`PlatformUserRepDTO`（后端响应 DTO）中 **没有** `Remark` 字段，`MapToDto` 方法未映射 `Remark`。因此编辑时 `onEdit()` 中 `Remark: ''` 是硬编码空字符串，而非从后端获取。
3. **新增备注**：创建时 `Remark` 字段通过 `CreatePlatformUserReqDTO` 发送到后端并保存到数据库，但列表/详情接口不返回该字段。

**修复方案**：
1. **详情弹窗**：确保 DxPopup 内容使用正确的插槽方式渲染，或检查 DevExtreme Vue 3 中 DxPopup 的 content-template / 默认插槽用法
2. **备注回显**（需后端配合）：
   - 后端 `PlatformUserRepDTO` 添加 `Remark` 字段
   - 后端 `MapToDto` 方法添加 `Remark = u.Remark`
   - 前端 `PlatformUserRepDTO` 类型添加 `Remark` 字段
   - `onEdit()` 中改为 `Remark: user.Remark ?? ''`
3. **详情弹窗显示备注**：在详情列表中添加备注行

**涉及文件**：
- `src/views/platform-users/PlatformUsersView.vue`
- `src/types/platformUser.ts`（添加 Remark 字段）
- `src/YTStdTenantPlatform/Application/Dtos/PlatformUser/PlatformUserRepDTO.cs`（后端，添加 Remark）
- `src/YTStdTenantPlatform/Application/Services/PlatformUserAppService.cs`（后端，MapToDto 添加 Remark）

**验收标准**：
- [ ] 用户详情弹窗内容正确展示在弹窗中（非页面底部）
- [ ] 新增用户时填写备注，保存后编辑可回显
- [ ] 编辑用户修改备注后保存，再次打开可看到最新备注
- [ ] 详情弹窗中显示备注字段
- [ ] `npm run build` 通过
- [ ] `dotnet build YTStd.slnx` 通过

---

##### Issue #4：批量启用/禁用必须依据后端接口实现

**现象**：批量启用/禁用功能需要严格按照后端 API 接口实现。

**根因分析**：
- 后端 `BatchUserIdsReqDTO` 定义 `Ids` 为 `long[]` 类型
- 前端 `batchEnablePlatformUsers` 发送 `{ Ids: ids }` 其中 ids 为 `number[]`
- 后端端点 `PUT /api/platform-users/batch-enable` 和 `PUT /api/platform-users/batch-disable` 已存在
- 需确认前端请求体字段名和类型与后端完全匹配

**修复方案**：
1. 核实前端发送的 JSON 字段名 `Ids` 与后端 `BatchUserIdsReqDTO.Ids` 匹配
2. 确认 HTTP 方法为 PUT（已正确）
3. 添加错误处理 — 部分 ID 操作失败时的反馈
4. 批量操作后刷新列表并清空选中状态（已实现）

**涉及文件**：
- `src/views/platform-users/PlatformUsersView.vue`
- `src/api/platformUsers.ts`

**验收标准**：
- [ ] 批量启用选中用户后状态正确变更
- [ ] 批量禁用选中用户后状态正确变更
- [ ] 操作失败时有错误提示
- [ ] 操作成功后列表刷新、选中状态清空

---

##### Issue #5：用户管理缺少角色权限管理

**现象**：用户管理页面没有角色分配功能，用户无法关联到角色。

**根因分析**：
- 后端通过 `POST /api/platform-roles/{id}/members` 从角色侧绑定用户，但用户侧没有直接的角色绑定入口
- 用户管理页面缺少"分配角色"操作按钮和弹窗
- 后端没有 `GET /api/platform-users/{id}/roles` 接口来获取用户已绑定的角色列表

**修复方案**：
1. 在用户操作列添加"分配角色"按钮
2. 实现角色分配弹窗：
   - 加载所有角色列表（`GET /api/platform-roles/all`）
   - 显示当前用户已绑定的角色（需要后端支持或从 roles-members 关系中反查）
   - 用 DxDataGrid 多选模式选择角色
   - 保存时调用角色绑定成员接口
3. 如后端无直接的"获取用户角色"接口，可通过遍历角色列表的成员关系来反查（或请求后端新增接口）
4. 添加对应的权限常量和权限检查

**涉及文件**：
- `src/views/platform-users/PlatformUsersView.vue`
- `src/api/platformUsers.ts`（可能需新增获取用户角色API）
- `src/api/platformRoles.ts`（已有 getAllPlatformRoles）
- `src/constants/permissions.ts`

**验收标准**：
- [ ] 用户操作列有"分配角色"按钮
- [ ] 点击后弹窗显示所有可选角色
- [ ] 已绑定的角色被预选中
- [ ] 保存后角色绑定生效
- [ ] `npm run build` 通过

---

#### G2-FIX-B：角色管理页面修复（PlatformRolesView）

##### Issue #6：角色管理缺少权限绑定实际功能

**现象**：角色管理仍是 demo 级别，权限绑定和权限修改功能不完善。

**根因分析**：
- 当前角色管理已有权限绑定弹窗和成员绑定弹窗
- 权限绑定使用 DxTreeList + 多选模式，预加载已绑定权限 IDs
- 但可能存在以下问题：
  - 权限树数据加载是否正确（`getPermissions()` 返回的是 tree 还是 flat？）
  - 绑定权限 API (`POST /api/platform-roles/{id}/permissions`) 是否正确调用
  - 权限修改后是否能正确反映在角色详情中
  - 成员绑定是否正确显示已有成员

**修复方案**：
1. 确认权限树加载使用 `GET /api/platform-permissions/tree` 而非 `GET /api/platform-permissions/`
2. 确认 DxTreeList 的 parent-id-expr 正确处理树形数据
3. 添加权限绑定后的即时反馈（显示当前已绑定权限数量）
4. 成员绑定弹窗应预加载已有成员（当前 `boundMemberIds` 初始化为空，需从后端获取）
5. 角色列表中增加"已绑定权限数"列或标签

**涉及文件**：
- `src/views/platform-roles/PlatformRolesView.vue`
- `src/api/platformRoles.ts`

**验收标准**：
- [ ] 点击"绑定权限"能正确加载权限树
- [ ] 已绑定的权限在树中被预选中
- [ ] 修改权限选择后保存，再次打开能看到更新后的状态
- [ ] 成员绑定弹窗预加载已有成员
- [ ] 权限绑定/解绑操作后有 Toast 反馈
- [ ] `npm run build` 通过

---

#### G2-FIX-C：权限管理页面修复（PlatformPermissionsView）

##### Issue #7：权限管理未读取后端数据

**现象**：权限管理页面标记为"权限查看"，但没有从后端读取到实际权限数据。

**根因分析**：
- 代码中 `onMounted(loadData)` 调用 `getPermissions()` → `GET /api/platform-permissions`
- `getPermissions()` API 函数确实存在，调用的是 `/api/platform-permissions` 端点
- 后端有 `GET /api/platform-permissions/` 和 `GET /api/platform-permissions/tree` 两个端点
- 需确认：
  1. API 请求是否成功发出
  2. 返回数据格式是否匹配 `PlatformPermissionRepDTO` 类型
  3. 是否有权限种子数据已初始化

**修复方案**：
1. 确认使用 `GET /api/platform-permissions/tree` 获取树形数据（而非 flat list）
2. 检查 `flattenTree` 方法是否正确将树形数据转为 DxTreeList 需要的 flat 数组
3. 如果后端返回的字段名大小写与前端 interface 不匹配，需要调整
4. 添加数据加载失败时的错误提示
5. 添加空数据状态提示（"暂无权限数据"）

**涉及文件**：
- `src/views/platform-permissions/PlatformPermissionsView.vue`
- `src/api/platformPermissions.ts`

**验收标准**：
- [ ] 权限树正确加载后端数据
- [ ] 树形层级关系正确展示
- [ ] 搜索过滤功能正常
- [ ] 空数据和加载失败有提示
- [ ] `npm run build` 通过

---

#### G2-FIX-D：安全设置页面修复（PlatformSecurityView）

##### Issue #8：安全设置无实际功能

**现象**：安全设置页面只显示硬编码的默认值，没有任何实际功能。

**根因分析**：
- `loadData()` 方法内部是空的：`// Security API will be integrated in a future phase`
- 所有安全策略数据来自 `reactive` 初始值（硬编码）
- 后端没有独立的安全策略管理端点
- 但后端有 `POST /api/auth/change-password` 端点

**修复方案**：
由于后端暂无安全策略 CRUD API，采取以下渐进方案：
1. **密码修改功能**：对接 `POST /api/auth/change-password`，在安全中心添加"修改密码"功能
2. **安全策略展示**：使用系统配置接口（如存在）读取密码策略配置，否则保持只读展示并明确标注"系统默认配置"
3. **明确标注**：对无后端支持的功能（IP白名单、MFA）标注"功能开发中"或"需在后端配置"
4. **移除空壳**：不要展示用户无法操作的功能项，或明确标注为展示模式

**涉及文件**：
- `src/views/platform-security/PlatformSecurityView.vue`
- `src/api/auth.ts`（可能需新增 changePassword）

**验收标准**：
- [ ] 修改密码功能可用（旧密码+新密码+确认密码）
- [ ] 密码策略展示有明确数据来源说明
- [ ] 未实现功能有明确的"开发中"标注
- [ ] `npm run build` 通过

---

## 修复执行顺序

1. **G1-FIX**（Issue #1 + #2）→ 人工验证登录页和导航
2. **G2-FIX-A**（Issue #3 + #4 + #5）→ 人工验证用户管理
3. **G2-FIX-B**（Issue #6）→ 人工验证角色管理
4. **G2-FIX-C**（Issue #7）→ 人工验证权限管理
5. **G2-FIX-D**（Issue #8）→ 人工验证安全设置
6. 所有修复验证通过后 → 继续 G3

## 后端修改清单（前端修复所需的后端配合）

| 修改 | 文件 | 说明 |
|------|------|------|
| PlatformUserRepDTO 添加 Remark 字段 | `Application/Dtos/PlatformUser/PlatformUserRepDTO.cs` | 添加 `public string? Remark { get; set; }` |
| MapToDto 映射 Remark | `Application/Services/PlatformUserAppService.cs` | `MapToDto` 方法添加 `Remark = u.Remark` |
