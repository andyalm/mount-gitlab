using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class GroupOrProjectPathHandler : PathHandler
{
    public GroupOrProjectPathHandler(ItemPath path, IPathHandlerContext context) : base(path, context) { }

    protected override IItem? GetItemImpl()
    {
        var group = Context.GetGroups( "-GroupId", Path.ToString())
            .FirstOrDefault();

        if (group != null)
        {
            return group;
        }

        return GetProjectImpl();
    }

    public GroupOrProjectItem? GetProject()
    {
        if (Cache.TryGetItem(Path, out var item))
        {
            return item.Item as GroupOrProjectItem;
        }

        return GetProjectImpl();
    }

    private GroupOrProjectItem? GetProjectImpl()
    {
        return Context.GetProjects("-ProjectId", Path.ToString()).FirstOrDefault();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var item = GetItem() as GroupOrProjectItem;
        WriteDebug($"GetChildItemsImpl<{item?.GetType().Name}>");
        return item switch
        {
            { IsProject: true } => GetProjectChildren(),
            not null => GetGroupChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetGroupChildren()
    {
        return Context.GetGroups("-ParentGroupId", Path.ToString())
            .Concat(Context.GetProjects("-GroupId", Path.ToString()));
    }

    private IEnumerable<IItem> GetProjectChildren()
    {
        yield return new ProjectSection(Path, "branches");
        yield return new ProjectSection(Path, "files");
        yield return new ProjectSection(Path, "merge-requests");
        yield return new ProjectSection(Path, "pipelines");
    }

    public override Freshness GetItemCommandDefaultFreshness => Freshness.Default;
    public override Freshness GetChildItemsCommandDefaultFreshness => Freshness.Default;
}