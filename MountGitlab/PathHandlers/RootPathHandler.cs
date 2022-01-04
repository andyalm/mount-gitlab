using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class RootPathHandler : PathHandler
{
    public RootPathHandler(ItemPath path, IPathHandlerContext context) : base(path, context) {}

    protected override IItem GetItemImpl()
    {
        return new GitlabRoot();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Context.GetGroups();
    }
}