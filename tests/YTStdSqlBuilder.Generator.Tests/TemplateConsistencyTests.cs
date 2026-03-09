using Xunit;

namespace YTStdSqlBuilder.Generator.Tests;

public class TemplateConsistencyTests
{
    [Fact]
    public void RuntimeBuilder_ProducesConsistentSql()
    {
        // Verify that building the same query twice produces identical SQL
        var user = Table.Def("users").As("u");

        var result1 = PgSql
            .Select(user["id"].As<int>("id"), user["name"])
            .From(user)
            .Where(user["id"], Op.Eq, Param.Value(1))
            .Build();

        var user2 = Table.Def("users").As("u");
        var result2 = PgSql
            .Select(user2["id"].As<int>("id"), user2["name"])
            .From(user2)
            .Where(user2["id"], Op.Eq, Param.Value(1))
            .Build();

        Assert.Equal(result1.Sql, result2.Sql);
        Assert.Equal(result1.Params.Length, result2.Params.Length);
    }

    [Fact]
    public void ParameterNumbering_IsConsistent()
    {
        var user = Table.Def("users").As("u");

        var result = PgSql
            .Select(user["id"].As<int>(), user["name"])
            .From(user)
            .Where(user["name"], Op.Eq, Param.Value("test"))
            .And(user["age"], Op.Gte, Param.Value(18))
            .Build();

        Assert.Equal("@p0", result.Params[0].Name);
        Assert.Equal("@p1", result.Params[1].Name);
    }

    [Fact]
    public void IdentifierEscaping_IsConsistent()
    {
        var user = Table.Def("users").As("u");

        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Build();

        Assert.Contains("\"u\".\"id\"", result.Sql);
        Assert.Contains("\"users\" AS \"u\"", result.Sql);
    }
}
