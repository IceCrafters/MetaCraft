// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;

namespace MetaCraft.Core.Dependency;

public static class DependencyChecker
{
    public static bool DoesDependencySatisfy(IEnumerable<PackageManifest> toInstall, IPackageScope scope, ITransactionAgent agent)
    {
        if (scope.Referrals.GetSerial() != scope.Container.GetSerial())
        {
            new UpdateReferrersTransaction(scope.Container, new UpdateReferrersTransaction.Parameters()).Commit(agent);
        }

        return CheckDependency(toInstall, scope, agent)
            && CheckConflict(toInstall, scope, agent);
    }

    private static bool CheckConflict(IEnumerable<PackageManifest> toInstall, IPackageScope scope, ITransactionAgent agent)
    {
        bool DoesConflict(RangedPackageReference reference, PackageManifest from)
        {
            var retVal = !toInstall
            .Where(x => !ReferenceEquals(x, from))
            .Any(x
                => reference.Contains(x)
                    || x.Provides?.Any(x => reference.Contains(x)) != true) 
                && scope.Referrals.Locate(reference) == null;
            if (!retVal)
            {
                agent.PrintError(Lc.L("from '{0}' ({1}): conflicting clause: '{2}' ({3})", 
                    from.Id, 
                    from.Version, 
                    reference.Id, 
                    reference.Version));
            }

            return retVal;
        }

        foreach (var package in toInstall)
        {
            if (package.ConflictsWith != null &&
                !package.ConflictsWith.Select(x => new RangedPackageReference(x.Key, x.Value))
                .All(x => DoesConflict(x, package)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool CheckDependency(IEnumerable<PackageManifest> toInstall, IPackageScope scope, ITransactionAgent agent)
    {
        bool DoesIncludeDep(RangedPackageReference reference, PackageManifest from)
        {
            var retVal = toInstall.Any(reference.Contains) || scope.Referrals.Locate(reference) != null;
            if (!retVal)
            {
                agent.PrintError(Lc.L("from '{0}' ({1}): missing dependency: '{2}' ({3})", 
                    from.Id, 
                    from.Version, 
                    reference.Id, 
                    reference.Version));
            }

            return retVal;
        }

        foreach (var package in toInstall)
        {
            if (package.Dependencies != null &&
                !package.Dependencies.Select(x => new RangedPackageReference(x.Key, x.Value))
                .All(x => DoesIncludeDep(x, package)))
            {
                return false;
            }
        }

        return true;
    }
}
