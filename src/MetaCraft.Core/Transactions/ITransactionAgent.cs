// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.ComponentModel;

namespace MetaCraft.Core.Transactions;

public interface ITransactionAgent
{
    void PrintInfo([Localizable(true)] string message);
    void PrintInfo(string message, params object[] args);
    void PrintWarning([Localizable(true)] string message);
    void PrintWarning(string message, params object[] args);
}
