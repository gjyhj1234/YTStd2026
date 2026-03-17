using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using YTStdLogger.Core;
using YTStdTenantPlatform.Bootstrap;

// ──────────────────────────────────────────────────────────────
// 租户平台单体 WebAPI 主程序入口
// AOT 友好的 Minimal API 架构
// ──────────────────────────────────────────────────────────────

var builder = WebApplication.CreateSlimBuilder(args);

// 服务注册
ServiceRegistration.ConfigureServices(builder);

var app = builder.Build();

// 中间件管道配置
ServiceRegistration.ConfigureMiddleware(app);

// 路由注册
RouteRegistration.MapRoutes(app);

// 启动初始化（建表、种子数据、缓存预热）
Logger.Info(0, 0, "[Program] 租户平台启动中...");
await StartupInitialization.InitializeAsync();
Logger.Info(0, 0, "[Program] 租户平台启动完成");

app.Run();
