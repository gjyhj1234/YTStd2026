using YTStdSqlBuilder.Expressions;
using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder.Conditions;

public sealed class SqlCondition
{
    public SqlExpr Left { get; }
    public SqlComparisonOperator Operator { get; }
    public SqlExpr? Right { get; }

    public SqlCondition(SqlExpr left, SqlComparisonOperator @operator, SqlExpr? right = null)
    {
        Left = Guard.NotNull(left);
        Operator = @operator;
        Right = right;
    }
}
