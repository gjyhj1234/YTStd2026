using YTStdSqlBuilder.Internal;

namespace YTStdSqlBuilder;

public sealed class QueryResultShape
{
    public string QueryName { get; }
    public List<QueryResultColumn> Columns { get; }

    public QueryResultShape(string queryName)
    {
        QueryName = Guard.NotNullOrEmpty(queryName);
        Columns = new List<QueryResultColumn>();
    }

    public QueryResultShape(string queryName, List<QueryResultColumn> columns)
    {
        QueryName = Guard.NotNullOrEmpty(queryName);
        Columns = Guard.NotNull(columns);
    }
}
