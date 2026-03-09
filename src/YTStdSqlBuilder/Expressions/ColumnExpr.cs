using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder.Expressions;

public sealed class ColumnExpr : SqlExpr
{
    public override SqlExprKind Kind => SqlExprKind.Column;

    public SqlColumn Column { get; }

    public ColumnExpr(SqlColumn column)
    {
        Column = Guard.NotNull(column);
    }
}
