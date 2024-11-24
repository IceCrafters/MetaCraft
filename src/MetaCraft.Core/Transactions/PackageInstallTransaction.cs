// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

public class PackageInstallTransaction
{
    private readonly IEnumerable<PackageArchive> _packages;
    private readonly PackageContainer _destination;
    private readonly bool _overwrite;

    public PackageInstallTransaction(IEnumerable<PackageArchive> packages, PackageContainer destination, bool overwrite)
    {
        _packages = packages;
        _destination = destination;
        _overwrite = overwrite;
    }

    public void Execute(ITransactionAgent agent)
    {
        foreach (var package in _packages)
        {
            ExecuteInternal(package, agent);
        }
    }

    private void ExecuteInternal(PackageArchive package, ITransactionAgent agent)
    {
        using var lck = _destination.Parent.Lock();
     
        // Expand the archive.
        var location = _destination.InsertPackage(package.Manifest, _overwrite);
     
        agent.PrintInfo(Strings.PackageInstallExpand, package.Manifest.Id, package.Manifest.Version);
        package.ExpandArchive(location, _overwrite);
        
        // Find and execute configure scripts
        var configurePath = Path.Combine(location, "config", "scripts", "install");
        
        // Doesn't need to run scripts if none
        if (!Directory.Exists(configurePath)) return;
        
        if (!OperatingSystem.IsWindows()
            && !OperatingSystem.IsLinux()
            && !OperatingSystem.IsMacOS()
            && !OperatingSystem.IsFreeBSD())
        {
            // No scripts will ever match, and if they do, we don't have stuff to
            // execute them
            return;
        }

        // Find the file to execute.
        var fileToExecute = GetInstallScriptExecute(configurePath);
        if (fileToExecute == null)
        {
            return;
        }

        // Execute the file.
        agent.PrintInfo(Strings.PackageInstallConfigure, package.Manifest.Id, package.Manifest.Version);
        var exitCode = PlatformUtil.ExecuteBatch(fileToExecute);
                
        // On nonzero exit code
        if (exitCode != 0)
        {
            throw new TransactionException(string.Format(Strings.PackageConfigureScriptFailed,
                exitCode));
        }
        
        File.WriteAllText(Path.Combine(location, ".configured"), "CONFIGURED");
    }

    private static string? GetInstallScriptExecute(string configurePath)
    {
        var exactPlatformFile = Path.Combine(configurePath, PlatformIdentifier.Current.ToString());
        var anyArchitectureFile = Path.Combine(configurePath, new PlatformIdentifier(PlatformIdentifier.Current.System,
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
}