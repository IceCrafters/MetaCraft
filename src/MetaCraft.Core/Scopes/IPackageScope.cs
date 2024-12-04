// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes.Referral;

namespace MetaCraft.Core.Scopes;

public interface IPackageScope
{
    IPackageContainer Container { get; }
    
    PackageReferralDatabase Referrals { get; }
    
    string Root { get; }

    PackageScopeLock Lock();
}