// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive.References;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using MetaCraft.Localisation;
using Semver;

namespace MetaCraft.Core.Dependency;

public class DependencyChecker : IDependencyChecker
{
    private readonly List<(string, SemVersion)> _packageCache = [];
    private readonly IPackageScope _packageScope;
    private bool _cacheBuilt;

    public DependencyChecker(IPackageScope packageScope)
    {
        _packageScope = packageScope;
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

        _cacheBuilt = true;
    }

    private List<(string, SemVersion)> GetCache()
    {
        if (!_cacheBuilt)
        {
            BuildPackageCache();
        }
        
        return _packageCache;
    }
    
    private bool SearchFor(RangedPackageReference reference,
        PackageManifest from,
        PackageManifest[] toInstall, 
        IPackageScope scope)
    {
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
        foreach (var (package, version) in GetCache())
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
            new UpdateReferrersTransaction(_packageScope, new UpdateReferrersTransaction.Parameters()).Commit(agent);
        }

        var packageManifests = toInstall as PackageManifest[] ?? toInstall.ToArray();
        return CheckDependency(packageManifests, _packageScope, agent)
               && CheckConflict(packageManifests, _packageScope, agent);
    }

    public bool HasDependents(PackageManifest package)
    {
        foreach (var (id, version) in GetCache())
        {
            // Don't cache this. If we have say a thousand of packages, caching all of this
            // takes many memory.
            var manifest = _packageScope.Container.InspectLocal(id, version);

            if (manifest?.Dependencies == null)
            {
                continue;
            }

            // We only go so far as one dependency exist then return true.
            // This however means that if we have no dependencies, we go through everything.
            // TODO: make a proper cache for this
            foreach (var dependency in manifest.Dependencies)
            {
                // Check if contains the package itself
                if (dependency.Key == package.Id
                    && dependency.Value.Contains(package.Version))
                {
                    return true;
                }
                
                // Check if contains any provided referrals
                if (package.Provides?.Any(x =>
                        x.Key == dependency.Key
                        && dependency.Value.Contains(x.Value))
                    == true)
                {
                    return true;
                }
            }
        }
        
        return false;
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
