using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabProject : GitlabObject
{
    public GitlabProject(PSObject underlyingObject) : base(underlyingObject)
    {
        
    }

    public override string Name => Property<string>("Name");
    public override string FullPath => Property<string>("PathWithNamespace");
    public override bool IsContainer => true;
    public string DefaultBranch => Property<string>("DefaultBranch");
}