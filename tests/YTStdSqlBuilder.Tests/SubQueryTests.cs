using Xunit;
using YTStdSqlBuilder;
using YTStdSqlBuilder.Expressions;

namespace YTStdSqlBuilder.Tests;

public class SubQueryTests
{
    [Fact]
    public void ExistsSubQuery_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var subquery = PgSql
            .Select(LiteralExpr.Number(1))
            .From(order)
            .Where(order["user_id"], Op.Eq, user["id"]);

        var existsExpr = Exists.Of(subquery);

        var result = PgSql
            .Select(user["id"], user["name"])
            .From(user)
            .Where(existsExpr)
            .Build();

        Assert.Contains("EXISTS", result.Sql);
        Assert.Contains("SELECT 1 FROM \"orders\" AS \"o\"", result.Sql);
        Assert.Contains("\"o\".\"user_id\" = \"u\".\"id\"", result.Sql);
    }

    [Fact]
    public void InSubQuery_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var subResult = PgSql
            .Select(order["user_id"])
            .From(order)
            .Where(order["status"], Op.Eq, Param.Value("completed"))
            .Build();
        var subExpr = new SubQueryExpr(subResult);

        var result = PgSql
            .Select(user["name"])
            .From(user)
            .Where(user["id"], Op.In, subExpr)
            .Build();

        Assert.Contains("IN (SELECT", result.Sql);
        Assert.Contains("\"o\".\"user_id\"", result.Sql);
    }

    [Fact]
    public void ScalarSubQuery_InSelect_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var subResult = PgSql
            .Select(new FuncExpr("COUNT", order["id"]))
            .From(order)
            .Where(order["user_id"], Op.Eq, user["id"])
            .Build();
        var subExpr = new SubQueryExpr(subResult);

        var result = PgSql
            .Select(user["name"], subExpr.As("order_count"))
            .From(user)
            .Build();

        Assert.Contains("(SELECT COUNT(\"o\".\"id\") FROM \"orders\" AS \"o\"", result.Sql);
        Assert.Contains("AS \"order_count\"", result.Sql);
    }

    [Fact]
    public void SubQuery_ParameterNumbering_ContinuesFromMainQuery()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var subResult = PgSql
            .Select(order["user_id"])
            .From(order)
            .Where(order["status"], Op.Eq, Param.Value("active"))
            .Build();
        var subExpr = new SubQueryExpr(subResult);

        var result = PgSql
            .Select(user["name"])
            .From(user)
            .Where(user["age"], Op.Gte, Param.Value(18))
            .And(user["id"], Op.In, subExpr)
            .Build();

        // Main query param @p0 = 18, subquery param renumbered from @p0 to @p1
        Assert.Contains("@p0", result.Sql);
        Assert.Contains("@p1", result.Sql);
        Assert.Equal(2, result.Params.Length);
        Assert.Equal(18, result.Params[0].Value);
        Assert.Equal("active", result.Params[1].Value);
    }

    [Fact]
    public void CorrelatedSubQuery_ReferencesOuterTable()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var subResult = PgSql
            .Select(new FuncExpr("COUNT", AllExpr.Star))
            .From(order)
            .Where(order["user_id"], Op.Eq, user["id"])
            .And(order["total"], Op.Gt, Param.Value(100m))
            .Build();
        var subExpr = new SubQueryExpr(subResult);

        var result = PgSql
            .Select(user["name"], subExpr.As("big_orders"))
            .From(user)
            .Build();

        Assert.Contains("\"o\".\"user_id\" = \"u\".\"id\"", result.Sql);
        Assert.Single(result.Params);
        Assert.Equal(100m, result.Params[0].Value);
    }
}
