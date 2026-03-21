using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace YTStdTenantPlatform.Infrastructure.Serialization;

/// <summary>基于 Utf8JsonWriter 的 JSON 构造辅助方法</summary>
internal static class Utf8JsonWriterHelper
{
    /// <summary>构造 JSON 字符串</summary>
    public static string BuildString<TState>(TState state, Action<Utf8JsonWriter, TState> writeAction)
    {
        ArgumentNullException.ThrowIfNull(writeAction);

        var buffer = new ArrayBufferWriter<byte>(256);
        using var writer = new Utf8JsonWriter(buffer);
        writeAction(writer, state);
        writer.Flush();
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    /// <summary>直接写入 HTTP 响应</summary>
    public static async Task WriteResponseAsync<TState>(
        HttpResponse response,
        TState state,
        Action<Utf8JsonWriter, TState> writeAction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(writeAction);

        var buffer = new ArrayBufferWriter<byte>(256);
        using var writer = new Utf8JsonWriter(buffer);
        writeAction(writer, state);
        writer.Flush();

        response.ContentLength = buffer.WrittenCount;
        await response.Body.WriteAsync(buffer.WrittenMemory, cancellationToken);
    }
}
