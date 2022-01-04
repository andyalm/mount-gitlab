using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class GitlabBranch : Item
{
    public GitlabBranch(ItemPath parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject)
    {
        
    }

    public override string ItemName => Property<string>("Name")!;
    public override bool IsContainer => true;
}