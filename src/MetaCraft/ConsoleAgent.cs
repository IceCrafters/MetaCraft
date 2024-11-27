// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Runtime.CompilerServices;
using MetaCraft.Core.Transactions;
using Spectre.Console;

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

    public void PrintWarning(string message)
    {
        AnsiConsole.MarkupLineInterpolated($"[yellow]{Application.BaseName}:[/] {message}");
    }

    public void PrintWarning(string message, params object[] args)
    {
        AnsiConsole.MarkupInterpolated($"[yellow]{Application.BaseName}: [/]");
        AnsiConsole.WriteLine(message, args);
    }
}
