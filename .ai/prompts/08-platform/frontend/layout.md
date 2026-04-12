# 租户平台 — 前端布局与导航

## 目标

实现和完善前端布局组件和导航，确保侧边栏、登录页、顶栏的交互行为正确。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/prompts/03-frontend/component.md`
- `.github/copilot-instructions.md` — 关键编码约束（第 7-12 条为前端约束）

---

## DevExpress 文档查阅（强制前置步骤）

在开始编码前，必须通过 `dxdocs` MCP 工具查阅以下组件：

```
devexpress_docs_search(technologies: ["Vue"], question: "DxDrawer opened-state-mode shrink reveal-mode expand position left")
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeView item template customization selectByClick focusStateEnabled activeStateEnabled CSS")
devexpress_docs_search(technologies: ["Vue"], question: "DxForm label-mode static floating browser autofill")
devexpress_docs_search(technologies: ["Vue"], question: "DxSelectBox value-changed event items display-expr value-expr")
```

---

## 布局结构

```
┌──────────────────────────────────────────────┐
│  Topbar（Logo、搜索、通知、用户信息、语言切换）  │
├────────────┬─────────────────────────────────┤
│            │  Breadcrumb                      │
│  Sidebar   ├─────────────────────────────────┤
│  (菜单树)  │                                 │
│            │  Main Content                    │
│            │  （FunctionDescriptionCard +     │
│            │    业务内容 + OperationGuide）    │
│            │                                 │
└────────────┴─────────────────────────────────┘
```

---

## 登录页（LoginView.vue）关键约束

1. **DxForm label-mode 必须使用 `"static"`**（不使用 `"floating"`）
   - 原因：浏览器自动填充用户名/密码时，DevExtreme floating label 不感知填充值，导致 label 与值重叠
   - 这是 DevExtreme 的已知行为，无法通过 CSS 修复
2. 表单验证：用户名 required，密码 required + minLength 6
3. 登录失败使用 `notifyError`，登录成功跳转到 redirect 参数或 /dashboard

---

## 侧边栏（MainLayout.vue）关键约束

1. **使用 `DxDrawer` + `DxTreeView`** 实现侧边栏
2. **DxTreeView 选中态样式**：必须确保点击子菜单后不出现靠左对齐偏移
   - 必须通过 dxdocs 查阅 DxTreeView 的 CSS 定制方案
   - 需要覆盖 `.dx-treeview-item` 和 `.dx-state-selected` 的 padding/margin 样式
   - 自定义 `#item` 模板时，需确保模板内容的布局不被 DevExtreme 内部选中机制影响
3. **菜单按权限过滤**：通过 `authStore.hasAnyPermission()` 过滤
4. **折叠态**：折叠时隐藏文本，仅显示图标
5. **语言切换**：切换语言后菜单文本必须实时更新（通过访问 `locale.value` 触发响应式）

---

## 菜单结构

菜单树与 `docs/TenantPlatform/architecture.md` 功能模块一致：

- 首页/仪表盘
- 平台管理
  - 用户管理
  - 角色管理
  - 权限管理
  - 安全设置
- 租户管理
  - 租户列表
  - 租户信息
  - 资源配额
  - 配置与开关
- SaaS 运营
  - 套餐管理
  - 订阅管理
  - 账单管理
- API 集成
- 系统管理
  - 菜单管理
  - 字典管理
  - 审计日志
  - 通知管理
  - 文件管理

---

## 验收标准

- [ ] 布局组件正确渲染
- [ ] 侧边栏可折叠，折叠时仅显示图标
- [ ] 菜单按权限过滤
- [ ] **点击子菜单后不出现靠左对齐偏移**（DxTreeView CSS 验证）
- [ ] **登录页 DxForm 使用 `label-mode="static"`**
- [ ] `grep -rn 'label-mode="floating"' LoginView.vue` 结果为 0
- [ ] 语言切换后所有菜单文本更新
- [ ] `npm run build` 通过
