# 配置模块提示词

## 目标

实现系统配置管理和功能开关管理。

---

## 适用范围

实现系统配置和功能开关功能时使用。

---

## 前置阅读

- `.ai/rules/backend.md`
- `.ai/rules/database.md`

---

## 输入

- 配置需求
- 功能开关需求

---

## 输出

### 系统配置

- `Application/Services/SystemConfigAppService.cs`
- `Endpoints/SystemConfigEndpoints.cs`
- 表：`sys_config`

### 功能开关

- `Application/Services/FeatureFlagAppService.cs`
- `Endpoints/FeatureFlagEndpoints.cs`
- 表：`sys_feature_flag`

---

## 配置类型

| 配置类型 | 表 | 示例 |
|---------|---|------|
| 系统配置 | `sys_config` | 密码策略、登录策略、安全策略 |
| 功能开关 | `sys_feature_flag` | MFA 开关、自助注册开关 |
| UI 品牌 | `sys_config` | 平台名称、Logo URL |
| 通知模板 | `ntf_template` | 邮件模板、短信模板 |

---

## 约束

- 配置数据可缓存
- 配置变更记录审计日志
- 功能开关支持租户级和全局级

---

## 验收标准

- [ ] 配置 CRUD 完整
- [ ] 功能开关 CRUD 完整
- [ ] 缓存生效
- [ ] 审计记录
- [ ] 编译通过
