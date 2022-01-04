using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class GroupOrProjectItem : Item
{
    public static GroupOrProjectItem ForProject(PSObject project)
    {
        var parentPath = new ItemPath(project.Property<string>("PathWithNamespace")!).Parent;
        return new GroupOrProjectItem(parentPath, project, "Name");
    }

    public static GroupOrProjectItem ForGroup(PSObject group)
    {
        var parentPath = new ItemPath(group.Property<string>("FullPath")!).Parent;
        return new GroupOrProjectItem(parentPath, group, "Path");
    }
    
    private GroupOrProjectItem(ItemPath parentPath, PSObject underlyingObject, string itemNameProperty) : base(parentPath, underlyingObject)
    {
        ItemName = Property<string>(itemNameProperty)!;
    }

    public bool IsProject => Property<string>("ProjectId") != null;
    protected override string TypeName => "MountGitlab.GroupOrProjectItem";
    public override string ItemName { get; }
    public override bool IsContainer => true;
}