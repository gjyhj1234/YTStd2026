# 单元测试提示词

## 目标

为后端应用服务和业务逻辑编写单元测试。

---

## 适用范围

创建后端单元测试时使用。

---

## 前置阅读

- `.ai/rules/testing.md`
- `.ai/rules/backend.md`

---

## 输入

- 已完成的应用服务
- 业务规则描述

---

## 输出

- `tests/{Project}.Tests/{Module}Tests.cs`

---

## 执行步骤

1. 创建测试类
2. 编写成功路径测试
3. 编写唯一性冲突测试
4. 编写输入验证失败测试
5. 编写权限拒绝测试
6. 编写不存在资源测试
7. 执行 `dotnet test` 验证

---

## 测试模式

```csharp
public class PlatformUserApiTests
{
    [Fact]
    public async Task CreateUser_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var dto = new CreatePlatformUserReqDTO
        {
            Username = "testuser",
            DisplayName = "测试用户"
        };

        // Act
        var result = await _service.CreateAsync(0, 1, dto);

        // Assert
        Assert.Equal(0, result.Code);
        Assert.True(result.Data > 0);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateUsername_ReturnsError()
    {
        // Arrange - 先创建一个用户
        // ...

        // Act - 使用相同用户名再创建
        var result = await _service.CreateAsync(0, 1, duplicateDto);

        // Assert
        Assert.Equal(ErrorCodes.UsernameExists, result.Code);
    }
}
```

---

## 覆盖要求

每个 API 端点至少覆盖：
- 成功路径
- 权限拒绝
- 输入验证失败
- 唯一性冲突（有唯一索引的字段）
- 资源不存在

---

## 约束

- 使用 xUnit 框架
- CRUD 结果使用 `.Success` 断言
- ApiResult 结果使用 `.Code` 断言
- 测试可独立运行

---

## 验收标准

- [ ] 覆盖所有必需场景
- [ ] `dotnet test` 全部通过
- [ ] 断言方法正确（.Success vs .Code）
