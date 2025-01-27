// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Transactions;
using MetaCraft.Localisation;
using Semver;

namespace MetaCraft;

public class InteractiveException : Exception
{
    public InteractiveException(string message) : base(message)
    {
    }

    public static InteractiveException FromException(string message, Exception exception)
    {
        return new InteractiveException($"{message}: {exception.Message}");
    }
    
    public static InteractiveException FileExists(string fileName)
    {
        return new InteractiveException(Lc.L("file already exists: {0}", fileName));
    }
    
    public static InteractiveException CreateNoSuchPackage(string packageId, SemVersion? version)
    {
        return version != null
            ? new InteractiveException(Lc.L("no such package '{0}' ({1})", packageId, version))
            : new InteractiveException(Lc.L("no such package '{0}'", packageId));
    }

    public static InteractiveException CreateTransactionFailed(TransactionException ex)
    {
        var msg = ex.InnerException?.Message ?? ex.Message;
        return new InteractiveException(Lc.L("transaction failed: {0}", msg));
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
