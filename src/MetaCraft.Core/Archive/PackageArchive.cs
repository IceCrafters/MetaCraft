// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.IO.Compression;
using System.Text.Json;
using MetaCraft.Archive;

namespace MetaCraft.Core.Archive;

public class PackageArchive
{
    private const string ManifestFileName = "manifest.json";
    private readonly string _fullPath;

    public PackageArchive(PackageManifest manifest, string fullPath)
    {
        Manifest = manifest;
        _fullPath = fullPath;
    }
    
    public PackageManifest Manifest { get; }

    public void ExpandArchive(string destination, bool overwrite)
    {
        ZipFile.ExtractToDirectory(_fullPath, destination, overwrite);
    }
    
    public static PackageArchive Open(string packageFile)
    {
        using var fileStream = File.OpenRead(packageFile);
        using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
        
        return Open(zipArchive, Path.GetFullPath(packageFile));
    }

    private static PackageArchive Open(ZipArchive file, string fullPath)
    {
        var entry = file.GetEntry(ManifestFileName);
        if (entry == null)
        {
            throw InvalidPackageException.CreateNoManifest();
        }

        using var stream = entry.Open();
        var manifest = JsonSerializer.Deserialize(stream, CoreJsonContext.Default.PackageManifest);
        if (manifest == null)
        {
            throw InvalidPackageException.CreateInvalidManifest();
        }
            
        return new PackageArchive(manifest, fullPath);
    }

    /// <summary>
    /// Gets the manifest contained in the specified package file.
    /// </summary>
    /// <param name="packageFile">The package to inspect.</param>
    /// <returns>The package manifest inside the archive.</returns>
    /// <exception cref="InvalidPackageException">The manifest file does not exist in the package file.</exception>
    public static PackageManifest Inspect(string packageFile)
    {
        var archive = Open(packageFile);
        return archive.Manifest;
    }
}
