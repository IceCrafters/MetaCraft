// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace MetaCraft.Core.Scopes.Referral;

/// <summary>
/// An index containing all package referrers for a given version, as well as the currently
/// selected referrer for that version.
/// </summary>
public sealed class PackageReferralIndex
{
    [JsonConstructor]
    public PackageReferralIndex() {}

    [SetsRequiredMembers]
    public PackageReferralIndex(PackageReferrerDictionary referrers, string current)
    {
        Referrers = referrers;
        Current = current;
    }

    public required PackageReferrerDictionary Referrers { get; init; }

    public required string Current { get; init; }
}
