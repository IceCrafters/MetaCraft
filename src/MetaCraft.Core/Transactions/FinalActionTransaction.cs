// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

public sealed class FinalActionTransaction : BaseFinalActionTransaction
{
    public FinalActionTransaction(IPackageContainer target, ITransaction toExecute, ITransaction? final) : base(target, final)
    {
        ToExecute = toExecute;
    }

    public ITransaction ToExecute { get; }

    public override void OnCommit(ITransactionAgent agent)
    {
        ToExecute.Commit(agent);
    }
}
