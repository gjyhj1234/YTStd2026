using YTStdSqlBuilder.Conditions;
using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder;

public sealed class SqlJoin
{
    public SqlTableSource Table { get; }
    public SqlJoinType JoinType { get; }
    public SqlConditionGroup OnCondition { get; }

    public SqlJoin(SqlTableSource table, SqlJoinType joinType, SqlConditionGroup onCondition)
    {
        Table = Guard.NotNull(table);
        JoinType = joinType;
        OnCondition = Guard.NotNull(onCondition);
    }
}
