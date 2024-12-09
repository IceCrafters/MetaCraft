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
    
    /// <summary>
    /// Reads and returns the manifest associated with the package specified by its identifier and
    /// version.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is not recommended to use this method to check for package existence. Instead,
    /// <see cref="GetInstalledPackageLocationOrDefault(string, SemVersion)"/> or one of its other
    /// overloads should be used.
    /// </para>
    /// </remarks>
    /// <param name="packageId">The ID of the package to read manifest from.</param>
    /// <param name="version">The version of the package to read manifest from.</param>
    /// <returns>The package manifest if found; otherwise, <see langword="null"/>.</returns>
    PackageManifest? InspectLocal(string packageId, SemVersion version);

    void DeletePackage(PackageManifest manifest);
}
