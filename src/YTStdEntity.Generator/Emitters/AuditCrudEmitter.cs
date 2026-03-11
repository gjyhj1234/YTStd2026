using YTStdEntity.Generator.Models;

namespace YTStdEntity.Generator.Emitters;

/// <summary>审计查询代码生成器：生成审计查询/历史/比较/主从联合审计查询代码</summary>
internal static class AuditCrudEmitter
{
    /// <summary>生成 {Entity}AuditCRUD.g.cs 内容</summary>
    public static string Emit(EntityModel model)
    {
        // TODO: 实现审计查询代码生成
        return $"// Generated AuditCRUD for {model.ClassName}";
    }
}
