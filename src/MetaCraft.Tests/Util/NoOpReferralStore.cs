// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;

namespace MetaCraft.Tests.Util;

public sealed class NoOpReferralStore : IReferralDatabaseStore
{
    public void Clear()
    {
        // no-op
    }

    public SerialFile GetSerialFile()
    {
        return new SerialFile(Path.GetTempFileName());
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
