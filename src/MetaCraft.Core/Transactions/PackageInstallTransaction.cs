// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Common.Platform;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Configuration;
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
        
        PackageConfigApplier.Apply(Target,
            package.Manifest,
            location,
            agent);
    }
}
