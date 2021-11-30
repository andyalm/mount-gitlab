namespace MountGitlab.PathHandlers;

public class RootPathHandler : PathHandler
{
    public RootPathHandler(string path, IPathHandlerContext context) : base(path, context) {}

    protected override bool ExistsImpl() => true;

    protected override GitlabObject GetItemImpl()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Context.GetGroups();
    }
}