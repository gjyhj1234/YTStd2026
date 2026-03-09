using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace YTStdSqlBuilder.Internal;

internal static class Guard
{
    public static T NotNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : class
    {
        if (value is null)
            ThrowHelper.ThrowArgumentNull(paramName);
        return value;
    }

    public static string NotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNull(paramName);
            else
                ThrowHelper.ThrowArgumentEmpty(paramName);
        }
        return value;
    }
}
