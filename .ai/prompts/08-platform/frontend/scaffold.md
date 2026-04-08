# 租户平台 — 前端工程脚手架

## 目标

校验租户平台前端工程现有结构。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.ai/prompts/03-frontend/scaffold.md`
- `.ai/context/tech-stack.md`

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
