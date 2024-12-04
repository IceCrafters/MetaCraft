// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

/// <summary>
/// Represents an operation on the package container.
/// </summary>
public abstract class ArgumentedTransaction<TArg> : ITransaction
{
    protected ArgumentedTransaction(IPackageContainer target, TArg argument)
    {
        Argument = argument;
        Target = target;
    }

    public IPackageContainer Target { get; }
    public TArg Argument { get; }

    /// <summary>
    /// If implemented, executes this transaction.
    /// </summary>
    /// <param name="agent">The agent to report to.</param>
    /// <exception cref="TransactionException">Error occurred during transaction.</exception>
    public abstract void Commit(ITransactionAgent agent);
}