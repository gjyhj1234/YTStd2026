# 前端历史问题与反模式清单

> 本文件总结前端开发中已确认的历史问题和反模式。
> 所有前端任务 Agent 必须阅读本文件，避免重蹈覆辙。
> 每一条都来自真实的代码审查或 bug 修复。

---

## 一、DevExtreme 组件使用反模式

### 反模式 1：登录页 DxForm 使用 floating label-mode

**问题**：浏览器自动填充用户名/密码时，DevExtreme `floating` label 不感知填充值，导致 label 与值重叠。这是 DevExtreme 的已知行为，无法通过 CSS 修复。

**正确做法**：登录页 DxForm 的 `label-mode` 必须使用 `"static"`。

```vue
<!-- ✅ 正确 -->
<DxForm label-mode="static">

<!-- ❌ 错误 -->
<DxForm label-mode="floating">
```

**来源**：`web/tenant-platform-web/src/views/login/LoginView.vue`

---

### 反模式 2：DxColumn caption 硬编码

**问题**：DxColumn 的 `caption` 属性如果使用硬编码字符串，切换语言后不会更新。

**正确做法**：必须使用 `:caption="$t('...')"` 绑定。

```vue
<!-- ✅ 正确 -->
<DxColumn data-field="Code" :caption="$t('角色编码')" />

<!-- ❌ 错误 -->
<DxColumn data-field="Code" caption="角色编码" />
```

**检查命令**：`grep -rn 'caption="' --include='*.vue' | grep -v ':caption'`

---

### 反模式 3：DxTreeView 侧边栏选中态偏移

**问题**：DxTreeView 用于侧边栏时，点击子菜单后出现靠左对齐偏移。原因是 `font-weight` 变化导致文字宽度变化，进而影响布局。

**正确做法**：

- 使用固定 padding（20px）为子容器
- 使用 `text-shadow` 替代 `font-weight` 变化模拟加粗效果
- 对 `.dx-state-focused`、`.dx-state-active`、`.dx-state-selected`、`.dx-state-hover` 使用 `!important` 覆盖默认样式

**来源**：`web/tenant-platform-web/src/styles/global.css:114-142`

---

### 反模式 4：未查阅 dxdocs 凭印象实现

**问题**：Agent 凭印象使用 DevExtreme 组件 API，导致属性名错误、事件名不准确、配置不生效。

**正确做法**：首次使用任何 DevExtreme 组件时，必须通过 `devexpress_docs_search` + `devexpress_docs_get_content` 查阅官方文档。

---

## 二、国际化反模式

### 反模式 5：notifySuccess / confirmAction 双重 t()

**问题**：`useNotify` 的 `notifySuccess` / `confirmAction` 内部已调用 `t()` 翻译。如果调用方再用 `t()` 包裹，会导致双重翻译或翻译失败。

**正确做法**：仅传 i18n key 字符串，不用 `t()` 包裹。

```typescript
// ✅ 正确
notifySuccess('创建成功')
confirmAction('确认启用此角色')

// ❌ 错误
notifySuccess(t('创建成功'))
confirmAction(t('确认启用此角色'))
```

**检查命令**：`grep -rn "notifySuccess(t(" --include='*.vue'`

**来源**：`web/tenant-platform-web/src/composables/useNotify.ts:5-7`

---

### 反模式 6：组件特有 key 放入主语言文件

**问题**：组件特有的翻译 key 被放入 `src/locales/{locale}.json` 主语言文件中，导致主语言文件膨胀、key 归属不清、维护困难。

**正确做法**：组件特有的 key 必须放在组件级语言文件中（`*.vue.{locale}.json`）。复用文本在组件 JSON 中写 `null`，由 common 提供翻译。

**来源**：`web/tenant-platform-web/src/locales/index.ts:32-72`

---

### 反模式 7：语言文件不完整

**问题**：只创建了 `zh-CN` 语言文件，遗漏其他 4 种语言。或者不同语言文件的 key 不一致。

**正确做法**：每个 `.vue` 文件必须有对应的 5 个语言文件，且 key 完全一致。

```
ComponentName.vue
ComponentName.vue.zh-CN.json  ← 必须
ComponentName.vue.en-US.json  ← 必须
ComponentName.vue.ja-JP.json  ← 必须
ComponentName.vue.ms-MY.json  ← 必须
ComponentName.vue.zh-TW.json  ← 必须
```

---

### 反模式 8：notify key 仅存在于组件级文件

**问题**：`useNotify` 内部使用 `i18n.global.t()`（全局作用域），而非组件作用域的 `t()`。如果 notify 用的 key 仅存在于组件级 `*.vue.*.json` 文件中，在某些场景下可能查找不到。

**正确做法**：notify/confirm 使用的 key 必须在 `common/*.json` 或其他全局语言文件中存在。

**来源**：`web/tenant-platform-web/src/composables/useNotify.ts:5-7`，`web/tenant-platform-web/src/locales/index.ts:44-49`

---

## 三、HTTP 与数据访问反模式

### 反模式 9：使用原生 fetch 而非 axios

**问题**：旧项目使用原生 fetch，缺少拦截器、自动重试、请求取消等标准化能力。

**正确做法**：新前端项目统一使用 axios，遵循 `05-axios-standard.md` 规范。

---

### 反模式 10：ApiResult 拆包不统一

**问题**：不同页面对 `ApiResult<T>` 的处理方式不一致——有的直接取 `.Data`，有的先检查 `.Code`，有的忽略错误。

**正确做法**：在 axios 响应拦截器中统一拆包，业务层只接收 `.Data` 或抛出标准错误。

---

## 四、提示词质量反模式

### 反模式 11：业务提示词过于笼统

**问题**：业务提示词只写"实现用户列表""实现查询功能"，Agent 实现时遗漏字段、按钮、查询条件、权限控制。

**正确做法**：业务提示词必须详细到每个字段、每个按钮、每个查询条件、每个权限码，参见 `07-business-prompt-template.md`。

---

### 反模式 12：验收标准只写"编译通过"

**问题**：编译通过不代表功能完整。页面可能缺少查询条件、缺少操作按钮、缺少权限控制、缺少分页。

**正确做法**：验收标准必须是逐项功能点 checklist，且按 P0→P1→P2→P3 优先级排列。

---

### 反模式 13：提示词中存在字符乱码

**问题**：提示词文件中出现 UTF-8 截断或编码错误导致的乱码（如中文字符显示为不可识别的碎片），Agent 读取后产生误解。

**正确做法**：所有提示词文件必须经过乱码检查（见 `00-governance.md` 第五节）。

---

## 五、项目结构反模式

### 反模式 14：误删旧前端目录

**问题**：Agent 在重建新前端时误删 `web/tenant-platform-web`。

**正确做法**：旧目录必须保留，新项目在 `src/WebTenantPlatfrom` 下创建。

---

### 反模式 15：静态配置未拆分

**问题**：表格列定义、查询表单配置、状态字典等静态数据全部内联在 `.vue` 文件中，导致文件过大、难以维护。

**正确做法**：静态配置应拆分为独立 `.ts` 文件（如 `columns.ts`、`query-form.ts`、`status.ts`），并确保其中的文本有明确的国际化归属。

---

## 六、检查命令速查表

```bash
# 检查 caption 硬编码
grep -rn 'caption="' --include='*.vue' | grep -v ':caption'

# 检查 notifySuccess 双重 t()
grep -rn "notifySuccess(t(" --include='*.vue'

# 检查 confirmAction 双重 t()
grep -rn "confirmAction(t(" --include='*.vue'

# 检查 floating label-mode（登录页）
grep -rn 'label-mode="floating"' --include='*.vue'

# 检查 fetch 使用（新项目中应为 0）
grep -rn 'fetch(' src/WebTenantPlatfrom/src/ | grep -v 'node_modules' | grep -v 'import.meta.glob'

# 检查乱码字符
grep -rn $'\xEF\xBF\xBD' .ai/prompts/ src/WebTenantPlatfrom/

# 检查语言文件完整性（找出缺少语言文件的 .vue）
for f in $(find src/WebTenantPlatfrom/src/views -name '*.vue'); do
  for lang in zh-CN en-US ja-JP ms-MY zh-TW; do
    [ ! -f "${f}.${lang}.json" ] && echo "MISSING: ${f}.${lang}.json"
  done
done
```
