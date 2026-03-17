using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Initialization.SeedData;

namespace YTStdTenantPlatform.Infrastructure.Initialization.Contributors
{
    /// <summary>权限种子数据贡献者</summary>
    public sealed class PermissionSeedContributor : ISeedContributor
    {
        /// <summary>贡献者名称</summary>
        public string Name => "Permission";

        /// <summary>执行顺序</summary>
        public int Order => 30;

        /// <summary>执行幂等初始化</summary>
        public async ValueTask SeedAsync(PlatformSeedContext context)
        {
            int tid = context.TenantId;
            long uid = context.SystemUserId;

            var (result, existingPermissions) = await PlatformPermissionCRUD.GetListAsync(tid, uid);
            var existingMap = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
            if (result.Success && existingPermissions != null)
            {
                foreach (var p in existingPermissions)
                {
                    if (!string.IsNullOrEmpty(p.Code))
                    {
                        existingMap[p.Code] = p.Id;
                    }
                }
            }

            // 将已存在的权限预填到 context 中
            foreach (var kvp in existingMap)
            {
                context.PermissionIdMap[kvp.Key] = kvp.Value;
            }

            var allPermissions = DefaultPermissions.GetDefaultPermissions();

            // 按层级插入：先处理无父级的，再处理有父级的
            // Resource 字段临时存放父级 Code
            foreach (var perm in allPermissions)
            {
                if (existingMap.ContainsKey(perm.Code))
                {
                    context.Log("[Permission] 权限已存在，跳过: " + perm.Code);
                    continue;
                }

                // 解析父级 ID
                string? parentCode = perm.Resource;
                if (!string.IsNullOrEmpty(parentCode))
                {
                    if (context.PermissionIdMap.TryGetValue(parentCode, out long parentId))
                    {
                        perm.ParentId = parentId;
                    }
                    else
                    {
                        context.Log("[Permission] 警告: 父级权限未找到: " + parentCode + " (权限: " + perm.Code + ")");
                    }
                }

                // 清除 Resource 中临时存放的父级编码，避免写入数据库
                perm.Resource = null;

                DbInsResult ins = await PlatformPermissionCRUD.InsertAsync(tid, uid, perm);
                if (ins.Success)
                {
                    context.PermissionIdMap[perm.Code] = ins.Id;
                    context.Log("[Permission] 插入权限: " + perm.Code + " => Id=" + ins.Id);
                }
                else
                {
                    context.Log("[Permission] 插入权限失败: " + perm.Code + ", 错误: " + ins.ErrorMessage);
                }
            }

            context.Log("[Permission] 共处理权限: " + allPermissions.Count + ", 映射数: " + context.PermissionIdMap.Count);
        }
    }
}
