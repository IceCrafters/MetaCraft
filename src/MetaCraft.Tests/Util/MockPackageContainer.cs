// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using Semver;

namespace MetaCraft.Tests.Util;

public class MockPackageContainer : IPackageContainer
{

    private readonly Dictionary<string, Dictionary<SemVersion, PackageManifest>> _data = [];

    public MockPackageContainer(IPackageScope parent, long serial)
    {
        Parent = parent;
        Serial = serial;
    }

    public IPackageScope Parent { get; }

    public long Serial { get; private set; }

    #region Interface Implementation

    private bool DoesPackageExist(string id, SemVersion version)
    {
        return _data.TryGetValue(id, out var versions) && versions.ContainsKey(version);
    }

    public bool CompareSerialWith(SerialFile serial)
    {
        return Serial == serial.ReadSerial();
    }

    public void CopySerial(ISerialed from)
    {
        Serial = from.GetSerial();
    }

    public void DeletePackage(PackageManifest manifest)
    {
        if (!_data.TryGetValue(manifest.Id, out var entry))
        {
            return;
        }

        entry.Remove(manifest.Version);
    }

    public IEnumerable<string> EnumeratePackages()
    {
        return _data.Keys;
    }

    public IEnumerable<SemVersion> EnumerateVersions(string packageId)
    {
        if (!_data.TryGetValue(packageId, out var entry))
        {
            return [];
        }

        return entry.Keys;
    }

    public string? GetInstalledPackageLocationOrDefault(PackageReference reference)
    {
        return DoesPackageExist(reference.Name, reference.Version)
            ? "exists, but location does not matter"
            : null;
    }

    public string? GetInstalledPackageLocationOrDefault(PackageManifest manifest)
    {
       return DoesPackageExist(manifest.Id, manifest.Version)
            ? "exists, but location does not matter"
            : null;
    }

    public string? GetInstalledPackageLocationOrDefault(string packageId, SemVersion version)
    {
        return DoesPackageExist(packageId, version)
            ? "exists, but location does not matter"
            : null;
    }

    public SemVersion? GetLatestVersion(string packageId)
    {
        if (!_data.TryGetValue(packageId, out var entry))
        {
            return null;
        }

        return entry.Keys.Order(SemVersion.SortOrderComparer)
            .FirstOrDefault();
    }

    public string? GetPlatformFile(PackageManifest manifest, string subPath)
    {
        return DoesPackageExist(manifest.Id, manifest.Version) && manifest.Unitary
            ? "does not matter"
            : null;
    }

    public long GetSerial()
    {
        return Serial;
    }

    public string InsertPackage(PackageManifest manifest, bool overwrite = false, bool doNotCreateManifest = false)
    {
        var entry = _data.GetOrAdd(manifest.Id, () => []);
        if (overwrite)
        {
            entry[manifest.Version] = manifest;
        }
        else
        {
            entry.Add(manifest.Version, manifest);
        }

        return "does not matter";
    }

    public PackageManifest? InspectLocal(string packageId, SemVersion version)
    {
        return _data.GetValueOrDefault(packageId)?.GetValueOrDefault(version);
    }

    #endregion

    #region Helper Methods

    public MockPackageContainer WithPackage(string id, 
        SemVersion version,
        DateTime packageTime,
        PlatformIdentifier? platform = null,
        bool unitary = false,
        PackageReference[]? provides = null)
    {
        var manifest = new PackageManifest()
        {
            Id = id,
            Version = version,
            Unitary = unitary,
            PackageTime = packageTime,
            Platform = platform ?? PlatformIdentifier.Current,
            Provides = provides != null ? new PackageReferenceDictionary(provides.Length) : null
        };

        if (provides != null)
        {
            manifest.Provides!.EnsureCapacity(provides.Length);
            manifest.Provides!.AddRange(provides);
        }

        InsertPackage(manifest);
        return this;
    }

    #endregion

}
