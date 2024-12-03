// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Diagnostics;
using System.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build;

[TaskName("RegenerateLanguage")]
public class RegenerateLanguageTask : FrostingTask<BuildContext>
{
    private static readonly string[] Languages =
    [
        "zh-CN"
    ];

    private static readonly string[] Modules =
    [
        "core",
        "frontend"
    ];
    
    public override void Run(BuildContext context)
    {
        var formatter = LocateMsgFmt();

        foreach (var language in Languages)
        {
            var langBase = Path.Combine(MConstants.I10NPath, language);
            if (!Directory.Exists(langBase))
            {
                context.Log.Warning("Language {0} is defined but does not exist", language);
                continue;
            }
            
            // For each module, check if mo is up to date
            foreach (var module in Modules)
            {
                var translateFile = Path.Combine(langBase, $"{module}.po");
                var machineFile = Path.Combine(langBase, $"{module}.mo");

                if (!File.Exists(translateFile))
                {
                    context.Log.Warning("Module {0} is defined but does not exist", module);
                    continue;
                }

                // Skip if latest
                if (File.Exists(machineFile) &&
                    File.GetLastWriteTime(translateFile) <= File.GetLastWriteTime(machineFile)) continue;
                
                // Regenerate
                context.Log.Information("Regenerating module {0} for language {1}", module, language);
                Process.Start(formatter, 
                [
                    translateFile,
                    "-o",
                    machineFile
                ]);
            }
        }
    }

    private static string LocateMsgFmt()
    {
        var fmtName = OperatingSystem.IsWindows()
            ? "msgfmt.exe"
            : "msgfmt";
        
        var paths = Environment.GetEnvironmentVariable("PATH");
        if (paths == null)
        {
            throw new FileNotFoundException("PATH environment variable is missing.");
        }
        
        foreach (var path in paths.Split(Path.PathSeparator))
        {
            var searchFor = Path.Combine(path, fmtName);
            if (File.Exists(searchFor))
            {
                return searchFor;
            }
        }
        
        throw new FileNotFoundException($"Could not find {fmtName}.");
    }
}