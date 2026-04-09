using System;
using System.Collections.Generic;
using YTStdTenantPlatform.Entity.TenantPlatform;

namespace YTStdTenantPlatform.Infrastructure.Initialization.SeedData
{
    /// <summary>菜单种子数据项，包含菜单实体和父级编码</summary>
    public sealed class MenuSeed
    {
        /// <summary>菜单实体</summary>
        public PlatformMenu Menu { get; }

        /// <summary>父级菜单编码（用于树形结构解析，不会写入数据库）</summary>
        public string? ParentCode { get; }

        /// <summary>构造菜单种子数据项</summary>
        public MenuSeed(PlatformMenu menu, string? parentCode)
        {
            Menu = menu;
            ParentCode = parentCode;
        }
    }

    /// <summary>默认平台菜单种子数据</summary>
    public static class DefaultMenus
    {
        /// <summary>获取默认菜单列表（包含父级编码引用）</summary>
        public static IReadOnlyList<MenuSeed> GetDefaultMenus()
        {
            var now = DateTime.UtcNow;
            var list = new List<MenuSeed>();

            // ── 1. 平台管理体系 ──
            list.Add(Dir("menu:platform", "平台管理体系", "shield", null, 10, now));
            list.Add(Page("menu:platform:user", "平台用户管理", "user", "/platform/users", "views/platform/users/index", "platform:user", "menu:platform", 10, now));
            list.Add(Page("menu:platform:role", "平台角色管理", "users", "/platform/roles", "views/platform/roles/index", "platform:role", "menu:platform", 20, now));
            list.Add(Page("menu:platform:permission", "平台权限管理", "lock", "/platform/permissions", "views/platform/permissions/index", "platform:permission", "menu:platform", 30, now));
            list.Add(Page("menu:platform:security", "平台安全管理", "key", "/platform/security", "views/platform/security/index", "platform:security", "menu:platform", 40, now));

            // ── 2. 租户生命周期体系 ──
            list.Add(Dir("menu:tenant-lifecycle", "租户生命周期", "refresh-cw", null, 20, now));
            list.Add(Page("menu:tenant-lifecycle:list", "租户列表", "list", "/tenants", "views/tenants/index", "tenant:list", "menu:tenant-lifecycle", 10, now));
            list.Add(Page("menu:tenant-lifecycle:init", "租户初始化", "play-circle", "/tenants/init", "views/tenants/init/index", "tenant:list", "menu:tenant-lifecycle", 20, now));
            list.Add(Page("menu:tenant-lifecycle:status", "租户状态管理", "activity", "/tenants/status", "views/tenants/status/index", "tenant:list", "menu:tenant-lifecycle", 30, now));
            list.Add(Page("menu:tenant-lifecycle:data", "租户数据管理", "database", "/tenants/data", "views/tenants/data/index", "tenant:list", "menu:tenant-lifecycle", 40, now));

            // ── 3. 租户信息体系 ──
            list.Add(Dir("menu:tenant-info", "租户信息", "info", null, 30, now));
            list.Add(Page("menu:tenant-info:group", "租户分组", "folder", "/tenants/groups", "views/tenants/groups/index", "tenant:group", "menu:tenant-info", 10, now));
            list.Add(Page("menu:tenant-info:domain", "域名管理", "globe", "/tenants/domains", "views/tenants/domains/index", "tenant:domain", "menu:tenant-info", 20, now));
            list.Add(Page("menu:tenant-info:tag", "租户标签", "tag", "/tenants/tags", "views/tenants/tags/index", "tenant:tag", "menu:tenant-info", 30, now));

            // ── 4. 租户资源管理 ──
            list.Add(Dir("menu:tenant-resource", "租户资源管理", "cpu", null, 40, now));
            list.Add(Page("menu:tenant-resource:quota", "资源配额", "sliders", "/tenants/resources/quota", "views/tenants/resources/quota/index", "tenant:config", "menu:tenant-resource", 10, now));
            list.Add(Page("menu:tenant-resource:usage", "使用统计", "bar-chart-2", "/tenants/resources/usage", "views/tenants/resources/usage/index", "tenant:config", "menu:tenant-resource", 20, now));

            // ── 5. 租户配置中心 ──
            list.Add(Dir("menu:tenant-config", "租户配置中心", "settings", null, 50, now));
            list.Add(Page("menu:tenant-config:system", "系统配置", "tool", "/tenants/config/system", "views/tenants/config/system/index", "tenant:config", "menu:tenant-config", 10, now));
            list.Add(Page("menu:tenant-config:feature", "功能开关", "toggle-left", "/tenants/config/features", "views/tenants/config/features/index", "tenant:config", "menu:tenant-config", 20, now));
            list.Add(Page("menu:tenant-config:param", "参数配置", "file-text", "/tenants/config/params", "views/tenants/config/params/index", "tenant:config", "menu:tenant-config", 30, now));
            list.Add(Page("menu:tenant-config:brand", "UI品牌化", "palette", "/tenants/config/brand", "views/tenants/config/brand/index", "tenant:config", "menu:tenant-config", 40, now));

            // ── 6. SaaS 套餐系统 ──
            list.Add(Dir("menu:package", "SaaS套餐系统", "package", null, 60, now));
            list.Add(Page("menu:package:list", "套餐管理", "box", "/packages", "views/packages/index", "package:list", "menu:package", 10, now));
            list.Add(Page("menu:package:version", "套餐版本", "git-branch", "/packages/versions", "views/packages/versions/index", "package:version", "menu:package", 20, now));
            list.Add(Page("menu:package:capability", "套餐能力", "zap", "/packages/capabilities", "views/packages/capabilities/index", "package:capability", "menu:package", 30, now));

            // ── 7. 订阅系统 ──
            list.Add(Dir("menu:subscription", "订阅系统", "credit-card", null, 70, now));
            list.Add(Page("menu:subscription:list", "订阅列表", "list", "/subscriptions", "views/subscriptions/index", "subscription:list", "menu:subscription", 10, now));
            list.Add(Page("menu:subscription:trial", "试用管理", "clock", "/subscriptions/trials", "views/subscriptions/trials/index", "subscription:trial", "menu:subscription", 20, now));
            list.Add(Page("menu:subscription:change", "变更记录", "file-minus", "/subscriptions/changes", "views/subscriptions/changes/index", "subscription:change", "menu:subscription", 30, now));

            // ── 8. 计费与账单系统 ──
            list.Add(Dir("menu:billing", "计费与账单", "dollar-sign", null, 80, now));
            list.Add(Page("menu:billing:invoice", "账单管理", "file-text", "/billing/invoices", "views/billing/invoices/index", "billing:invoice", "menu:billing", 10, now));
            list.Add(Page("menu:billing:payment", "支付记录", "credit-card", "/billing/payments", "views/billing/payments/index", "billing:payment", "menu:billing", 20, now));
            list.Add(Page("menu:billing:refund", "退款记录", "rotate-ccw", "/billing/refunds", "views/billing/refunds/index", "billing:payment", "menu:billing", 30, now));

            // ── 9. API 与集成平台 ──
            list.Add(Dir("menu:api", "API与集成", "code", null, 90, now));
            list.Add(Page("menu:api:key", "API密钥管理", "key", "/api-platform/keys", "views/api-platform/keys/index", "infrastructure:management", "menu:api", 10, now));
            list.Add(Page("menu:api:webhook", "Webhook管理", "share-2", "/api-platform/webhooks", "views/api-platform/webhooks/index", "infrastructure:management", "menu:api", 20, now));

            // ── 10. 平台运营体系 ──
            list.Add(Dir("menu:operation", "平台运营", "trending-up", null, 100, now));
            list.Add(Page("menu:operation:tenant-stat", "租户统计", "pie-chart", "/operations/tenant-stats", "views/operations/tenant-stats/index", "infrastructure:management", "menu:operation", 10, now));
            list.Add(Page("menu:operation:usage-stat", "使用统计", "bar-chart", "/operations/usage-stats", "views/operations/usage-stats/index", "infrastructure:management", "menu:operation", 20, now));
            list.Add(Page("menu:operation:monitor", "平台监控", "monitor", "/operations/monitor", "views/operations/monitor/index", "infrastructure:management", "menu:operation", 30, now));

            // ── 11. 日志与审计 ──
            list.Add(Dir("menu:log", "日志与审计", "file-text", null, 110, now));
            list.Add(Page("menu:log:operation", "操作日志", "edit", "/logs/operations", "views/logs/operations/index", "log:operation", "menu:log", 10, now));
            list.Add(Page("menu:log:audit", "审计日志", "eye", "/logs/audits", "views/logs/audits/index", "log:audit", "menu:log", 20, now));
            list.Add(Page("menu:log:login", "登录日志", "log-in", "/logs/logins", "views/logs/logins/index", "log:operation", "menu:log", 30, now));
            list.Add(Page("menu:log:system", "系统日志", "terminal", "/logs/system", "views/logs/system/index", "log:system", "menu:log", 40, now));

            // ── 12. 通知系统 ──
            list.Add(Dir("menu:notification", "通知系统", "bell", null, 120, now));
            list.Add(Page("menu:notification:template", "通知模板", "mail", "/notifications/templates", "views/notifications/templates/index", "notification:template", "menu:notification", 10, now));
            list.Add(Page("menu:notification:record", "通知记录", "inbox", "/notifications/records", "views/notifications/records/index", "notification:record", "menu:notification", 20, now));

            // ── 13. 文件与存储 ──
            list.Add(Dir("menu:storage", "文件与存储", "hard-drive", null, 130, now));
            list.Add(Page("menu:storage:policy", "存储策略", "settings", "/storage/policies", "views/storage/policies/index", "infrastructure:management", "menu:storage", 10, now));
            list.Add(Page("menu:storage:file", "文件管理", "file", "/storage/files", "views/storage/files/index", "infrastructure:management", "menu:storage", 20, now));

            // ── 14. 系统设置 ──
            list.Add(Dir("menu:system", "系统设置", "settings", null, 140, now));
            list.Add(Page("menu:system:menu", "菜单管理", "menu", "/system/menus", "views/system/menus/index", "platform:permission", "menu:system", 10, now));
            list.Add(Page("menu:system:dict", "字典管理", "book-open", "/system/dictionaries", "views/system/dictionaries/index", "platform:management", "menu:system", 20, now));

            return list;
        }

        /// <summary>创建目录菜单</summary>
        private static MenuSeed Dir(string code, string name, string icon, string? parentCode, int sortOrder, DateTime now)
        {
            return new MenuSeed(
                new PlatformMenu
                {
                    Code = code,
                    Name = name,
                    Icon = icon,
                    MenuType = 1,
                    IsEnabled = true,
                    IsVisible = true,
                    SortOrder = sortOrder,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                parentCode);
        }

        /// <summary>创建页面菜单</summary>
        private static MenuSeed Page(string code, string name, string icon, string routePath, string componentPath, string permissionCode, string parentCode, int sortOrder, DateTime now)
        {
            return new MenuSeed(
                new PlatformMenu
                {
                    Code = code,
                    Name = name,
                    Icon = icon,
                    RoutePath = routePath,
                    ComponentPath = componentPath,
                    PermissionCode = permissionCode,
                    MenuType = 2,
                    IsEnabled = true,
                    IsVisible = true,
                    SortOrder = sortOrder,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                parentCode);
        }
    }
}
