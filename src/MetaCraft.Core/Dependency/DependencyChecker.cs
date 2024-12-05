// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;

namespace MetaCraft.Core.Dependency;

public static class DependencyChecker
{
    public static bool DoesDependencySatisfy(ISet<PackageManifest> toInstall, IPackageScope scope, ITransactionAgent agent)
    {
        if (scope.Referrals.GetSerial() != scope.Container.GetSerial())
        {
            new UpdateReferrersTransaction(scope.Container, new UpdateReferrersTransaction.Parameters()).Commit(agent);
        }

        return CheckDependency(toInstall, scope, agent)
            && CheckConflict(toInstall, scope, agent);
    }

    private static bool CheckConflict(ISet<PackageManifest> toInstall, IPackageScope scope, ITransactionAgent agent)
    {
        bool DoesConflict(RangedPackageReference reference)
        {
            var retVal = !toInstall.Any(x
                => reference.Contains(x)
                    || x.Provides?.Any(x => reference.Contains(x)) != true) 
                && scope.Referrals.Locate(reference) == null;
            if (!retVal)
            {
                agent.PrintError(Lc.L("Conflicting clause: '{0}' ({1})", reference.Id, reference.Version));
            }

            return retVal;
        }

        foreach (var package in toInstall)
        {
            if (package.ConflictsWith != null &&
                !package.ConflictsWith.Select(x => new RangedPackageReference(x.Key, x.Value))
                .All(DoesConflict))
            {
                return false;
            }
        }

        return true;
    }

    private static bool CheckDependency(ISet<PackageManifest> toInstall, IPackageScope scope, ITransactionAgent agent)
    {
        bool DoesIncludeDep(RangedPackageReference reference)
        {
            var retVal = toInstall.Any(reference.Contains) || scope.Referrals.Locate(reference) != null;
            if (!retVal)
            {
                agent.PrintError(Lc.L("Unsatisified dependency: '{0}' ({1})", reference.Id, reference.Version));
            }

            return retVal;
        }

        foreach (var package in toInstall)
        {
            if (package.Dependencies != null &&
                !package.Dependencies.Select(x => new RangedPackageReference(x.Key, x.Value))
                .All(DoesIncludeDep))
            {
                return false;
            }
        }

        return true;
    }
}
