# 子任务 13 — E2E 测试补齐（F2-2 / F2-3 / F2-4）

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 13-E2E |
| 模块名称 | E2E 测试补齐 |
| 并行组 | — |
| 对应测试协议 | `.ai/prompts/08-platform/frontend/0040_e2e-testing-protocol.md` |
| 依赖任务 | F2-2、F2-3、F2-4（✅ 已完成编码，尚未编写 E2E 测试） |

---

## 任务目标

为已完成编码但尚未编写 E2E 测试的三个模块补齐 Playwright 测试：
1. F2-2 平台用户管理
2. F2-3 平台角色管理
3. F2-4 平台权限管理

---

## 前置阅读（子任务特定）

除通用前置阅读外，必须额外阅读：

- `.ai/prompts/08-platform/frontend/0021_platform-user-page.md` — 用户管理功能定义
- `.ai/prompts/08-platform/frontend/0022_platform-role-page.md` — 角色管理功能定义
- `.ai/prompts/08-platform/frontend/0023_platform-permission-page.md` — 权限管理功能定义
- `.ai/prompts/08-platform/frontend/0040_e2e-testing-protocol.md` — E2E 测试要点
- `.ai/system/e2e-testing-workflow.md` — 迭代工作流

---

## 背景

这三个模块在 session-summary-20260413-13 中完成了编码和 `npm run build`，但由于会话时间限制未编写 E2E 测试。本子任务专门补齐这部分。

---

## 预期产出文件

| 文件路径 | 用途 |
|---------|------|
| `src/WebTenantPlatfrom/e2e/tests/platform-users/platform-users.spec.ts` | 用户管理 E2E 测试 |
| `src/WebTenantPlatfrom/e2e/tests/platform-roles/platform-roles.spec.ts` | 角色管理 E2E 测试 |
| `src/WebTenantPlatfrom/e2e/tests/platform-permissions/platform-permissions.spec.ts` | 权限管理 E2E 测试 |

---

## 测试覆盖范围

每个模块的 E2E 测试必须覆盖 `0040_e2e-testing-protocol.md` 定义的所有维度：

### F2-2 平台用户管理
- [ ] 页面渲染（标题、搜索区、表格）
- [ ] 数据列表加载
- [ ] 搜索与筛选
- [ ] 新增用户（表单验证、异步唯一性校验、提交）
- [ ] 编辑用户
- [ ] 状态操作（启用/禁用）
- [ ] 重置密码
- [ ] 删除用户

### F2-3 平台角色管理
- [ ] 页面渲染
- [ ] 数据列表加载
- [ ] 搜索与筛选
- [ ] 新增角色
- [ ] 编辑角色
- [ ] 权限绑定
- [ ] 成员绑定
- [ ] 状态操作
- [ ] 删除角色

### F2-4 平台权限管理
- [ ] 页面渲染（DxTreeList 树形结构）
- [ ] 数据加载
- [ ] 搜索过滤（含祖先节点保留）
- [ ] 权限类型颜色标签
- [ ] HTTP 方法颜色标签

---

## 注意事项

- 如果测试运行发现前端代码 bug，需要修复前端代码后重新测试
- 最多迭代 5 次
- 不允许删除测试用例来"通过"测试
- 已知问题：多语言测试中 ja-JP/en-US/ms-MY 翻译可能不匹配，需检查语言文件

---

## 验收标准

- [ ] 3 个模块的 E2E 测试全部编写完成
- [ ] 所有测试通过 `npx playwright test`
- [ ] 测试覆盖上述所有维度
- [ ] 未删除或跳过任何测试用例

---

## 续接说明

完成后更新：
1. `.ai/tasks/platform-frontend/README.md` 中 13 行状态 → ✅
2. 输出 session-summary，记录测试结果和迭代次数
