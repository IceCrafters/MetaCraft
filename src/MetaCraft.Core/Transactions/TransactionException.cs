// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Localisation;

namespace MetaCraft.Core.Transactions;

public class TransactionException : Exception
{
    public TransactionException()
    {
    }

    public TransactionException(string? message) : base(message)
    {
    }

    public TransactionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public static TransactionException CreateFailed(Exception cause)
    {
        return new TransactionException(Lc.L("Transaction failed."), cause);
    }
}
