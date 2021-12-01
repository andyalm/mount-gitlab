using System.Management.Automation;
using System.Management.Automation.Provider;
using MountGitlab.Models;
using MountGitlab.PathHandlers;

namespace MountGitlab;
[CmdletProvider("MountGitlab", ProviderCapabilities.Filter)]
public class MountGitlabProvider : NavigationCmdletProvider, IPathHandlerContext, IContentCmdletProvider
{
    private static readonly Cache _cache = new();

    public Cache Cache => _cache;

    protected override string MakePath(string parent, string child)
    {
        var returnValue = base.MakePath(parent, child);
        //WriteDebug($"{returnValue} MakePath({parent},{child})");

        return returnValue;
    }

    protected override string GetParentPath(string path, string root)
    {
        var returnValue = base.GetParentPath(path, root);
        //WriteDebug($"{returnValue} GetParentPath({path}, {root})");

        return returnValue;
    }

    protected override string[] ExpandPath(string path)
    {
        WriteDebug($"ExpandPath({path})");
        return base.ExpandPath(path);
    }

    protected override bool IsValidPath(string path)
    {
        WriteDebug($"IsValidPath({path})");
        return true;
    }

    protected override bool ItemExists(string path)
    {
        var handler = GetPathHandler(path);
        var returnValue = handler.Exists();
        WriteDebug($"{returnValue} {handler.GetType().Name}.ItemExists({handler.Path})");

        return returnValue;
    }
    
    protected override void GetItem(string path)
    {
        WriteDebug($"GetItem({path})");
        var item = GetPathHandler(path).GetItem();
        if (item != null)
        {
            WriteGitlabObject(item);
        }
    }

    protected override bool HasChildItems(string path)
    {
        WriteDebug($"HasChildItems({path})");
        return GetPathHandler(path).GetChildItems(false).Any();
    }

    protected override bool IsItemContainer(string path)
    {
        WriteDebug($"IsItemContainer({path})");
        return GetPathHandler(path).GetItem()?.IsContainer ?? false;
    }

    protected override string NormalizeRelativePath(string path, string basePath)
    {
        var returnValue = base.NormalizeRelativePath(path, basePath);
        //WriteDebug($"{returnValue} NormalizeRelativePath({path}, {basePath})");

        return returnValue;
    }

    protected override void GetChildItems(string path, bool recurse, uint depth)
    {
        GetChildItems(path, recurse);
    }

    protected override void GetChildItems(string path, bool recurse)
    {
        WriteDebug($"GetChildItems({path}, {recurse})");
        try
        {
            var pathHandler = GetPathHandler(path);
            WriteDebug($"{pathHandler.GetType().Name}.GetChildItems({path})");
            WriteGitlabObjects(pathHandler.GetChildItems(recurse));
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
        }
    }

    private void WriteGitlabObjects<T>(IEnumerable<T> gitlabObjects) where T : GitlabObject
    {
        foreach (var gitlabObject in gitlabObjects)
        {
            WriteGitlabObject(gitlabObject);
        }
    }
    
    private void WriteGitlabObject<T>(T gitlabObject) where T : GitlabObject
    {
        WriteDebug($"WriteItemObject<{gitlabObject.UnderlyingObject.TypeNames.First()}>(,{gitlabObject.FullPath},{gitlabObject.IsContainer})");
        var providerPath =
            $"{Path.DirectorySeparatorChar}{gitlabObject.FullPath.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString())}";
        WriteItemObject(gitlabObject.UnderlyingObject, providerPath, gitlabObject.IsContainer);
    }

    private IPathHandler GetPathHandler(string path)
    {
        try
        {
            path = ToNormalizedGitlabPath(path);
            if (path.StartsWith("/"))
            {
                throw new InvalidOperationException($"Path '{path}' is invalid. It should not start with a /");
            }

            if (string.IsNullOrEmpty(path))
            {
                return new RootPathHandler(path, this);
            }

            if (path.EndsWith("branches"))
            {
                return new BranchesPathHandler(path, this);
            }

            if (path.EndsWith("merge-requests"))
            {
                return new MergeRequestsPathHandler(path, this);
            }

            if (path.EndsWith("pipelines"))
            {
                return new PipelinesPathHandler(path, this);
            }

            if (MergeRequestPathHandler.Matches(path))
            {
                return new MergeRequestPathHandler(path, this);
            }

            if (PipelinePathHandler.Matches(path))
            {
                return new PipelinePathHandler(path, this);
            }
            
            if (BranchPathHandler.Matches(path))
            {
                return new BranchPathHandler(path, this);
            }

            if (FilesPathHandler.Matches(path))
            {
                return new FilesPathHandler(path, this);
            }
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
        }
        
        return new GroupOrProjectPathHandler(path, this);
    }

    private string ToNormalizedGitlabPath(string path)
    {
        var normalizedPath = path.Replace(@"\", "/");
        if (normalizedPath.StartsWith("/"))
        {
            return normalizedPath.Substring(1);
        }

        return normalizedPath;
    }

    public void ClearContent(string path)
    {
        throw new NotSupportedException();
    }

    public object ClearContentDynamicParameters(string path)
    {
        throw new NotSupportedException();
    }

    public IContentReader GetContentReader(string path)
    {
        var pathHandler = GetPathHandler(path);
        if (pathHandler is ISupportContentReader contentReadHandler)
        {
            return contentReadHandler.GetContentReader();
        }

        throw new InvalidOperationException("This item does not support reading content");
    }

    public object GetContentReaderDynamicParameters(string path)
    {
        return null;
    }

    public IContentWriter GetContentWriter(string path)
    {
        throw new NotSupportedException();
    }

    public object GetContentWriterDynamicParameters(string path)
    {
        throw new NotSupportedException();
    }
}
