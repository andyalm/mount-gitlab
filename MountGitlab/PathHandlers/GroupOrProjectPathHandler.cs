using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class GroupOrProjectPathHandler : PathHandler
{
    public GroupOrProjectPathHandler(string path, IPathHandlerContext context) : base(path, context) { }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        var group = Context.GetGroups( "-GroupId", Path)
            .FirstOrDefault();

        if (group != null)
        {
            return group;
        }

        return GetProjectImpl();
    }

    public GitlabProject? GetProject()
    {
        if (Cache.TryGetItem(Path, out var item))
        {
            return item as GitlabProject;
        }

        return GetProjectImpl();
    }

    private GitlabProject? GetProjectImpl()
    {
        return Context.GetProjects("-ProjectId", Path).FirstOrDefault();
    }

    protected override IEnumerable<GitlabObject> GetChildItemsImpl(bool recurse)
    {
        var item = GetItem();
        WriteDebug($"GetChildItemsImpl<{item?.GetType().Name}>");
        return item switch
        {
            GitlabGroup => GetGroupChildren(recurse),
            GitlabProject => GetProjectChildren(recurse),
            _ => Enumerable.Empty<GitlabObject>()
        };
    }

    public override IEnumerable<GitlabObject> NormalizeChildItems(IEnumerable<GitlabObject> items)
    {
        var itemsByType = items.GroupBy(i => i.GetType());
        if (itemsByType.Count() > 1)
        {
            return items.Select(i => new GenericGitlabObject(i));
        }

        return items;
    }

    private IEnumerable<GitlabObject> GetGroupChildren(bool recurse)
    {
        return Context.GetGroups("-ParentGroupId", Path)
            .Cast<GitlabObject>()
            .Concat(Context.GetProjects("-GroupId", Path));
    }

    private IEnumerable<GitlabObject> GetProjectChildren(bool recurse)
    {
        yield return new ProjectSection(Path, "branches");
        yield return new ProjectSection(Path, "files");
        yield return new ProjectSection(Path, "merge-requests");
        yield return new ProjectSection(Path, "pipelines");
    }
}