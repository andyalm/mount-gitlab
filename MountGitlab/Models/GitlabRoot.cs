using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabRoot : GitlabObject
{
    public GitlabRoot() : base(new PSObject(new
    {
        Name = "gitlab"
    })) { }

    public override string Name => string.Empty;
    public override string FullPath => string.Empty;
    public override bool IsContainer => true;
}