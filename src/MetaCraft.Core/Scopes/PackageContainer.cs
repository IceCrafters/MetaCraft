// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Platform;
using Semver;

namespace MetaCraft.Core.Scopes;

public class PackageContainer
{
    private const string VersionFileName = "VERSION";
    private readonly string _root;

    internal PackageContainer(string root, PackageScope parent)
    {
        _root = root;
        Directory.CreateDirectory(_root);
        Parent = parent;
    }

    public PackageScope Parent { get; }

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

    public string? GetInstalledPackageLocation(PackageManifest manifest)
    {
        var location = GetPackageStoreLocation(manifest);
        return !Directory.Exists(location) ? null : location;
    }
    
    public string? GetInstalledPackageLocation(string packageId, SemVersion version)
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
            {   
                using var stream = File.Create(Path.Combine(location, "manifest.json"));
                JsonSerializer.Serialize(stream, manifest, CoreJsonContext.Default.PackageManifest);
            }

            // Place version number file in case of need
            File.WriteAllText(Path.Combine(location, VersionFileName),
                manifest.Version.ToString());
        }
        catch (Exception ex)
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
        var location = GetInstalledPackageLocation(packageId, version);
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
    }
}
