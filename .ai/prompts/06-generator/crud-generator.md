# CRUD 生成器提示词

## 目标

说明 CRUD 生成器的使用方式和生成后代码的调用模式。

---

## 适用范围

使用生成器产出的 CRUD 代码进行业务开发时参考。

---

## 前置阅读

- `.ai/rules/generator.md`
- `.ai/context/existing-modules.md`

---

## 生成器产出

YTStdEntity.Generator 会为每个标注 `[Entity]` 的类生成以下方法：

```csharp
// 插入 — 返回 DbInsResult
DB.InsertAsync(entity)

// 更新 — 返回 DbUdqResult
DB.UpdateAsync(entity)

// 删除 — 返回 DbUdqResult
DB.DeleteAsync<T>(id)

// 查询单个 — 返回 T?
DB.GetAsync<T>(id)

// 查询列表 — 返回 T[]
DB.GetListAsync<T>(query)
```

---

## 调用注意事项

### ID 生成

```csharp
// 正确 — 插入前必须获取 ID
entity.Id = await DB.GetNextLongIdAsync();
var result = await DB.InsertAsync(entity);

// 错误 — 忘记获取 ID
var result = await DB.InsertAsync(entity); // entity.Id 为 0
```

### 结果判断

```csharp
// 正确 — CRUD 结果使用 .Success
var result = await DB.InsertAsync(entity);
if (!result.Success) { ... }

// 错误 — CRUD 结果没有 .Code（只有 ApiResult 有 Code）
if (result.Code != 0) { ... } // 编译错误
```

### Update 三态

```csharp
// 使用 DbNullable<T> 表示更新时的三态
entity.SomeField = new DbNullable<string>(value);  // 更新为具体值
entity.SomeField = DBNULL.String;                    // 更新为 NULL
// 不赋值则不更新该字段
```

---

## 约束

- 不自行实现 CRUD 方法，使用生成器产出
- 必须理解 DbInsResult/DbUdqResult 与 ApiResult 的区别

---

## 验收标准

- [ ] 正确调用生成的 CRUD 方法
- [ ] 使用 .Success 判断 CRUD 结果
- [ ] 使用 DB.GetNextLongIdAsync() 获取 ID
