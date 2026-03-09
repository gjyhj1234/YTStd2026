using NpgsqlTypes;

namespace YTStdSqlBuilder.Expressions;

public sealed class ParamExpr : SqlExpr
{
    public override SqlExprKind Kind => SqlExprKind.Param;

    public object? Value { get; }
    public NpgsqlDbType? DbType { get; }

    public ParamExpr(object? value, NpgsqlDbType? dbType = null)
    {
        Value = value;
        DbType = dbType;
    }
}
