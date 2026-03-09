using BenchmarkDotNet.Attributes;
using YTStdSqlBuilder;
using YTStdSqlBuilder.Expressions;

[MemoryDiagnoser]
[ShortRunJob]
public class QueryBenchmarks
{
    [Benchmark(Description = "Naive string concat - Simple")]
    public string NaiveStringConcat_Simple()
    {
        return "SELECT \"u\".\"id\" AS \"id\", \"u\".\"name\" FROM \"users\" AS \"u\" WHERE \"u\".\"age\" >= @p0";
    }

    [Benchmark(Description = "StringBuilder - Simple")]
    public string StringBuilder_Simple()
    {
        var sb = new System.Text.StringBuilder(128);
        sb.Append("SELECT \"u\".\"id\" AS \"id\", \"u\".\"name\"");
        sb.Append(" FROM \"users\" AS \"u\"");
        sb.Append(" WHERE \"u\".\"age\" >= @p0");
        return sb.ToString();
    }

    [Benchmark(Description = "Runtime Interpreter - Simple")]
    public PgSqlRenderResult RuntimeInterpreter_Simple()
    {
        var user = Table.Def("users").As("u");
        return PgSql
            .Select(user["id"].As<int>("id"), user["name"])
            .From(user)
            .Where(user["age"], Op.Gte, Param.Value(18))
            .Build();
    }

    [Benchmark(Description = "Runtime Interpreter - Complex")]
    public PgSqlRenderResult RuntimeInterpreter_Complex()
    {
        var user = Table.Def("users").As("u");
        var order = Table.Def("orders").As("o");
        return PgSql
            .Select(
                user["id"].As<int>("id"),
                user["name"],
                Func.Count(All.Value).As<int>("order_count"))
            .From(user)
            .LeftJoin(order, join => join
                .On(user["id"], Op.Eq, order["user_id"])
                .And(order["is_deleted"], Op.Eq, Literal.False))
            .Where(user["age"], Op.Gte, Param.Value(18))
            .GroupBy(user["id"], user["name"])
            .Having(Func.Count(All.Value), Op.Gt, Param.Value(0))
            .OrderByDesc(user["id"])
            .Limit(20)
            .Offset(0)
            .Build();
    }

    [Benchmark(Description = "Runtime Interpreter - Dynamic WhereIf")]
    public PgSqlRenderResult RuntimeInterpreter_Dynamic()
    {
        var user = Table.Def("users").As("u");
        string? name = "test";
        int? minAge = 18;
        int? maxAge = null;
        return PgSql
            .Select(user["id"].As<int>(), user["name"])
            .From(user)
            .WhereIf(!string.IsNullOrEmpty(name), user["name"], Op.ILike, Param.Value($"%{name}%"))
            .AndIf(minAge.HasValue, user["age"], Op.Gte, Param.Value(minAge))
            .AndIf(maxAge.HasValue, user["age"], Op.Lte, Param.Value(maxAge))
            .Build();
    }
}
