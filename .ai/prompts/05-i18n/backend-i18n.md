# 后端国际化提示词

## 目标

为后端模块接入国际化支持，确保所有用户可见消息使用整形常量，后端零文本输出，并通过 YTStdI18n.Generator 自动生成前端语言包。

---

## 适用范围

后端代码中涉及国际化的部分。适用于本项目及所有使用 YTStdI18n 体系的项目。

---

## 前置阅读

- `.ai/rules/i18n.md` — 完整国际化规范（必须通读）
- `.ai/rules/global.md` — 全局开发规范
- `.ai/context/existing-modules.md` — YTStdI18n 部分

---

## 输入

- 需要国际化的后端模块名称
- 已有的 Messages 常量清单
- 已有的枚举定义清单

---

## 输出

- `Application/Constants/Messages.cs` — 更新或新增 Messages 常量
- `Application/Constants/ErrorCodes.cs` — 更新或新增错误码常量
- 相关服务代码中硬编码中文替换为 Messages 常量
- `I18nResourceAttribute` 标注的资源类定义（如尚未定义）
- `dotnet build` 触发 YTStdI18n.Generator 生成前端语言包

---

## 执行步骤

1. 扫描目标模块中所有 `ApiResult.Fail()` 调用
2. 检查 message 参数是否使用 `Messages.XXX` 常量
3. 扫描所有硬编码中文字符串，替换为 Messages 常量
4. 在 Messages.cs 中添加缺失的常量，常量值为 dot.separated 格式的 i18n key
5. 扫描所有枚举定义，确保枚举值不附带显示名文本（枚举值为 int）
6. 确认 `I18nResourceAttribute` 已标注到对应资源类
7. 执行 `dotnet build YTStd.slnx` 触发 Generator
8. 验证 Generator 在前端 `locales/generated/` 目录下生成了对应的语言包文件
9. 验证 generated 中的 key 与后端常量一一对应
10. 编译通过且无警告

---

## 后端零文本原则

### 错误消息

```csharp
// 正确 — 使用 Messages 常量（i18n key 字符串）
return ApiResult.Fail(ErrorCodes.UsernameExists, Messages.UsernameExists);

// 错误 — 硬编码中文
return ApiResult.Fail(ErrorCodes.UsernameExists, "用户名已存在");
```

### 枚举值

```csharp
// 正确 — 枚举只有整形值
public enum UserStatus
{
    Active = 0,
    Disabled = 1,
    Locked = 2
}
// 前端通过 t('enum.user_status.0') 获取显示文本

// 错误 — 枚举附带显示名
[Description("启用")]
Active = 0
```

### 系统通知

```csharp
// 正确 — 使用整形常量标识模板
notification.TemplateCode = NotificationTemplates.WelcomeNewUser;
// 前端通过 t('notification.system.welcome_new_user') 翻译

// 错误 — 通知内容包含中文文本
notification.Content = "欢迎注册本系统";
```

---

## YTStdI18n.Generator 生成规则

### Generator 触发

- `dotnet build` 时自动触发
- 读取后端 `I18nResourceAttribute` 标注的资源
- 生成前端 `locales/generated/{类型}/{模块}/{语言}.json`

### 增量生成策略

- 遍历后端定义的所有 key
- 检查前端对应语言文件中该 key 是否存在
- **已存在 → 不修改**（保留人工翻译校正）
- **不存在 → 新增**（使用默认翻译或空字符串）
- **后端已删除 → 从 generated 中删除**

### 生成目标目录

```text
web/{project}/src/locales/generated/
├── error/{模块}/zh-CN.json
├── message/{模块}/zh-CN.json
├── enum/{枚举名}/zh-CN.json
└── notification/{模块}/zh-CN.json
```

---

## 约束

- 所有 `ApiResult.Fail()` 必须使用 `Messages.XXX` 常量
- Messages 常量值为 dot.separated 格式的 i18n key
- 后端不保留任何用户可见的文本字符串
- 枚举值为整形，不附带 `[Description]` 等文本属性
- 日志消息不需要国际化
- 系统通知使用模板 Code 而非文本内容

---

## 禁止事项

- 禁止硬编码中文字符串作为 API 错误消息
- 禁止直接使用字符串字面量作为 i18n key
- 禁止在日志中使用 i18n（日志不国际化）
- 禁止枚举定义中包含 `[Description]` 中文文本
- 禁止后端 API 响应中返回翻译后的文本（只返回 key）
- 禁止手动创建或修改 `locales/generated/` 目录下的文件

---

## 验收标准

- [ ] 所有 `ApiResult.Fail()` 使用 Messages 常量
- [ ] Messages 常量值为合法 i18n key
- [ ] 所有枚举值无 `[Description]` 文本属性
- [ ] 后端 API 响应不包含中文文本（仅 key）
- [ ] `I18nResourceAttribute` 已标注到所有资源类
- [ ] `dotnet build YTStd.slnx` 通过
- [ ] Generator 在 `locales/generated/` 下生成了对应文件
- [ ] generated 文件中 key 与后端常量一一对应
