using Xunit;
using YTStdSqlBuilder;
using YTStdSqlBuilder.Expressions;

namespace YTStdSqlBuilder.Tests;

public class DmlTests
{
    [Fact]
    public void Insert_WithColumnsAndValues_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.InsertInto(user)
            .Columns(user["name"], user["email"])
            .Values(Param.Value("John"), Param.Value("john@example.com"))
            .Build();

        Assert.Equal(
            "INSERT INTO \"users\" (\"name\", \"email\") VALUES (@p0, @p1)",
            result.Sql);
        Assert.Equal("John", result.Params[0].Value);
        Assert.Equal("john@example.com", result.Params[1].Value);
    }

    [Fact]
    public void Insert_WithSetPattern_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.InsertInto(user)
            .Set(user["name"], Param.Value("Jane"))
            .Set(user["email"], Param.Value("jane@example.com"))
            .Build();

        Assert.Equal(
            "INSERT INTO \"users\" (\"name\", \"email\") VALUES (@p0, @p1)",
            result.Sql);
        Assert.Equal("Jane", result.Params[0].Value);
        Assert.Equal("jane@example.com", result.Params[1].Value);
    }

    [Fact]
    public void Insert_WithReturning_RendersReturningClause()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.InsertInto(user)
            .Set(user["name"], Param.Value("Test"))
            .Returning(user["id"])
            .Build();

        Assert.Contains("RETURNING", result.Sql);
        Assert.Contains("\"u\".\"id\"", result.Sql);
    }

    [Fact]
    public void Update_WithSet_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.Update(user)
            .Set(user["name"], Param.Value("Updated"))
            .Where(user["id"], Op.Eq, Param.Value(1))
            .Build();

        Assert.Equal(
            "UPDATE \"users\" AS \"u\" SET \"name\" = @p0 WHERE \"u\".\"id\" = @p1",
            result.Sql);
        Assert.Equal("Updated", result.Params[0].Value);
        Assert.Equal(1, result.Params[1].Value);
    }

    [Fact]
    public void Update_SetIfTrue_IncludesAssignment()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.Update(user)
            .Set(user["name"], Param.Value("Base"))
            .SetIf(true, user["email"], Param.Value("new@example.com"))
            .Where(user["id"], Op.Eq, Param.Value(1))
            .Build();

        Assert.Contains("\"name\" = @p0", result.Sql);
        Assert.Contains("\"email\" = @p1", result.Sql);
        Assert.Equal(3, result.Params.Length);
    }

    [Fact]
    public void Update_SetIfFalse_SkipsAssignment()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.Update(user)
            .Set(user["name"], Param.Value("Base"))
            .SetIf(false, user["email"], Param.Value("new@example.com"))
            .Where(user["id"], Op.Eq, Param.Value(1))
            .Build();

        Assert.DoesNotContain("\"email\"", result.Sql);
        Assert.Equal(2, result.Params.Length);
    }

    [Fact]
    public void Update_WithoutWhere_Throws()
    {
        var user = Table.Def("users").As("u");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            PgSql.Update(user)
                .Set(user["name"], Param.Value("Test"))
                .Build());

        Assert.Contains("unsafe", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Update_AllowUnsafeUpdate_AllowsNoWhere()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.Update(user)
            .Set(user["active"], Param.Value(false))
            .AllowUnsafeUpdate()
            .Build();

        Assert.DoesNotContain("WHERE", result.Sql);
        Assert.Contains("SET \"active\" = @p0", result.Sql);
    }

    [Fact]
    public void Delete_WithWhere_RendersCorrectly()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.DeleteFrom(user)
            .Where(user["id"], Op.Eq, Param.Value(42))
            .Build();

        Assert.Equal(
            "DELETE FROM \"users\" AS \"u\" WHERE \"u\".\"id\" = @p0",
            result.Sql);
        Assert.Equal(42, result.Params[0].Value);
    }

    [Fact]
    public void Delete_WithoutWhere_Throws()
    {
        var user = Table.Def("users").As("u");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            PgSql.DeleteFrom(user).Build());

        Assert.Contains("unsafe", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Delete_AllowUnsafeDelete_AllowsNoWhere()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql.DeleteFrom(user)
            .AllowUnsafeDelete()
            .Build();

        Assert.Equal("DELETE FROM \"users\" AS \"u\"", result.Sql);
    }

    [Fact]
    public void Update_AllSetIfFalse_EmptySetThrows()
    {
        var user = Table.Def("users").As("u");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            PgSql.Update(user)
                .SetIf(false, user["name"], Param.Value("Test"))
                .SetIf(false, user["email"], Param.Value("test@example.com"))
                .Where(user["id"], Op.Eq, Param.Value(1))
                .Build());

        Assert.Contains("assignment", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
