// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using Semver;

namespace MetaCraft.Core.Serialization;

/// <summary>
/// Represents <see cref="SemVersion"/> as a key in a dictionary.
/// </summary>
[JsonConverter(typeof(SemVersionKeyConverter))]
public sealed class SemVersionKey
{
    public SemVersionKey(SemVersion value)
    {
        Value = value;
    }

    public SemVersion Value { get; }
}
