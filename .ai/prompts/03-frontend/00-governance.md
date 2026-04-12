# 前端总治理提示词

> 本文件是前端提示词体系的顶层入口，定义前端开发的基础规则、技术底线、模板来源、项目路径、质量标准和验收流程。
> 所有前端任务提示词必须引用本文件作为治理依据。

---

## 一、前置阅读清单

Agent 执行任何前端任务前必须阅读以下文件（按顺序）：

| 序号 | 文件 | 用途 |
|:----:|------|------|
| 1 | `.ai/system/agent-contract.md` | Agent 协作契约 |
| 2 | `.ai/rules/global.md` | 全局开发规范 |
| 3 | `.ai/rules/frontend.md` | 前端开发规范 |
| 4 | `.ai/rules/i18n.md` | 国际化规范 |
| 5 | `.ai/context/tech-stack.md` | 技术栈约束 |
| 6 | `.github/copilot-instructions.md` | 关键编码约束（第 7-13 条为前端约束） |
| 7 | 本文件 | 前端总治理 |

---

## 二、项目路径与保留规则

### 2.1 新前端项目路径

**新前端项目目标路径固定为：`src/WebTenantPlatfrom`**

> 注意：路径名称中的拼写 `WebTenantPlatfrom` 为项目约定，不得自行修正。

### 2.2 旧前端项目保留

- 旧前端目录 `web/tenant-platform-web` **必须保留，不得删除**
- 旧项目仅作为参考和回滚依据
- **禁止**输出会导致 Agent 误删 `web/tenant-platform-web` 的描述或命令
- 新提示词中的"重建"一词均指在 `src/WebTenantPlatfrom` 下从零创建新项目

### 2.3 新旧项目区分

| 属性 | 旧项目 | 新项目 |
|------|--------|--------|
| 路径 | `web/tenant-platform-web` | `src/WebTenantPlatfrom` |
| HTTP 方案 | fetch | **axios** |
| 模板来源 | 泛指应用模板 | **DevExtreme Vue Application Template + UI Templates** |
| 状态 | 冻结（仅参考） | 活跃开发 |

---

## 三、技术底线

### 3.1 模板体系（强制约束）

1. **Application Template 必须使用 DevExtreme Vue 的 Application Template**
2. **UI Templates 必须使用 DevExtreme Vue 的 UI Templates**
3. 不得写成泛泛的"应用模板/页面模板"，必须明确写出完整名称
4. 详细规范见 → `03-frontend/04-devextreme-templates.md`

### 3.2 HTTP 方案（强制约束）

1. **新前端统一使用 axios**
2. **禁止使用原生 fetch**
3. 必须遵守 axios 标准化实现规范
4. 详细规范见 → `03-frontend/05-axios-standard.md`

### 3.3 UI 组件库（强制约束）

1. 所有页面、布局、组件必须优先使用 **devextreme-vue**
2. **禁止**默认手写组件替代可由 DevExtreme Vue 提供的标准能力
3. 如确实需要手写组件：
   - 必须先通过 **dxdocs** 检索确认是否有现成能力
   - 若无合适方案，必须停下并请求人工确认
4. **禁止**以下行为：
   - 未确认直接手写表格
   - 未确认直接手写表单控件
   - 未确认直接手写树、分页器、弹窗、抽屉、导航结构
   - 用自定义组件替代可由 DevExtreme Vue 官方能力完成的实现

### 3.4 国际化（强制约束）

1. 所有用户可见文本必须使用 `t()` / `$t()` 国际化
2. 每个 `.vue` 文件必须有对应的 5 个语言文件
3. 详细规范见 → `03-frontend/06-i18n-execution.md`

### 3.5 dxdocs 文档查阅（强制约束）

1. 首次使用任何 DevExtreme 组件时必须查阅 dxdocs
2. 遇到组件行为异常时必须查阅 dxdocs
3. 配置高级功能时必须查阅 dxdocs
4. 详细规范见 → `03-frontend/04-devextreme-templates.md`

---

## 四、质量标准

### 4.1 验收优先级（从高到低）

业务提示词中必须按以下优先级验收：

| 优先级 | 验收维度 | 说明 |
|:------:|---------|------|
| **P0** | 功能点完整性 | 所有页面结构、查询条件、表格列、操作按钮、表单字段是否齐全 |
| **P1** | 业务规则完整性 | 验证规则、权限控制、状态流转、唯一性校验是否完整 |
| **P2** | 国际化完整性 | 5 语言文件是否齐全、key 是否一致、是否有遗漏 |
| **P3** | 编译与构建 | `npm run build` 是否通过 |

> **"编译通过"不是主要验收标准。** 功能点缺失比编译失败更严重。

### 4.2 Code Review 检查点

每个前端任务完成后必须通过以下自动化检查：

```bash
# 1. DxColumn caption 绑定检查（结果必须为 0）
grep -rn 'caption="' src/WebTenantPlatfrom/src/views/ | grep -v ':caption' | grep -v '\.json'

# 2. notifySuccess 双重 t() 检查（结果必须为 0）
grep -rn "notifySuccess(t(" src/WebTenantPlatfrom/src/views/

# 3. confirmAction 双重 t() 检查（结果必须为 0）
grep -rn "confirmAction(t(" src/WebTenantPlatfrom/src/views/

# 4. 硬编码中文检查（应仅出现在 i18n key 或注释中）
grep -rn '[\u4e00-\u9fff]' src/WebTenantPlatfrom/src/views/ --include='*.vue' --include='*.ts' | grep -v "t('" | grep -v '$t(' | grep -v '//' | grep -v 'console'

# 5. 语言文件完整性检查（每个 .vue 必须有 5 个语言文件）
# 详见 06-i18n-execution.md

# 6. fetch 使用检查（结果必须为 0，新项目禁止 fetch）
grep -rn 'fetch(' src/WebTenantPlatfrom/src/ | grep -v 'node_modules' | grep -v 'import.meta.glob'

# 7. label-mode floating 检查（登录页禁止使用 floating）
grep -rn 'label-mode="floating"' src/WebTenantPlatfrom/src/views/login/
```

---

## 五、字符与编码质量检查（强制）

### 5.1 乱码零容忍

所有新提示词文件和 Agent 生成的代码/资源文件中不得包含以下字符：

| 类型 | 示例 | 说明 |
|------|------|------|
| 替换字符 | `�` | Unicode U+FFFD |
| 截断字符 | `状态字段默认值` → `状��字段默认值` | UTF-8 截断 |
| 乱码组合 | `é¢` `ç®¡` | 编码错误产生的多字节乱码 |

### 5.2 必须检查范围

| 文件类型 | 检查内容 |
|---------|---------|
| Markdown 文件 (`.md`) | 标题、正文、代码块中的中文 |
| JSON 语言文件 (`.json`) | 所有 value 值 |
| Vue 文件 (`.vue`) | template 中的文本、script 中的字符串 |
| TypeScript 文件 (`.ts`) | 字符串常量、注释 |

### 5.3 检查命令

```bash
# 检查 U+FFFD 替换字符
grep -rn $'\xEF\xBF\xBD' src/WebTenantPlatfrom/
grep -rn $'\xEF\xBF\xBD' .ai/prompts/03-frontend/
grep -rn $'\xEF\xBF\xBD' .ai/prompts/08-platform/frontend/
```

### 5.4 验收要求

1. 每轮输出必须显式说明是否进行了乱码检查，以及检查结果
2. 如果发现乱码，必须先修复再进入下一轮
3. 乱码检查结果必须包含在 session summary 中

---

## 六、前端提示词体系结构

```
.ai/prompts/03-frontend/
├── 00-governance.md              ← 本文件：总治理入口
├── 01-task-splitting.md          ← 任务拆分规范
├── 02-parallel-execution.md      ← 并行执行与续接规范
├── 03-anti-patterns.md           ← 历史问题与反模式清单
├── 04-devextreme-templates.md    ← DevExtreme Vue 模板/组件/dxdocs 规范
├── 05-axios-standard.md          ← axios 标准化实现规范
├── 06-i18n-execution.md          ← 前端 i18n 执行规范
└── 07-business-prompt-template.md ← 极细化业务实施提示词模板

.ai/prompts/08-platform/frontend/
├── 00-platform-frontend-overview.md  ← 平台前端总览（模块清单、执行顺序）
├── login-page.md                     ← 登录页（极细化样板）
├── platform-user-page.md            ← 平台用户管理页面（极细化样板）
├── platform-role-page.md            ← 平台角色管理页面
├── platform-permission-page.md      ← 平台权限管理页面
├── ... (其他业务模块页面)
└── refactoring-master.md            ← 重构主控文档（保留参考，后续替换）
```

---

## 七、前端任务执行闭环

所有前端任务必须遵循以下执行闭环：

```
分析需求
  ↓
阅读前置文件（本治理文件 + 相关规范）
  ↓
通过 dxdocs 查阅 DevExtreme 文档
  ↓
制定实现计划（report_progress）
  ↓
实现代码
  ↓
编译验证（npm run build）
  ↓
Code Review 自检（第四节检查命令）
  ↓
乱码检查（第五节检查命令）
  ↓
功能点 checklist 验收（P0 → P1 → P2 → P3）
  ↓
修复违规
  ↓
再次编译
  ↓
输出 session summary
```

> **编译和测试通过不是最终验收标准。**
> Agent 在标记任务完成之前，必须完成功能点 checklist 验收和 Code Review 自检。

---

## 八、跨文件引用索引

| 需求 | 参考文件 |
|------|---------|
| 任务如何拆分 | `03-frontend/01-task-splitting.md` |
| 任务如何并行 | `03-frontend/02-parallel-execution.md` |
| 历史踩坑清单 | `03-frontend/03-anti-patterns.md` |
| DevExtreme 模板与组件使用 | `03-frontend/04-devextreme-templates.md` |
| axios 实现规范 | `03-frontend/05-axios-standard.md` |
| i18n 实现细节 | `03-frontend/06-i18n-execution.md` |
| 业务提示词写法模板 | `03-frontend/07-business-prompt-template.md` |
| 平台前端模块清单 | `08-platform/frontend/00-platform-frontend-overview.md` |
| 各模块业务提示词 | `08-platform/frontend/{module}-page.md` |
