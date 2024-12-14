// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.CommandLine;
using System.CommandLine.Parsing;
using MetaCraft.Localisation;
using Semver;

namespace MetaCraft.Commands;

internal static class ArgValidates
{
    internal static void ExistingDirectory(ArgumentResult symbol)
    {
        var value = symbol.GetValueOrDefault<DirectoryInfo?>();
        if (value == null)
        {
            return;
        }

        if (!value.Exists)
        {
            symbol.ErrorMessage = Lc.L("directory not found: {0}", value);
        }
    }
    
    internal static void Version(ArgumentResult symbol)
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
}