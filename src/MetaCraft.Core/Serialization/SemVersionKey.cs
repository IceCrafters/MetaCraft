// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using Semver;

namespace MetaCraft.Core.Serialization;

/// <summary>
/// Represents <see cref="SemVersion"/> as a key in a dictionary.
/// </summary>
[JsonConverter(typeof(SemVersionKeyConverter))]
public sealed class SemVersionKey : IEquatable<SemVersionKey>
{
    public SemVersionKey(SemVersion value)
    {
        Value = value;
    }

    public SemVersion Value { get; }

    public bool Equals(SemVersionKey? other)
    {
        return other != null && Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is SemVersionKey key && Equals(key);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(SemVersionKey? left, SemVersionKey? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SemVersionKey? left, SemVersionKey? right)
    {
        return !Equals(left, right);
    }
}
