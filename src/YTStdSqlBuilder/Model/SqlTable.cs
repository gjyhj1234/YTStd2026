using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder;

public sealed class SqlTable
{
    public string Name { get; }
    public string? Schema { get; }

    public SqlTable(string name, string? schema = null)
    {
        Name = Guard.NotNullOrEmpty(name);
        Schema = schema;
    }

    public SqlTableSource As(string alias) => new(this, alias);
}
