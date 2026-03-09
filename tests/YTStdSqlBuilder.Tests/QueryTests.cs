using Xunit;
using YTStdSqlBuilder;
using YTStdSqlBuilder.Expressions;

namespace YTStdSqlBuilder.Tests;

public class QueryTests
{
    [Fact]
    public void Select_BasicQueryWithWhere_GeneratesCorrectSql()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"].As<int>("id"), user["name"])
            .From(user)
            .Where(user["age"], Op.Gte, Param.Value(18))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\" AS \"id\", \"u\".\"name\" FROM \"users\" AS \"u\" WHERE \"u\".\"age\" >= @p0",
            result.Sql);
        Assert.Single(result.Params);
        Assert.Equal("@p0", result.Params[0].Name);
        Assert.Equal(18, result.Params[0].Value);
    }

    [Fact]
    public void Select_WithAlias_RendersAsClause()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["email"].As("user_email"))
            .From(user)
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"email\" AS \"user_email\" FROM \"users\" AS \"u\"",
            result.Sql);
        Assert.Empty(result.Params);
    }

    [Fact]
    public void Select_MultipleColumns_CommaSeparated()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"], user["name"], user["email"])
            .From(user)
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\", \"u\".\"name\", \"u\".\"email\" FROM \"users\" AS \"u\"",
            result.Sql);
    }

    [Fact]
    public void Select_Star_RendersStar()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(AllExpr.Star)
            .From(user)
            .Build();

        Assert.Equal("SELECT * FROM \"users\" AS \"u\"", result.Sql);
    }

    [Fact]
    public void Select_Distinct_RendersDistinctKeyword()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["status"])
            .Distinct()
            .From(user)
            .Build();

        Assert.Equal(
            "SELECT DISTINCT \"u\".\"status\" FROM \"users\" AS \"u\"",
            result.Sql);
    }

    [Fact]
    public void Where_EqOperator_RendersEquals()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["name"], Op.Eq, Param.Value("Alice"))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\" FROM \"users\" AS \"u\" WHERE \"u\".\"name\" = @p0",
            result.Sql);
        Assert.Equal("Alice", result.Params[0].Value);
    }

    [Fact]
    public void Where_GtOperator_RendersGreaterThan()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["age"], Op.Gt, Param.Value(21))
            .Build();

        Assert.Contains("\"u\".\"age\" > @p0", result.Sql);
        Assert.Equal(21, result.Params[0].Value);
    }

    [Fact]
    public void Where_LtOperator_RendersLessThan()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["age"], Op.Lt, Param.Value(65))
            .Build();

        Assert.Contains("\"u\".\"age\" < @p0", result.Sql);
        Assert.Equal(65, result.Params[0].Value);
    }

    [Fact]
    public void Where_LteOperator_RendersLessThanOrEqual()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["age"], Op.Lte, Param.Value(30))
            .Build();

        Assert.Contains("\"u\".\"age\" <= @p0", result.Sql);
    }

    [Fact]
    public void Where_LikeOperator_RendersLike()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["name"], Op.Like, Param.Value("%Alice%"))
            .Build();

        Assert.Contains("LIKE @p0", result.Sql);
        Assert.Equal("%Alice%", result.Params[0].Value);
    }

    [Fact]
    public void Where_ILikeOperator_RendersILike()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["name"], Op.ILike, Param.Value("%alice%"))
            .Build();

        Assert.Contains("ILIKE @p0", result.Sql);
    }

    [Fact]
    public void Where_InWithList_RendersExpandedParams()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["status"], Op.In, Param.Value(new[] { "active", "pending" }))
            .Build();

        Assert.Contains("IN (@p0, @p1)", result.Sql);
        Assert.Equal("active", result.Params[0].Value);
        Assert.Equal("pending", result.Params[1].Value);
    }

    [Fact]
    public void WhereIf_True_IncludesCondition()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .WhereIf(true, user["age"], Op.Gte, Param.Value(18))
            .Build();

        Assert.Contains("WHERE", result.Sql);
        Assert.Single(result.Params);
    }

    [Fact]
    public void WhereIf_False_SkipsCondition()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .WhereIf(false, user["age"], Op.Gte, Param.Value(18))
            .Build();

        Assert.DoesNotContain("WHERE", result.Sql);
        Assert.Empty(result.Params);
    }

    [Fact]
    public void AndIf_True_IncludesCondition()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["active"], Op.Eq, Param.Value(true))
            .AndIf(true, user["age"], Op.Gte, Param.Value(18))
            .Build();

        Assert.Contains("AND", result.Sql);
        Assert.Equal(2, result.Params.Length);
    }

    [Fact]
    public void AndIf_False_SkipsCondition()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["active"], Op.Eq, Param.Value(true))
            .AndIf(false, user["age"], Op.Gte, Param.Value(18))
            .Build();

        Assert.DoesNotContain("AND", result.Sql);
        Assert.Single(result.Params);
    }

    [Fact]
    public void OrIf_True_IncludesCondition()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["role"], Op.Eq, Param.Value("admin"))
            .OrIf(true, user["role"], Op.Eq, Param.Value("superadmin"))
            .Build();

        Assert.Contains("OR", result.Sql);
        Assert.Equal(2, result.Params.Length);
    }

    [Fact]
    public void OrIf_False_SkipsCondition()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["role"], Op.Eq, Param.Value("admin"))
            .OrIf(false, user["role"], Op.Eq, Param.Value("superadmin"))
            .Build();

        Assert.DoesNotContain("OR", result.Sql);
        Assert.Single(result.Params);
    }

    [Fact]
    public void LeftJoin_WithOnCondition_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var result = PgSql
            .Select(user["id"], order["total"])
            .From(user)
            .LeftJoin(order, j => j.On(order["user_id"], Op.Eq, user["id"]))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\", \"o\".\"total\" FROM \"users\" AS \"u\" LEFT JOIN \"orders\" AS \"o\" ON \"o\".\"user_id\" = \"u\".\"id\"",
            result.Sql);
    }

    [Fact]
    public void InnerJoin_WithOnCondition_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var result = PgSql
            .Select(user["name"], order["total"])
            .From(user)
            .Join(order, j => j.On(order["user_id"], Op.Eq, user["id"]))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"name\", \"o\".\"total\" FROM \"users\" AS \"u\" INNER JOIN \"orders\" AS \"o\" ON \"o\".\"user_id\" = \"u\".\"id\"",
            result.Sql);
    }

    [Fact]
    public void Join_WithMultipleOnConditions_RendersWithAnd()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var result = PgSql
            .Select(user["name"])
            .From(user)
            .Join(order, j => j
                .On(order["user_id"], Op.Eq, user["id"])
                .And(order["active"], Op.Eq, Param.Value(true)))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"name\" FROM \"users\" AS \"u\" INNER JOIN \"orders\" AS \"o\" ON \"o\".\"user_id\" = \"u\".\"id\" AND \"o\".\"active\" = @p0",
            result.Sql);
        Assert.Equal(true, result.Params[0].Value);
    }

    [Fact]
    public void GroupBy_WithHaving_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");

        var result = PgSql
            .Select(user["status"], new FuncExpr("COUNT", user["id"]).As("cnt"))
            .From(user)
            .GroupBy(user["status"])
            .Having(new FuncExpr("COUNT", user["id"]), Op.Gt, Param.Value(5))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"status\", COUNT(\"u\".\"id\") AS \"cnt\" FROM \"users\" AS \"u\" GROUP BY \"u\".\"status\" HAVING COUNT(\"u\".\"id\") > @p0",
            result.Sql);
        Assert.Equal(5, result.Params[0].Value);
    }

    [Fact]
    public void OrderBy_AscAndDesc_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");

        var result = PgSql
            .Select(user["id"], user["name"])
            .From(user)
            .OrderBy(user["name"].Asc(), user["id"].Desc())
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\", \"u\".\"name\" FROM \"users\" AS \"u\" ORDER BY \"u\".\"name\" ASC, \"u\".\"id\" DESC",
            result.Sql);
    }

    [Fact]
    public void LimitAndOffset_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");

        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Limit(10)
            .Offset(20)
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\" FROM \"users\" AS \"u\" LIMIT 10 OFFSET 20",
            result.Sql);
    }

    [Fact]
    public void CaseExpression_InSelect_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");

        var caseExpr = Case
            .When(user["age"], Op.Lt, Param.Value(18))
            .Then(LiteralExpr.String("minor"))
            .When(user["age"], Op.Lt, Param.Value(65))
            .Then(LiteralExpr.String("adult"))
            .Else(LiteralExpr.String("senior"));

        var result = PgSql
            .Select(user["name"], caseExpr.As("age_group"))
            .From(user)
            .Build();

        Assert.Contains("CASE WHEN", result.Sql);
        Assert.Contains("THEN 'minor'", result.Sql);
        Assert.Contains("THEN 'adult'", result.Sql);
        Assert.Contains("ELSE 'senior' END", result.Sql);
        Assert.Contains("AS \"age_group\"", result.Sql);
        Assert.Equal(2, result.Params.Length);
    }

    [Fact]
    public void CombinedComplexQuery_RendersAllClauses()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var result = PgSql
            .Select(
                user["id"].As<int>("user_id"),
                user["name"],
                new FuncExpr("SUM", order["total"]).As<decimal>("total_spent"))
            .Distinct()
            .From(user)
            .LeftJoin(order, j => j.On(order["user_id"], Op.Eq, user["id"]))
            .Where(user["active"], Op.Eq, Param.Value(true))
            .And(user["age"], Op.Gte, Param.Value(18))
            .GroupBy(user["id"], user["name"])
            .Having(new FuncExpr("SUM", order["total"]), Op.Gt, Param.Value(100m))
            .OrderBy(user["name"].Asc())
            .Limit(50)
            .Offset(0)
            .Build();

        Assert.Contains("SELECT DISTINCT", result.Sql);
        Assert.Contains("LEFT JOIN", result.Sql);
        Assert.Contains("WHERE", result.Sql);
        Assert.Contains("GROUP BY", result.Sql);
        Assert.Contains("HAVING", result.Sql);
        Assert.Contains("ORDER BY", result.Sql);
        Assert.Contains("LIMIT 50", result.Sql);
        Assert.Contains("OFFSET 0", result.Sql);
        Assert.Equal(3, result.Params.Length);
    }
}
