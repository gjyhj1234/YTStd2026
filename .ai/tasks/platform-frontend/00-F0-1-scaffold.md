# 子任务 F0-1 — 脚手架搭建

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 00-F0-1 |
| 模块名称 | 脚手架搭建（DevExtreme Vue Application Template） |
| 并行组 | — （串行，基础设施首个任务） |
| 对应提示词 | `.ai/prompts/08-platform/frontend/0001_scaffold.md` |
| 后端 API 提示词 | 无 |
| 依赖任务 | 无（首个任务） |
| 完成会话 | `session-summary-20260413-09` |
| 状态 | ✅ 已完成 |

---

## 任务目标

使用 DevExtreme CLI 生成 Vue 3 前端项目脚手架（`src/WebTenantPlatfrom`），安装额外依赖（axios、pinia、vue-i18n），将 JS 文件转为 TS，配置 TypeScript 严格模式，确保 `npm run build` 通过。

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0001_scaffold.md` — 脚手架搭建流程定义
- `.ai/prompts/03-frontend/04-devextreme-templates.md` — DevExtreme 模板规范
- `.ai/context/tech-stack.md` — 技术栈约束

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/package.json` | 依赖管理 |
| `src/WebTenantPlatfrom/tsconfig.json` | TypeScript 主配置 |
| `src/WebTenantPlatfrom/tsconfig.app.json` | 应用 TS 配置 |
| `src/WebTenantPlatfrom/tsconfig.node.json` | Node TS 配置 |
| `src/WebTenantPlatfrom/vite.config.ts` | Vite 构建配置 |
| `src/WebTenantPlatfrom/env.d.ts` | TypeScript 环境声明 |
| `src/WebTenantPlatfrom/.env.development` | 开发环境变量 |
| `src/WebTenantPlatfrom/.env.production` | 生产环境变量 |
| `src/WebTenantPlatfrom/src/main.ts` | 应用入口（Pinia + vue-i18n 集成） |
| `src/WebTenantPlatfrom/src/App.vue` | 根组件 |
| `src/WebTenantPlatfrom/src/app-info.ts` | 应用元信息 |
| `src/WebTenantPlatfrom/src/app-navigation.ts` | 导航菜单配置 |
| `src/WebTenantPlatfrom/src/auth.ts` | 认证模块 |
| `src/WebTenantPlatfrom/src/router.ts` | 路由配置 |
| `src/WebTenantPlatfrom/src/theme-service.ts` | 主题服务 |
| `src/WebTenantPlatfrom/src/utils/media-query.ts` | 响应式断点 |
| `src/WebTenantPlatfrom/src/layouts/side-nav-outer-toolbar.vue` | 主布局 |
| `src/WebTenantPlatfrom/src/layouts/single-card.vue` | 登录卡片布局 |
| `src/WebTenantPlatfrom/src/components/header-toolbar.vue` | 顶栏组件 |
| `src/WebTenantPlatfrom/src/components/side-navigation-menu.vue` | 侧边栏菜单 |
| `src/WebTenantPlatfrom/src/api/` | axios 封装目录（F0-2 填充） |
| `src/WebTenantPlatfrom/src/locales/` | i18n 资源目录（F0-3 填充） |
| `src/WebTenantPlatfrom/src/store/` | Pinia 状态管理目录 |
| `src/WebTenantPlatfrom/src/types/` | TypeScript 类型目录 |
| `src/WebTenantPlatfrom/src/styles/` | 全局样式目录 |

---

## 验收标准

- [x] 项目在 `src/WebTenantPlatfrom` 路径创建成功
- [x] 基于 DevExtreme Vue Application Template 生成（`devextreme-cli`）
- [x] Application Template 核心文件全部存在
- [x] 额外依赖已安装：axios、pinia、vue-i18n、typescript、vue-tsc
- [x] JS → TS 全量转换完成（7 个文件）
- [x] `npm run build` 通过
- [x] 旧项目 `web/tenant-platform-web` 未被修改

---

## 已完成说明

本子任务已在 `session-summary-20260413-09` 中完成。主要产出：

1. 使用 `npx devextreme-cli new vue-app WebTenantPlatfrom --version=3` 生成项目
2. 安装额外依赖：axios@1.15.0、pinia@3.0.4、vue-i18n@11.3.2、typescript@6.0.2、vue-tsc@3.2.6
3. JS → TS 全量转换（main、router、auth、app-info、app-navigation、theme-service、media-query）
4. 创建 TypeScript 配置（tsconfig.json + tsconfig.app.json + tsconfig.node.json）
5. 创建环境变量文件（.env.development、.env.production）
6. 修复模板 demo 页面 `tasks-page.vue` 的 `fetch` 调用为 `axios`
