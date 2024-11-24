// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Transactions;

namespace MetaCraft;

public class ConsoleAgent : ITransactionAgent
{
    public void PrintInfo(string message)
    {
        Console.WriteLine(message);
    }

    public void PrintInfo(string message, params object[] args)
    {
        Console.WriteLine(message, args);
    }
}
