using YTStdSqlBuilder.Expressions;
using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder;

public sealed class SqlOrderItem
{
    public SqlExpr Expression { get; }
    public bool Descending { get; }

    public SqlOrderItem(SqlExpr expression, bool descending = false)
    {
        Expression = Guard.NotNull(expression);
        Descending = descending;
    }
}
