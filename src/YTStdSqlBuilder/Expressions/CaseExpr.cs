using YTStdSqlBuilder.Conditions;

namespace YTStdSqlBuilder.Expressions;

public sealed class CaseExpr : SqlExpr
{
    public override SqlExprKind Kind => SqlExprKind.Case;

    public List<(SqlConditionGroup Condition, SqlExpr Result)> WhenClauses { get; }
    public SqlExpr? ElseResult { get; }

    public CaseExpr(List<(SqlConditionGroup Condition, SqlExpr Result)> whenClauses, SqlExpr? elseResult = null)
    {
        WhenClauses = whenClauses ?? throw new ArgumentNullException(nameof(whenClauses));
        ElseResult = elseResult;
    }
}
