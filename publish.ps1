$ErrorActionPreference='Stop'

if(-not (Get-Module GitlabCli)) {
    Install-Module -Name GitlabCli -Force
}
Publish-Module -Path ./bin/MountGitlab -NuGetApiKey $env:NuGetApiKey