# 前端国际化提示词

## 目标

为前端项目接入完整的国际化支持，覆盖界面文本、日期时间格式、数字格式和 DevExtreme 组件。

---

## 适用范围

前端国际化实现和维护。

---

## 前置阅读

- `.ai/rules/i18n.md`
- `.ai/rules/frontend.md`

---

## 输入

- 已完成的前端页面
- 后端 Messages 常量清单

---

## 输出

- `src/locales/zh-CN.json` — 简体中文资源（基准）
- `src/locales/en-US.json` — 英语资源
- `src/locales/ms-MY.json` — 马来语资源
- `src/locales/zh-TW.json` — 繁体中文资源
- `src/locales/index.ts` — i18n 实例配置
- 页面和组件中的文本替换为 `$t()` 调用

---

## 执行步骤

1. 配置 vue-i18n 实例（支持回退到 zh-CN）
2. 创建基准语言资源（zh-CN.json）
3. 按模块组织 key：common、menu、auth、views.{module}、validation、enums
4. 扫描所有页面和组件，替换硬编码文本为 `$t()`
5. 配置 DevExtreme 组件本地化
6. 添加语言切换功能
7. 创建其他语言资源文件
8. 确保后端 i18n key 在前端资源中有对应翻译
9. 执行 `npm run build` 验证

---

## 资源 key 结构

```json
{
  "common": { "save": "...", "cancel": "...", "delete": "..." },
  "menu": { "dashboard": "...", "platform_users": "..." },
  "auth": { "invalid_credentials": "...", "token_expired": "..." },
  "views": {
    "platformUsers": {
      "title": "...",
      "columns": { "username": "...", "status": "..." },
      "form": { "username": "...", "password": "..." },
      "description": { "title": "...", "content": "..." },
      "guide": { "title": "...", "step1": "..." }
    }
  },
  "validation": { "field_required": "{field} 不能为空" },
  "enums": { "userStatus": { "0": "启用", "1": "禁用" } }
}
```

---

## DevExtreme 本地化

```typescript
import { locale, loadMessages } from 'devextreme/localization';
import zhMessages from 'devextreme/localization/messages/zh';

loadMessages(zhMessages);
locale('zh');
```

---

## 约束

- 基准语言为 zh-CN
- 回退链：当前 locale → zh-CN → 显示 key
- 所有新增的后端 Messages 必须在前端有翻译
- DevExtreme 组件通过 DevExtreme 自身 locale 机制处理

---

## 禁止事项

- 禁止在组件模板中硬编码中文
- 禁止使用 `v-html` 渲染翻译内容（除非明确需要富文本）
- 禁止忘记更新其他语言文件

---

## 验收标准

- [ ] 所有用户可见文本使用 `$t()`
- [ ] zh-CN 和 en-US 资源文件完整
- [ ] DevExtreme 组件本地化正确
- [ ] 语言切换功能正常
- [ ] 后端错误消息在前端有翻译
- [ ] `npm run build` 通过
