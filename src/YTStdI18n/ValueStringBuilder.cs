using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace YTStdI18n;

/// <summary>
/// 高性能零分配字符串构建器（ref struct + ArrayPool + stackalloc）。
/// 替代 StringBuilder / 字符串拼接，避免中间字符串分配和 GC 压力。
/// </summary>
[InterpolatedStringHandler]
internal ref struct ValueStringBuilder
{
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStringBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars = _arrayToReturnToPool;
        _pos = 0;
    }

    public int Length => _pos;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        int pos = _pos;
        Span<char> chars = _chars;
        if ((uint)pos < (uint)chars.Length)
        {
            chars[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? s)
    {
        if (s is null) return;
        Append(s.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(ReadOnlySpan<char> value)
    {
        int pos = _pos;
        if (pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }
        value.CopyTo(_chars.Slice(_pos));
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(int value)
    {
        if (value.TryFormat(_chars.Slice(_pos), out int written))
        {
            _pos += written;
        }
        else
        {
            Grow(16);
            value.TryFormat(_chars.Slice(_pos), out written);
            _pos += written;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(long value)
    {
        if (value.TryFormat(_chars.Slice(_pos), out int written))
        {
            _pos += written;
        }
        else
        {
            Grow(24);
            value.TryFormat(_chars.Slice(_pos), out written);
            _pos += written;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    private void Grow(int additionalCapacityBeyondPos)
    {
        int newCapacity = (int)Math.Max(
            (uint)(_pos + additionalCapacityBeyondPos),
            Math.Min((uint)_chars.Length * 2, 0x3FFFFFDF));

        if (newCapacity < _pos + additionalCapacityBeyondPos)
            newCapacity = _pos + additionalCapacityBeyondPos;

        char[] poolArray = ArrayPool<char>.Shared.Rent(newCapacity);
        _chars.Slice(0, _pos).CopyTo(poolArray);

        char[]? toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public override string ToString()
    {
        string s = _chars.Slice(0, _pos).ToString();
        Dispose();
        return s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        char[]? toReturn = _arrayToReturnToPool;
        this = default;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}
