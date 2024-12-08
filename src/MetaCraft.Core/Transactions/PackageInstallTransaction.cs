// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Dependency;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

/// <summary>
/// Performs the installation of a single package.
/// </summary>
public sealed class PackageInstallTransaction : ArgumentedTransaction<PackageInstallTransaction.Parameters>
{
    public readonly record struct Parameters(PackageArchive Package, bool Overwrite);

    public PackageInstallTransaction(IPackageContainer target, Parameters argument) : base(target, argument)
    {
    }

    public override void Commit(ITransactionAgent agent)
    {
        ExecuteInternal(Argument.Package, agent);
    }

    private void ExecuteInternal(PackageArchive package, ITransactionAgent agent)
    {
        using var lck = Target.Parent.Lock();
     
        // Expand the archive.
        var location = Target.InsertPackage(package.Manifest, Argument.Overwrite, true);
     
        agent.PrintInfo(Lc.L("Expanding package '{0}' ({1})...", package.Manifest.Id, package.Manifest.Version));
        package.ExpandArchive(location, Argument.Overwrite);
        
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
        agent.PrintInfo(Lc.L("Running configure script for '{0}' ({1})...", package.Manifest.Id, package.Manifest.Version));
        var exitCode = PlatformUtil.ExecuteBatch(fileToExecute);
                
        // On nonzero exit code
        if (exitCode != 0)
        {
            throw new TransactionException(Lc.L("Configure script exited with exit code {0}.",
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
