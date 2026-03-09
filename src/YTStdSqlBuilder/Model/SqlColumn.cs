using YTStdSqlBuilder.Expressions;
using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder;

public sealed class SqlColumn
{
    public SqlTableSource TableSource { get; }
    public string Name { get; }
    public Type? ClrType { get; }

    public SqlColumn(SqlTableSource tableSource, string name, Type? clrType = null)
    {
        TableSource = Guard.NotNull(tableSource);
        Name = Guard.NotNullOrEmpty(name);
        ClrType = clrType;
    }

    public SqlSelectItem As<T>(string? alias = null) =>
        new(new ColumnExpr(this), alias ?? Name, typeof(T));

    public SqlSelectItem As(string alias) =>
        new(new ColumnExpr(this), alias, ClrType);
}
