using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class RootPathHandler : PathHandler
{
    public RootPathHandler(string path, IPathHandlerContext context) : base(path, context) {}

    protected override bool ExistsImpl() => true;

    protected override GitlabObject GetItemImpl()
    {
        return new GitlabRoot();
    }

    protected override IEnumerable<GitlabObject> GetChildItemsImpl(bool recurse)
    {
        return Context.GetGroups();
    }
}