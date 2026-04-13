# 测试规范

## 目标

定义后端和前端的测试标准，确保代码质量和回归安全。

---

## 适用范围

所有 `tests/` 下的测试项目和前端测试。

---

## 后端测试

### 测试框架

- 使用 xUnit
- 测试项目位于 `tests/{Project}.Tests/`
- 测试项目必须纳入 `YTStd.slnx` 解决方案

### 测试分类

| 类别 | 说明 | 命名 |
|------|------|------|
| 实体测试 | 验证实体定义和生成器产出 | `{Entity}EntityTests.cs` |
| 初始化测试 | 验证种子数据幂等性 | `InitializationTests.cs` |
| 基础设施测试 | 验证中间件、缓存等 | `{Component}Tests.cs` |
| 核心 API 测试 | 验证核心接口行为 | `{Module}ApiTests.cs` |
| 扩展 API 测试 | 验证扩展接口行为 | `{Module}ExtendedApiTests.cs` |
| 权限测试 | 验证权限控制 | `PermissionTests.cs` |

### 测试覆盖要求

每个 API 端点至少覆盖以下场景：

| 场景 | 必须覆盖 | 说明 |
|------|:-------:|------|
| 成功路径 | ✓ | 正常输入，预期成功 |
| 权限拒绝 | ✓ | 无权限时返回 403/权限错误码 |
| 输入验证失败 | ✓ | 缺少必填字段 |
| 唯一性冲突 | ✓ | 有唯一约束的字段 |
| 不存在的资源 | ✓ | 按 ID 查询不存在的记录 |
| 状态不允许 | 按需 | 状态流转限制 |
| 分页边界 | 按需 | 第一页、最后一页、超出范围 |

### 测试编写规则

```csharp
[Fact]
public async Task CreateUser_WithValidInput_ReturnsSuccess()
{
    // Arrange
    var dto = new CreatePlatformUserReqDTO { ... };

    // Act
    var result = await _service.CreateAsync(dto);

    // Assert
    Assert.Equal(0, result.Code);
    Assert.NotNull(result.Data);
}

[Fact]
public async Task CreateUser_WithDuplicateUsername_ReturnsError()
{
    // Arrange
    // ... 先创建一个用户
    var dto = new CreatePlatformUserReqDTO { Username = "existing" };

    // Act
    var result = await _service.CreateAsync(dto);

    // Assert
    Assert.Equal(ErrorCodes.UsernameExists, result.Code);
}
```

### CRUD 结果断言

```csharp
// 正确 — CRUD 结果使用 .Success
var dbResult = await DB.InsertAsync(entity);
Assert.True(dbResult.Success);

// 错误 — CRUD 结果没有 .Code（只有 ApiResult 有）
// Assert.Equal(0, dbResult.Code); // 编译错误
```

---

## 前端测试

### 测试要求

- 前端测试不做硬性覆盖率要求
- 优先保证路由、权限、API 封装的基本正确性
- 复杂业务逻辑需要单元测试

### 测试方式

- 组件测试：vitest + @vue/test-utils
- API Mock：MSW 2.x
- E2E 测试：Playwright（详见 `.ai/prompts/03-frontend/08-playwright-e2e.md`）

### E2E 测试（Playwright）

| 项目 | 说明 |
|------|------|
| 框架 | @playwright/test ^1.52.0 |
| 配置 | `src/WebTenantPlatfrom/playwright.config.ts` |
| 测试目录 | `src/WebTenantPlatfrom/e2e/tests/` |
| 工具函数 | `src/WebTenantPlatfrom/e2e/helpers/` |
| 工作流协议 | `.ai/system/e2e-testing-workflow.md` |
| 模块测试协议 | `.ai/prompts/08-platform/frontend/0040_e2e-testing-protocol.md` |

**E2E 测试执行前提：**
1. PostgreSQL 运行中（localhost:5432, db=test1）
2. 后端运行中（http://127.0.0.1:5000）
3. 前端 dev server 运行中（http://localhost:5173）或由 Playwright webServer 自动启动

---

## 测试执行命令

| 操作 | 命令 |
|------|------|
| 后端全量测试 | `dotnet test YTStd.slnx` |
| 后端指定项目 | `dotnet test tests/{Project}.Tests/` |
| 前端测试 | `cd web/{project} && npm run test` |
| **E2E 全量测试** | `cd src/WebTenantPlatfrom && npx playwright test` |
| **E2E 指定模块** | `cd src/WebTenantPlatfrom && npx playwright test e2e/tests/{module}/` |
| **E2E 无需登录** | `cd src/WebTenantPlatfrom && npx playwright test --project=no-auth` |
| **E2E 有头模式** | `cd src/WebTenantPlatfrom && npx playwright test --headed` |

---

## 测试时机

| 时机 | 必须测试 |
|------|:-------:|
| 创建新实体后 | ✓ |
| 创建新 API 后 | ✓ |
| 修改已有逻辑后 | ✓ |
| 修改中间件后 | ✓ |
| 修改权限逻辑后 | ✓ |
| 纯文档修改 | ✗ |
| 纯样式修改 | ✗ |

---

## 测试不通过的处理

1. 分析失败原因
2. 如果是代码 bug，修复后重新测试
3. 如果是测试本身问题，修正测试
4. 禁止删除或跳过已有测试
5. 如果确实需要修改测试，必须在会话总结中说明原因
