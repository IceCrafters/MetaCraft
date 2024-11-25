// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using MetaCraft.Locales;

namespace MetaCraft.Commands;

internal class InstallCommand
{
    private static readonly Argument<FileInfo[]> ArgFile = new("files", AppMessages.InstallCommandArgumentFile);
    private static readonly Option<bool> OptionOverwrite = new(["-f", "--force"], AppMessages.InstallCommandOptionForce);
    private readonly PackageContainer _container;

    public InstallCommand(PackageContainer container)
    {
        _container = container;
    }

    internal Command Create()
    {
        var command = new Command("install", AppMessages.InstallCommandDescription);
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
                throw new InvalidOperationException(string.Format(AppMessages.InstallCommandPlatformNotSupported,
                    archive.Manifest.Id,
                    archive.Manifest.Version,
                    PlatformIdentifier.Current,
                    archive.Manifest.Platform));
            }
            
            // Add new transaction to the list
            list.Add(new PackageInstallTransaction(_container, new PackageInstallTransaction.Parameters()
            {
                Package = archive,
                Overwrite = force
            }));
        }
        
        // Perform all transactions
        var aggregate = new AggregateTransaction(_container, list);
        aggregate.Commit(new ConsoleAgent());
    }
}
