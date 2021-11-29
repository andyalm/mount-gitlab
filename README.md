# Mount Gitlab

An experimental powershell provider that allows you to explore gitlab as a filesystem.
It integrates with/depends on [pwsh-gitlab](https://github.com/chris-peterson/pwsh-gitlab) for the gitlab interaction.

## Usage

```powershell
New-PSDrive -Name gitlab -PSProvider MountGitlab -Root /
cd gitlab:
ls #will list all gitlab groups in the project
cd mygroup
ls #will list all projects and subgroups within the mygroup group
```
