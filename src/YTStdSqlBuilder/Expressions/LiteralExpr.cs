namespace YTStdSqlBuilder.Expressions;

public sealed class LiteralExpr : SqlExpr
{
    public override SqlExprKind Kind => SqlExprKind.Literal;

    public string SqlText { get; }
    public object? Value { get; }

    private LiteralExpr(string sqlText, object? value = null)
    {
        SqlText = sqlText;
        Value = value;
    }

    public static readonly LiteralExpr True = new("TRUE", true);
    public static readonly LiteralExpr False = new("FALSE", false);
    public static readonly LiteralExpr Null = new("NULL");

    public static LiteralExpr Number(int value) => new(value.ToString(), value);
    public static LiteralExpr Number(long value) => new(value.ToString(), value);
    public static LiteralExpr Number(decimal value) => new(value.ToString(), value);
    public static LiteralExpr Number(double value) => new(value.ToString(), value);
    public static LiteralExpr String(string value) => new($"'{value.Replace("'", "''")}'", value);
    public static LiteralExpr Raw(string sqlText) => new(sqlText);
}
