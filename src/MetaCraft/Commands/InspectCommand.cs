// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Globalization;
using System.Net.Mail;
using MetaCraft.Core.Archive;
using MetaCraft.Locales;
using MetaCraft.Locales;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace MetaCraft.Commands;

internal static class InspectCommand
{
    private static readonly Argument<FileInfo> ArgFile = new("file");
    private static readonly Text Colon = new(":");
    
    internal static Command Create()
    {
        var command = new Command("inspect", AppMessages.InspectCommandDescription);
        command.AddAlias("info");
        
        command.AddArgument(ArgFile);
        ArgFile.AddValidator(ValidateFileArgument);

        command.SetHandler(Execute, ArgFile);
        return command;
    }

    private static void Execute(FileInfo file)
    {
        var manifest = PackageArchive.Inspect(file.FullName);
        if (manifest == null)
        {
            throw new InvalidOperationException(AppMessages.InspectCommandNotAPackage);
        }

        var grid = new Grid();
        grid.AddColumn(new GridColumn()
            .Padding(0, 0, 1, 0));
        grid.AddColumn(new GridColumn()
            .Padding(0, 0));
        grid.AddColumn(new GridColumn()
            .Padding(1, 0, 0, 0));
        
        grid.ColonRow(AppMessages.InspectCommandPackageId, manifest.Id)
            .ColonRow(AppMessages.InspectCommandPackageTime, manifest.PackageTime.ToLocalTime().ToString(CultureInfo.CurrentUICulture))
            .ColonRow(AppMessages.InspectCommandPackageVersion, manifest.Version.ToString())
            .ColonRow(AppMessages.InspectCommandPackagePlatform, manifest.Platform.ToString())
            .ColonRow(AppMessages.InspectCommandPackageSize, file.Length.ToString(CultureInfo.InvariantCulture));

        grid.ColonRow(AppMessages.InspectCommandDependencies, RenderRefList(manifest.Dependencies))
            .ColonRow(AppMessages.InspectCommandConflictsWith, RenderRefList(manifest.ConflictsWith))
            .ColonRow(AppMessages.InspectCommandRefProvides, RenderRefList(manifest.Provides));
        
        if (manifest.Label != null)
        {
            grid.ColonRow(AppMessages.InspectCommandPackageDescription, manifest.Label.Description);
            grid.ColonRow(AppMessages.InspectCommandPackageLicence, manifest.Label.License);
            grid.ColonRow(AppMessages.InspectCommandAuthors, RenderUserList(manifest.Label.Authors));
            grid.ColonRow(AppMessages.InspectCommandMaintainers, RenderUserList(manifest.Label.Maintainers));
        }
        
        AnsiConsole.Write(grid);
    }

    private static IRenderable RenderUserList(MailAddressCollection? collection)
    {
        if (collection == null)
        {
            return Text.Empty;
        }
        
        return new Rows(collection.Select(address => new Text(address.ToString())));
    }
    
    private static IRenderable RenderRefList(ProvisionReferenceDictionary? collection)
    {
        if (collection == null)
        {
            return Text.Empty;
        }

        var list = new List<Text>(collection.Count);
        foreach (var (id, range) in collection)
        {
            var rangeStr = range.ToString();
            if (rangeStr == "*")
            {
                rangeStr = "=*";
            }
            
            list.Add(new Text($"{id}{rangeStr}"));
        }
        
        return new Rows(list);
    }
    
    private static IRenderable RenderRefList(PackageReferenceDictionary? collection)
    {
        if (collection == null)
        {
            return Text.Empty;
        }

        var list = new List<Text>(collection.Count);
        foreach (var (id, version) in collection)
        {
            list.Add(new Text($"{id}={version}"));
        }
        
        return new Rows(list);
    }
    
    private static void ValidateFileArgument(ArgumentResult symbol)
    {
        var info = symbol.GetValueForArgument(ArgFile);
        if (!info.Exists)
        {
            symbol.ErrorMessage = string.Format(AppMessages.InspectCommandFileNotFound, info.Name);
        }
    }

    private static Grid ColonRow(this Grid grid, string key, string? value)
    {
        grid.AddRow(key, ":", value ?? "");
        return grid;
    }
    
    private static Grid ColonRow(this Grid grid, string key, IRenderable? value)
    {
        grid.AddRow(new Text(key), Colon, value ?? Text.Empty);
        return grid;
    }
}