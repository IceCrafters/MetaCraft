// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;

namespace MetaCraft.Core.Scopes.Referral;

public sealed class PackageReferrerDictionary : Dictionary<string, PackageReference>
{
    public PackageReferrerDictionary()
    {
    }

    public PackageReferrerDictionary(IDictionary<string, PackageReference> dictionary) : base(dictionary)
    {
    }

    public PackageReferrerDictionary(IEnumerable<KeyValuePair<string, PackageReference>> collection) : base(collection)
    {
    }

    public PackageReferrerDictionary(int capacity) : base(capacity)
    {
    }
}
