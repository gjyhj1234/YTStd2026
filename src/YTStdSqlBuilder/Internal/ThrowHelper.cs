using System.Diagnostics.CodeAnalysis;

namespace YTStdSqlBuilder.Internal;

internal static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowArgumentNull(string? paramName) =>
        throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    public static void ThrowArgumentEmpty(string? paramName) =>
        throw new ArgumentException("Value cannot be empty.", paramName);

    [DoesNotReturn]
    public static void ThrowInvalidOperation(string message) =>
        throw new InvalidOperationException(message);
}
