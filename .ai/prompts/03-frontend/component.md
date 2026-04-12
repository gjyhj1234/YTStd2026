# 前端组件提示词

## 目标

创建可复用的前端通用组件。

---

## 适用范围

创建非业务相关的通用组件时使用。

---

## 前置阅读

- `.ai/rules/frontend.md`
- `.github/copilot-instructions.md` — 关键编码约束（第 7-12 条为前端约束）

---

## DevExpress 文档查阅（强制前置步骤）

使用 DevExtreme 组件时，必须按照官方 dxdocs 工作流查阅文档：

1. **调用 `devexpress_docs_search`**（每个问题仅调用一次，使用 `technologies: ["Vue"]`）
2. **调用 `devexpress_docs_get_content`** 获取最相关帮助主题的全文
3. **反思内容**，提取可用的 API、属性、代码示例
4. **基于检索到的信息编码**，引用具体控件和属性名称

---

## 输入

- 组件需求描述
- 使用场景

---

## 输出

- `src/components/{ComponentName}.vue`

---

## 约束

- 组件命名使用 PascalCase
- 组件必须使用 TypeScript + Composition API
- Props 必须有完整的类型定义
- 必须使用 DevExtreme Vue 组件（如适用）
- 所有文本使用 `t()` 国际化（key 为中文）

---

## 禁止事项

- 禁止在组件内直接调用 API
- 禁止组件间强耦合
- 禁止硬编码样式魔法数字

---

## 验收标准

- [ ] 组件可独立使用
- [ ] Props 类型完整
- [ ] 文本已国际化
- [ ] `npm run build` 通过
