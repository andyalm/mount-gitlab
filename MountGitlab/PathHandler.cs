using System.Management.Automation;

namespace MountGitlab;

public abstract class PathHandler : IPathHandler
{
    protected PathHandler(string path, IPathHandlerContext context)
    {
        Path = path;
        Context = context;
    }
    
    protected string Path { get; }
    protected IPathHandlerContext Context { get; }

    protected Cache Cache => Context.Cache;
    protected CommandInvocationIntrinsics InvokeCommand => Context.InvokeCommand;
    protected void WriteDebug(string message) => Context.WriteDebug(message);

    public abstract bool Exists();
    public abstract GitlabObject GetItem();
    public abstract IEnumerable<GitlabObject> GetChildItems(bool recurse);
}