// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Projection;
using MetaCraft.Core.Scopes.Referral;

namespace MetaCraft.Core.Scopes;

public interface IPackageScope
{
    IPackageContainer Container { get; }
    
    PackageReferralDatabase Referrals { get; }
    
    IProjectionSpace Projection { get; }
    
    string Root { get; }

    IDisposable Lock();
}