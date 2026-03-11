using YTStdEntity.Generator.Models;

namespace YTStdEntity.Generator.Emitters;

/// <summary>CRUD 代码生成器：生成 Insert/Update/Delete/Get/GetList 代码</summary>
internal static class CrudEmitter
{
    /// <summary>生成 {Entity}CRUD.g.cs 内容</summary>
    public static string Emit(EntityModel model)
    {
        // TODO: 实现 CRUD 代码生成
        return $"// Generated CRUD for {model.ClassName}";
    }
}
