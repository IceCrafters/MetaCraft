#!/usr/bin/env pwsh

# SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
# SPDX-License-Identifier: GPL-3.0-or-later

$pss = [System.IO.Path]::DirectorySeparatorChar
$i10nDir = [System.Environment]::CurrentDirectory
$templateDir = "${i10nDir}${pss}templates"

Import-Module -Name "./checkers.psm1"

if (!(Test-IconVCommand "msgmerge")) {
    exit 1
}

foreach ($moduleId in $ProjectModules) {
    $templateFile = Get-Item "${templateDir}${pss}${moduleId}.pot"

    foreach ($langId in $ProjectLanguages) {
        $langBase = "${i10nDir}${pss}${langId}"
        $poPath = "${langBase}${pss}${moduleId}.po"

        if (!(Test-Path $poPath -PathType Leaf)) {
            # Copy the old file. No merge necessary
            Copy-Item -Path $templateFile -Destination $poPath
        } else {
            # Invoke msgmerge!
            msgmerge -U $poPath $templateFile.FullName
        }
    }
}

exit 0
