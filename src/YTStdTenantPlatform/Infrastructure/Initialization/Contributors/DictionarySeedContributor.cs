using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Initialization.SeedData;

namespace YTStdTenantPlatform.Infrastructure.Initialization.Contributors
{
    /// <summary>字典种子数据贡献者</summary>
    public sealed class DictionarySeedContributor : ISeedContributor
    {
        /// <summary>贡献者名称</summary>
        public string Name => "Dictionary";

        /// <summary>执行顺序</summary>
        public int Order => 55;

        /// <summary>执行幂等初始化</summary>
        public async ValueTask SeedAsync(PlatformSeedContext context)
        {
            int tid = context.TenantId;
            long uid = context.SystemUserId;

            var (result, existingDicts) = await PlatformDictionaryCRUD.GetListAsync(tid, uid);
            var existingKeys = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            if (result.Success && existingDicts != null)
            {
                foreach (var d in existingDicts)
                {
                    existingKeys.Add(d.TypeCode + ":" + d.ItemCode);
                }
            }

            var allSeeds = DefaultDictionaries.GetDefaultDictionaries();
            int insertedCount = 0;

            foreach (var dict in allSeeds)
            {
                string key = dict.TypeCode + ":" + dict.ItemCode;
                if (existingKeys.Contains(key))
                {
                    context.Log("[Dictionary] 字典已存在，跳过: " + key);
                    continue;
                }

                dict.Id = await context.GetNextLongIdAsync();
                DbInsResult ins = await PlatformDictionaryCRUD.InsertAsync(tid, uid, dict);
                if (ins.Success)
                {
                    insertedCount++;
                    context.Log("[Dictionary] 插入字典: " + key + " => Id=" + ins.Id);
                }
                else
                {
                    context.Log("[Dictionary] 插入字典失败: " + key + ", 错误: " + ins.ErrorMessage);
                }
            }

            context.Log("[Dictionary] 共处理字典: " + allSeeds.Count + ", 新增: " + insertedCount);
        }
    }
}
