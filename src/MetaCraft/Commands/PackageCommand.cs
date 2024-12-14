// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using System.IO.Compression;
using System.Text.Json;
using MetaCraft.Archive;
using MetaCraft.Archive.Json;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Localisation;

namespace MetaCraft.Commands;

internal static class PackageCommand
{
    internal static Command Create()
    {
        var optSkipVerify = new Option<bool>("--skip-verify", 
            Lc.L("If specified, skips verification of package details"));

        var optOutFile = new Option<string?>(["-O", "--out-file"],
            () => null,
            Lc.L("Location to place output file at, defaults to current directory"));
        
        var optOverwriteFile = new Option<bool>("--overwrite-file",
            Lc.L("If specified, overwrites the output file if it exists"));

        var argFolder = new Argument<DirectoryInfo>("pack-directory",
            Lc.L("The directory to pack"));
        argFolder.AddValidator(ArgValidates.ExistingDirectory);

        var command = new Command("package",
            Lc.L("Creates a new package archive"))
        {
            optSkipVerify,
            optOutFile,
            optOverwriteFile,
            argFolder,
        };
        
        command.SetHandler(Execute, optSkipVerify, argFolder, optOutFile, optOverwriteFile);
        return command;
    }

    private static void Execute(bool skipVerify, 
        DirectoryInfo folder, 
        string? outFile,
        bool overwriteFile)
    {
        var realOutFile = outFile ?? Environment.CurrentDirectory;
        if (Directory.Exists(realOutFile))
        {
            var targetFileName = folder.Name.EndsWith(".metacraft")
                ? $"{folder.Name}.zip"
                : $"{folder.Name}.metacraft.zip";
            
            realOutFile = Path.Combine(realOutFile, targetFileName);
        }
        
        var manifestPath = Path.Combine(folder.FullName, PackageArchive.ManifestFileName);
        if (!File.Exists(manifestPath))
        {
            throw new InteractiveException(Lc.L("missing manifest file: {0}", manifestPath));
        }

        var manifest = InspectManifest(manifestPath);

        if (!skipVerify)
        {
            VerifyExports(manifest, folder.FullName);
        }
        
        // Package the final file
        // Check if it already exists
        if (File.Exists(realOutFile) && !overwriteFile)
        {
            throw InteractiveException.FileExists(realOutFile);
        }

        try
        {
            using var outStream = File.Create(realOutFile);
            ZipFile.CreateFromDirectory(folder.FullName, outStream);
        }
        catch (Exception ex)
        {
            throw InteractiveException.FromException(Lc.L("packaging failed"),
                ex);
        }
    }

    private static void VerifyExports(PackageManifest manifest, string folderFullName)
    {
        if (manifest.Runtime is not { ExportAssemblies: not null })
        {
            return;
        }

        var contentDir = Path.Combine(folderFullName, PackageArchive.ContentsDir);
        var error = false;

        foreach (var export in manifest.Runtime.ExportAssemblies)
        {
            var realPath = Path.Combine(contentDir,
                export.Path.Replace('/', Path.DirectorySeparatorChar));
            // ReSharper disable once InvertIf
            if (!File.Exists(realPath))
            {
                GlobalOutput.Error(Lc.L("missing export '{0}' ({1}), expected in '{2}')",
                    export.Name, 
                    export.Version,
                    realPath));
                error = true;
            }
        }

        if (error)
        {
            throw new InteractiveException(Lc.L("export verification failed"));
        }
    }

    private static PackageManifest InspectManifest(string manifestPath)
    {
        PackageManifest? manifest;
        try
        {
            using var manifestFile = File.OpenRead(manifestPath);
            manifest = JsonSerializer.Deserialize(manifestFile, ArchiveJsonContext.Default.PackageManifest);
        }
        catch (JsonException jex)
        {
            throw new InteractiveException(Lc.L("invalid manifest file: {0} (-> {3}, Line {1} at 0x{2})", jex.Message,
                jex.LineNumber?.ToString() ?? "?",
                jex.BytePositionInLine?.ToString("x2") ?? "??",
                jex.Path ?? "???"));
        }

        if (manifest == null)
        {
            throw new InteractiveException(Lc.L("manifest contains only null: {0}", manifestPath));
        }

        return manifest;
    }
}