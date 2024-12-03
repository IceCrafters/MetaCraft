#!/usr/bin/env pwsh

# SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
# SPDX-License-Identifier: GPL-3.0-or-later

$buildScript = Join-Path $PSScriptRoot 'build.ps1'
& $buildScript -t RegenerateLanguage
$realExitCode = $LASTEXITCODE;

Write-Output "Using this configure script is deprecated. Please use"
Write-Output "    build.ps1 -t RegenerateLanguage"
Write-Output "for regenerating language files. Thank you."

exit $realExitCode;
