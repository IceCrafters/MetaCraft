// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using Semver;

namespace MetaCraft.Testing;

/// <summary>
/// Provides utilities to create instances of <see cref="PackageManifest"/> for use with mocking
/// tests.
/// </summary>
public static class ManifestHelper
{
    private static PackageReferenceDictionary? CreateDictionaryOf(PackageReference[]? references)
    {
        if (references == null)
        {
            return null;
        }
        
        var result = new PackageReferenceDictionary(references.Length);
        foreach (var reference in references)
        {
            result.Add(reference.Name, reference.Version);
        }

        return result;
    }
    
    private static RangedPackageReferenceDictionary? CreateDictionaryOf(RangedPackageReference[]? references)
    {
        if (references == null)
        {
            return null;
        }
        
        var result = new RangedPackageReferenceDictionary(references.Length);
        foreach (var reference in references)
        {
            result.Add(reference.Id, reference.Version);
        }

        return result;
    }
    
    /// <summary>
    /// Creates an instance of <see cref="PackageManifest"/> for mocking purposes.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="version">The version number.</param>
    /// <param name="packageTime">
    /// The time of packaging. If the <see cref="PackageManifest.PackageTime"/> property does not
    /// matter, use a value like <see cref="DateTime.MinValue"/>.
    /// </param>
    /// <param name="platform">
    /// The platform. If <see langword="null"/>, uses <see cref="PlatformIdentifier.Current"/>.
    /// </param>
    /// <param name="unitary">If <see langword="true"/>, specifies the package as unitary.</param>
    /// <param name="provides">The referral clauses provided by the package.</param>
    /// <param name="dependencies">The packages that is required.</param>
    /// <param name="conflictsWith">The packages that conflicts with the package.</param>
    /// <returns>An instance of <see cref="PackageManifest"/>.</returns>
    public static PackageManifest CreateManifest(string id,
        SemVersion version,
        DateTime packageTime,
        PlatformIdentifier? platform = null,
        bool unitary = false,
        PackageReference[]? provides = null,
        RangedPackageReference[]? dependencies = null,
        RangedPackageReference[]? conflictsWith = null)
    {
        var manifest = new PackageManifest
        {
            Id = id,
            Version = version,
            Unitary = unitary,
            PackageTime = packageTime,
            Platform = platform ?? PlatformIdentifier.Current,
            Provides = CreateDictionaryOf(provides),
            Dependencies = CreateDictionaryOf(dependencies),
            ConflictsWith = CreateDictionaryOf(conflictsWith)
        };

        return manifest;
    }
}
