// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;
using MetaCraft.Archive.References;

namespace MetaCraft.Core.Scopes;

public static class PackageContainerExtensions
{
    /// <summary>
    /// Reads the manifest of a local package and returns the deserialized data of that manifest.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="packageReference">The package reference.</param>
    /// <returns>The package manifest if found; if not found, returns <see langword="null"/>.</returns>
    public static PackageManifest? InspectLocal(this IPackageContainer container,
        PackageReference packageReference)
    {
        return container.InspectLocal(packageReference.Name, packageReference.Version);
    }
}