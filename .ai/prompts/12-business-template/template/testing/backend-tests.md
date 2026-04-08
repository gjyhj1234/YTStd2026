# {业务名称} — 后端测试

## 目标

为 {业务名称} 后端编写单元测试和集成测试。

---

## 前置阅读

- `.ai/rules/testing.md` — 测试规范
- `.ai/prompts/07-testing/unit-test.md` — 单元测试通用规范
- `.ai/prompts/07-testing/integration-test.md` — 集成测试通用规范

---

## 输出

- `tests/YTStd{Business}.Tests/` — 测试项目
- 每个模块至少包含：成功路径测试、主要失败场景测试

---

## 验收标准

- [ ] `dotnet test YTStd.slnx` 通过
- [ ] 每个模块有成功路径测试
- [ ] 关键失败场景有覆盖
