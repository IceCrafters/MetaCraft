// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;
using MetaCraft.Common.Platform;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Transactions;
using MetaCraft.Localisation;

namespace MetaCraft.Core.Scopes.Configuration;

public static class PackageConfigApplier
{
    public static void Apply(IPackageScope target,
        PackageManifest manifest,
        string packageRoot,
        ITransactionAgent agent)
    {
        ExecuteConfigureScript(manifest, packageRoot, agent);
        ProcessProjections(target, manifest, packageRoot, agent);
        File.WriteAllText(Path.Combine(packageRoot, ".configured"), "CONFIGURED");
    }
    
    private static void ExecuteConfigureScript(PackageManifest manifest,
        string packageRoot,
        ITransactionAgent agent)
    {
        // Find and execute configure scripts
        var configurePath = Path.Combine(packageRoot, "config", "scripts", "install");
        
        // Doesn't need to run scripts if none
        if (!Directory.Exists(configurePath)) return;
        
        if (!OperatingSystem.IsWindows()
            && !OperatingSystem.IsLinux()
            && !OperatingSystem.IsMacOS()
            && !OperatingSystem.IsFreeBSD())
        {
            // No scripts will ever match, and even if they do, we don't have stuff to
            // execute them
            agent.PrintWarning(Lc.L("Package contains scripts but they are not supported on this platform"));
            return;
        }

        // Find the file to execute.
        var fileToExecute = GetInstallScriptExecute(configurePath);
        if (fileToExecute == null)
        {
            return;
        }

        // Execute the file.
        agent.PrintInfo(Lc.L("Running configure script for '{0}' ({1})...", manifest.Id, manifest.Version));
        var exitCode = PlatformUtil.ExecuteBatch(fileToExecute);
                
        // On nonzero exit code
        if (exitCode != 0)
        {
            throw new TransactionException(Lc.L("Configure script exited with exit code {0}.",
                exitCode));
        }
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
    
    private static void ProcessProjections(IPackageScope target, PackageManifest manifest, string packageRoot,
        ITransactionAgent agent)
    {
        // Handle projection.
        var exports = manifest.Runtime?.ExportAssemblies;
        if (exports == null) return;
        
        agent.PrintInfo(Lc.L("Processing projections for package '{0}' ({1})...",
            manifest.Id, 
            manifest.Version));
        foreach (var export in exports)
        {
            var overwrite = false;
                
            // Get the projection source and see if it is an upgrade. If not, fail.
            var source = target.Projection.GetProjectionSource(export);
            if (source.HasValue)
            {
                var sourceManifest = target.Container.InspectLocal(source.Value);
                
                // The package has to exist and valid.
                if (sourceManifest != null
                    && (sourceManifest.Id != manifest.Id
                    || source.Value.Version.CompareSortOrderTo(manifest.Version) >= 1
                    || !manifest.Unitary
                    || !sourceManifest.Unitary))
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
            var projectPath = Path.Combine(packageRoot, PackageArchive.ContentsDir, export.Path);
            if (!File.Exists(projectPath))
            {
                throw new TransactionException(Lc.L("Package '{0}' ({1}) tries to project file '{2}' which does not exist",
                    manifest.Id,
                    manifest.Version,
                    export.To));
            }
                
            // Insert the projection.
            target.Projection.Insert(manifest, 
                export,
                projectPath,
                overwrite);
        }
    }
}