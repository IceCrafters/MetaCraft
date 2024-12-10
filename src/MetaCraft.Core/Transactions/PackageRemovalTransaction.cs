// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Locales;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using Semver;

namespace MetaCraft.Core.Transactions;

public class PackageRemovalTransaction : ArgumentedTransaction<PackageRemovalTransaction.Parameters>
{
    public readonly record struct Parameters(string Id, SemVersion Version, bool IgnoreScriptFailure);

    public PackageRemovalTransaction(IPackageScope target, Parameters argument) : base(target, argument)
    {
    }

    private void ExecuteInternal(ITransactionAgent agent)
    {
        var manifest = Container.InspectLocal(Argument.Id, Argument.Version);
        if (manifest == null)
        {
            agent.PrintWarning(Lc.L("skipped nonexistent package '{0}' ({1})", Argument.Id, Argument.Version));
            return;
        }
        
        // Execute pre-removal script
        var d = Path.DirectorySeparatorChar;
        var scriptFile = Container.GetPlatformFile(manifest, $"config{d}scripts{d}remove");

        // Only execute if ExecuteBatch is supported, and scriptFile is not null
        if (scriptFile != null && PlatformUtil.IsBatchSupported())
        {
            var location = Container.GetInstalledPackageLocationOrDefault(manifest)!;
            
            agent.PrintInfo(Lc.L("Executing pre-removal script from '{0}' ({1})...", Argument.Id, Argument.Version));
            var exitCode = PlatformUtil.ExecuteBatch(scriptFile, location, manifest.Version.ToString());
            if (exitCode != 0)
            {
                // If --force is specified, do not fail when script fails to execute
                if (Argument.IgnoreScriptFailure)
                {
                    agent.PrintInfo(Lc.L("ignoring failure exit code {0} for pre-removal script, proceeding removal", exitCode));   
                }
                else
                {
                    throw new TransactionException(Lc.L("Pre-removal script returned failure exit code {0}", exitCode));
                }
            }
        }
        
        agent.PrintInfo(Lc.L("Removing package '{0}' ({1})...", manifest.Id, manifest.Version));
        Container.DeletePackage(manifest);
    }

    public override void Commit(ITransactionAgent agent)
    {
        ExecuteInternal(agent);
    }
}
