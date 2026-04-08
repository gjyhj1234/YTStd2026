# 多轮会话续接协议

## 目标

定义多轮 Agent 会话之间的上下文传递、状态恢复和任务继续机制，确保长任务可跨会话无缝衔接。

---

## 适用范围

所有需要多轮 Agent 执行才能完成的任务。

---

## 续接机制

### 1. 会话结束时的输出

每轮会话结束时，Agent 必须按 `.ai/templates/session-summary-template.md` 输出会话总结，至少包含：

- 当前所处阶段
- 本轮已完成的内容（具体到文件名）
- 本轮输出到哪个文件
- 下一轮应从哪里继续
- 下一轮必须保持一致的规则
- 尚未输出的剩余部分
- 风险点与待确认事项

### 2. 新会话开始时的恢复

新一轮会话开始时，Agent 必须：

1. 阅读上一轮的会话总结文件（`.ai/workspace/session-summary-{date}-{seq}.md`）
2. 阅读 `.ai/system/agent-contract.md` 确认协作规则
3. 检查代码库当前状态（`git status`、`dotnet build`）
4. 确认上一轮的产出是否完整可用
5. 从上一轮标记的"下一轮应继续"处开始执行

### 3. 上下文传递

Agent 不依赖会话内存进行上下文传递。所有跨会话信息通过以下方式传递：

| 信息类型 | 传递方式 |
|---------|---------|
| 已完成文件 | Git 仓库中的实际文件 |
| 任务状态 | `.ai/workspace/session-summary-*.md` |
| 编译状态 | 实际执行 `dotnet build` / `npm run build` |
| 测试状态 | 实际执行 `dotnet test` / `npm run test` |
| 设计决策 | 会话总结中的"决策记录"节 |
| 风险与问题 | 会话总结中的"风险点"节 |

---

## 续接提示词模板

人类在启动新一轮 Agent 时，可使用以下模板：

```markdown
请继续上一轮未完成的任务。

## 上下文恢复

1. 请先阅读 `.ai/workspace/session-summary-{最新日期}.md`
2. 请阅读以下规则文件：
   - `.ai/system/agent-contract.md`
   - `.ai/rules/global.md`
   - {其他相关规则文件}

## 本轮目标

继续完成上一轮未完成的内容，具体为：
- {列出本轮要完成的子任务}

## 验证要求

- 后端：`dotnet build YTStd.slnx` && `dotnet test YTStd.slnx`
- 前端：`cd web/tenant-platform-web && npm run build`
```

---

## 状态文件命名

会话总结文件命名格式：`session-summary-{YYYYMMDD}-{序号}.md`

示例：
- `session-summary-20260407-01.md`（2026年4月7日第1轮）
- `session-summary-20260407-02.md`（2026年4月7日第2轮）
- `session-summary-20260408-01.md`（2026年4月8日第1轮）

---

## 断点恢复策略

### 情况 1：上一轮正常结束

直接从会话总结中的"下一轮应继续"处开始。

### 情况 2：上一轮异常中断（Agent 超时）

1. 检查 Git 中最后提交的文件
2. 执行 `dotnet build` 确认编译状态
3. 如果编译失败，优先修复编译错误
4. 如果编译成功，从最后完成的文件之后继续

### 情况 3：上一轮的代码被人类修改

1. 查看 Git diff 了解人类的修改内容
2. 确认修改是否影响当前任务
3. 根据实际情况调整执行计划
4. 在会话总结中记录"检测到人类修改"

---

## 跨会话一致性保证

以下内容在多轮会话中必须保持一致：

1. **命名规范**：实体名、表名、字段名、API 路径、DTO 名称
2. **目录结构**：文件放置位置不得在会话间变化
3. **技术选型**：框架版本、依赖版本不得在会话间变化
4. **编码风格**：缩进、注释风格、变量命名风格
5. **错误码体系**：ErrorCodes 的编号分配
6. **权限码体系**：Permission 的命名规则
7. **i18n 键命名**：Messages 的键命名规则

---

## 长任务推进示例

以"实现租户管理模块"为例，多轮推进方式：

```
第 1 轮：实体建模
  输入：database_dictionary.md
  输出：entity/*.cs, Domain/Enums/*.cs
  状态文件：session-summary-20260407-01.md

第 2 轮：DTO 与应用服务
  输入：第 1 轮产出的实体 + session-summary-20260407-01.md
  输出：Application/Dtos/Tenant/*.cs, Application/Services/TenantLifecycleAppService.cs
  状态文件：session-summary-20260407-02.md

第 3 轮：API 端点与测试
  输入：第 2 轮产出 + session-summary-20260407-02.md
  输出：Endpoints/TenantEndpoints.cs, tests/TenantLifecycleTests.cs
  状态文件：session-summary-20260408-01.md

第 4 轮：Postman 与前端 API 封装
  输入：第 3 轮产出 + session-summary-20260408-01.md
  输出：docs/tenant-postman.json, web/.../api/tenant.ts
  状态文件：session-summary-20260408-02.md
```
