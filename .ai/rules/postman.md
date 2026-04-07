# Postman 测试规范

## 目标

定义 Postman 测试集合的生成标准和覆盖要求，确保每个后端接口模块都有配套的 Postman 测试。

---

## 适用范围

所有后端 API 端点。

---

## 集合组织方式

### 文件位置

Postman 集合文件位于 `docs/` 目录：

```
docs/
├── YTStd-TenantPlatform-Postman-Collection.json  # 完整集合
└── postman/                                        # 拆分集合（可选）
    ├── auth.json
    ├── platform-users.json
    ├── tenants.json
    └── ...
```

### 集合结构

```
集合根
├── 环境变量
├── Auth（认证）
│   ├── 登录-成功
│   ├── 登录-密码错误
│   └── Token 刷新
├── Platform Users（平台用户）
│   ├── 列表-成功
│   ├── 详情-成功
│   ├── 创建-成功
│   ├── 创建-用户名重复
│   ├── 创建-缺少必填字段
│   ├── 编辑-成功
│   ├── 启用-成功
│   ├── 禁用-成功
│   ├── 删除-成功
│   ├── 检查用户名存在
│   └── 无权限访问
├── Platform Roles（平台角色）
│   └── ...
└── ...
```

---

## 环境变量

### 必须定义的环境变量

```json
{
  "baseUrl": "http://localhost:5000",
  "token": "",
  "adminUsername": "admin",
  "adminPassword": "Admin@123456",
  "testUserId": "",
  "testRoleId": "",
  "testTenantId": ""
}
```

### 变量使用

- 基础 URL：`{{baseUrl}}`
- 认证 Token：`{{token}}`
- 动态 ID：通过前置请求获取并设置到变量

---

## 鉴权方式

### 集合级鉴权

在集合根级别设置 Bearer Token：

```json
{
  "auth": {
    "type": "bearer",
    "bearer": [
      {
        "key": "token",
        "value": "{{token}}"
      }
    ]
  }
}
```

### 登录前置脚本

在集合的 Pre-request Script 中自动登录获取 Token：

```javascript
if (!pm.environment.get("token")) {
  pm.sendRequest({
    url: pm.environment.get("baseUrl") + "/api/auth/login",
    method: "POST",
    header: { "Content-Type": "application/json" },
    body: {
      mode: "raw",
      raw: JSON.stringify({
        Username: pm.environment.get("adminUsername"),
        Password: pm.environment.get("adminPassword")
      })
    }
  }, function(err, response) {
    var json = response.json();
    if (json.Code === 0) {
      pm.environment.set("token", json.Data.Token);
    }
  });
}
```

---

## 每个端点必须覆盖的场景

### 成功场景

每个端点至少一个成功场景，包含：
- 有效的请求参数
- 预期的响应状态码（200）
- 预期的 `Code` 值（0）
- 响应数据结构验证

### 失败场景

根据端点类型，至少覆盖：

| 端点类型 | 必须覆盖的失败场景 |
|---------|-----------------|
| 创建 | 必填字段缺失、唯一性冲突 |
| 更新 | 资源不存在、唯一性冲突 |
| 删除 | 资源不存在 |
| 状态变更 | 状态不允许操作 |
| 所有 | 无权限访问（403） |

### 分页场景（列表接口）

- 默认分页（不传参数）
- 指定页码和页大小
- 关键词搜索
- 状态筛选
- 排序

---

## 断言脚本

### 成功响应断言

```javascript
pm.test("状态码为 200", function() {
  pm.response.to.have.status(200);
});

pm.test("Code 为 0（成功）", function() {
  var json = pm.response.json();
  pm.expect(json.Code).to.eql(0);
});

pm.test("Data 不为空", function() {
  var json = pm.response.json();
  pm.expect(json.Data).to.not.be.null;
});
```

### 列表响应断言

```javascript
pm.test("返回分页数据", function() {
  var json = pm.response.json();
  pm.expect(json.Code).to.eql(0);
  pm.expect(json.Data.Items).to.be.an("array");
  pm.expect(json.Data.Total).to.be.a("number");
  pm.expect(json.Data.Page).to.be.a("number");
  pm.expect(json.Data.PageSize).to.be.a("number");
});
```

### 错误响应断言

```javascript
pm.test("返回唯一性冲突错误", function() {
  var json = pm.response.json();
  pm.expect(json.Code).to.eql(18001); // ErrorCodes.UsernameExists
  pm.expect(json.Message).to.eql("user.username_exists");
});
```

### 权限拒绝断言

```javascript
pm.test("无权限访问返回 403", function() {
  pm.response.to.have.status(403);
});
```

---

## 测试数据管理

### 前置条件

- 每个测试请求的 Pre-request Script 中准备必要数据
- 创建的测试数据保存到环境变量供后续请求使用
- 清理脚本在测试结束后清理测试数据

### 数据隔离

- 测试数据使用特殊前缀（如 `test_`）便于识别和清理
- 不依赖特定的数据库状态
- 每次运行前可以重新初始化

---

## 与开发流程的集成

### 生成时机

每个后端接口模块分块完成时，必须同步生成对应的 Postman 测试：

1. 完成 API 端点实现
2. 生成对应的 Postman 请求
3. 添加断言脚本
4. 验证所有请求可正常执行

### 命名对齐

Postman 请求名必须与 API 路由对齐：

```
API:     PUT /api/tenant-api-keys/{id}/disable
Postman: 禁用 API Key - PUT /api/tenant-api-keys/{id}/disable
```
