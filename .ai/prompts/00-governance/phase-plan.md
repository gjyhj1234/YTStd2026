# 阶段推进计划

## 目标

定义从零开始重建整个解决方案的分阶段执行计划，指导 GitHub Agents 按顺序推进。

---

## 适用范围

使用 GitHub Agents + Visual Studio 2026 重建整个解决方案。

---

## 前置阅读

- `.ai/system/agent-contract.md`
- `.ai/system/task-splitting.md`
- `.ai/system/session-handoff.md`
- `.ai/context/tech-stack.md`

---

## 阶段总览

| 阶段 | 名称 | 目标 | 依赖 | 预估轮次 |
|------|------|------|------|---------|
| P0 | 协作规则建立 | 确认规则和目录结构 | 无 | 1 |
| P1 | 数据库设计 | 数据字典与表结构确认 | P0 | 1-2 |
| P2 | 实体建模 | 创建实体、枚举、触发生成器 | P1 | 2-3 |
| P3 | 初始化数据 | 建表、种子数据、缓存预热 | P2 | 1-2 |
| P4 | 后端基础设施 | 主程序骨架、中间件、认证 | P3 | 2-3 |
| P5 | 核心后端 API | 平台管理、租户核心 API | P4 | 3-5 |
| P6 | 扩展后端 API | 套餐、订阅、计费等扩展 API | P5 | 3-5 |
| P7 | 后端测试 | 单元测试、集成测试 | P5-P6 | 2-3 |
| P8 | Postman 测试 | 生成 Postman 测试集合 | P5-P6 | 1-2 |
| P9 | 前端骨架 | 前端工程、登录、布局 | P5 | 2-3 |
| P10 | 前端模块 | 业务页面、API 对接 | P6, P9 | 5-8 |
| P11 | 前端国际化 | 全量 i18n 接入 | P10 | 2-3 |
| P12 | 文档整理 | API 文档、架构文档 | P6, P10 | 1-2 |
| P13 | 最终审查 | 全量校验、补漏、收尾 | 全部 | 1-2 |

---

## 阶段详细说明

### P0：协作规则建立

**目标**：确认 Agent 协作规则和项目结构。

**执行内容**：
1. 阅读 `.ai/` 下所有 system、context、rules 文件
2. 确认技术栈和约束
3. 确认目录结构
4. 确认构建和测试命令可用

**验收**：
- Agent 确认已理解所有规则
- `dotnet build YTStd.slnx` 通过
- `dotnet test YTStd.slnx` 通过

**人类介入点**：确认规则无需调整

---

### P1：数据库设计

**目标**：确认或补全数据字典。

**执行内容**：
1. 审查 `docs/TenantPlatform/database_dictionary.md`
2. 按 `.ai/rules/database.md` 验证命名规范
3. 验证前缀体系（plt_、tnt_、saas_、sys_、log_、ntf_、api_、stg_）
4. 验证字段命名（审计字段、布尔字段、状态字段、时间字段）
5. 验证索引命名
6. 补全缺失的表或字段

**验收**：
- 数据字典与命名规范完全一致
- 所有表有前缀
- 无裸 `tenant_id` 字段

**人类介入点**：确认数据字典变更

---

### P2：实体建模

**目标**：创建所有实体、枚举，触发 Source Generator。

**前置阅读**：
- `.ai/rules/generator.md`
- `.ai/rules/backend.md`
- `.ai/prompts/02-backend/entity-modeling.md`

**执行内容**：
1. 按数据字典创建实体类
2. 创建对应枚举
3. 创建必要值对象
4. 编译触发 Source Generator
5. 修正编译错误

**按模块分批**：
- 批次 1：平台管理实体（plt_user, plt_role, plt_permission, plt_user_role, plt_role_permission）
- 批次 2：租户实体（tnt_info, tnt_lifecycle_event, tnt_group, tnt_domain, tnt_tag）
- 批次 3：资源与配置实体（tnt_resource_quota, sys_config, sys_feature_flag）
- 批次 4：SaaS 业务实体（saas_package, saas_subscription, saas_billing）
- 批次 5：运营支撑实体（log_audit, ntf_template, api_key, stg_file）

**验收**：
- `dotnet build YTStd.slnx` 通过
- Source Generator 成功生成代码
- 无裸 `TenantId` 字段

---

### P3：初始化数据

**目标**：实现幂等初始化数据和缓存预热。

**前置阅读**：
- `.ai/prompts/04-database/seed-data.md`

**执行内容**：
1. 实现 SeedRunner 和 ISeedContributor 基础设施
2. 实现各模块的 SeedContributor
3. 实现缓存预热
4. 编写幂等性测试

**验收**：
- 初始化数据可重复执行无脏数据
- 缓存预热成功
- 初始化测试通过

---

### P4：后端基础设施

**目标**：搭建主程序骨架和中间件。

**前置阅读**：
- `.ai/prompts/02-backend/middleware.md`
- `.ai/rules/security.md`

**执行内容**：
1. Program.cs 入口
2. 服务注册和路由注册
3. 全局异常中间件
4. 请求日志 / TraceId 中间件
5. 权限中间件
6. 限流中间件
7. 审计中间件
8. 认证（JWT）
9. 健康检查

**验收**：
- 主程序可启动
- 中间件正常工作
- 中间件测试通过

**人类介入点**：确认中间件管道顺序

---

### P5-P6：后端 API

**目标**：实现所有业务 API。

**按模块分块**（每个模块一个子任务）：

P5 核心模块：
1. 认证 API（登录、Token）
2. 平台用户管理 API
3. 平台角色管理 API
4. 平台权限管理 API
5. 租户生命周期 API
6. 租户信息管理 API
7. 租户资源管理 API
8. 租户配置管理 API

P6 扩展模块：
1. 套餐管理 API
2. 订阅管理 API
3. 计费与账单 API
4. API 集成管理 API
5. 平台运营 API
6. 审计日志 API
7. 通知系统 API
8. 文件存储 API

每个模块的子任务包含：
- DTO 定义
- 应用服务实现
- API 端点注册
- 错误码和消息常量更新
- 基础测试

**验收**：
- 编译通过
- 每个模块有至少成功路径测试
- API 路由与文档一致

---

### P7-P8：测试与 Postman

**目标**：补齐测试覆盖和 Postman 集合。

**验收**：
- 所有 API 有成功和主要失败场景测试
- Postman 集合可导入执行
- 测试全部通过

---

### P9-P10：前端

**目标**：构建完整前端。

**P9 前端骨架**（按子任务）：
1. 工程初始化（Vite + Vue 3 + TypeScript）
2. DevExtreme 配置
3. HTTP 封装和类型定义
4. 路由和权限守卫
5. 布局组件（侧边栏、顶栏、面包屑）
6. 登录页和认证状态管理
7. 通用组件（FunctionDescriptionCard、OperationGuideDrawer）
8. 首页/仪表盘

**P10 前端模块**（每个模块一个子任务）：
1. 平台用户管理页面
2. 平台角色管理页面
3. 平台权限管理页面
4. 租户管理页面
5. 租户信息页面
6. 资源配额页面
7. 配置与开关页面
8. 套餐管理页面
9. 订阅管理页面
10. 账单管理页面
11. API 集成管理页面
12. 审计日志页面
13. 通知管理页面
14. 文件管理页面

**验收**：
- `npm run build` 通过
- 所有页面可正常渲染
- 权限控制正确

---

### P11：前端国际化

**目标**：全量接入 i18n。

**前置阅读**：
- `.ai/rules/i18n.md`
- `.ai/prompts/05-i18n/frontend-i18n.md`

---

### P12：文档整理

**目标**：确保所有文档与代码一致。

---

### P13：最终审查

**目标**：全量校验、补漏、收尾。

**检查清单**：
- [ ] 后端编译通过
- [ ] 后端测试全部通过
- [ ] 前端编译通过
- [ ] API 文档与代码一致
- [ ] 数据字典与实体一致
- [ ] Postman 集合完整
- [ ] 国际化资源完整
- [ ] 权限码完整
- [ ] 无安全漏洞
- [ ] 无文档乱码

---

## 执行规则

1. 严格按阶段顺序执行，不跳跃
2. 每个阶段结束后验证，再进入下一阶段
3. 每轮 Agent 执行后输出会话总结
4. 人类在标记的介入点确认后再继续
5. 如果某个阶段的某个模块失败，不影响其他模块继续
6. 优先完成平台底座（P0-P4），再完成业务模块（P5-P6）
