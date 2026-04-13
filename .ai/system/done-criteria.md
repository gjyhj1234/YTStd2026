# 完成标准定义

## 目标

定义各类任务的"完成"标准，确保 Agent 和人类对"什么算完成"有统一理解。

---

## 适用范围

所有由 Agent 执行的任务。

---

## 通用完成标准

无论任务类型如何，以下标准必须满足：

1. **编译通过**：`dotnet build YTStd.slnx` 无错误（后端）；`npm run build` 无错误（前端）
2. **测试通过**：已有测试全部通过（`dotnet test YTStd.slnx`）
3. **代码搜索审查通过**：按 `.ai/system/self-review-protocol.md` 执行全部审查项，违规数为 0
4. **无新增警告**：不引入新的编译警告（除非有合理说明）
5. **注释完整**：所有公开类型和方法有中文 XML 注释
6. **会话总结**：输出符合模板格式的会话总结，包含自动化审查结果数据

---

## 后端实体任务完成标准

- 实体文件位于 `src/{Project}/entity/{Module}/*.cs`
- 实体类使用 `[Entity]`、`[Column]`、`[Index]` 等特性正确标注
- 枚举定义完整，与数据字典一致
- 审计字段（`CreatedAt`、`UpdatedAt`、`CreatedBy`、`UpdatedBy`）按需添加
- 编译触发源代码生成器成功
- 禁止使用裸 `TenantId` / `tenant_id` 作为字段名

---

## 后端 DTO 任务完成标准

- DTO 位于 `Application/Dtos/{Module}/` 目录
- 命名遵循 `{Business}ReqDTO`（请求）和 `{Business}RepDTO`（响应）
- 请求 DTO 仅包含客户端需要提交的字段
- 响应 DTO 仅包含客户端需要展示的字段
- 分页查询使用统一的分页参数约定
- 所有 DTO 属性有中文 XML 注释

---

## 后端应用服务任务完成标准

- 服务位于 `Application/Services/{Module}AppService.cs`
- 所有 `InsertAsync` 调用前使用 `DB.GetNextLongIdAsync()` 获取 ID — **必须用 `grep -B 15 "InsertAsync"` 搜索验证**
- 所有 `ApiResult.Fail()` 仅传 `ErrorCodes.XXX`（不传 message 参数）— **必须用 `grep "ApiResult\.Fail"` 搜索验证**
- 所有 `Logger.Debug` 使用 `Func<string>` 延迟求值重载 — **必须用 `grep "Logger\.Debug"` 搜索验证**
- 有唯一索引的实体有 check-exists 验证
- 唯一性验证使用 `GetListAsync` + 内存循环模式
- 无反射、无 `dynamic`、无 LINQ — **必须用 `grep "System\.Linq"` 搜索验证**
- **代码搜索审查结果记录在会话总结中**

---

## 后端 API 端点任务完成标准

- 端点位于 `Endpoints/{Module}Endpoints.cs`
- 使用 Minimal API / RouteGroup 方式注册
- 所有端点有权限码标注
- 返回值统一使用 `ApiResult<T>` 格式
- `code=0` 表示成功，非零映射到 `ErrorCodes` 常量
- 已在 `RouteRegistration.cs` 中注册

---

## 后端初始化数据任务完成标准

- 初始化逻辑通过 `ISeedContributor` 实现
- 支持重复执行且不产生脏数据（幂等）
- 使用实体类和 CRUD 能力，不使用手写 SQL
- 初始化顺序正确（先权限、角色，再绑定）
- 有对应的幂等性测试

---

## 后端测试任务完成标准

- 测试位于 `tests/{Project}.Tests/`
- 覆盖成功路径和主要失败路径
- 包含权限拒绝场景测试
- 包含唯一性冲突场景测试
- 包含输入验证场景测试
- 所有测试可独立运行，不依赖外部服务

---

## 前端页面任务完成标准

### 适用项目路径

| 项目 | 页面路径 | API 路径 | 类型路径 | 状态 |
|------|---------|---------|---------|------|
| 新前端 | `src/WebTenantPlatfrom/src/views/{module}/` | `src/WebTenantPlatfrom/src/api/{module}.ts` | `src/WebTenantPlatfrom/src/types/{module}.ts` | 活跃 |
| 旧前端 | `web/tenant-platform-web/src/views/{module}/` | `web/tenant-platform-web/src/api/{module}.ts` | `web/tenant-platform-web/src/types/{module}.ts` | 冻结 |

### 通用完成标准

- 使用 DevExtreme Vue 组件（DxDataGrid、DxForm、DxPopup 等）
- 包含功能说明卡片（FunctionDescriptionCard）
- 包含操作指引（OperationGuideDrawer）
- 权限控制正确（按钮级权限显隐）
- API 调用使用模块化封装（`api/{module}.ts`）
- 类型定义位于 `types/{module}.ts`
- 路由已注册且有权限守卫

### 新前端项目额外要求

- HTTP 使用 axios（禁止 fetch）
- 国际化使用 vue-i18n（5 种语言文件齐备）
- 每个 `.vue` 文件有 5 个对应的 `.vue.{locale}.json` 语言文件
- 通过 `self-review-protocol.md` 中 F1-F7 全部审查项

---

## 前端 API 封装任务完成标准

- API 模块位于 `src/api/{module}.ts`
- 类型定义位于 `src/types/{module}.ts`
- 使用 PascalCase 字段名匹配后端
- 返回类型使用 `ApiResult<T>`
- 错误处理统一

---

## 前端国际化任务完成标准

- 语言资源文件位于项目 `src/locales/` 目录下
- 所有用户可见文本使用 `t()` / `$t()` 函数（key 为中文）
- **新前端要求**：必须支持 5 种语言（zh-CN、en-US、ja-JP、ms-MY、zh-TW）
- **旧前端要求**：至少支持中文（zh-CN）和英文（en-US）
- DevExtreme 组件本地化正确
- 日期、数字、货币格式化随 locale 切换
- 每个 `.vue` 文件有对应的语言文件（新前端为 5 个，旧前端为 2+ 个）
- notifySuccess/confirmAction 不双重 t()（仅传 i18n key）
- 组件特有 key 在组件级语言文件中（非主语言文件）

---

## Postman 测试任务完成标准

- 集合文件位于 `docs/` 或 `tests/postman/`
- 包含环境变量定义
- 包含鉴权方式说明
- 每个 API 至少有成功场景和一个失败场景
- 分页接口有分页参数测试
- 有前置条件说明
- 有断言脚本
- **Postman 路由一致性验证通过**：每个 Postman 请求的 HTTP 方法 + URL 路径必须在 `Endpoints/*.cs` 或 `RouteRegistration.cs` 中有精确匹配的注册，不允许存在代码中不存在的路由 — **必须按 `.ai/system/self-review-protocol.md` 审查项 7 执行验证**

---

## 文档任务完成标准

- API 文档与代码中的路由一致
- 实体数据字典与代码中的实体一致
- 架构文档反映当前实际架构
- 无乱码、异常字符
- Markdown 标题层级正确
- 无断裂链接
