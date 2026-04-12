# 前端国际化提示词

## 目标

为前端项目接入完整的国际化支持，覆盖 YTStdI18n.Generator 生成的语言包、组件级语言文件、全局 common 资源和 DevExtreme 组件本地化。

---

## 适用范围

前端国际化实现和维护。适用于本项目及所有使用 YTStdI18n 体系的前端项目。

---

## 前置阅读

- `.ai/rules/i18n.md` — 完整国际化规范（必须通读，包含目录结构、key 设计、t()/gt() 实现等）
- `.ai/rules/frontend.md` — 前端开发规范
- `.github/copilot-instructions.md` — 关键编码约束（第 7-12 条为前端约束）

---

## 输入

- 已完成的前端页面和组件清单
- 后端 ErrorCodes 常量清单
- 后端枚举定义清单
- `locales/generated/` 下 YTStdI18n.Generator 已生成的语言包

---

## 输出

- `src/locales/index.ts` — i18n 核心配置，使用 `import.meta.glob` 自动加载组件级语言文件
- `src/locales/generated/{locale}.json` — 由 Generator 自动生成（允许手动编辑翻译，Generator 不覆盖已有 key）
- `src/locales/common/{locale}.json` — 全局复用资源（5 种语言）
- `{Component}.vue.zh-CN.json` — 组件级语言文件（紧贴 .vue 文件，5 种语言）
- `{Component}.vue.en-US.json` — 组件级语言文件
- `{Component}.vue.ms-MY.json` — 组件级语言文件
- `{Component}.vue.zh-TW.json` — 组件级语言文件
- `{Component}.vue.ja-JP.json` — 组件级语言文件
- 页面和组件中的硬编码文本替换为 `$t()` 或 `t()` 调用
- DevExtreme 组件本地化配置

---

## 核心实现：组件级语言文件自动加载（import.meta.glob）

### 关键问题

组件级语言文件（`*.vue.{locale}.json`）必须被实际加载到 vue-i18n 中，否则修改这些文件不会影响界面。**禁止**将组件特有的翻译 key 放在主语言文件（`src/locales/{locale}.json`）中——那些文件仅保留嵌套结构键（`app`、`languages`、`menu`、`route` 等）。

### locales/index.ts 实现模板（可直接复用到其他业务工程）

```typescript
import { createI18n } from 'vue-i18n'

// ① 静态导入：主语言文件（仅保留嵌套结构键）、common、generated、enums
import zhCN from './zh-CN.json'
import enUS from './en-US.json'
// ... 其他语言同理
import commonZhCN from './common/zh-CN.json'
import commonEnUS from './common/en-US.json'
import generatedZhCN from './generated/zh-CN.json'
import generatedEnUS from './generated/en-US.json'
import enumsZhCN from './enums/zh-CN.json'
import enumsEnUS from './enums/en-US.json'

// ② 使用 import.meta.glob 自动加载所有组件级语言文件
//    eager: true — 编译时静态导入，构建时自动包含所有匹配文件
//    import: 'default' — 直接获取 JSON 默认导出
const componentZhCNModules = import.meta.glob<Record<string, string | null>>(
  ['../views/**/*.vue.zh-CN.json', '../components/**/*.vue.zh-CN.json', '../layouts/**/*.vue.zh-CN.json'],
  { eager: true, import: 'default' },
)
const componentEnUSModules = import.meta.glob<Record<string, string | null>>(
  ['../views/**/*.vue.en-US.json', '../components/**/*.vue.en-US.json', '../layouts/**/*.vue.en-US.json'],
  { eager: true, import: 'default' },
)
// ... 对 ms-MY、zh-TW、ja-JP 同理

// ③ 合并组件级消息：遍历所有模块，仅保留非 null 值
//    null 值表示"此 key 由 common 层提供"，不覆盖上层翻译
function mergeComponentMessages(modules: Record<string, Record<string, string | null>>): Record<string, string> {
  const result: Record<string, string> = {}
  for (const data of Object.values(modules)) {
    if (!data) continue
    for (const key of Object.keys(data)) {
      const value = data[key]
      if (value !== null && value !== undefined) {
        result[key] = value
      }
    }
  }
  return result
}

const componentZhCN = mergeComponentMessages(componentZhCNModules)
const componentEnUS = mergeComponentMessages(componentEnUSModules)
// ... 对 ms-MY、zh-TW、ja-JP 同理

// ④ 消息合并优先级（从低到高）：
//    1. common — 全局复用翻译（最低优先级）
//    2. generated — Generator 生成的 ErrorCode 翻译
//    3. enums — 枚举值翻译
//    4. 主语言文件 — 应用级结构化翻译（app/menu/route 等嵌套键）
//    5. 组件级语言文件 — 组件自有翻译（最高优先级，修改后直接生效）
const messages = {
  'zh-CN': { ...commonZhCN, ...generatedZhCN, ...enumsZhCN, ...zhCN, ...componentZhCN },
  'en-US': { ...commonEnUS, ...generatedEnUS, ...enumsEnUS, ...enUS, ...componentEnUS },
  // ... 对 ms-MY、zh-TW、ja-JP 同理
}

export const i18n = createI18n({
  legacy: false,
  globalInjection: true,
  locale: 'zh-CN',
  fallbackLocale: 'en-US',
  messages,
})
```

### 关键设计说明

| 层级 | 文件位置 | 优先级 | 说明 |
|------|---------|:------:|------|
| 组件级 | `*.vue.{locale}.json`（紧贴 .vue 文件） | 最高 | 修改后直接生效，便于人工维护 |
| 主语言文件 | `locales/{locale}.json` | 中高 | 仅保留嵌套结构键（app/menu/route） |
| enums | `locales/enums/{locale}.json` | 中 | 枚举值翻译 |
| generated | `locales/generated/{locale}.json` | 中低 | Generator 自动生成的 ErrorCode 翻译 |
| common | `locales/common/{locale}.json` | 最低 | 全局复用翻译，组件 null 值从此获取 |

### null 值机制

组件 JSON 中值为 `null` 的 key 不会被 `mergeComponentMessages` 收集，因此该 key 的翻译由更低优先级的层（common/generated/主语言文件）提供。这是"使用全局 common"的显式声明方式。

---

## 执行步骤

### 第 1 步：配置 locales/index.ts（组件级文件自动加载）

按照上述「locales/index.ts 实现模板」修改 `src/locales/index.ts`：

1. 添加 `import.meta.glob` 导入所有 `*.vue.{locale}.json` 文件（5 种语言各一组 glob）
2. 实现 `mergeComponentMessages` 函数过滤 null 值
3. 在消息合并中将组件级消息放在最后（最高优先级）

### 第 2 步：确认 generated 目录

1. 执行 `dotnet build YTStd.slnx` 确保 Generator 已生成 `locales/generated/{locale}.json`
2. 确认 JSON 格式为 `{ "100001": "用户不存在", ... }`（Code → 文本映射）

### 第 3 步：创建全局 common 资源

在 `locales/common/` 下创建 5 个语言文件：

- `zh-CN.json`、`en-US.json`、`ms-MY.json`、`zh-TW.json`、`ja-JP.json`

包含全局复用的文本：按钮文本（编辑/删除/保存）、表单标签（用户名/状态）、通用操作等。

### 第 4 步：逐组件创建语言文件

对每个已完成的页面组件：

1. 确认组件路径（如 `src/views/User/UserView.vue`）
2. 创建紧贴的 5 个语言文件（`UserView.vue.zh-CN.json` + `UserView.vue.en-US.json` + `UserView.vue.ms-MY.json` + `UserView.vue.zh-TW.json` + `UserView.vue.ja-JP.json`）
3. 在 JSON 中声明 key：自有文本写具体翻译值，复用文本写 `null`
4. 在组件中将硬编码文本替换为 `$t('中文key')` 或 `t('中文key')`

**重要**：组件特有的翻译 key 必须写在组件级语言文件中，**禁止**放在主语言文件 `locales/{locale}.json` 中。主语言文件仅保留嵌套结构键（`app`、`languages`、`menu`、`route`、`components`、`status`、`enum` 等）。

### 第 5 步：配置 DevExtreme 组件本地化

**必须先通过 dxdocs 查阅 DevExtreme 本地化文档：**

```
devexpress_docs_search(technologies: ["Vue"], question: "DevExtreme localization locale loadMessages formatMessage")
```

查阅后调用 `devexpress_docs_get_content` 获取全文，基于文档中的代码示例进行配置。

示例参考（以文档为准）：

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

### 第 7 步：校验与构建

1. 检查所有组件 `.vue` 文件旁有对应的 `.vue.{locale}.json`（5 种语言）
2. 检查 null 值在 common 中有对应 key
3. 检查同一组件的 5 个语言文件 key 集合完全一致
4. 检查主语言文件中不包含组件特有的扁平中文 key
5. 执行 `npm run build` 验证

---

## 组件级语言文件示例

```json
// src/views/User/UserView.vue.zh-CN.json — 自有翻译写值，复用翻译写 null
{
  "用户列表": "用户列表",
  "新增用户": "新增用户",
  "用户名": null,
  "状态": null,
  "创建时间": null,
  "保存": null
}

// src/views/User/UserView.vue.en-US.json — 同样 key 集合，自有翻译写英文值
{
  "用户列表": "User List",
  "新增用户": "Add User",
  "用户名": null,
  "状态": null,
  "创建时间": null,
  "保存": null
}
```

使用方式：

```typescript
$t('用户列表')     // → 组件 JSON 有值 "用户列表"（zh-CN）或 "User List"（en-US）→ 直接使用
$t('用户名')       // → 组件 JSON 值为 null → 跳过 → 从 common 获取
$t('保存')         // → 组件 JSON 值为 null → 跳过 → 从 common 获取
```

**修改组件文件后界面直接生效**：因为 `import.meta.glob` 在构建时自动收集所有 `*.vue.{locale}.json`，合并后以最高优先级覆盖。

---

## common 资源示例

```json
// locales/common/zh-CN.json — 全局复用翻译，组件 null 值从此获取
{
  "用户名": "用户名",
  "状态": "状态",
  "保存": "保存",
  "取消": "取消",
  "确认": "确认",
  "删除": "删除"
}

// locales/common/en-US.json
{
  "用户名": "Username",
  "状态": "Status",
  "保存": "Save",
  "取消": "Cancel",
  "确认": "Confirm",
  "删除": "Delete"
}
```

---

## 主语言文件示例

```json
// locales/zh-CN.json — 仅保留嵌套结构键，禁止包含组件特有的扁平中文 key
{
  "app": { "title": "租户管理平台", "logout": "退出" },
  "languages": { "zh-CN": "简体中文", "en-US": "English" },
  "menu": { "dashboard": "仪表盘", "platformUsers": "用户管理" },
  "route": { ... },
  "status": { ... }
}
```

**禁止在主语言文件中添加组件特有的扁平中文 key**（如 `"用户列表": "用户列表"`）。这些 key 必须放在组件级语言文件中。

---

## 后端错误翻译

后端返回整形 Code，前端通过 t() 查找 generated 语言包翻译：

```typescript
function handleError(result: ApiResult) {
  if (result.Code !== 0) {
    const message = t(result.Code.toString());
    showNotification({ message, type: 'error' });
  }
}
```

---

## 约束

- 基准语言为 zh-CN
- 回退链（优先级从高到低）：组件级 → 主语言文件 → enums → generated → common → key 本身
- 所有 key 使用中文，禁止使用 dot.separated 英文 key（嵌套结构键除外）
- 复用文本使用 `null` 声明，必须在 common 中存在
- 组件特有的 key 必须放在组件级语言文件中，禁止放在主语言文件中
- DevExtreme 组件通过 DevExtreme 自身 locale 机制处理，不通过 t()
- generated 目录允许手动编辑翻译内容（校正翻译），但 Generator 不会覆盖已有 key 的值
- 同一组件的 5 个语言文件（zh-CN/en-US/ms-MY/zh-TW/ja-JP）的 key 集合必须完全一致

---

## 禁止事项

- 禁止在组件模板中硬编码中文（必须使用 `$t()` 或 `t()`）
- 禁止使用 `v-html` 渲染翻译内容（除非明确需要富文本且已消毒）
- 禁止在组件 JSON 中写 `gt('xxx')`（使用 `null` 代替）
- 禁止使用 dot.separated 英文 key（使用中文 key）
- 禁止缺少 zh-CN.json 文件
- 禁止遗漏组件对应的语言文件
- 禁止由前端开发者在 generated 目录新增或删除 key（key 管理由 Generator 负责，翻译内容可手动编辑）
- **禁止将组件特有的扁平中文 key 放在主语言文件（`locales/{locale}.json`）中**

---

## 验收标准

- [ ] `src/locales/index.ts` 使用 `import.meta.glob` 自动加载组件级语言文件
- [ ] `mergeComponentMessages` 函数正确过滤 null 值
- [ ] 组件级消息在消息合并中具有最高优先级
- [ ] 所有用户可见文本使用 `$t()` 或 `t()`
- [ ] 每个 .vue 组件旁有对应的 5 个语言文件（zh-CN/en-US/ms-MY/zh-TW/ja-JP）
- [ ] null 值在 common 中有对应 key
- [ ] 同一组件的 5 个语言文件 key 完全一致
- [ ] 主语言文件中不包含组件特有的扁平中文 key
- [ ] DevExtreme 组件本地化正确
- [ ] 语言切换功能正常（用户偏好 → 租户默认 → 系统默认）
- [ ] 后端错误 Code 在 generated 中有翻译
- [ ] `npm run build` 通过
