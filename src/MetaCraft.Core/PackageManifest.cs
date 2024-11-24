using System.Text.Json.Serialization;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Manifest;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Serialization;
using Semver;

namespace MetaCraft.Core;

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
    
    public ProvisionReferenceDictionary? Dependencies { get; init; }
    
    public ProvisionReferenceDictionary? ConflictsWith { get; init; }
    
    public PackageReferenceDictionary? Provides { get; init; }
}
