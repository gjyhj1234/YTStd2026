# 模块脚手架提示词

## 目标

为一个新的业务模块生成完整的代码脚手架，包含实体、DTO、服务、端点、测试的基本结构。

---

## 适用范围

新增一个完整的业务模块时使用。

---

## 前置阅读

- `.ai/rules/global.md`
- `.ai/rules/backend.md`
- `.ai/rules/naming.md`
- `.ai/rules/generator.md`
- `.ai/rules/api-design.md`

---

## 输入

- 模块名称（英文）
- 实体清单（从数据字典提取）
- 业务说明

---

## 输出

按以下顺序生成文件：

1. `entity/{Module}/{Entity}.cs` — 实体定义
2. `Domain/Enums/{Enum}.cs` — 枚举定义
3. `Application/Dtos/{Module}/` — DTO 文件
4. `Application/Services/{Module}AppService.cs` — 应用服务
5. `Application/Constants/ErrorCodes.cs` — 更新错误码
6. `Application/Constants/Messages.cs` — 更新消息常量
7. `Endpoints/{Module}Endpoints.cs` — API 端点
8. `Bootstrap/RouteRegistration.cs` — 更新路由注册
9. `Infrastructure/Initialization/Contributors/{Module}SeedContributor.cs` — 初始化数据
10. `tests/{Module}Tests.cs` — 测试

---

## 执行步骤

1. 阅读数据字典中该模块的表定义
2. 创建实体和枚举
3. 执行 `dotnet build` 触发 Source Generator
4. 创建 DTO（请求、响应、查询）
5. 创建应用服务
6. 更新 ErrorCodes 和 Messages
7. 创建 API 端点
8. 更新路由注册
9. 创建初始化数据贡献者
10. 创建测试
11. 执行 `dotnet build` 验证
12. 执行 `dotnet test` 验证

---

## 约束

- 每个模块遵循 `.ai/rules/generator.md` 中的完整产出清单
- DTO 命名遵循 `{Business}ReqDTO` / `{Business}RepDTO`
- API 路径遵循 `.ai/rules/api-design.md`
- 错误码在已有范围内分配

---

## 禁止事项

- 禁止跳过 Source Generator 编译步骤
- 禁止使用裸 `TenantId`
- 禁止省略 XML 注释

---

## 验收标准

- [ ] 实体编译通过且 Generator 成功
- [ ] 所有 DTO 有完整注释
- [ ] 应用服务遵循 ID 生成规则
- [ ] API 端点已注册
- [ ] 测试覆盖成功路径和唯一性冲突
- [ ] `dotnet build` 和 `dotnet test` 通过
