# 前端 i18n 执行规范

> 本文件是前端国际化的可执行实施规范，定义所有需要国际化的内容范围、文件结构、翻译归属规则和验收检查。
> 与 `.ai/rules/i18n.md` 互补：`i18n.md` 定义通用规则，本文件定义前端执行细节。

---

## 一、支持语言

| 语言 | 文件后缀 | 说明 |
|------|---------|------|
| 简体中文 | `zh-CN` | 默认语言，key 即中文 |
| 英语 | `en-US` | |
| 日语 | `ja-JP` | |
| 马来语 | `ms-MY` | |
| 繁体中文 | `zh-TW` | |

---

## 二、必须国际化的内容清单

### 2.1 页面级

| 序号 | 内容 | 示例 |
|:----:|------|------|
| 1 | 页面主标题 | `$t('平台用户管理')` |
| 2 | 页面副标题 | `$t('管理平台级用户账号')` |
| 3 | 功能说明卡片标题 | `$t('功能说明')` |
| 4 | 功能说明卡片内容 | `$t('本页面用于管理...')` |
| 5 | 操作指南抽屉标题 | `$t('操作指南')` |
| 6 | 操作指南抽屉内容 | `$t('第一步：...')` |

### 2.2 查询区

| 序号 | 内容 | 示例 |
|:----:|------|------|
| 7 | 查询区标题 | `$t('查询条件')` |
| 8 | 高级查询标题 | `$t('高级查询')` |
| 9 | 查询字段 label | `$t('用户名')` |
| 10 | 查询字段 placeholder | `$t('请输入用户名')` |
| 11 | 查询按钮文本 | `$t('查询')` |
| 12 | 重置按钮文本 | `$t('重置')` |
| 13 | 展开/收起文本 | `$t('展开高级查询')` / `$t('收起')` |

### 2.3 表格区

| 序号 | 内容 | 示例 |
|:----:|------|------|
| 14 | 表格列标题 | `:caption="$t('用户名')"` |
| 15 | 表格空状态文本 | `:no-data-text="$t('暂无数据')"` |
| 16 | 加载状态文本 | 通过 DxLoadPanel |
| 17 | 状态列显示值 | `$t('已启用')` / `$t('已禁用')` |

### 2.4 弹窗 / 抽屉 / Tabs

| 序号 | 内容 | 示例 |
|:----:|------|------|
| 18 | 弹窗标题 | `$t('新增用户')` |
| 19 | 抽屉标题 | `$t('用户详情')` |
| 20 | tabs 标签文本 | `$t('基本信息')` |

### 2.5 表单

| 序号 | 内容 | 示例 |
|:----:|------|------|
| 21 | 表单标题 | `$t('新增用户')` |
| 22 | 字段 label | `$t('用户名')` |
| 23 | 字段 placeholder | `$t('请输入用户名')` |
| 24 | 字段 helpText | `$t('用户名长度 3-50 个字符')` |

### 2.6 按钮与工具栏

| 序号 | 内容 | 示例 |
|:----:|------|------|
| 25 | 按钮文本 | `$t('新增')` |
| 26 | 按钮 tooltip | `$t('点击新增用户')` |
| 27 | toolbar 文本 | `$t('批量操作')` |

### 2.7 操作反馈

| 序号 | 内容 | 示例 |
|:----:|------|------|
| 28 | 验证提示 | 通过 DxForm validation message |
| 29 | 成功提示 | `notifySuccess('创建成功')` |
| 30 | 失败提示 | 通过 axios 拦截器统一处理 |
| 31 | confirm 文案 | `confirmAction('确认启用此用户')` |
| 32 | 删除确认文案 | `confirmDelete(item.Name)` |

### 2.8 导航与状态

| 序号 | 内容 | 示例 |
|:----:|------|------|
| 33 | 菜单文本 | `$t('用户管理')` |
| 34 | 面包屑文本 | `$t('平台管理')` > `$t('用户管理')` |
| 35 | 状态字典 | `$t('已启用')` / `$t('已禁用')` |

---

## 三、文件结构与翻译归属

### 3.1 语言文件层级（优先级从低到高）

```
src/WebTenantPlatfrom/src/locales/
├── generated/              ← Generator 生成（ErrorCode → 文本映射）最低优先级
│   ├── zh-CN.json
│   ├── en-US.json
│   ├── ja-JP.json
│   ├── ms-MY.json
│   └── zh-TW.json
├── common/                 ← 全局复用资源（通用按钮、状态、提示文本）
│   ├── zh-CN.json
│   ├── en-US.json
│   ├── ja-JP.json
│   ├── ms-MY.json
│   └── zh-TW.json
├── enums/                  ← 枚举翻译
│   ├── zh-CN.json
│   └── ...
├── zh-CN.json              ← 主语言文件
├── en-US.json
├── ja-JP.json
├── ms-MY.json
├── zh-TW.json
└── index.ts                ← i18n 初始化 + import.meta.glob 自动加载
```

### 3.2 组件级语言文件

每个 `.vue` 文件必须有紧贴的 5 个语言文件：

```
views/platform-users/
├── PlatformUsersView.vue
├── PlatformUsersView.vue.zh-CN.json
├── PlatformUsersView.vue.en-US.json
├── PlatformUsersView.vue.ja-JP.json
├── PlatformUsersView.vue.ms-MY.json
└── PlatformUsersView.vue.zh-TW.json
```

### 3.3 静态配置文件的翻译归属

静态配置文件（如 `columns.ts`、`query-form.ts` 等）中的所有文本必须使用 `t()` 函数，其翻译 key 归属于**所属页面的组件级语言文件**。

| 静态配置文件 | 翻译归属 | 说明 |
|-------------|---------|------|
| `columns.ts` | 所属页面的 `*.vue.{locale}.json` | 表格列标题 |
| `query-form.ts` | 所属页面的 `*.vue.{locale}.json` | 查询字段 label/placeholder |
| `advanced-query.ts` | 所属页面的 `*.vue.{locale}.json` | 高级查询字段 |
| `form.ts` | 所属页面的 `*.vue.{locale}.json` | 表单字段配置 |
| `toolbar.ts` | 所属页面的 `*.vue.{locale}.json` | 工具栏按钮文本 |
| `guide.ts` | 所属页面的 `*.vue.{locale}.json` | 操作指南内容 |
| `status.ts` | `common/{locale}.json` 或 `enums/{locale}.json` | 状态字典（可跨模块复用） |
| `tabs.ts` | 所属页面的 `*.vue.{locale}.json` | 标签页标题 |

---

## 四、翻译归属规则

### 4.1 归属判定流程

```
某个 i18n key 需要归属到哪里？
  ↓
该 key 是否仅在单个组件/页面中使用？
  ├── 是 → 放入该组件的 *.vue.{locale}.json
  └── 否（多个组件共用）
        ↓
      该 key 是否属于通用操作（如"确定""取消""保存""删除""查询"）？
        ├── 是 → 放入 common/{locale}.json
        └── 否
              ↓
            该 key 是否属于枚举/状态翻译？
              ├── 是 → 放入 enums/{locale}.json
              └── 否 → 放入 common/{locale}.json
```

### 4.2 禁止行为

1. **禁止**把组件特有 key 放入主语言文件 `locales/{locale}.json`
2. **禁止**把所有 key 堆进 common
3. **禁止**组件级语言文件中出现非该组件使用的 key
4. **禁止**不同语言文件的 key 不一致

### 4.3 null 值约定

组件级语言文件中，如果某个 key 的值为 `null`，表示该 key 的翻译由 common 或更高优先级的文件提供。

```json
// PlatformUsersView.vue.zh-CN.json
{
  "平台用户管理": "平台用户管理",
  "管理平台级用户账号": "管理平台级用户账号",
  "查询": null,
  "重置": null,
  "新增": null,
  "确定": null,
  "取消": null
}
```

---

## 五、import.meta.glob 自动加载

`src/locales/index.ts` 必须使用 Vite 的 `import.meta.glob` 自动收集组件级语言文件：

```typescript
// 自动收集 views/、components/、layouts/ 下的组件级语言文件
const componentMessages = import.meta.glob([
  '../views/**/*.vue.*.json',
  '../components/**/*.vue.*.json',
  '../layouts/**/*.vue.*.json',
], { eager: true })

/** 合并组件级消息（过滤 null 值，以最高优先级合并） */
function mergeComponentMessages(locale: string, target: Record<string, string>): void {
  const suffix = `.${locale}.json`
  for (const [path, mod] of Object.entries(componentMessages)) {
    if (!path.endsWith(suffix)) continue
    const messages = (mod as { default: Record<string, string | null> }).default
    for (const [key, value] of Object.entries(messages)) {
      if (value !== null) {
        target[key] = value
      }
    }
  }
}
```

**加载优先级（从低到高）**：

1. `generated/{locale}.json` — Generator 生成
2. `common/{locale}.json` — 全局复用
3. `enums/{locale}.json` — 枚举翻译
4. `{locale}.json` — 主语言文件
5. `*.vue.{locale}.json` — 组件级文件（最高优先级）

---

## 六、notifySuccess / confirmAction 的 i18n 规则

### 6.1 调用规则

```typescript
// ✅ 正确 — 仅传 i18n key，useNotify 内部调用 t()
notifySuccess('创建成功')
confirmAction('确认启用此用户')
confirmDelete(item.Name)

// ❌ 错误 — 禁止双重 t()
notifySuccess(t('创建成功'))
confirmAction(t('确认启用此用户'))
```

### 6.2 key 归属

由于 `useNotify` 使用 `i18n.global.t()`（全局作用域），notify/confirm 使用的 key 必须在**全局可见范围**内：

- 通用操作提示（如"创建成功""更新成功""删除成功"）→ `common/{locale}.json`
- 模块特有提示（如"确认启用此角色"）→ 对应组件级语言文件（会被 import.meta.glob 自动加载到全局）

---

## 七、验收检查

### 7.1 自动化检查命令

```bash
# 1. 检查每个 .vue 是否有 5 个语言文件
for f in $(find src/WebTenantPlatfrom/src/views -name '*.vue'); do
  for lang in zh-CN en-US ja-JP ms-MY zh-TW; do
    [ ! -f "${f}.${lang}.json" ] && echo "MISSING: ${f}.${lang}.json"
  done
done

# 2. 检查 DxColumn caption 是否使用绑定
grep -rn 'caption="' src/WebTenantPlatfrom/src/ --include='*.vue' | grep -v ':caption'

# 3. 检查 notifySuccess/confirmAction 双重 t()
grep -rn "notifySuccess(t(" src/WebTenantPlatfrom/src/ --include='*.vue'
grep -rn "confirmAction(t(" src/WebTenantPlatfrom/src/ --include='*.vue'

# 4. 检查组件级语言文件 key 一致性
# 对比同一组件的 5 个语言文件，确保 key 集合相同
for f in $(find src/WebTenantPlatfrom/src/views -name '*.vue.zh-CN.json'); do
  base="${f%.zh-CN.json}"
  zhKeys=$(python3 -c "import json; print(sorted(json.load(open('$f')).keys()))")
  for lang in en-US ja-JP ms-MY zh-TW; do
    langFile="${base}.${lang}.json"
    if [ -f "$langFile" ]; then
      langKeys=$(python3 -c "import json; print(sorted(json.load(open('$langFile')).keys()))")
      if [ "$zhKeys" != "$langKeys" ]; then
        echo "KEY MISMATCH: $f vs $langFile"
      fi
    fi
  done
done

# 5. 检查主语言文件中是否有组件特有 key（需人工判断）
# 列出主语言文件中的所有 key，人工检查是否有应属于组件级的 key
```

### 7.2 Code Review 检查项

| 检查项 | 通过标准 |
|--------|---------|
| 语言文件完整 | 每个 .vue 有 5 个 .json |
| key 一致 | 同组件 5 个语言文件 key 集合相同 |
| key 归属正确 | 组件特有 key 在组件级文件，通用 key 在 common |
| null 值有对应 | 组件级文件中 null 值的 key 在 common 中存在 |
| caption 绑定 | DxColumn caption 使用 `:caption="$t()"` |
| notify 规范 | notifySuccess/confirmAction 不双重 t() |
| 静态配置覆盖 | columns/query-form/form 等文件中的文本已国际化 |
| 无硬编码中文 | .vue/.ts 中无未经 t() 的硬编码中文 |
