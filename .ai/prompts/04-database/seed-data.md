# 初始化数据提示词

## 目标

为指定模块创建幂等的初始化数据（种子数据），通过实体和 CRUD 能力写入数据库。

---

## 适用范围

创建或修改初始化数据时使用。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/global.md`
- `.ai/context/existing-modules.md`

---

## 输入

- 已完成的实体和 CRUD 能力
- 业务需求（需要哪些初始数据）

---

## 输出

- `Infrastructure/Initialization/Contributors/{Module}SeedContributor.cs`
- `Infrastructure/Initialization/SeedData/{Module}SeedData.cs`（可选）

---

## 执行步骤

1. 创建 SeedContributor 类实现 `ISeedContributor`
2. 定义初始化数据（使用实体类和枚举，不用魔法字符串）
3. 实现幂等逻辑：检查数据是否存在，不存在则插入
4. 注意初始化顺序：先基础数据（权限、角色），后关联数据（角色-权限绑定）
5. 必须使用 `DB.GetNextLongIdAsync()` 获取 ID
6. 编写幂等性测试
7. 执行 `dotnet build` 和 `dotnet test` 验证

---

## 幂等模式

```csharp
public class PermissionSeedContributor : ISeedContributor
{
    public async Task SeedAsync(PlatformSeedContext context)
    {
        // 检查是否已存在
        var existing = await DB.GetListAsync<PlatformPermission>(query);
        if (existing.Length > 0)
        {
            // 已存在，跳过或补充缺失项
            return;
        }

        // 不存在，执行插入
        var permission = new PlatformPermission();
        permission.Id = await DB.GetNextLongIdAsync();
        permission.Code = "platform.user.list";
        permission.Name = "用户列表";
        await DB.InsertAsync(permission);
    }
}
```

---

## 初始化数据范围

### 平台管理基础数据

- 默认超级管理员（用户名、密码哈希）
- 默认管理员角色
- 全量权限清单（菜单权限、API 权限、操作权限）
- 角色-权限绑定
- 角色-用户绑定

### 系统配置基础数据

- 默认密码策略
- 默认安全策略
- 默认功能开关
- 默认系统参数

### SaaS 基础数据

- 默认套餐
- 默认套餐能力配置
- 默认通知模板

---

## 约束

- 禁止使用手写 SQL 文件
- 必须使用实体类和 CRUD 能力
- 必须幂等
- 必须显式获取 ID

---

## 验收标准

- [ ] 初始化可重复执行无脏数据
- [ ] 使用实体类不使用魔法字符串
- [ ] ID 显式生成
- [ ] 幂等性测试通过
- [ ] 编译通过
