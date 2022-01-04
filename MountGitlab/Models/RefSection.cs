using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class RefSection : Item
{
    public RefSection(ItemPath projectPath, string refName, string sectionName) : base(
        projectPath.Combine("branches", refName), new PSObject(new
        {
            ProjectPath = projectPath,
            Ref = refName,
        }))
    {
        ItemName = sectionName;
    }

    public override string ItemName { get; }
    protected override string TypeName => "MountGitlab.RefSection";
    public override bool IsContainer => true;
}