using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class GitlabMergeRequest : Item
{
    public GitlabMergeRequest(ItemPath parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject)
    {
    }

    public override string ItemName => Iid.ToString();
    public override bool IsContainer => false;
    public long Iid => Property<long>("Iid");
}