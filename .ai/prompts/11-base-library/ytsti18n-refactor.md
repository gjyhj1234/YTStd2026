# YTStdI18n 重构提示词

## 目标

根据 `.ai/rules/i18n.md` 中定义的最终国际化架构，重构 `YTStdI18n` 运行时库，使其完全符合"后端零文本、整形 Code 驱动"的设计原则。

---

## 适用范围

仅限 `src/YTStdI18n/` 目录下的文件及 `tests/YTStdI18n.Tests/` 下对应的测试。

---

## 前置阅读

- `.ai/rules/i18n.md` — 完整国际化规范（**必须通读**）
- `.ai/rules/global.md` — 全局开发规范（AOT 友好、性能优先）
- `.ai/context/tech-stack.md` — 技术栈约束
- `.ai/context/existing-modules.md` — YTStdI18n 当前 API
- `.ai/prompts/05-i18n/backend-i18n.md` — 后端国际化提示词

---

## 输入

- 现有 `src/YTStdI18n/` 源代码
- 现有 `tests/YTStdI18n.Tests/` 测试代码
- `.ai/rules/i18n.md` 中的架构定义

---

## 输出

- 重构后的 `src/YTStdI18n/I18nCore.cs` — 核心状态管理（租户语言、默认语言）
- 重构后的 `src/YTStdI18n/Lang.cs` — 语言枚举（ZhCn=0, En=1, Ja=2, ZhTw=3, MsMy=4）
- 重构后的 `src/YTStdI18n/I18nResourceAttribute.cs` — 资源标注特性
- 保留或优化的 `src/YTStdI18n/ValueStringBuilder.cs` — 零分配字符串构建器
- 更新后的 `tests/YTStdI18n.Tests/` — 覆盖新 API 的测试
- `dotnet build YTStd.slnx` 通过
- `dotnet test YTStd.slnx` 通过

---

## 重构要点

### 1. I18nCore 职责明确

I18nCore 是运行时核心，职责：

- 管理全局默认语言
- 管理租户语言偏好（`ConcurrentDictionary<int, Lang>`）
- 提供 `Init()` 初始化方法
- 提供 `SetTenantLang()` / `GetTenantLang()` 方法
- 提供 `T(int tenantId, int key)` 翻译方法（由 Generator 生成的 `I18n` 类包装调用）

### 2. 翻译数据存储方式

- 翻译数据以 `string[][]` 二维数组存储（一维=语言, 二维=key 索引）
- 由 Generator 生成的 `I18n.Register()` 方法填充数组
- 查找路径：`array[(int)lang][keyIndex]` → 零分配、O(1) 查找
- 回退策略：找不到 → 尝试 ZhCn → 找不到 → 返回空字符串

### 3. I18nResourceAttribute 保持不变

- `[I18nResource]` 标注静态类
- `IsBase` 属性标记基准语言
- Generator 扫描此属性生成代码

### 4. Lang 枚举保持不变

确认 Lang 枚举值与 `i18n.md` 中的定义完全一致。

---

## 性能要求

- 所有翻译查找 O(1)，基于数组索引
- 零堆分配（`T()` 方法返回 `string`，无新对象创建）
- 热路径方法标注 `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
- 禁止反射、LINQ、dynamic
- 线程安全（volatile + ConcurrentDictionary）

---

## 约束

- 不得破坏 Generator 依赖的 public API（如 `I18nResourceAttribute`、`Lang`、`I18nCore`）
- 如需变更 public API，必须同步更新 `YTStdI18n.Generator`（参见 `ytsti18n-generator-refactor.md`）
- 日志使用 `Logger.Debug()` 的 `Func<string>` 重载
- 所有 public 成员有完整中文 XML 文档注释

---

## 禁止事项

- 禁止引入新外部依赖
- 禁止使用反射、dynamic、LINQ
- 禁止使用 ResourceManager / .resx
- 禁止在翻译查找路径上产生堆分配

---

## 验收标准

- [ ] `I18nCore.Init()` 正确初始化默认语言
- [ ] `I18nCore.SetTenantLang()` / `GetTenantLang()` 正确管理租户语言
- [ ] `Lang` 枚举与 `i18n.md` 定义一致（5 种语言）
- [ ] `I18nResourceAttribute` 保持兼容
- [ ] 所有 public 成员有中文 XML 文档注释
- [ ] 无 AOT 不兼容代码
- [ ] `dotnet build YTStd.slnx` 通过
- [ ] `dotnet test YTStd.slnx` 通过
- [ ] 无新增编译警告

---

## 后续任务

本任务完成后，继续执行 `.ai/prompts/11-base-library/ytsti18n-generator-refactor.md`（Generator 重构）。
