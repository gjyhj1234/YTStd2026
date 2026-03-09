using Xunit;

namespace YTStdSqlBuilder.Generator.Tests;

public class ResultShapeGenerationTests
{
    [Fact]
    public void QueryResultColumn_PascalCaseConversion()
    {
        var col = new QueryResultColumn("user_name", typeof(string), 0);
        Assert.Equal("UserName", col.PropertyName);
    }

    [Fact]
    public void QueryResultColumn_SimpleName()
    {
        var col = new QueryResultColumn("id", typeof(int), 0);
        Assert.Equal("Id", col.PropertyName);
    }

    [Fact]
    public void QueryResultColumn_AlreadyPascalCase()
    {
        var col = new QueryResultColumn("UserName", typeof(string), 0);
        Assert.Equal("UserName", col.PropertyName);
    }

    [Fact]
    public void QueryResultShape_HasCorrectColumns()
    {
        var shape = new QueryResultShape("GetUser");
        shape.Columns.Add(new QueryResultColumn("id", typeof(int), 0));
        shape.Columns.Add(new QueryResultColumn("name", typeof(string), 1));

        Assert.Equal("GetUser", shape.QueryName);
        Assert.Equal(2, shape.Columns.Count);
    }

    [Fact]
    public void QueryResultColumn_NullableType()
    {
        var col = new QueryResultColumn("age", typeof(int?), 0, isNullable: true);
        Assert.True(col.IsNullable);
        Assert.Equal("Age", col.PropertyName);
    }

    [Fact]
    public void QueryResultColumn_OrdinalPreserved()
    {
        var col0 = new QueryResultColumn("id", typeof(int), 0);
        var col1 = new QueryResultColumn("name", typeof(string), 1);
        var col2 = new QueryResultColumn("age", typeof(int), 2);

        Assert.Equal(0, col0.Ordinal);
        Assert.Equal(1, col1.Ordinal);
        Assert.Equal(2, col2.Ordinal);
    }
}
