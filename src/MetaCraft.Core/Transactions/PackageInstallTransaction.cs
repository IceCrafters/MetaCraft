// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Common.Platform;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using MetaCraft.Localisation;

namespace MetaCraft.Core.Transactions;

/// <summary>
/// Performs the installation of a single package.
/// </summary>
public sealed class PackageInstallTransaction : ArgumentedTransaction<PackageInstallTransaction.Parameters>
{
    public readonly record struct Parameters(PackageArchive Package, bool Overwrite);

    public PackageInstallTransaction(IPackageScope target, Parameters argument) : base(target, argument)
    {
    }

    public override void Commit(ITransactionAgent agent)
    {
        ExecuteInternal(Argument.Package, agent);
    }

    private void ExecuteInternal(PackageArchive package, ITransactionAgent agent)
    {
        using var lck = Target.Lock();
     
        // Expand the archive.
        var location = Container.InsertPackage(package.Manifest, Argument.Overwrite, true);
     
        agent.PrintInfo(Lc.L("Expanding package '{0}' ({1})...", package.Manifest.Id, package.Manifest.Version));
        package.ExpandArchive(location, Argument.Overwrite);
        
        if (!ExecuteConfigureScript(package, agent, location)) return;
        
        // Handle projection.
        var exports = package.Manifest.Runtime?.ExportAssemblies;
        if (exports != null)
        {
            agent.PrintInfo(Lc.L("Processing projections for package '{0}' ({1})...",
                package.Manifest.Id, 
                package.Manifest.Version));
            foreach (var export in exports)
            {
                var overwrite = false;
                
                // Get the projection source and see if it is an upgrade. If not, fail.
                var source = Target.Projection.GetProjectionSource(export);
                if (source.HasValue)
                {
                    if (source.Value.Name != package.Manifest.Id
                        || source.Value.Version.CompareSortOrderTo(package.Manifest.Version) >= 1)
                    {
                        throw new TransactionException(Lc.L("Projection '{0}' ({1}) already provided by '{2}' ({3})",
                            export.Name,
                            export.Version,
                            source.Value.Name,
                            source.Value.Version));
                    }

                    overwrite = true;
                }

                // Check for existence - this allows for a clearer error message.
                var projectPath = Path.Combine(location, export.Path);
                if (!File.Exists(projectPath))
                {
                    throw new TransactionException(Lc.L("Package '{0}' ({1}) tries to project file '{2}' which does not exist",
                        package.Manifest.Id,
                        package.Manifest.Version,
                        export.Path));
                }
                
                // Insert the projection.
                Target.Projection.Insert(package.Manifest, 
                    export,
                    Path.Combine(location, export.Path),
                    overwrite);
            }   
        }

        File.WriteAllText(Path.Combine(location, ".configured"), "CONFIGURED");
    }

    private static bool ExecuteConfigureScript(PackageArchive package, ITransactionAgent agent, string location)
    {
        // Find and execute configure scripts
        var configurePath = Path.Combine(location, "config", "scripts", "install");
        
        // Doesn't need to run scripts if none
        if (!Directory.Exists(configurePath)) return true;
        
        if (!OperatingSystem.IsWindows()
            && !OperatingSystem.IsLinux()
            && !OperatingSystem.IsMacOS()
            && !OperatingSystem.IsFreeBSD())
        {
            // No scripts will ever match, and even if they do, we don't have stuff to
            // execute them
            return false;
        }

        // Find the file to execute.
        var fileToExecute = GetInstallScriptExecute(configurePath);
        if (fileToExecute == null)
        {
            return false;
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

        return true;
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
