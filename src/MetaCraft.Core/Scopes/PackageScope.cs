// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

namespace MetaCraft.Core.Scopes;

/// <summary>
/// Manages a set of installed packages and their associated environment and application
/// configuration.
/// </summary>
public class PackageScope
{
    public PackageScope(string root)
    {
        Root = root;
        Directory.CreateDirectory(Root);

        Container = new PackageContainer(Path.Combine(root, "packages"), this);
        Provisions = new PackageProvisionDatabase(this);
    }
    
    public PackageContainer Container { get; }

    public PackageProvisionDatabase Provisions { get; }
    
    public string Root { get; }

    public PackageScopeLock Lock()
    {
        return PackageScopeLock.Create(Path.Combine(Root, "lock"));
    }
}
