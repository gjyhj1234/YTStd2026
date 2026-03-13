using System.Runtime.CompilerServices;

namespace YTStdSqlBuilder;

/// <summary>
/// SQL 标识符转义工具。提供带引号的表名、列名构建方法。
/// </summary>
public static class SqlIdentifier
{
    /// <summary>
    /// 为标识符添加双引号转义。
    /// </summary>
    /// <param name="name">标识符名称</param>
    /// <returns>转义后的标识符（如 "users"）</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Escape(string name)
    {
        var vsb = new ValueStringBuilder(stackalloc char[64]);
        vsb.Append('"');
        vsb.Append(name);
        vsb.Append('"');
        return vsb.ToString();
    }

    /// <summary>
    /// 构建带可选 schema 前缀的限定标识符。
    /// </summary>
    /// <param name="schema">schema 名称（可为 null）</param>
    /// <param name="name">标识符名称</param>
    /// <returns>限定标识符（如 "public"."users"）</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EscapeQualified(string? schema, string name)
    {
        if (schema is null) return Escape(name);
        var vsb = new ValueStringBuilder(stackalloc char[128]);
        vsb.Append('"');
        vsb.Append(schema);
        vsb.Append("\".\"");
        vsb.Append(name);
        vsb.Append('"');
        return vsb.ToString();
    }

    /// <summary>
    /// 构建 表别名."列名" 形式的限定列标识符。
    /// </summary>
    /// <param name="source">表源</param>
    /// <param name="column">列名</param>
    /// <returns>限定列名（如 "u"."id"）</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EscapeColumn(SqlTableSource source, string column)
    {
        var vsb = new ValueStringBuilder(stackalloc char[128]);
        vsb.Append('"');
        vsb.Append(source.Alias);
        vsb.Append("\".\"");
        vsb.Append(column);
        vsb.Append('"');
        return vsb.ToString();
    }
}
