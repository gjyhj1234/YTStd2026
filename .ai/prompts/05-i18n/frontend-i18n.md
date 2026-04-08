# 前端国际化提示词

## 目标

为前端项目接入完整的国际化支持，覆盖 YTStdI18n.Generator 生成的语言包、组件级语言包、全局资源、格式化工具和 DevExtreme 组件本地化。

---

## 适用范围

前端国际化实现和维护。适用于本项目及所有使用 YTStdI18n 体系的前端项目。

---

## 前置阅读

- `.ai/rules/i18n.md` — 完整国际化规范（必须通读，包含目录结构、key 命名、校验规则等）
- `.ai/rules/frontend.md` — 前端开发规范

---

## 输入

- 已完成的前端页面和组件清单
- 后端 Messages 常量清单
- 后端枚举定义清单
- `locales/generated/` 下 YTStdI18n.Generator 已生成的语言包

---

## 输出

- `src/locales/index.ts` — i18n 初始化入口（vue-i18n 配置）
- `src/locales/merge.ts` — 合并 generated + global + components 的加载逻辑
- `src/locales/generated/**` — 由 Generator 自动生成（禁止手写）
- `src/locales/components/{Module}/{Component}/zh-CN.json` — 组件级语言文件
- `src/locales/components/{Module}/{Component}/en-US.json` — 组件级语言文件
- `src/locales/global/{类别}/zh-CN.json` — 全局资源
- `src/locales/global/{类别}/en-US.json` — 全局资源
- `src/locales/format/datetime.ts` — 日期时间格式化
- `src/locales/format/number.ts` — 数字格式化
- `src/locales/format/currency.ts` — 货币格式化
- 页面和组件中的硬编码文本替换为 `$t()` / `t()` 调用
- DevExtreme 组件本地化配置

---

## 执行步骤

### 第 1 步：初始化 i18n 基础设施

1. 创建 `src/locales/index.ts` — 配置 vue-i18n 实例，支持回退到 zh-CN
2. 创建 `src/locales/merge.ts` — 实现 generated + global + components 三层合并
3. 合并优先级：generated（最低） < global < components（最高）

### 第 2 步：确认 generated 目录

1. 执行 `dotnet build YTStd.slnx` 确保 Generator 已生成 `locales/generated/` 下的语言包
2. 检查 `generated/error/`、`generated/message/`、`generated/enum/`、`generated/notification/` 目录结构正确
3. 确认每个模块每种语言有独立的 JSON 文件

### 第 3 步：创建全局资源

在 `locales/global/` 下按类别创建：

- `menu/zh-CN.json` + `menu/en-US.json` — 菜单文本
- `button/zh-CN.json` + `button/en-US.json` — 通用按钮文本
- `validation/zh-CN.json` + `validation/en-US.json` — 验证消息
- `guide/zh-CN.json` + `guide/en-US.json` — 操作指引通用文本
- `description/zh-CN.json` + `description/en-US.json` — 功能描述通用文本

### 第 4 步：逐组件创建语言文件

对每个已完成的页面组件：

1. 确认组件路径（如 `src/views/User/List.vue`）
2. 创建对应语言目录（`locales/components/User/List/`）
3. 创建 `zh-CN.json` 和 `en-US.json`
4. 在 JSON 中定义该组件使用的所有 key
5. 在组件中将硬编码文本替换为 `t('User.List.xxx')`

通用组件规则：

```text
src/components/Dialog.vue → locales/components/Common/Dialog/zh-CN.json
```

### 第 5 步：配置 DevExtreme 组件本地化

```typescript
import { locale, loadMessages } from 'devextreme/localization';
import zhMessages from 'devextreme/localization/messages/zh';

loadMessages(zhMessages);
locale('zh');
```

### 第 6 步：实现语言切换

1. 从用户个人设置获取语言偏好
2. 回退到租户默认语言
3. 再回退到系统默认语言（zh-CN）
4. 切换后立即生效，无需刷新

### 第 7 步：创建格式化工具

- `locales/format/datetime.ts` — 基于 `Intl.DateTimeFormat` 的日期时间格式化
- `locales/format/number.ts` — 基于 `Intl.NumberFormat` 的数字格式化
- `locales/format/currency.ts` — 基于 `Intl.NumberFormat` 的货币格式化

### 第 8 步：校验与构建

1. 检查所有 components 目录与 views 目录的一一对应
2. 检查 zh-CN.json 和 en-US.json 的 key 一致性
3. 检查 generated 中的 key 与后端常量一一对应
4. 执行 `npm run build` 验证

---

## 组件级 key 结构示例

```json
// locales/components/User/List/zh-CN.json
{
  "title": "用户列表",
  "columns.username": "用户名",
  "columns.status": "状态",
  "columns.created_at": "创建时间",
  "form.username": "用户名",
  "form.password": "密码",
  "button.create": "新增用户",
  "button.batch_enable": "批量启用",
  "description.title": "功能说明",
  "description.content": "管理平台管理员账户...",
  "guide.title": "操作指引",
  "guide.step1": "点击新增按钮..."
}
```

使用方式：

```typescript
t('User.List.title')           // "用户列表"
t('User.List.columns.username') // "用户名"
```

---

## 全局资源 key 结构示例

```json
// locales/global/menu/zh-CN.json
{
  "dashboard": "仪表盘",
  "platform_management": "平台管理",
  "platform_users": "用户管理",
  "platform_roles": "角色管理"
}
```

使用方式：

```typescript
t('global.menu.dashboard')      // "仪表盘"
t('global.button.save')         // "保存"
t('global.validation.required') // "此字段为必填项"
```

---

## 后端错误消息翻译

后端返回 i18n key，前端负责翻译后显示：

```typescript
// utils/http.ts
function handleError(result: ApiResult) {
  if (result.Code !== 0) {
    const message = t(result.Message) || result.Message;
    showNotification({ message, type: 'error' });
  }
}
```

后端 `result.Message` 值如 `"user.username_exists"` 对应 `locales/generated/error/user/zh-CN.json` 中的 key。

---

## 约束

- 基准语言为 zh-CN
- 回退链：当前 locale → zh-CN → 显示 key 本身
- 所有新增的后端 Messages 必须在前端 generated 中有翻译（由 Generator 保证）
- DevExtreme 组件通过 DevExtreme 自身 locale 机制处理，不通过 vue-i18n
- 每个组件的语言文件路径必须与组件路径一一对应
- 禁止手动修改 `locales/generated/` 目录下的文件
- 同一目录下 zh-CN.json 和 en-US.json 的 key 集合必须完全一致

---

## 禁止事项

- 禁止在组件模板中硬编码中文
- 禁止使用 `v-html` 渲染翻译内容（除非明确需要富文本且已消毒）
- 禁止手动修改 `locales/generated/` 目录
- 禁止跨组件复用 key（每个组件有独立的 key 空间）
- 禁止在单个 JSON 文件中放置所有语言资源（必须分文件分目录）
- 禁止缺少 zh-CN.json 文件
- 禁止遗漏组件对应的语言文件目录

---

## 验收标准

- [ ] `src/locales/index.ts` 正确配置 vue-i18n
- [ ] `src/locales/merge.ts` 正确实现三层合并（generated < global < components）
- [ ] 所有用户可见文本使用 `$t()` 或 `t()`
- [ ] 每个 views 组件有对应的 `locales/components/{Module}/{Component}/` 目录
- [ ] 每个语言目录包含 zh-CN.json 和 en-US.json
- [ ] zh-CN.json 与 en-US.json 的 key 完全一致
- [ ] `locales/generated/` 目录未被手动修改
- [ ] DevExtreme 组件本地化正确
- [ ] 语言切换功能正常（用户偏好 → 租户默认 → 系统默认）
- [ ] 日期、数字、货币格式化支持多语言
- [ ] 后端错误消息在前端 generated 中有翻译
- [ ] `npm run build` 通过
