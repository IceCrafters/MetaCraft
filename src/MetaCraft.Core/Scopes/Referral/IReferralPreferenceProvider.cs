// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using Semver;

namespace MetaCraft.Core.Scopes.Referral;

public interface IReferralPreferenceProvider
{
    string? GetPreferredId(string name, SemVersion version);
    
    void SetPreferredId(string clauseId, SemVersion clauseVersion, string preferred);
}