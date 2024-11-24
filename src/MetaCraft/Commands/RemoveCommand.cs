// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

using System.CommandLine;
using System.CommandLine.Parsing;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using MetaCraft.Locales;
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
        var argPackage = new Argument<string>("packages", AppMessages.RemoveCommandArgumentPackage);
        
        var argVersion = new Argument<string?>("version", () => null, AppMessages.RemoveCommandArgumentVersion);
        argVersion.AddValidator(ValidateVersion);
        
        var optForce = new Option<bool>("--force", AppMessages.RemoveCommandOptionForce);
        
        // -- COMMAND --
        var command = new Command("remove", AppMessages.RemoveCommandDescription)
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
            symbol.ErrorMessage = AppMessages.RemoveCommandInvalidVersion;
        }
    }

    private void Execute(string package, string? version, bool force)
    {
        var semVer = (version != null
            ? SemVersion.Parse(version, SemVersionStyles.Any)
            : _scope.Container.GetLatestVersion(package))
              ?? throw new InteractiveException(string.Format(AppMessages.RemoveCommandNonExistent,
                  package,
                  version ?? "latest"));

        var transaction = new PackageRemovalTransaction([(package, semVer)],
            _scope.Container,
            force);
        transaction.Execute(new ConsoleAgent());
    }
}