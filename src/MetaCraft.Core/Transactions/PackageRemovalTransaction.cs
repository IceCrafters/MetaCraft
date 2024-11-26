// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Data.Common;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using Semver;

namespace MetaCraft.Core.Transactions;

public class PackageRemovalTransaction : ArgumentedTransaction<PackageRemovalTransaction.Parameters>
{
    public readonly record struct Parameters(string Id, SemVersion Version, bool Force);

    public PackageRemovalTransaction(PackageContainer target, Parameters argument) : base(target, argument)
    {
    }

    private void ExecuteInternal(ITransactionAgent agent)
    {
        var manifest = Target.InspectLocal(Argument.Id, Argument.Version);
        if (manifest == null)
        {
            agent.PrintInfo(Strings.PackageRemoveNonExistent, Argument.Id, Argument.Version);
            return;
        }
        
        // Execute pre-removal script
        var d = Path.DirectorySeparatorChar;
        var scriptFile = Target.GetPlatformFile(manifest, $"config{d}scripts{d}remove");

        // Only execute if ExecuteBatch is supported, and scriptFile is not null
        if (scriptFile != null && PlatformUtil.IsBatchSupported())
        {
            var location = Target.GetInstalledPackageLocationOrDefault(manifest)!;
            
            agent.PrintInfo(Strings.PackageRemoveConfigure, Argument.Id, Argument.Version);
            var exitCode = PlatformUtil.ExecuteBatch(scriptFile, location, manifest.Version.ToString());
            if (exitCode != 0)
            {
                // If --force is specified, do not fail when script fails to execute
                if (Argument.Force)
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
        Target.DeletePackage(manifest);
    }

    public override void Commit(ITransactionAgent agent)
    {
        ExecuteInternal(agent);
    }
}
