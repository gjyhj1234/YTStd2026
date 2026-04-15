# 前端通用组件与交互规范

> 本文档定义了平台前端所有模块必须遵守的通用组件和交互规范。
> 所有后续模块开发必须复用这些标准能力。

---

## 一、功能说明 & 操作指引组件

### 1.1 组件位置

| 组件 | 路径 | 用途 |
|------|------|------|
| FunctionDescriptionCard | `src/components/FunctionDescriptionCard.vue` | 功能说明弹窗（info 图标触发） |
| OperationGuideDrawer | `src/components/OperationGuideDrawer.vue` | 操作指引弹窗（help 图标触发） |

### 1.2 使用方式

每个业务模块页面的标题区域必须包含这两个组件的图标入口：

```vue
<div class="page-header">
  <div class="page-header-text">
    <h2 class="page-title">{{ $t('模块标题') }}</h2>
    <p class="page-subtitle">{{ $t('模块描述') }}</p>
  </div>
  <div class="page-header-actions">
    <FunctionDescriptionCard v-model:visible="showDescription">
      <!-- 功能说明内容：功能用途、字段含义、使用场景 -->
    </FunctionDescriptionCard>
    <OperationGuideDrawer v-model:visible="showGuide">
      <!-- 操作指引内容：分步操作流程 -->
    </OperationGuideDrawer>
  </div>
</div>
```

### 1.3 内容要求

- **功能说明**必须包含：功能用途、字段含义说明、使用场景说明
- **操作指引**必须包含：分步骤操作流程，覆盖完整使用路径
- 所有文本必须走 i18n（组件级语言文件）

---

## 二、统一搜索区域规范

### 2.1 布局结构

所有查询条件统一为一个搜索区域（`.search-area`），不分 basic/advanced 两个区域：

```vue
<div class="search-area">
  <div class="search-row">
    <!-- 基本搜索字段 -->
    <div class="search-field">
      <label class="search-label">{{ $t('关键词') }}</label>
      <DxTextBox ... />
    </div>
    <!-- 高级搜索字段（条件展示） -->
    <template v-if="showAdvanced">
      <div class="search-field">
        <label class="search-label">{{ $t('角色') }}</label>
        <DxSelectBox ... />
      </div>
    </template>
    <!-- 操作按钮始终在末尾 -->
    <div class="search-actions">
      <DxButton :text="$t('查询')" icon="search" type="default" @click="onSearch" />
      <DxButton :text="$t('重置')" icon="refresh" styling-mode="outlined" @click="onReset" />
      <DxButton
        :text="showAdvanced ? $t('收起') : $t('高级查询')"
        :icon="showAdvanced ? 'chevronup' : 'chevrondown'"
        styling-mode="text"
        @click="showAdvanced = !showAdvanced"
      />
    </div>
  </div>
</div>
```

### 2.2 样式规范

- 所有字段使用统一 label 模式（`label` 在上，`input` 在下）
- label 字体 12px, 颜色 #666
- search-row 使用 flex-wrap, gap 16px
- 搜索区域背景 #fafafa, 圆角 4px, padding 16px

---

## 三、Grid 操作列溢出规范

### 3.1 操作列按钮规则

- 前 2 个常用操作（查看、编辑）直接显示为文字按钮
- 超过 2 个的操作使用 `DxDropDownButton`（"更多"下拉菜单）

```vue
<template #actionCell="{ data: cellData }">
  <div class="action-buttons">
    <DxButton text="查看" icon="search" styling-mode="text" @click="..." />
    <DxButton text="编辑" icon="edit" styling-mode="text" @click="..." />
    <DxDropDownButton
      :text="$t('更多')"
      icon="overflow"
      styling-mode="text"
      :items="getMoreActions(cellData.data)"
      display-expr="text"
      key-expr="id"
      :drop-down-options="{ width: 160 }"
      @item-click="onMoreActionClick($event, cellData.data)"
    />
  </div>
</template>
```

### 3.2 操作列宽度

- 使用溢出模式后，操作列宽度控制在 200px（原 420px）

---

## 四、国际化处理规范

### 4.1 枚举/下拉数据源必须用 computed

```typescript
// ✅ 正确 — 使用 computed，切换语言时自动刷新
const statusOptions = computed(() => [
  { value: null, label: t('全部') },
  { value: 'Active', label: t('已启用') },
  { value: 'Disabled', label: t('已禁用') }
])

// ❌ 错误 — 静态数组，语言切换不生效
const statusOptions = [
  { value: null, label: t('全部') },
  ...
]
```

### 4.2 DevExtreme 组件国际化

在 `main.ts` 中加载 DevExtreme 语言包：

```typescript
import { locale as dxLocale, loadMessages } from 'devextreme/localization'
import zhMessages from 'devextreme/localization/messages/zh.json'
import zhTwMessages from 'devextreme/localization/messages/zh-tw.json'
import jaMessages from 'devextreme/localization/messages/ja.json'
import enMessages from 'devextreme/localization/messages/en.json'

loadMessages(zhMessages)
loadMessages(zhTwMessages)
loadMessages(jaMessages)
loadMessages(enMessages)
```

在语言切换时同步 DevExtreme locale：

```typescript
const dxLocaleMap: Record<string, string> = {
  'zh-CN': 'zh', 'zh-TW': 'zh-TW', 'ja-JP': 'ja', 'en-US': 'en', 'ms-MY': 'en'
}
dxLocale(dxLocaleMap[newLocale] || 'en')
```

### 4.3 DxDateRangeBox 必须配置 i18n

```vue
<DxDateRangeBox
  :start-date-label="$t('开始日期')"
  :end-date-label="$t('结束日期')"
  display-format="yyyy-MM-dd"
/>
```

---

## 五、新增/编辑后行高亮

使用 DxDataGrid 的 `focused-row-enabled` + `focused-row-key` 实现：

```vue
<DxDataGrid
  :focused-row-enabled="true"
  :focused-row-key="focusedRowKey"
  :auto-navigate-to-focused-row="true"
>
```

保存成功后设置 focusedRowKey：

```typescript
async function onSubmitForm() {
  // ... save logic ...
  if (savedId) {
    focusedRowKey.value = savedId
  }
}
```

---

## 六、权限分配组件规范

### 6.1 组件说明

权限分配弹窗使用 `DxTreeView`（非 DxTreeList），提供以下标准能力：

| 能力 | 实现方式 |
|------|---------|
| 快捷模板 | 顶部提供预设模板按钮（如"系统管理员"、"只读用户"），点击后自动勾选对应权限 |
| 级联勾选 | `selectNodesRecursive: true`，勾选父级自动全选子级，部分勾选显示半选状态 |
| 实时计数 | 树顶部显示"已选择 12/85 项"，点击可切换过滤已勾选权限 |
| 实时搜索 | 提供搜索框，输入关键字定位权限点 |
| 数据整合 | 加载全部权限树 + 角色已有权限，合并后显示已勾选状态 |
| 取消/保存 | 底部操作栏 |

### 6.2 DxTreeView 配置

```vue
<DxTreeView
  ref="permTreeRef"
  :data-source="permTreeItems"
  :show-check-boxes-mode="'normal'"
  :selection-mode="'multiple'"
  :select-nodes-recursive="true"
  :search-enabled="false"
  :search-value="permSearchText"
  :search-expr="'Name'"
  key-expr="Id"
  parent-id-expr="ParentId"
  display-expr="Name"
  data-structure="plain"
  @selection-changed="onPermTreeSelectionChanged"
/>
```

### 6.3 快捷模板实现模式

```typescript
function applyPermTemplate(templateId: string): void {
  const tree = permTreeRef.value.instance
  if (templateId === 'admin') {
    tree.selectAll()
  } else if (templateId === 'viewer') {
    tree.unselectAll()
    // 仅选中 view/list/detail 权限
    for (const p of allPermItems.value) {
      if (/view|list|detail/i.test(p.Code)) {
        tree.selectItem(p.Id)
      }
    }
  }
}
```

### 6.4 数据结构

权限树使用扁平结构（`data-structure="plain"`），字段：

```typescript
interface FlatPermission {
  Id: number
  Code: string
  Name: string
  PermissionType: string
  ParentId: number | null
}
```

---

## 七、远程排序规范

### 7.1 后端参数

`PagedRequest` 支持 `SortField` 和 `SortOrder` 参数：

| 参数 | 类型 | 说明 |
|------|------|------|
| SortField | string? | 排序字段名（如 Code, Name, CreatedAt） |
| SortOrder | string? | 排序方向（asc / desc） |

### 7.2 CustomStore 排序传递

```typescript
const dataSource = new CustomStore({
  key: 'Id',
  load: async (loadOptions: LoadOptions) => {
    let sortField: string | undefined
    let sortOrder: string | undefined
    if (loadOptions.sort && Array.isArray(loadOptions.sort) && loadOptions.sort.length > 0) {
      const firstSort = loadOptions.sort[0] as { selector?: string; desc?: boolean }
      if (firstSort.selector) {
        sortField = firstSort.selector
        sortOrder = firstSort.desc ? 'desc' : 'asc'
      }
    }
    // 传递给 API
    const result = await getListApi({ ..., SortField: sortField, SortOrder: sortOrder })
    return { data: result.Items, totalCount: result.Total }
  }
})
```

### 7.3 DxColumn 排序配置

- 支持排序的列设置 `:allow-sorting="true"`
- 不支持排序的列设置 `:allow-sorting="false"`

---

## 八、删除操作关联校验规范

### 8.1 后端校验

删除操作必须检查是否存在关联数据：

- 删除角色 → 检查 `sys_user_role` 是否有关联用户
- 删除用户 → 无需额外检查（级联处理）

### 8.2 错误码

| 错误码 | 含义 | 前端展示 |
|--------|------|---------|
| 19111 | 禁止删除超级管理员角色 | error.19111 |
| 19112 | 角色下存在关联用户 | error.19112 |

### 8.3 前端处理

删除失败时，`http.ts` 拦截器自动通过 `error.${code}` 查找 i18n 翻译并展示。

---

## 九、重置密码结果展示

重置密码后必须展示系统生成的新密码：

```vue
<DxPopup :visible="resetPwdResultVisible" :title="$t('重置密码成功')">
  <template #content>
    <div class="reset-pwd-result">
      <p>{{ $t('新密码已生成，请妥善保管：') }}</p>
      <div class="reset-pwd-value">{{ resetPwdNewPassword }}</div>
    </div>
  </template>
</DxPopup>
```

---

## 版本

- 版本：1.0
- 创建日期：2026-04-14
- 用途：定义平台前端通用组件与交互规范，确保后续所有模块统一复用
