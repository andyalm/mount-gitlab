namespace MountGitlab.PathHandlers;

public class GroupOrProjectPathHandler : PathHandler
{
    private readonly GroupPathHandler _groupHandler;
    private readonly ProjectPathHandler _projectHandler;
    
    public GroupOrProjectPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        _groupHandler = new GroupPathHandler(path, context);
        _projectHandler = new ProjectPathHandler(path, context);
    }

    public override bool Exists()
    {
        return TryGetObject(out _);
    }

    public override GitlabObject GetItem()
    {
        if (TryGetObject(out var gitlabObject))
        {
            return gitlabObject;
        }
        else
        {
            throw new ArgumentException($"No group or project is accessible at {Path}");
        }
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return _groupHandler.GetChildItems(recurse).Concat(_projectHandler.GetChildItems(recurse));
    }

    private bool TryGetObject(out GitlabObject gitlabObject)
    {
        if (_groupHandler.TryGetGroup(out var group))
        {
            gitlabObject = group;
            return true;
        }
        
        if(_projectHandler.TryGetProject(out var project))
        {
            gitlabObject = project;
            return true;
        }

        gitlabObject = default!;
        return false;
    }

    
}