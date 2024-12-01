// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;
using MetaCraft.Core.Transactions;

namespace MetaCraft.Commands;

internal class InstallCommand
{
    private static readonly Argument<FileInfo[]> ArgFile = new(@"files", Lc.L("The package archives to install"));
    private static readonly Option<bool> OptionOverwrite = new([@"-f", @"--force"], Lc.L("If specified, remove already existing packages of same ID and version"));
    private readonly PackageContainer _container;

    public InstallCommand(PackageContainer container)
    {
        _container = container;
    }

    internal Command Create()
    {
        var command = new Command(@"install", Lc.L("Installs one or more package archives"));
        command.AddArgument(ArgFile);
        command.AddOption(OptionOverwrite);
        command.SetHandler(Execute, ArgFile, OptionOverwrite);

        return command;
    }

    private void Execute(FileInfo[] file, bool force)
    {
        var list = new List<PackageInstallTransaction>(file.Length);

        foreach (var f in file)
        {
            var archive = PackageArchive.Open(f.FullName);
            if (!archive.Manifest.Platform.Covers(PlatformIdentifier.Current))
            {
                throw new InteractiveException(
                    Lc.L($"platform '{archive.Manifest.Id}' ({archive.Manifest.Version}) requires platform {archive.Manifest.Platform} but current is {PlatformIdentifier.Current}"));
            }
            
            // Add new transaction to the list
            list.Add(new PackageInstallTransaction(_container, new PackageInstallTransaction.Parameters()
            {
                Package = archive,
                Overwrite = force
            }));
        }

        // Perform all transactions
        var arguments = new FinalActionAggregateTransaction.Parameter(list, 
            new UpdateReferrersTransaction(_container, new UpdateReferrersTransaction.Parameters(false,
                new NullReferralPreferenceProvider())));

        var aggregate = new FinalActionAggregateTransaction(_container, arguments);
        aggregate.Commit(new ConsoleAgent());
    }
}
