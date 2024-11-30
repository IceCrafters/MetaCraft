// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using Semver;

namespace MetaCraft.Core.Archive;

public sealed class RangedPackageReference
{
    [SetsRequiredMembers]
    public RangedPackageReference(string id, SemVersionRange version)
    {
        Id = id;
        Version = version;
    }
    
    public required string Id { get; init; }
    public required SemVersionRange Version { get; init; }
}
