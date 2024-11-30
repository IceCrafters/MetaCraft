// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;

namespace MetaCraft.Core.Transactions;

public class UpdateReferrersTransaction : ArgumentedTransaction<UpdateReferrersTransaction.Parameters>
{
    public UpdateReferrersTransaction(PackageContainer target, Parameters argument) : base(target, argument)
    {
    }

    public override void Commit(ITransactionAgent agent)
    {
        throw new NotImplementedException();
    }

    public readonly record struct Parameters(bool IgnoreSerial);
}
