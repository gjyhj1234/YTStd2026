# 解决方案初始化提示词

## 目标

初始化一个新的解决方案项目或在现有解决方案中添加新的子项目。

---

## 适用范围

新建后端项目、测试项目或前端项目时使用。

---

## 前置阅读

- `.ai/system/agent-contract.md`
- `.ai/rules/global.md`
- `.ai/context/tech-stack.md`
- `.ai/context/project-structure.md`

---

## 输入

- 项目名称
- 项目类型（后端主程序 / 类库 / 测试 / 前端）
- 业务模块名称

---

## 执行步骤

### 后端主程序项目

1. 在 `src/` 下创建项目目录
2. 创建 `.csproj` 文件，目标框架 `net10.0`
3. 创建 `Program.cs` 入口
4. 创建 `Bootstrap/` 目录（ServiceRegistration、RouteRegistration、StartupInitialization）
5. 创建 `Application/`、`Domain/`、`Endpoints/`、`Infrastructure/`、`entity/` 目录
6. 将项目添加到 `YTStd.slnx`
7. 执行 `dotnet build YTStd.slnx` 验证

### 后端测试项目

1. 在 `tests/` 下创建项目目录
2. 创建 `.csproj` 文件，引用 xUnit 和被测项目
3. 创建初始测试文件
4. 将项目添加到 `YTStd.slnx`
5. 执行 `dotnet test YTStd.slnx` 验证

### 前端项目

1. 在 `web/` 下创建项目目录
2. 使用 Vite 创建 Vue 3 + TypeScript 项目
3. 安装 DevExtreme Vue、vue-router、pinia、vue-i18n
4. 创建目录结构（api、components、views、router、store、types 等）
5. 配置 Vite 和 TypeScript
6. 执行 `npm run build` 验证

---

## 约束

- 后端项目必须目标 `net10.0`
- 后端项目必须启用 AOT 相关配置
- 前端项目必须使用 DevExtreme Vue
- 所有项目必须纳入解决方案文件

---

## 禁止事项

- 禁止引入不必要的 NuGet/npm 依赖
- 禁止创建与已有项目重复的功能
- 禁止修改已有项目的代码

---

## 验收标准

- [ ] 项目已添加到解决方案
- [ ] 编译通过
- [ ] 目录结构符合规范
- [ ] 项目配置正确
