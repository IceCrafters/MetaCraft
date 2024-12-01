# SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
# SPDX-License-Identifier: GPL-3.0-or-later

$ProjectLanguages = @('zh-CN')
$ProjectModules = @('frontend','core')

function Test-CommandExists {
    param (
        [Parameter(Mandatory = $true)]
        [string]
        $Command
    )
    
    $oldPreference = $ErrorActionPreference
    $ErrorActionPreference = 'stop'

    try {
        if (Get-Command $Command)
        {
            return $true
        }
    }
    catch {
        return $false
    }
    finally {
        $ErrorActionPreference = $oldPreference
    }
}

#endregion

<#
.SYNOPSIS
Determines whether the specified command exists. If not, shows a message to tell users
to download gettext.

.PARAMETER CommandName
The command to check.

.EXAMPLE
Test-IconVCommand msgfmt
#>
function Test-IconVCommand {
    param (
        [Parameter(Mandatory = $true)]
        [string]
        $CommandName
    )

    if (!(Test-CommandExists $CommandName)) {
        Write-Warning "$CommandName not found. You'll need GNU Gettext to build the 'mo' files."
    
        if ($IsWindows) {
            Write-Information 'To get Windows binaries, follow this link:'
            Write-Information 'https://mlocati.github.io/articles/gettext-iconv-windows.html'
        }
    
        if ($IsLinux) {
            Write-Information 'It may be available through your package manager.'
        }
    
        if ($IsMacOS) {
            Write-Information 'Gettext is available through homebrew. To install it, type:'
            Write-Information 'brew install gettext'
        }
    
        return $false
    }

    return $true
}

Export-ModuleMember -Function "Test-IconVCommand"
Export-ModuleMember -Variable "ProjectModules"
Export-ModuleMember -Variable "ProjectLanguages"
