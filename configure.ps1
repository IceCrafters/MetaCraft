#!/usr/bin/env pwsh

# SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
# SPDX-License-Identifier: GPL-3.0-or-later

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$pss = [System.IO.Path]::DirectorySeparatorChar
$i10nDir = "${projectRoot}${pss}i10n"

# Import module
Import-Module -Name "${i10nDir}${pss}checkers.psm1"

# Get our current location
$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Check if i10n file exists
foreach ($langId in $ProjectLanguages) {
    $langBase = "${i10nDir}${pss}${langId}"

    # For each module, check language file is alright
    foreach ($moduleId in $ProjectModules) {
        $langPath = "${langBase}${pss}${moduleId}.mo"
        $poFile = Get-Item "${langBase}${pss}${moduleId}.po"

        if (!($poFile.Exists)) {
            continue
        }

        if (!(Test-Path $langPath -PathType Leaf) -or ($langFile.LastWriteTime -lt $poFile.LastWriteTime)) {
            # Rebuild the language file
            Write-Output "Recompiling language file $(($poFile.Name))"
            msgfmt $poFile.FullName  -o $langPath
        }
    }
}

exit 0