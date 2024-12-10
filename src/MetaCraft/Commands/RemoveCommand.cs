// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using System.CommandLine.Parsing;
using MetaCraft.Core.Dependency;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using Semver;

namespace MetaCraft.Commands;

public class RemoveCommand
{
    private readonly IPackageScope _scope;
    private readonly IDependencyChecker _dependencyChecker;

    public RemoveCommand(IPackageScope scope, IDependencyChecker dependencyChecker)
    {
        _scope = scope;
        _dependencyChecker = dependencyChecker;
    }

    public Command Create()
    {
        // -- ARGUMENTS --
        var argPackage = new Argument<string>(@"package", Lc.L("The package to remove"));
        
        var argVersion = new Argument<string?>("version", () => null, Lc.L("The version to remove"));
        argVersion.AddValidator(ValidateVersion);

        var optForce = new Option<bool>(["-F", "--force"],
            Lc.L("If specified, ignore dependent packages"));
        var optIgnoreScriptFail = new Option<bool>("--ignore-script-failure", 
            Lc.L("If specified, ignore pre-removal script failures"));
        
        // -- COMMAND --
        var command = new Command("remove", Lc.L("Uninstalls a package from the current scope"))
        {
            argPackage,
            argVersion,
            optForce,
            optIgnoreScriptFail
        };
        
        command.SetHandler(Execute, argPackage, argVersion, optForce, optIgnoreScriptFail);
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

    private void Execute(string package, string? version, bool force, bool ignoreScriptFail)
    {
        var semVer = (version != null
            ? SemVersion.Parse(version, SemVersionStyles.Any)
            : _scope.Container.GetLatestVersion(package))
              ?? throw new InteractiveException(Lc.L("no such package: '{0}' ({1})",
                  package,
                  version ?? "latest"));
        
        if (!force)
        {
            var manifest = _scope.Container.InspectLocal(package, semVer);
            if (_dependencyChecker.HasDependents(manifest!))
            {
                // Fail here, can be bypassed by --force
                throw new InteractiveException(Lc.L("package '{0}' ({1}) is currently being depended on",
                    package,
                    semVer));
            }
        }

        var child = new PackageRemovalTransaction(_scope,
            new PackageRemovalTransaction.Parameters
            {
                Id = package,
                Version = semVer,
                IgnoreScriptFailure = ignoreScriptFail
            });
        var updateTask = new UpdateReferrersTransaction(_scope, new UpdateReferrersTransaction.Parameters(false));

        var transaction = new FinalActionTransaction(_scope, child, updateTask);
        transaction.Commit(new ConsoleAgent());
    }
}
