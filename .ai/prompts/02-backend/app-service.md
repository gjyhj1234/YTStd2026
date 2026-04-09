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

---

## 执行步骤

1. 创建应用服务类
2. 实现查询方法（GetAsync、GetListAsync）
3. 实现创建方法（CreateAsync）— 必须包含 ID 生成和唯一性验证
4. 实现更新方法（UpdateAsync）— 必须包含存在性检查
5. 实现删除方法（DeleteAsync）
6. 实现状态变更方法（EnableAsync、DisableAsync）— 如适用
7. Update ErrorCodes
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
                return ApiResult.Fail<long>(ErrorCodes.UsernameExists);
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
            return ApiResult.Fail<long>(ErrorCodes.OperationFailed);
        }

        Logger.Debug(tenantId, userId, () => $"[CreateAsync] 用户创建成功: Id={entity.Id}");
        return ApiResult.Ok(entity.Id);
    }
}
```

---

## 约束

- 所有 `InsertAsync` 前**必须** `entity.Id = await DB.GetNextLongIdAsync()` — **此规则无任何例外，包括关联表、中间表（如 RolePermission、RoleMember、TagBinding）也必须遵守**
- 所有 `ApiResult.Fail()` 仅传 `ErrorCodes.XXX`（不传 message 参数）
- 所有 `Logger.Debug` 使用 `Func<string>` 重载
- 唯一性验证使用 `GetListAsync` + foreach 模式
- 返回值使用 `ApiResult<T>`
- **Create/Save 方法必须遵循唯一性双重校验模式**：
  1. **前置校验**：InsertAsync 前遍历现有数据检查唯一字段是否重复，重复时返回 `ErrorCodes.XxxExists`
  2. **后置复核**：InsertAsync 失败时重新查询判断是否唯一冲突，冲突时返回 `ErrorCodes.XxxExists` 而非笼统的 `ErrorCodes.XxxCreateFailed`
  3. 每个唯一字段必须有对应的 `ErrorCodes.XxxExists` 错误码（位于 18xxx 段）
  4. 详见 `.ai/rules/backend.md` 中的"唯一性双重校验模式"

---

## 禁止事项

- 禁止使用 LINQ
- 禁止使用反射
- 禁止硬编码中文错误消息
- 禁止省略 ID 生成步骤（任何实体的 InsertAsync 都不例外）
- 禁止在 InsertAsync 调用时 entity.Id 为 0 或默认值

---

## 验收标准

- [ ] 服务方法签名包含 `tenantId` 和 `userId`
- [ ] 所有创建操作有 ID 生成 — **使用 `grep -B 15 "InsertAsync"` 搜索验证，逐一确认每处 InsertAsync 前有 GetNextLongIdAsync**
- [ ] 所有唯一字段有验证 — **前置校验 + 后置复核双重模式，使用 `grep -B 30 "InsertAsync"` 和 `grep -A 10 "!insResult.Success"` 验证**
- [ ] 每个唯一字段有对应的 `ErrorCodes.XxxExists` 错误码（18xxx 段）
- [ ] InsertAsync 失败时不返回笼统的 `XxxCreateFailed`，而是通过后置复核返回精确的 `XxxExists`
- [ ] 错误码常量已定义
- [ ] Debug 日志使用委托重载
- [ ] 编译通过
- [ ] **代码搜索审查通过**（按 `.ai/system/self-review-protocol.md` 执行，含审查项 8 唯一性校验）
