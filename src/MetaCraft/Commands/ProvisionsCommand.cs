// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;

namespace MetaCraft.Commands;

public class ProvisionsCommand
{
    private readonly PackageScope _scope;

    public ProvisionsCommand(PackageScope scope)
    {
        _scope = scope;
    }

    public Command Create()
    {
        var command = new Command("provisions", Lc.L("Query and update provisions"));

        command.AddCommand(CreateUpdate());

        return command;
    }

    private Command CreateUpdate()
    {
        var optForce = new Option<bool>(["-F", "--force"], Lc.L("Regenerates even if the provisions database is up-to-date"));

        var command = new Command("update", Lc.L("Regenerates provision file"))
        {
            optForce
        };

        command.SetHandler(ExecuteUpdate, optForce);

        return command;
    }

    private void ExecuteUpdate(bool force)
    {
        var args = new RefreshProvisionsTransaction.Parameters(force);
        var transaction = new RefreshProvisionsTransaction(_scope.Container, args);

        transaction.Commit(new ConsoleAgent());
    }
}
