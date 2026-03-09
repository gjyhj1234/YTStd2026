namespace YTStdSqlBuilder.Conditions;

public sealed class SqlConditionGroup
{
    public List<SqlConditionNode> Nodes { get; }
    public SqlLogicalOperator DefaultOperator { get; }

    public SqlConditionGroup(SqlLogicalOperator defaultOperator = SqlLogicalOperator.And)
    {
        Nodes = new List<SqlConditionNode>();
        DefaultOperator = defaultOperator;
    }

    public SqlConditionGroup(List<SqlConditionNode> nodes, SqlLogicalOperator defaultOperator = SqlLogicalOperator.And)
    {
        Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
        DefaultOperator = defaultOperator;
    }

    public bool IsEmpty => Nodes.Count == 0;
}
