# 租户平台 — 后端测试

## 目标

补齐和完善租户平台后端测试。

---

## 前置阅读

- `.ai/rules/testing.md`
- `.ai/prompts/07-testing/unit-test.md`
- `.ai/prompts/07-testing/integration-test.md`

---

## 现有测试

`tests/YTStdTenantPlatform.Tests/` — 已有 140 个测试

---

## 补充目标

1. 所有 API 模块有成功路径测试
2. 关键失败场景有覆盖（如唯一性冲突、权限不足、状态流转非法）
3. 中间件行为测试
4. 初始化数据幂等性测试

---

## 验收标准

- [ ] `dotnet test YTStd.slnx` 通过
- [ ] 每个模块有成功路径测试
- [ ] 状态流转边界测试
- [ ] 唯一性冲突测试
