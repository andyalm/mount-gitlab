using System.Management.Automation;

namespace MountGitlab;

public abstract class PathHandler : IPathHandler
{
    protected PathHandler(string path, IPathHandlerContext context)
    {
        Path = path;
        Context = context;
    }
    
    public string Path { get; }
    protected IPathHandlerContext Context { get; }

    protected Cache Cache => Context.Cache;
    protected CommandInvocationIntrinsics InvokeCommand => Context.InvokeCommand;
    protected void WriteDebug(string message) => Context.WriteDebug(message);

    protected string ParentPath => System.IO.Path.GetDirectoryName(Path)!.Replace(@"\", "/");
    protected string ItemName => System.IO.Path.GetFileName(Path);

    public bool Exists()
    {
        if (Cache.TryGetItem(Path, out _))
        {
            return true;
        }

        return ExistsImpl();
    }

    public GitlabObject GetItem()
    {
        if (Cache.TryGetItem(Path, out var cachedItem))
        {
            return cachedItem;
        }

        return GetItemImpl()!;
    }

    protected abstract bool ExistsImpl();
    protected abstract GitlabObject? GetItemImpl();
    public abstract IEnumerable<GitlabObject> GetChildItems(bool recurse);
}