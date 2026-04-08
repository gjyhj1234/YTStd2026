# 前端组件提示词

## 目标

创建可复用的前端通用组件。

---

## 适用范围

创建非业务相关的通用组件时使用。

---

## 前置阅读

- `.ai/rules/frontend.md`

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
