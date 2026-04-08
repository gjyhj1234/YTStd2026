# 集成测试提示词

## 目标

为后端 API 端点编写集成测试，验证完整的请求-响应流程。

---

## 适用范围

创建后端集成测试时使用。

---

## 前置阅读

- `.ai/rules/testing.md`
- `.ai/rules/api-design.md`

---

## 输入

- 已完成的 API 端点
- API 路由文档

---

## 输出

- `tests/{Project}.Tests/Integration/{Module}IntegrationTests.cs`

---

## 执行步骤

1. 创建测试基础设施（如需要）
2. 编写 API 端点的集成测试
3. 覆盖 HTTP 状态码验证
4. 覆盖 ApiResult 响应结构验证
5. 覆盖认证和权限场景
6. 执行 `dotnet test` 验证

---

## 测试场景

### 认证场景

- 未携带 Token → 401
- Token 过期 → 401
- Token 有效 → 通过

### 权限场景

- 无操作权限 → 403 / 权限错误码
- 有操作权限 → 正常处理

### 限流场景

- 正常请求 → 通过
- 超过限流 → 429

### 审计场景

- 关键操作触发审计记录

---

## 约束

- 集成测试在 `tests/{Project}.Tests/Integration/` 目录
- 可独立运行
- 不依赖外部服务

---

## 验收标准

- [ ] 覆盖认证、权限、限流场景
- [ ] HTTP 状态码正确
- [ ] `dotnet test` 通过
