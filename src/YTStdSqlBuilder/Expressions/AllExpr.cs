namespace YTStdSqlBuilder.Expressions;

public sealed class AllExpr : SqlExpr
{
    public override SqlExprKind Kind => SqlExprKind.All;

    public SqlTableSource? TableSource { get; }

    public AllExpr(SqlTableSource? tableSource = null)
    {
        TableSource = tableSource;
    }

    public static readonly AllExpr Star = new();
}
