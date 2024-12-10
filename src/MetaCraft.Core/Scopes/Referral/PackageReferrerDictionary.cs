// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive.References;
using MetaCraft.Core.Archive;

namespace MetaCraft.Core.Scopes.Referral;

/// <summary>
/// Represents a collection of referrer identifiers and package references.
/// </summary>
/// <remarks>
/// <para>
/// There can exist more than one package referrers under one package version, and each referrer is
/// uniquely identified by a referrer ID. This referrer ID can be any arbitrary string that fits
/// into the JSON.
/// </para>
/// </remarks>
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
