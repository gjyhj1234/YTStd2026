import { defineConfig, devices } from '@playwright/test'

/**
 * Playwright E2E 测试配置
 *
 * 使用方式：
 *   npx playwright test                    # 运行所有测试
 *   npx playwright test e2e/tests/login    # 运行指定目录
 *   npx playwright test --headed           # 有头模式（调试）
 *   npx playwright test --ui               # 交互式 UI 模式
 *
 * 前提条件：
 *   1. PostgreSQL 运行中（localhost:5432, db=test1）
 *   2. 后端运行中（http://127.0.0.1:5000）
 *   3. 前端运行中（http://localhost:5173）或由 webServer 自动启动
 */
export default defineConfig({
  // 测试文件目录
  testDir: './e2e/tests',

  // 测试结果输出
  outputDir: './e2e/test-results',

  // 全局超时：单个测试 60 秒
  timeout: 60_000,

  // expect 断言超时
  expect: {
    timeout: 10_000,
  },

  // 完整重试：CI 中重试 1 次，本地不重试
  retries: process.env.CI ? 1 : 0,

  // 并行度：E2E 测试串行执行避免数据冲突
  fullyParallel: false,
  workers: 1,

  // 报告器
  reporter: [
    ['list'],
    ['html', { outputFolder: './e2e/playwright-report', open: 'never' }],
  ],

  // 全局设置：所有测试共享的配置
  use: {
    // 基础 URL
    baseURL: 'http://localhost:5173',

    // 浏览器行为
    headless: true,
    viewport: { width: 1280, height: 720 },
    ignoreHTTPSErrors: true,

    // 调试辅助
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    trace: 'retain-on-failure',

    // 操作超时
    actionTimeout: 10_000,
    navigationTimeout: 15_000,
  },

  // 项目配置：仅使用 Chromium
  projects: [
    // 认证状态设置（登录并保存状态）
    {
      name: 'auth-setup',
      testDir: './e2e/helpers',
      testMatch: /auth\.setup\.ts/,
      use: {
        ...devices['Desktop Chrome'],
      },
    },
    // 主测试项目（依赖 auth-setup 的登录状态）
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        storageState: './e2e/.auth/user.json',
      },
      dependencies: ['auth-setup'],
      testIgnore: /.*\.noauth\.spec\.ts/,
    },
    // 无需登录的测试（如登录页本身）
    {
      name: 'no-auth',
      use: {
        ...devices['Desktop Chrome'],
      },
      testMatch: /.*\.noauth\.spec\.ts/,
    },
  ],

  // Web Server 配置：自动启动前端 dev server
  // 注意：后端需要由 Agent 手动启动（因为需要 dotnet run）
  webServer: {
    command: 'npm run dev',
    url: 'http://localhost:5173',
    reuseExistingServer: true,
    timeout: 30_000,
  },
})
