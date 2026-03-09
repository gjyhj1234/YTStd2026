using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder.Expressions;

public sealed class SubQueryExpr : SqlExpr
{
    public override SqlExprKind Kind => SqlExprKind.SubQuery;

    public string Sql { get; }
    public PgSqlParam[] Params { get; }

    public SubQueryExpr(string sql, PgSqlParam[]? @params = null)
    {
        Sql = Guard.NotNullOrEmpty(sql);
        Params = @params ?? Array.Empty<PgSqlParam>();
    }

    public SubQueryExpr(PgSqlRenderResult renderResult)
    {
        Sql = Guard.NotNullOrEmpty(renderResult.Sql);
        Params = renderResult.Params;
    }
}
