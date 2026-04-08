# {业务名称} — 前端工程脚手架

## 目标

初始化 {业务名称} 前端工程。

---

## 前置阅读

- `.ai/rules/frontend.md` — 前端开发规范
- `.ai/prompts/03-frontend/scaffold.md` — 前端脚手架通用规范
- `.ai/context/tech-stack.md` — 技术栈

---

## 输出

- `web/{business}-web/` — 前端项目
  - `package.json`
  - `vite.config.ts`
  - `tsconfig.json`
  - `src/main.ts`
  - `src/App.vue`
  - `src/utils/http.ts` — HTTP 封装
  - `src/types/base.ts` — 基础类型（ApiResult 等）
  - `src/router/index.ts` — 路由
  - `src/store/` — 状态管理

---

## 技术栈

- Vite + Vue 3 + TypeScript
- DevExtreme Vue
- vue-router + Pinia
- 原生 fetch 封装

---

## 验收标准

- [ ] `npm run build` 通过
- [ ] `npm run dev` 可启动
- [ ] 路由守卫正确
- [ ] HTTP 封装正确处理 ApiResult 格式
