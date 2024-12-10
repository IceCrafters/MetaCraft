// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

/// <summary>
/// Executes all contained transactions in serial, and executes a "final action" regardless of
/// whether the previous transactions succeeds or not.
/// </summary>
public sealed class FinalActionAggregateTransaction : BaseFinalActionTransaction
{
    public FinalActionAggregateTransaction(IPackageScope target, Parameter argument) : base(target,
        argument.Finally)
    {
        Argument = argument;
    }

    public Parameter Argument { get; }

    public override void OnCommit(ITransactionAgent agent)
    {
        foreach (var item in Argument.Items)
        {
            item.Commit(agent);
        }
    }

    public sealed class Parameter
    {
        [SetsRequiredMembers]
        public Parameter(IEnumerable<ITransaction> items, ITransaction? @finally = null)
        {
            Items = items;
            Finally = @finally;
        }

        public required IEnumerable<ITransaction> Items { get; init; }
        public ITransaction? Finally { get; init; }
    }
}
