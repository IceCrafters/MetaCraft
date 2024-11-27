#!/usr/bin/env pwsh
$projBase = Split-Path $PSScriptRoot -Parent

$frontendProj = Join-Path $projBase 'src' 'MetaCraft' 'MetaCraft.csproj'
$frontendFile = Join-Path $PSScriptRoot "templates" "frontend.pot"

GetText.Extractor --source $frontendProj --target $frontendFile -as L