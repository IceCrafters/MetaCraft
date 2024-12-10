// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;

namespace MetaCraft.Testing;

/// <summary>
/// Provides an implementation fo <see cref="MockPackageScope"/> that is suitable for mock
/// testing.
/// </summary>
public sealed class MockPackageScope : IPackageScope
{
    private MockDisposable? _currentLock;

    public MockPackageScope(IPackageContainer container, PackageReferralDatabase referrals)
    {
        Container = container;
        Referrals = referrals;
        
        Root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(Root);
    }
    
    public IPackageContainer Container { get; }
    public PackageReferralDatabase Referrals { get; }
    public string Root { get; }
    
    public IDisposable Lock()
    {
        if (_currentLock is { IsDisposed: false })
        {
            throw new InvalidOperationException("Another lock still present.");
        }
        
        _currentLock = new MockDisposable();
        return _currentLock;
    }
}