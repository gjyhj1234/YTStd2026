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

---

## 输入

- 已完成的前端页面和组件清单
- 后端 ErrorCodes 常量清单
- 后端枚举定义清单
- `locales/generated/` 下 YTStdI18n.Generator 已生成的语言包

---

## 输出

- `src/locales/runtime/t.ts` — t() 实现（组件 → common → generated → fallback）
- `src/locales/runtime/gt.ts` — gt() 实现（仅查 common）
- `src/locales/runtime/loader.ts` — 语言文件加载器
- `src/locales/generated/{locale}.json` — 由 Generator 自动生成（允许手动编辑翻译，Generator 不覆盖已有 key）
- `src/locales/common/zh-CN.json` — 全局复用资源
- `src/locales/common/en-US.json` — 全局复用资源
- `{Component}.vue.zh-CN.json` — 组件级语言文件（紧贴 .vue 文件）
- `{Component}.vue.en-US.json` — 组件级语言文件
- 页面和组件中的硬编码文本替换为 `t()` 调用
- DevExtreme 组件本地化配置

---

## 执行步骤

### 第 1 步：实现 t()/gt() 运行时

1. 创建 `src/locales/runtime/t.ts` — 实现 t() 分层解析：组件 → common → generated → key
2. 创建 `src/locales/runtime/gt.ts` — 实现 gt() 仅查 common
3. 创建 `src/locales/runtime/loader.ts` — 语言文件加载逻辑

### 第 2 步：确认 generated 目录

1. 执行 `dotnet build YTStd.slnx` 确保 Generator 已生成 `locales/generated/{locale}.json`
2. 确认 JSON 格式为 `{ "100001": "用户不存在", ... }`（Code → 文本映射）

### 第 3 步：创建全局 common 资源

在 `locales/common/` 下创建：

- `zh-CN.json` — 中文全局复用文本
- `en-US.json` — 英文全局复用文本

包含全局复用的文本：按钮文本、表单标签、状态名称、通用操作等。

### 第 4 步：逐组件创建语言文件

对每个已完成的页面组件：

1. 确认组件路径（如 `src/views/User/User.vue`）
2. 创建紧贴的语言文件（`User.vue.zh-CN.json` + `User.vue.en-US.json`）
3. 在 JSON 中声明 key：自有文本写值，复用文本写 `null`
4. 在组件中将硬编码文本替换为 `t('中文key')`

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

### 第 7 步：校验与构建

1. 检查所有组件 `.vue` 文件旁有对应的 `.vue.{locale}.json`
2. 检查 null 值在 common 中有对应 key
3. 检查 zh-CN.json 和 en-US.json 的 key 一致性
4. 执行 `npm run build` 验证

---

## 组件级语言文件示例

```json
// src/views/User/User.vue.zh-CN.json
{
  "用户列表": "用户列表",
  "用户名": null,
  "状态": null,
  "创建时间": "创建时间",
  "新增用户": "新增用户",
  "保存": null
}
```

使用方式：

```typescript
t('用户列表')     // → 从组件 JSON 获取 "用户列表"
t('用户名')       // → null → 从 common 获取
t('保存')         // → null → 从 common 获取
```

---

## common 资源示例

```json
// locales/common/zh-CN.json
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
  "用户名": "User Name",
  "状态": "Status",
  "保存": "Save",
  "取消": "Cancel",
  "确认": "Confirm",
  "删除": "Delete"
}
```

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
- 回退链：组件 → common → generated → key 本身（即中文原文）
- 所有 key 使用中文，禁止使用 dot.separated 英文 key
- 复用文本使用 `null` 声明，必须在 common 中存在
- DevExtreme 组件通过 DevExtreme 自身 locale 机制处理，不通过 t()
- generated 目录允许手动编辑翻译内容（校正翻译），但 Generator 不会覆盖已有 key 的值
- 同一组件的 zh-CN.json 和 en-US.json 的 key 集合必须完全一致

---

## 禁止事项

- 禁止在组件模板中硬编码中文（必须使用 `t()`）
- 禁止使用 `v-html` 渲染翻译内容（除非明确需要富文本且已消毒）
- 禁止在组件 JSON 中写 `gt('xxx')`（使用 `null` 代替）
- 禁止使用 dot.separated 英文 key（使用中文 key）
- 禁止缺少 zh-CN.json 文件
- 禁止遗漏组件对应的语言文件
- 禁止由前端开发者在 generated 目录新增或删除 key（key 管理由 Generator 负责，翻译内容可手动编辑）

---

## 验收标准

- [ ] `src/locales/runtime/t.ts` 正确实现 t() 分层解析
- [ ] `src/locales/runtime/gt.ts` 正确实现 gt() 查 common
- [ ] `src/locales/runtime/loader.ts` 正确加载语言文件
- [ ] 所有用户可见文本使用 `t()`
- [ ] 每个 .vue 组件旁有对应的 `.vue.zh-CN.json` 和 `.vue.en-US.json`
- [ ] null 值在 common 中有对应 key
- [ ] zh-CN.json 与 en-US.json 的 key 完全一致
- [ ] DevExtreme 组件本地化正确
- [ ] 语言切换功能正常（用户偏好 → 租户默认 → 系统默认）
- [ ] 后端错误 Code 在 generated 中有翻译
- [ ] `npm run build` 通过
