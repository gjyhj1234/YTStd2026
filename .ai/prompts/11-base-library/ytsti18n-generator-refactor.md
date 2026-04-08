# YTStdI18n.Generator 重构提示词

## 目标

根据 `.ai/rules/i18n.md` 中定义的最终国际化架构，重构 `YTStdI18n.Generator`，使其：

1. 为后端生成整形常量索引类（K 类）和翻译注册代码
2. 为前端生成 `locales/generated/{locale}.json`（增量策略：只增不覆盖、删除已废弃）
3. 编译期校验所有语言包一致性

---

## 适用范围

仅限 `src/YTStdI18n.Generator/` 目录下的文件。

---

## 前置阅读

- `.ai/rules/i18n.md` — 完整国际化规范（**必须通读**，特别是第七节 Generator 生成规范）
- `.ai/rules/generator.md` — 源代码生成器规范
- `.ai/rules/global.md` — 全局开发规范
- `.ai/prompts/05-i18n/backend-i18n.md` — 后端国际化提示词
- `.ai/prompts/05-i18n/frontend-i18n.md` — 前端国际化提示词
- `.ai/prompts/11-base-library/ytsti18n-refactor.md` — YTStdI18n 运行时重构（前置任务）

---

## 输入

- 现有 `src/YTStdI18n.Generator/I18nGenerator.cs` 源代码
- 现有 `src/YTStdI18n.Generator/DiagnosticDescriptors.cs`
- 重构后的 `src/YTStdI18n/` 运行时 API
- 前端项目路径（`web/{project}/src/locales/generated/`）

---

## 输出

### 后端生成物（Source Generator 在 `dotnet build` 时自动产出）

1. **K 常量索引类**
   ```csharp
   // Generated: K.cs
   public static class K
   {
       public const int UserNotExist = 0;    // 数组索引
       public const int UserDisabled = 1;
       // ...
   }
   ```

2. **I18n 静态类**（包装 I18nCore，添加翻译数据注册）
   ```csharp
   // Generated: I18n.cs
   public static class I18n
   {
       public static void Register() { ... }  // 注册所有翻译数组
       public static string T(int tenantId, int key) => I18nCore.T(tenantId, key);
   }
   ```

3. **翻译数据注册**
   ```csharp
   // Generated: I18n.Register.cs
   // Register() 内部填充 string[][] 翻译数组
   ```

### 前端生成物（增量输出到文件系统）

4. **`locales/generated/zh-CN.json`**
   ```json
   {
     "100001": "用户不存在",
     "100002": "用户已禁用"
   }
   ```

5. **`locales/generated/en-US.json`**
   ```json
   {
     "100001": "User not found",
     "100002": "User disabled"
   }
   ```

---

## Generator 核心逻辑

### 第 1 步：扫描 [I18nResource] 标注的类

- 使用 `ForAttributeWithMetadataName` 增量管线
- 提取类名、`IsBase` 属性、所有 `const int` 成员
- 提取成员的中文注释或 `[Description]` 作为基准翻译文本

### 第 2 步：生成后端代码

- 生成 K 常量索引类（数组索引，从 0 开始递增）
- 生成 I18n 包装类（调用 I18nCore）
- 生成 Register() 方法（填充翻译数组）
- 生成编译期校验（Diagnostic）：
  - 检查不同语言包的 key 集合是否一致
  - 检查是否有重复 key
  - 检查 IsBase 标记是否恰好一个

### 第 3 步：生成前端语言包（增量策略）

Generator 需要输出前端 JSON 文件，实现增量生成策略：

1. 读取前端 `locales/generated/{locale}.json` 现有内容
2. 遍历后端定义的所有整形 Code（`ErrorCodes` + `MessageCodes` 中的 const int）
3. 对每个 Code：
   - **已存在于 JSON → 不修改**（保留人工翻译校正）
   - **不存在于 JSON → 新增**（使用默认翻译值）
4. 对 JSON 中存在但后端已删除的 Code → **从 JSON 中删除**
5. 写回 JSON 文件（格式化、排序）

### 前端输出路径配置

通过 MSBuild 属性传入前端项目路径：

```xml
<!-- 在业务项目 .csproj 中配置 -->
<PropertyGroup>
  <I18nFrontendOutputPath>$(MSBuildThisFileDirectory)../../web/tenant-platform-web/src/locales/generated</I18nFrontendOutputPath>
</PropertyGroup>
```

Generator 通过 `AnalyzerConfigOptionsProvider` 读取此属性。

---

## ErrorCodes → 前端 Code 的映射规则

| 后端定义 | JSON Key | 说明 |
|---------|----------|------|
| `ErrorCodes.UserNotExist = 100001` | `"100001"` | 错误码的整数值转字符串 |
| `MessageCodes.ConfirmDelete = 200001` | `"200001"` | 提示码的整数值转字符串 |

前端使用方式：
```typescript
t(errorCode.toString())  // t("100001") → "用户不存在"
```

---

## 编译期诊断

| 诊断 ID | 严重性 | 说明 |
|---------|--------|------|
| YTSI001 | Error | 未找到 IsBase=true 的基准语言包 |
| YTSI002 | Error | 存在多个 IsBase=true 的语言包 |
| YTSI003 | Error | 非基准语言包缺少基准语言中的 key |
| YTSI004 | Warning | 非基准语言包包含基准语言中不存在的 key |
| YTSI005 | Error | 同一语言包中存在重复 key |
| YTSI006 | Warning | 前端 generated 目录不存在（跳过前端生成） |

---

## 约束

- Generator 必须使用增量生成模式（`IIncrementalGenerator`）
- 禁止在 Generator 中使用反射
- 禁止 LINQ（可使用 foreach 遍历）
- 前端 JSON 生成是可选的（如果路径不存在则跳过 + 警告）
- 前端 JSON 必须使用 UTF-8 无 BOM 编码
- 前端 JSON 的 key 按数字排序
- 所有 public 成员有完整中文 XML 文档注释

---

## 禁止事项

- 禁止覆盖前端 generated JSON 中已存在 key 的值
- 禁止在 Generator 中引入运行时依赖
- 禁止使用 Newtonsoft.Json（手写 JSON 输出或使用 System.Text.Json 源生成）
- 禁止修改 common/ 或 runtime/ 目录下的文件

---

## 验收标准

- [ ] `dotnet build` 触发 Generator 成功执行
- [ ] 生成 K 常量索引类，索引从 0 递增
- [ ] 生成 I18n 包装类，正确调用 I18nCore
- [ ] 生成 Register() 方法，正确填充翻译数组
- [ ] 编译期诊断覆盖所有场景（YTSI001-YTSI006）
- [ ] 前端 `locales/generated/zh-CN.json` 正确生成
- [ ] 前端 `locales/generated/en-US.json` 正确生成
- [ ] 增量策略正确：已有 key 不覆盖、新 key 新增、废弃 key 删除
- [ ] JSON key 按数字排序
- [ ] `dotnet build YTStd.slnx` 通过
- [ ] `dotnet test YTStd.slnx` 通过
- [ ] 无新增编译警告

---

## 前置任务

- `.ai/prompts/11-base-library/ytsti18n-refactor.md`（YTStdI18n 运行时必须先完成重构）

## 后续任务

本任务完成后，可执行业务模块的后端国际化接入（`.ai/prompts/05-i18n/backend-i18n.md`）。
