namespace YTStdSqlBuilder.Expressions;

public enum SqlExprKind
{
    Column,
    Param,
    Literal,
    Function,
    SubQuery,
    Raw,
    All,
    Case,
    Binary
}
