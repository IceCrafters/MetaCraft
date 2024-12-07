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

        var packageManifests = toInstall as PackageManifest[] ?? toInstall.ToArray();
        return CheckDependency(packageManifests, scope, agent)
               && CheckConflict(packageManifests, scope, agent);
    }

    private static bool CheckConflict(PackageManifest[] toInstall, IPackageScope scope, ITransactionAgent agent)
    {
        bool DoesConflict(RangedPackageReference reference, PackageManifest from)
        {
            var retVal = true;
            // TODO replace with an optimised search table
            // Search for toInstall
            foreach (var entry in toInstall)
            {
                if (ReferenceEquals(from, entry))
                {
                    continue;
                }
                
                if (reference.Contains(entry))
                {
                    retVal = false;
                    break;
                }

                if (entry.Provides?.Any(reference.Contains) == true)
                {
                    retVal = false;
                    break;
                }
            }

            // Search for local
            foreach (var package in scope.Container.EnumeratePackages())
            {
                foreach (var version in scope.Container.EnumerateVersions(package))
                {
                    if (reference.Contains(package, version))
                    {
                        retVal = false;
                        break;
                    }
                    
                    var manifest = scope.Container.InspectLocal(package, version);
                    if (manifest.Provides?.Any(reference.Contains) == true)
                    {
                        retVal = false;
                        break;
                    }
                }
            }
            
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

    private static bool CheckDependency(PackageManifest[] toInstall, IPackageScope scope, ITransactionAgent agent)
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
