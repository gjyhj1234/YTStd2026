# 前端脚手架提示词

## 目标

创建前端工程骨架，包含构建配置、路由、布局、认证、HTTP 封装等基础能力。

---

## 适用范围

新建前端项目或重建前端骨架时使用。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/rules/naming.md`
- `.ai/rules/i18n.md`
- `.ai/context/tech-stack.md`

---

## 输入

- 项目名称
- 后端 API 基础 URL
- 菜单结构（来自架构文档）

---

## 输出

```
web/{project}/
├── src/
│   ├── main.ts
│   ├── App.vue
│   ├── api/              # API 封装基础
│   ├── components/        # 通用组件
│   │   ├── FunctionDescriptionCard.vue
│   │   └── OperationGuideDrawer.vue
│   ├── constants/         # 常量
│   │   ├── permissions.ts
│   │   └── errorCodes.ts
│   ├── layouts/           # 布局
│   │   └── MainLayout.vue
│   ├── locales/           # 国际化
│   │   ├── zh-CN.json
│   │   ├── en-US.json
│   │   └── index.ts
│   ├── router/            # 路由
│   │   └── index.ts
│   ├── store/             # 状态管理
│   │   ├── auth.ts
│   │   └── app.ts
│   ├── types/             # 类型定义
│   │   └── base.ts
│   ├── utils/             # 工具
│   │   └── http.ts
│   └── views/             # 页面
│       ├── login/
│       │   └── LoginView.vue
│       └── dashboard/
│           └── DashboardView.vue
├── public/
├── index.html
├── package.json
├── vite.config.ts
└── tsconfig.json
```

---

## 执行步骤

1. 创建 Vite + Vue 3 + TypeScript 项目
2. 安装依赖：devextreme-vue、vue-router、pinia、vue-i18n
3. 配置 TypeScript 严格模式
4. 实现 HTTP 封装（统一错误处理、Token 注入）
5. 实现基础类型（ApiResult、PagedResult）
6. 实现路由和权限守卫
7. 实现主布局（侧边栏、顶栏、面包屑）
8. 实现登录页和认证状态管理
9. 实现通用组件（FunctionDescriptionCard、OperationGuideDrawer）
10. 实现首页/仪表盘
11. 创建国际化基础资源（zh-CN、en-US）
12. 执行 `npm run build` 验证

---

## 约束

- 使用 DevExtreme Vue 组件
- TypeScript 严格模式
- API 类型使用 PascalCase 匹配后端
- `code=0` 表示成功

---

## 禁止事项

- 禁止引入其他 UI 组件库
- 禁止引入 axios（使用原生 fetch 封装）
- 禁止使用 jQuery

---

## 验收标准

- [ ] `npm run build` 通过
- [ ] 登录页可正常渲染
- [ ] 主布局结构完整
- [ ] HTTP 封装支持 Token 和错误处理
- [ ] 路由守卫正常工作
- [ ] 通用组件可正常使用
