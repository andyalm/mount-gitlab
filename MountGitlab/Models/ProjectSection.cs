using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class ProjectSection : Item
{
    public ProjectSection(ItemPath projectPath, string sectionName) : base(projectPath, new PSObject())
    {
        ItemName = sectionName;
    }

    public override string ItemName { get; }
    protected override string TypeName => "MountGitlab.ProjectSection";
    public override bool IsContainer => true;
}