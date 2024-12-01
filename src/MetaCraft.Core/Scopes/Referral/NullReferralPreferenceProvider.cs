// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

namespace MetaCraft.Core.Scopes.Referral;

public sealed class NullReferralPreferenceProvider : IReferralPreferenceProvider
{
    public string? GetPreferredId()
    {
        // TODO temporary
        return null;
    }
}