// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

public abstract class Transaction : ITransaction
{
    protected Transaction(IPackageContainer target)
    {
        Target = target;
    }

    public IPackageContainer Target { get; }

    /// <summary>
    /// If implemented, executes this transaction.
    /// </summary>
    /// <param name="agent">The agent to report to.</param>
    /// <exception cref="TransactionException">Error occurred during transaction.</exception>
    public abstract void Commit(ITransactionAgent agent);
}
