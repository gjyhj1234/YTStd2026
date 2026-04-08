# 应用服务提示词

## 目标

为指定模块实现应用服务层，包含 CRUD 操作、业务规则验证和错误处理。

---

## 适用范围

创建或修改应用服务时使用。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/global.md`
- `.ai/rules/api-design.md`
- `.ai/context/existing-modules.md`

---

## 输入

- 已完成的实体定义
- 已完成的 DTO 定义
- 业务规则描述

---

## 输出

- `Application/Services/{Module}AppService.cs`
- `Application/Constants/ErrorCodes.cs` 更新
- `Application/Constants/Messages.cs` 更新

---

## 执行步骤

1. 创建应用服务类
2. 实现查询方法（GetAsync、GetListAsync）
3. 实现创建方法（CreateAsync）— 必须包含 ID 生成和唯一性验证
4. 实现更新方法（UpdateAsync）— 必须包含存在性检查
5. 实现删除方法（DeleteAsync）
6. 实现状态变更方法（EnableAsync、DisableAsync）— 如适用
7. 更新 ErrorCodes 和 Messages
8. 编写 Logger.Debug 日志（使用 Func<string> 重载）
9. 执行 `dotnet build` 验证

---

## 服务实现模式

```csharp
/// <summary>
/// 平台用户应用服务
/// </summary>
public class PlatformUserAppService
{
    /// <summary>
    /// 创建平台用户
    /// </summary>
    public async Task<ApiResult<long>> CreateAsync(int tenantId, long userId, CreatePlatformUserReqDTO dto)
    {
        Logger.Debug(tenantId, userId, () => $"[CreateAsync] 开始创建用户: {dto.Username}");

        // 1. 唯一性验证
        var existingList = await DB.GetListAsync<PlatformUser>(query);
        foreach (var item in existingList)
        {
            if (item.Username == dto.Username)
            {
                return ApiResult.Fail<long>(ErrorCodes.UsernameExists, Messages.UsernameExists);
            }
        }

        // 2. 构建实体
        var entity = new PlatformUser();
        entity.Id = await DB.GetNextLongIdAsync();  // 必须显式获取 ID
        entity.Username = dto.Username;
        entity.CreatedAt = DateTimeOffset.UtcNow;

        // 3. 插入
        var result = await DB.InsertAsync(entity);
        if (!result.Success)
        {
            return ApiResult.Fail<long>(ErrorCodes.OperationFailed, Messages.OperationFailed);
        }

        Logger.Debug(tenantId, userId, () => $"[CreateAsync] 用户创建成功: Id={entity.Id}");
        return ApiResult.Ok(entity.Id);
    }
}
```

---

## 约束

- 所有 `InsertAsync` 前必须 `entity.Id = await DB.GetNextLongIdAsync()`
- 所有错误消息使用 `Messages.XXX` 整形常量
- 所有 `Logger.Debug` 使用 `Func<string>` 重载
- 唯一性验证使用 `GetListAsync` + foreach 模式
- 返回值使用 `ApiResult<T>`

---

## 禁止事项

- 禁止使用 LINQ
- 禁止使用反射
- 禁止硬编码中文错误消息
- 禁止省略 ID 生成步骤

---

## 验收标准

- [ ] 服务方法签名包含 `tenantId` 和 `userId`
- [ ] 所有创建操作有 ID 生成
- [ ] 所有唯一字段有验证
- [ ] 错误码和消息常量已定义
- [ ] Debug 日志使用委托重载
- [ ] 编译通过
