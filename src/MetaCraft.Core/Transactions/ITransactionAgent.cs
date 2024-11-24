// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

namespace MetaCraft.Core.Transactions;

public interface ITransactionAgent
{
    void PrintInfo(string message);
    void PrintInfo(string message, params object[] args);
}