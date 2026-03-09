namespace YTStdSqlBuilder;

public readonly struct PgSqlRenderResult
{
    public readonly string Sql;
    public readonly PgSqlParam[] Params;

    public PgSqlRenderResult(string sql, PgSqlParam[] @params)
    {
        Sql = sql;
        Params = @params;
    }
}
