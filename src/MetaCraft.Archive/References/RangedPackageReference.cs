// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using MetaCraft.Core;
using MetaCraft.Core.Archive;
using Semver;

namespace MetaCraft.Archive.References;

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

    public bool Contains(string id, SemVersion version)
    {
        return id == Id && Version.Contains(version);
    }
    
    public bool Contains(KeyValuePair<string, SemVersion> reference)
    {
        return Contains(reference.Key, reference.Value);
    }

    public bool Contains(PackageReference reference)
    {
        return reference.Name == this.Id && Version.Contains(reference.Version);
    }

    public bool Contains(PackageManifest manifest)
    {
        return manifest.Id == this.Id && Version.Contains(manifest.Version);
    }
}
