using System.Management.Automation;

namespace MountGitlab.Models;

public class ProjectSection : GitlabObject
{
    public ProjectSection(string projectPath, string sectionName) : base("MountGitlab.ProjectSection",new
    {
        Name = sectionName,
        ProjectPath = projectPath,
        FullPath = $"{projectPath}/{sectionName}"
    }) {}

    public override string Name => Property<string>("Name");
    public override string FullPath => Property<string>("FullPath");
    public override bool IsContainer => true;
}