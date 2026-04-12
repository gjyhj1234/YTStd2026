# 租户平台 — 前端工程脚手架

## 目标

校验租户平台前端工程现有结构。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/prompts/03-frontend/04-devextreme-templates.md`
- `.ai/context/tech-stack.md`
- `.github/copilot-instructions.md` — 关键编码约束（第 7-12 条为前端约束）

---

## DevExpress 文档查阅（强制前置步骤）

使用 DevExtreme 组件时，必须按照官方 dxdocs 工作流查阅文档（详见 `03-frontend/04-devextreme-templates.md` 第二节）：

1. **调用 `devexpress_docs_search`**（每个问题仅调用一次，使用 `technologies: ["Vue"]`）
2. **调用 `devexpress_docs_get_content`** 获取最相关帮助主题的全文
3. **反思内容**，提取可用的 API、属性、代码示例
4. **基于检索到的信息编码**，引用具体控件和属性名称

---

## 现有结构

```
web/tenant-platform-web/
├── src/
│   ├── main.ts
│   ├── App.vue
│   ├── api/             # API 封装
│   ├── assets/
│   ├── components/      # 通用组件
│   ├── composables/
│   ├── constants/       # 常量（errorCodes.ts）
│   ├── layouts/
│   ├── locales/         # 国际化资源
│   ├── mocks/           # MSW Mock
│   ├── router/
│   ├── store/           # Pinia
│   ├── types/           # TypeScript 类型
│   ├── utils/           # 工具（http.ts）
│   └── views/           # 页面
├── public/
└── package.json
```

---

## 校验要点

1. DevExtreme Vue 25.2 正确配置
2. HTTP 封装正确处理 `ApiResult<T>` 格式
3. TypeScript 类型使用 PascalCase（匹配后端）
4. 路由守卫正确

---

## 验收标准

- [ ] `npm run build` 通过
- [ ] 项目结构符合规范
