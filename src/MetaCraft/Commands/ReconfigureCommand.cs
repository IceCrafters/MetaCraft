// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Configuration;
using MetaCraft.Localisation;
using Semver;

namespace MetaCraft.Commands;

public class ReconfigureCommand
{
    private readonly IPackageScope _scope;

    public ReconfigureCommand(IPackageScope scope)
    {
        _scope = scope;
    }

    public Command Create()
    {
        var argPackage = new Argument<string>("package", Lc.L("The package to reconfigure"));
        var argVersion = new Argument<string?>("version", () => null, Lc.L("The version to reconfigure"));
        argVersion.AddValidator(ArgValidates.Version);

        var command = new Command("reconfigure")
        {
            argPackage,
            argVersion
        };

        command.SetHandler(Execute, argPackage, argVersion);

        return command;
    }

    private void Execute(string packageName, string? packageVersion)
    {
        var version = packageVersion != null
            ? SemVersion.Parse(packageVersion)
            : _scope.Container.GetLatestVersion(packageName);
        // If still null
        if (version == null)
        {
            throw InteractiveException.CreateNoValidVersionFound(packageName);
        }
        
        var location = _scope.Container.GetInstalledPackageLocationOrDefault(packageName,
            version);
        if (location == null)
        {
            throw InteractiveException.CreateNoSuchClause(packageName, version);
        }
        
        var manifest = _scope.Container.InspectLocal(packageName, version)!;
        
        PackageConfigApplier.Apply(_scope, manifest, location, new ConsoleAgent());
    }
}