// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;
using MetaCraft.Core.Transactions;

namespace MetaCraft.Core.Dependency;

public interface IDependencyChecker
{
    bool DoesDependencySatisfy(IEnumerable<PackageManifest> toInstall, ITransactionAgent agent);
    
    /// <summary>
    /// Scans the local package container for any package that depends on the specified package.
    /// </summary>
    /// <param name="package">The package to scan for dependents.</param>
    /// <returns>
    /// <see langword="true"/> if there exists packages dependent on the specified package;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool HasDependents(PackageManifest package);
}