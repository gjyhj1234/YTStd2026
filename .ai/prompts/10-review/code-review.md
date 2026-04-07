# 代码审查提示词

## 目标

对已完成的代码进行系统审查，发现问题并给出修改建议。

---

## 适用范围

代码审查时使用。

---

## 前置阅读

- `.ai/rules/global.md`
- `.ai/rules/backend.md`（后端代码审查时）
- `.ai/rules/frontend.md`（前端代码审查时）
- `.ai/rules/security.md`
- `.ai/rules/naming.md`
- `.ai/system/review-policy.md`

---

## 输入

- 需要审查的文件列表
- 审查重点（如有特定关注点）

---

## 输出

审查报告（按 `.ai/system/review-policy.md` 中的格式）

---

## 审查检查清单

### 后端代码审查

- [ ] 无反射、无 dynamic、无 LINQ
- [ ] 所有 InsertAsync 前有 ID 生成
- [ ] 所有 Logger.Debug 使用 Func<string>
- [ ] 所有错误消息使用 Messages 常量
- [ ] JSON 使用 Utf8JsonWriter 或 JsonSerializerContext
- [ ] 中间件响应使用 PascalCase
- [ ] 无裸 TenantId
- [ ] XML 注释完整
- [ ] 权限码正确
- [ ] 无安全漏洞
- [ ] 编译通过
- [ ] 测试通过

### 前端代码审查

- [ ] TypeScript 类型使用 PascalCase
- [ ] API 封装正确
- [ ] 权限控制完整
- [ ] 功能说明和操作指引完整
- [ ] 文本已国际化
- [ ] 编译通过

### 文档审查

- [ ] 与代码一致
- [ ] 无乱码
- [ ] 标题层级正确
- [ ] 编号连续

---

## 执行步骤

1. 阅读所有规则文件
2. 逐文件审查
3. 按检查清单逐项检查
4. 输出审查报告
5. 按严重级别排序问题

---

## 约束

- 审查报告必须具体到行号
- 必须区分严重级别（严重/警告/建议）
- 必须给出具体修改建议

---

## 验收标准

- [ ] 审查覆盖所有指定文件
- [ ] 审查覆盖所有检查清单项
- [ ] 问题有具体修改建议
