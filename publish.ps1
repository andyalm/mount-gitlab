$ErrorActionPreference='Stop'

Import-Module ./MountGitlab/bin/Release/net6.0/MountGitlab.psd1
Publish-Module -Name MountGitlab -NuGetApiKey $env:NuGetApiKey