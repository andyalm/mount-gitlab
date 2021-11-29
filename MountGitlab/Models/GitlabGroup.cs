using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabGroup : GitlabObject
{
    public GitlabGroup(PSObject underlyingObject) : base(underlyingObject)
    {
    }

    public override string Name => Property<string>("Path");
    public override string FullPath => Property<string>("FullPath");
    public override bool IsContainer => true;
}