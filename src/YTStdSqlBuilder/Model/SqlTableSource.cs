using YTStdSqlBuilder.Expressions;
using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder;

public sealed class SqlTableSource
{
    public SqlTable Table { get; }
    public string Alias { get; }

    public SqlTableSource(SqlTable table, string alias)
    {
        Table = Guard.NotNull(table);
        Alias = Guard.NotNullOrEmpty(alias);
    }

    public ColumnExpr this[string columnName] => new(new SqlColumn(this, columnName));

    public ColumnExpr Col(string columnName) => this[columnName];

    public ColumnExpr Col<T>(string columnName) => new(new SqlColumn(this, columnName, typeof(T)));
}
