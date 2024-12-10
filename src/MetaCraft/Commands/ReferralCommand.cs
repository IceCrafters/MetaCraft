// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using System.CommandLine.Parsing;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using Semver;
using Spectre.Console;

namespace MetaCraft.Commands;

public class ReferralCommand
{
    private readonly IPackageScope _scope;

    public ReferralCommand(IPackageScope scope)
    {
        _scope = scope;
    }

    public Command Create()
    {
        var command = new Command("referral", Lc.L("Query and update referral database"));

        command.AddCommand(CreateUpdate());
        command.AddCommand(CreateSelect());
        command.AddCommand(CreateList());

        return command;
    }
    
    #region 'list'

    private Command CreateList()
    {
        var argClauseName = new Argument<string>("name", Lc.L("The name for the clause"));
        var optClauseVersion = new Option<string>(["--cV", "--clause-version"],
            Lc.L("The clause version to list referrers for"));
        optClauseVersion.AddValidator(ValidateVersion);
        var optIgnoreNonexistent = new Option<bool>(["-N", "--ignore-non-existent"],
            Lc.L("Treat non-existent clauses as empty clauses"));
        
        var command = new Command("list", Lc.L("List available referrers for a given clause"))
        {
            argClauseName,
            optClauseVersion,
            optIgnoreNonexistent
        };
        
        command.SetHandler(ExecuteList, argClauseName, optClauseVersion, optIgnoreNonexistent);

        return command;
    }

    private void ExecuteList(string clauseName, string? clauseVersionToken, bool ignoreNonExistent)
    {
        var clauseVersion =  clauseVersionToken != null
            ? SemVersion.Parse(clauseVersionToken, SemVersionStyles.Any)
            : null;

        if (!_scope.Referrals.ContainsClause(clauseName, clauseVersion))
        {
            if (ignoreNonExistent)
            {
                // Pretend nothing happened
                return;
            }

            throw InteractiveException.CreateNoSuchClause(clauseName, clauseVersion);
        }

        if (clauseVersion == null)
        {
            var latest = _scope.Referrals.GetLatestClauseVersion(clauseName);
            if (latest == null)
            {
                if (ignoreNonExistent)
                {
                    // Pretend nothing happened.
                    return;
                }
                
                throw InteractiveException.CreateNoValidVersionFound(clauseName);
            }

            clauseVersion = latest;
        }

        // Assemble some display stuff.
        var grid = new Grid();
        grid.AddColumn(new GridColumn()
            .Padding(0, 0, 1, 0));
        grid.AddColumn(new GridColumn()
            .Padding(0, 0));
        grid.AddColumn(new GridColumn()
            .Padding(1, 0, 0, 0));
        
        // Do the printing matter.
        foreach (var pair in _scope.Referrals.EnumerateReferrers(clauseName, clauseVersion))
        {
            grid.AddRow(pair.Key, ":", $"'{pair.Value.Name}' ({pair.Value.Version})");
        }
        
        AnsiConsole.Write(grid);
    }
    
    #endregion

    #region 'select'
    
    private Command CreateSelect()
    {
        var argClauseName = new Argument<string>("referrerName", Lc.L("Name of the referrer"));
        var argReferrerName = new Argument<string>("packageName", Lc.L("Name of the package"));

        var optClauseVersion = new Option<string>(["--cV", "--clause-version"],
            Lc.L("Version of the clause to select a package for"));
        
        optClauseVersion.AddValidator(ValidateVersion);

        var command = new Command("select", Lc.L("Selects a package as preferred provider for a clause"))
        {
            argClauseName,
            argReferrerName,
            optClauseVersion,
        };
        
        command.SetHandler(ExecuteSelect, argClauseName, argReferrerName, optClauseVersion);
        
        return command;
    }

    private void ExecuteSelect(string clauseName, string referrerName, string? clauseVersionToken)
    {
        var clauseVersion = clauseVersionToken != null
            ? SemVersion.Parse(clauseVersionToken, SemVersionStyles.Any)
            : null;

        if (!_scope.Referrals.ContainsClause(clauseName, clauseVersion))
        {
            throw new InteractiveException(Lc.L("Clause '{0}' not found",
                clauseName));
        }

        if (clauseVersion == null)
        {
            clauseVersion = _scope.Referrals.GetLatestClauseVersion(clauseName)
                ?? throw new InteractiveException(Lc.L("no valid version found for clause '{0}'", clauseName));
        }

        if (!_scope.Referrals.ContainsReferrer(clauseName, clauseVersion, referrerName))
        {
            throw new InteractiveException(Lc.L("not such referrer '{0}' for clause '{1}' ({2})",
                referrerName, 
                clauseName, 
                clauseVersion));
        }
        
        _scope.Referrals.SetPreferred(clauseName, clauseVersion, referrerName);
        
        ExecuteUpdate(true);
    }
    
    #endregion
    
    #region 'update'
    
    private Command CreateUpdate()
    {
        var optForce = new Option<bool>(["-F", "--force"], Lc.L("Regenerates even if the referral database is up-to-date"));

        var command = new Command("update", Lc.L("Regenerate referral database entries"))
        {
            optForce
        };

        command.SetHandler(ExecuteUpdate, optForce);

        return command;
    }

    private void ExecuteUpdate(bool force)
    {
        var args = new UpdateReferrersTransaction.Parameters(force);
        var transaction = new UpdateReferrersTransaction(_scope, args);

        transaction.Commit(new ConsoleAgent());
    }
    
    #endregion
    
    #region Validation
    
    private void ValidateVersion(OptionResult symbol)
    {
        if (!SemVersion.TryParse(symbol.Tokens.Single().Value, SemVersionStyles.Any, out _))
        {
            symbol.ErrorMessage = Lc.L("Invalid version number");
        }
    }
    
    #endregion
}
