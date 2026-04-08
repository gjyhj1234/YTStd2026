# {业务名称} — {模块名} API

## 目标

实现 {模块名} 的后端 API，包含 DTO、应用服务、API 端点。

---

## 前置阅读

- `.ai/rules/backend.md` — 后端开发规范
- `.ai/rules/api-design.md` — API 设计规范
- `.ai/prompts/02-backend/app-service.md` — 应用服务通用规范
- `.ai/prompts/02-backend/endpoint.md` — 端点通用规范
- `database/schema.md` — 本业务表结构
- `backend/error-codes.md` — 本业务错误码

---

## 输入

- 已完成的实体定义
- 已完成的 ErrorCodes

---

## 输出

- `Application/Dtos/{Module}/*.cs` — DTO（ReqDTO + RepDTO）
- `Application/Services/{Module}AppService.cs` — 应用服务
- `Endpoints/{Module}Endpoints.cs` — API 端点
- 更新 `Application/Constants/ErrorCodes.cs`（如需新增错误码）

---

## API 端点设计

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/{resources}` | 分页列表 |
| GET | `/api/{resources}/{id}` | 获取详情 |
| POST | `/api/{resources}` | 创建 |
| PUT | `/api/{resources}/{id}` | 更新 |
| DELETE | `/api/{resources}/{id}` | 删除 |
| ... | ... | ... |

---

## 业务规则

{描述该模块的业务规则、状态流转、校验逻辑等}

---

## 约束

- 所有 InsertAsync 前必须 `entity.Id = await DB.GetNextLongIdAsync()`
- 所有 ApiResult.Fail() 仅传 ErrorCodes.XXX
- CRUD 结果使用 `.Success` 判断
- 唯一字段提供 check-exists 端点

---

## 验收标准

- [ ] API 端点完整
- [ ] DTO 定义正确
- [ ] 业务校验完整
- [ ] 编译通过
- [ ] 基本测试通过
