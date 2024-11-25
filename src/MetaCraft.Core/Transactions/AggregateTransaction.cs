// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

public class AggregateTransaction : Transaction<IEnumerable<ITransaction>>
{
    public AggregateTransaction(PackageContainer target, IEnumerable<ITransaction> argument) : base(target, argument)
    {
    }

    public override void Commit(ITransactionAgent agent)
    {
        foreach (var transaction in Argument)
        {
            transaction.Commit(agent);
        }
    }
}