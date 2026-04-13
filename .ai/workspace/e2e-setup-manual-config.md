# E2E 测试环境配置指南（人工操作清单）

## 概述

本文档列出了 E2E 测试基础设施中需要**仓库所有者手动完成**的配置项。
这些配置在 AI Agent 的权限范围之外，必须由人工操作。

---

## 一、必须手动完成的配置

### 1.1 合并 `copilot-setup-steps.yml` 到默认分支 ⚠️ 最关键

**原因：** `copilot-setup-steps.yml` 只有在默认分支（通常是 `main`）上存在时才会被 Copilot Agent 使用。

**操作：**
1. 将此 PR 合并到 `main` 分支
2. 确认 `.github/workflows/copilot-setup-steps.yml` 在 `main` 分支可见

**验证：**
- 进入仓库 Actions 页面，应看到 "Copilot Setup Steps" workflow
- 手动触发该 workflow（点击 "Run workflow"），确认所有步骤通过

---

### 1.2 验证 GitHub Actions workflow 运行通过

**操作：**
1. 合并 PR 后，进入仓库的 **Actions** 标签页
2. 找到 "Copilot Setup Steps" workflow
3. 点击 "Run workflow" 手动触发
4. 等待运行完成，确认所有步骤 ✅

**预期结果：**
- PostgreSQL 连接验证通过
- 后端构建和启动验证通过
- 前端依赖安装完成
- Playwright 浏览器安装完成

**如果失败：**
- 检查 `.NET 10 SDK` 是否可用（可能需要调整 `dotnet-quality` 参数）
- 检查 PostgreSQL 服务容器是否正常启动
- 检查网络访问（npm install 需要下载包）

---

### 1.3 防火墙设置（如启用了 Copilot Agent 防火墙）

**原因：** PostgreSQL Docker 镜像需要从 Docker Hub 拉取；npm 包需要从 npmjs.org 下载。

**操作（如果使用默认 GitHub-hosted runner，通常不需要）：**
- 确保未阻止 `docker.io` / `registry.hub.docker.com`
- 确保未阻止 `registry.npmjs.org`
- 确保未阻止 `nuget.org`（dotnet restore）

如果使用自托管 runner 或私有网络，需要配置防火墙允许这些域名。

---

## 二、可选配置

### 2.1 GitHub Actions 环境变量（可选）

如果需要为 Copilot Agent 设置环境变量（如自定义数据库密码），可以：

1. 进入仓库 **Settings** → **Environments**
2. 选择或创建 `copilot` 环境
3. 添加环境变量或 Secret

**当前无需配置**：数据库连接信息已硬编码在 Program.cs 和 copilot-setup-steps.yml 中。

### 2.2 使用更大的 Runner（可选）

如果 Agent 任务经常超时或需要更好的性能：

1. 在组织层级配置 Larger Runner
2. 修改 `copilot-setup-steps.yml` 中的 `runs-on` 为更大的 runner 标签

```yaml
# 示例：使用 4 核 runner
copilot-setup-steps:
  runs-on: ubuntu-4-core
```

---

## 三、配置状态检查清单

PR 合并后，请逐项确认：

- [ ] `copilot-setup-steps.yml` 在 `main` 分支可见
- [ ] "Copilot Setup Steps" workflow 手动触发运行通过
- [ ] PostgreSQL 连接验证步骤 ✅
- [ ] 后端启动验证步骤 ✅
- [ ] 前端依赖安装步骤 ✅
- [ ] Playwright 浏览器安装步骤 ✅

---

## 四、后续 Agent 任务的使用方式

配置完成后，您可以直接给 Copilot Agent 分配前端开发任务。Agent 会：

1. 自动获得 PostgreSQL 数据库环境（通过 services 容器）
2. 自动获得 .NET 10 SDK 和已构建的后端
3. 自动获得 Node.js 和已安装的前端依赖
4. 自动获得 Playwright 浏览器

**任务提示词示例：**

```markdown
请继续 S5 阶段前端开发。

## 上下文恢复

1. 先阅读 `.ai/workspace/session-summary-20260413-11.md`
2. 阅读 `.ai/prompts/08-platform/frontend/0000_overview.md`
3. 阅读 `.ai/system/e2e-testing-workflow.md`

## 本轮目标

实现 F2-1 仪表盘页面（`0020_dashboard-page.md`），完成后编写 Playwright E2E 测试并迭代通过。

## 环境

- PostgreSQL 已通过 copilot-setup-steps.yml 自动配置
- 启动后端：`dotnet run --project src/YTStdTenantPlatform -c Release`
- 启动前端：`cd src/WebTenantPlatfrom && npm run dev`
- 运行测试：`cd src/WebTenantPlatfrom && npx playwright test`

## 验证要求

1. `npm run build` 通过
2. Playwright E2E 测试全部通过
3. 代码搜索审查（F1-F7）通过
```

---

## 五、技术架构说明

```
┌──────────────────────────────────────────────────────────┐
│                   Copilot Agent Environment              │
│                                                          │
│  ┌─────────────────┐     ┌───────────────────────────┐  │
│  │   PostgreSQL 17  │←───│  YTStdTenantPlatform      │  │
│  │   localhost:5432 │     │  http://127.0.0.1:5000    │  │
│  │   db: test1      │     │  (.NET 10 AOT Backend)    │  │
│  │   user: postgres │     │  Auto: tables + seed data │  │
│  │   pass: gjwq1234 │     └───────────┬───────────────┘  │
│  └─────────────────┘                  │ /api             │
│                                       ▼                  │
│  ┌────────────────────────────────────────────────────┐  │
│  │   Vite Dev Server (WebTenantPlatfrom)              │  │
│  │   http://localhost:5173                            │  │
│  │   Proxy: /api → http://127.0.0.1:5000             │  │
│  └────────────────────────────────────────────────────┘  │
│                          ▲                               │
│                          │                               │
│  ┌────────────────────────────────────────────────────┐  │
│  │   Playwright (Chromium)                            │  │
│  │   Tests: e2e/tests/**/*.spec.ts                    │  │
│  │   Auth:  e2e/helpers/auth.setup.ts                 │  │
│  └────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────┘
```

---

## 版本

- 版本：1.0
- 创建日期：2026-04-13
