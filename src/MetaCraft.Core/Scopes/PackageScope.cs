// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

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
    }
    
    public IPackageContainer Container { get; }

    public PackageReferralDatabase Referrals { get; }
    
    public string Root { get; }

    public PackageScopeLock Lock()
    {
        return PackageScopeLock.Create(Path.Combine(Root, "lock"));
    }
}
