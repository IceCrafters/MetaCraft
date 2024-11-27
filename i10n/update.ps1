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
        $poFile = Get-Item "${langBase}${pss}${moduleId}.po"

        # Invoke msgmerge!
        msgmerge -U $poFile.FullName $templateFile.FullName
    }
}

exit 0