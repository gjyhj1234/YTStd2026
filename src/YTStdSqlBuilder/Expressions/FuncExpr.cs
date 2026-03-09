using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder.Expressions;

public sealed class FuncExpr : SqlExpr
{
    public override SqlExprKind Kind => SqlExprKind.Function;

    public string FunctionName { get; }
    public SqlExpr[] Arguments { get; }

    public FuncExpr(string functionName, params SqlExpr[] arguments)
    {
        FunctionName = Guard.NotNullOrEmpty(functionName);
        Arguments = arguments ?? Array.Empty<SqlExpr>();
    }
}
