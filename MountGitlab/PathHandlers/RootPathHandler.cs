namespace MountGitlab.PathHandlers;

public class RootPathHandler : PathHandler
{
    public RootPathHandler(string path, IPathHandlerContext context) : base(path, context) {}

    public override bool Exists() => true;

    public override GitlabObject GetItem()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Context.GetGroups();
    }
}