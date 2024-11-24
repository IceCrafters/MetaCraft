// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

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
}