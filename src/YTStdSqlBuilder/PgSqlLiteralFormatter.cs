using System.Globalization;

namespace YTStdSqlBuilder;

internal static class PgSqlLiteralFormatter
{
    public static string Format(object? value)
    {
        switch (value)
        {
            case null:
                return "NULL";
            case bool b:
                return b ? "TRUE" : "FALSE";
            case string s:
            {
                var vsb = new ValueStringBuilder(stackalloc char[64]);
                vsb.Append('\'');
                vsb.Append(s.Replace("'", "''"));
                vsb.Append('\'');
                return vsb.ToString();
            }
            case char c:
            {
                var vsb = new ValueStringBuilder(stackalloc char[8]);
                vsb.Append('\'');
                vsb.Append(c);
                vsb.Append('\'');
                return vsb.ToString();
            }
            case int i:
                return i.ToString(CultureInfo.InvariantCulture);
            case long l:
                return l.ToString(CultureInfo.InvariantCulture);
            case decimal d:
                return d.ToString(CultureInfo.InvariantCulture);
            case double d:
                return d.ToString(CultureInfo.InvariantCulture);
            case float f:
                return f.ToString(CultureInfo.InvariantCulture);
            case short s:
                return s.ToString(CultureInfo.InvariantCulture);
            case byte b:
                return b.ToString(CultureInfo.InvariantCulture);
            case DateTime dt:
            {
                var vsb = new ValueStringBuilder(stackalloc char[64]);
                vsb.Append('\'');
                vsb.Append(dt.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture));
                vsb.Append('\'');
                return vsb.ToString();
            }
            case DateTimeOffset dto:
            {
                var vsb = new ValueStringBuilder(stackalloc char[64]);
                vsb.Append('\'');
                vsb.Append(dto.ToString("o", CultureInfo.InvariantCulture));
                vsb.Append('\'');
                return vsb.ToString();
            }
            case Guid g:
            {
                var vsb = new ValueStringBuilder(stackalloc char[48]);
                vsb.Append('\'');
                vsb.Append(g.ToString());
                vsb.Append("'::uuid");
                return vsb.ToString();
            }
            case Enum e:
                return Convert.ToInt64(e, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            default:
            {
                var vsb = new ValueStringBuilder(stackalloc char[64]);
                vsb.Append('\'');
                vsb.Append(value.ToString());
                vsb.Append('\'');
                return vsb.ToString();
            }
        }
    }

    public static IEnumerable<string> FormatEnumerable(System.Collections.IEnumerable values)
    {
        foreach (var item in values)
            yield return Format(item);
    }
}
