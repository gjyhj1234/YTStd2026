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
- `.github/copilot-instructions.md` — 关键编码约束（第 7-12 条为前端约束）

---

## DevExpress 文档查阅（强制前置步骤）

在开始搭建脚手架前，必须按照官方 dxdocs 工作流查阅相关组件文档：

**工作流（每个 DevExtreme 组件必须执行）：**

1. **调用 `devexpress_docs_search`**（每个问题仅调用一次，使用 `technologies: ["Vue"]`）
2. **调用 `devexpress_docs_get_content`** 获取最相关帮助主题的全文
3. **反思内容**，提取可用的 API、属性、代码示例
4. **基于检索到的信息编码**，引用具体控件和属性名称

**本任务必须查阅的组件：**

```
devexpress_docs_search(technologies: ["Vue"], question: "DxDrawer opened-state-mode reveal-mode position")
devexpress_docs_search(technologies: ["Vue"], question: "DxTreeView item template customization selectByClick")
devexpress_docs_search(technologies: ["Vue"], question: "DxForm label-mode static floating")
devexpress_docs_search(technologies: ["Vue"], question: "DevExtreme Vue theme CSS variables")
```

查阅后必须调用 `devexpress_docs_get_content` 获取全文，阅读其中的代码示例并在编码中参考。

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
│   │   ├── generated/    # Generator 生成（Code→文本映射）
│   │   ├── common/       # 全局复用资源
│   │   ├── runtime/      # t()/gt()/loader
│   │   │   ├── t.ts
│   │   │   ├── gt.ts
│   │   │   └── loader.ts
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
11. 创建国际化基础结构（common/ + runtime/）
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
