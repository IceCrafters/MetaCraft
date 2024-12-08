// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.ComponentModel;
using MetaCraft.Core.Transactions;
using Xunit.Abstractions;

namespace MetaCraft.Tests.Util;

public class TestTransactionAgent : ITransactionAgent
{
    private readonly ITestOutputHelper _output;

    public TestTransactionAgent(ITestOutputHelper output)
    {
        _output = output;
    }

    private void PrintLevel(string level, string message)
    {
        _output.WriteLine("[MetaCraft] {0}: {1}", level, message);
    }

    public void PrintDebug(Exception exception, string message)
    {
        PrintLevel("debug", message);
        PrintLevel("debug", exception.ToString());
    }

    public void PrintInfo([Localizable(true)] string message)
    {
        PrintLevel("info", message);
    }

    public void PrintInfo(string message, params object[] args)
    {
        PrintInfo(string.Format(message, args));
    }

    public void PrintWarning([Localizable(true)] string message)
    {
        PrintLevel("warning", message);
    }

    public void PrintWarning(Exception exception, [Localizable(true)] string message)
    {
        PrintWarning(message);
        PrintWarning(exception.ToString());
    }

    public void PrintWarning(string message, params object[] args)
    {
        PrintWarning(string.Format(message, args));
    }

    public void PrintError([Localizable(true)] string message)
    {
        PrintLevel("error", message);
    }
}
