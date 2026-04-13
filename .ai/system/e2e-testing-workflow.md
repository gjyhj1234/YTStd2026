# E2E 测试工作流协议

## 目标

定义 Agent 在前端开发任务中执行 Playwright E2E 测试的完整工作流，包括环境检查、服务启动、测试编写、迭代修复的闭环流程。

---

## 适用范围

所有涉及 `src/WebTenantPlatfrom` 前端页面开发的编码任务。

---

## 核心原则

> **编码 → 测试 → 修复 → 再测试，直到全部通过。**

E2E 测试是前端开发任务的闭环验收手段。Agent 在完成前端页面编码后，**必须**编写对应的 Playwright 测试并执行，根据测试结果迭代修复代码，直到所有测试通过。

---

## 一、环境预检（每次会话启动时必须执行）

Agent 在开始任何前端任务之前，必须按以下清单逐项检查环境：

### 检查清单

```bash
# ── 1. 检查 PostgreSQL 连接 ──
PGPASSWORD=gjwq1234 psql -h localhost -U postgres -d test1 -c "SELECT 1;" 2>/dev/null
# 如果失败，执行 PostgreSQL 手动启动（见"备用方案"节）

# ── 2. 检查后端是否已在运行 ──
curl -s http://127.0.0.1:5000/api/health/ | head -c 200
# 如果失败，启动后端

# ── 3. 检查前端依赖 ──
cd src/WebTenantPlatfrom && ls node_modules/.package-lock.json 2>/dev/null
# 如果失败，运行 npm install

# ── 4. 检查 Playwright 浏览器 ──
cd src/WebTenantPlatfrom && npx playwright --version 2>/dev/null
# 如果失败，运行 npx playwright install --with-deps chromium

# ── 5. 检查前端 dev server 是否已在运行 ──
curl -s http://localhost:5173/ | head -c 200
# 如果失败，启动前端 dev server
```

### 环境启动顺序（如需启动）

**必须按以下顺序启动服务：**

1. **PostgreSQL**（copilot-setup-steps 已通过 services 容器自动启动）
2. **后端**：`dotnet run --project src/YTStdTenantPlatform -c Release &`
   - 等待 health check 通过：`curl -s http://127.0.0.1:5000/api/health/`
   - 后端启动时自动建表和填充种子数据
3. **前端 dev server**：`cd src/WebTenantPlatfrom && npm run dev &`
   - Vite proxy 已配置，`/api` 请求自动转发到 `http://127.0.0.1:5000`
   - 等待就绪：`curl -s http://localhost:5173/`

### PostgreSQL 手动启动备用方案

如果 PostgreSQL 服务容器不可用，使用以下命令手动启动：

```bash
# 安装 PostgreSQL
sudo apt-get update && sudo apt-get install -y postgresql postgresql-client

# 启动服务
sudo systemctl start postgresql

# 设置密码
sudo -u postgres psql -c "ALTER USER postgres WITH PASSWORD 'gjwq1234';"

# 创建数据库
sudo -u postgres psql -c "CREATE DATABASE test1 OWNER postgres;"

# 验证
PGPASSWORD=gjwq1234 psql -h localhost -U postgres -d test1 -c "SELECT 1;"
```

---

## 二、数据库管理

### 何时需要重置数据库

| 场景 | 需要重置 | 说明 |
|------|:-------:|------|
| 首次启动后端 | ❌ | 后端自动建表和种子数据 |
| 测试产生脏数据 | ✅ | 测试创建的数据影响后续测试 |
| 修改实体结构后 | ✅ | 表结构变更需要重建 |
| 测试功能依赖前置操作 | ✅ | 需要从干净状态重新执行 |
| 单纯修改前端 UI | ❌ | 不影响数据库 |

### 数据库重置流程

```bash
# 1. 停止后端进程
kill $(lsof -t -i:5000) 2>/dev/null || true

# 2. 断开所有数据库连接并重建
PGPASSWORD=gjwq1234 psql -h localhost -U postgres -d postgres -c "
  SELECT pg_terminate_backend(pid)
  FROM pg_stat_activity
  WHERE datname = 'test1' AND pid <> pg_backend_pid();
"
PGPASSWORD=gjwq1234 psql -h localhost -U postgres -d postgres -c "DROP DATABASE IF EXISTS test1;"
PGPASSWORD=gjwq1234 psql -h localhost -U postgres -d postgres -c "CREATE DATABASE test1 OWNER postgres;"

# 3. 重新启动后端（自动建表 + 种子数据）
dotnet run --project src/YTStdTenantPlatform -c Release &

# 4. 等待 health check 通过
for i in $(seq 1 30); do
  curl -s http://127.0.0.1:5000/api/health/ | grep -q '"Code":0' && break
  sleep 1
done
```

---

## 三、E2E 测试编写规范

### 测试文件组织

```
src/WebTenantPlatfrom/e2e/
├── helpers/
│   ├── auth.setup.ts       # 认证设置（登录并保存状态）
│   ├── test-helpers.ts     # 通用工具函数
│   └── db-helpers.ts       # 数据库操作工具
├── tests/
│   ├── login/
│   │   └── login.noauth.spec.ts    # 登录页测试（不需要预认证）
│   ├── dashboard/
│   │   └── dashboard.spec.ts       # 仪表盘测试
│   ├── platform-users/
│   │   └── platform-users.spec.ts  # 平台用户管理测试
│   ├── platform-roles/
│   │   └── platform-roles.spec.ts  # 平台角色管理测试
│   └── ... （每个模块一个目录）
└── .auth/
    └── user.json           # Playwright 保存的认证状态（.gitignore）
```

### 测试文件命名规则

| 后缀 | 含义 | 需要预登录 |
|------|------|:---------:|
| `*.spec.ts` | 标准测试 | ✅（使用 auth-setup 的登录状态） |
| `*.noauth.spec.ts` | 无需认证的测试 | ❌（如登录页测试） |

### 每个模块的测试必须覆盖

| 测试类别 | 必须覆盖 | 说明 |
|---------|:-------:|------|
| 页面渲染 | ✅ | 页面标题、关键元素是否正确展示 |
| 数据列表 | ✅ | DxDataGrid 是否正确加载数据 |
| 创建操作 | ✅ | 新建表单提交是否成功、表单验证是否生效 |
| 编辑操作 | ✅ | 编辑弹窗是否正确回填数据、提交修改是否生效 |
| 删除操作 | ✅ | 删除确认弹窗、删除后列表更新 |
| 搜索/筛选 | ✅ | 搜索框、筛选条件是否生效 |
| 分页 | ✅ | 分页器是否正确工作（如有足够数据） |
| 表单验证 | ✅ | 必填字段、格式验证、唯一性提示 |
| 权限控制 | 按需 | 无权限时按钮是否隐藏/禁用 |
| i18n | 按需 | 切换语言后文本是否更新 |

### 测试编写模板

```typescript
import { test, expect } from '@playwright/test'
import {
  navigateTo,
  waitForGridLoaded,
  fillDxTextBox,
  clickDxButton,
  clickGridToolbarButton,
  waitForPopup,
  expectNotification,
  confirmDialog,
  getGridRowCount,
} from '../../helpers/test-helpers'

/**
 * {模块名} E2E 测试
 *
 * 测试模块：F2-X {模块名}（00XX_xxx-page.md）
 * 测试范围：{列出范围}
 *
 * 前置条件：
 *   - 后端已启动（http://127.0.0.1:5000）
 *   - 前端已启动（http://localhost:5173）
 *   - 已通过 auth.setup.ts 完成登录
 *   - {其他前置条件}
 */

test.describe('{模块名} — 页面渲染', () => {
  test.beforeEach(async ({ page }) => {
    await navigateTo(page, '/{route}')
    await waitForGridLoaded(page)
  })

  test('应正确展示页面标题', async ({ page }) => {
    await expect(page.locator('text={页面标题}')).toBeVisible()
  })

  test('应加载数据列表', async ({ page }) => {
    const rowCount = await getGridRowCount(page)
    expect(rowCount).toBeGreaterThanOrEqual(0)
  })
})

test.describe('{模块名} — CRUD 操作', () => {
  test('创建新记录', async ({ page }) => {
    await navigateTo(page, '/{route}')
    await clickGridToolbarButton(page, '新增')
    await waitForPopup(page)
    // ... 填写表单
    await clickDxButton(page, '保存')
    // ... 验证结果
  })
})
```

---

## 四、迭代修复工作流

### 标准循环

```
┌─────────────────────────────────────────────┐
│ 1. 编写/修改前端代码                          │
│ 2. 编写/更新 Playwright 测试                  │
│ 3. 确保后端运行中                             │
│ 4. 运行测试：npx playwright test              │
│ 5. 分析测试结果                               │
│    ├─ 全部通过 → 执行 self-review-protocol   │
│    └─ 有失败 → 分析失败原因                   │
│       ├─ 前端代码问题 → 修复代码 → 回到步骤 3  │
│       ├─ 测试代码问题 → 修复测试 → 回到步骤 3  │
│       └─ 数据问题 → 重置数据库 → 回到步骤 3   │
└─────────────────────────────────────────────┘
```

### 迭代规则

1. **最多迭代 5 次**：如果 5 次迭代仍有失败，停下来在会话总结中记录问题并请求人类介入
2. **每次迭代必须记录**：
   - 第 N 次迭代
   - 运行了哪些测试
   - 通过/失败数量
   - 失败的具体错误信息
   - 修复了什么
3. **不允许删除测试**：如果测试失败，只能修复前端代码或修正测试逻辑（不能删除测试用例）
4. **每次修复后先运行单个失败测试**，确认修复有效后再运行全量测试

### 运行测试命令

```bash
# 运行所有 E2E 测试
cd src/WebTenantPlatfrom && npx playwright test

# 运行指定模块
cd src/WebTenantPlatfrom && npx playwright test e2e/tests/login/

# 运行指定测试文件
cd src/WebTenantPlatfrom && npx playwright test e2e/tests/login/login.noauth.spec.ts

# 运行失败的测试（重试）
cd src/WebTenantPlatfrom && npx playwright test --last-failed

# 使用有头模式调试
cd src/WebTenantPlatfrom && npx playwright test --headed --project=no-auth

# 查看测试报告
cd src/WebTenantPlatfrom && npx playwright show-report e2e/playwright-report
```

---

## 五、依赖链处理

某些模块的测试依赖于其他模块的数据。当测试需要前置数据时，必须通过以下方式之一准备：

### 方式 1：API 直接创建（推荐）

在测试的 `beforeAll` 或 `beforeEach` 中，通过 API 直接创建所需数据：

```typescript
test.beforeAll(async ({ request }) => {
  // 登录获取 Token
  const loginRes = await request.post('/api/auth/login', {
    data: { Username: 'admin', Password: 'gjwq1234' }
  })
  const loginData = await loginRes.json()
  const token = loginData.Data.Token

  // 创建前置数据
  await request.post('/api/platform-roles', {
    headers: { Authorization: `Bearer ${token}` },
    data: { Name: '测试角色', Code: 'test-role', Status: 1 }
  })
})
```

### 方式 2：数据库重置 + 完整流程（当 API 不可用时）

```bash
# 1. 重置数据库
# 2. 重启后端
# 3. 从登录开始，依次执行所有前置操作
# 4. 最后执行当前模块的测试
```

### 模块依赖关系

```
登录 → 仪表盘 → 无依赖
                → 平台用户管理 → 无依赖
                → 平台角色管理 → 依赖权限数据
                → 平台权限管理 → 无依赖
                → 租户管理 → 无依赖
                → 租户信息管理 → 依赖租户数据
                → 租户资源管理 → 依赖租户数据
                → 租户配置管理 → 依赖租户数据
                → 套餐管理 → 无依赖
                → 订阅管理 → 依赖租户 + 套餐数据
                → 账单管理 → 依赖订阅数据
```

---

## 六、与现有审查协议的集成

E2E 测试是在 `self-review-protocol.md` 的代码搜索审查**之前**执行的。完整验收流程为：

```
编码 → npm run build → E2E 测试迭代 → 代码搜索审查（F1-F7）→ 修复违规 → 收尾
```

### 在会话总结中记录 E2E 结果

```
E2E 测试结果：✅ 全部通过
- 总测试数：N
- 通过：N
- 失败：0
- 跳过：0
- 迭代次数：M（如有修复迭代）
- 测试覆盖模块：{列出}
```

---

## 七、注意事项

1. **Hash 路由**：本项目使用 `createWebHashHistory()`，所有路由以 `/#/` 开头
2. **DevExtreme 组件**：DxTextBox、DxSelectBox 等组件的 input 元素可能需要特殊定位方式
3. **异步加载**：Vue 组件和 API 数据加载需要足够的等待时间
4. **种子数据**：后端启动时创建的管理员账号为 `admin / gjwq1234`
5. **RequirePasswordReset**：种子管理员的密码过期标记为 true，首次登录可能跳转到修改密码页
6. **Token 存储**：登录成功后 Token 存储在 `localStorage('auth_token')`
7. **API 代理**：前端 dev server 通过 Vite proxy 将 `/api` 转发到 `http://127.0.0.1:5000`

---

## 版本

- 版本：1.0
- 创建日期：2026-04-13
- 创建原因：实现前端开发的自动化 E2E 测试闭环，解决"编译通过但页面空白"类问题的检测能力不足
