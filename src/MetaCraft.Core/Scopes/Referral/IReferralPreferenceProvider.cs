// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

namespace MetaCraft.Core.Scopes.Referral;

public interface IReferralPreferenceProvider
{
    string? GetPreferredId();
}