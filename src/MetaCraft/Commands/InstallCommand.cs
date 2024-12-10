// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using MetaCraft.Core;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Dependency;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;

namespace MetaCraft.Commands;

internal class InstallCommand
{
    private static readonly Argument<FileInfo[]> ArgFile = new(@"files", Lc.L("The package archives to install"));
    private static readonly Option<bool> OptionOverwrite = new([@"-f", @"--force"], Lc.L("If specified, remove already existing packages of same ID and version"));
    private static readonly Option<bool> OptionIgnoreDeps = new(["--ignore-deps"], Lc.L("Ignore missing dependencies and conflicting packages"));
    private readonly IPackageScope _scope;

    public InstallCommand(IPackageScope scope)
    {
        _scope = scope;
    }

    internal Command Create()
    {
        var command = new Command(@"install", Lc.L("Installs one or more package archives"));
        command.AddArgument(ArgFile);
        command.AddOption(OptionOverwrite);
        command.AddOption(OptionIgnoreDeps);
        command.SetHandler(Execute, ArgFile, OptionOverwrite, OptionIgnoreDeps);

        return command;
    }

    private void Execute(FileInfo[] file, bool force, bool ignoreDeps)
    {
        var agent = new ConsoleAgent();

        var list = new List<PackageInstallTransaction>(file.Length);
        var packages = new List<PackageManifest>(file.Length);
        var checker = new DependencyChecker(_scope);

        foreach (var f in file)
        {
            var archive = PackageArchive.Open(f.FullName);
            if (!archive.Manifest.Platform.Covers(PlatformIdentifier.Current))
            {
                throw new InteractiveException(
                    Lc.L($"platform '{archive.Manifest.Id}' ({archive.Manifest.Version}) requires platform {archive.Manifest.Platform} but current is {PlatformIdentifier.Current}"));
            }

            packages.Add(archive.Manifest);
            // Add new transaction to the list
            list.Add(new PackageInstallTransaction(_scope, new PackageInstallTransaction.Parameters()
            {
                Package = archive,
                Overwrite = force
            }));
        }

        // Check for dependencies
        if (!checker.DoesDependencySatisfy(packages, agent))
        {
            if (ignoreDeps)
            {
                agent.PrintWarning(Lc.L("Package requirements were not satisified; package may fail to configure."));
                agent.PrintWarning(Lc.L("See the above messages for details."));
            }
            else
            {
                throw new InteractiveException(Lc.L("Package requirements unsatisified; see above for details."));
            }
        }

        // Perform all transactions
        var arguments = new FinalActionAggregateTransaction.Parameter(list, 
            new UpdateReferrersTransaction(_scope, new UpdateReferrersTransaction.Parameters(false)));

        var aggregate = new FinalActionAggregateTransaction(_scope, arguments);

        try
        {
            aggregate.Commit(agent);
        }
        catch (TransactionException te)
        {
            throw InteractiveException.CreateTransactionFailed(te);
        }
    }
}
