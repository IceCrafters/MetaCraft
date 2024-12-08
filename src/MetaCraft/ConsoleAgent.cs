// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.ComponentModel;
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

    public void PrintWarning(Exception exception, string message)
    {
        PrintWarning(message);
        AnsiConsole.WriteException(exception);
    }

    public void PrintWarning(string message, params object[] args)
    {
        AnsiConsole.MarkupInterpolated($"[yellow]{Application.BaseName}: [/]");
        AnsiConsole.WriteLine(message, args);
    }

    private static void PrintDebug(string message)
    {
        AnsiConsole.MarkupInterpolated($"[yellow]{Application.BaseName}: [/][bold gray]warning:[/] ");
        AnsiConsole.MarkupLineInterpolated($"[gray]{message}[/]");
    }
    
    public void PrintDebug(Exception exception, string message)
    {
        #if !DEBUG
        return;
        #else
        PrintDebug(message);
        AnsiConsole.WriteException(exception);
        #endif
    }

    public void PrintError([Localizable(true)] string message)
    {
        AnsiConsole.MarkupInterpolated($"[red]{Application.BaseName}: [/][bold gray]error:[/] ");
        AnsiConsole.MarkupLineInterpolated($"[white]{message}[/]");
    }
}
