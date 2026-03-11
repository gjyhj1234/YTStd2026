using YTStdEntity.Generator.Models;

namespace YTStdEntity.Generator.Emitters;

/// <summary>描述类代码生成器：生成字段常量/元数据/索引器代码</summary>
internal static class DescEmitter
{
    /// <summary>生成 {Entity}Desc.g.cs 内容</summary>
    public static string Emit(EntityModel model)
    {
        // TODO: 实现描述类代码生成
        return $"// Generated Desc for {model.ClassName}";
    }
}
