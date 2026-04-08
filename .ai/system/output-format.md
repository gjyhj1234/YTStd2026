# 输出格式规范

## 目标

统一 Agent 在各种场景下的输出格式，确保输出可读、可追踪、可用于续接。

---

## 适用范围

所有 Agent 执行的任务输出。

---

## 代码文件输出格式

### 新建文件

Agent 创建新文件时，必须说明：
- 文件路径
- 文件用途
- 关键设计决策（如有）

### 修改文件

Agent 修改已有文件时，必须说明：
- 文件路径
- 修改内容摘要
- 修改原因

---

## 任务报告格式

每个任务完成后，输出以下格式的报告：

```markdown
## 任务完成报告

### 任务信息
- 任务名称：{名称}
- 所属阶段：{阶段编号}
- 执行时间：{起止时间}

### 已完成内容

| 序号 | 操作 | 文件路径 | 说明 |
|-----|------|---------|------|
| 1 | 新建 | src/.../*.cs | ... |
| 2 | 修改 | src/.../*.cs | ... |

### 验证结果
- 编译：✓ 通过 / ✗ 失败（附错误信息）
- 测试：✓ N 个通过 / ✗ N 个失败（附失败信息）

### 风险与待确认
- {风险描述}

### 下一步建议
- {建议内容}
```

---

## 文件清单格式

列出文件时使用表格格式：

```markdown
| 序号 | 文件路径 | 用途 | 状态 |
|-----|---------|------|------|
| 1 | src/Entity/PlatformUser.cs | 平台用户实体 | ✓ 完成 |
| 2 | src/Dtos/PlatformUser/*.cs | 用户 DTO | ✓ 完成 |
| 3 | src/Services/PlatformUserAppService.cs | 用户应用服务 | ◯ 进行中 |
| 4 | src/Endpoints/PlatformUserEndpoints.cs | 用户 API | ☐ 待开始 |
```

状态符号：
- `✓` 已完成
- `◯` 进行中
- `☐` 待开始
- `✗` 失败/阻塞

---

## 错误报告格式

遇到错误时，使用以下格式报告：

```markdown
## 错误报告

### 错误信息
- 类型：编译错误 / 运行时错误 / 测试失败
- 位置：{文件:行号}
- 错误消息：{完整错误消息}

### 原因分析
- {分析内容}

### 已尝试的修复
1. {修复方案1} → 结果：{成功/失败}
2. {修复方案2} → 结果：{成功/失败}

### 建议
- {建议内容}
```

---

## API 路由清单格式

```markdown
| HTTP 方法 | 路径 | 说明 | 权限码 |
|----------|------|------|-------|
| GET | /api/platform-users | 用户列表（分页） | platform.user.list |
| GET | /api/platform-users/{id} | 用户详情 | platform.user.detail |
| POST | /api/platform-users | 创建用户 | platform.user.create |
| PUT | /api/platform-users/{id} | 编辑用户 | platform.user.update |
| PUT | /api/platform-users/{id}/enable | 启用用户 | platform.user.enable |
| PUT | /api/platform-users/{id}/disable | 禁用用户 | platform.user.disable |
| DELETE | /api/platform-users/{id} | 删除用户 | platform.user.delete |
```

---

## 实体清单格式

```markdown
| 实体名 | 表名 | 说明 | 索引 | 审计 |
|-------|------|------|------|------|
| PlatformUser | sys_user | 平台管理员 | username (唯一) | ✓ |
| PlatformRole | sys_role | 平台角色 | code (唯一) | ✓ |
```

---

## Postman 测试场景格式

```markdown
| 场景 | HTTP 方法 | 路径 | 预期状态码 | 预期响应 |
|------|----------|------|-----------|---------|
| 创建用户-成功 | POST | /api/platform-users | 200 | code=0 |
| 创建用户-用户名重复 | POST | /api/platform-users | 200 | code=18001 |
| 创建用户-无权限 | POST | /api/platform-users | 403 | 权限不足 |
```

---

## 禁止的输出方式

1. 禁止使用"以下略""后续同理""其余类似"等省略表达
2. 禁止只给目录名不给文件内容
3. 禁止只给思路不给实际代码
4. 禁止把重要规则压缩成一句话
5. 禁止用摘要版替代完整版
6. 禁止用"请自行补充"代替实际内容
