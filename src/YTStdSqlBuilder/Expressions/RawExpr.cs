namespace YTStdSqlBuilder.Expressions;

public sealed class RawExpr : SqlExpr
{
    public override SqlExprKind Kind => SqlExprKind.Raw;

    public string SqlText { get; }

    public RawExpr(string sqlText)
    {
        SqlText = sqlText ?? string.Empty;
    }
}
