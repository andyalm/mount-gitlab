param(
    [Parameter(Position=0, Mandatory=$true)]
    $Directory
)

$ErrorActionPreference='Stop'

$ModuleVersion='0.0.1'
if($env:GITHUB_REF_NAME -and $env:GITHUB_REF_NAME -match '^v(?<Version>\d+\.\d+\.\d+)$') {
    $ModuleVersion = $Matches.Version
}

New-ModuleManifest -Path $(Join-Path $Directory MountGitlab.psd1) `
    -RootModule 'MountGitlab.dll' `
    -ModuleVersion $ModuleVersion `
    -Guid '5e4bb943-62b6-4cde-9bf7-45fea047ce11' `
    -Author 'Andy Alm' `
    -Copyright '(c) 2021 Andy Alm. All rights reserved.' `
    -Description 'An experimental powershell provider that allows you to explore gitlab as a filesystem.' `
    -PowerShellVersion '7.2' `
    -FormatsToProcess @('Formats.ps1xml') `
    -FunctionsToExport @() `
    -VariablesToExport @() `
    -CmdletsToExport @() `
    -AliasesToExport @()