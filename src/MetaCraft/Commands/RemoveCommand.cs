// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using System.CommandLine.Parsing;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using Semver;

namespace MetaCraft.Commands;

public class RemoveCommand
{
    private readonly PackageScope _scope;

    public RemoveCommand(PackageScope scope)
    {
        _scope = scope;
    }

    public Command Create()
    {
        // -- ARGUMENTS --
        var argPackage = new Argument<string>(@"package", Lc.L("The package to remove"));
        
        var argVersion = new Argument<string?>(@"version", () => null, Lc.L("The version to remove"));
        argVersion.AddValidator(ValidateVersion);
        
        var optForce = new Option<bool>(@"--force", Lc.L("If specified, ignore pre-removal script failures"));
        
        // -- COMMAND --
        var command = new Command(@"remove", Lc.L("Uninstalls a package from the current scope"))
        {
            argPackage,
            argVersion,
            optForce
        };
        
        command.SetHandler(Execute, argPackage, argVersion, optForce);
        return command;
    }

    private static void ValidateVersion(ArgumentResult symbol)
    {
        var value = symbol.GetValueOrDefault<string?>();
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!SemVersion.TryParse(value, SemVersionStyles.Any, out _))
        {
            symbol.ErrorMessage = Lc.L("version not found: {0}", value);
        }
    }

    private void Execute(string package, string? version, bool force)
    {
        var semVer = (version != null
            ? SemVersion.Parse(version, SemVersionStyles.Any)
            : _scope.Container.GetLatestVersion(package))
              ?? throw new InteractiveException(Lc.L($"no such package: '{package}' ({version ?? @"latest"})"));

        var transaction = new PackageRemovalTransaction(_scope.Container,
            new PackageRemovalTransaction.Parameters
            {
                Id = package,
                Version = semVer,
                Force = force
            });
        transaction.Commit(new ConsoleAgent());
    }
}
