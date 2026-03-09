namespace YTStdSqlBuilder;

public static class SqlIdentifier
{
    public static string Escape(string name) => $"\"{name}\"";

    public static string EscapeQualified(string? schema, string name) =>
        schema is null ? Escape(name) : $"{Escape(schema)}.{Escape(name)}";

    public static string EscapeColumn(SqlTableSource source, string column) =>
        $"{Escape(source.Alias)}.{Escape(column)}";
}
