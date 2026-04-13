# 子任务 14 — F3 集成验证

> **执行前必读**：`.ai/tasks/platform-frontend/00-common-prereqs.md`

---

## 子任务信息

| 属性 | 值 |
|------|---|
| 子任务编号 | 14-F3 |
| 模块名称 | 集成验证 |
| 并行组 | — |
| 依赖任务 | 所有 F2 模块全部完成 |

---

## 任务目标

在所有业务页面（F2-1 ~ F2-16）全部完成后，执行一次全面的集成验证，确保：

1. **全量编译验证**：所有代码编译通过
2. **i18n 完整性验证**：5 种语言文件 key 一致，无遗漏
3. **权限完整性验证**：所有权限码定义与使用一致
4. **E2E 全量回归**：所有模块 E2E 测试通过
5. **自审全量执行**：self-review F1-F7 全部通过

---

## 前置条件

所有以下子任务必须标记为 ✅ 才能执行本任务：
- 01-F2-5 ~ 12-F2-16（全部业务页面）
- 13-E2E（E2E 测试补齐）

---

## 执行步骤

### 步骤 1：全量编译

```bash
cd src/WebTenantPlatfrom && npm run build
```

### 步骤 2：i18n 完整性检查

```bash
# 检查每个 .vue 是否有 5 个语言文件
cd src/WebTenantPlatfrom/src
for vue in $(find views components layouts -name "*.vue"); do
  for lang in zh-CN en-US ja-JP ms-MY zh-TW; do
    if [ ! -f "${vue}.${lang}.json" ]; then
      echo "❌ 缺失: ${vue}.${lang}.json"
    fi
  done
done

# 检查所有同组语言文件 key 一致性
# 取 zh-CN 为基准，对比其他 4 个语言文件的 key 数量
```

### 步骤 3：权限完整性检查

```bash
# 检查 permissions.ts 中定义的权限码是否都被使用
cd src/WebTenantPlatfrom/src
grep -r "platform:" constants/permissions.ts | wc -l
grep -rn "Permissions\." views/ | head -20
```

### 步骤 4：E2E 全量回归

```bash
cd src/WebTenantPlatfrom && npx playwright test
```

### 步骤 5：self-review F1-F7

按 `.ai/system/self-review-protocol.md` 执行全部 7 项审查。

---

## 验收标准

- [ ] `npm run build` 通过
- [ ] 所有 .vue 文件有完整的 5 个语言文件
- [ ] 所有语言文件 key 一致
- [ ] 权限码定义与使用一致
- [ ] 全量 E2E 测试通过
- [ ] self-review F1-F7 全部通过
- [ ] 无乱码字符（U+FFFD）

---

## 续接说明

完成后更新：
1. `.ai/tasks/platform-frontend/README.md` 中 14 行状态 → ✅
2. `.ai/prompts/08-platform/frontend/0000_overview.md` 中 F3-1/F3-2/F3-3 状态 → ✅
3. 输出 session-summary，标记 S5 阶段完成
