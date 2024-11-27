// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

namespace MetaCraft.Core;

public static class DictionaryExtensions
{
    /// <summary>
    /// Returns the value associated with the specified key. If the specified key does not exist,
    /// inserts a new value provided by the specified provider function, and returns that new value.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the value of the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="valueProvider">The value provider function.</param>
    /// <returns>The value associated with the specified key.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> valueProvider)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var retVal))
        {
            return retVal;
        }

        retVal = valueProvider();
        dictionary.Add(key, retVal);
        return retVal;
    }
}
