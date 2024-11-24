// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Locales;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using Semver;

namespace MetaCraft.Core.Transactions;

public class PackageRemovalTransaction
{
    private readonly IEnumerable<(string, SemVersion)> _packages;
    private readonly PackageContainer _destination;
    private readonly bool _force;

    public PackageRemovalTransaction(IEnumerable<(string, SemVersion)> packages, PackageContainer destination,
        bool force)
    {
        _packages = packages;
        _destination = destination;
        _force = force;
    }

    public void Execute(ITransactionAgent agent)
    {
        foreach (var package in _packages)
        {
            ExecuteInternal(agent, package);
        }
    }

    private void ExecuteInternal(ITransactionAgent agent, (string, SemVersion) toRemove)
    {
        var manifest = _destination.InspectLocal(toRemove.Item1, toRemove.Item2);
        if (manifest == null)
        {
            agent.PrintInfo(Strings.PackageRemoveNonExistent, toRemove.Item1, toRemove.Item2);
            return;
        }
        
        // Execute pre-removal script
        var d = Path.DirectorySeparatorChar;
        var scriptFile = _destination.GetPlatformFile(manifest, $"config{d}scripts{d}remove");

        // Only execute if ExecuteBatch is supported, and scriptFile is not null
        if (scriptFile != null && PlatformUtil.IsBatchSupported())
        {
            var location = _destination.GetInstalledPackageLocation(manifest)!;
            var exitCode = PlatformUtil.ExecuteBatch(scriptFile, location, manifest.Version.ToString());
            if (exitCode != 0)
            {
                // If --force is specified, do not fail when script fails to execute
                if (_force)
                {
                    agent.PrintInfo(Strings.PackageRemoveScriptFailNoError, exitCode);   
                }
                else
                {
                    throw new TransactionException(string.Format(Strings.PackageRemoveScriptError, exitCode));
                }
            }
        }
        
        agent.PrintInfo(Strings.PackageRemoveDeleting, manifest.Id, manifest.Version);
        _destination.DeletePackage(manifest);
    }
}
