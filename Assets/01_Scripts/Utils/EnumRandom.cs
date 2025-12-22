using System;
using System.Collections.Generic;

public static class EnumRandom<T> where T : struct, Enum
{
    private static readonly T[] _values = (T[])Enum.GetValues(typeof(T));

    public static T Pick()
    {
        if (_values.Length == 0)
            throw new InvalidOperationException($"{typeof(T)} has no values.");

        return _values[UnityEngine.Random.Range(0, _values.Length)];
    }

    public static T PickExcept(T except)
    {
        if (_values.Length <= 1)
            throw new InvalidOperationException($"{typeof(T)} has no alternative values.");

        T value;
        do
        {
            value = Pick();
        }
        while (EqualityComparer<T>.Default.Equals(value, except));

        return value;
    }

    public static ReadOnlySpan<T> Values => _values;
    public static int Count => _values.Length;
}
