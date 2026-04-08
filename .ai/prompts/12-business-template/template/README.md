# {业务名称} — 业务总览

## 目标

{一句话描述业务系统的目标}

---

## 技术栈

- **后端项目**：`src/YTStd{Business}/`
- **前端项目**：`web/{business}-web/`
- **底层框架**：复用 YTStdSqlBuilder、YTStdLogger、YTStdAdo、YTStdEntity、YTStdI18n（及其 Generator）
- **数据库**：PostgreSQL，表前缀 `{prefix}_`
- **文档**：`docs/{Business}/`

---

## 模块列表

| 模块 | 后端提示词 | 前端提示词 | 状态 |
|------|-----------|-----------|------|
| {模块1} | `backend/{module1}-api.md` | `frontend/{module1}-page.md` | 待开发 |
| {模块2} | `backend/{module2}-api.md` | `frontend/{module2}-page.md` | 待开发 |
| ... | ... | ... | ... |

---

## 执行顺序

### 第 1 阶段：数据库与实体

1. `database/schema.md` — 表结构设计
2. `database/seed-data.md` — 初始化数据

### 第 2 阶段：后端基础设施

3. `backend/infrastructure.md` — 主程序骨架、中间件、认证
4. `backend/error-codes.md` — 错误码常量定义

### 第 3 阶段：后端 API（按模块）

5. `backend/{module1}-api.md`
6. `backend/{module2}-api.md`
7. ...

### 第 4 阶段：后端测试

8. `testing/backend-tests.md`
9. `testing/postman.md`

### 第 5 阶段：前端

10. `frontend/scaffold.md` — 前端工程初始化
11. `frontend/layout.md` — 布局与导航
12. `frontend/{module1}-page.md`
13. `frontend/{module2}-page.md`
14. ...

### 第 6 阶段：前端国际化

15. `frontend/i18n.md`

---

## 前置阅读（全局）

所有任务开始前，Agent 必须阅读：

- `.ai/system/agent-contract.md`
- `.ai/rules/global.md`
- `.ai/context/tech-stack.md`
- `.ai/context/project-structure.md`

---

## 文档

- 数据字典：`docs/{Business}/database_dictionary.md`
- API 文档：`docs/{Business}/API.md`
- 架构文档：`docs/{Business}/architecture.md`
