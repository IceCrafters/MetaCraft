// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

public abstract class BaseFinalActionTransaction : Transaction
{
    public BaseFinalActionTransaction(IPackageScope target, ITransaction? final) : base(target)
    {
        Final = final;
    }

    public ITransaction? Final { get; }

    public override void Commit(ITransactionAgent agent)
    {
        try
        {
            OnCommit(agent);
        }
        catch (Exception ex)
        {
            throw ExecuteFinalActionInternalSafe(agent, ex);
        }

        ExecuteFinalActionInternal(agent);
    }

    public abstract void OnCommit(ITransactionAgent agent);

    private void ExecuteFinalActionInternal(ITransactionAgent agent)
    {
        Final?.Commit(agent);
    }

    private Exception ExecuteFinalActionInternalSafe(ITransactionAgent agent, Exception other)
    {
        if (Final == null)
        {
            return TransactionException.CreateFailed(other);
        }

        try
        {
            ExecuteFinalActionInternal(agent);
        }
        catch (Exception ex)
        {
            return new AggregateException(ex, other);
        }

        return TransactionException.CreateFailed(other);
    }

}
