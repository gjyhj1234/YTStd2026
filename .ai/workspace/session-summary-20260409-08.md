## 会话总结

### 会话信息
- **日期**：2026-04-09
- **轮次**：第 9 轮（唯一性校验专项审计 — 非编码任务）
- **任务编号**：TASK-PLATFORM-001
- **任务标题**：租户平台检查与重构 — 唯一性校验缺失审计与提示词优化

### 当前所处阶段
- 阶段 D 后端测试已完成
- 子任务 E0（B 阶段修复）已完成
- 本轮为非编码审计任务，聚焦唯一性校验问题
- 阶段 E 前端重构尚未开始

### 本轮目标
- 审计所有 AppService 的 Create/Save 方法中唯一性校验缺失问题
- 添加规则和提示词到 .ai 体系中防止未来复现
- 详细记录审计结果和修复计划，为下一轮编码任务提供完整上下文

### 本轮已完成

| 序号 | 操作 | 文件路径 | 说明 |
|-----|------|---------|------|
| 1 | 修改 | `.github/copilot-instructions.md` | 新增规则 6：唯一性双重校验模式（含正反面代码示例） |
| 2 | 修改 | `.ai/system/self-review-protocol.md` | 新增审查项 8：唯一性校验审查（含搜索命令和输出格式）；更新执行时机表和全通过输出 |
| 3 | 修改 | `.ai/rules/backend.md` | 唯一性验证模式由单重校验改为双重校验（前置校验 + 后置复核） |
| 4 | 修改 | `.ai/prompts/02-backend/app-service.md` | 约束项增加双重校验要求，验收标准增加唯一性审查项 |
| 5 | 修改 | `.ai/tasks/task-platfrom.md` | 添加问题 4 完整审计结果（4A/4B/4C/4D 四节） |
| 6 | 新建 | `.ai/workspace/session-summary-20260409-08.md` | 本轮会话总结 |

### 验证结果
- 编译：未执行（本轮非编码任务，未修改 src/ 文件）
- 测试：未执行（本轮非编码任务）
- 上轮测试基线：374 个通过（329 平台 + 23 SQL + 12 i18n + 10 实体）

### 决策记录
1. **双重校验模式设计**：前置校验负责 99% 场景下的精确错误返回；后置复核负责并发竞争导致的唯一索引冲突场景。选择重新查询而非解析数据库异常，原因是框架 CRUD 层只返回 Success/Failure，不暴露底层异常类型。
2. **错误码分段**：唯一性冲突统一在 18xxx 段（已有 18001-18006，新增 18007-18015），不与业务模块段（19xxx）混用，便于前端统一处理唯一冲突提示。
3. **审计粒度**：区分"完全缺失"（4A，12 个方法）和"缺少后置复核"（4B，3 个方法），便于下一轮按优先级修复。

### 审计发现摘要

#### 4A. 完全缺少唯一性前置校验（12 个方法）
- PlatformUserAppService.CreateAsync — Username + Email
- PlatformRoleAppService.CreateAsync — Code
- TenantLifecycleAppService.CreateAsync — TenantCode
- TenantInfoAppService.CreateGroupAsync — GroupCode
- TenantInfoAppService.CreateDomainAsync — Domain
- TenantInfoAppService.CreateTagAsync — TagKey
- TenantConfigAppService.SaveFeatureFlagAsync — FeatureKey
- TenantConfigAppService.SaveParameterAsync — ParamKey
- DictionaryAppService.CreateAsync — TypeCode+ItemCode
- PackageAppService.CreateVersionAsync — VersionCode
- NotificationAppService.CreateTemplateAsync — TemplateCode

#### 4B. 有前置校验但缺少后置复核（3 个方法）
- PlatformPermissionAppService.CreateAsync — Code → PermissionCodeExists
- PackageAppService.CreatePackageAsync — PackageCode → PackageCodeExists
- MenuAppService.CreateAsync — Code → MenuCodeExists

#### 需新增的 ErrorCodes（9 个）
18007 EmailExists, 18008 GroupCodeExists, 18009 DomainExists,
18010 TagKeyExists, 18011 FeatureKeyExists, 18012 ParamKeyExists,
18013 DictItemCodeExists, 18014 PackageVersionCodeExists, 18015 NotificationTemplateCodeExists

### 未完成内容（下一轮编码任务）
1. 在 ErrorCodes.cs 中添加 9 个新错误码（18007-18015）
2. 修复 4A 中 12 个方法（添加前置校验 + 后置复核）
3. 修复 4B 中 3 个方法（补充后置复核）
4. 编译 + 测试 + 自审（含审查项 8）
5. 预计涉及 11 个源文件修改

### 风险与待确认
1. **TenantConfigAppService.SaveParameterAsync**：ParamKey 的唯一性应是同一租户内唯一（TenantRefId + ParamKey），修复时需注意复合唯一校验
2. **TenantConfigAppService.SaveFeatureFlagAsync**：FeatureKey 的唯一性同样应是同一租户内唯一（TenantRefId + FeatureKey）
3. **DictionaryAppService.CreateAsync**：唯一性是 TypeCode + ItemCode 组合唯一，非单字段唯一
4. **PackageAppService.CreateVersionAsync**：VersionCode 是同一套餐（PackageId）内唯一
5. **TenantInfoAppService.CreateDomainAsync**：Domain 是全局唯一（不同租户不能绑定相同域名）

### 下一轮应继续
1. 阅读 `.ai/workspace/session-summary-20260409-08.md`（本轮总结）
2. 阅读 `.ai/tasks/task-platfrom.md` 中 "问题 4" 的完整审计结果
3. 按修复计划 4C 执行编码：Step 1 → Step 6
4. 编码完成后执行 self-review-protocol 全部 8 个审查项

### 下一轮必须保持一致的规则
- 错误码体系：唯一性冲突统一在 18xxx 段
- 唯一性双重校验模式：前置校验 + 后置复核
- InsertAsync 前 GetNextLongIdAsync（零容忍）
- ApiResult.Fail() 仅传 ErrorCodes 常量
- Logger.Debug 使用 Func<string> 延迟求值
- 复合唯一校验注意业务范围（同一租户、同一套餐等）
- 374 个测试全部通过（修复后数量只增不减）

### 下一轮建议阅读的文件
- `.ai/workspace/session-summary-20260409-08.md`（本轮总结）
- `.ai/tasks/task-platfrom.md`（问题 4 审计详情 + 修复计划 4C）
- `.github/copilot-instructions.md`（新增规则 6）
- `.ai/system/self-review-protocol.md`（新增审查项 8）
- `.ai/rules/backend.md`（更新后的唯一性双重校验模式）
