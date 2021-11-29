using System.Management.Automation;
using System.Management.Automation.Provider;
using MountGitlab.Models;
using MountGitlab.PathHandlers;

namespace MountGitlab;
[CmdletProvider("MountGitlab", ProviderCapabilities.Filter)]
public class MountGitlabProvider : NavigationCmdletProvider, IPathHandlerContext
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
        WriteDebug($"ItemExists({path})");
        return GetPathHandler(path).Exists();
    }
    
    protected override void GetItem(string path)
    {
        WriteDebug($"GetItem({path})");
        WriteGitlabObject(GetPathHandler(path).GetItem());
    }

    protected override bool HasChildItems(string path)
    {
        WriteDebug($"HasChildItems({path})");
        return base.HasChildItems(path);
    }

    protected override bool IsItemContainer(string path)
    {
        WriteDebug($"IsItemContainer({path})");
        return true;
    }

    protected override string NormalizeRelativePath(string path, string basePath)
    {
        var returnValue = base.NormalizeRelativePath(path, basePath);
        //WriteDebug($"{returnValue} NormalizeRelativePath({path}, {basePath})");

        return returnValue;
    }

    protected override void GetChildItems(string path, bool recurse, uint depth)
    {
        WriteDebug($"GetChildItems({path}, {recurse}, {depth})");
        GetChildItems(path, recurse);
    }

    protected override void GetChildItems(string path, bool recurse)
    {
        WriteDebug($"GetChildItems({path}, {recurse})");

        WriteGitlabObjects(GetPathHandler(path).GetChildItems(recurse));
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
        WriteItemObject(gitlabObject.UnderlyingObject, gitlabObject.FullPath, gitlabObject.IsContainer);
    }

    private IPathHandler GetPathHandler(string path)
    {
        path = ToNormalizedGitlabPath(path);
        if (string.IsNullOrEmpty(path))
        {
            return new RootPathHandler(path, this);
        }

        if(_cache.TryGetItem(path, out var cachedObject))
        {
            return cachedObject switch
            {
                GitlabProject => new ProjectPathHandler(path, this),
                GitlabGroup => new GroupPathHandler(path, this),
                _ => throw new ArgumentOutOfRangeException(nameof(cachedObject))
            };
        }
        else
        {
            return new GroupOrProjectPathHandler(path, this);
        }
    }

    private string ToNormalizedGitlabPath(string path)
    {
        var normalizedPath = path.Replace(@"\", "/");
        if (normalizedPath.StartsWith("/"))
        {
            return normalizedPath.Substring(1);
        }
        else
        {
            return normalizedPath;
        }
    }
}
