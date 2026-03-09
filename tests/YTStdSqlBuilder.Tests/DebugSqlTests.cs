using Xunit;
using YTStdSqlBuilder;
using YTStdSqlBuilder.Expressions;

namespace YTStdSqlBuilder.Tests;

public class DebugSqlTests
{
    [Fact]
    public void DebugSql_IntegerValue_RendersLiteral()
    {
        var user = Table.Def("users").As("u");
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["age"], Op.Gte, Param.Value(18))
            .BuildDebugSql();

        Assert.Contains(">= 18", sql);
        Assert.DoesNotContain("@p", sql);
    }

    [Fact]
    public void DebugSql_StringValue_RendersQuoted()
    {
        var user = Table.Def("users").As("u");
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["name"], Op.Eq, Param.Value("Alice"))
            .BuildDebugSql();

        Assert.Contains("= 'Alice'", sql);
    }

    [Fact]
    public void DebugSql_StringWithSingleQuote_EscapesQuote()
    {
        var user = Table.Def("users").As("u");
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["name"], Op.Eq, Param.Value("O'Brien"))
            .BuildDebugSql();

        Assert.Contains("= 'O''Brien'", sql);
    }

    [Fact]
    public void DebugSql_BooleanTrue_RendersTRUE()
    {
        var user = Table.Def("users").As("u");
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["active"], Op.Eq, Param.Value(true))
            .BuildDebugSql();

        Assert.Contains("= TRUE", sql);
    }

    [Fact]
    public void DebugSql_BooleanFalse_RendersFALSE()
    {
        var user = Table.Def("users").As("u");
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["active"], Op.Eq, Param.Value(false))
            .BuildDebugSql();

        Assert.Contains("= FALSE", sql);
    }

    [Fact]
    public void DebugSql_NullValue_RendersIsNull()
    {
        var user = Table.Def("users").As("u");
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["deleted_at"], Op.Eq, Param.Value(null))
            .BuildDebugSql();

        Assert.Contains("IS NULL", sql);
    }

    [Fact]
    public void DebugSql_DateTime_RendersFormatted()
    {
        var user = Table.Def("users").As("u");
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["created_at"], Op.Gte, Param.Value(dt))
            .BuildDebugSql();

        Assert.Contains("'2024-01-15T10:30:00.0000000Z'", sql);
    }

    [Fact]
    public void DebugSql_MultipleParams_AllInlined()
    {
        var user = Table.Def("users").As("u");
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["age"], Op.Gte, Param.Value(18))
            .And(user["name"], Op.Like, Param.Value("%test%"))
            .BuildDebugSql();

        Assert.Contains(">= 18", sql);
        Assert.Contains("LIKE '%test%'", sql);
        Assert.DoesNotContain("@p", sql);
    }

    [Fact]
    public void DebugSql_Guid_RendersWithUuidCast()
    {
        var user = Table.Def("users").As("u");
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var sql = PgSql
            .Select(user["id"])
            .From(user)
            .Where(user["external_id"], Op.Eq, Param.Value(guid))
            .BuildDebugSql();

        Assert.Contains("'12345678-1234-1234-1234-123456789012'::uuid", sql);
    }
}
