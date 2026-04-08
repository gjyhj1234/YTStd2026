# {业务名称} — Postman 测试集合

## 目标

为 {业务名称} 生成 Postman 测试集合。

---

## 前置阅读

- `.ai/rules/postman.md` — Postman 测试规范
- `.ai/prompts/07-testing/postman-collection.md` — Postman 通用规范
- `docs/{Business}/API.md` — API 文档

---

## 输出

- `docs/YTStd-{Business}-Postman-Collection.json`

---

## 验收标准

- [ ] 集合可导入 Postman
- [ ] 每个 API 有成功测试
- [ ] 包含认证 Token 环境变量
