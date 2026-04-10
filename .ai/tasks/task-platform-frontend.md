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
| G3 — 租户生命周期模块 | ⬜ 待执行 | |
| G4 — 租户配置模块 | ⬜ 待执行 | |
| G5 — SaaS 套餐订阅 | ⬜ 待执行 | |
| G6 — 计费支付模块 | ⬜ 待执行 | |
| G7 — API 集成模块 | ⬜ 待执行 | |
| G8 — 运营日志模块 | ⬜ 待执行 | |
| G9 — 通知存储模块 | ⬜ 待执行 | |
| G10 — 基础设施系统设置 | ⬜ 待执行 | |
