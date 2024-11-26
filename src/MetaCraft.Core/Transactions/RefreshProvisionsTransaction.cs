// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

public class RefreshProvisionsTransaction : Transaction
{
    public RefreshProvisionsTransaction(PackageContainer target) : base(target)
    {
    }

    public override void Commit(ITransactionAgent agent)
    {
        
    }
}
