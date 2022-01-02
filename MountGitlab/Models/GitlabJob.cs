using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class GitlabJob : Item
{
    public GitlabJob(ItemPath parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject) { }

    public long Id => Property<long>("Id");

    public override string ItemName => Property<string>("Name")!;

    protected override IEnumerable<string> Aliases
    {
        get
        {
            yield return Id.ToString();
        }
    }
    public string ProjectId => Property<object>("ProjectId")!.ToString()!;
    public override bool IsContainer => false;
}