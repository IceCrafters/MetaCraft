// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Projection;
using MetaCraft.Core.Scopes.Referral;

namespace MetaCraft.Core.Scopes;

/// <summary>
/// Manages a set of installed packages and their associated environment and application
/// configuration.
/// </summary>
public class PackageScope : IPackageScope
{
    public PackageScope(string root)
    {
        Root = root;
        Directory.CreateDirectory(Root);

        Container = new PackageContainer(Path.Combine(root, "packages"), this);
        Referrals = new PackageReferralDatabase(new JsonFileReferralDatabaseStore(Path.Combine(root, "referrals")),
            new JsonFileReferralPreferenceProvider(Path.Combine(Root, "ref_preferences.json")));
        Projection = new ProjectionSpace(Path.Combine(root, "projections"));
    }
    
    public IPackageContainer Container { get; }

    public PackageReferralDatabase Referrals { get; }
    
    public IProjectionSpace Projection { get; }
    
    public string Root { get; }

    public IDisposable Lock()
    {
        return PackageScopeLock.Create(Path.Combine(Root, "lock"));
    }
}
