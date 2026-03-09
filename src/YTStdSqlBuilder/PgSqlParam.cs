using NpgsqlTypes;

namespace YTStdSqlBuilder;

public readonly struct PgSqlParam
{
    public readonly string Name;
    public readonly NpgsqlDbType? DbType;
    public readonly object? Value;

    public PgSqlParam(string name, object? value, NpgsqlDbType? dbType = null)
    {
        Name = name;
        DbType = dbType;
        Value = value;
    }
}
