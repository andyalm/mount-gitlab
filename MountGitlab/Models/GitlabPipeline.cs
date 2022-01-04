using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class GitlabPipeline : Item
{
    public GitlabPipeline(ItemPath parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject)
    {
        
    }

    public override string ItemName => Property<string>("Id")!;
    public override bool IsContainer => true;
    public string Ref => Property<string>("Ref")!;
}