namespace MountGitlab.Models;

public class RefSection : GitlabObject
{
    public RefSection(string projectPath, string refName, string sectionName) : base("MountGitlab.RefSection",new
    {
        Name = sectionName,
        ProjectPath = projectPath,
        Ref = refName,
        FullPath = $"{projectPath}/branches/{refName}/{sectionName}"
    }) {}

    public override string Name => Property<string>("Name");
    public override string FullPath => Property<string>("FullPath");
    public override bool IsContainer => true;
}