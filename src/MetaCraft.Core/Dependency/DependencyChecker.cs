// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using Semver;

namespace MetaCraft.Core.Dependency;

public class DependencyChecker
{
    private readonly List<(string, SemVersion)> _packageCache = [];
    private readonly IPackageScope _packageScope;

    public DependencyChecker(IPackageScope packageScope)
    {
        _packageScope = packageScope;
        BuildPackageCache();
    }

    private void BuildPackageCache()
    {
        _packageCache.Clear();
        var container = _packageScope.Container;
        foreach (var id in container.EnumeratePackages())
        {
            foreach (var version in container.EnumerateVersions(id))
            {
                _packageCache.Add((id, version));   
            }
        }
    }
    
    private bool SearchFor(RangedPackageReference reference,
        PackageManifest from,
        PackageManifest[] toInstall, 
        IPackageScope scope)
    {
        // TODO replace with better searching logic
        
        // Search for toInstall
        foreach (var entry in toInstall)
        {
            if (ReferenceEquals(from, entry))
            {
                continue;
            }
                
            if (reference.Contains(entry)
                || entry.Provides?.Any(reference.Contains) == true)
            {
                return true;
            }

            if (entry.Provides?.Any(reference.Contains) == true)
            {
                return true;
            }
        }

        // Search for local
        foreach (var (package, version) in _packageCache)
        {
            if (reference.Contains(package, version))
            {
                return true;
            }
                    
            var manifest = scope.Container.InspectLocal(package, version);
            if (manifest?.Provides?.Any(reference.Contains) == true)
            {
                return true;
            }
        }

        return false;
    }

    public static bool DoesDependencySatisfy(IEnumerable<PackageManifest> toInstall, IPackageScope scope,
        ITransactionAgent agent)
    {
        var checker = new DependencyChecker(scope);
        return checker.DoesDependencySatisfy(toInstall, agent);
    }
    
    public bool DoesDependencySatisfy(IEnumerable<PackageManifest> toInstall, ITransactionAgent agent)
    {
        if (_packageScope.Referrals.GetSerial() != _packageScope.Container.GetSerial())
        {
            new UpdateReferrersTransaction(_packageScope.Container, new UpdateReferrersTransaction.Parameters()).Commit(agent);
        }

        var packageManifests = toInstall as PackageManifest[] ?? toInstall.ToArray();
        return CheckDependency(packageManifests, _packageScope, agent)
               && CheckConflict(packageManifests, _packageScope, agent);
    }

    private bool CheckConflict(PackageManifest[] toInstall, IPackageScope scope, ITransactionAgent agent)
    {
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

        bool DoesConflict(RangedPackageReference reference, PackageManifest from)
        {
            var retVal = !SearchFor(reference, from, toInstall, scope);
            
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
    }

    private static bool CheckDependency(PackageManifest[] toInstall, IPackageScope scope, ITransactionAgent agent)
    {
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
    }
}
