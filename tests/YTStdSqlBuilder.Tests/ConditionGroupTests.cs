using Xunit;
using YTStdSqlBuilder;
using YTStdSqlBuilder.Expressions;

namespace YTStdSqlBuilder.Tests;

public class ConditionGroupTests
{
    [Fact]
    public void SimpleAndConditions_JoinedWithAnd()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["age"], Op.Gte, Param.Value(18))
            .And(user["active"], Op.Eq, Param.Value(true))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\" FROM \"users\" AS \"u\" WHERE \"u\".\"age\" >= @p0 AND \"u\".\"active\" = @p1",
            result.Sql);
    }

    [Fact]
    public void SimpleOrConditions_JoinedWithOr()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["role"], Op.Eq, Param.Value("admin"))
            .Or(user["role"], Op.Eq, Param.Value("superadmin"))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\" FROM \"users\" AS \"u\" WHERE \"u\".\"role\" = @p0 OR \"u\".\"role\" = @p1",
            result.Sql);
    }

    [Fact]
    public void MixedAndOr_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["age"], Op.Gte, Param.Value(18))
            .And(user["active"], Op.Eq, Param.Value(true))
            .Or(user["role"], Op.Eq, Param.Value("admin"))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\" FROM \"users\" AS \"u\" WHERE \"u\".\"age\" >= @p0 AND \"u\".\"active\" = @p1 OR \"u\".\"role\" = @p2",
            result.Sql);
    }

    [Fact]
    public void WhereGroup_NestedAnd_RendersParentheses()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["active"], Op.Eq, Param.Value(true))
            .AndGroup(g => g
                .Where(user["age"], Op.Gte, Param.Value(18))
                .And(user["age"], Op.Lte, Param.Value(65)))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\" FROM \"users\" AS \"u\" WHERE \"u\".\"active\" = @p0 AND (\"u\".\"age\" >= @p1 AND \"u\".\"age\" <= @p2)",
            result.Sql);
    }

    [Fact]
    public void OrGroup_NestedConditions_RendersParentheses()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["active"], Op.Eq, Param.Value(true))
            .OrGroup(g => g
                .Where(user["role"], Op.Eq, Param.Value("admin"))
                .And(user["verified"], Op.Eq, Param.Value(true)))
            .Build();

        Assert.Equal(
            "SELECT \"u\".\"id\" FROM \"users\" AS \"u\" WHERE \"u\".\"active\" = @p0 OR (\"u\".\"role\" = @p1 AND \"u\".\"verified\" = @p2)",
            result.Sql);
    }

    [Fact]
    public void MultiLevelNesting_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["active"], Op.Eq, Param.Value(true))
            .AndGroup(g => g
                .Where(user["role"], Op.Eq, Param.Value("admin"))
                .OrGroup(g2 => g2
                    .Where(user["age"], Op.Gte, Param.Value(21))
                    .And(user["verified"], Op.Eq, Param.Value(true))))
            .Build();

        Assert.Contains("AND (", result.Sql);
        Assert.Contains("OR (", result.Sql);
        Assert.Equal(4, result.Params.Length);
    }

    [Fact]
    public void AllWhereIfFalse_NoWhereClause()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .WhereIf(false, user["age"], Op.Gte, Param.Value(18))
            .AndIf(false, user["active"], Op.Eq, Param.Value(true))
            .Build();

        Assert.DoesNotContain("WHERE", result.Sql);
        Assert.Empty(result.Params);
    }

    [Fact]
    public void FieldVsFieldComparison_NoParameters()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");

        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Join(order, j => j.On(order["user_id"], Op.Eq, user["id"]))
            .Where(order["total"], Op.Gt, user["credit_limit"])
            .Build();

        Assert.Contains("\"o\".\"total\" > \"u\".\"credit_limit\"", result.Sql);
        Assert.Empty(result.Params);
    }
}
