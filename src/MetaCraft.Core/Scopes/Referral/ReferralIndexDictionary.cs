// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Common;

namespace MetaCraft.Core.Scopes.Referral;

public class ReferralIndexDictionary : Dictionary<SemVersionKey, PackageReferralIndex>
{
    public ReferralIndexDictionary()
    {
    }

    public ReferralIndexDictionary(IDictionary<SemVersionKey, PackageReferralIndex> dictionary) : base(dictionary)
    {
    }

    public ReferralIndexDictionary(IEnumerable<KeyValuePair<SemVersionKey, PackageReferralIndex>> collection) : base(collection)
    {
    }

    public ReferralIndexDictionary(int capacity) : base(capacity)
    {
    }

#if DEBUG

    public override string ToString()
    {
        return $"ReferralIndexDictionary[{Count}]";
    }

#endif
}
