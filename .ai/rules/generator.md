# 源代码生成器规范

## 目标

定义实体代码生成器的使用规范，确保 AI 在生成实体相关代码时系统考虑所有关联产出。

---

## 适用范围

所有使用 YTStdEntity.Generator 和 YTStdSqlBuilder.Generator 的代码。

---

## 生成器类型

| 生成器 | 项目 | 生成内容 |
|-------|------|---------|
| YTStdEntity.Generator | `src/YTStdEntity.Generator/` | DAL 层 CRUD 方法、描述器 |
| YTStdSqlBuilder.Generator | `src/YTStdSqlBuilder.Generator/` | SQL 构建辅助代码 |
| YTStdI18n.Generator | `src/YTStdI18n.Generator/` | 国际化常量索引 |

---

## 实体生成器完整产出清单

当 AI 创建一个新实体时，不仅是定义实体类本身，还必须系统考虑以下所有关联产出：

### 必须产出（每个实体都需要）

| 序号 | 产出 | 位置 | 说明 |
|-----|------|------|------|
| 1 | 实体类 | `entity/{Module}/{Entity}.cs` | 带 `[Entity]`、`[Column]`、`[Index]` 特性 |
| 2 | 枚举 | `Domain/Enums/{EnumName}.cs` | 状态、类型等枚举 |
| 3 | 请求 DTO | `Application/Dtos/{Module}/Create{Entity}ReqDTO.cs` | 创建请求 |
| 4 | 请求 DTO | `Application/Dtos/{Module}/Update{Entity}ReqDTO.cs` | 更新请求 |
| 5 | 查询 DTO | `Application/Dtos/{Module}/Query{Entity}ReqDTO.cs` | 列表查询参数 |
| 6 | 响应 DTO | `Application/Dtos/{Module}/{Entity}RepDTO.cs` | 响应数据 |
| 7 | 响应 DTO | `Application/Dtos/{Module}/{Entity}ListRepDTO.cs` | 列表项数据 |
| 8 | 应用服务 | `Application/Services/{Module}AppService.cs` | CRUD 服务 |
| 9 | API 端点 | `Endpoints/{Module}Endpoints.cs` | HTTP API |
| 10 | 错误码 | `Application/Constants/ErrorCodes.cs` | 新增错误码 |
| 11 | 消息常量 | `Application/Constants/Messages.cs` | 新增 i18n key |
| 12 | 权限码 | 权限初始化数据中 | 菜单/API/操作权限 |
| 13 | 初始化数据 | `Infrastructure/Initialization/` | 种子数据 |

### 按需产出（根据业务需要）

| 序号 | 产出 | 条件 | 说明 |
|-----|------|------|------|
| 14 | 审计字段 | 需要审计的实体 | `CreatedAt`、`UpdatedAt` 等 |
| 15 | 软删除 | 需要逻辑删除的实体 | `IsDeleted`、`DeletedAt` |
| 16 | 排序 | 需要排序的实体 | `SortOrder` 字段 |
| 17 | 启用/禁用 | 有状态的实体 | `IsEnabled` 或 `Status` |
| 18 | 状态流转 | 有生命周期的实体 | 状态机方法 |
| 19 | 批量操作 | 需要批量处理的实体 | 批量启用/禁用/删除 |
| 20 | 导入/导出 | 需要数据交换的实体 | 导入导出服务 |
| 21 | 值对象 | 复合属性 | `Domain/ValueObjects/` |
| 22 | 映射 | DTO 与实体转换 | 手动映射方法 |
| 23 | 验证 | 复杂验证规则 | 验证方法或验证器 |
| 24 | 菜单项 | 需要前端菜单的实体 | 菜单初始化数据 |
| 25 | 前端页面 | 需要管理界面的实体 | ListView/CreateView/DetailView |
| 26 | 前端 API | 需要前端调用的实体 | `api/{module}.ts` |
| 27 | 前端类型 | 需要前端使用的实体 | `types/{module}.ts` |
| 28 | 单元测试 | 所有实体 | `tests/{Module}Tests.cs` |
| 29 | Postman 测试 | 有 API 的实体 | Postman 集合 |
| 30 | API 文档 | 有 API 的实体 | `docs/API.md` 更新 |

---

## 生成器触发方式

1. 创建或修改实体文件后执行 `dotnet build`
2. 编译时 Source Generator 自动运行
3. 生成的代码在编译产物中，不需要手写 `.g.cs`
4. 如果编译报错，根据错误信息修正实体定义后重新编译

---

## 实体定义检查清单

创建新实体时，逐项确认：

- [ ] 实体类使用 `[Entity]` 特性标注表名和描述
- [ ] 所有属性使用 `[Column]` 特性标注列名、类型和描述
- [ ] 主键 `Id` 标注 `IsPrimaryKey = true`
- [ ] 唯一约束使用 `[Index(IsUnique = true)]`
- [ ] 状态字段有对应枚举定义
- [ ] 审计字段按需添加（`created_at`、`updated_at`、`created_by`、`updated_by`）
- [ ] 外键引用字段命名正确（禁止裸 `TenantId`）
- [ ] 主从关系使用 `[DetailOf]` 特性
- [ ] 所有公开成员有中文 XML 注释
- [ ] 编译成功触发生成器

---

## 与旧提示词的对应关系

| 旧提示词 | 新规范位置 |
|---------|-----------|
| `entity-prompt.md` | `.ai/rules/generator.md`（本文件）+ `.ai/prompts/02-backend/entity-modeling.md` |
| `sql-builder-prompt.md` | `.ai/context/existing-modules.md` |
| `ado-prompt.md` | `.ai/context/existing-modules.md` |
| `i18n-prompt.md` | `.ai/rules/i18n.md` + `.ai/context/existing-modules.md` |
