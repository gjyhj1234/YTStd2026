# Agent 自动化代码审查协议

## 目标

定义 Agent 在每次编码任务完成后、标记任务完成之前，必须执行的自动化代码搜索审查流程。此协议的目的是**通过代码搜索验证取代纯编译验证**，确保所有编码约束被严格遵守。

---

## 适用范围

所有涉及后端代码创建或修改的编码任务。

---

## 核心原则

> **编译通过 ≠ 任务完成。编译只验证语法正确性，不验证规则遵守情况。**

Agent 必须在 `dotnet build` 和 `dotnet test` 通过后，额外执行本协议定义的代码搜索审查。如果审查发现违规，必须修复后再次编译验证。

---

## 强制审查项（后端）

### 审查项 1：InsertAsync 前必须有 GetNextLongIdAsync（零容忍）

**搜索命令：**

```bash
# 步骤 1：找到所有 InsertAsync 调用
grep -rn "InsertAsync" src/{Project}/Application/Services/*.cs

# 步骤 2：对每个 InsertAsync 调用，向上检查 5-15 行内是否有 GetNextLongIdAsync
grep -B 15 "InsertAsync" src/{Project}/Application/Services/*.cs | grep -A 1 "InsertAsync"
```

**验证规则：**
- 对每一处 `InsertAsync` 调用，其前方（同一方法内）必须存在对应实体的 `Id = await DB.GetNextLongIdAsync()` 或 `Id = await context.GetNextLongIdAsync()`
- 关联表（如 `RolePermission`、`RoleMember`、`TagBinding`）同样适用
- 中间件、后台任务中的 `InsertAsync` 同样适用
- **违规数量为 0 时才算通过**

**输出格式：**

```
[InsertAsync 审查] 共找到 N 处 InsertAsync 调用
- ✅ {文件}:{行号} — 已有 GetNextLongIdAsync
- ❌ {文件}:{行号} — 缺少 GetNextLongIdAsync（必须修复）
结论：通过 / 不通过（N 处违规需修复）
```

### 审查项 2：ApiResult.Fail 仅传错误码

**搜索命令：**

```bash
grep -rn "ApiResult\.Fail\|ApiResult<.*>\.Fail" src/{Project}/Application/Services/*.cs
```

**验证规则：**
- `Fail(` 调用中仅有一个 `int` 类型参数（`ErrorCodes.XXX`）
- 不允许传第二个字符串 message 参数
- **违规数量为 0 时才算通过**

### 审查项 3：Logger.Debug 使用 Func<string> 延迟求值

**搜索命令：**

```bash
grep -rn "Logger\.Debug" src/{Project}/Application/Services/*.cs src/{Project}/Infrastructure/*.cs
```

**验证规则：**
- 每处 `Logger.Debug` 的第三个参数必须以 `() =>` 开头（lambda 表达式）
- 不允许直接传字符串插值（`$"..."`）或字符串连接（`"..." + var`）
- **违规数量为 0 时才算通过**

### 审查项 4：禁止 LINQ / 反射 / dynamic

**搜索命令：**

```bash
grep -rn "using System\.Linq\|\.Where(\|\.Select(\|\.FirstOrDefault(\|\.Any(\|\.Count(\|\.OrderBy(" src/{Project}/ --include="*.cs"
grep -rn "System\.Reflection\|GetType()\|typeof(" src/{Project}/ --include="*.cs"
grep -rn "\bdynamic\b" src/{Project}/ --include="*.cs"
```

**验证规则：**
- 搜索结果为 0（排除测试项目）
- **违规数量为 0 时才算通过**

### 审查项 5：无裸 TenantId / tenant_id

**搜索命令：**

```bash
grep -rn "TenantId\b\|tenant_id\b" src/{Project}/entity/ --include="*.cs"
```

**验证规则：**
- 搜索结果为 0（必须使用 `TenantRefId`、`OwnerTenantId` 等业务语义命名）
- **违规数量为 0 时才算通过**

### 审查项 6：XML 注释完整性

**搜索命令：**

```bash
# 找到缺少 <summary> 的 public 类/方法/属性
grep -B 1 "public " src/{Project}/Application/Services/*.cs | grep -v "summary\|///"
```

**验证规则：**
- 所有 `public` 类型、方法、属性前一行必须有 `/// <summary>` 注释
- 注释内容使用中文

---

## 强制审查项（唯一性校验）

### 审查项 8：Create/Save 方法的唯一性双重校验

**搜索命令：**

```bash
# 步骤 1：找到所有 Create/Save 方法中的 InsertAsync 调用
grep -rn "InsertAsync" src/{Project}/Application/Services/*.cs

# 步骤 2：对每个 InsertAsync 调用，检查前方是否有唯一性前置校验（XxxExists 错误码）
grep -B 30 "InsertAsync" src/{Project}/Application/Services/*.cs | grep "Exists"

# 步骤 3：对每个 InsertAsync 失败分支，检查是否有后置复核（重新查询+返回 Exists 错误码）
grep -A 10 "!insResult.Success" src/{Project}/Application/Services/*.cs | grep "Exists"
```

**验证规则：**
- 对每一个包含唯一索引字段的实体的 Create/Save 方法：
  1. InsertAsync 前必须有唯一性前置校验（遍历现有数据检查重复）
  2. InsertAsync 失败时必须有唯一性后置复核（重新查询判断是否唯一冲突）
  3. 每个唯一字段必须有对应的 `ErrorCodes.XxxExists` 错误码（位于 18xxx 段）
  4. 不允许唯一字段冲突时仅返回笼统的 `XxxCreateFailed` 错误码
- **违规数量为 0 时才算通过**

**输出格式：**

```
[唯一性校验审查] 共检查 N 个 Create/Save 方法
- ✅ {服务}.{方法} — 已有前置校验 + 后置复核（错误码: XxxExists）
- ⚠️ {服务}.{方法} — 有前置校验但缺少后置复核
- ❌ {服务}.{方法} — 缺少前置校验（唯一字段: xxx）
结论：通过 / 不通过（N 处违规需修复）
```

---

## 强制审查项（Postman 集合）

### 审查项 7：Postman 路由与代码端点一致性

**验证方法：**

1. 从 Postman JSON 中提取所有请求的 HTTP 方法 + URL 路径
2. 从 `Endpoints/*.cs` 和 `Bootstrap/RouteRegistration.cs` 中提取所有已注册的路由
3. 逐一比对：
   - Postman 中有但代码中没有的路由 → **必须删除或修复 Postman**
   - 代码中有但 Postman 中没有的路由 → **必须补充到 Postman**
4. 特别注意路由路径的精确匹配（如 `/refresh` vs `/refresh-token`）

**输出格式：**

```
[Postman 一致性审查]
- Postman 请求总数：N
- 代码端点总数：M
- ✅ 匹配：K 个
- ❌ Postman 有但代码无：{列表}
- ❌ 代码有但 Postman 无：{列表}
结论：通过 / 不通过
```

---

## 强制审查项（前端）

### 审查项 F1：所有 DxColumn caption 使用 $t() 绑定

**搜索命令：**

```bash
# 找到所有硬编码 caption（未使用绑定形式的 caption）
grep -rn 'caption="' web/tenant-platform-web/src/ --include="*.vue" | grep -v ':caption'
```

**验证规则：**
- 搜索结果必须为 0
- 所有 DxColumn、DxTreeColumn 等的 caption 必须使用 `:caption="$t('...')"`
- **违规数量为 0 时才算通过**

### 审查项 F2：notifySuccess / confirmAction 不双重 t()

**搜索命令：**

```bash
grep -rn "notifySuccess(t(" web/tenant-platform-web/src/ --include="*.vue"
grep -rn "confirmAction(t(" web/tenant-platform-web/src/ --include="*.vue"
grep -rn "confirmDelete(t(" web/tenant-platform-web/src/ --include="*.vue"
```

**验证规则：**
- 搜索结果必须为 0
- useNotify.ts 内部已调用 t()，调用方仅传 i18n key 字符串
- **违规数量为 0 时才算通过**

### 审查项 F3：每个 .vue 文件有 5 个对应语言文件

**搜索命令：**

```bash
for f in $(find web/tenant-platform-web/src/views -name "*.vue" -not -name "PlaceholderView.vue"); do
  for lang in zh-CN en-US ja-JP ms-MY zh-TW; do
    [ ! -f "${f}.${lang}.json" ] && echo "MISSING: ${f}.${lang}.json"
  done
done
```

**验证规则：**
- 搜索结果必须为空（无缺失文件）
- **缺失数量为 0 时才算通过**

### 审查项 F4：语言文件 key 一致性

**验证方法：**
- 对每个视图目录，比较 zh-CN 和 en-US 的 JSON key 集合
- 所有语言文件的 key 必须完全一致
- en-US、ja-JP、ms-MY、zh-TW 的值不能为空字符串

### 审查项 F5：DxForm label-mode 检查（登录页专项）

**搜索命令：**

```bash
grep -rn 'label-mode="floating"' web/tenant-platform-web/src/views/login/ --include="*.vue"
```

**验证规则：**
- 登录页搜索结果必须为 0
- 登录页的 DxForm 必须使用 `label-mode="static"`（避免浏览器自动填充与 floating label 重叠）

---

## 执行时机

| 时机 | 必须执行的审查项 |
|------|---------------|
| 每个应用服务文件编写完成后 | 审查项 1、2、3、4、8 |
| 每个端点文件编写完成后 | 审查项 4、5、6 |
| 每个阶段（Phase）完成后 | 全部审查项 1-8 |
| Postman 集合更新后 | 审查项 7 |
| 每个前端 .vue 文件编写完成后 | 审查项 F1、F2、F3 |
| 每个前端模块（子任务）完成后 | 全部审查项 F1-F5 |
| 标记任务完成之前（后端任务） | 全部审查项 1-8 |
| 标记任务完成之前（前端任务） | 全部审查项 F1-F5 + npm run build |

---

## 审查结果处理

### 发现违规时

1. **立即修复**所有违规项
2. 修复后重新执行 `dotnet build` + `dotnet test`
3. 修复后重新执行对应的审查项验证
4. 在会话总结中记录"自审发现 N 处违规，已全部修复"

### 全部通过时

在会话总结中记录：

```
自动化代码审查结果：✅ 全部通过
- InsertAsync + GetNextLongIdAsync：N/N 处合规
- ApiResult.Fail 仅传 ErrorCodes：N/N 处合规
- Logger.Debug Func<string>：N/N 处合规
- 无 LINQ/反射/dynamic：✅
- 无裸 TenantId：✅
- XML 注释完整：✅
- Postman 一致性：✅ N/N 匹配
- 唯一性双重校验：✅ N/N 个 Create/Save 方法合规
```

---

## 审查失败的后果

如果 Agent 未执行本协议就标记任务完成，人类审查者将要求 Agent 回退并重新执行。为避免浪费 Token 和时间，Agent 必须在编码阶段就时刻注意约束，并在完成后执行审查确认。

---

## 版本

- 版本：1.0
- 创建日期：2026-04-09
- 创建原因：历史任务中发现 B 阶段多个 AppService 缺少 `GetNextLongIdAsync`，Postman 集合存在路由不匹配，根因是 Agent 仅以编译通过作为验收标准，未执行代码搜索审查
