param(
    [switch]
    $Debug
)

$DebugPreference=$Debug ? 'Continue' : 'SilentlyContinue'
$env:NO_MOUNT_GITLAB='1'
dotnet build
pwsh -Interactive -NoExit -c "Set-Variable -Scope Global -Name DebugPreference -Value $DebugPreference;Import-Module $PWD/MountGitlab/bin/Debug/net6.0/Module/MountGitlab.psd1 && cd gitlab:"
