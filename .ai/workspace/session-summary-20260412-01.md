# Session Summary: 组件级语言文件自动加载修复

## 日期
2026-04-12

## 问题描述
组件级语言文件（`*.vue.{locale}.json`）未被加载到 vue-i18n 中，修改这些文件后界面不生效。实际生效的翻译来源是 `src/locales/zh-CN.json`（主语言文件），因为组件特有的扁平中文 key 全部重复存在于主语言文件中。

## 根因分析
`src/locales/index.ts` 仅合并了 `common + generated + enums + 主语言文件`，未加载任何 `*.vue.{locale}.json` 组件级文件。组件级文件虽然创建了但从未被 import，是死代码。

## 修复方案

### 1. 前端代码修改（`src/locales/index.ts`）
- 使用 Vite 的 `import.meta.glob` 自动收集 `views/**`、`components/**`、`layouts/**` 下的 `*.vue.{locale}.json` 文件
- 实现 `mergeComponentMessages()` 函数，遍历所有组件级模块，仅收集非 null 值
- 在消息合并中将组件级消息放在最后（最高优先级）
- 合并优先级（从低到高）：common → generated → enums → 主语言文件 → 组件级文件

### 2. 主语言文件清理
- 从 5 个主语言文件（zh-CN/en-US/ms-MY/zh-TW/ja-JP）中移除 151 个已被组件文件或 common 覆盖的扁平中文 key
- 主语言文件仅保留嵌套结构键（app、languages、common、menu、route、components、status、enum）

### 3. 组件文件修复
- DashboardView：将 7 个 null 键改为实际值（总租户数、活跃租户等，这些不在 common 中）
- MainLayout：将 3 个 null 键改为实际值（租户管理平台、退出、语言）

### 4. 提示词更新
- `.ai/prompts/05-i18n/frontend-i18n.md`：添加完整的 `import.meta.glob` 实现代码模板，可直接复用到其他业务工程
- `.ai/prompts/08-platform/frontend/i18n.md`：更新执行步骤和验收标准
- `.ai/rules/i18n.md`：重写目录结构（含优先级列）、组件级自动加载机制、回退策略、开发流程
- `.github/copilot-instructions.md`：添加规则 13（组件级语言文件自动加载）

## 修改文件清单
- `web/tenant-platform-web/src/locales/index.ts` — 核心修改
- `web/tenant-platform-web/src/locales/{zh-CN,en-US,ms-MY,zh-TW,ja-JP}.json` — 移除覆盖键
- `web/tenant-platform-web/src/layouts/MainLayout.vue.{5个locale}.json` — null → 值
- `web/tenant-platform-web/src/views/dashboard/DashboardView.vue.{5个locale}.json` — null → 值
- `.ai/prompts/05-i18n/frontend-i18n.md` — 添加实现模板
- `.ai/prompts/08-platform/frontend/i18n.md` — 更新步骤
- `.ai/rules/i18n.md` — 架构重写
- `.github/copilot-instructions.md` — 添加规则 13

## 验证
- `npm run build` 通过
- 所有 null 键在 common 中有对应翻译

## 关键设计决策
1. 使用 `import.meta.glob` 而非手动 import：自动发现新组件文件，无需修改 index.ts
2. null 值过滤：组件文件中 null 表示"由 common 提供"，不覆盖上层
3. 主语言文件仅保留嵌套键：扁平中文 key 必须在组件文件中，便于人工维护
