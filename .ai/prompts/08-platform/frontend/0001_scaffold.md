# 租户平台 — 前端工程脚手架（基于 DevExtreme Vue Application Template）

> 本文件定义新前端项目的脚手架搭建流程。
> **核心约束**：必须使用 DevExtreme Vue Application Template 生成项目骨架，而非从零手动搭建。

---

## 任务信息

| 属性 | 值 |
|------|---|
| 任务编号 | F0-1 |
| 所属阶段 | 层级 0：基础设施 |
| 依赖任务 | 无（首个任务） |
| 预计文件数 | 20+ 个（模板生成） |
| 新前端项目路径 | `src/WebTenantPlatfrom` |

---

## 前置阅读

- `.ai/prompts/03-frontend/00-governance.md` — 前端总治理
- `.ai/prompts/03-frontend/04-devextreme-templates.md` — DevExtreme 规范
- `.ai/rules/frontend.md` — 前端开发规范
- `.ai/context/tech-stack.md` — 技术栈约束
- `.github/copilot-instructions.md` — 关键编码约束（第 7-13 条为前端约束）

---

## DevExpress 文档查阅（强制前置步骤）

| 主题 | dxdocs 查阅问题 | 用途 |
|------|----------------|------|
| Application Template | `DevExtreme Vue Application Template generate new application vue-app CLI` | 了解模板生成命令 |
| Application Template Structure | `DevExtreme Vue Application Template project structure files layouts views` | 了解生成后的文件结构 |
| Add DevExtreme to Vue | `Add DevExtreme to Vue application npm install devextreme devextreme-vue` | 备选：手动添加 DevExtreme |

---

## 执行步骤

### 步骤 1：使用 DevExtreme CLI 生成项目

```bash
# 在 src/ 目录下生成 Vue 3 应用（使用 outer toolbar 布局）
cd src
npx devextreme-cli new vue-app WebTenantPlatfrom --version=3

# 或指定布局
npx devextreme-cli new vue-app WebTenantPlatfrom --version=3 --layout=side-nav-outer-toolbar
```

> **如果 `devextreme-cli` 无法使用**：
> 手动创建 Vue 3 + TypeScript + Vite 项目，然后安装 `devextreme` 和 `devextreme-vue`，
> 并手动创建与 Application Template 同构的目录结构（详见 `0010_layout.md` 第 1.2 节）。

### 步骤 2：安装额外依赖

```bash
cd src/WebTenantPlatfrom
npm install axios pinia vue-i18n vue-router@4
npm install -D sass
```

### 步骤 3：验证 Application Template 标准文件

确认以下 Application Template 核心文件存在：

| 文件 | 用途 | 必须 |
|------|------|:----:|
| `src/App.vue` | 根组件 | ✅ |
| `src/app-info.ts` | 应用元信息 | ✅ |
| `src/app-navigation.ts` | 导航菜单配置 | ✅ |
| `src/auth.ts` | 认证模块 | ✅ |
| `src/router.ts` | 路由配置 | ✅ |
| `src/theme-service.ts` | 主题服务 | ✅ |
| `src/layouts/side-nav-outer-toolbar.vue` | 主布局 | ✅ |
| `src/layouts/single-card.vue` | 登录卡片布局 | ✅ |
| `src/components/header-toolbar.vue` | 顶栏 | ✅ |
| `src/components/side-navigation-menu.vue` | 侧边栏菜单 | ✅ |
| `src/themes/metadata.base.json` | 基础主题 | ✅ |
| `src/variables.scss` | CSS 变量 | ✅ |

### 步骤 4：适配项目技术栈

1. 将 `.js` 文件转为 `.ts`（Application Template 默认生成 JS）
2. 集成 Pinia 状态管理
3. 集成 vue-i18n
4. 配置 axios（详见 `03-frontend/05-axios-standard.md`）
5. 保留旧项目 `web/tenant-platform-web` 不动

---

## 目标项目结构（Application Template + 扩展）

> 以下也是本任务"必须产出的文件"清单。标注"Application Template"的文件由 CLI 生成，标注"扩展"的文件需手动创建。

```
src/WebTenantPlatfrom/
├── src/
│   ├── main.ts
│   ├── App.vue
│   ├── app-info.ts              # Application Template
│   ├── app-navigation.ts        # Application Template
│   ├── auth.ts                  # Application Template
│   ├── router.ts                # Application Template
│   ├── theme-service.ts         # Application Template
│   ├── api/                     # axios 封装（扩展）
│   ├── components/              # Application Template + 自定义
│   │   ├── header-toolbar.vue
│   │   ├── side-navigation-menu.vue
│   │   ├── login-form.vue
│   │   ├── FunctionDescriptionCard.vue
│   │   └── OperationGuideDrawer.vue
│   ├── layouts/                 # Application Template
│   │   ├── side-nav-outer-toolbar.vue
│   │   ├── side-nav-inner-toolbar.vue
│   │   └── single-card.vue
│   ├── locales/                 # vue-i18n 资源（扩展）
│   ├── store/                   # Pinia 状态管理（扩展）
│   ├── styles/                  # 全局样式（扩展）
│   ├── themes/                  # Application Template
│   ├── types/                   # TypeScript 类型（扩展）
│   └── views/                   # 页面视图
├── public/
├── package.json
├── tsconfig.json
└── vite.config.ts
```

### 必须产出的文件（checklist）

| 序号 | 文件路径 | 来源 | 用途 |
|:----:|---------|------|------|
| 1 | `src/WebTenantPlatfrom/package.json` | CLI 生成 | 依赖管理 |
| 2 | `src/WebTenantPlatfrom/tsconfig.json` | CLI 生成 | TypeScript 配置 |
| 3 | `src/WebTenantPlatfrom/vite.config.ts` | CLI 生成 | Vite 构建配置 |
| 4 | `src/WebTenantPlatfrom/src/main.ts` | CLI 生成 / 手动 | 应用入口 |
| 5 | `src/WebTenantPlatfrom/src/App.vue` | CLI 生成 | 根组件 |
| 6 | `src/WebTenantPlatfrom/src/app-info.ts` | CLI 生成 | 应用元信息 |
| 7 | `src/WebTenantPlatfrom/src/app-navigation.ts` | CLI 生成 | 导航菜单配置 |
| 8 | `src/WebTenantPlatfrom/src/auth.ts` | CLI 生成 | 认证模块 |
| 9 | `src/WebTenantPlatfrom/src/router.ts` | CLI 生成 | 路由配置 |
| 10 | `src/WebTenantPlatfrom/src/theme-service.ts` | CLI 生成 | 主题服务 |
| 11 | `src/WebTenantPlatfrom/src/layouts/side-nav-outer-toolbar.vue` | CLI 生成 | 主布局 |
| 12 | `src/WebTenantPlatfrom/src/layouts/single-card.vue` | CLI 生成 | 登录卡片布局 |
| 13 | `src/WebTenantPlatfrom/src/components/header-toolbar.vue` | CLI 生成 | 顶栏组件 |
| 14 | `src/WebTenantPlatfrom/src/components/side-navigation-menu.vue` | CLI 生成 | 侧边栏菜单 |
| 15 | `src/WebTenantPlatfrom/src/themes/metadata.base.json` | CLI 生成 | 基础主题 |
| 16 | `src/WebTenantPlatfrom/src/variables.scss` | CLI 生成 | CSS 变量 |
| 17 | `src/WebTenantPlatfrom/src/api/` | 手动创建目录 | axios 封装（F0-2 填充） |
| 18 | `src/WebTenantPlatfrom/src/locales/` | 手动创建目录 | i18n 资源（F0-3 填充） |
| 19 | `src/WebTenantPlatfrom/src/store/` | 手动创建目录 | Pinia 状态管理 |
| 20 | `src/WebTenantPlatfrom/src/types/` | 手动创建目录 | TypeScript 类型 |
| 21 | `src/WebTenantPlatfrom/src/styles/` | 手动创建目录 | 全局样式 |

---

## 验收标准

### P0 — 功能点完整性

- [ ] 项目在 `src/WebTenantPlatfrom` 路径创建成功
- [ ] 基于 DevExtreme Vue Application Template 生成（或手动同构）
- [ ] Application Template 核心文件全部存在（见步骤 3 清单）
- [ ] `npm run dev` 能启动且显示 Application Template 默认页面
- [ ] `npm run build` 通过

### P1 — 技术栈集成

- [ ] Vue 3 + TypeScript + Vite 正确配置
- [ ] devextreme + devextreme-vue 正确安装
- [ ] axios 依赖已安装
- [ ] pinia 依赖已安装
- [ ] vue-i18n 依赖已安装
- [ ] vue-router@4 依赖已安装

### P2 — 旧项目保护

- [ ] `web/tenant-platform-web` 目录未被修改或删除

### P3 — 编译与质量

- [ ] `npm run build` 通过
- [ ] 无乱码字符
