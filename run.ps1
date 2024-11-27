#!/usr/bin/env pwsh

# SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
# SPDX-License-Identifier: GPL-3.0-or-later

param (
    # The arguments to pass to the program
    [Parameter(Mandatory = $true)]
    [array]
    $Arguments
)

# Build stuff as necessary
$configureScript = Join-Path $PSScriptRoot 'configure.ps1'
& $configureScript

# Create directories
$runBase = Join-Path $PSScriptRoot "run.d"
$runScope = Join-Path $PSScriptRoot "run.d" "scope"

New-Item -ItemType Directory -Path $runBase -ErrorAction Ignore | Out-Null
New-Item -ItemType Directory -Path $runScope -ErrorAction Ignore | Out-Null

# Configure environment variables
$env:METACRAFT_SCOPE = $runScope
$env:METACRAFT_DRY_ENV = 1
$env:METACRAFT_CONFIG_HOME = Join-Path $runBase "config"
$env:METACRAFT_CACHE_HOME = Join-Path $runBase "caches"

# Run!
$PROJNAME = 'MetaCraft'
$projLocation = Join-Path $PSScriptRoot "src" $PROJNAME "$PROJNAME.csproj"

dotnet run --project $projLocation --no-self-contained -- @Arguments