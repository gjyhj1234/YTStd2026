namespace YTStdSqlBuilder.Conditions;

public sealed class SqlConditionNode
{
    public SqlConditionNodeKind NodeKind { get; }
    public SqlCondition? Condition { get; }
    public List<SqlConditionNode>? Children { get; }

    private SqlConditionNode(SqlConditionNodeKind nodeKind, SqlCondition? condition, List<SqlConditionNode>? children)
    {
        NodeKind = nodeKind;
        Condition = condition;
        Children = children;
    }

    public static SqlConditionNode Simple(SqlCondition condition) =>
        new(SqlConditionNodeKind.SimpleCondition, condition ?? throw new ArgumentNullException(nameof(condition)), null);

    public static SqlConditionNode AndGroup(List<SqlConditionNode> children) =>
        new(SqlConditionNodeKind.AndGroup, null, children ?? throw new ArgumentNullException(nameof(children)));

    public static SqlConditionNode OrGroup(List<SqlConditionNode> children) =>
        new(SqlConditionNodeKind.OrGroup, null, children ?? throw new ArgumentNullException(nameof(children)));
}
