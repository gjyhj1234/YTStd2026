using YTStdEntity.Generator.Models;

namespace YTStdEntity.Generator.Emitters;

/// <summary>DAL 代码生成器：生成建表/视图/索引/审计表/触发器代码</summary>
internal static class DalEmitter
{
    /// <summary>生成 {Entity}DAL.g.cs 内容</summary>
    public static string Emit(EntityModel model)
    {
        // TODO: 实现 DAL 代码生成
        return $"// Generated DAL for {model.ClassName}";
    }
}
