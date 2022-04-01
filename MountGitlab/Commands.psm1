function Enter-MountGitlab {
    Push-Location $PWD -StackName MountGitlab
    $ProjectPath = Get-GitlabProject | Select -Expand PathWithNamespace
    Set-Location "gitlab:/$ProjectPath"
}

function Exit-MountGitlab {
    Pop-Location -StackName MountGitlab
}