// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using Semver;

namespace MetaCraft;

public class InteractiveException : Exception
{
    public InteractiveException(string message) : base(message)
    {
    }

    public static InteractiveException CreateNoSuchClause(string clauseId, SemVersion? clauseVersion)
    {
        return clauseVersion != null
            ? new InteractiveException(Lc.L("no such clause '{0}' ({1})", clauseId, clauseVersion))
            : new InteractiveException(Lc.L("no such clause '{0}'", clauseId));
    }

    public static InteractiveException CreateNoValidVersionFound(string clauseId)
    {
        return new InteractiveException(Lc.L("no valid version found for clause '{0}'", clauseId));
    }
}
