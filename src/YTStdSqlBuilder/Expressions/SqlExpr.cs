namespace YTStdSqlBuilder.Expressions;

public abstract class SqlExpr
{
    public abstract SqlExprKind Kind { get; }

    public SqlSelectItem As<T>(string? alias = null) =>
        new(this, alias, typeof(T));

    public SqlSelectItem As(string alias) =>
        new(this, alias);

    public SqlOrderItem Asc() => new(this, descending: false);

    public SqlOrderItem Desc() => new(this, descending: true);
}
