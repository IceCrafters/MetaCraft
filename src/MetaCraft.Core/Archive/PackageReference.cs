// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MetaCraft.Core.Serialization;
using Semver;

namespace MetaCraft.Core.Archive;

public readonly record struct PackageReference
{
    [SetsRequiredMembers]
    public PackageReference(string name, SemVersion version)
    {
        Name = name;
        Version = version;
    }
    
    public required string Name { get; init; }
    
    [JsonConverter(typeof(SemVersionConverter))]
    public required SemVersion Version { get; init; }
}
