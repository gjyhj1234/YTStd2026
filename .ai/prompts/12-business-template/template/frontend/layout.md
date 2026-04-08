# {业务名称} — 布局与导航

## 目标

实现 {业务名称} 前端布局组件和导航菜单。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/prompts/03-frontend/component.md`

---

## 输出

- `src/layouts/MainLayout.vue` — 主布局
- `src/components/Sidebar.vue` — 侧边栏
- `src/components/Topbar.vue` — 顶栏
- `src/components/Breadcrumb.vue` — 面包屑

---

## 布局结构

```
┌──────────────────────────────────────┐
│  Topbar                              │
├────────┬─────────────────────────────┤
│        │                             │
│ Sidebar│     Main Content            │
│        │                             │
│        │                             │
└────────┴─────────────────────────────┘
```

---

## 验收标准

- [ ] 布局组件正确
- [ ] 侧边栏可折叠
- [ ] 菜单权限过滤正确
- [ ] `npm run build` 通过
