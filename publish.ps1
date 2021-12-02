$ErrorActionPreference='Stop'

Publish-Module -Path ./MountGitlab/bin/Release/net6.0/MountGitlab.psd1 -NuGetApiKey $env:NuGetApiKey