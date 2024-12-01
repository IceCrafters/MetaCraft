#!/usr/bin/env pwsh
$projBase = Split-Path $PSScriptRoot -Parent

## FRONTEND

$frontendProj = Join-Path $projBase 'src' 'MetaCraft' 'MetaCraft.csproj'
$frontendFile = Join-Path $PSScriptRoot "templates" "frontend.pot"

GetText.Extractor --source $frontendProj --target $frontendFile -as L

## CORE
$coreName = 'MetaCraft.Core'
$coreProj = Join-Path $projBase 'src' $coreName "${coreName}.csproj"
$coreFile = Join-Path $PSScriptRoot 'templates' 'core.pot'

GetText.Extractor --source $coreProj --target $coreFile -as L
