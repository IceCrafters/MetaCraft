// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;

namespace MetaCraft.Tests.Util;

public class MockReferralStore : IReferralDatabaseStore
{
    private readonly Dictionary<string, ReferralIndexDictionary> _dataIndex = [];

    public void Clear()
    {
        _dataIndex.Clear();
    }

    public SerialFile GetSerialFile()
    {
        return new SerialFile(Path.GetRandomFileName());
    }

    public ReferralIndexDictionary? ReadFile(string packageName)
    {
        return _dataIndex.GetValueOrDefault(packageName);
    }

    public void WriteFile(string packageName, ReferralIndexDictionary data)
    {
        _dataIndex[packageName] = data;
    }
}
