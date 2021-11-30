param(
    [switch]
    $Debug
)

$DebugPreference=$Debug ? 'Continue' : 'SilentlyContinue'
dotnet build
pwsh -Interactive -NoExit -c "Set-Variable -Scope Global -Name DebugPreference -Value $DebugPreference;Import-Module $PWD/MountGitlab/bin/Debug/net6.0/MountGitlab.psd1 && New-PSDrive -Name gitlab -PSProvider MountGitlab -Root $([IO.Path]::DirectorySeparatorChar)"
