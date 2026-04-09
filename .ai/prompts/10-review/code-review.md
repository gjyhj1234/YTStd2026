# 代码审查提示词

## 目标

对已完成的代码进行系统审查，**通过代码搜索命令（而非人工阅读）验证编码约束的遵守情况**，发现问题并给出修改建议。

---

## 适用范围

代码审查时使用。Agent 在编码任务完成后必须执行本审查（参见 `.ai/system/self-review-protocol.md`）。

---

## 前置阅读

- `.ai/rules/global.md`
- `.ai/rules/backend.md`（后端代码审查时）
- `.ai/rules/frontend.md`（前端代码审查时）
- `.ai/rules/security.md`
- `.ai/rules/naming.md`
- `.ai/system/review-policy.md`
- `.ai/system/self-review-protocol.md`（自动化审查流程）

---

## 输入

- 需要审查的文件列表
- 审查重点（如有特定关注点）

---

## 输出

审查报告（按 `.ai/system/review-policy.md` 中的格式），**必须包含每个审查项的搜索命令执行结果**

---

## 审查检查清单

### 后端代码审查（必须使用代码搜索验证）

以下每一项必须通过实际执行 `grep` 命令来验证，不允许仅凭记忆或阅读确认：

- [ ] **InsertAsync + GetNextLongIdAsync**：`grep -rn -B 15 "InsertAsync" src/{Project}/Application/Services/*.cs` — 每处 InsertAsync 前方必须有 GetNextLongIdAsync
- [ ] **ApiResult.Fail 仅传 ErrorCodes**：`grep -rn "ApiResult\.Fail" src/{Project}/Application/Services/*.cs` — 参数仅为 `ErrorCodes.XXX`
- [ ] **Logger.Debug Func<string>**：`grep -rn "Logger\.Debug" src/{Project}/` — 第三参数必须为 `() => ` lambda
- [ ] **无 LINQ**：`grep -rn "using System\.Linq" src/{Project}/` — 结果为 0
- [ ] **无反射**：`grep -rn "System\.Reflection" src/{Project}/` — 结果为 0
- [ ] **无 dynamic**：`grep -rn "\bdynamic\b" src/{Project}/` — 结果为 0（排除注释）
- [ ] **无裸 TenantId**：`grep -rn "TenantId\b" src/{Project}/entity/` — 结果为 0
- [ ] **JSON PascalCase**：`grep -rn "Utf8JsonWriter\|JsonSerializerContext" src/{Project}/` — 确认使用正确
- [ ] **中间件 PascalCase 响应**：`grep -rn "Code\|Message\|Data" src/{Project}/Infrastructure/Middleware/*.cs` — 属性名为 PascalCase
- [ ] **XML 注释完整**：`grep -B 1 "public " src/{Project}/Application/Services/*.cs | grep -v "summary"` — 检查是否缺少注释
- [ ] 编译通过：`dotnet build YTStd.slnx`
- [ ] 测试通过：`dotnet test YTStd.slnx`
- [ ] **Postman 路由一致性**（如涉及 Postman）：提取 Postman JSON 中所有路由，与 Endpoints 代码逐一比对

### 前端代码审查

- [ ] 编译通过：`npm run build` 无错误
- [ ] 类型使用 PascalCase 匹配后端
- [ ] API 封装使用 `ApiResult<T>` 格式
- [ ] 所有页面有功能说明和操作指引
- [ ] 权限控制正确
- [ ] 路由注册且有守卫
- [ ] DevExtreme 组件使用正确

### 文档审查

- [ ] 与代码一致
- [ ] 无乱码
- [ ] 标题层级正确
- [ ] 编号连续

---

## 执行步骤

1. 阅读所有规则文件
2. **执行 `.ai/system/self-review-protocol.md` 中的所有搜索命令**
3. 按检查清单逐项检查，记录每项的搜索命令执行结果
4. 输出审查报告
5. 按严重级别排序问题
6. **如发现违规，必须修复后重新执行对应的搜索验证**

---

## 约束

- 审查报告必须具体到行号
- 必须区分严重级别（严重/警告/建议）
- 必须给出具体修改建议
- **必须包含搜索命令的执行结果，不允许声称"已检查"但不提供搜索输出**

---

## 验收标准

- [ ] 审查覆盖所有指定文件
- [ ] 审查覆盖所有检查清单项
- [ ] **每个检查项有对应的 `grep` 搜索命令执行结果**
- [ ] 问题有具体修改建议
- [ ] 违规数量为 0（或已全部修复后重新验证通过）
