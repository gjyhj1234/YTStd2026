using YTStdSqlBuilder.Expressions;
using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder;

public sealed class SqlAssignment
{
    public ColumnExpr Column { get; }
    public SqlExpr Value { get; }

    public SqlAssignment(ColumnExpr column, SqlExpr value)
    {
        Column = Guard.NotNull(column);
        Value = Guard.NotNull(value);
    }
}
