# Mount Gitlab

An experimental powershell provider that allows you to explore gitlab as a filesystem.
It integrates with/depends on [pwsh-gitlab](https://github.com/chris-peterson/pwsh-gitlab) for the gitlab interaction.

## Installation

1. Install the required powershell modules:

```powershell
# Ensure you have installed the dependent module GitlabCli
Install-Module -Name GitlabCli

# Then install MountGitlab
Install-Module -Name MountGitlab
```

2. Configure your gitlab connection by following the `GitlabCli` [instructions here](https://github.com/chris-peterson/pwsh-gitlab#configuration).

3. Finally, import the module and mount gitlab to a PSDrive with something like this (consider adding this to your profile):

```powershell
Import-Module MountGitlab
```
## Usage

```powershell
# navigate into the drive letter of your gitlab PSDrive
cd gitlab:

# list all gitlab groups in the project
dir

# list all of the projects and subgroups within a group
cd mygroup
dir

# list top level files and directories within the default branch of the project
cd myproject
dir files

# list the branches in the project
dir branches

# list the pipelines in the project
dir pipelines

# navigate into the main branch
cd branches/mybranch

# list the pipelines associated with the current branch
dir pipelines

# list the top level files and directories within the current branch
dir files

# list the jobs within a pipeline
dir pipelines/12345

# print out the job trace for a specific job
gc pipelines/12345/build
```
