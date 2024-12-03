// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using System.CommandLine.Parsing;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using Semver;

namespace MetaCraft.Commands;

public class ReferralCommand
{
    private readonly PackageScope _scope;

    public ReferralCommand(PackageScope scope)
    {
        _scope = scope;
    }

    public Command Create()
    {
        var command = new Command("referral", Lc.L("Query and update referral database"));

        command.AddCommand(CreateUpdate());
        command.AddCommand(CreateSelect());

        return command;
    }

    private Command CreateSelect()
    {
        var argClauseName = new Argument<string>("referrerName", Lc.L("Name of the referrer"));
        var argReferrerName = new Argument<string>("packageName", Lc.L("Name of the package"));

        var optClauseVersion = new Option<string>(["--rV", "--referrer-version"],
            Lc.L("Version of the referrer to select a package for"));
        
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

    private void ValidateVersion(OptionResult symbol)
    {
        if (!SemVersion.TryParse(symbol.Tokens.Single().Value, SemVersionStyles.Any, out _))
        {
            symbol.ErrorMessage = Lc.L("Invalid version number");
        }
    }

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
        var transaction = new UpdateReferrersTransaction(_scope.Container, args);

        transaction.Commit(new ConsoleAgent());
    }
    
    #endregion
    
}
