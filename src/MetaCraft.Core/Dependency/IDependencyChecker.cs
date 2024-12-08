// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Transactions;

namespace MetaCraft.Core.Dependency;

public interface IDependencyChecker
{
    bool DoesDependencySatisfy(IEnumerable<PackageManifest> toInstall, ITransactionAgent agent);
}