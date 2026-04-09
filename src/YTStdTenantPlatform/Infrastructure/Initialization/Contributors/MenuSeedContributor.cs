using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Initialization.SeedData;

namespace YTStdTenantPlatform.Infrastructure.Initialization.Contributors
{
    /// <summary>菜单种子数据贡献者</summary>
    public sealed class MenuSeedContributor : ISeedContributor
    {
        /// <summary>贡献者名称</summary>
        public string Name => "Menu";

        /// <summary>执行顺序</summary>
        public int Order => 45;

        /// <summary>执行幂等初始化</summary>
        public async ValueTask SeedAsync(PlatformSeedContext context)
        {
            int tid = context.TenantId;
            long uid = context.SystemUserId;

            var (result, existingMenus) = await PlatformMenuCRUD.GetListAsync(tid, uid);
            var existingMap = new Dictionary<string, long>(System.StringComparer.OrdinalIgnoreCase);
            if (result.Success && existingMenus != null)
            {
                foreach (var m in existingMenus)
                {
                    if (!string.IsNullOrEmpty(m.Code))
                    {
                        existingMap[m.Code] = m.Id;
                    }
                }
            }

            // 将已存在的菜单 Code → Id 映射预填到上下文
            var menuIdMap = new Dictionary<string, long>(System.StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in existingMap)
            {
                menuIdMap[kvp.Key] = kvp.Value;
            }

            var allSeeds = DefaultMenus.GetDefaultMenus();

            foreach (var seed in allSeeds)
            {
                var menu = seed.Menu;
                if (existingMap.ContainsKey(menu.Code))
                {
                    context.Log("[Menu] 菜单已存在，跳过: " + menu.Code);
                    continue;
                }

                // 解析父级 ID
                if (!string.IsNullOrEmpty(seed.ParentCode))
                {
                    if (menuIdMap.TryGetValue(seed.ParentCode, out long parentId))
                    {
                        menu.ParentId = parentId;
                    }
                    else
                    {
                        context.Log("[Menu] 警告: 父级菜单未找到: " + seed.ParentCode + " (菜单: " + menu.Code + ")");
                    }
                }

                menu.Id = await context.GetNextLongIdAsync();
                DbInsResult ins = await PlatformMenuCRUD.InsertAsync(tid, uid, menu);
                if (ins.Success)
                {
                    menuIdMap[menu.Code] = ins.Id;
                    context.Log("[Menu] 插入菜单: " + menu.Code + " => Id=" + ins.Id);
                }
                else
                {
                    context.Log("[Menu] 插入菜单失败: " + menu.Code + ", 错误: " + ins.ErrorMessage);
                }
            }

            context.Log("[Menu] 共处理菜单: " + allSeeds.Count + ", 映射数: " + menuIdMap.Count);
        }
    }
}
