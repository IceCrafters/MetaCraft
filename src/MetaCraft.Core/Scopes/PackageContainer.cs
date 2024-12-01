// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Buffers.Binary;
using System.Text.Json;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Platform;
using Semver;

namespace MetaCraft.Core.Scopes;

public class PackageContainer : IPackageContainer
{
    private const string VersionFileName = "VERSION";
    private readonly string _root;
    private readonly SerialFile _serialFile;

    internal PackageContainer(string root, PackageScope parent)
    {
        _root = root;
        Directory.CreateDirectory(_root);
        Parent = parent;

        _serialFile = new SerialFile(Path.Combine(_root, "serial"));
    }

    public PackageScope Parent { get; }

    public IEnumerable<string> EnumeratePackages()
    {
        foreach (var dir in Directory.EnumerateDirectories(_root))
        {
            yield return dir;
        }
    }

    public IEnumerable<SemVersion> EnumerateVersions(string packageId)
    {
        var packageDir = Path.Combine(_root, packageId);
        if (!Directory.Exists(packageDir))
        {
            // No versions
            return [];
        }
        
        var unitaryDir = Path.Combine(packageDir, "u");

        // If not unitary
        if (!Directory.Exists(unitaryDir))
            return Directory.EnumerateDirectories(packageDir)
                .Select(x => SemVersion.Parse(Path.GetFileName(x), SemVersionStyles.Strict));

        // If ever unitary
        // Version file is required for install and uninstall purposes.
        var versionFile = Path.Combine(unitaryDir, VersionFileName);
        if (!File.Exists(versionFile))
        {
            throw new FileNotFoundException(Strings.ContainerUnitaryNoVersion);
        }

        return [SemVersion.Parse(File.ReadAllText(versionFile))];
    }

    public SemVersion? GetLatestVersion(string packageId)
    {
        return EnumerateVersions(packageId)
            .OrderDescending(SemVersion.SortOrderComparer)
            .FirstOrDefault();
    }

    public string? GetPlatformFile(PackageManifest manifest, string subPath)
    {
        Console.WriteLine(subPath);
        var location = GetPackageStoreLocation(manifest);
        if (!Directory.Exists(location))
        {
            return null;
        }

        var exactPlatformFile = Path.Combine(location, subPath, PlatformIdentifier.Current.ToString());
        var anyArchitectureFile = Path.Combine(location, subPath, new PlatformIdentifier(PlatformIdentifier.Current.System,
            PlatformArchitecture.Any).ToString());

        if (File.Exists(exactPlatformFile))
        {
            return exactPlatformFile;
        }

        if (File.Exists(anyArchitectureFile))
        {
            return anyArchitectureFile;
        }
        
        return null;
    }

    /// <summary>
    /// Returns the storage location of the specified package if it exists in the current
    /// container.
    /// </summary>
    /// <param name="reference">The reference to acquire package.</param>
    /// <returns>The storage location if package exists; otherwise, <see langword="null"/>.</returns>
    public string? GetInstalledPackageLocationOrDefault(PackageReference reference)
    {
        return GetInstalledPackageLocationOrDefault(reference.Name, reference.Version);
    }

    /// <summary>
    /// Returns the storage location of the specified package if it exists in the current
    /// container.
    /// </summary>
    /// <param name="manifest">The manifest of the package.</param>
    /// <returns>The storage location if package exists; otherwise, <see langword="null"/>.</returns>
    public string? GetInstalledPackageLocationOrDefault(PackageManifest manifest)
    {
        var location = GetPackageStoreLocation(manifest);
        return !Directory.Exists(location) ? null : location;
    }
    
    /// <summary>
    /// Returns the storage location of the specified package if it exists in the current
    /// container.
    /// </summary>
    /// <param name="packageId">The package identifier. Case sensitive.</param>
    /// <param name="version">The package version.</param>
    /// <returns>The storage location if package exists; otherwise, <see langword="null"/>.</returns>
    public string? GetInstalledPackageLocationOrDefault(string packageId, SemVersion version)
    {
        var unitaryDir = Path.Combine(_root, packageId, "u");
        if (Directory.Exists(unitaryDir))
        {
            var realVersion = GetLatestVersion(packageId);
            return realVersion != version ? null : unitaryDir;
        }

        var location = Path.Combine(_root, packageId, version.ToString());
        return !Directory.Exists(location) ? null : location;
    }

    private string GetPackageStoreLocation(PackageManifest manifest)
    {
        var versionDir = manifest.Unitary
            ? "u"
            : manifest.Version.ToString();

        return Path.Combine(_root, manifest.Id, versionDir);
    }

    public string InsertPackage(PackageManifest manifest, bool overwrite = false, bool doNotCreateManifest = false)
    {
        var location = GetPackageStoreLocation(manifest);
        string? towedPackage = null;

        if (Directory.Exists(location))
        {
            if (!overwrite)
            {
                throw new InvalidOperationException(string.Format(Strings.ContainerInsertAlreadyExists,
                    manifest.Unitary ? Strings.PackageUnitary : manifest.Version.ToString()));
            }

            towedPackage = TowPackage(manifest);
        }

        try
        {
            Directory.CreateDirectory(location);

            // Create structure
            Directory.CreateDirectory(Path.Combine(location, "contents"));

            // Place a manifest file
            if (!doNotCreateManifest)
            { _serialFile.Refresh();  
                using var stream = File.Create(Path.Combine(location, "manifest.json"));
                JsonSerializer.Serialize(stream, manifest, CoreJsonContext.Default.PackageManifest);
            }

            // Place version number file in case of need
            File.WriteAllText(Path.Combine(location, VersionFileName),
                manifest.Version.ToString());
        }
        catch
        {
            if (towedPackage != null)
            {
                Directory.Delete(location, true);
                Directory.Move(towedPackage, location);
            }

            throw;
        }

        // Remove the temporarily towed package away.
        try
        {
            if (towedPackage != null)
            {
                Directory.Delete(towedPackage, true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(Strings.ContainerTowDeleteFailed, @"MetaCraft.Core");
            Console.WriteLine(e);
        }

        _serialFile.Refresh();
        return location;
    }

    private string TowPackage(PackageManifest manifest)
    {
        var location = GetPackageStoreLocation(manifest);

        var destination = $"{_root}/{manifest.Id}/backup_{Path.GetRandomFileName()}";
        Directory.Move(location, destination);
        return destination;
    }

    public PackageManifest? InspectLocal(string packageId, SemVersion version)
    {
        var location = GetInstalledPackageLocationOrDefault(packageId, version);
        if (location == null)
        {
            return null;
        }

        using var stream = File.OpenRead(Path.Combine(location, "manifest.json"));
        return JsonSerializer.Deserialize(stream, CoreJsonContext.Default.PackageManifest);
    }

    public void DeletePackage(PackageManifest manifest)
    {
        var location = GetPackageStoreLocation(manifest);
        Directory.Delete(location, true);
        _serialFile.Refresh();
    }

    public bool CompareSerialWith(SerialFile serial)
    {
        return ((ISerialed)_serialFile).CompareSerialWith(serial);
    }

    public void CopySerial(SerialFile from)
    {
        ((ISerialed)_serialFile).CopySerial(from);
    }

    public long GetSerial()
    {
        return ((ISerialed)_serialFile).GetSerial();
    }

    public void CopySerial(ISerialed from)
    {
        ((ISerialed)_serialFile).CopySerial(from);
    }
}
