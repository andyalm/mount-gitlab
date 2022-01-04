using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class GitlabRoot : Item
{
    public GitlabRoot() : base(ItemPath.Root, new PSObject()) { }

    public override string ItemName => string.Empty;
    public override bool IsContainer => true;
}