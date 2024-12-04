// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using Semver;

namespace MetaCraft.Core.Scopes;

public interface IPackageContainer : ISerialed
{   
    IPackageScope Parent { get; }

    public IEnumerable<string> EnumeratePackages();
    IEnumerable<SemVersion> EnumerateVersions(string packageId);
    SemVersion? GetLatestVersion(string packageId);
    string? GetPlatformFile(PackageManifest manifest, string subPath);

    string? GetInstalledPackageLocationOrDefault(PackageReference reference);
    string? GetInstalledPackageLocationOrDefault(PackageManifest manifest);
    string? GetInstalledPackageLocationOrDefault(string packageId, SemVersion version);

    string InsertPackage(PackageManifest manifest, bool overwrite = false, bool doNotCreateManifest = false);
    PackageManifest? InspectLocal(string packageId, SemVersion version);

    void DeletePackage(PackageManifest manifest);
}
