using System.Management.Automation;

namespace MountGitlab.Models;

public class Branch : GitlabObject
{
    public string ProjectPath { get; }
    
    public Branch(string projectPath, PSObject underlyingObject) : base(underlyingObject)
    {
        ProjectPath = projectPath;
    }

    public override string Name => Property<string>("Name");
    public override string FullPath => $"{ProjectPath}/branches/{Name}";
    public override bool IsContainer => true;
}