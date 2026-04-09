# Postman 集合生成提示词

## 目标

为指定模块的 API 生成完整的 Postman 测试集合。

---

## 适用范围

生成或更新 Postman 测试集合时使用。

---

## 前置阅读

- `.ai/rules/postman.md`
- `.ai/rules/api-design.md`

---

## 输入

- 已完成的 API 端点清单
- API 路径、HTTP 方法、请求/响应格式

---

## 输出

- `docs/YTStd-{Module}-Postman-Collection.json`（或更新已有集合）

---

## 执行步骤

1. 列出模块的所有 API 端点 — **从 `Endpoints/*.cs` 和 `RouteRegistration.cs` 中提取实际已注册的路由，禁止凭记忆或规格文件生成**
2. 为每个端点创建 Postman 请求
3. 添加环境变量（baseUrl、token）
4. 添加登录前置脚本
5. 为每个请求添加成功场景断言
6. 为关键请求添加失败场景
7. 添加分页/筛选场景（列表接口）
8. 验证集合格式正确
9. **执行路由一致性验证**：提取 Postman JSON 中所有请求的 HTTP 方法 + URL 路径，与 Endpoints 代码逐一比对，确保 100% 匹配

---

## 每个端点必须包含的请求

| API 类型 | 必须包含的测试请求 |
|---------|-----------------|
| 创建 | 成功创建、必填字段缺失、唯一性冲突 |
| 查询列表 | 默认分页、关键词搜索、状态筛选 |
| 查询详情 | 存在的 ID、不存在的 ID |
| 更新 | 成功更新、资源不存在 |
| 删除 | 成功删除、资源不存在 |
| 状态变更 | 成功变更、状态不允许 |
| 所有 | 无权限访问 |

---

## 约束

- 集合格式为 Postman Collection v2.1
- 环境变量按 `.ai/rules/postman.md` 定义
- 断言脚本使用 JavaScript
- 请求名与 API 路由对齐
- **Postman 中的每个请求路径必须在 Endpoints 代码中有精确匹配的注册**
- **禁止 Postman 中存在代码中不存在的路由（如 `/api/auth/refresh` 对应代码中实际为 `/api/auth/refresh-token` 的情况）**

---

## 验收标准

- [ ] 集合可导入 Postman
- [ ] 每个端点有成功和失败场景
- [ ] 断言脚本正确
- [ ] 环境变量完整
- [ ] **路由一致性验证通过** — Postman 中 0 个路由在代码中不存在，代码中 0 个公开路由在 Postman 中缺失
