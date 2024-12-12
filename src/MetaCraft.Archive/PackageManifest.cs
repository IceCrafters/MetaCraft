// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Archive.References;
using MetaCraft.Common.Json;
using MetaCraft.Common.Platform;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Manifest;
using Semver;

namespace MetaCraft.Archive;

public sealed class PackageManifest
{
    public required string Id { get; init; }
    
    [JsonConverter(typeof(SemVersionConverter))]
    public required SemVersion Version { get; init; }
    
    public required DateTime PackageTime { get; init; }
    
    public required PlatformIdentifier Platform { get; init; }
    
    public ManifestRuntimeInfo? Runtime { get; init; }
    
    public PackageLabel? Label { get; init; }
    
    public bool Unitary { get; init; }
    
    public RangedPackageReferenceDictionary? Dependencies { get; init; }
    
    public RangedPackageReferenceDictionary? ConflictsWith { get; init; }
    
    public PackageReferenceDictionary? Provides { get; init; }
}
