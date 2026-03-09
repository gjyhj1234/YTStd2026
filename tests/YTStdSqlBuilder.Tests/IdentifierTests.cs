using Xunit;
using YTStdSqlBuilder;
using YTStdSqlBuilder.Expressions;

namespace YTStdSqlBuilder.Tests;

public class IdentifierTests
{
    [Fact]
    public void SimpleTableName_IsDoubleQuoted()
    {
        Assert.Equal("\"users\"", SqlIdentifier.Escape("users"));
    }

    [Fact]
    public void SchemaQualifiedTableName_BothPartsQuoted()
    {
        Assert.Equal("\"public\".\"users\"", SqlIdentifier.EscapeQualified("public", "users"));
    }

    [Fact]
    public void SchemaQualifiedTableName_NullSchema_OnlyNameQuoted()
    {
        Assert.Equal("\"users\"", SqlIdentifier.EscapeQualified(null, "users"));
    }

    [Fact]
    public void ColumnWithTableAlias_RendersAsDotNotation()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["name"])
            .From(user)
            .Build();

        Assert.Contains("\"u\".\"name\"", result.Sql);
    }

    [Fact]
    public void TableWithAlias_RendersWithAs()
    {
        var user = Table.Def("users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Build();

        Assert.Contains("\"users\" AS \"u\"", result.Sql);
    }

    [Fact]
    public void ReservedKeyword_AsIdentifier_IsEscaped()
    {
        var selectTable = Table.Def("select").As("s");
        var result = PgSql
            .Select(selectTable["order"])
            .From(selectTable)
            .Build();

        Assert.Contains("\"select\"", result.Sql);
        Assert.Contains("\"order\"", result.Sql);
    }

    [Fact]
    public void SchemaQualifiedTable_InQuery_RendersWithSchema()
    {
        var user = Table.Def("public", "users").As("u");
        var result = PgSql
            .Select(user["id"])
            .From(user)
            .Build();

        Assert.Contains("\"public\".\"users\" AS \"u\"", result.Sql);
    }
}
