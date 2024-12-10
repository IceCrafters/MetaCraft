// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Globalization;
using System.Net.Mail;
using MetaCraft.Archive.References;
using MetaCraft.Core.Archive;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace MetaCraft.Commands;

internal static class InspectCommand
{
    private static readonly Argument<FileInfo> ArgFile = new(@"file");
    private static readonly Text Colon = new(@":");
    
    internal static Command Create()
    {
        var command = new Command("inspect", Lc.L("Reads a package and shows its information"));
        command.AddAlias(@"info");
        
        command.AddArgument(ArgFile);
        ArgFile.AddValidator(ValidateFileArgument);

        command.SetHandler(Execute, ArgFile);
        return command;
    }

    private static void Execute(FileInfo file)
    {
        var manifest = PackageArchive.Inspect(file.FullName) 
            ?? throw new InteractiveException(Lc.L("not a package: {0}", file.Name));

        var grid = new Grid();
        grid.AddColumn(new GridColumn()
            .Padding(0, 0, 1, 0));
        grid.AddColumn(new GridColumn()
            .Padding(0, 0));
        grid.AddColumn(new GridColumn()
            .Padding(1, 0, 0, 0));
        
        grid.ColonRow(Lc.L("Package ID"), manifest.Id)
            .ColonRow(Lc.L("Package Time"), manifest.PackageTime.ToLocalTime().ToString(CultureInfo.CurrentUICulture))
            .ColonRow(Lc.L("Version"), manifest.Version.ToString())
            .ColonRow(Lc.L("Platform"), manifest.Platform.ToString())
            .ColonRow(Lc.L("File Size"), file.Length.ToString(CultureInfo.InvariantCulture));

        grid.ColonRow(Lc.L("Dependencies"), RenderRefList(manifest.Dependencies))
            .ColonRow(Lc.L("Conflicts with"), RenderRefList(manifest.ConflictsWith))
            .ColonRow(Lc.L("Provides"), RenderRefList(manifest.Provides));
        
        if (manifest.Label != null)
        {
            grid.ColonRow(Lc.L("Description"), manifest.Label.Description);
            grid.ColonRow(Lc.L("Licence"), manifest.Label.License);
            grid.ColonRow(Lc.L("Authors"), RenderUserList(manifest.Label.Authors));
            grid.ColonRow(Lc.L("Maintainers"), RenderUserList(manifest.Label.Maintainers));
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
    
    private static IRenderable RenderRefList(RangedPackageReferenceDictionary? collection)
    {
        if (collection == null)
        {
            return Text.Empty;
        }

        var list = new List<Text>(collection.Count);
        foreach (var (id, range) in collection)
        {
            var rangeStr = range.ToString();
            if (rangeStr == @"*")
            {
                rangeStr = @"=*";
            }
            
            list.Add(new Text($@"{id}{rangeStr}"));
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
            list.Add(new Text($@"{id}={version}"));
        }
        
        return new Rows(list);
    }
    
    private static void ValidateFileArgument(ArgumentResult symbol)
    {
        var info = symbol.GetValueForArgument(ArgFile);
        if (!info.Exists)
        {
            symbol.ErrorMessage = Lc.L("file not found: {1}", info.Name);
        }
    }

    private static Grid ColonRow(this Grid grid, string key, string? value)
    {
        grid.AddRow(key, @":", value ?? @"");
        return grid;
    }
    
    private static Grid ColonRow(this Grid grid, string key, IRenderable? value)
    {
        grid.AddRow(new Text(key), Colon, value ?? Text.Empty);
        return grid;
    }
}
