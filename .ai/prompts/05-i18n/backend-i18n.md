# 后端国际化提示词

## 目标

为后端模块接入国际化支持，确保所有用户可见消息使用 i18n key。

---

## 适用范围

后端代码中涉及国际化的部分。

---

## 前置阅读

- `.ai/rules/i18n.md`
- `.ai/rules/global.md`
- `.ai/context/existing-modules.md`（YTStdI18n 部分）

---

## 输入

- 需要国际化的模块
- 已有的 Messages 常量

---

## 输出

- `Application/Constants/Messages.cs` 更新
- 相关服务代码中硬编码中文替换为 Messages 常量

---

## 执行步骤

1. 扫描模块中所有 `ApiResult.Fail()` 调用
2. 确认 message 参数是否使用 `Messages.XXX`
3. 如有硬编码中文，替换为 Messages 常量
4. 在 Messages.cs 中添加缺失的常量
5. 确保常量值为 i18n key 格式（如 `user.username_exists`）
6. 验证编译通过

---

## 约束

- 所有 `ApiResult.Fail()` 使用 `Messages.XXX`
- Messages 常量值为 dot.separated 格式的 i18n key
- 日志消息不需要国际化

---

## 禁止事项

- 禁止硬编码中文字符串作为 API 错误消息
- 禁止直接使用字符串字面量作为 i18n key
- 禁止在日志中使用 i18n（日志不国际化）

---

## 验收标准

- [ ] 所有 `ApiResult.Fail()` 使用 Messages 常量
- [ ] Messages 常量值为合法 i18n key
- [ ] 编译通过
