// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;

namespace MetaCraft.Testing;

/// <summary>
/// Provides a pure implementation of <see cref="IReferralDatabaseStore"/> that does nothing.
/// </summary>
/// <remarks>
/// <para>
/// This implementation will simply dictate all referral files that API consumers query as
/// non-existent and all write operations will be ignored.
/// </para>
/// </remarks>
public sealed class NoOpReferralStore : IReferralDatabaseStore
{
    private readonly InMemorySerialFile _serial = new(-1, readOnly: true);
    
    public void Clear()
    {
        // no-op
    }

    public ISerialed GetSerialFile()
    {
        return _serial;
    }

    public ReferralIndexDictionary? ReadFile(string packageName)
    {
        return null;
    }

    public void WriteFile(string packageName, ReferralIndexDictionary data)
    {
        // no-op
    }
}
