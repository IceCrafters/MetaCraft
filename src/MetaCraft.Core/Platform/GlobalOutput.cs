// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.ComponentModel;
using MetaCraft.Core.Transactions;

namespace MetaCraft.Core.Platform;

public static class GlobalOutput
{
    private static readonly List<ITransactionAgent> Agents = [];

    public static void AddAgent(ITransactionAgent agent)
    {
        Agents.Add(agent);
    }

    public static void Information([Localizable(true)] string message)
    {
        Agents.ForEach(agent => agent.PrintInfo(message));
    }

    public static void Warning(Exception ex, [Localizable(true)] string message)
    {
        Agents.ForEach(agent => agent.PrintWarning(ex, message));
    }
    
    public static void Warning([Localizable(true)] string message)
    {
        Agents.ForEach(agent => agent.PrintWarning(message));
    }
}